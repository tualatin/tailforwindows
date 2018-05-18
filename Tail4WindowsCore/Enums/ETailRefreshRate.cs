using System.ComponentModel;


namespace Org.Vs.TailForWin.Core.Enums
{
  /// <summary>
  /// Enum RefreshRate
  /// </summary>
  public enum ETailRefreshRate
  {
    /// <summary>
    /// 1000 ms
    /// </summary>
    [Description("ETailRefreshRateNormal")]
    Normal = 1000,

    /// <summary>
    /// 200 ms
    /// </summary>
    [Description("ETailRefreshRateFast")]
    Fast = 200,

    /// <summary>
    /// 50 ms
    /// </summary>
    [Description("ETailRefreshRateHighest")]
    Highest = 50
  }
}
