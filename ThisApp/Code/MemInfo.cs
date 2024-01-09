using System;
using System.Linq;
using System.Reflection;
using static ThisApp.Code.Constants;

namespace ThisApp.Code
{
  public class MemInfo
  {
    public MemInfo(MemberInfo mInfo, IVisibility parentVisibility, ClassRule rule)
    {
      MemberInfo = mInfo;
      Name = mInfo.Name;

      var labels = GetLabelAndDetails();
      LabelExtended = labels.Details;
      Label = labels.Label;

      OwnVisibility = new Visibility<MemberInfo>(mInfo, IsPublic, IsProtectedPublic);
      Visibility = new MemVisibility(OwnVisibility, parentVisibility, rule);
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
        if (canRead && canWrite) return (Name, Name + " { get; set; }");
        if (canRead) return (Name, Name + " { get; }");
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
    public Visibility<MemberInfo> OwnVisibility { get; }

    public Status TypeSummary => _typeSummary ??= GetTypeSummary();
    private Status _typeSummary;

    private Status GetTypeSummary()
    {
      var type = MemberInfo.MemberType;
      switch (type) {
        case MemberTypes.Constructor:
          return new Status("🏗️", "constructor");
        case MemberTypes.Event:
          return new Status("🔫", "event");
        case MemberTypes.Field:
          return new Status("⏹️", "field");
        case MemberTypes.Method:
          return new Status("🚀", "method");
        case MemberTypes.NestedType:
          return new Status("❓", "nested type");
        case MemberTypes.Property:
          if (PropertyInfo.CanRead && PropertyInfo.CanWrite) return new Status("🧊", "property r/w");
          if (PropertyInfo.CanRead) return new Status("📤", "property r");
          if (PropertyInfo.CanWrite) return new Status("📥", "property w");
          return new Status("🧊", "property");
        default:
          return new Status("❔", "other?");
      }
    }
    // public ThingStats<MemberInfo> Members { get; }
  }

}