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

      var hasBrowsable = relevant
        .Where(t => t.Members.Relevant.Any(m => m.Visibility.EditorStatus.Ok))
        .ToList();

      var hasBrowsableInfo = hasBrowsable.Count == 0
        ? Constants.VisEditHiddenOnly
        : hasBrowsable.Count == relevant.Count
          ? Constants.VisEditVisible
          : $"{hasBrowsable.Count}";

      Types = new ThingStats<ApiTypeInfo>(all, relevant,
        ts => $"{hasBrowsableInfo}/{ts.Relevant.Count}/{ts.All.Count}",
        ts => string.Join("\n", new [] {
          $"Statistics",
          "",
          $"{hasBrowsable.Count:00} Visible in IntelliSense",
          $"{ts.Relevant.Count:00} Relevant",
          $"{ts.All.Count:00} Total" })
      );

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