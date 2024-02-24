// DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
// See also: https://go.2sxc.org/copilot-data
// To extend it, create a "RuleNamespace.cs" with this contents:
/*
namespace AppCode.Data
{
  public partial class RuleNamespace
  {
    // Add your own properties and methods here
  }
}
*/

// Generator:   DataModelGenerator v17.02.01
// App/Edition: App-CSharpApi/
// User:        2sichost
// When:        2024-02-24 10:50:47Z
namespace AppCode.Data
{
  // This is a generated class for RuleNamespace
  // To extend/modify it, see instructions above.

  /// <summary>
  /// RuleNamespace data. <br/>
  /// Generated 2024-02-24 10:50:47Z. Re-generate whenever you change the ContentType. <br/>
  /// <br/>
  /// Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
  /// Most properties have a simple access, such as `.IgnoreAll`. <br/>
  /// For other properties or uses, use methods such as
  /// .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
  /// </summary>
  public partial class RuleNamespace: RuleNamespaceAutoGenerated
  {  }

  /// <summary>
  /// Auto-Generated base class for RuleNamespace.
  /// </summary>
  public abstract class RuleNamespaceAutoGenerated: Custom.Data.Item16
  {
    /// <summary>
    /// IgnoreAll as bool. <br/>
    /// To get nullable use .Get("IgnoreAll") as bool?;
    /// </summary>
    public bool IgnoreAll => _myItem.Bool("IgnoreAll");

    /// <summary>
    /// IgnoreTypeMembers as bool. <br/>
    /// To get nullable use .Get("IgnoreTypeMembers") as bool?;
    /// </summary>
    public bool IgnoreTypeMembers => _myItem.Bool("IgnoreTypeMembers");

    /// <summary>
    /// Notes as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("Notes", scrubHtml: true) etc.
    /// </summary>
    public string Notes => _myItem.String("Notes", fallback: "");

    /// <summary>
    /// Title as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("Title", scrubHtml: true) etc.
    /// </summary>
    public string Title => _myItem.String("Title", fallback: "");
  }
}