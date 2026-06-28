using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ToSic.Sys.Documentation;

namespace AppCode
{
  public static class Analyze
  {

    public static List<string> GetNamespaces(Assembly assembly)
    {
      var namespaces = assembly
        .GetTypes()
        .Select(t => t.Namespace)
        .Distinct()
        .OrderBy(n => n)
        .ToList();
      return namespaces;
    }

    public static string GetTopLevelNamespace(Type t)
    {
        string ns = t.Namespace ?? "";
        int firstDot = ns.IndexOf('.');
        return firstDot == -1 ? ns : ns.Substring(0, firstDot);
    }

    public static bool HasInternalApi(this MemberInfo type) 
      => type.GetCustomAttributes(typeof(InternalApi_DoNotUse_MayChangeWithoutNotice), true).Length > 0;

    public static bool HasPublicApi(this MemberInfo type)
      => type.GetCustomAttributes(typeof(PublicApi), true).Length > 0;

    public static bool HasPrivateApi(this MemberInfo type)
      => type.GetCustomAttributes(typeof(PrivateApi), true).Length > 0;

    public static bool HasWorkInProgressApi(this MemberInfo type)
      => type.GetCustomAttributes(typeof(WorkInProgressApi), true).Length > 0;

    public static bool HasObsolete(this MemberInfo type)
      => type.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length > 0;

    public static bool HasHideInIntellisense(this MemberInfo type)
    {
      // ATM the following won't work, because we can't reference the type EditorBrowsable because System.Runtime.dll is not from the bin folder...?
      // var attribs = type.GetCustomAttributes(typeof(System.ComponentModel.EditorBrowsable), true);
      var attribs = type.GetCustomAttributes()
        .Where(a => FullNamesForHidden.Contains(a.GetType().FullName))
        .ToArray();

      // todo: check if it's set to 'Never'

      return attribs.Length > 0;
    }

    private static readonly List<string> FullNamesForHidden = new List<string>
    {
      // The standard EditorBrowsableAttribute is in System.Runtime.dll
      "System.ComponentModel.EditorBrowsableAttribute",
      // The custom one, used during debug builds; it's fake but this checker should still recognize it.
      "FixEditorBrowsable.FakeEditorBrowsableAttribute",
    };
  }

}