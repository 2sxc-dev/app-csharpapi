using System.Reflection;
using AppCode.Models;

namespace AppCode.Analyzers
{
  public class VisibilityManager
  {
    public IVisibility Create(MemberInfo memberInfo, bool isPublic, bool isProtected, IExpectedDocsAndIntellisense expected = null)
    {
      return new ApiVisibility()
      {
        MemberInfo = memberInfo,
        IsPublic = isPublic,
        IsProtected = isProtected,
        Expected = expected,

        HasPrivateApi = memberInfo?.HasPrivateApi() ?? false,
        HasPublicApi = memberInfo?.HasPublicApi() ?? false,
        HasInternalApi = memberInfo?.HasInternalApi() ?? false,
        HasWorkInProgressApi = memberInfo?.HasWorkInProgressApi() ?? false,
        HasEditorBrowsable = memberInfo?.HasHideInIntellisense() ?? false,
        HasObsolete = memberInfo?.HasObsolete() ?? false
      };
    }
  }

}