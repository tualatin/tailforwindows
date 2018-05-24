namespace Org.Vs.TailForWin.Data.Messages
{
  /// <summary>
  /// Set <see cref="UI.FloatWindow.VsFloatingWindow"/> topmost flag message
  /// </summary>
  public class SetFloatingTopmostFlagMessage
  {
    /// <summary>
    /// Topmost flag
    /// </summary>
    public bool Topmost
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="topmost">topmost flag</param>
    public SetFloatingTopmostFlagMessage(bool topmost) => Topmost = topmost;
  }
}
