using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using log4net;
using Org.Vs.TailForWin.Business.Data.Messages;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Native;
using Org.Vs.TailForWin.Core.Native.Data;
using Org.Vs.TailForWin.Core.Native.Data.Enum;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.UI.Services;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils.Interfaces;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils.Utils;


namespace Org.Vs.TailForWin.BaseView
{
  /// <summary>
  /// Interaction logic for T4Window.xaml
  /// </summary>
  public partial class T4Window : IDragDropToTabWindow, IDragWindow
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(T4Window));

    private DropOverlayWindow _overlayWindow;

    /// <summary>
    /// TabItem source
    /// </summary>
    // ReSharper disable once UnassignedGetOnlyAutoProperty
    public ObservableCollection<DragSupportTabItem> TabItems
    {
      get;
      set;
    }


    /// <summary>
    /// Standard constructor
    /// </summary>
    public T4Window()
    {
      InitializeComponent();

      DragWindowManager.Instance.Register(this);
      SourceInitialized += T4WindowSourceInitialized;

      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<ShowNotificationPopUpMessage>(PopUpVisibilityChanged);
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<OpenSettingsDialogMessage>(OpenSettingsDialog);

      IsParent = true;
    }

    #region Events

    private void MainWindowClosing(object sender, System.ComponentModel.CancelEventArgs e) => OnExit();

    private void MainWindowStateChanged(object sender, EventArgs e)
    {
      if ( SettingsHelperController.CurrentSettings.CurrentWindowState != WindowState.Maximized || WindowState != WindowState.Normal )
        return;

      MainWindow.Width = (int) SettingsHelperController.CurrentSettings.WindowWidth == -1 ? 800 : SettingsHelperController.CurrentSettings.WindowWidth;
      MainWindow.Height = (int) SettingsHelperController.CurrentSettings.WindowHeight == -1 ? 400 : SettingsHelperController.CurrentSettings.WindowHeight;

      MainWindow.Left = SettingsHelperController.CurrentSettings.WindowPositionX;
      MainWindow.Top = SettingsHelperController.CurrentSettings.WindowPositionY;
    }

    private void T4WindowSourceInitialized(object sender, EventArgs e)
    {
      var handle = new WindowInteropHelper(this).Handle;
      var sysMenuHandle = NativeMethods.GetSystemMenu(handle, false);

      NativeMethods.InsertMenu(sysMenuHandle, 5, NativeMethods.MF_BYPOSITION | NativeMethods.MF_SEPARATOR, 0, string.Empty);
      NativeMethods.InsertMenu(sysMenuHandle, 6, NativeMethods.MF_BYPOSITION, 1000, Application.Current.TryFindResource("OptionsSystemMenu").ToString());

      HwndSource source = HwndSource.FromHwnd(handle);
      source?.AddHook(WndProc);
    }

    #endregion

    #region HelperFunctions

    private void OnExit()
    {
      LOG.Trace("Try to save window size, position and state");

      MouseService.SetBusyState();
      DragWindowManager.Instance.Clear();

      SettingsHelperController.CurrentSettings.CurrentWindowState = MainWindow.WindowState;

      if ( WindowState != WindowState.Normal )
        return;

      SettingsHelperController.CurrentSettings.WindowHeight = SettingsHelperController.CurrentSettings.RestoreWindowSize ? MainWindow.Height : -1;
      SettingsHelperController.CurrentSettings.WindowWidth = SettingsHelperController.CurrentSettings.RestoreWindowSize ? MainWindow.Width : -1;

      SettingsHelperController.CurrentSettings.WindowPositionX = SettingsHelperController.CurrentSettings.SaveWindowPosition ? MainWindow.Left : -1;
      SettingsHelperController.CurrentSettings.WindowPositionY = SettingsHelperController.CurrentSettings.SaveWindowPosition ? MainWindow.Top : -1;
    }

    // ReSharper disable once RedundantAssignment
    private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
      handled = false;

      switch ( msg )
      {
      case NativeMethods.WM_SYSCOMMAND:

        if ( wParam.ToInt32() == 1000 )
        {
          OpenSettingsDialog();
          handled = true;
        }
        break;

      case NativeMethods.WM_ENTERSIZEMOVE:

        break;

      case NativeMethods.WM_EXITSIZEMOVE:

        break;

      case NativeMethods.WM_MOVE:

        break;

      case NativeMethods.WM_GETMINMAXINFO:

        WindowGetMinMaxInfo.WmGetMinMaxInfo(hWnd, lParam);
        handled = true;
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
        handled = true;
        break;
      }
      return IntPtr.Zero;
    }

    private void OpenSettingsDialog()
    {
      var options = new Options
      {
        Owner = this
      };
      options.ShowDialog();
    }

    #endregion

    #region Messages

    private void OpenSettingsDialog(OpenSettingsDialogMessage args)
    {
      if ( args == null )
        return;

      if ( args.OpenSettings )
        OpenSettingsDialog();
    }

    private void PopUpVisibilityChanged(ShowNotificationPopUpMessage args)
    {
      if ( args == null )
        return;

      try
      {
        TbIcon.ShowCustomBalloon(args.Balloon, args.Animation, args.Timeout);
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    #endregion

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
        _overlayWindow.Left = Left - 7;
        _overlayWindow.Top = Top;
      }
      _overlayWindow.Width = ActualWidth + 14;
      _overlayWindow.Height = ActualHeight - 15;
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

    /// <summary>
    /// Add TabItem
    /// </summary>
    /// <param name="tabItem"><see cref="DragSupportTabItem"/></param>
    public void AddTabItem(DragSupportTabItem tabItem) => EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new AddTabItemMessage(this, tabItem));

    /// <summary>
    /// Remove TabItem
    /// </summary>
    /// <param name="tabItem"><see cref="DragSupportTabItem"/></param>
    /// <exception cref="NotImplementedException"></exception>
    public void RemoveTabItem(DragSupportTabItem tabItem) => throw new NotImplementedException();
  }
}
