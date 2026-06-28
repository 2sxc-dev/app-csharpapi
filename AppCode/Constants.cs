namespace AppCode
{
  public static class Constants
  {
    public const string PathToDlls = "..\\..\\..\\..\\bin";
    public const string ToSicSxc = "ToSic.Sxc.Core";

    public static string[] FilterNamespaces = new string[] {
      null,
      "",
      "Microsoft.CodeAnalysis",
      "System.Runtime.CompilerServices",
    };

    #region Visibility according to .net public / private

    public const string VisPublic = "🌐";
    public const string VisInvisible = "⭕";

    #endregion

    public const string IconClass = "🟦";
    public const string IconInterface = "⏹️";

    public const string IconSelected = "🎯";

    #region Visibility according to Docs

    public const string NotPublic = "🥷";
    public const string PrivApi = "🔐";
    public const string PubApi = "👁️";
    public const string PrivUntagged = "🔒";
    public const string IntApi = "🔓";
    public const string WipApi = "🚧";

    #endregion

    #region Visibility according to Editor

    public const string VisEditObsAndHidden = "🌚";
    public const string VisEditHiddenByParent = "🌒";
    public const string VisEditHiddenOnly = "🌑";
    public const string VisEditVisible = "🌕";
    public const string VisEditObsOnly = "☪️";

    #endregion

    public const string Ok100 = "✅";
    public const string Ok99 = "🟩";
    public const string Ok75 = "🟢";
    public const string Ok50 = "🟡";
    public const string Ok25 = "🟠";
    public const string Ok0 = "🔴";

    public static string Ok(int percent)
    {
      if (percent >= 100) return Ok100;
      if (percent >= 99) return Ok99;
      if (percent >= 75) return Ok75;
      if (percent >= 50) return Ok50;
      if (percent >= 25) return Ok25;
      return Ok0;
    }
  }
}