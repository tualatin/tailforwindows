﻿namespace Org.Vs.TailForWin.Core.Native.Data.Enum
{
  /// <summary>
  /// Set window position flags
  /// </summary>
  public enum SWP : uint
  {
    /// <summary>
    /// Retains the current size (ignores the cx and cy members).
    /// </summary>
    NOSIZE = 0x0001,

    /// <summary>
    /// Retains the current position (ignores the x and y members).
    /// </summary>
    NOMOVE = 0x0002,

    /// <summary>
    /// Retains the current Z order (ignores the hwndInsertAfter member).
    /// </summary>
    NOZORDER = 0x0004,

    /// <summary>
    /// Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), 
    /// and any part of the parent window uncovered as a result of the window being moved. When this flag is set, the application must explicitly invalidate or 
    /// redraw any parts of the window and parent window that need redrawing.
    /// </summary>
    NOREDRAW = 0x0008,

    /// <summary>
    /// Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group 
    /// (depending on the setting of the hwndInsertAfter member).
    /// </summary>
    NOACTIVATE = 0x0010,

    /// <summary>
    /// Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when 
    /// the window's size is being changed.
    /// </summary>
    FRAMECHANGED = 0x0020,

    /// <summary>
    /// Displays the window.
    /// </summary>
    SHOWWINDOW = 0x0040,

    /// <summary>
    /// Hides the window.
    /// </summary>
    HIDEWINDOW = 0x0080,

    /// <summary>
    /// Discards the entire contents of the client area. If this flag is not specified, the valid contents of the client area are saved and copied back into the client
    /// area after the window is sized or repositioned.
    /// </summary>
    NOCOPYBITS = 0x0100,

    /// <summary>
    /// Does not change the owner window's position in the Z order.
    /// </summary>
    NOOWNERZORDER = 0x0200,

    /// <summary>
    /// Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
    /// </summary>
    NOSENDCHANGING = 0x0400,
  }
}
