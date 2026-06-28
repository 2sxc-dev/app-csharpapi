using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AppCode.Data;
using ToSic.Sxc.Services.Cache;
using static AppCode.Constants;

namespace AppCode
{
  public class AssemblyInfo: Custom.Hybrid.CodeTyped
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
    public AssemblyInfo GetOrCreate(string name, string path, bool alwaysRecreate)
    {
      // First check cached and use that if possible
      var specs = GetSpecs(name);
      if (!alwaysRecreate && Kit.Cache.TryGet<AssemblyInfo>(specs, out var result))
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
    public AssemblyInfo GetCached(string name) =>
      Kit.Cache.Get<AssemblyInfo>(GetSpecs(name));

    #endregion

    public AssemblyInfo Setup(string path, List<RuleNamespace> nsRules, List<RuleClass> rules) {
      Path = path;
      Assembly = Assembly.LoadFrom(path);

      // Get all namespaces
      var nsList = Analyze.GetNamespaces(Assembly).ToList();

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
      var allTypes = Assembly.GetTypes()
        .Select(t =>
        {
          var rule = rules.FirstOrDefault(r => r.Title == t.FullName);
          var nsRule = nsWithRules.FirstOrDefault(r => r.Title == t.Namespace);
          return new TypeInfo(t, rule, nsRule?.Rule);
        })
        .ToList();
      
      var relevant = allTypes
        .Where(t => t.Visibility.IsPublic)
        .Where(t => !FilterNamespaces.Contains(t.Namespace))
        .OrderBy(t => t.Name)
        .ToList();

      Types = new ThingStats<TypeInfo>(allTypes, relevant);

      // From the remaining types, get the namespaces etc.

      // Figure out all Namespaces
      var allNs = nsWithRules // nsList
        .Select(ns => new NamespaceInfo(ns.Title, Assembly, Types, ns.Rule))
        .ToList();
      
      var relevantNs = allNs
        .Where(ns => !FilterNamespaces.Contains(ns.Namespace))
        .Where(ns => ns.Types.Relevant.Count > 0)
        .ToList();

      Namespaces = new ThingStats<NamespaceInfo>(allNs, relevantNs);

      return this;
    }

    public string Path { get; private set; }

    public Assembly Assembly { get; private set; }

    public string Name => System.IO.Path.GetFileName(Path);


    public ThingStats<NamespaceInfo> Namespaces { get; private set; }

    public ThingStats<TypeInfo> Types { get; private set; }

    #region Overall Status

    public Status Overall => _overall ??= GetOverall(this);
    private Status _overall;

    public static Status GetOverall(AssemblyInfo assemblyInfo)
    {
      var relevant = assemblyInfo.Namespaces.Relevant;
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

    #endregion

  }

}