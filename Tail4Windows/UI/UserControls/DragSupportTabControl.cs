using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Org.Vs.TailForWin.Core.Native;
using Org.Vs.TailForWin.Core.Native.Data;


namespace Org.Vs.TailForWin.UI.UserControls
{
  /// <summary>
  /// Drag support tab control
  /// </summary>
  public class DragSupportTabControl : TabControl
  {
    private static readonly object MyLockWindow = new object();
    private Point _startPoint;
    private Window _dragToWindow;

    /// <summary>
    /// Standard constructor
    /// </summary>
    public DragSupportTabControl()
    {
      AllowDrop = true;
      MouseMove += DragSupportTabControlMouseMove;
      Drop += DragSupportTabControlDrop;
      PreviewMouseLeftButtonDown += DragSupportTabControlPreviewMouseLeftButtonDown;
    }

    /// <summary>
    /// Remove a TabItem
    /// </summary>
    /// <param name="tabItem">TabItem to remove</param>
    public void RemoveTabItem(TabItem tabItem)
    {
      if ( Items.Contains(tabItem) )
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
      if ( source == null || target == null )
        return false;

      if ( target.Equals(source) )
        return false;

      if ( !(target.Parent is TabControl tabControl) )
        return false;

      int sourceIndex = tabControl.Items.IndexOf(source);
      int targetIndex = tabControl.Items.IndexOf(target);

      tabControl.Items.Remove(source);
      tabControl.Items.Insert(targetIndex, source);

      tabControl.SelectedIndex = targetIndex;

      return true;
    }

    #region Events

    private void DragSupportTabControlPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => _startPoint = e.GetPosition(this);

    private void DragSupportTabControlDrop(object sender, DragEventArgs e)
    {
      QueryContinueDrag -= DragSupportTabControlQueryContinueDrag;

      if ( !(e.Source is TabItem tabItemTarget) )
        return;

      if ( !(e.Data.GetData(typeof(TailForWinTabItem)) is TabItem tabItemSource) )
        return;

      SwapTabItems(tabItemSource, tabItemTarget);
    }

    private void DragSupportTabControlMouseMove(object sender, MouseEventArgs e)
    {
      // One tab with add tab is open, no draging
      if ( Items.Count <= 2 )
        return;

      Point mpos = e.GetPosition(null);
      Vector diff = _startPoint - mpos;

      if ( e.LeftButton != MouseButtonState.Pressed || !(Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance) && !(Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance) )
        return;

      if ( !(e.Source is TailForWinTabItem tabItem) )
        return;

      QueryContinueDrag += DragSupportTabControlQueryContinueDrag;
      GiveFeedback += DragSupportTabControlGiveFeedback;

      DragDrop.DoDragDrop(tabItem, tabItem, DragDropEffects.All);

      GiveFeedback -= DragSupportTabControlGiveFeedback;
    }

    private void DragSupportTabControlGiveFeedback(object sender, GiveFeedbackEventArgs e)
    {
      if ( _dragToWindow == null )
        return;

      Mouse.SetCursor(Cursors.Arrow);
      e.Handled = true;
    }

    private void DragSupportTabControlQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
    {
      switch ( e.KeyStates )
      {
      case DragDropKeyStates.LeftMouseButton:

        Win32Point p = new Win32Point();

        if ( NativeMethods.GetCursorPos(ref p) )
        {
          Point tabPos = PointToScreen(new Point(0, 0));

          if ( !(p.X >= tabPos.X && p.X <= tabPos.X + ActualWidth && p.Y >= tabPos.Y && p.Y <= tabPos.Y + ActualHeight) )
          {
            if ( e.Source is TabItem item )
              UpdateWindowLocation(p.X - 50, p.Y - 10, item);
          }
          else
          {
            if ( _dragToWindow != null )
              UpdateWindowLocation(p.X - 50, p.Y - 10, null);
          }
        }
        break;

      case DragDropKeyStates.None:

        QueryContinueDrag -= DragSupportTabControlQueryContinueDrag;
        e.Handled = true;

        if ( _dragToWindow != null )
        {
          _dragToWindow = null;

          if ( e.Source is TabItem item )
            RemoveTabItem(item);
        }
        break;

      default:

        throw new ArgumentOutOfRangeException();
      }
    }

    #endregion

    #region HelperFunctions

    private void UpdateWindowLocation(double left, double top, TabItem tabItem)
    {
      //if(dragToWindow == null)
      //{
      //  lock(synLockTabWindow)
      //  {
      //    if(dragToWindow == null)
      //      dragToWindow = TabWindow.CreateTabWindow(left, top, ActualWidth, ActualHeight, tabItem);
      //  }
      //}

      //if(dragToWindow != null)
      //{
      //  dragToWindow.Left = left;
      //  dragToWindow.Top = top;
      //}
    }

    #endregion
  }
}
