using System.Linq;
using System.Reflection;
using AppCode.Data;
using AppCode.Models;
using ApiTypeInfo = AppCode.Models.ApiTypeInfo;

namespace AppCode
{
  public class ApiNamespaceInfo
  {
    public ApiNamespaceInfo(string ns, Assembly assembly, ThingStats<ApiTypeInfo> types, RuleNamespace rule)
    {
      Namespace = ns;
      Assembly = assembly;
      Rule = rule;
      var all = types.All
        .Where(t => t.Namespace == ns)
        .ToList();

      var relevant = types.Relevant
        .Where(t => t.Namespace == ns)
        .ToList();

      var visibleInCode = relevant
        .Where(t => t.Members.Relevant.Any(m => m.Visibility.EditorStatus.Ok))
        .ToList();

      Types = new StatisticsWithVisible<ApiTypeInfo>() {
        CountVisible = visibleInCode.Count,
        All = all,
        Relevant = relevant,
      };

      Overall = (rule?.IgnoreAll ?? false)
        ? Status.Ignored()
        : Status.GetFromRelevantInfos(relevant.Select(t => t.Overall).ToList());
    }

    public string Namespace { get; internal set; }

    public Assembly Assembly { get; internal set; }
    public RuleNamespace Rule { get; internal set; }

    public ThingStats<ApiTypeInfo> Types { get; internal set; }

    public Status Overall { get; internal set; }

  }

}