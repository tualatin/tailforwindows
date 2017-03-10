using System.Windows.Controls;
using Org.Vs.TailForWin.Data;


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
    /// Add tab item to control when drag and drop window
    /// </summary>
    /// <param name="tabHeader">Tab header</param>
    /// <param name="content">Content</param>
    void AddTabItem(string tabHeader, Control content);

    /// <summary>
    /// Add tab item to control
    /// </summary>
    /// <param name="properties">File Manager data object, <c>default</c> is <c>NULL</c></param>
    void AddTabItem(FileManagerData properties = null);


    /// <summary>
    /// Remove a tab item
    /// </summary>
    /// <param name="tabItem">Item to remove</param>
    void RemoveTabItem(TabItem tabItem);
  }
}
