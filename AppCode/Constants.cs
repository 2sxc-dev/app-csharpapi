namespace AppCode
{
  public static class Constants
  {
    public const string PathToDlls = "..\\..\\..\\..\\bin";
    public const string ToSicSxc = "ToSic.Sxc";

    public static string[] FilterNamespaces = new string[] {
      null,
      "",
      "Microsoft.CodeAnalysis",
      "System.Runtime.CompilerServices",
    };

    public const string IconClass = "🟦";
    public const string IconInterface = "⏹️";

    public const string IconSelected = "🎯";

    public const string NotPublic = "🥷";
    public const string PrivApi = "🔏";
    public const string PubApi = "👁️";
    public const string IntApi = "🔓";
    public const string WipApi = "🚧";

    public const string Ok100 = "✅";
    public const string Ok75 = "🟢";
    public const string Ok50 = "🟡";
    public const string Ok25 = "🟠";
    public const string Ok0 = "🔴";

    public static string Ok(int percent)
    {
      if (percent >= 100) return Ok100;
      // if (percent >= 75) return Ok75;
      if (percent >= 50) return Ok50;
      if (percent >= 25) return Ok25;
      return Ok0;
    }
  }
}