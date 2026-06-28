namespace AppCode.Models
{
  public interface ICommonInfo
  {
    public string Name { get; }

    public IVisibility Visibility { get; }
    // public IVisibility OwnVisibility { get; internal set; }

    public InfoWithIcon TypeInfo  { get; }
 }

}