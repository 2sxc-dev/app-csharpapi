namespace AppCode
{
  public static class Show
  {
    /// <summary>
    /// Show a boolean value as an emoji
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// 
    public static string BoolMoji(this bool value, string ifTrue = default, string ifFalse = default) {
      return value ? (ifTrue ?? "✅") : (ifFalse ?? "❌");
    }
  }
}
