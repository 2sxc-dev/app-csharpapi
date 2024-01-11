using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Experimental;

namespace ThisApp.Data
{
  public abstract partial class DataModelBase: TypedItem
  {
    public DataModelBase(ITypedItem item): base(item) { }
  }

  public partial class DllInfo: DataModelBase
  {
    public DllInfo(ITypedItem item): base(item) { }

    public string Name => GetThis<string>();
    // public string Title => Item.Title;

    public string Description => GetThis<string>();

    public bool IgnoreAll => GetThis<bool>();
  }
}