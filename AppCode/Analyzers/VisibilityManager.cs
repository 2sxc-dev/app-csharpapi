using System.Reflection;
using AppCode.Data;
using AppCode.Models;

namespace AppCode.Analyzers
{
  public class VisibilityManager
  {
    public IVisibility Create(MemberInfo memberInfo, bool isPublic, bool isProtected, IExpectedDocsAndIntellisense expected = null)
    {
      var hasPrivateApi = memberInfo?.HasPrivateApi() ?? false;
      var hasPublicApi = memberInfo?.HasPublicApi() ?? false;
      var hasInternalApi = memberInfo?.HasInternalApi() ?? false;
      var hasWorkInProgressApi = memberInfo?.HasWorkInProgressApi() ?? false;

      var hasEditorBrowsable = memberInfo?.HasHideInIntellisense() ?? false;
      var hasObsolete = memberInfo?.HasObsolete() ?? false;

      return new ApiVisibility()
      {
        MemberInfo = memberInfo,
        IsPublic = isPublic,
        IsProtected = isProtected,
        Expected = expected,

        HasPrivateApi = hasPrivateApi,
        HasPublicApi = hasPublicApi,
        HasInternalApi = hasInternalApi,
        HasWorkInProgressApi = hasWorkInProgressApi,
        ShowInDocs = isPublic && !hasPrivateApi && (hasPublicApi || hasInternalApi || hasWorkInProgressApi),
        HasDocs = hasPrivateApi || hasPublicApi || hasInternalApi || hasWorkInProgressApi,

        HasEditorBrowsable = hasEditorBrowsable,
        HasObsolete = hasObsolete,
        EditorHideOrWarn = hasEditorBrowsable || hasObsolete
      };
    }

    public IVisibility CreateWithParent(IVisibility own, IVisibility parent, RuleClass rule)
    {
      var hasPrivateApi = own.HasPrivateApi || parent.HasPrivateApi;
      var hasPublicApi = own.HasPublicApi || parent.HasPublicApi;
      var hasInternalApi = own.HasInternalApi || parent.HasInternalApi;
      var hasWorkInProgressApi = own.HasWorkInProgressApi || parent.HasWorkInProgressApi;
      var result = new ApiVisibilityWithParent(own, parent, rule)
      {
        // MemberInfo = own.MemberInfo,
        IsPublic = own.IsPublic,
        IsProtected = own.IsProtected,
        Expected = (own as ApiVisibility)?.Expected,

        HasPrivateApi = hasPrivateApi,
        HasPublicApi = hasPublicApi,
        HasInternalApi = hasInternalApi,
        HasWorkInProgressApi = hasWorkInProgressApi,
        HasDocs = own.HasDocs,
        ShowInDocs = own.HasDocs
          ? own.ShowInDocs
          : parent.ShowInDocs,



        HasEditorBrowsable = own.HasEditorBrowsable,
        HasObsolete = own.HasObsolete,
        EditorHideOrWarn = own.EditorHideOrWarn
      };

      return result;
    }
  }

}