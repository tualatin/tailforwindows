using System.Runtime.InteropServices;
using Org.Vs.Tail4Win.Core.Native;
using Org.Vs.Tail4Win.Core.Native.Data;

namespace Org.Vs.Tail4Win.Core.Utils
{
  /// <summary>
  /// WindowGetMinMaxInfo
  /// </summary>
  public class WindowGetMinMaxInfo
  {
    /// <summary>
    /// This is required, when the window has own WPF style, it's maximized, that the window hides taskbar
    /// The reason is, the window style <c>None</c>
    /// </summary>
    /// <param name="hwnd">Handle of window</param>
    /// <param name="lParam">Low parameter</param>
    public static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
    {
      MINMAXINFO mmi = (MINMAXINFO) Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

      // Adjust the maximized size and position to fit the work area of the correct monitor
      int MONITOR_DEFAULTTONEAREST = 0x00000002;
      var monitor = NativeMethods.MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

      if ( monitor != IntPtr.Zero )
      {
        MonitorInfo monitorInfo = new MonitorInfo();

        NativeMethods.GetMonitorInfo(monitor, monitorInfo);

        var rcWorkArea = monitorInfo.rcWork;
        var rcMonitorArea = monitorInfo.rcMonitor;
        mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
        mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
        mmi.ptMaxSize.X = Math.Abs(rcWorkArea.right - rcWorkArea.left);
        mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
      }

      Marshal.StructureToPtr(mmi, lParam, true);
    }
  }
}
