using System.Reflection;

namespace AppCode.Models
{

  public class ApiVisibility: IVisibility
  {
    #region Reflection Stuff

    public MemberInfo MemberInfo { get; internal set; }

    public IExpectedDocsAndIntellisense Expected { get; internal set; }

    #endregion
    
    public bool IsPublic { get; internal set; }

    public bool IsProtected { get; internal set; }

    #region Attributes for Docs

    public Status Docs => _docs ??= this.GetDocsStatus();
    private Status _docs;

    // TODO: MISSING check for IsProtected which can also result in showing
    public bool ShowInDocs => IsPublic && !HasPrivateApi && (HasPublicApi || HasInternalApi || HasWorkInProgressApi);

    public bool HasPrivateApi { get; internal set; }
    public bool HasPublicApi { get; internal set; }
    public bool HasInternalApi { get; internal set; }
    public bool HasWorkInProgressApi { get; internal set; }

    public bool HasDocs => HasPrivateApi || HasPublicApi || HasInternalApi || HasWorkInProgressApi;

    /// <summary>
    /// Summary for editor - either obsolete warning or hide attribute
    /// </summary>
    public bool EditorHideOrWarn => HasEditorBrowsable || HasObsolete;
    public bool HasEditorBrowsable { get; internal set; }
    public bool HasObsolete { get; internal set; }

    public Status EditorStatus => _editorStatus ??= this.GetEditorStatus();
    private Status _editorStatus;

    #endregion

    public Status Summary => _summary ??= this.GetSummary();
    private Status _summary;

  }

}