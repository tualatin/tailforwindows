namespace Org.Vs.TailForWin.Data.Messages
{
  /// <summary>
  /// <see cref="UI.UserControls.DragSupportUtils.DragWindow"/> TabItem changed message
  /// </summary>
  public class DragWindowTabItemChangedMessage
  {
    /// <summary>
    /// Who sends the message
    /// </summary>
    public object Sender
    {
      get;
    }

    /// <summary>
    /// New title
    /// </summary>
    public string NewTitle
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="sender">Who sends the message</param>
    /// <param name="title">New title</param>
    public DragWindowTabItemChangedMessage(object sender, string title)
    {
      Sender = sender;
      NewTitle = title;
    }
  }
}
