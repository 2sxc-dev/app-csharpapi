using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AppCode.Data;
using ToSic.Sxc.Services.Cache;
using static AppCode.Constants;

namespace AppCode
{
  public class AssemblyInfo // : Custom.Hybrid.CodeTyped
  {
    public string Path { get; internal set; }

    public Assembly Assembly { get; internal set; }

    public string Name { get; internal set; }


    public ThingStats<NamespaceInfo> Namespaces { get; internal set; }

    public ThingStats<TypeInfo> Types { get; internal set; }

    public Status Overall { get; internal set; }

  }

}