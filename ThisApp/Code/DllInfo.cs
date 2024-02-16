namespace AppCode.Data
{
  public abstract partial class DataModelBase: Custom.Data.Item16
  {
  }

  public partial class DllInfo: DataModelBase
  {
    public AssemblyInfo AssemblyInfo => _assemblyInfo ??= AssemblyInfo.GetIfCached(Name);
    private AssemblyInfo _assemblyInfo;

    public string Name => GetThis<string>();
    // public string Title => Item.Title;

    public string Description => GetThis<string>();

    public bool IgnoreAll => GetThis<bool>();
  }
}
