using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils;


namespace Org.Vs.TailForWin.Business.Data.Messages
{
  /// <summary>
  /// Remove a <see cref="DragSupportTabItem"/> from item source
  /// </summary>
  public class RemoveTabItemMessage
  {
    /// <summary>
    /// Item to remove
    /// </summary>
    public DragSupportTabItem ItemToRemove
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="item">Item to remove from type of <see cref="DragSupportTabItem"/></param>
    public RemoveTabItemMessage(DragSupportTabItem item) => ItemToRemove = item;
  }
}
