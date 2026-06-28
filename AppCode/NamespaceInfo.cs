using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AppCode.Data;
using AppCode.Models;
using static AppCode.Constants;
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
      var all = types.All.Where(t => t.Namespace == ns).ToList();
      var relevant = types.Relevant.Where(t => t.Namespace == ns).ToList();
      Overall = (rule?.IgnoreAll ?? false)
        ? Status.Ignored()
        : Status.GetFromRelevantInfos(relevant.Select(t => t.Overall).ToList());
      Types = new ThingStats<ApiTypeInfo>(all, relevant);
    }

    public string Namespace { get; internal set; }

    public Assembly Assembly { get; internal set; }
    public RuleNamespace Rule { get; internal set; }

    public ThingStats<ApiTypeInfo> Types { get; internal set; }

    public Status Overall { get; internal set; }

  }

}