namespace Org.Vs.TailForWin.Business.Data.Messages
{
  /// <summary>
  /// Reset data message
  /// </summary>
  public class ResetDataMessage
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
    /// <param name="sender">Whos sends the message</param>
    public ResetDataMessage(object sender) => Sender = sender;
  }
}
