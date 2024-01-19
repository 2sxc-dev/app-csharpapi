using ToSic.Sxc.Data;

namespace ThisApp.Code
{
  public class RuleNamespace: Custom.Data.Item16Experimental
  {
    public RuleNamespace(ITypedItem item, bool shared = false): base(item) {
      SharedRule = shared;
    }

    public bool IgnoreAll => Bool();

    public bool IgnoreTypeMembers => Bool();

    public bool SharedRule { get; }
  }

}
