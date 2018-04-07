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
    /// <summary>
    /// Creates a <see cref="DragSupportTabItem"/>
    /// </summary>
    /// <param name="header">Name of header</param>
    /// <param name="toolTip">ToolTip</param>
    /// <param name="busyIndicator">State of busy indicator</param>
    /// <param name="content">Content as <see cref="LogWindowControl"/></param>
    /// <param name="backgroundColor">BackgroundColor as hex string</param>
    /// <returns><see cref="DragSupportTabItem"/></returns>
    public static DragSupportTabItem CreateDragSupportTabItem(string header, string toolTip, Visibility busyIndicator, ILogWindow content = null, string backgroundColor = "#FFD6DBE9")
    {
      var tabItem = new DragSupportTabItem
      {
        HeaderContent = header,
        IsSelected = true,
        HeaderToolTip = toolTip,
        TabItemBackgroundColorStringHex = backgroundColor,
        TabItemBusyIndicator = busyIndicator
      };

      ILogWindow logWindowControl;

      if ( content == null )
      {
        logWindowControl = new LogWindowControl();
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

      return tabItem;
    }
  }
}
