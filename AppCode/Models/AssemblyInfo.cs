using System.Reflection;

namespace AppCode.Models
{
  public class AssemblyInfo
  {
    public string Path { get; internal set; }

    public Assembly Assembly { get; internal set; }

    public string Name { get; internal set; }


    public ThingStats<NamespaceInfo> Namespaces { get; internal set; }

    public ThingStats<TypeInfo> Types { get; internal set; }

    public Status Overall { get; internal set; }

  }

}