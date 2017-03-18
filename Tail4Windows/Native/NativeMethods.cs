using System;
using System.Runtime.InteropServices;
using Org.Vs.TailForWin.Data.Native;


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
    internal const uint HWND_BROADCAST = 0xffff;

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
    /// Sent to a window when the size or position of the window is about to change. An application can use this message to override the window's 
    /// default maximized size and position, or its default minimum or maximum tracking size.
    /// A window receives this message through its WindowProc function.
    /// </summary>
    internal const int WM_GETMINMAXINFO = 0x0024;

    /// <summary>
    /// The retrieved handle identifies the window below the specified window in the Z order.
    /// </summary>
    internal const uint GW_HWNDNEXT = 2;

    /// <summary>
    /// The retrieved handle identifies the window above the specified window in the Z order.
    /// </summary>
    internal const uint GW_HWNDPREV = 3;

    /// <summary>
    /// A window receives this message when the user chooses a command from the Window menu (formerly known as the system or control menu) or when the user chooses the maximize button,
    /// minimize button, restore button, or close button.
    /// </summary>
    internal const uint WM_SYSCOMMAND = 0x112;

    /// <summary>
    /// Sent to a window whose size, position, or place in the Z order is about to change as a result of a call to the SetWindowPos function or another window-management function.
    /// </summary>
    internal const int WM_WINDOWPOSCHANGING = 0x0046;

    /// <summary>
    /// Positions the shortcut menu so that its left side is aligned with the coordinate specified by the x parameter.
    /// </summary>
    internal const uint TPM_LEFTALIGN = 0x0000;

    /// <summary>
    /// The function returns the menu item identifier of the user's selection in the return value.
    /// </summary>
    internal const uint TPM_RETURNCMD = 0x0100;

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

    /// <summary>
    /// Enables the application to access the window menu (also known as the system menu or the control menu) for copying and modifying.
    /// </summary>
    /// <param name="hWnd">A handle to the window that will own a copy of the window menu.</param>
    /// <param name="bRevert">The action to be taken. If this parameter is FALSE, GetSystemMenu returns a handle to the copy of the window menu currently in use. 
    /// The copy is initially identical to the window menu, but it can be modified. If this parameter is TRUE, GetSystemMenu resets the window menu back to the default state.
    /// The previous window menu, if any, is destroyed.</param>
    /// <returns>If the bRevert parameter is <c>FALSE</c>, the return value is a handle to a copy of the window menu. If the bRevert parameter is <c>TRUE</c>, the return value is <c>NULL</c>.</returns>
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

    /// <summary>
    /// Displays a shortcut menu at the specified location and tracks the selection of items on the shortcut menu. The shortcut menu can appear anywhere on the screen.
    /// </summary>
    /// <param name="hmenu">A handle to the shortcut menu to be displayed. This handle can be obtained by calling the CreatePopupMenu function to create a new shortcut menu or
    /// by calling the GetSubMenu function to retrieve a handle to a submenu associated with an existing menu item.</param>
    /// <param name="fuFlags">Specifies function options.</param>
    /// <param name="x">The horizontal location of the shortcut menu, in screen coordinates.</param>
    /// <param name="y">The vertical location of the shortcut menu, in screen coordinates.</param>
    /// <param name="hwnd">A handle to the window that owns the shortcut menu. This window receives all messages from the menu. The window does not receive a WM_COMMAND message from the menu 
    /// until the function returns. If you specify TPM_NONOTIFY in the fuFlags parameter, the function does not send messages to the window identified by hwnd. However, 
    /// you must still pass a window handle in hwnd. It can be any window handle from your application.</param>
    /// <param name="lptpm">A pointer to a TPMPARAMS structure that specifies an area of the screen the menu should not overlap. This parameter can be NULL.</param>
    /// <returns>If you specify TPM_RETURNCMD in the fuFlags parameter, the return value is the menu-item identifier of the item that the user selected. 
    /// If the user cancels the menu without making a selection, or if an error occurs, the return value is zero.</returns>
    [DllImport("user32.dll")]
    internal static extern int TrackPopupMenuEx(IntPtr hmenu, uint fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);

    /// <summary>
    /// Places (posts) a message in the message queue associated with the thread that created the specified window and returns without waiting for the thread to process the message.
    /// To post a message in the message queue associated with a thread, use the PostThreadMessage function.
    /// </summary>
    /// <param name="hWnd">A handle to the window whose window procedure is to receive the message. The following values have special meanings.</param>
    /// <param name="Msg">The message to be posted.
    /// For lists of the system-provided messages, see System-Defined Messages.</param>
    /// <param name="wParam">Additional message-specific information.</param>
    /// <param name="lParam">Additional message-specific information.</param>
    /// <returns>If the function succeeds, the return value is nonzero.
    /// If the function fails, the return value is zero. To get extended error information, call GetLastError. GetLastError returns ERROR_NOT_ENOUGH_QUOTA when the limit is hit.</returns>
    [DllImport("user32.dll")]
    internal static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    /// <summary>
    /// Enables, disables, or grays the specified menu item.
    /// </summary>
    /// <param name="hMenu">A handle to the menu.</param>
    /// <param name="uIDEnableItem">The menu item to be enabled, disabled, or grayed, as determined by the uEnable parameter. This parameter specifies an item in a menu bar, menu, or submenu.</param>
    /// <param name="uEnable">Controls the interpretation of the uIDEnableItem parameter and indicate whether the menu item is enabled, disabled, or grayed. 
    /// This parameter must be a combination of the following values.</param>
    /// <returns>The return value specifies the previous state of the menu item (it is either MF_DISABLED, MF_ENABLED, or MF_GRAYED). If the menu item does not exist, the return value is -1.</returns>
    [DllImport("user32.dll")]
    internal static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

    /// <summary>
    /// The GetMonitorInfo function retrieves information about a display monitor.
    /// </summary>
    /// <param name="hMonitor">A handle to the display monitor of interest.</param>
    /// <param name="lpmi">A pointer to a MONITORINFO or MONITORINFOEX structure that receives information about the specified display monitor.</param>
    /// <returns>If the function succeeds, the return value is nonzero.</returns>
    [DllImport("user32")]
    internal static extern bool GetMonitorInfo(IntPtr hMonitor, MonitorInfo lpmi);

    /// <summary>
    /// The MonitorFromWindow function retrieves a handle to the display monitor that has the largest area of intersection with the bounding rectangle of a specified window.
    /// </summary>
    /// <param name="handle">A handle to the window of interest.</param>
    /// <param name="flags">Determines the function's return value if the window does not intersect any display monitor.</param>
    /// <returns>If the window intersects one or more display monitor rectangles, the return value is an HMONITOR handle to the display monitor that has the largest area of 
    /// intersection with the window.
    /// If the window does not intersect a display monitor, the return value depends on the value of dwFlags.</returns>
    [DllImport("User32")]
    internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

    /// <summary>
    /// Retrieves a handle to the top-level window whose class name and window name match the specified strings. This function does not search child windows. 
    /// This function does not perform a case-sensitive search.
    /// </summary>
    /// <param name="lpClassName">The class name or a class atom created by a previous call to the RegisterClass or RegisterClassEx function. The atom must be in the low
    /// -order word of lpClassName; the high-order word must be zero.
    /// If lpClassName points to a string, it specifies the window class name. The class name can be any name registered with RegisterClass or RegisterClassEx, 
    /// or any of the predefined control-class names.
    /// If lpClassName is NULL, it finds any window whose title matches the lpWindowName parameter.</param>
    /// <param name="lpWindowName">The window name (the window's title). If this parameter is NULL, all window names match.</param>
    /// <returns>If the function succeeds, the return value is a handle to the window that has the specified class name and window name.
    /// If the function fails, the return value is NULL.To get extended error information, call GetLastError.</returns>
    [DllImport("user32.dll", SetLastError = true)]
    internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
  }
}
