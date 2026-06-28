using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AppCode.Data;
using AppCode.Models;
using static AppCode.Constants;
using ApiTypeInfo = AppCode.Models.ApiTypeInfo;

namespace AppCode.Analyzers
{
  public class TypeInfoManager
  {
    public ApiTypeInfo Create(Type type, RuleClass rule, RuleNamespace ruleNamespace)
    {
      var visManager = new VisibilityManager();
      var visibility = visManager.Create(type, type.IsPublic, type.IsAbstract, rule);
      var apiMemberInfoManager = new ApiMemberInfoManager();
      var allMembers = type.GetMembers()
        .Select(m => apiMemberInfoManager.Create(m, visibility, rule))
        .ToList();
      var typeObject = typeof(object);
      var typeEnum = typeof(Enum);
      var relevantMembers = type.GetMembers()
        // todo: what about constructors?
        .Where(m => !(m is MethodInfo mInfo) || !mInfo.IsSpecialName)
        // Filter out members which are from the object base class
        .Where(m => m.DeclaringType != typeObject && m.DeclaringType != typeEnum)
        .OrderBy(m => m.Name)
        .Select(m => apiMemberInfoManager.Create(m, visibility, rule))
        .ToList();

      var members = new ThingStats<ApiMemberInfo>(allMembers, relevantMembers);
      var result = new ApiTypeInfo()
      {
        Type = type,
        Rule = rule,
        RuleNamespace = ruleNamespace,
        Name = type.Name,
        Namespace = type.Namespace,

        Visibility = visibility,

        Members = members,
        TypeInfo = type.IsClass
          ? new InfoWithIcon(IconClass, "class")
          : new InfoWithIcon(IconInterface, "interface"),
        Overall = GetOverall(members.Relevant, visibility, rule, ruleNamespace),
      };
      return result;
    }

    private Status GetOverall(List<ApiMemberInfo> relevant, IVisibility visibility, RuleClass? rule, RuleNamespace? ruleNamespace)
    {
      var skip = rule?.IgnoreAllProperties ?? ruleNamespace?.IgnoreTypeMembers ?? false;
      if (skip)
        return Status.Ignored();

      var toUse = relevant.Select(m => m.Visibility.Summary).ToList();
      var initial = Status.GetFromRelevantInfos(toUse);
      if (visibility.Summary.Ok)
        return initial;

      var newPercent = initial.Percent / 2;
      return new Status(false, Ok(newPercent), initial.Message, initial.Icons,
        $"**Own** {visibility.Summary.Details}\n\n**Members** {initial.Details}")
      {
        Percent = newPercent,
      };
    }
  }

}