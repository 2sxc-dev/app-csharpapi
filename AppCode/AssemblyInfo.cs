using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ToSic.Sxc.Data;
using AppCode.Data;
using static AppCode.Constants;

namespace AppCode
{
  public class AssemblyInfo: Custom.Hybrid.CodeTyped
  {
    public AssemblyInfo Setup(string path, List<RuleNamespace> nsRules, List<RuleClass> rules) {
      Path = path;
      Assembly = Assembly.LoadFrom(path);

      var nsList = Analyze.GetNamespaces(Assembly).ToList();
      var ruleInternal = As<RuleNamespace>(nsRules.FirstOrDefault(r => r.Title == "Special:*.Internal")).Setup(true);
      var ruleBackend = As<RuleNamespace>(nsRules.FirstOrDefault(r => r.Title == "Special:*.Backend")).Setup(true);
      var ruleIntegration = As<RuleNamespace>(nsRules.FirstOrDefault(r => r.Title == "Special:*.Integration")).Setup(true);
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
          var ruleItem = rules.FirstOrDefault(r => r.Title == t.FullName);
          var rule = ruleItem == null ? null : As<RuleClass>(ruleItem);
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
        .Select(ns =>
        {
          // var ruleItem = nsRules.FirstOrDefault(r => r.Title == ns.Title);
          // var rule = ruleItem == null ? null : new RuleNamespace(ruleItem);
          return new NamespaceInfo(ns.Title, Assembly, Types, ns.Rule);
        })
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




    #region Cache

    public static IDictionary<string, AssemblyInfo> Cache = new Dictionary<string, AssemblyInfo>();

    public static AssemblyInfo Get(string name, string path, List<RuleNamespace> nsRules, List<RuleClass> rules, Func<AssemblyInfo> generate)
    {
      if (Cache.ContainsKey(name)) return Cache[name];
      var assemblyInfo = generate().Setup(path, nsRules, rules);
      Cache[name] = assemblyInfo;
      return assemblyInfo;
    }

    public static AssemblyInfo GetIfCached(string path) => Cache.ContainsKey(path) ? Cache[path] : null;

    #endregion

  }

}