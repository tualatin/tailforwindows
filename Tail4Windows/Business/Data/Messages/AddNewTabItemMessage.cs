namespace Org.Vs.TailForWin.Business.Data.Messages
{
  /// <summary>
  /// AddNewTabItem message
  /// </summary>
  public class AddNewTabItemMessage
  {
    /// <summary>
    /// Whos sends the message
    /// </summary>
    public object Sender
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="sender">Whos sends the message</param>
    public AddNewTabItemMessage(object sender) => Sender = sender;
  }
}
