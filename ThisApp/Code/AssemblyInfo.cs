using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ToSic.Sxc.Data;
using static ThisApp.Code.Constants;

namespace ThisApp.Code
{
  public class AssemblyInfo
  {
    public AssemblyInfo(string path, List<ITypedItem> todo, List<ITypedItem> nsRules, List<ITypedItem> rules) {
      Path = path;
      Assembly = Assembly.LoadFrom(path);

      var nsList = Analyze.GetNamespaces(Assembly).ToList();
      var ruleInternal = new RuleNamespace(nsRules.FirstOrDefault(r => r.Title == "Special:*.Internal"), shared: true);
      var ruleBackend = new RuleNamespace(nsRules.FirstOrDefault(r => r.Title == "Special:*.Backend"), shared: true);
      var ruleIntegration = new RuleNamespace(nsRules.FirstOrDefault(r => r.Title == "Special:*.Integration"), shared: true);
      var nsWithRules = nsList
        .Select(ns =>
        {
          var currentNsRule = nsRules.FirstOrDefault(r => r.Title == ns);
          var nsRule = currentNsRule == null ? null : new RuleNamespace(currentNsRule);
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
          var rule = ruleItem == null ? null : new ClassRule(ruleItem);
          var nsRule = nsWithRules.FirstOrDefault(r => r.Title == t.Namespace);
          return new TypeInfo(t, rule, nsRule?.Rule);
        })
        .ToList();
      
      var relevant = allTypes
        .Where(t => t.Visibility.IsPublic)
        .Where(t => !Constants.FilterNamespaces.Contains(t.Namespace))
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
        .Where(ns => !Constants.FilterNamespaces.Contains(ns.Namespace))
        .Where(ns => ns.Types.Relevant.Count > 0)
        .ToList();

      Namespaces = new ThingStats<NamespaceInfo>(allNs, relevantNs);
    }

    public string Path { get; }

    public Assembly Assembly { get; }

    public string Name => System.IO.Path.GetFileName(Path);


    public ThingStats<NamespaceInfo> Namespaces { get; }

    public ThingStats<TypeInfo> Types { get; }

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

    public static AssemblyInfo Get(string name, string path, List<ITypedItem> todo, List<ITypedItem> nsRules, List<ITypedItem> rules)
    {
      if (Cache.ContainsKey(name)) return Cache[name];
      var assemblyInfo = new AssemblyInfo(path, todo, nsRules, rules);
      Cache[name] = assemblyInfo;
      return assemblyInfo;
    }

    public static AssemblyInfo GetIfCached(string path) => Cache.ContainsKey(path) ? Cache[path] : null;

    #endregion

  }

}