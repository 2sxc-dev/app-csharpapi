using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ToSic.Sxc.Data;

namespace ThisApp.Code
{
  public class AssemblyInfo
  {
    public AssemblyInfo(string path, List<ITypedItem> todo, List<ITypedItem> nsRules, List<ITypedItem> rules) {
      Path = path;
      Assembly = Assembly.LoadFrom(path);

      // Get all types in assembly - and filter out the ones we don't want
      var allTypes = Assembly.GetTypes()
        .Select(t =>
        {
          var ruleItem = rules.FirstOrDefault(r => r.Title == t.FullName);
          var rule = ruleItem == null ? null : new ClassRule(ruleItem);

          var currentNsRule = nsRules.FirstOrDefault(r => r.Title == t.Namespace);
          var nsRule = currentNsRule == null ? null : new RuleNamespace(currentNsRule);
          return new TypeInfo(t, rule, nsRule);
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
      var allNs = Analyze.GetNamespaces(Assembly)
        .Select(ns =>
        {
          var ruleItem = nsRules.FirstOrDefault(r => r.Title == ns);
          var rule = ruleItem == null ? null : new RuleNamespace(ruleItem);
          return new NamespaceInfo(ns, Assembly, Types, rule);
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


  }

}