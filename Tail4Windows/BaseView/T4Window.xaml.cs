using System;
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


namespace Org.Vs.TailForWin.BaseView
{
  /// <summary>
  /// Interaction logic for T4Window.xaml
  /// </summary>
  public partial class T4Window
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(T4Window));


    /// <summary>
    /// Standard constructor
    /// </summary>
    public T4Window()
    {
      InitializeComponent();

      SourceInitialized += T4WindowSourceInitialized;

      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<ShowNotificationPopUpMessage>(PopUpVisibilityChanged);
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<OpenSettingsDialogMessage>(OpenSettingsDialog);
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

      SettingsHelperController.CurrentSettings.CurrentWindowState = MainWindow.WindowState;

      if ( WindowState != WindowState.Normal )
        return;

      SettingsHelperController.CurrentSettings.WindowHeight = SettingsHelperController.CurrentSettings.RestoreWindowSize ? MainWindow.Height : -1;
      SettingsHelperController.CurrentSettings.WindowWidth = SettingsHelperController.CurrentSettings.RestoreWindowSize ? MainWindow.Width : -1;

      SettingsHelperController.CurrentSettings.WindowPositionX = SettingsHelperController.CurrentSettings.SaveWindowPosition ? MainWindow.Left : -1;
      SettingsHelperController.CurrentSettings.WindowPositionY = SettingsHelperController.CurrentSettings.SaveWindowPosition ? MainWindow.Top : -1;
    }

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
  }
}
