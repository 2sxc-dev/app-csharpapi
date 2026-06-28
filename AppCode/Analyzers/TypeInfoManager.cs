using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AppCode.Data;
using AppCode.Models;
using static AppCode.Constants;
using TypeInfo = AppCode.Models.TypeInfo;

namespace AppCode.Analyzers
{
  public class TypeInfoManager
  {
    public TypeInfo Create(Type type, RuleClass rule, RuleNamespace ruleNamespace)
    {
      var visibility = new Visibility(type, type.IsPublic, type.IsAbstract, rule);
      var allMembers = type.GetMembers()
        .Select(m => new MemInfo(m, visibility, rule))
        .ToList();
      var typeObject = typeof(object);
      var typeEnum = typeof(Enum);
      var relevantMembers = type.GetMembers()
        // todo: what about constructors?
        .Where(m => !(m is MethodInfo mInfo) || !mInfo.IsSpecialName)
        // Filter out members which are from the object base class
        .Where(m => m.DeclaringType != typeObject && m.DeclaringType != typeEnum)
        .OrderBy(m => m.Name)
        .Select(m => new MemInfo(m, visibility, rule))
        .ToList();

      var members = new ThingStats<MemInfo>(allMembers, relevantMembers);
      var result = new TypeInfo()
      {
        Type = type,
        Rule = rule,
        RuleNamespace = ruleNamespace,
        Name = type.Name,
        Namespace = type.Namespace,

        Visibility = visibility,

        Members = members,
        TypeSummary = TypeSummary(type),
        Overall = GetOverall(members.Relevant, visibility, rule, ruleNamespace),
      };
      return result;
    }

    private Status TypeSummary(Type type) =>
      type.IsClass ? new Status(IconClass, "class") : new Status(IconInterface, "interface");

    private Status GetOverall(List<MemInfo> relevant, IVisibility visibility, RuleClass? rule, RuleNamespace? ruleNamespace)
    {

      var ok = relevant.All(m => m.Visibility.Summary.Ok);
      var notOk = relevant.Where(m => !m.Visibility.Summary.Ok).ToList();
      var percent = relevant.Count == 0
        ? 100
        : 100 - (int) (100 * (double) notOk.Count / relevant.Count);

      var skip = rule?.IgnoreAllProperties ?? ruleNamespace?.IgnoreTypeMembers ?? false;
      if (skip) {
        ok = true;
        percent = 100;
      }

      if (!visibility.Summary.Ok) {
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