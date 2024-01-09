using System.Linq;
using System.Reflection;
using static ThisApp.Code.Constants;

namespace ThisApp.Code
{
  public class NamespaceInfo
  {
    public NamespaceInfo(string ns, Assembly assembly, ThingStats<TypeInfo> types, RuleNamespace rule)
    {
      Namespace = ns;
      Assembly = assembly;
      Rule = rule;
      Types = new ThingStats<TypeInfo>(types.All.Where(t => t.Namespace == ns), types.Relevant.Where(t => t.Namespace == ns));
    }

    public string Namespace { get; }

    public Assembly Assembly { get; }
    public RuleNamespace Rule { get; }

    public ThingStats<TypeInfo> Types { get; }

    public Status Overall => _overall ??= GetOverall();
    private Status _overall;

    private Status GetOverall()
    {
      var relevant = Types.Relevant;
      var ok = relevant.All(m => m.Overall.Ok);
      var notOk = relevant.Where(m => !m.Overall.Ok).ToList();
      var percent = relevant.Count == 0
        ? 100
        : 100 - (int) (100 * (double) notOk.Count / relevant.Count);

      var skip = Rule?.IgnoreAll ?? false;
      if (skip) return new Status(true, Ok(100), "Ignored");
      
      // var visSum = Visibility.Summary;
      // if (!visSum.Ok) {
      //   ok = false;
      //   percent = percent / 2;
      // }
      // ok = ok && visSum.Ok;
      return new Status(ok, Ok(percent),
        ok
          ? "All members are ok"
          : $"{notOk.Count} members are not ok - {percent}%"
      );
    }

  }

}