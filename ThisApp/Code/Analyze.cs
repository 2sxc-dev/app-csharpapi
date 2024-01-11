using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ToSic.Lib.Documentation;

namespace ThisApp.Code
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
      // ATM this doesn't work, because we can't reference the type EditorBrowsable because System.Runtime.dll is not from the bin folder...?
      // var attribs = type.GetCustomAttributes(typeof(System.ComponentModel.EditorBrowsable), true);
      var attribs = type.GetCustomAttributes().Where(a => a.GetType().FullName == "System.ComponentModel.EditorBrowsableAttribute").ToArray();
      if (attribs.Length == 0) return false;

      // todo: check if it's set to never
      return true;
      // type.GetCustomAttributes(typeof(HideFromIntellisense), true).Length > 0;
    }
  }

}