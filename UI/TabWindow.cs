using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using Org.Vs.TailForWin.Interfaces;
using Org.Vs.TailForWin.Native;
using Org.Vs.TailForWin.UI.Utils;


namespace Org.Vs.TailForWin.UI
{
  /// <summary>
  /// Tab window logic
  /// </summary>
  public class TabWindow : Window, ITabWindow, IDragDropToTabWindow
  {
    private OverlayDragWnd overlayWindow;
    private DragSupportTabControl tabControl;
    private bool hasFocus;
    private TailForWinTabItem tabAdd;


    /// <summary>
    /// Standarc constructor
    /// </summary>
    public TabWindow()
      : base()
    {
      Grid gridContent = new Grid();
      Content = gridContent;
      tabControl = new DragSupportTabControl();
      tabControl.SelectionChanged += TabControl_SelectionChanged;

      gridContent.Children.Add(tabControl);
      tabControl.Margin = new Thickness(0);

      tabAdd = new TailForWinTabItem
      {
        Header = "+",
        Name = "AddChildTab",
        Style = (Style) FindResource("TabItemAddStyle")
      };
      tabAdd.PreviewMouseLeftButtonDown += TabAdd_MouseLeftButtonDown;
      tabControl.Items.Add(tabAdd);

      DragWindowManager.Instance.Register(this);
      SourceInitialized += TabWindow_SourceInitialized;
      PreviewMouseDown += TabWindow_PreviewMouseDown;
    }

    /// <summary>
    /// Create a new tab window
    /// </summary>
    /// <param name="left">Left</param>
    /// <param name="top">Top</param>
    /// <param name="width">Width</param>
    /// <param name="height">Height</param>
    /// <param name="tabItem">TabItem</param>
    /// <returns>New instance of TabWindow</returns>
    public static TabWindow CreateTabWindow(double left, double top, double width, double height, TabItem tabItem)
    {
      TabWindow tabWin = new TabWindow
      {
        Width = width,
        Height = height,
        Left = left,
        Top = top,
        WindowStartupLocation = WindowStartupLocation.Manual,
      };
      Control tabContent = tabItem.Content as Control;

      if(tabContent == null)
      {
        tabContent = new ContentControl();
        ((ContentControl) tabContent).Content = tabItem.Content;
      }

      ((ITabWindow) tabWin).AddTabItem(tabItem.Header.ToString(), tabContent);
      tabWin.Show();
      tabWin.Activate();
      tabWin.Focus();

      return (tabWin);
    }

    #region Events

    private void TabAdd_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if(sender is TailForWinTabItem)
      {
        var tabItem = sender as TailForWinTabItem;

        if(tabItem.Equals(tabAdd))
          AddTabItem("No file.", null);
      }
    }

    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if(!IsInitialized)
        return;

      if(e.Source is TabControl)
      {
        if(e.AddedItems.Count == 0)
          return;

        //LOG.Trace("{0}", System.Reflection.MethodBase.GetCurrentMethod());

        var tab = e.AddedItems[0] as TabItem;
        var tabControl = e.Source as TabControl;

        if(tab == null || tabControl == null)
          return;

        e.Handled = true;

        if(tab.Equals(tabAdd))
        {
          tabControl.SelectedItem = tabControl.Items[tabControl.Items.Count - 2];
          return;
        }
      }
    }

    private void TabWindow_SourceInitialized(object sender, EventArgs e)
    {
      HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
      source.AddHook(new HwndSourceHook(WndProc));
    }

    private void TabWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      if(e.MiddleButton != MouseButtonState.Pressed)
        return;

      Point mousePoint = PointToScreen(Mouse.GetPosition(this));
      bool addNew = false;

      foreach(TabItem item in tabControl.Items)
      {
        Point relativePoint = item.PointToScreen(new Point(0, 0));
        System.Drawing.Rectangle rc = new System.Drawing.Rectangle((int) relativePoint.X, (int) relativePoint.Y, (int) item.DesiredSize.Width, (int) item.DesiredSize.Height);

        if(rc.Contains((int) mousePoint.X, (int) mousePoint.Y))
          return;

        addNew = true;
      }

      if(!addNew)
        return;

      AddTabItem("No file.", null);
    }

    private void TabItem_TabHeaderDoubleClick(object sender, RoutedEventArgs e)
    {
      // When one tab and the add tab is open, no other operation possible
      if(tabControl.Items.Count <= 2)
        return;

      if(e.Source is TailForWinTabItem tabItem)
      {
        Point mousePos = PointToScreen(Mouse.GetPosition(tabItem));
        TabWindow tabWin = CreateTabWindow(mousePos.X, mousePos.Y, ActualWidth, ActualHeight, tabItem);

        tabControl.RemoveTabItem(tabItem);
        tabWin.Activate();
        tabWin.Focus();
      }
    }

    private void TabItem_LastTabWindowOpen(object sender, RoutedEventArgs e)
    {
      if(e.Source is TailForWinTabItem tabItem)
      {
        AddTabItem("No file.", null);
        RemoveTabItem(tabItem);
      }
    }

    #endregion

    #region ITabWindow

    /// <summary>
    /// Tab items collection
    /// </summary>
    public ItemCollection TabItems
    {
      get => tabControl.Items;
    }

    /// <summary>
    /// Tab header selected
    /// </summary>
    public string TabHeaderSelected
    {
      get
      {
        if(tabControl.SelectedItem is TailForWinTabItem tab)
          return (tab.Header.ToString());

        return (string.Empty);
      }
      set
      {
        SelectTabItem(value);
      }
    }

    /// <summary>
    /// Add tab item to control
    /// </summary>
    /// <param name="tabHeader">Tab header</param>
    /// <param name="content">Content</param>
    public void AddTabItem(string tabHeader, Control content)
    {
      if(tabControl.Items.Count > 8)
        return;

      TailForWinTabItem item = new TailForWinTabItem
      {
        Header = tabHeader,
        Content = content,
        Style = (Style) FindResource("TabItemStopStyle")
      };
      item.TabHeaderDoubleClick += TabItem_TabHeaderDoubleClick;
      item.LastTabWindowOpen += TabItem_LastTabWindowOpen;

      tabControl.Items.Insert(tabControl.Items.Count - 1, item);
      tabControl.SelectedItem = item;
    }

    /// <summary>
    /// Remove a tab item
    /// </summary>
    /// <param name="tabItem">Item to remove</param>
    public void RemoveTabItem(TabItem tabItem)
    {
      if(tabControl.Items.Contains(tabItem))
      {
        ((TailForWinTabItem) tabItem).TabHeaderDoubleClick -= TabItem_TabHeaderDoubleClick;
        ((TailForWinTabItem) tabItem).LastTabWindowOpen -= TabItem_LastTabWindowOpen;
        tabControl.Items.Remove(tabItem);
      }
    }

    #endregion

    #region IDragDropToTabWindow

    /// <summary>
    /// Add TabItem
    /// </summary>
    public TabItem TabAdd => tabAdd;

    /// <summary>
    /// Is drag mouse over
    /// </summary>
    /// <param name="mousePosition">Current mouse position</param>
    /// <returns>If mouse pointer is over <c>true</c> otherwise <c>false</c></returns>
    public bool IsDragMouseOver(Point mousePosition)
    {
      if(WindowState == WindowState.Minimized)
        return (false);

      double left, top;

      if(WindowState == WindowState.Maximized)
      {
        left = 0;
        top = 0;
      }
      else
      {
        left = Left;
        top = Top;
      }

      bool isMouseOver = (mousePosition.X > left && mousePosition.X < (left + ActualWidth) && mousePosition.Y > top && mousePosition.Y < (top + ActualHeight));

      return (isMouseOver);
    }

    /// <summary>
    /// Is mouse over tab zone
    /// </summary>
    /// <param name="mousePosition">Current mouse position</param>
    /// <returns>If mouse pointer is over <c>true</c> otherwise <c>false</c></returns>
    public bool IsDragMouseOverTabZone(Point mousePosition)
    {
      if(overlayWindow == null)
        return (false);

      return (overlayWindow.IsMouseOverTabTarget(mousePosition));
    }

    /// <summary>
    /// Drag leave
    /// </summary>
    public void OnDrageLeave()
    {
      if(overlayWindow != null)
      {
        overlayWindow.Close();
        overlayWindow = null;
      }
    }

    /// <summary>
    /// Drag enter
    /// </summary>
    public void OnDragEnter()
    {
      if(overlayWindow == null)
        overlayWindow = new OverlayDragWnd();

      if(WindowState == WindowState.Maximized)
      {
        overlayWindow.Left = 0;
        overlayWindow.Top = 0;
      }
      else
      {
        overlayWindow.Left = Left;
        overlayWindow.Top = Top;
      }

      overlayWindow.Width = ActualWidth;
      overlayWindow.Height = ActualHeight;
      overlayWindow.Topmost = true;

      overlayWindow.Show();
    }

    #endregion

    #region HelperFunctions

    private void SelectTabItem(string tabHeader)
    {
      TailForWinTabItem selectedTab = null;

      foreach(TailForWinTabItem item in tabControl.Items)
      {
        if(item.Header.ToString() == tabHeader)
        {
          selectedTab = item;
          break;
        }
      }

      if(selectedTab != null)
        tabControl.SelectedItem = selectedTab;
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
      if(msg == NativeMethods.WM_ENTERSIZEMOVE)
      {
        hasFocus = true;
      }
      else if(msg == NativeMethods.WM_EXITSIZEMOVE)
      {
        hasFocus = false;
        DragWindowManager.Instance.DragEnd(this);
      }
      else if(msg == NativeMethods.WM_MOVE)
      {
        if(hasFocus)
          DragWindowManager.Instance.DragMove(this);
      }

      handled = false;
      return (IntPtr.Zero);
    }

    #endregion
  }
}
