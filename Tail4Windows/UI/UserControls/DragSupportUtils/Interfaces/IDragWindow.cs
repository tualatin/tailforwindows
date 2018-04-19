using System.Collections.ObjectModel;


namespace Org.Vs.TailForWin.UI.UserControls.DragSupportUtils.Interfaces
{
  /// <summary>
  /// DragWindow interface
  /// </summary>
  public interface IDragWindow
  {
    /// <summary>
    /// Add TabItem
    /// </summary>
    /// <param name="tabItem"><see cref="DragSupportTabItem"/></param>
    void AddTabItem(DragSupportTabItem tabItem);

    /// <summary>
    /// TabItem source
    /// </summary>
    ObservableCollection<DragSupportTabItem> TabItems
    {
      get;
      set;
    }

    /// <summary>
    /// Remove TabItem
    /// </summary>
    /// <param name="tabItem"><see cref="DragSupportTabItem"/></param>
    /// <param name="dragWndRemove">Drag window remove</param>
    void RemoveTabItem(DragSupportTabItem tabItem, bool dragWndRemove);
  }
}
