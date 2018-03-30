using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Org.Vs.TailForWin.Core.Native;
using Org.Vs.TailForWin.Core.Native.Data;
using Org.Vs.TailForWin.Core.Native.Data.Enum;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils.Interfaces;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils.Utils;


namespace Org.Vs.TailForWin.UI.UserControls.DragSupportUtils
{
  /// <summary>
  /// Interaction logic for DragWindow.xaml
  /// </summary>
  public partial class DragWindow : IDragWindow, IDragDropToTabWindow
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public DragWindow()
    {
      InitializeComponent();

      SourceInitialized += DragWindowSourceInitialized;
      DragWindowManager.Instance.Register(this);
    }

    private void DragWindowSourceInitialized(object sender, EventArgs e)
    {
      var handle = new WindowInteropHelper(this).Handle;
      var sysMenuHandle = NativeMethods.GetSystemMenu(handle, false);

      NativeMethods.InsertMenu(sysMenuHandle, 5, NativeMethods.MF_BYPOSITION | NativeMethods.MF_SEPARATOR, 0, string.Empty);

      HwndSource source = HwndSource.FromHwnd(handle);
      source?.AddHook(WndProc);
    }

    private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
      handled = false;

      switch ( msg )
      {
      case NativeMethods.WM_ENTERSIZEMOVE:

        break;

      case NativeMethods.WM_EXITSIZEMOVE:

        break;

      case NativeMethods.WM_MOVE:

        break;

      case NativeMethods.WM_GETMINMAXINFO:

        WindowGetMinMaxInfo.WmGetMinMaxInfo(hWnd, lParam);
        handled = true;
        break;

      case NativeMethods.WM_WINDOWPOSCHANGING:

        WINDOWPOS pos = (WINDOWPOS) Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));

        if ( (pos.flags & (int) SWP.NOMOVE) != 0 )
          return IntPtr.Zero;

        Window wnd = (Window) HwndSource.FromHwnd(hWnd)?.RootVisual;

        if ( wnd == null )
          return IntPtr.Zero;

        bool changedPos = false;

        if ( pos.cx < MinWidth )
        {
          pos.cx = (int) MinWidth;
          changedPos = true;
        }

        if ( pos.cy < MinHeight )
        {
          pos.cy = (int) MinHeight;
          changedPos = true;
        }

        if ( !changedPos )
          return IntPtr.Zero;

        Marshal.StructureToPtr(pos, lParam, true);
        handled = true;
        break;
      }
      return IntPtr.Zero;
    }

    /// <summary>
    /// Creates a new <see cref="DragWindow"/>
    /// </summary>
    /// <param name="left">Left</param>
    /// <param name="top">Top</param>
    /// <param name="width">Width</param>
    /// <param name="height">Height</param>
    /// <param name="tabItem"><see cref="DragSupportTabItem"/></param>
    /// <returns><see cref="DragWindow"/></returns>
    public static DragWindow CreateTabWindow(double left, double top, double width, double height, DragSupportTabItem tabItem)
    {
      var dragWindow = new DragWindow
      {
        Left = left,
        Top = top,
        Width = width,
        Height = height,
        WindowStartupLocation = WindowStartupLocation.Manual
      };

      dragWindow.Show();
      dragWindow.Activate();
      dragWindow.Focus();

      return dragWindow;
    }

    /// <summary>
    /// TabItem source
    /// </summary>
    public ItemCollection TabItems => throw new NotImplementedException();

    /// <summary>
    /// Add TabItem
    /// </summary>
    /// <param name="tabHeader">Name of tab header</param>
    /// <param name="content">TabItem content</param>
    public void AddTabItem(string tabHeader, Control content)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Remove TabItem
    /// </summary>
    /// <param name="tabItem"><see cref="DragSupportTabItem"/></param>
    public void RemoveTabItem(DragSupportTabItem tabItem)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// On Drag enter
    /// </summary>
    public void OnDragEnter()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// On Drag leave
    /// </summary>
    public void OnDrageLeave()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Is drag mouse ober
    /// </summary>
    /// <param name="mousePosition">Current mouse position</param>
    /// <returns>If it is over<c>True</c> otherwise <c>False</c></returns>
    public bool IsDragMouseOver(Point mousePosition)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Is drag mouse over tab zone
    /// </summary>
    /// <param name="mousePosition">Current mouse position</param>
    /// <returns>If it is over <c>True</c> otherwise <c>False</c></returns>
    public bool IsDragMouseOverTabZone(Point mousePosition)
    {
      throw new NotImplementedException();
    }
  }
}
