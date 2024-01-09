using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Experimental;

namespace ThisApp.Code
{
  public class RuleNamespace: TypedItem
  {
    public RuleNamespace(ITypedItem item): base(item) { }

    public bool IgnoreAll => GetThis<bool>();

    public bool IgnoreTypeMembers => GetThis<bool>();
  }

}
