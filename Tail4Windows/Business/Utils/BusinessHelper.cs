using System.Windows;
using Org.Vs.TailForWin.PlugIns.LogWindowModule;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils;


namespace Org.Vs.TailForWin.Business.Utils
{
  /// <summary>
  /// BusinessHelper
  /// </summary>
  public static class BusinessHelper
  {
    /// <summary>
    /// Creates a <see cref="DragSupportTabItem"/>
    /// </summary>
    /// <param name="header">Name of header</param>
    /// <param name="toolTip">ToolTip</param>
    /// <param name="busyIndicator">State of busy indicator</param>
    /// <param name="backgroundColor">BackgroundColor as hex string</param>
    /// <param name="content">Content as <see cref="LogWindowControl"/></param>
    /// <returns><see cref="DragSupportTabItem"/></returns>
    public static DragSupportTabItem CreateDragSupportTabItem(string header, string toolTip, Visibility busyIndicator, string backgroundColor = "#FFD6DBE9", LogWindowControl content = null)
    {
      var tabItem = new DragSupportTabItem
      {
        HeaderContent = header,
        IsSelected = true,
        HeaderToolTip = toolTip,
        TabItemBackgroundColorStringHex = backgroundColor,
        TabItemBusyIndicator = busyIndicator
      };

      if ( content != null )
      {
        content.LogWindowTabItem = tabItem;
        tabItem.Content = content;
      }
      else
      {
        tabItem.Content = new LogWindowControl
        {
          LogWindowTabItem = tabItem
        };
      }
      return tabItem;
    }
  }
}
