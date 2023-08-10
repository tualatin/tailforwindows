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
    /// Current Window Id
    /// </summary>
    public Guid DragWindowId
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
    /// <param name="dragWindowId">Current DragWindow Id</param>
    /// <param name="isPinned">Is pinned state</param>
    public DragSupportTabItemPinnedChangedMessage(object sender, Guid dragWindowId, bool isPinned)
    {
      Sender = sender;
      DragWindowId = dragWindowId;
      IsPinned = isPinned;
    }
  }
}
