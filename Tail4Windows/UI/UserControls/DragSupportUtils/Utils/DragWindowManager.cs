using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using log4net;
using Org.Vs.TailForWin.Core.Native;
using Org.Vs.TailForWin.Core.Native.Data;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils.Interfaces;


namespace Org.Vs.TailForWin.UI.UserControls.DragSupportUtils.Utils
{
  /// <summary>
  /// Drag window manager
  /// </summary>
  public class DragWindowManager
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(DragWindow));

    private static DragWindowManager instance;

    /// <summary>
    /// Current instance
    /// </summary>
    public static DragWindowManager Instance => instance ?? (instance = new DragWindowManager());

    private readonly List<IDragDropToTabWindow> _allWindows;
    private readonly List<IDragDropToTabWindow> _dragEnteredWindows;

    private DragWindowManager()
    {
      _allWindows = new List<IDragDropToTabWindow>();
      _dragEnteredWindows = new List<IDragDropToTabWindow>();
    }

    /// <summary>
    /// Register a <see cref="IDragDropToTabWindow"/>
    /// </summary>
    /// <param name="window">Window of <see cref="IDragWindow"/></param>
    public void Register(IDragDropToTabWindow window)
    {
      if ( _allWindows.Contains(window) )
        return;

      _allWindows.Add(window);
      ((Window) window).Closed += DragWindowManagerClosed;
    }

    /// <summary>
    /// Unregister a <see cref="IDragDropToTabWindow"/>
    /// </summary>
    /// <param name="window">Window of <see cref="IDragWindow"/></param>
    public void Unregister(IDragDropToTabWindow window)
    {
      if ( _allWindows.Contains(window) )
        _allWindows.Remove(window);
    }

    /// <summary>
    /// Close all <see cref="IDragDropToTabWindow"/>
    /// </summary>
    public void Clear()
    {
      LOG.Trace($"Close all DragDropWindow {_allWindows.Count} items");

      var listWnd = new List<IDragDropToTabWindow>(_allWindows);

      foreach ( var dropToTabWindow in listWnd )
      {
        if ( dropToTabWindow.IsParent )
          continue;

        var wnd = _allWindows.Find(p => p == dropToTabWindow);
        ((Window) wnd)?.Close();
      }
    }

    /// <summary>
    /// Drag move
    /// </summary>
    /// <param name="dragWin">DragWindow of <see cref="IDragDropToTabWindow"/></param>
    public void DragMove(IDragDropToTabWindow dragWin)
    {
      if ( dragWin == null )
        return;

      var p = new Win32Point();

      if ( !NativeMethods.GetCursorPos(ref p) )
        return;

      var dragWinPosition = new Point(p.X, p.Y);

      foreach ( var existWin in _allWindows )
      {
        if ( dragWin.Equals(existWin) )
          continue;

        if ( existWin.IsDragMouseOver(dragWinPosition) )
        {
          if ( !_dragEnteredWindows.Contains(existWin) )
            _dragEnteredWindows.Add(existWin);
        }
        else
        {
          if ( !_dragEnteredWindows.Contains(existWin) )
            continue;

          _dragEnteredWindows.Remove(existWin);
          existWin.OnDragLeave();
        }
      }

      if ( _dragEnteredWindows.Count > 0 )
      {
        var dragWinHwnd = new WindowInteropHelper((Window) dragWin).Handle;
        var dragBelowHwnd = NativeMethods.GetWindow(dragWinHwnd, NativeMethods.GW_HWNDNEXT);
        IDragDropToTabWindow nextTopWin = null;
        bool foundTabTarget = false;

        for ( var hWind = dragBelowHwnd; hWind != IntPtr.Zero; hWind = NativeMethods.GetWindow(hWind, NativeMethods.GW_HWNDNEXT) )
        {
          foreach ( var dragWindow in _dragEnteredWindows )
          {
            var enteredWin = (Window) dragWindow;
            var enterWinHwnd = new WindowInteropHelper(enteredWin).Handle;

            if ( hWind != enterWinHwnd )
              continue;

            nextTopWin = (IDragDropToTabWindow) enteredWin;
            ((IDragDropToTabWindow) enteredWin).OnDragEnter();

            foundTabTarget = true;
            break;

          }
          if ( foundTabTarget )
            break;
        }

        if ( nextTopWin == null )
          return;

        foreach ( var dragWindow in _dragEnteredWindows )
        {
          var enteredWin = (Window) dragWindow;

          if ( !nextTopWin.Equals(enteredWin) )
            ((IDragDropToTabWindow) enteredWin).OnDragLeave();
        }

        if ( nextTopWin.IsDragMouseOverTabZone(dragWinPosition) )
          ((Window) dragWin).Hide();
        else
          ((Window) dragWin).Show();
      }
      else
      {
        if ( !((Window) dragWin).IsVisible )
          ((Window) dragWin).Show();
      }
    }

    /// <summary>
    /// Drag end
    /// </summary>
    /// <param name="dragWin">DragWindow <see cref="IDragDropToTabWindow"/></param>
    public void DragEnd(IDragDropToTabWindow dragWin)
    {
      if ( dragWin == null )
        return;

      var p = new Win32Point();

      if ( !NativeMethods.GetCursorPos(ref p) )
        return;

      var dragWinPosition = new Point(p.X, p.Y);

      foreach ( var targetWin in _dragEnteredWindows )
      {
        if ( targetWin.IsDragMouseOverTabZone(dragWinPosition) )
        {
          var items = ((IDragWindow) dragWin).TabItems;

          foreach ( var t in items )
          {
            if ( t is DragSupportTabItem item )
              ((IDragWindow) targetWin).AddTabItem(item);
          }

          for ( int i = items.Count; i > 0; i-- )
          {
            if ( items[i - 1] is DragSupportTabItem item )
              ((IDragWindow) dragWin).RemoveTabItem(item, true);
          }
        }

        targetWin.OnDragLeave();
      }

      if ( _dragEnteredWindows.Count > 0 && ((IDragWindow) dragWin).TabItems.Count == 0 )
        ((Window) dragWin).Close();

      _dragEnteredWindows.Clear();
    }

    private void DragWindowManagerClosed(object sender, EventArgs e)
    {
      if ( sender is Window window )
        Unregister((IDragDropToTabWindow) window);
    }
  }
}
