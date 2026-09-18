namespace AppCode.Visibility
{
  public interface IVisibility
  {
    /// <summary>
    /// Member or type is publicly visible.
    /// </summary>
    bool IsPublic { get; }
    /// <summary>
    /// Member or type is protected.
    /// </summary>
    bool IsProtected { get; }

    Status Docs { get; }


    bool ShowInDocs { get; }

    bool HasPrivateApi { get; }
    bool HasPublicApi { get; }
    bool HasInternalApi { get; }
    bool HasWorkInProgressApi { get; }

    bool HasDocs { get; }
    bool HasEditorBrowsable { get; }
    bool HasObsolete { get; }
    bool EditorHideOrWarn { get; }

    Status EditorStatus { get; }
    Status Summary { get; }

  }
}