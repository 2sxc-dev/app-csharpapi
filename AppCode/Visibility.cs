using System.Reflection;
using static AppCode.Show;
using static AppCode.Constants;

namespace AppCode
{
  public class Visibility<T> : Visibility
  {
    public Visibility(T target, bool isPublic, bool isProtected = false) : base(memberInfo: target as MemberInfo, isPublic, isProtected)
    {
    }
  }

  public class Visibility: IVisibility
  {
    public Visibility(MemberInfo memberInfo, bool isPublic, bool isProtected, IExpectedDocsAndIntellisense expected = null)
    {
      // Show if public
      IsPublic = isPublic;
      IsProtected = isProtected;

      MemberInfo = memberInfo;
      Expected = expected;

      if (memberInfo != null) {
        HasPrivateApi = memberInfo.HasPrivateApi();
        HasPublicApi = memberInfo.HasPublicApi();
        HasInternalApi = memberInfo.HasInternalApi();
        HasWorkInProgressApi = memberInfo.HasWorkInProgressApi();
        HasEditorBrowsable = memberInfo.HasHideInIntellisense();
        HasObsolete = memberInfo.HasObsolete();
      }
    }

    #region Reflection Stuff

    public MemberInfo MemberInfo { get; }

    public IExpectedDocsAndIntellisense Expected { get; }

    #endregion
    
    public bool IsPublic { get; }

    public bool IsProtected { get; }

    #region Attributes for Docs

    public Status Docs => _docs ??= GetDocs(this);
    private Status _docs;

    // TODO: MISSING check for IsProtected which can also result in showing
    public bool ShowInDocs => IsPublic && !HasPrivateApi && (HasPublicApi || HasInternalApi || HasWorkInProgressApi);

    public bool HasPrivateApi { get; }
    public bool HasPublicApi { get; }
    public bool HasInternalApi { get; }
    public bool HasWorkInProgressApi { get; }

    public bool HasDocs => HasPrivateApi || HasPublicApi || HasInternalApi || HasWorkInProgressApi;

    /// <summary>
    /// Summary for editor - either obsolete warning or hide attribute
    /// </summary>
    public bool EditorHideOrWarn => HasEditorBrowsable || HasObsolete;
    public bool HasEditorBrowsable { get; }
    public bool HasObsolete { get; }

    public Status EditorStatus => _editorStatus ??= GetEditorStatus(this);
    private Status _editorStatus;

    public static Status GetEditorStatus(IVisibility v)
    {
      if (v.HasEditorBrowsable && v.HasObsolete) return new Status(false, "🫥", "obsolete & hide in intellisense");
      if (v.HasEditorBrowsable) return new Status(false, "🔒", "hide in intellisense");
      if (v.HasObsolete) return new Status(false, "👮", "obsolete");

      // Fallback: use default visibility check
      return new Status(true, "👁", "show in intellisense");
    }

    #endregion

    public Status Summary => _summary ??= GetSummary(this);
    private Status _summary;


    #region Static Helpers

    public static Status GetDocs(IVisibility v)
    {
      var icons = GenDocsAttributes(v);
      var details = "Summary: " + icons + "\n" + GenDocsDetails(v);

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

    private static string GenDocsDetails(IVisibility v) => 
      $"✅Public: {v.IsPublic}; {PrivApi}PrivateApi: {v.HasPrivateApi}; {PubApi}PublicApi: {v.HasPublicApi}; {IntApi}InternalApi: {v.HasInternalApi}; {WipApi}WorkInProgressApi: {v.HasWorkInProgressApi}"
      .Replace("; ", "\n")
      .Replace(";", "\n");

    private static string GenDocsAttributes(IVisibility v) =>
      $"{BoolMoji(v.HasPrivateApi, PrivApi, "")}{BoolMoji(v.HasPublicApi, PubApi, "")}{BoolMoji(v.HasInternalApi, IntApi, "")}{BoolMoji(v.HasWorkInProgressApi, WipApi, "")}";



    public static Status GetSummary(IVisibility v)
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

      var exp = (v as Visibility)?.Expected;
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
  }

}