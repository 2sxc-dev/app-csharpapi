using System.Reflection;

namespace AppCode.Models
{
  public class ApiAssemblyInfo
  {
    public string Path { get; internal set; }

    public Assembly Assembly { get; internal set; }

    public string Name { get; internal set; }


    public ThingStats<ApiNamespaceInfo> Namespaces { get; internal set; }

    public ThingStats<ApiTypeInfo> Types { get; internal set; }

    public Status Overall { get; internal set; }

  }

}