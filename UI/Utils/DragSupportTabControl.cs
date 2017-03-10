using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Org.Vs.TailForWin.Data;
using Org.Vs.TailForWin.Native;

namespace Org.Vs.TailForWin.UI.Utils
{
  /// <summary>
  /// Drag supported tabcontrol
  /// </summary>
  public class DragSupportTabControl : TabControl
  {
    private readonly object synLockTabWindow;

    private Point startPoint;
    private Window dragToWindow;

    /// <summary>
    /// Standard constructor
    /// </summary>
    public DragSupportTabControl()
      : base()
    {
      synLockTabWindow = new object();

      AllowDrop = true;
      MouseMove += DragSupportTabControl_MouseMove;
      Drop += DragSupportTabControl_Drop;
      PreviewMouseLeftButtonDown += DragSupportTabControl_PreviewMouseLeftButtonDown;
    }

    /// <summary>
    /// Remove a TabItem
    /// </summary>
    /// <param name="tabItem">TabItem to remove</param>
    public void RemoveTabItem(TabItem tabItem)
    {
      if(Items.Contains(tabItem))
        Items.Remove(tabItem);
    }

    /// <summary>
    /// Tab two TabItems
    /// </summary>
    /// <param name="source">Soure TabItem</param>
    /// <param name="target">Target TabItem</param>
    /// <returns>If success <c>true</c> otherwise <c>false</c></returns>
    public bool SwapTabItems(TabItem source, TabItem target)
    {
      if(source == null || target == null)
        return (false);

      if(!target.Equals(source))
      {
        var tabControl = target.Parent as TabControl;
        int sourceIndex = tabControl.Items.IndexOf(source);
        int targetIndex = tabControl.Items.IndexOf(target);

        tabControl.Items.Remove(source);
        tabControl.Items.Insert(targetIndex, source);

        tabControl.SelectedIndex = targetIndex;

        return (true);
      }
      return (false);
    }


    #region Events

    private void DragSupportTabControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      startPoint = e.GetPosition(this);
    }

    private void DragSupportTabControl_Drop(object sender, DragEventArgs e)
    {
      QueryContinueDrag -= DragSupportTabControl_QueryContinueDrag;

      var tabItemTarget = e.Source as TabItem;

      if(tabItemTarget == null || tabItemTarget.Header.Equals("+"))
        return;

      var tabItemSource = e.Data.GetData(typeof(TailForWinTabItem)) as TabItem;

      if(tabItemSource == null)
        return;

      SwapTabItems(tabItemSource, tabItemTarget);
    }

    private void DragSupportTabControl_MouseMove(object sender, MouseEventArgs e)
    {
      // One tab with add tab is open, no draging
      if(Items.Count <= 2)
        return;

      Point mpos = e.GetPosition(null);
      Vector diff = startPoint - mpos;

      if(e.LeftButton == MouseButtonState.Pressed && (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
      {
        var tabItem = e.Source as TailForWinTabItem;

        if(tabItem == null)
          return;

        QueryContinueDrag += DragSupportTabControl_QueryContinueDrag;
        GiveFeedback += new GiveFeedbackEventHandler(DragSupportTabControl_GiveFeedback);

        DragDrop.DoDragDrop(tabItem, tabItem, DragDropEffects.All);

        GiveFeedback -= new GiveFeedbackEventHandler(DragSupportTabControl_GiveFeedback);
      }
    }

    private void DragSupportTabControl_GiveFeedback(object sender, GiveFeedbackEventArgs e)
    {
      if(dragToWindow != null)
      {
        Mouse.SetCursor(Cursors.Arrow);
        e.Handled = true;
      }
    }

    private void DragSupportTabControl_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
    {
      if(e.KeyStates == DragDropKeyStates.LeftMouseButton)
      {
        Win32Point p = new Win32Point();

        if(NativeMethods.GetCursorPos(ref p))
        {
          Point _tabPos = PointToScreen(new Point(0, 0));

          if(!((p.X >= _tabPos.X && p.X <= (_tabPos.X + ActualWidth) && p.Y >= _tabPos.Y && p.Y <= (_tabPos.Y + ActualHeight))))
          {
            if(e.Source is TabItem item)
              UpdateWindowLocation(p.X - 50, p.Y - 10, item);
          }
          else
          {
            if(dragToWindow != null)
              UpdateWindowLocation(p.X - 50, p.Y - 10, null);
          }
        }
      }
      else if(e.KeyStates == DragDropKeyStates.None)
      {
        QueryContinueDrag -= DragSupportTabControl_QueryContinueDrag;
        e.Handled = true;

        if(dragToWindow != null)
        {
          dragToWindow = null;
          var item = e.Source as TabItem;

          if(item != null)
            RemoveTabItem(item);
        }
      }
    }

    #endregion

    #region HelperFunctions

    private void UpdateWindowLocation(double left, double top, TabItem tabItem)
    {
      if(dragToWindow == null)
      {
        lock(synLockTabWindow)
        {
          if(dragToWindow == null)
            dragToWindow = TabWindow.CreateTabWindow(left, top, ActualWidth, ActualHeight, tabItem);
        }
      }

      if(dragToWindow != null)
      {
        dragToWindow.Left = left;
        dragToWindow.Top = top;
      }
    }

    #endregion
  }
}
