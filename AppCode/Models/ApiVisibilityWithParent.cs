using System;
using AppCode.Data;

namespace AppCode.Models
{
  public class ApiVisibilityWithParent: ApiVisibility, IVisibility
  {
    public ApiVisibilityWithParent(IVisibility memVis, IVisibility parentVis, RuleClass rule) {
      MemVis = memVis;
      ParentVis = parentVis;
      ClassRule = rule;
    }
    private IVisibility MemVis { get; }
    private IVisibility ParentVis { get; }
    private RuleClass ClassRule { get; }

    private Status _editorStatus;
    private Status _summary;
    private Status _docs;

    public Status EditorStatus => _editorStatus ??= MemVis.GetEditorStatus(ParentVis);

    public Status Summary => _summary ??= new Func<Status>(() => {
      // Special case, where nothing is actually set, and the parent is not visible, so things are just ok?
      if (ClassRule?.IgnoreMembersWithoutSpecs == true && !MemVis.HasDocs && !MemVis.HasEditorBrowsable)
        return Status.Ignored("🛅", "ignore members without specs");

      // Fallback: use default visibility check
      return MemVis.GetApiStatus(ParentVis);
    })();


    public Status Docs => _docs ??= GetDocsVisibilityStatus();


    private Status GetDocsVisibilityStatus()
    {
      var docs = this.GetDocsStatus();
      return new Status(docs.Ok, docs.Icon, docs.Message, docs.Icons,
        "**Own** "
        + MemVis?.Docs.Details
        + "\n\n**Parent** "
        + ParentVis?.Docs.Details
        + "\n\n**Merged** "
        + docs.Details);
    }

  }
}