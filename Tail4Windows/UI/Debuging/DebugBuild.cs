using System.Windows;


namespace Org.Vs.TailForWin.UI.Debuging
{
  /// <summary>
  /// For debug use only!
  /// </summary>
  public static class DebugBuild
  {
    /// <summary>
    /// Visible in debug mode only
    /// </summary>
    public static Visibility IsDebug
    {
#if DEBUG
      get => Visibility.Visible;
#else
      get => Visibility.Collapsed;
#endif
    }
  }
}
