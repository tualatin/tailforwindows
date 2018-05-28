namespace Org.Vs.TailForWin.Data.Messages
{
  /// <summary>
  /// Bring MainWindow to front message
  /// </summary>
  public class BringMainWindowToFrontMessage
  {
    /// <summary>
    /// Who sends the message
    /// </summary>
    public object Sender
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="sender"></param>
    public BringMainWindowToFrontMessage(object sender) => Sender = sender;
  }
}
