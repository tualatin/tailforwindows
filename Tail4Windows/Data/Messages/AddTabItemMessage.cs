using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils;


namespace Org.Vs.TailForWin.Data.Messages
{
  /// <summary>
  /// Add <see cref="DragSupportTabItem"/> message
  /// </summary>
  public class AddTabItemMessage
  {
    /// <summary>
    /// <see cref="DragSupportTabItem"/> to add
    /// </summary>
    public DragSupportTabItem TabItem
    {
      get;
    }

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
    /// <param name="sender">Who is sending the message</param>
    /// <param name="tabItem"><see cref="DragSupportTabItem"/></param>
    public AddTabItemMessage(object sender, DragSupportTabItem tabItem)
    {
      Sender = sender;
      TabItem = tabItem;
    }
  }
}
