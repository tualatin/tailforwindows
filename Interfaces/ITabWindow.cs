using System.Windows.Controls;


namespace Org.Vs.TailForWin.Interfaces
{
  /// <summary>
  /// Tab window interface
  /// </summary>
  public interface ITabWindow
  {
    /// <summary>
    /// Tab items collection
    /// </summary>
    ItemCollection TabItems
    {
      get;
    }

    /// <summary>
    /// Tab header selected
    /// </summary>
    string TabHeaderSelected
    {
      get;
      set;
    }

    /// <summary>
    /// Add tab item to control
    /// </summary>
    /// <param name="tabHeader">Tab header</param>
    /// <param name="content">Content</param>
    void AddTabItem(string tabHeader, Control content);

    /// <summary>
    /// Remove a tab item
    /// </summary>
    /// <param name="tabItem">Item to remove</param>
    void RemoveTabItem(TabItem tabItem);
  }
}
