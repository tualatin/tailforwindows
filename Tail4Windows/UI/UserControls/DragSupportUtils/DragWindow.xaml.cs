﻿using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using log4net;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Core.Native;
using Org.Vs.TailForWin.Core.Native.Data;
using Org.Vs.TailForWin.Core.Native.Data.Enum;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.PlugIns.LogWindowModule;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Interfaces;
using Org.Vs.TailForWin.UI.Commands;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils.Interfaces;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils.Utils;


namespace Org.Vs.TailForWin.UI.UserControls.DragSupportUtils
{
  /// <summary>
  /// Interaction logic for DragWindow.xaml
  /// </summary>
  public partial class DragWindow : IDragWindow, IDragDropToTabWindow
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(DragWindow));

    private bool _hasFocus;
    private DropOverlayWindow _overlayWindow;

    /// <summary>
    /// Selected <see cref="DragSupportTabItem"/>
    /// </summary>
    public DragSupportTabItem SelectedTabItem
    {
      get;
      set;
    }


    /// <summary>
    /// Standard constructor
    /// </summary>
    public DragWindow()
    {
      InitializeComponent();

      TabItems = new ObservableCollection<DragSupportTabItem>();

      DragWindowManager.Instance.Register(this);
      SourceInitialized += DragWindowSourceInitialized;

      IsParent = false;
      DataContext = this;
    }

    private void DragWindowSourceInitialized(object sender, EventArgs e)
    {
      var handle = new WindowInteropHelper(this).Handle;
      HwndSource source = HwndSource.FromHwnd(handle);
      source?.AddHook(WndProc);
    }

    // ReSharper disable once RedundantAssignment
    private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
      handled = false;

      switch ( msg )
      {
      case NativeMethods.WM_ENTERSIZEMOVE:

        _hasFocus = true;
        break;

      case NativeMethods.WM_EXITSIZEMOVE:

        _hasFocus = false;
        DragWindowManager.Instance.DragEnd(this);
        break;

      case NativeMethods.WM_MOVE:

        if ( _hasFocus )
          DragWindowManager.Instance.DragMove(this);
        break;

      case NativeMethods.WM_GETMINMAXINFO:

        WindowGetMinMaxInfo.WmGetMinMaxInfo(hWnd, lParam);
        break;

      case NativeMethods.WM_WINDOWPOSCHANGING:

        WINDOWPOS pos = (WINDOWPOS) Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));

        if ( (pos.flags & (int) SWP.NOMOVE) != 0 )
          return IntPtr.Zero;

        Window wnd = (Window) HwndSource.FromHwnd(hWnd)?.RootVisual;

        if ( wnd == null )
          return IntPtr.Zero;

        bool changedPos = false;

        if ( pos.cx < MinWidth )
        {
          pos.cx = (int) MinWidth;
          changedPos = true;
        }

        if ( pos.cy < MinHeight )
        {
          pos.cy = (int) MinHeight;
          changedPos = true;
        }

        if ( !changedPos )
          return IntPtr.Zero;

        Marshal.StructureToPtr(pos, lParam, true);
        break;
      }
      return IntPtr.Zero;
    }

    /// <summary>
    /// Creates a new <see cref="DragWindow"/>
    /// </summary>
    /// <param name="left">Left</param>
    /// <param name="top">Top</param>
    /// <param name="width">Width</param>
    /// <param name="height">Height</param>
    /// <param name="tabItem"><see cref="DragSupportTabItem"/></param>
    /// <returns><see cref="DragWindow"/></returns>
    public static DragWindow CreateTabWindow(double left, double top, double width, double height, DragSupportTabItem tabItem)
    {
      var dragWindow = new DragWindow
      {
        Left = left,
        Top = top,
        Width = width,
        Height = height,
        WindowStartupLocation = WindowStartupLocation.Manual
      };

      dragWindow.Show();
      dragWindow.Activate();
      dragWindow.Focus();

      if ( tabItem != null )
        ((IDragWindow) dragWindow).AddTabItem(tabItem);

      return dragWindow;
    }

    /// <summary>
    /// TabItem source
    /// </summary>
    public ObservableCollection<DragSupportTabItem> TabItems
    {
      get;
      set;
    }

    /// <summary>
    /// Add TabItem
    /// </summary>
    /// <param name="tabItem"><see cref="DragSupportTabItem"/></param>
    public void AddTabItem(DragSupportTabItem tabItem) => AddTabItem(tabItem.HeaderContent, tabItem.HeaderToolTip, tabItem.TabItemBusyIndicator, tabItem.TabItemBackgroundColorStringHex, (LogWindowControl) tabItem.Content);

    /// <summary>
    /// Remove TabItem
    /// </summary>
    /// <param name="tabItem"><see cref="DragSupportTabItem"/></param>
    public void RemoveTabItem(DragSupportTabItem tabItem)
    {
      if ( !TabItems.Contains(tabItem) )
        return;

      if ( SelectedTabItem.TabItemBusyIndicator == Visibility.Visible )
      {
        string message = $"{Application.Current.TryFindResource("QRemoveTab")} \n {SelectedTabItem.HeaderFullText}";

        if ( EnvironmentContainer.ShowQuestionMessageBox(message) == MessageBoxResult.No )
          return;
      }

      tabItem.TabHeaderDoubleClick -= TabItemTabHeaderDoubleClick;
      tabItem.CloseTabWindow -= TabItemCloseTabWindow;

      BusinessHelper.UnregisterTabItem(tabItem);
      TabItems.Remove(tabItem);

      if ( TabItems.Count == 0 )
        Close();
    }

    /// <summary>
    /// Is parent window
    /// </summary>
    public bool IsParent
    {
      get;
    }

    /// <summary>
    /// On Drag enter
    /// </summary>
    public void OnDragEnter()
    {
      if ( _overlayWindow == null )
        _overlayWindow = new DropOverlayWindow();

      if ( WindowState == WindowState.Maximized )
      {
        _overlayWindow.Left = 0;
        _overlayWindow.Top = 0;
      }
      else
      {
        _overlayWindow.Left = Left;
        _overlayWindow.Top = Top;
      }
      _overlayWindow.Width = ActualWidth;
      _overlayWindow.Height = ActualHeight;
      _overlayWindow.Topmost = true;

      _overlayWindow.Show();
    }

    /// <summary>
    /// On Drag leave
    /// </summary>
    public void OnDrageLeave()
    {
      if ( _overlayWindow == null )
        return;

      _overlayWindow.Close();
      _overlayWindow = null;
    }

    /// <summary>
    /// Is drag mouse ober
    /// </summary>
    /// <param name="mousePosition">Current mouse position</param>
    /// <returns>If it is over<c>True</c> otherwise <c>False</c></returns>
    public bool IsDragMouseOver(Point mousePosition)
    {
      if ( WindowState == WindowState.Minimized )
        return false;

      double left, top;

      if ( WindowState == WindowState.Maximized )
      {
        left = 0;
        top = 0;
      }
      else
      {
        left = Left;
        top = Top;
      }

      bool isMouseOver = mousePosition.X > left && mousePosition.X < left + ActualWidth && mousePosition.Y > top && mousePosition.Y < top + ActualHeight;

      return isMouseOver;
    }

    /// <summary>
    /// Is drag mouse over tab zone
    /// </summary>
    /// <param name="mousePosition">Current mouse position</param>
    /// <returns>If it is over <c>True</c> otherwise <c>False</c></returns>
    public bool IsDragMouseOverTabZone(Point mousePosition) => _overlayWindow?.IsMouseOverTabTarget(mousePosition) ?? false;

    #region Events

    private void TabItemTabHeaderDoubleClick(object sender, RoutedEventArgs e)
    {
      LOG.Trace("MouseDoubleClick");
    }

    private void TabItemCloseTabWindow(object sender, RoutedEventArgs e)
    {
      if ( e.Source is DragSupportTabItem item )
        RemoveTabItem(item);
    }

    private void TabControlOnAddTabItemEvent(object sender, RoutedEventArgs e) => AddTabItem($"{Application.Current.TryFindResource("NoFile")}", $"{Application.Current.TryFindResource("NoFile")}", Visibility.Collapsed);

    #endregion

    private void AddTabItem(string header, string toolTip, Visibility busyIndicator, string backgroundColor = "#FFD6DBE9", ILogWindowControl content = null)
    {
      var tabItem = BusinessHelper.CreateDragSupportTabItem(header, toolTip, busyIndicator, content, backgroundColor);

      tabItem.CloseTabWindow += TabItemCloseTabWindow;
      tabItem.TabHeaderDoubleClick += TabItemTabHeaderDoubleClick;

      TabItems.Add(tabItem);
    }

    #region Commands

    private ICommand _addNewTabItemCommand;

    /// <summary>
    /// Add new tab item command
    /// </summary>
    public ICommand AddNewTabItemCommand => _addNewTabItemCommand ?? (_addNewTabItemCommand = new RelayCommand(p =>ExecuteAddNewTabItemCommand()));

    private ICommand _closeTabItemCommand;

    /// <summary>
    /// Add new tab item command
    /// </summary>
    public ICommand CloseTabItemCommand => _closeTabItemCommand ?? (_closeTabItemCommand = new RelayCommand(p => ExecuteCloseTabItemCommand()));

    #endregion

    #region Command functions

    private void ExecuteAddNewTabItemCommand() => AddTabItem($"{Application.Current.TryFindResource("NoFile")}", $"{Application.Current.TryFindResource("NoFile")}", Visibility.Collapsed);

    private void ExecuteCloseTabItemCommand()
    {
      if ( SelectedTabItem == null )
        return;

      RemoveTabItem(SelectedTabItem);
    }

    #endregion
  }
}