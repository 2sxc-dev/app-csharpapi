using static ThisApp.Code.Show;

namespace ThisApp.Code
{
  public class VisibilityMulti: IVisibility
  {
    public VisibilityMulti(IVisibility memVis, IVisibility classVis) {
      MemVis = memVis;
      ClassVis = classVis;
    }
    private IVisibility MemVis { get; }
    private IVisibility ClassVis { get; }

    // This is only relevant if the parent is somehow public
    public bool IsPublic => MemVis.IsPublic;

    // For this only the own is relevant
    public bool IsProtected => MemVis.IsProtected;

    public bool ShowInDocs => _showInDocs ?? (_showInDocs = MergeShowInDocs(MemVis, ClassVis)).Value;
    private bool? _showInDocs;
    private bool MergeShowInDocs(IVisibility own, IVisibility parent) {
      if (!own.IsPublic) return false;
      // todo: protected if parent class is public

      // if own has docs attributes, it wins
      if (own.HasDocs) return own.ShowInDocs;

      // fallback: parent show in docs
      return parent.ShowInDocs;
    }

    public bool HasPrivateApi => MemVis.HasPrivateApi || ClassVis.HasPrivateApi;

    public bool HasPublicApi => MemVis.HasPublicApi || ClassVis.HasPublicApi;

    public bool HasInternalApi => MemVis.HasInternalApi || ClassVis.HasInternalApi;

    public bool HasWorkInProgressApi => MemVis.HasWorkInProgressApi || ClassVis.HasWorkInProgressApi;

    public bool HasDocs => MemVis.HasDocs;

    // For this only the own is relevant
    public bool HasEditorBrowsable => MemVis.HasEditorBrowsable;

    // For this only the own is relevant
    public bool ShowInIntelliSense => MemVis.ShowInIntelliSense;

    public Status Summary => _summary ??= Visibility.GetSummary(this);
    private Status _summary;

    public Status Docs => _docs ??= GetDocs();
    private Status _docs;

    private Status GetDocs()
    {
      var docs = Visibility.GetDocs(this);
      return new Status(docs.Icon, docs.Message, docs.Icons, 
        "**Own** "
        + MemVis?.Docs.Details
        + "\n\n**Parent** "
        + ClassVis?.Docs.Details
        + "\n\n**Merged** "
        + docs.Details);
    }

  }
}