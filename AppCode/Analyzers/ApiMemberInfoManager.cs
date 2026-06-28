using System.Linq;
using System.Reflection;
using AppCode.Data;
using AppCode.Models;

namespace AppCode.Analyzers
{
  public class ApiMemberInfoManager
  {
    public ApiMemberInfo Create(MemberInfo MemberInfo, IVisibility parentVisibility, RuleClass rule)
    {
      var FieldInfo = MemberInfo as FieldInfo;
      var PropertyInfo = MemberInfo as PropertyInfo;
      var MethodInfo = MemberInfo as MethodInfo;

      var IsPublic = FieldInfo?.IsPublic
        ?? PropertyInfo?.GetMethod.IsPublic
        ?? MethodInfo?.IsPublic
        ?? false;

      var IsStatic = FieldInfo?.IsStatic
        ?? PropertyInfo?.GetMethod.IsStatic
        ?? MethodInfo?.IsStatic
        ?? false;

      var IsAbstract = PropertyInfo?.GetMethod.IsAbstract
        ?? MethodInfo?.IsAbstract
        ?? false;


      var IsVirtual = PropertyInfo?.GetMethod.IsVirtual
        ?? MethodInfo?.IsVirtual
        ?? false;

      var IsPrivate = FieldInfo?.IsPrivate
        ?? PropertyInfo?.GetMethod.IsPrivate
        ?? MethodInfo?.IsPrivate
        ?? false;

      // note: IsFamilyOrAssembly or IsFamilyAndAssembly would be protected not public
      // https://learn.microsoft.com/en-us/dotnet/api/system.reflection.methodbase.isfamily?view=net-8.0
      var IsProtectedPublic = FieldInfo?.IsFamily
        ?? PropertyInfo?.GetMethod.IsFamily
        ?? MethodInfo?.IsFamily
        ?? false;

      var labels = GetLabelAndDetails(MemberInfo, MemberInfo.Name);

      var visManager = new VisibilityManager();
      var ownVisibility = visManager.Create(MemberInfo, IsPublic, IsProtectedPublic);
      var Visibility = visManager.CreateWithParent(ownVisibility, parentVisibility, rule);

      return new ApiMemberInfo()
      {
        MemberInfo = MemberInfo,
        Name = MemberInfo.Name,
        LabelExtended = labels.Details,
        Label = labels.Label,
        OwnVisibility = ownVisibility,
        Visibility = Visibility,

        IsPublic = IsPublic,
        IsStatic = IsStatic,
        IsAbstract = IsAbstract,
        IsVirtual = IsVirtual,
        IsPrivate = IsPrivate,
        IsProtectedPublic = IsProtectedPublic
      };
    }

    private (string Label, string Details) GetLabelAndDetails(MemberInfo MemberInfo, string Name)
    {
      if (MemberInfo is MethodInfo mInfoMethod) {
        var parameters = mInfoMethod.GetParameters();
        if (parameters.Length > 0) {
          var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterType.Name + " " + p.Name));
          return (Name + $"({parameters.Count()})", Name + $"({parameterNames})");
        } else {
          return (Name + "()", Name + "()");
        }
      }

      if (MemberInfo is PropertyInfo mInfoProp) {
        var canRead = mInfoProp.CanRead;
        var canWrite = mInfoProp.CanWrite;
        if (canRead && canWrite)
          return (Name, Name + " { get; set; }");
        if (canRead)
          return (Name, Name + " { get; internal set; }");
        return (Name, Name + " { set; }");
      }

      return (Name, Name);
    }

  }

}