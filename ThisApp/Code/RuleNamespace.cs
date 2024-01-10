using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Experimental;

namespace ThisApp.Code
{
  public class RuleNamespace: TypedItem
  {
    public RuleNamespace(ITypedItem item, bool shared = false): base(item) {
      SharedRule = shared;
    }

    public bool IgnoreAll => GetThis<bool>();

    public bool IgnoreTypeMembers => GetThis<bool>();

    public bool SharedRule { get; }
  }

}
