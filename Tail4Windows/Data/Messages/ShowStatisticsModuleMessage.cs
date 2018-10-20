namespace Org.Vs.TailForWin.Data.Messages
{
  /// <summary>
  /// Show statistics module message
  /// </summary>
  public class ShowStatisticsModuleMessage
  {
    /// <summary>
    /// Current sender
    /// </summary>
    public object Sender
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="sender">Current sender</param>
    public ShowStatisticsModuleMessage(object sender) => Sender = sender;
  }
}
