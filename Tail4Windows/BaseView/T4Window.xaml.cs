using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Org.Vs.TailForWin.Core.Native;
using Org.Vs.TailForWin.Core.Native.Data;
using Org.Vs.TailForWin.Core.Native.Data.Enum;

using Rect = Org.Vs.TailForWin.Core.Native.Data.Rect;


namespace Org.Vs.TailForWin.BaseView
{
  /// <summary>
  /// Interaction logic for T4Window.xaml
  /// </summary>
  public partial class T4Window
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public T4Window()
    {
      InitializeComponent();

      SourceInitialized += T4WindowSourceInitialized;
    }

    #region Events

    private void T4WindowSourceInitialized(object sender, EventArgs e)
    {
      HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
      source?.AddHook(WndProc);
    }

    #endregion

    #region HelperFunctions

    private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
      handled = false;

      switch( msg )
      {
      case NativeMethods.WM_ENTERSIZEMOVE:

        break;

      case NativeMethods.WM_EXITSIZEMOVE:

        break;

      case NativeMethods.WM_MOVE:

        break;

      case NativeMethods.WM_GETMINMAXINFO:

        WmGetMinMaxInfo(hWnd, lParam);
        handled = true;
        break;

      case NativeMethods.WM_WINDOWPOSCHANGING:

        WINDOWPOS pos = (WINDOWPOS) Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));

        if( (pos.flags & (int) SWP.NOMOVE) != 0 )
          return IntPtr.Zero;

        Window wnd = (Window) HwndSource.FromHwnd(hWnd)?.RootVisual;

        if( wnd == null )
          return IntPtr.Zero;

        bool changedPos = false;

        if( pos.cx < MinWidth )
        {
          pos.cx = (int) MinWidth;
          changedPos = true;
        }

        if( pos.cy < MinHeight )
        {
          pos.cy = (int) MinHeight;
          changedPos = true;
        }

        if( !changedPos )
          return IntPtr.Zero;

        Marshal.StructureToPtr(pos, lParam, true);
        handled = true;
        break;
      }
      return IntPtr.Zero;
    }

    /// <summary>
    /// This is required, when the window has own WPF style, it's maximized, that the window hides taskbar
    /// The reason is, the window style <c>None</c>
    /// </summary>
    /// <param name="hwnd">Handle of window</param>
    /// <param name="lParam">Low parameter</param>
    private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
    {
      MINMAXINFO mmi = (MINMAXINFO) Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

      // Adjust the maximized size and position to fit the work area of the correct monitor
      int MONITOR_DEFAULTTONEAREST = 0x00000002;
      IntPtr monitor = NativeMethods.MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

      if( monitor != IntPtr.Zero )
      {
        MonitorInfo monitorInfo = new MonitorInfo();

        NativeMethods.GetMonitorInfo(monitor, monitorInfo);

        Rect rcWorkArea = monitorInfo.rcWork;
        Rect rcMonitorArea = monitorInfo.rcMonitor;
        mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
        mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
        mmi.ptMaxSize.X = Math.Abs(rcWorkArea.right - rcWorkArea.left);
        mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
      }

      Marshal.StructureToPtr(mmi, lParam, true);
    }

    #endregion

    private void Window_Initialized()
    {

    }
  }
}
