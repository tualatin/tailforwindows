using System.Windows;
using System.Windows.Interop;
using Org.Vs.TailForWin.Core.Native;


namespace Org.Vs.TailForWin.Ui.Utils.Extensions
{
  /// <summary>
  /// Window extension
  /// </summary>
  public static class WindowExtension
  {
    private const int WsExDlgmodalframe = 0x0001;

    /// <summary>
    /// Hides the minimize and maximize buttons
    /// </summary>
    /// <param name="window">The window, where the buttons are hide</param>
    public static void HideMinimizeMaximizeButtons(this Window window)
    {
      var hWnd = new WindowInteropHelper(window).Handle;
      int currentStyle = NativeMethods.GetWindowLong(hWnd, NativeMethods.GWL_STYLE);

      NativeMethods.SetWindowLong(hWnd, NativeMethods.GWL_STYLE, currentStyle & ~NativeMethods.WS_MINIMIZEBOX & ~NativeMethods.WS_MAXIMIZEBOX);
    }
  }
}
