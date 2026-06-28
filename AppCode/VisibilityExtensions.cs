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
      return new Status(false, "🛅", details, icons);
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

    public static Status GetSummary(this IVisibility v)
    {
      // not public, all is ok
      if (!v.IsPublic)
        return new Status(true, Ok100, "Not public");

      // public, but private in docs and intellisense
      if (!v.ShowInDocs && !v.EditorStatus.Ok)
        return new Status(true, Ok100, "Private/warned in docs and intellisense");

      // public, but show in both docs and intellisense
      if (v.ShowInDocs && v.EditorStatus.Ok)
        return new Status(true, Ok75, "Public in docs and intellisense");

      var exp = (v as ApiVisibility)?.Expected;
      if (exp != null)
      {
        if (exp.ExpectedDocs == v.ShowInDocs && exp.ExpectedIntellisense == v.EditorStatus.Ok)
          return new Status(true, Ok75, "Expected override in rule");
      }

      // something not ok
      return new Status(false, Ok0, "Something not ok.\n"
        + $"Public: {v.IsPublic}\n"
        + $"Docs: {v.ShowInDocs}\n"
        + $"Editor: {v.EditorStatus.Ok} ({v.EditorStatus.Icon})\n\n"
        + $"Expected Rule: {exp} (empty if no rule defined)\n"
        + $"ExpectedDocs: {exp?.ExpectedDocs}\n"
        + $"ExpectedIntellisense: {exp?.ExpectedIntellisense}");
    }

    #endregion


    #region GetEditorStatus

    public static Status GetEditorStatus(this IVisibility v)
    {
      if (v.HasEditorBrowsable && v.HasObsolete)
        return new Status(false, "🫥", "obsolete & hide in intellisense");
      if (v.HasEditorBrowsable)
        return new Status(false, "🔒", "hide in intellisense");
      if (v.HasObsolete)
        return new Status(false, "👮", "obsolete");

      // Fallback: use default visibility check
      return new Status(true, "👁", "show in intellisense");
    }

    #endregion
  }
}