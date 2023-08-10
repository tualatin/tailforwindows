using System;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils;

namespace Org.Vs.TailForWin.Data.Messages.DragSupportTabControl
{
  /// <summary>
  /// <see cref="DragSupportTabItem"/> pinned state changed
  /// </summary>
  public class DragSupportTabItemPinnedChangedMessage
  {
    /// <summary>
    /// Who sends the event
    /// </summary>
    public object Sender
    {
      get;
    }

    /// <summary>
    /// TabItem Id
    /// </summary>
    public Guid TabItemId
    {
      get;
    }

    /// <summary>
    /// Is pinned state
    /// </summary>
    public bool IsPinned
    {
      get;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="sender">Who sends the event</param>
    /// <param name="tabItemId">TabItem Id</param>
    /// <param name="isPinned">Is pinned state</param>
    public DragSupportTabItemPinnedChangedMessage(object sender, Guid tabItemId, bool isPinned)
    {
      Sender = sender;
      TabItemId = tabItemId;
      IsPinned = isPinned;
    }
  }
}
