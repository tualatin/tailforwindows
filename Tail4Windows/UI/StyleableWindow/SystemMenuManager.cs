using System;
using System.Windows;
using System.Windows.Interop;
using Org.Vs.TailForWin.Native;
using Org.Vs.TailForWin.Utils;


namespace Org.Vs.TailForWin.UI.StyleableWindow
{
  /// <summary>
  /// System menu manager
  /// </summary>
  public static class SystemMenuManager
  {
    /// <summary>
    /// Show menu
    /// </summary>
    /// <param name="targetWindow">Target window</param>
    /// <param name="menuLocation">Menu location</param>
    public static void ShowMenu(Window targetWindow, Point menuLocation)
    {
      Arg.NotNull(targetWindow, "TargetWindow");

      int x, y;

      try
      {
        x = Convert.ToInt32(menuLocation.X);
        y = Convert.ToInt32(menuLocation.Y);
      }
      catch ( OverflowException )
      {
        x = 0;
        y = 0;
      }

      IntPtr window = new WindowInteropHelper(targetWindow).Handle;
      IntPtr wMenu = NativeMethods.GetSystemMenu(window, false);
      int command = NativeMethods.TrackPopupMenuEx(wMenu, NativeMethods.TPM_LEFTALIGN | NativeMethods.TPM_RETURNCMD, x, y, window, IntPtr.Zero);

      if ( command == 0 )
        return;

      NativeMethods.PostMessage(window, NativeMethods.WM_SYSCOMMAND, new IntPtr(command), IntPtr.Zero);
    }
  }
}
