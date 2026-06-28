using System.Reflection;

namespace AppCode.Models
{
  public class ApiMemberInfo: ICommonInfo
  {
    public string Name { get; internal set; }

    public string LabelExtended { get; internal set; }

    public string Label { get; internal set; }

    public MemberInfo MemberInfo { get; internal set; }

    public bool IsPublic { get; internal set; }

    public bool IsStatic { get; internal set; }

    public bool IsAbstract { get; internal set; }

    public bool IsVirtual { get; internal set; }

    public bool IsPrivate { get; internal set; }

    // note: IsFamilyOrAssembly or IsFamilyAndAssembly would be protected not public
    // https://learn.microsoft.com/en-us/dotnet/api/system.reflection.methodbase.isfamily?view=net-8.0
    public bool IsProtectedPublic { get; internal set; }

    public IVisibility Visibility { get; internal set; }
    public IVisibility OwnVisibility { get; internal set; }

    public InfoWithIcon TypeInfo => _typeInfo ??= GetTypeInfo();
    private InfoWithIcon _typeInfo;

    private InfoWithIcon GetTypeInfo() => MemberInfo.MemberType switch
    {
      MemberTypes.Constructor => new InfoWithIcon("🏗️", "constructor"),
      MemberTypes.Event => new InfoWithIcon("🔫", "event"),
      MemberTypes.Field => new InfoWithIcon("⏹️", "field"),
      MemberTypes.Method => new InfoWithIcon("🚀", "method"),
      MemberTypes.NestedType => new InfoWithIcon("❓", "nested type"),
      MemberTypes.Property => (MemberInfo as PropertyInfo) switch
      {
        { CanRead: true, CanWrite: true } => new InfoWithIcon("🧊", "property r/w"),
        { CanRead: true } => new InfoWithIcon("📤", "property r"),
        { CanWrite: true } => new InfoWithIcon("📥", "property w"),
        _ => new InfoWithIcon("🧊", "property")
      },
      _ => new InfoWithIcon("❔", "other?"),
    };
  }

}