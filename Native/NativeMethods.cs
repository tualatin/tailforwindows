using System;
using System.Runtime.InteropServices;
using TailForWin.Data;


namespace TailForWin.Native
{
  /// <summary>
  /// Native windows methods for .NET
  /// </summary>
  public class NativeMethods
  {
    /// <summary>
    /// hWnd broadcast
    /// </summary>
    public const int HWND_BROADCAST = 0xffff;

    /// <summary>
    /// sw_show flag
    /// </summary>
    public const uint SW_SHOW = 5;

    /// <summary>
    /// sw_maximize flag
    /// </summary>
    public const uint SW_MAXIMIZE = 3;

    /// <summary>
    /// sw_restore flag
    /// </summary>
    public const uint SW_RESTORE = 9;


    [DllImport ("User32.dll")]
    public static extern Int32 SetForegroundWindow (IntPtr hWnd);

    [DllImport ("user32.dll", SetLastError = true)]
    public static extern bool BringWindowToTop (IntPtr hWnd);

    [DllImport ("user32.dll", SetLastError = true)]
    public static extern bool BringWindowToTop (HandleRef hWnd);

    [DllImport ("user32.dll")]
    public static extern bool ShowWindow (IntPtr hWnd, uint nCmdShow);

    [DllImport ("user32.dll")]
    public static extern bool IsZoomed (IntPtr hWnd);

    [DllImport ("Kernel32.dll", SetLastError = true)]
    public static extern bool GlobalMemoryStatusEx ([In, Out] MemoryObject lpBuffer);

    [DllImport ("user32.dll")]
    public static extern IntPtr GetDesktopWindow ( );
  }
}
