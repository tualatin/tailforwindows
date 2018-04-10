using System.Collections.Generic;
using System.Windows;
using Org.Vs.TailForWin.PlugIns.LogWindowModule;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Interfaces;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils;


namespace Org.Vs.TailForWin.Business.Utils
{
  /// <summary>
  /// BusinessHelper
  /// </summary>
  public static class BusinessHelper
  {
    private static readonly List<DragSupportTabItem> TabItemList = new List<DragSupportTabItem>();

    /// <summary>
    /// Get current tab item list
    /// </summary>
    /// <returns>List of <see cref="DragSupportTabItem"/></returns>
    public static List<DragSupportTabItem> GetTabItemList() => TabItemList;

    /// <summary>
    /// Unregister a <see cref="DragSupportTabItem"/>
    /// </summary>
    /// <param name="tabItem"><see cref="DragSupportTabItem"/></param>
    public static void UnregisterTabItem(DragSupportTabItem tabItem)
    {
      if ( TabItemList.Contains(tabItem) )
        TabItemList.Remove(tabItem);
    }

    /// <summary>
    /// Creates a <see cref="DragSupportTabItem"/>
    /// </summary>
    /// <param name="header">Name of header</param>
    /// <param name="toolTip">ToolTip</param>
    /// <param name="busyIndicator">State of busy indicator</param>
    /// <param name="content">Content as <see cref="LogWindowControl"/></param>
    /// <param name="backgroundColor">BackgroundColor as hex string</param>
    /// <returns><see cref="DragSupportTabItem"/></returns>
    public static DragSupportTabItem CreateDragSupportTabItem(string header, string toolTip, Visibility busyIndicator, ILogWindowControl content = null, string backgroundColor = "#FFD6DBE9")
    {
      var tabItem = new DragSupportTabItem
      {
        HeaderContent = header,
        IsSelected = true,
        HeaderToolTip = toolTip,
        TabItemBackgroundColorStringHex = backgroundColor,
        TabItemBusyIndicator = busyIndicator
      };

      ILogWindowControl logWindowControl;

      if ( content == null )
      {
        logWindowControl = new LogWindowControl
        {
          LogWindowTabItem = tabItem
        };
      }
      else
      {
        logWindowControl = new LogWindowControl
        {
          LogWindowTabItem = tabItem,
          CurrenTailData = content.CurrenTailData,
          LogWindowState = content.LogWindowState,
          FileIsValid = content.FileIsValid
        };
      }

      tabItem.Content = logWindowControl;
      TabItemList.Add(tabItem);

      return tabItem;
    }
  }
}
