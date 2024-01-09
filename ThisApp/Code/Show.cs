using System;

namespace ThisApp.Code
{
  public static class Show
  {
    /// <summary>
    /// Show a boolean value as an emoji
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// 
    public static string BoolMoji(bool value, string ifTrue = default, string ifFalse = default) {
      return value ? (ifTrue ?? "✅") : (ifFalse ?? "❌");
    }

    public static string ToEmoji(this bool value, string ifTrue = default, string ifFalse = default) {
      return value ? (ifTrue ?? "✅") : (ifFalse ?? "❌");
    }
  }
}
