using System.Windows.Controls;


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
    /// <param name="tabHeader">Name of tab header</param>
    /// <param name="content">TabItem content</param>
    void AddTabItem(string tabHeader, Control content);

    /// <summary>
    /// TabItem source
    /// </summary>
    ItemCollection TabItems
    {
      get;
    }

    /// <summary>
    /// Remove TabItem
    /// </summary>
    /// <param name="tabItem"><see cref="DragSupportTabItem"/></param>
    void RemoveTabItem(DragSupportTabItem tabItem);
  }
}
