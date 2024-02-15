using ToSic.Sxc.Data;

namespace ThisApp.Code
{
  public class RuleNamespace: Custom.Data.Item16
  {
    // public RuleNamespace(ITypedItem item, bool shared = false): base(item) {
    //   SharedRule = shared;
    // }
    public RuleNamespace Setup(bool shared = false) {
      SharedRule = shared;
      return this;
    }

    public bool IgnoreAll => Bool();

    public bool IgnoreTypeMembers => Bool();

    public bool SharedRule { get; private set; }
  }

}
