﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using log4net;
using Org.Vs.TailForWin.Business.Services.Data;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Core.Data.Settings;
using Org.Vs.TailForWin.Data.Messages;
using Org.Vs.TailForWin.PlugIns.LogWindowModule;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Interfaces;
using Org.Vs.TailForWin.UI.UserControls;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils;


namespace Org.Vs.TailForWin.UI.Utils
{
  /// <summary>
  /// UiHelper
  /// </summary>
  public static class UiHelper
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(UiHelper));

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

          if ( content.TailReader != null && content.TailReader.IsBusy )
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
        FileIsValid = content.FileIsValid,
        CurrentTailData = content.CurrentTailData,
        LogWindowState = content.LogWindowState,
        SplitterPosition = content.SplitterPosition,
        IsSmartWatchAutoRun = content.IsSmartWatchAutoRun
      };

      if ( content.CurrentTailData.IsWindowsEvent )
        logWindowControl.SetWindowsEventTailReader(content.CurrentTailData);
      else
        logWindowControl.SelectedItem = content.SelectedItem;

      if ( content.SplitWindow != null )
      {
        logWindowControl.SplitWindow.LogEntries = content.SplitWindow.LogEntries ?? new ObservableCollection<LogEntry>();
        logWindowControl.SplitWindow.SelectedItem = content.SplitWindow.SelectedItem;
        logWindowControl.SplitWindow.FloodData = content.SplitWindow.FloodData;
        logWindowControl.SplitWindow.CollectionView = content.SplitWindow.CollectionView;
        logWindowControl.SplitWindow.HighlightData = content.SplitWindow.HighlightData;
        logWindowControl.SplitWindow.ExtendedToolbarVisibility = content.SplitWindow.ExtendedToolbarVisibility;

        logWindowControl.SplitWindow.CacheManager.SetCacheData(content.SplitWindow.CacheManager.GetCacheData());
      }

      if ( content.TailReader == null )
        return logWindowControl;

      logWindowControl.TailReader.TailData = content.CurrentTailData;
      logWindowControl.TailReader.SetIndex(content.TailReader.Index);
      logWindowControl.TailReader.SetFileOffset(content.TailReader.GetFileOffset());

      return logWindowControl;
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

    /// <summary>
    /// Move UI into view
    /// </summary>
    /// <param name="name">Name of control to move</param>
    /// <param name="posX">Position X</param>
    /// <param name="posY">Position Y</param>
    /// <param name="width">Width</param>
    /// <param name="height">Height</param>
    public static void MoveIntoView(string name, ref double posX, ref double posY, double width, double height)
    {
      LOG.Trace($"Move {name} into view, if required.");

      if ( posY + height / 2 > SystemParameters.VirtualScreenHeight )
        posY = SystemParameters.VirtualScreenHeight - height;

      if ( posX + width / 2 > SystemParameters.VirtualScreenWidth )
        posX = SystemParameters.VirtualScreenWidth - width;

      if ( posY < 0 )
        posY = 0;

      if ( posX < 0 )
        posX = 0;
    }
  }
}