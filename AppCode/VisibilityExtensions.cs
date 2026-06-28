using static AppCode.Constants;
using AppCode.Models;

namespace AppCode
{
  public static class VisibilityExtensions
  {

    #region GetDocs

    public static Status GetDocsStatus(this IVisibility v)
    {
      var icons = v.GenDocsAttributes();
      var details = "Summary: " + icons + "\n" + v.GenDocsDetails();

      // not public, all is ok
      if (!v.IsPublic)
        return new Status(true, NotPublic, "Not public", icons, details);

      // public, but private in docs and intellisense
      if (v.HasPrivateApi)
        return new Status(true, PrivApi, "PrivateApi", icons, details);

      // public, but show in both docs and intellisense
      if (v.HasPublicApi)
        return new Status(true, PubApi, "PublicApi", icons, details);

      if (v.HasInternalApi)
        return new Status(true, IntApi, "InternalApi", icons, details);

      if (v.HasWorkInProgressApi)
        return new Status(true, WipApi, "WorkInProgressApi", icons, details);

      // something not ok
      details = "No Docs Specs on Type; Docs not visible.\n"
        + $"Public: {v.IsPublic}\n"
        + $"Docs: {v.ShowInDocs}\n"
        + $"Editor: {v.EditorStatus.Ok} ({v.EditorStatus.Icon})\n\n" + details;
      return new Status(false, "🔏", details, icons);
    }

    private static string GenDocsDetails(this IVisibility v) =>
      string.Join("\n", new string[] {
        $"✅Public: {v.IsPublic}",
        $"{PrivApi}PrivateApi: {v.HasPrivateApi}",
        $"{PubApi}PublicApi: {v.HasPublicApi}",
        $"{IntApi}InternalApi: {v.HasInternalApi}",
        $"{WipApi}WorkInProgressApi: {v.HasWorkInProgressApi}"
      });

    private static string GenDocsAttributes(this IVisibility v) =>
      string.Join(" ", new string[] {
        v.HasPrivateApi ? PrivApi : "",
        v.HasPublicApi ? PubApi : "",
        v.HasInternalApi ? IntApi : "",
        v.HasWorkInProgressApi ? WipApi : ""
      });

    #endregion



    #region GetSummary

    public static Status GetApiStatus(this IVisibility ownVis, IVisibility parentVis = null)
    {
      // First try with main result
      var result = ownVis.GetApiStatus("")
        // then try with parent
        ?? parentVis?.GetApiStatus("parent");
      if (result != null)
        return result;

      // something not ok
      var exp = (ownVis as ApiVisibility)?.Expected;
      return new Status(false, Ok0, "Something not ok.\n"
        + $"Public: {ownVis.IsPublic}\n"
        + $"Docs: {ownVis.ShowInDocs}\n"
        + $"Editor: {ownVis.EditorStatus.Ok} ({ownVis.EditorStatus.Icon})\n\n"
        + $"Expected Rule: {exp} (empty if no rule defined)\n"
        + $"ExpectedDocs: {exp?.ExpectedDocs}\n"
        + $"ExpectedIntellisense: {exp?.ExpectedIntellisense}");
    }

    private static Status GetApiStatus(this IVisibility v, string addOnMessage)
    {
      // not public, all is ok
      if (!v.IsPublic)
        return Status.Perfect("Not public");

      // public, but private in docs and intellisense
      if (!v.ShowInDocs && !v.EditorStatus.Ok)
        return Status.Perfect($"Private/warned in docs and intellisense {addOnMessage}");

      // public, but show in both docs and intellisense
      if (v.ShowInDocs && v.EditorStatus.Ok)
        return new Status(true, Ok99, $"Public in docs and intellisense {addOnMessage}");

      // internal, but show in docs and hide in intellisense
      if (v.ShowInDocs && v.HasInternalApi && !v.EditorStatus.Ok)
        return new Status(true, Ok99, $"Docs: Internal, hide intellisense {addOnMessage}");

      var exp = (v as ApiVisibility)?.Expected;
      if (exp != null && exp.ExpectedDocs == v.ShowInDocs && exp.ExpectedIntellisense == v.EditorStatus.Ok)
        return new Status(true, Ok75, $"Expected override in rule {addOnMessage}");

      return null;
    }

    #endregion


    #region GetEditorStatus

    public static Status GetEditorStatus(this IVisibility ownVis, IVisibility parentVis = null) =>
      ownVis.GetEditorStatusOne(false, "")
        ?? parentVis?.GetEditorStatusOne(true, "parent")
        // Fallback: use default visibility check
        ?? new Status(true, "☀️", $"show in intellisense");

    private static Status GetEditorStatusOne(this IVisibility vis, bool isParent, string addOnMessage)
    {
      // Problem: is browsable but obsolete, not ok; obsolete should be non-browsable
      if (vis.HasEditorBrowsable && vis.HasObsolete)
        return new Status(false, "🌚", $"obsolete & hide in intellisense {addOnMessage}");

      // is browsable, could be a problem
      if (vis.HasEditorBrowsable)
        return new Status(false, isParent ? "🌒" : "🌑", $"hide in intellisense {addOnMessage}");

      // Is Obsolete, but not hide! could be a problem
      if (vis.HasObsolete)
        return new Status(false, "☪️", $"obsolete {addOnMessage}");

      // Fallback: hand back as not specified yet
      return null;
    }

    #endregion
  }
}