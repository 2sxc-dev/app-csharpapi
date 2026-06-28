using System;
using AppCode.Data;

namespace AppCode.Models
{
  public class ApiTypeInfo
  {
    public Type Type { get; internal set; }

    public RuleClass Rule { get; internal set; }

    public RuleNamespace RuleNamespace { get; internal set; }

    public Status TypeSummary { get; internal set; }

    public string Name { get; internal set; }
    public string Namespace { get; internal set; }
    public IVisibility Visibility { get; internal set; }

    public ThingStats<ApiMemberInfo> Members { get; internal set; }

    public Status Overall { get; internal set; }

  }

}