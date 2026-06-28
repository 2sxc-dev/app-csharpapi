namespace AppCode
{
  public class Status
  {
    public Status(string icon, string message, string icons = default, string details = default)
      : this(false, icon, message, icons, details)
    { }

    public Status(bool ok, string icon, string message, string icons = default, string details = default)
    {
      Ok = ok;
      Icon = icon;
      Icons = icons ?? "";
      Message = message;
      Details = details ?? message;
    }

    public bool Ok { get; }
    public string Icon { get; }
    public string Icons {get;}
    public string Message { get; }
    public string Details { get; }
  }
}
