namespace Org.Vs.TailForWin.Ui.Utils.EventMessages
{
  /// <summary>
  /// Set topmost flag message
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
