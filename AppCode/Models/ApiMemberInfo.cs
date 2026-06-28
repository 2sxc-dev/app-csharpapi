using System.Linq;
using System.Reflection;
using AppCode.Analyzers;
using AppCode.Data;

namespace AppCode.Models
{
  public class ApiMemberInfo
  {
    public ApiMemberInfo(MemberInfo mInfo, IVisibility parentVisibility, RuleClass rule)
    {
      MemberInfo = mInfo;
      Name = mInfo.Name;

      var labels = GetLabelAndDetails();
      LabelExtended = labels.Details;
      Label = labels.Label;

      var visManager = new VisibilityManager();
      OwnVisibility = visManager.Create(mInfo, IsPublic, IsProtectedPublic);
      Visibility = new ApiVisibilityOfMember(OwnVisibility, parentVisibility, rule);
    }

    private (string Label, string Details) GetLabelAndDetails()
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
          return (Name, Name + " { get; }");
        return (Name, Name + " { set; }");
      }

      return (Name, Name);
    }

    public string Name { get; }

    public string LabelExtended { get; }

    public string Label { get; }

    public MemberInfo MemberInfo { get; }
    public FieldInfo FieldInfo => MemberInfo as FieldInfo;
    public PropertyInfo PropertyInfo => MemberInfo as PropertyInfo;
    public MethodInfo MethodInfo => MemberInfo as MethodInfo;

    public bool IsPublic => _isPublic ?? (_isPublic = FieldInfo?.IsPublic
      ?? PropertyInfo?.GetMethod.IsPublic
      ?? MethodInfo?.IsPublic
      ?? false).Value;
    private bool? _isPublic;

    public bool IsStatic => _isStatic ?? (_isStatic = FieldInfo?.IsStatic
      ?? PropertyInfo?.GetMethod.IsStatic
      ?? MethodInfo?.IsStatic
      ?? false).Value;
    private bool? _isStatic;

    public bool IsAbstract => _isAbstract ?? (_isAbstract =
      PropertyInfo?.GetMethod.IsAbstract
      ?? MethodInfo?.IsAbstract
      ?? false).Value;
    private bool? _isAbstract;

    public bool IsVirtual => _isVirtual ?? (_isVirtual =
      PropertyInfo?.GetMethod.IsVirtual
      ?? MethodInfo?.IsVirtual
      ?? false).Value;
    private bool? _isVirtual;

    public bool IsPrivate => _isPrivate ?? (_isPrivate = FieldInfo?.IsPrivate
      ?? PropertyInfo?.GetMethod.IsPrivate
      ?? MethodInfo?.IsPrivate
      ?? false).Value;
    private bool? _isPrivate;

    // note: IsFamilyOrAssembly or IsFamilyAndAssembly would be protected not public
    // https://learn.microsoft.com/en-us/dotnet/api/system.reflection.methodbase.isfamily?view=net-8.0
    public bool IsProtectedPublic => _isProtected ?? (_isProtected = FieldInfo?.IsFamily
      ?? PropertyInfo?.GetMethod.IsFamily
      ?? MethodInfo?.IsFamily
      ?? false).Value;
    private bool? _isProtected;

    public IVisibility Visibility { get; }
    public IVisibility OwnVisibility { get; }

    public Status TypeSummary => _typeSummary ??= GetTypeSummary();
    private Status _typeSummary;

    private Status GetTypeSummary() => MemberInfo.MemberType switch
    {
      MemberTypes.Constructor => new Status("🏗️", "constructor"),
      MemberTypes.Event => new Status("🔫", "event"),
      MemberTypes.Field => new Status("⏹️", "field"),
      MemberTypes.Method => new Status("🚀", "method"),
      MemberTypes.NestedType => new Status("❓", "nested type"),
      MemberTypes.Property when PropertyInfo.CanRead && PropertyInfo.CanWrite => new Status("🧊", "property r/w"),
      MemberTypes.Property when PropertyInfo.CanRead => new Status("📤", "property r"),
      MemberTypes.Property when PropertyInfo.CanWrite => new Status("📥", "property w"),
      MemberTypes.Property => new Status("🧊", "property"),
      _ => new Status("❔", "other?"),
    };
  }

}