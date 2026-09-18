using AppCode.Models;

namespace AppCode.Razor
{
  public class DetailsSpecs
  {
    public ApiAssemblyInfo DllInfo { get; set; }
    public string CurrentNs { get; set; }
    public string CurrentType { get; set; }
    public ApiNamespaceInfo NsInfo { get; set; }
    public ApiTypeInfo ClassInfo { get; set; }
    public string SelectedMemberName { get; set; }

    public ICommonInfo Selected { get; set; }
  }
}