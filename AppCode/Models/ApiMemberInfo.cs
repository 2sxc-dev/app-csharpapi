using System.Reflection;

namespace AppCode.Models
{
  public class ApiMemberInfo
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

    public Status TypeSummary => _typeSummary ??= GetTypeStatus();
    private Status _typeSummary;

    private Status GetTypeStatus() => MemberInfo.MemberType switch
    {
      MemberTypes.Constructor => new Status("🏗️", "constructor"),
      MemberTypes.Event => new Status("🔫", "event"),
      MemberTypes.Field => new Status("⏹️", "field"),
      MemberTypes.Method => new Status("🚀", "method"),
      MemberTypes.NestedType => new Status("❓", "nested type"),
      MemberTypes.Property => (MemberInfo as PropertyInfo) switch
      {
        { CanRead: true, CanWrite: true } => new Status("🧊", "property r/w"),
        { CanRead: true } => new Status("📤", "property r"),
        { CanWrite: true } => new Status("📥", "property w"),
        _ => new Status("🧊", "property")
      },
      _ => new Status("❔", "other?"),
    };
  }

}