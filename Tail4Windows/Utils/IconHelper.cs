using System;
using System.Windows;
using System.Windows.Interop;
using Org.Vs.TailForWin.Native;


namespace Org.Vs.TailForWin.Utils
{
  /// <summary>
  /// Icon Helper to remove icons from WPF window
  /// </summary>
  public static class IconHelper
  {
    /// <summary>
    /// Remove NotifyIcon from taskbar
    /// </summary>
    /// <param name="window">Current window</param>
    /// <exception cref="ArgumentException">If window is null</exception>
    public static void RemoveIcon(Window window)
    {
      Arg.NotNull(window, "Window");

      // Get this window's handle
      IntPtr hwnd = new WindowInteropHelper(window).Handle;

      // Change the extended window style to not show a window icon
      int extendedStyle = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE);
      NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE, extendedStyle | NativeMethods.WS_EX_DLGMODALFRAME);

      // Update the window's non-client area to reflect the changes
      NativeMethods.SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOZORDER | NativeMethods.SWP_FRAMECHANGED);
    }
  }
}