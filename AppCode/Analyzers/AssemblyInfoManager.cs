using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AppCode.Data;
using AppCode.Models;
using ToSic.Sxc.Services.Cache;
using static AppCode.Constants;
using ApiTypeInfo = AppCode.Models.ApiTypeInfo;

namespace AppCode.Analyzers
{
  public class AssemblyInfoManager: Custom.Hybrid.CodeTyped
  {
    #region Get from Cache or Setup

    private ICacheSpecs GetSpecs(string name) =>
      Kit.Cache.CreateSpecs(name)
        .WatchAppData()
        .WatchAppFolder();

    /// <summary>
    /// Get or create, called by the main entry file; needs to know the path.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="path"></param>
    /// <param name="alwaysRecreate"></param>
    /// <returns></returns>
    public ApiAssemblyInfo GetOrCreate(string name, string path, bool alwaysRecreate)
    {
      // First check cached and use that if possible
      var specs = GetSpecs(name);
      if (!alwaysRecreate && Kit.Cache.TryGet<ApiAssemblyInfo>(specs, out var result))
        return result;

      // Load rules from AppData and start Setup
      var rulesNs = App.Data.GetAll<RuleNamespace>().ToList();
      var rulesClass = App.Data.GetAll<RuleClass>().ToList();
      var assemblyInfo = Setup(path, rulesNs, rulesClass);

      // Cache & return
      Kit.Cache.Set(specs, assemblyInfo);
      return assemblyInfo;
    }

    /// <summary>
    /// Get already created from cache, called from sub-files which can assume that it's already in the cache.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public ApiAssemblyInfo GetCached(string name) =>
      Kit.Cache.Get<ApiAssemblyInfo>(GetSpecs(name));

    #endregion

    public ApiAssemblyInfo Setup(string path, List<RuleNamespace> nsRules, List<RuleClass> rules) {
      var assembly = Assembly.LoadFrom(path);

      // Get all namespaces
      var nsList = Analyze.GetNamespaces(assembly).ToList();

      // Retrieve special rules for internal, backend and integration namespaces
      var ruleInternal = As<RuleNamespace>(nsRules.FirstOrDefault(r => r.Title == "Special:*.Internal")).Setup(true);
      var ruleBackend = As<RuleNamespace>(nsRules.FirstOrDefault(r => r.Title == "Special:*.Backend")).Setup(true);
      var ruleIntegration = As<RuleNamespace>(nsRules.FirstOrDefault(r => r.Title == "Special:*.Integration")).Setup(true);

      // Create pairs of namespace and rule, so that we can easily find the rule for a namespace
      var nsWithRules = nsList
        .Select(ns =>
        {
          var currentNsRule = nsRules.FirstOrDefault(r => r.Title == ns);
          var nsRule = currentNsRule == null ? null : As<RuleNamespace>(currentNsRule);
          nsRule ??= ns?.Contains(".Internal") == true ? ruleInternal : null;
          nsRule ??= ns?.Contains(".Backend") == true ? ruleBackend : null;
          nsRule ??= ns?.Contains(".Integration") == true ? ruleIntegration : null;

          return new
          {
            Title = ns,
            Rule = nsRule
          };
        })
        .ToList();

      // Get all types in assembly - and filter out the ones we don't want
      var typeInfoManager = new TypeInfoManager();
      var allTypes = assembly.GetTypes()
        .Select(t =>
        {
          var rule = rules.FirstOrDefault(r => r.Title == t.FullName);
          var nsRule = nsWithRules.FirstOrDefault(r => r.Title == t.Namespace);
          return typeInfoManager.Create(t, rule, nsRule?.Rule);
        })
        .ToList();
      
      var relevant = allTypes
        .Where(t => t.Visibility.IsPublic)
        .Where(t => !FilterNamespaces.Contains(t.Namespace))
        .OrderBy(t => t.Name)
        .ToList();

      var types = new ThingStats<ApiTypeInfo>(allTypes, relevant);

      // From the remaining types, get the namespaces etc.

      // Figure out all Namespaces
      var allNs = nsWithRules // nsList
        .Select(ns => new ApiNamespaceInfo(ns.Title, assembly, types, ns.Rule))
        .ToList();
      
      var relevantNs = allNs
        .Where(ns => !FilterNamespaces.Contains(ns.Namespace))
        .Where(ns => ns.Types.Relevant.Count > 0)
        .ToList();

      var namespaces = new ThingStats<ApiNamespaceInfo>(allNs, relevantNs);

      var result = new ApiAssemblyInfo
      {
        Path = path,
        Name = System.IO.Path.GetFileName(path),
        Assembly = assembly,
        Namespaces = namespaces,
        Types = types,
        Overall = GetOverall(namespaces.Relevant)
      };
      return result;
    }

    private static Status GetOverall(List<ApiNamespaceInfo> relevant)
    {
      var ok = relevant.All(m => m.Overall.Ok);
      var notOk = relevant.Where(m => !m.Overall.Ok).ToList();
      var percent = relevant.Count == 0
        ? 100
        : 100 - (int) (100 * (double) notOk.Count / relevant.Count);

      // var skip = Rule?.IgnoreAll ?? false;
      // if (skip) return new Status(true, Ok(100), "Ignored");
      
      return new Status(ok, Ok(percent),
        ok
          ? "All members are ok"
          : $"{notOk.Count} members are not ok - {percent}%"
      );
    }

  }
}