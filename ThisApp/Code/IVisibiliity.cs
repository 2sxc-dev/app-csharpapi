namespace ThisApp.Code
{
  public interface IVisibility
  {
    bool IsPublic { get; }
    bool IsProtected { get; }

    Status Docs { get; }


    bool ShowInDocs { get; }

    bool HasPrivateApi { get; }
    bool HasPublicApi { get; }
    bool HasInternalApi { get; }
    bool HasWorkInProgressApi { get; }

    bool HasDocs { get; }
    bool HasEditorBrowsable { get; }


    bool ShowInIntelliSense { get; }
    Status Summary { get; }

  }
}