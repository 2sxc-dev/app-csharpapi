using static ThisApp.Code.Show;
using static ThisApp.Code.Constants;
using System;

namespace ThisApp.Code
{
  public class MemVisibility: IVisibility
  {
    public MemVisibility(IVisibility memVis, IVisibility classVis, ClassRule rule) {
      MemVis = memVis;
      ClassVis = classVis;
      ClassRule = rule;
    }
    private IVisibility MemVis { get; }
    private IVisibility ClassVis { get; }
    private ClassRule ClassRule { get; }

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

    public bool EditorHideOrWarn => MemVis.HasObsolete;

    // For this only the own is relevant
    public bool HasEditorBrowsable => MemVis.HasEditorBrowsable;

    private Status _editorStatus;
    public Status EditorStatus => _editorStatus ??= Visibility.GetEditorStatus(MemVis);

    private Status _summary;
    public Status Summary => _summary ??= new Func<Status>(() => {
      // Special case, where nothing is actually set, and the parent is not visible, so things are just ok?
      if (ClassRule?.IgnoreMembersWithoutSpecs == true && !MemVis.HasDocs && !MemVis.HasEditorBrowsable)
        return new Status(true, "🛅", "ignore members without specs");

      // Fallback: use default visibility check
      return Visibility.GetSummary(this);
    })();


    public Status Docs => _docs ??= GetDocs();

    public bool HasObsolete => MemVis.HasObsolete;

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