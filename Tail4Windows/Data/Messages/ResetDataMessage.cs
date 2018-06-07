namespace Org.Vs.TailForWin.Data.Messages
{
  /// <summary>
  /// Reset data message
  /// </summary>
  public class ResetDataMessage
  {
    /// <summary>
    /// Who is sending the message
    /// </summary>
    public object Sender
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="sender">Whos sends the message</param>
    public ResetDataMessage(object sender) => Sender = sender;
  }
}
