using System;
using System.Linq;
using System.Reflection;
using static ThisApp.Code.Constants;

namespace ThisApp.Code
{
  public class TypeInfo
  {
    public TypeInfo(Type type, ClassRule rule, RuleNamespace ruleNamespace)
    {
      Type = type;
      Rule = rule;
      RuleNamespace = ruleNamespace;
      Name = type.Name;
      Namespace = type.Namespace;

      Visibility = new Visibility(type, type.IsPublic, type.IsAbstract, rule);

      var allMembers = type.GetMembers()
        .Select(m => new MemInfo(m, Visibility, rule))
        .ToList();
      var typeObject = typeof(object);
      var typeEnum = typeof(Enum);
      var relevantMembers = type.GetMembers()
        // todo: what about constructors?
        .Where(m => !(m is MethodInfo mInfo) || !mInfo.IsSpecialName)
        // Filter out members which are from the object base class
        .Where(m => m.DeclaringType != typeObject && m.DeclaringType != typeEnum)
        .OrderBy(m => m.Name)
        .Select(m => new MemInfo(m, Visibility, rule))
        .ToList();
      Members = new ThingStats<MemInfo>(allMembers, relevantMembers);
    }

    public Type Type { get; }

    public ClassRule Rule { get; }

    public RuleNamespace RuleNamespace { get; }

    public Status TypeSummary => _typeSummary
      ??= Type.IsClass ? new Status(IconClass, "class") : new Status(IconInterface, "interface");
    private Status _typeSummary;

    public string Name { get; }
    public string Namespace { get; }
    public IVisibility Visibility { get; }

    public ThingStats<MemInfo> Members { get; }

    public Status Overall => _overall ??= GetOverall();
    private Status _overall;

    private Status GetOverall()
    {

      var relevant = Members.Relevant;
      var ok = relevant.All(m => m.Visibility.Summary.Ok);
      var notOk = relevant.Where(m => !m.Visibility.Summary.Ok).ToList();
      var percent = relevant.Count == 0
        ? 100
        : 100 - (int) (100 * (double) notOk.Count / relevant.Count);

      var skip = Rule?.IgnoreAllProperties ?? RuleNamespace?.IgnoreTypeMembers ?? false;
      if (skip) {
        ok = true;
        percent = 100;
      }

      var visSum = Visibility.Summary;
      if (!visSum.Ok) {
        ok = false;
        percent = percent / 2;
      }
      // ok = ok && visSum.Ok;
      return new Status(ok, Ok(percent),
        ok
          ? "All members are ok"
          : $"{notOk.Count} members are not ok - {percent}%"
      );
    }
  }

}