using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Experimental;

namespace ThisApp.Code
{
  public class ClassRule: TypedItem, IExpectedDocsAndIntellisense
  {
    public ClassRule(ITypedItem item): base(item) { }

    public bool IgnoreAllProperties => GetThis<bool>();
    public bool ExpectedDocs => GetThis<bool>();
    public bool ExpectedIntellisense => GetThis<bool>();

    public bool IgnoreMembersWithoutSpecs => GetThis<bool>();
  }

  public interface IExpectedDocsAndIntellisense
  {
    bool ExpectedDocs { get; }
    bool ExpectedIntellisense { get; }
  }
}
