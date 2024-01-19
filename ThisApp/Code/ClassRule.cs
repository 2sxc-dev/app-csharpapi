using ToSic.Sxc.Data;

namespace ThisApp.Code
{
  public class ClassRule: Custom.Data.Item16Experimental, IExpectedDocsAndIntellisense
  {
    public ClassRule(ITypedItem item): base(item) { }

    public bool IgnoreAllProperties => Bool();
    public bool ExpectedDocs => Bool();
    public bool ExpectedIntellisense => Bool();
    public bool IgnoreMembersWithoutSpecs => Bool();
  }

  public interface IExpectedDocsAndIntellisense
  {
    bool ExpectedDocs { get; }
    bool ExpectedIntellisense { get; }
  }
}
