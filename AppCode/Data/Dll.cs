namespace AppCode.Data
{

  public partial class Dll
  {
    public AssemblyInfo AssemblyInfo => _assemblyInfo ??= AssemblyInfo.GetIfCached(Name);
    private AssemblyInfo _assemblyInfo;
  }
}
