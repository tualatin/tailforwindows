using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Org.Vs.TailForWin.Data.Native;
using Org.Vs.TailForWin.Interfaces;
using Org.Vs.TailForWin.Native;


namespace Org.Vs.TailForWin.UI.Utils
{
  /// <summary>
  /// Drag window manager
  /// </summary>
  public class DragWindowManager
  {
    private static DragWindowManager instance;

    private readonly List<IDragDropToTabWindow> allWindows;
    private readonly List<IDragDropToTabWindow> dragEnteredWindows;

    /// <summary>
    /// Current instance of DragWindowManager
    /// </summary>
    public static DragWindowManager Instance => instance ?? (instance = new DragWindowManager());

    /// <summary>
    /// List of all Drag and drap tab windows
    /// </summary>
    public List<IDragDropToTabWindow> AllWindows => allWindows;

    private DragWindowManager()
    {
      allWindows = new List<IDragDropToTabWindow>();
      dragEnteredWindows = new List<IDragDropToTabWindow>();
    }

    /// <summary>
    /// Register current window
    /// </summary>
    /// <param name="window">Reference to DragDropToTabWindow window to register</param>
    public void Register(IDragDropToTabWindow window)
    {
      if(!allWindows.Contains(window))
      {
        allWindows.Add(window);
        ((Window) window).Closed += DragWindowManager_Closed;
      }
    }

    /// <summary>
    /// Unregister current window
    /// </summary>
    /// <param name="window">Reference to DragDropToTabWindow window to unregister</param>
    public void Unregister(IDragDropToTabWindow window)
    {
      if(allWindows.Contains(window))
        allWindows.Remove(window);
    }

    /// <summary>
    /// Drag move
    /// </summary>
    /// <param name="dragWindow">Window to move</param>
    public void DragMove(IDragDropToTabWindow dragWindow)
    {
      if(dragWindow == null)
        return;

      Win32Point p = new Win32Point();

      if(!NativeMethods.GetCursorPos(ref p))
        return;

      Point dragWinPosition = new Point(p.X, p.Y);

      foreach(IDragDropToTabWindow existWindow in allWindows)
      {
        if(dragWindow.Equals(existWindow))
          continue;

        if(existWindow.IsDragMouseOver(dragWinPosition))
        {
          if(!dragEnteredWindows.Contains(existWindow))
            dragEnteredWindows.Add(existWindow);
        }
        else
        {
          if(dragEnteredWindows.Contains(existWindow))
          {
            dragEnteredWindows.Remove(existWindow);
            existWindow.OnDrageLeave();
          }
        }
      }

      if(dragEnteredWindows.Count > 0)
      {
        IntPtr dragWinHwnd = new WindowInteropHelper((Window) dragWindow).Handle;
        IntPtr dragBelowhwnd = NativeMethods.GetWindow(dragWinHwnd, NativeMethods.GW_HWNDNEXT);
        IDragDropToTabWindow nextTopWindow = null;
        bool foundTabTarget = false;

        for(IntPtr hWind = dragBelowhwnd; hWind != IntPtr.Zero; hWind = NativeMethods.GetWindow(hWind, NativeMethods.GW_HWNDNEXT))
        {
          foreach(var dragDropToTabWindow in dragEnteredWindows)
          {
            var enteredWindow = (Window) dragDropToTabWindow;
            IntPtr enterWinHwnd = new WindowInteropHelper(enteredWindow).Handle;

            if(hWind == enterWinHwnd)
            {
              nextTopWindow = (IDragDropToTabWindow) enteredWindow;
              ((IDragDropToTabWindow) enteredWindow).OnDragEnter();
              foundTabTarget = true;
              break;
            }

          }

          if(foundTabTarget)
            break;
        }

        if(nextTopWindow != null)
        {
          foreach(var dragDropToTabWindow in dragEnteredWindows)
          {
            var enteredWin = (Window) dragDropToTabWindow;

            if(!nextTopWindow.Equals(enteredWin))
              ((IDragDropToTabWindow) enteredWin).OnDrageLeave();
          }

          if(nextTopWindow.IsDragMouseOverTabZone(dragWinPosition))
            ((Window) dragWindow).Hide();
          else
            ((Window) dragWindow).Show();
        }
      }
      else
      {
        if(!((Window) dragWindow).IsVisible)
          ((Window) dragWindow).Show();
      }
    }

    /// <summary>
    /// Drag end
    /// </summary>
    /// <param name="dragWindow">Drag window</param>
    public void DragEnd(IDragDropToTabWindow dragWindow)
    {
      if(dragWindow == null)
        return;

      Win32Point p = new Win32Point();

      if(!NativeMethods.GetCursorPos(ref p))
        return;

      Point dragWinPosition = new Point(p.X, p.Y);

      foreach(IDragDropToTabWindow targetWindow in dragEnteredWindows)
      {
        if(targetWindow.IsDragMouseOverTabZone(dragWinPosition))
        {
          ItemCollection items = ((ITabWindow) dragWindow).TabItems;

          for(int i = 0; i < items.Count; i++)
          {
            TabItem item = items[i] as TabItem;

            if(item != null && item.Header.Equals(targetWindow.TabAdd.Header))
              continue;

            if(item != null)
              ((ITabWindow) targetWindow).AddTabItem(item.Header.ToString(), (Control) item.Content);
          }

          for(int i = items.Count; i > 0; i--)
          {
            TabItem item = items[i - 1] as TabItem;

            if(item != null)
              ((ITabWindow) dragWindow).RemoveTabItem(item);
          }
        }

        targetWindow.OnDrageLeave();
      }

      if(dragEnteredWindows.Count > 0 && ((ITabWindow) dragWindow).TabItems.Count == 0)
        ((Window) dragWindow).Close();

      dragEnteredWindows.Clear();
    }

    private void DragWindowManager_Closed(object sender, EventArgs e)
    {
      if(sender is Window)
      {
        Window window = sender as Window;
        Unregister((IDragDropToTabWindow) window);
      }
    }
  }
}
