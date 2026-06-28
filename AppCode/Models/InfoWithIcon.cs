namespace AppCode.Models
{
  /// <summary>
  /// Simple info, with an icon to show the information in compact form.
  /// </summary>
  public class InfoWithIcon
  {
    public InfoWithIcon(string icon, string message)
    {
      Icon = icon;
      Message = message;
    }
    public string Icon { get; }
    public string Message { get; }
  }
}
