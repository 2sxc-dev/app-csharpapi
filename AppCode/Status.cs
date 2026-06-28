using System.Collections.Generic;
using System.Linq;

namespace AppCode
{
  public class Status
  {
    public Status(bool ok, string icon, string message, string icons = default, string details = default)
    {
      Ok = ok;
      Icon = icon;
      Icons = icons ?? "";
      Message = message;
      Details = details ?? message;
    }

    public bool Ok { get; }

    public int Percent { get; internal set; }

    public string Icon { get; }
    public string Icons {get;}
    public string Message { get; }
    public string Details { get; }

    internal static Status Perfect(string? message = null) =>
      new Status(true, Constants.Ok(100), message ?? "Perfect / Ok 100%");

    internal static Status Ignored(string? icon = null, string? message = null) =>
      new Status(true, icon ?? Constants.Ok(100), message ?? "Ignored");

    internal static Status GetFromRelevantInfos(List<Status> relevant)
    {
      var ok = relevant.All(m => m.Ok);
      var notOk = relevant.Where(m => !m.Ok).ToList();
      var percent = relevant.Count == 0
        ? 100
        : 100 - (int) (100 * (double) notOk.Count / relevant.Count);
      
      return new Status(ok, Constants.Ok(percent),
        ok
          ? "All members are ok"
          : $"{notOk.Count} members are not ok - {percent}%"
      )
      {
        Percent = percent,
      };
    }
  }
}
