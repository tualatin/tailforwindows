using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using log4net;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Native;
using Org.Vs.TailForWin.Core.Native.Data;
using Org.Vs.TailForWin.UI.Utils;


namespace Org.Vs.TailForWin.UI.UserControls.DragSupportUtils
{
  /// <summary>
  /// Drag support tab control
  /// </summary>
  public class DragSupportTabControl : TabControl
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(DragSupportTabItem));

    private static readonly object MyLockWindow = new object();
    private Point _startPoint;
    private Window _dragToWindow;
    private RepeatButton _repeatButtonLeft;
    private RepeatButton _repeatButtonRight;
    private Button _addTabItemButton;
    private ScrollViewer _scrollViewer;
    private Panel _headerPanel;

    #region RoutedEvents

    /// <summary>
    /// AddTabItem event handler
    /// </summary>
    public static readonly RoutedEvent AddTabItemRoutedEvent = EventManager.RegisterRoutedEvent("AddTabItemEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DragSupportTabControl));

    /// <summary>
    /// Add TabItem
    /// </summary>
    public event RoutedEventHandler AddTabItemEvent
    {
      add => AddHandler(AddTabItemRoutedEvent, value);
      remove => RemoveHandler(AddTabItemRoutedEvent, value);
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public DragSupportTabControl()
    {
      AllowDrop = true;
      MouseMove += DragSupportTabControlMouseMove;
      Drop += DragSupportTabControlDrop;
      PreviewMouseLeftButtonDown += DragSupportTabControlPreviewMouseLeftButtonDown;
      Loaded += DragSupportTabControlLoaded;
    }

    /// <summary>
    /// Called when <see cref="System.Windows.FrameworkElement.ApplyTemplate" /> is called.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      _repeatButtonLeft = GetTemplateChild("RepeatButtonLeft") as RepeatButton;
      _repeatButtonRight = GetTemplateChild("RepeatButtonRight") as RepeatButton;
      _scrollViewer = GetTemplateChild("ScrollViewerTabControl") as ScrollViewer;
      _headerPanel = GetTemplateChild("HeaderPanel") as Panel;

      if ( _repeatButtonLeft != null )
        _repeatButtonLeft.Click += RepeatButtonLeftClick;

      if ( _repeatButtonRight != null )
        _repeatButtonRight.Click += RepeatButtonRightClick;

      if ( _addTabItemButton != null )
        _addTabItemButton.Click += AddTabItemButtonClick;

      if ( _scrollViewer == null )
        return;

      SelectionChanged += (s, e) => ScrollToSelectedItem();
      _scrollViewer.Loaded += (s, e) => UpdateScrollButtonsVisibility();
      _scrollViewer.ScrollChanged += (s, e) => UpdateScrollButtonsVisibility();
    }

    private void DragSupportTabControlLoaded(object sender, RoutedEventArgs e)
    {
      if ( !(GetTemplateChild("ContentControlAddButton") is ContentControl contentControl) )
        return;

      _addTabItemButton = (Button) UiHelpers.RecursiveVisualChildFinder<Button>(contentControl);

      if ( _addTabItemButton == null )
        return;

      _addTabItemButton.Click -= AddTabItemButtonClick;
      _addTabItemButton.Click += AddTabItemButtonClick;
    }

    /// <summary>
    /// Remove a TabItem
    /// </summary>
    /// <param name="tabItem">TabItem to remove</param>
    private void RemoveTabItem(DragSupportTabItem tabItem)
    {
      if ( !Items.Contains(tabItem) )
        return;

      var list = ItemsSource as ObservableCollection<DragSupportTabItem>;
      list?.Remove(tabItem);
    }

    /// <summary>
    /// Tab two TabItems
    /// </summary>
    /// <param name="source">Soure TabItem</param>
    /// <param name="target">Target TabItem</param>
    /// <returns>If success <c>true</c> otherwise <c>false</c></returns>
    private void SwapTabItems(DragSupportTabItem source, DragSupportTabItem target)
    {
      if ( source == null || target == null )
        return;

      if ( target.Equals(source) )
        return;

      //int sourceIndex = Items.IndexOf(source);
      int targetIndex = Items.IndexOf(target);

      var list = ItemsSource as ObservableCollection<DragSupportTabItem>;
      list?.Remove(source);
      list?.Insert(targetIndex, source);

      SelectedIndex = targetIndex;
    }

    #region Events

    private void AddTabItemButtonClick(object sender, RoutedEventArgs e) => RaiseEvent(new RoutedEventArgs(AddTabItemRoutedEvent, this));

    private void RepeatButtonLeftClick(object sender, RoutedEventArgs e)
    {
      if ( _scrollViewer == null || _headerPanel == null )
        return;

      double leftItemOffset = Math.Max(_scrollViewer.HorizontalOffset - _headerPanel.Margin.Left, 0);
      var leftItem = GetItemByOffset(leftItemOffset);
      ScrollToItem(leftItem);
    }

    private void RepeatButtonRightClick(object sender, RoutedEventArgs e)
    {
      if ( _scrollViewer == null || _headerPanel == null )
        return;

      // added margin left for sure that the item will be scrolled
      double rightItemOffset = Math.Min(_scrollViewer.HorizontalOffset + _scrollViewer.ViewportWidth + 2, _scrollViewer.ExtentWidth);
      var rightItem = GetItemByOffset(rightItemOffset);
      ScrollToItem(rightItem);
    }

    private void DragSupportTabControlPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => _startPoint = e.GetPosition(this);

    private void DragSupportTabControlDrop(object sender, DragEventArgs e)
    {
      QueryContinueDrag -= DragSupportTabControlQueryContinueDrag;

      if ( !(e.Data.GetData(typeof(DragSupportTabItem)) is DragSupportTabItem tabItemSource) )
        return;

      try
      {
        DragSupportTabItem tabItemTarget = null;

        if ( ItemsSource == null )
          return;

        foreach ( DragSupportTabItem tabItem in ItemsSource )
        {
          var pt = e.GetPosition(tabItem);
          var result = VisualTreeHelper.HitTest(tabItem, pt);

          if ( result == null )
            continue;

          tabItemTarget = tabItem;
          break;
        }

        if ( tabItemTarget == null )
          return;

        SwapTabItems(tabItemSource, tabItemTarget);
      }
      catch
      {
        // Nothing
      }
      finally
      {
        e.Handled = true;
      }
    }

    private void DragSupportTabControlMouseMove(object sender, MouseEventArgs e)
    {
      if ( Items.Count <= 1 )
        return;

      var mpos = e.GetPosition(null);
      var diff = _startPoint - mpos;
      const int offset = 28;

      if ( e.LeftButton != MouseButtonState.Pressed || !(Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance + offset) && !(Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance + offset) )
        return;

      var tabControl = e.Source as DragSupportTabControl;

      if ( !(tabControl?.SelectedItem is DragSupportTabItem tabItem) )
        return;

      if ( tabItem.ColorPopupIsOpen )
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
      if ( !SettingsHelperController.CurrentSettings.ActivateDragDropWindow )
        return;

      DragSupportTabControl tabControl = null;

      if ( e.Source is DragSupportTabControl control )
        tabControl = control;

      if ( e.KeyStates == DragDropKeyStates.LeftMouseButton )
      {
        LOG.Trace($"DragSupportTabControlQueryContinueDrag action {e.Action}");

        var p = new Win32Point();

        if ( NativeMethods.GetCursorPos(ref p) )
        {
          var tabPos = PointToScreen(new Point(0, 0));

          if ( !(p.X >= tabPos.X && p.X <= tabPos.X + ActualWidth && p.Y >= tabPos.Y && p.Y <= tabPos.Y + ActualHeight) )
          {
            if ( tabControl?.SelectedItem is DragSupportTabItem item )
              UpdateWindowLocation(p.X - 50, p.Y - 10, item);
          }
          else
          {
            if ( _dragToWindow != null )
              UpdateWindowLocation(p.X - 50, p.Y - 10, null);
          }
        }
      }
      else if ( e.KeyStates == DragDropKeyStates.None )
      {
        LOG.Trace($"DragSupportTabControlQueryContinueDrag action {e.Action}");

        QueryContinueDrag -= DragSupportTabControlQueryContinueDrag;
        e.Handled = true;

        if ( _dragToWindow == null )
          return;

        _dragToWindow = null;

        if ( tabControl?.SelectedItem is DragSupportTabItem item )
          RemoveTabItem(item);
      }
    }

    #endregion

    #region HelperFunctions

    private void ScrollToSelectedItem()
    {
      var model = SelectedItem;

      if ( !(ItemContainerGenerator.ContainerFromItem(model) is TabItem si) || _scrollViewer == null )
        return;

      if ( Equals(si.ActualWidth, 0.0) && !si.IsLoaded )
      {
        si.Loaded += (s, e) => ScrollToSelectedItem();
        return;
      }

      ScrollToItem(si);
    }

    /// <summary>
    /// Change visibility and avalability of buttons if it is necessary
    /// </summary>
    /// <param name="horizontalOffset">the real offset instead of outdated one from the scroll viewer</param>
    private void UpdateScrollButtonsVisibility(double? horizontalOffset = null)
    {
      if ( _scrollViewer == null )
        return;

      double hOffset = horizontalOffset ?? _scrollViewer.HorizontalOffset;
      hOffset = Math.Max(hOffset, 0);

      double scrWidth = _scrollViewer.ScrollableWidth;
      scrWidth = Math.Max(scrWidth, 0);

      if ( _repeatButtonLeft != null )
      {
        _repeatButtonLeft.Visibility = Equals(scrWidth, 0.0) ? Visibility.Collapsed : Visibility.Visible;
        _repeatButtonLeft.Visibility = hOffset > 0 ? Visibility.Visible : Visibility.Collapsed;
      }

      if ( _repeatButtonRight == null )
        return;

      _repeatButtonRight.Visibility = Equals(scrWidth, 0.0) ? Visibility.Collapsed : Visibility.Visible;
      _repeatButtonRight.Visibility = hOffset < scrWidth ? Visibility.Visible : Visibility.Collapsed;
    }

    private void ScrollToItem(TabItem si)
    {
      var tabItems = Items.Cast<object>().Select(item => ItemContainerGenerator.ContainerFromItem(item) as TabItem);
      var leftItems = tabItems.Where(ti => ti != null).TakeWhile(ti => !Equals(ti, si)).ToList();

      double leftItemsWidth = leftItems.Sum(ti => ti.ActualWidth);

      //If the selected item is situated somewhere at the right area
      if ( leftItemsWidth + si.ActualWidth > _scrollViewer.HorizontalOffset + _scrollViewer.ViewportWidth )
      {
        double currentHorizontalOffset = (leftItemsWidth + si.ActualWidth) - _scrollViewer.ViewportWidth;
        // the selected item has extra width, so I add it to the offset
        double hMargin = !leftItems.Any(ti => ti.IsSelected) && !si.IsSelected ? _headerPanel.Margin.Left + _headerPanel.Margin.Right : 0;
        currentHorizontalOffset += hMargin;

        _scrollViewer.ScrollToHorizontalOffset(currentHorizontalOffset);
      }
      //if the selected item somewhere at the left
      else if ( leftItemsWidth < _scrollViewer.HorizontalOffset )
      {
        double currentHorizontalOffset = leftItemsWidth;
        // the selected item has extra width, so I remove it from the offset
        double hMargin = leftItems.Any(ti => ti.IsSelected) ? _headerPanel.Margin.Left + _headerPanel.Margin.Right : 0;
        currentHorizontalOffset -= hMargin;

        _scrollViewer.ScrollToHorizontalOffset(currentHorizontalOffset);
      }
    }

    private TabItem GetItemByOffset(double offset)
    {
      var tabItems = Items.Cast<object>().Select(item => ItemContainerGenerator.ContainerFromItem(item) as TabItem).ToList();
      double currentItemsWidth = 0;

      // get tabs one by one and calculate their aggregated width until the offset value is reached
      foreach ( var ti in tabItems )
      {
        if ( currentItemsWidth + ti.ActualWidth >= offset )
          return ti;

        currentItemsWidth += ti.ActualWidth;
      }
      return tabItems.LastOrDefault();
    }

    private void UpdateWindowLocation(double left, double top, DragSupportTabItem tabItem)
    {
      if ( _dragToWindow == null )
      {
        lock ( MyLockWindow )
        {
          _dragToWindow = DragWindow.CreateTabWindow(left, top, ActualWidth, ActualHeight, tabItem);
        }
      }

      if ( _dragToWindow == null )
        return;


      _dragToWindow.Left = left;
      _dragToWindow.Top = top;
    }

    #endregion
  }
}
