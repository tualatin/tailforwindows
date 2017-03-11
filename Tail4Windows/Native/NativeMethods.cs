using System;
using System.Runtime.InteropServices;
using Org.Vs.TailForWin.Data;


namespace Org.Vs.TailForWin.Native
{
  /// <summary>
  /// Native windows methods for .NET
  /// </summary>
  public class NativeMethods
  {
    /// <summary>
    /// HWND_BROADCAST
    /// </summary>
    internal const int HWND_BROADCAST = 0xffff;

    /// <summary>
    /// Activates the window and displays it in its current size and position.
    /// </summary>
    internal const uint SW_SHOW = 5;

    /// <summary>
    /// Maximizes the specified window.
    /// </summary>
    internal const uint SW_MAXIMIZE = 3;

    /// <summary>
    /// Activates and displays the window. If the window is minimized or maximized, the system restores it to its original size and position. 
    /// An application should specify this flag when restoring a minimized window.
    /// </summary>
    internal const uint SW_RESTORE = 9;

    /// <summary>
    /// The system sends the WM_ENTERSIZEMOVE message regardless of whether the dragging of full windows is enabled.
    /// </summary>
    internal const int WM_ENTERSIZEMOVE = 0x0231;

    /// <summary>
    /// A window receives this message through its WindowProc function.
    /// </summary>
    internal const int WM_EXITSIZEMOVE = 0x0232;

    /// <summary>
    /// Sent after a window has been moved.
    /// </summary>
    internal const int WM_MOVE = 0x0003;

    /// <summary>
    /// The retrieved handle identifies the window below the specified window in the Z order.
    /// </summary>
    internal const int GW_HWNDNEXT = 2;

    /// <summary>
    /// The retrieved handle identifies the window above the specified window in the Z order.
    /// </summary>
    internal const int GW_HWNDPREV = 3;

    /// <summary>
    /// Brings the thread that created the specified window into the foreground and activates the window. 
    /// Keyboard input is directed to the window, and various visual cues are changed for the user. The system assigns a slightly 
    /// higher priority to the thread that created the foreground window than it does to other threads. 
    /// </summary>
    /// <param name="hWnd">A handle to the window that should be activated and brought to the foreground. </param>
    /// <returns>If the window was brought to the foreground, the return value is <c>nonzero</c>. 
    /// If the window was not brought to the foreground, the return value is <c>zero</c>.
    /// </returns>
    [DllImport("User32.dll")]
    internal static extern Int32 SetForegroundWindow(IntPtr hWnd);

    /// <summary>
    /// Brings the specified window to the top of the Z order. If the window is a top-level window, it is activated. 
    /// If the window is a child window, the top-level parent window associated with the child window is activated. 
    /// </summary>
    /// <param name="hWnd">A handle to the window to bring to the top of the Z order.</param>
    /// <returns>If the function succeeds, the return value is nonzero.
    /// If the function fails, the return value is zero.To get extended error information, call <c>GetLastError</c>. 
    /// </returns>
    /// <remarks>Use the BringWindowToTop function to uncover any window that is partially or completely obscured by other windows. 
    /// Calling this function is similar to calling the SetWindowPos function to change a window's position in the Z order. 
    /// BringWindowToTop does not make a window a top-level window. 
    /// </remarks>
    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool BringWindowToTop(IntPtr hWnd);

    /// <summary>
    /// Brings the specified window to the top of the Z order. If the window is a top-level window, it is activated. 
    /// If the window is a child window, the top-level parent window associated with the child window is activated. 
    /// </summary>
    /// <param name="hWnd">A handle to the window to bring to the top of the Z order.</param>
    /// <returns>If the function succeeds, the return value is nonzero.
    /// If the function fails, the return value is zero.To get extended error information, call <c>GetLastError</c>. 
    /// </returns>
    /// <remarks>Use the BringWindowToTop function to uncover any window that is partially or completely obscured by other windows. 
    /// Calling this function is similar to calling the SetWindowPos function to change a window's position in the Z order. 
    /// BringWindowToTop does not make a window a top-level window. 
    /// </remarks>
    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool BringWindowToTop(HandleRef hWnd);

    /// <summary>
    /// Sets the specified window's show state.
    /// </summary>
    /// <param name="hWnd">A handle to the window. </param>
    /// <param name="nCmdShow">Controls how the window is to be shown. This parameter is ignored the first time an application calls ShowWindow, 
    /// if the program that launched the application provides a <c>STARTUPINFO</c> structure. Otherwise, the first time ShowWindow is called,
    /// the value should be the value obtained by the WinMain function in its nCmdShow parameter. In subsequent calls, this parameter can be one of the following values.</param>
    /// <returns>If the window was previously visible, the return value is <c>nonzero</c>. 
    /// If the window was previously hidden, the return value is <c>zero</c>. 
    /// </returns>
    [DllImport("user32.dll")]
    internal static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

    /// <summary>
    /// Determines whether a window is maximized. 
    /// </summary>
    /// <param name="hWnd">A handle to the window to be tested.</param>
    /// <returns>If the window is zoomed, the return value is <c>nonzero</c>.
    /// If the window is not zoomed, the return value is <c>zero</c>.
    /// </returns>
    [DllImport("user32.dll")]
    internal static extern bool IsZoomed(IntPtr hWnd);

    /// <summary>
    /// Sets the last-error code for the calling thread.
    /// </summary>
    /// <param name="lpBuffer">The last-error code for the thread.</param>
    /// <returns>This function does not return a value.</returns>
    /// <remarks>The last-error code is kept in thread local storage so that multiple threads do not overwrite each other's values.
    /// Most functions call SetLastError or SetLastErrorEx only when they fail.However, some system functions call SetLastError or SetLastErrorEx under conditions of success;
    /// those cases are noted in each function's documentation. Applications can optionally retrieve the value set by this function by using the GetLastError function 
    /// immediately after a function fails. Error codes are 32-bit values(bit 31 is the most significant bit). Bit 29 is reserved for application-defined error codes; 
    /// no system error code has this bit set. If you are defining an error code for your application, set this bit to indicate that the error code has been defined by your 
    /// application and to ensure that your error code does not conflict with any system-defined error codes.
    /// </remarks>
    [DllImport("Kernel32.dll", SetLastError = true)]
    internal static extern bool GlobalMemoryStatusEx([In, Out] MemoryObject lpBuffer);

    /// <summary>
    /// Retrieves a handle to the desktop window. The desktop window covers the entire screen. The desktop window is the area on top of which other windows are painted.
    /// </summary>
    /// <returns>The return value is a handle to the desktop window.</returns>
    [DllImport("user32.dll")]
    internal static extern IntPtr GetDesktopWindow();

    /// <summary>
    /// Defines a system-wide hot key.
    /// </summary>
    /// <param name="hWnd">A handle to the window that will receive <code>WM_HOTKEY</code> messages generated by the hot key. If this parameter is NULL, <code>WM_HOTKEY</code> 
    /// messages are posted to the message queue of the calling thread and must be processed in the message loop.</param>
    /// <param name="id">The identifier of the hot key. If the hWnd parameter is NULL, then the hot key is associated with the current thread rather than with a particular window.
    /// If a hot key already exists with the same hWnd and id parameters, see Remarks for the action taken.</param>
    /// <param name="fsModifiers">The keys that must be pressed in combination with the key specified by the <c>uVirtKey</c> parameter in order to generate the <code>WM_HOTKEY</code>
    /// message. The <c>fsModifiers</c> parameter can be a combination of the following values.</param>
    /// <param name="vk">The virtual-key code of the hot key. See Virtual Key Codes.</param>
    /// <returns>If the function succeeds, the return value is <c>nonzero</c>. 
    /// If the function fails, the return value is <c>zero</c>. To get extended error information, call GetLastError.</returns>
    /// <remarks>When a key is pressed, the system looks for a match against all hot keys. Upon finding a match, the system posts the WM_HOTKEY message to the message queue of the window
    /// with which the hot key is associated. If the hot key is not associated with a window, then the WM_HOTKEY message is posted to the thread associated with the hot key. 
    /// This function cannot associate a hot key with a window created by another thread. RegisterHotKey fails if the keystrokes specified for the hot key have already been 
    /// registered by another hot key. If a hot key already exists with the same hWnd and id parameters, it is maintained along with the new hot key.The application must explicitly call 
    /// UnregisterHotKey to unregister the old hot key. Windows Server 2003:  If a hot key already exists with the same hWnd and id parameters, it is replaced by the new hot key.
    /// The F12 key is reserved for use by the debugger at all times, so it should not be registered as a hot key.Even when you are not debugging an application, F12 is reserved in 
    /// case a kernel-mode debugger or a just-in-time debugger is resident. An application must specify an id value in the range 0x0000 through 0xBFFF. A shared DLL must specify a 
    /// value in the range 0xC000 through 0xFFFF (the range returned by the GlobalAddAtom function). To avoid conflicts with hot-key identifiers defined by other shared DLLs, a DLL 
    /// should use the GlobalAddAtom function to obtain the hot-key identifier.
    /// </remarks>
    [DllImport("User32.dll")]
    internal static extern bool RegisterHotKey([In] IntPtr hWnd, [In] int id, [In] uint fsModifiers, [In] uint vk);

    /// <summary>
    /// Frees a hot key previously registered by the calling thread. 
    /// </summary>
    /// <param name="hWnd">A handle to the window associated with the hot key to be freed. This parameter should be <code>NULL</code> if the hot key is not associated 
    /// with a window.</param>
    /// <param name="id">The identifier of the hot key to be freed.</param>
    /// <returns>If the function succeeds, the return value is <c>nonzero</c>.
    /// If the function fails, the return value is <c>zero</c>.To get extended error information, call <code>GetLastError</code>.</returns>
    [DllImport("User32.dll")]
    internal static extern bool UnregisterHotKey([In] IntPtr hWnd, [In] int id);

    /// <summary>
    /// Retrieves a handle to a window that has the specified relationship (Z-Order or owner) to the specified window.
    /// </summary>
    /// <param name="hWnd">A handle to a window. The window handle retrieved is relative to this window, based on the value of the uCmd parameter.</param>
    /// <param name="wCmd">The relationship between the specified window and the window whose handle is to be retrieved. This parameter can be one of the following values.</param>
    /// <returns>If the function succeeds, the return value is a window handle. If no window exists with the specified relationship to the specified window, the return value is NULL. 
    /// To get extended error information, call GetLastError.</returns>
    [DllImport("User32", SetLastError = true)]
    internal static extern IntPtr GetWindow(IntPtr hWnd, uint wCmd);

    /// <summary>
    /// Retrieves the position of the mouse cursor, in screen coordinates.
    /// </summary>
    /// <param name="pt">A pointer to a POINT structure that receives the screen coordinates of the cursor.</param>
    /// <returns>Returns nonzero if successful or zero otherwise. To get extended error information, call GetLastError.</returns>
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetCursorPos(ref Win32Point pt);
  }
}
