using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Org.Vs.TailForWin.Business.Services.Data;
using Org.Vs.TailForWin.Core.Data.Settings;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.Data.Messages;
using Org.Vs.TailForWin.PlugIns.LogWindowModule;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Interfaces;
using Org.Vs.TailForWin.UI.UserControls;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils;


namespace Org.Vs.TailForWin.UI.Utils
{
  /// <summary>
  /// BusinessHelper
  /// </summary>
  public static class BusinessHelper
  {
    /// <summary>
    /// List of registered <see cref="DragSupportTabItem"/>
    /// </summary>
    public static readonly ObservableCollection<DragSupportTabItem> TabItemList = new ObservableCollection<DragSupportTabItem>();

    private static readonly object MyLock = new object();

    /// <summary>
    /// Get current tab item list
    /// </summary>
    /// <returns>List of <see cref="DragSupportTabItem"/></returns>
    public static IEnumerable<DragSupportTabItem> GetTabItemList()
    {
      lock ( MyLock )
      {
        return TabItemList;
      }
    }

    /// <summary>
    /// Unregister a <see cref="DragSupportTabItem"/>
    /// </summary>
    /// <param name="tabItem"><see cref="DragSupportTabItem"/></param>
    public static void UnregisterTabItem(DragSupportTabItem tabItem)
    {
      lock ( MyLock )
      {
        if ( !TabItemList.Contains(tabItem) )
          return;

        TabItemList.Remove(tabItem);

        var control = tabItem.Content as ILogWindowControl;
        control?.DisposeAsync().GetAwaiter();
      }
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
    public static DragSupportTabItem CreateDragSupportTabItem(string header, string toolTip, Visibility busyIndicator, ILogWindowControl content = null, string backgroundColor = DefaultEnvironmentSettings.TabItemHeaderBackgroundColor)
    {
      lock ( MyLock )
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
          logWindowControl = SetLogWindowControl(content, tabItem);

          if ( content.TailReader.IsBusy )
            logWindowControl.TailReader.StartTail();
        }

        tabItem.Content = logWindowControl;
        TabItemList.Add(tabItem);

        return tabItem;
      }
    }

    private static ILogWindowControl SetLogWindowControl(ILogWindowControl content, DragSupportTabItem tabItem)
    {
      ILogWindowControl logWindowControl = new LogWindowControl
      {
        LogWindowTabItem = tabItem,
        LogWindowState = content.LogWindowState,
        FileIsValid = content.FileIsValid,
        SelectedItem = content.SelectedItem,
        SplitterPosition = content.SplitterPosition,
        CurrentTailData = content.CurrentTailData,
        IsSmartWatchAutoRun = content.IsSmartWatchAutoRun
      };

      if ( content.CurrentTailData.IsWindowsEvent )
        logWindowControl.SetWindowsEventTailReader();

      logWindowControl.SplitWindow.LogEntries = content.SplitWindow.LogEntries ?? new ObservableCollection<LogEntry>();
      logWindowControl.SplitWindow.SelectedItem = content.SplitWindow.SelectedItem;
      logWindowControl.SplitWindow.FloodData = content.SplitWindow.FloodData;
      logWindowControl.TailReader.TailData = content.CurrentTailData;
      logWindowControl.SplitWindow.CollectionView = content.SplitWindow.CollectionView;

      logWindowControl.TailReader.SetIndex(content.TailReader.Index);
      logWindowControl.SplitWindow.CacheManager.SetCacheData(content.SplitWindow.CacheManager.GetCacheData());


      return logWindowControl;
    }

    /// <summary>
    /// Create icon of type <see cref="BitmapImage"/>
    /// </summary>
    /// <param name="url">Url</param>
    /// <returns><see cref="BitmapImage"/></returns>
    public static BitmapImage CreateBitmapIcon(string url)
    {
      lock ( MyLock )
      {
        var icon = new BitmapImage();
        icon.BeginInit();
        icon.UriSource = new Uri(url, UriKind.Relative);
        icon.EndInit();

        RenderOptions.SetBitmapScalingMode(icon, BitmapScalingMode.NearestNeighbor);
        RenderOptions.SetEdgeMode(icon, EdgeMode.Aliased);

        return icon;
      }
    }

    /// <summary>
    /// Create popup window
    /// </summary>
    /// <param name="alert">Alert title</param>
    /// <param name="detail">Alert message</param>
    public static void CreatePopUpWindow(string alert, string detail)
    {
      var alertPopUp = new FancyNotificationPopUp
      {
        PopUpAlert = alert,
        PopUpAlertDetail = detail
      };
      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new ShowNotificationPopUpMessage(alertPopUp));
    }
  }
}
