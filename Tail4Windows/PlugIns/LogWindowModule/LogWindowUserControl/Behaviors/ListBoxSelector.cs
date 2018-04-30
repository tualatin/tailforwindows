using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule.LogWindowUserControl.Behaviors
{
  /// <summary>
  /// ListBoxSelector
  /// </summary>
  public sealed class ListBoxSelector
  {
    /// <summary>
    /// This stores the ListBoxSelector for each ListBox so we can unregister it.
    /// </summary>
    private static readonly Dictionary<ListBox, ListBoxSelector> AttachedControls = new Dictionary<ListBox, ListBoxSelector>();

    private readonly ListBox _listBox;
    private ScrollContentPresenter _scrollContent;

    private SelectionAdorner _selectionRect;
    private AutoScroller _autoScroller;
    private ItemsControlSelector _selector;

    private MouseButtonEventArgs _lbdEventArgs;
    private bool _mouseCaptured;

    private Point _start;
    private Point _end;

    /// <summary>
    /// Identifies the IsEnabled attached property.
    /// </summary>
    public static readonly DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached("Enabled", typeof(bool), typeof(ListBoxSelector),
      new UIPropertyMetadata(false, IsEnabledChangedCallback));

    /// <summary>
    /// Gets the value of the IsEnabled attached property that indicates
    /// whether a selection rectangle can be used to select items or not.
    /// </summary>
    /// <param name="obj">Object on which to get the property.</param>
    /// <returns>
    /// true if items can be selected by a selection rectangle; otherwise, false.
    /// </returns>
    public static bool GetEnabled(DependencyObject obj) => (bool) obj.GetValue(EnabledProperty);

    /// <summary>
    /// Sets the value of the IsEnabled attached property that indicates
    /// whether a selection rectangle can be used to select items or not.
    /// </summary>
    /// <param name="obj">Object on which to set the property.</param>
    /// <param name="value">Value to set.</param>
    public static void SetEnabled(DependencyObject obj, bool value) => obj.SetValue(EnabledProperty, value);

    private static void IsEnabledChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if ( !(d is ListBox listBox) )
        return;

      if ( (bool) e.NewValue )
      {
        // If we're enabling selection by a rectangle we can assume
        // this means we want to be able to select more than one item.
        if ( listBox.SelectionMode == SelectionMode.Single )
          listBox.SelectionMode = SelectionMode.Extended;

        AttachedControls.Add(listBox, new ListBoxSelector(listBox));
      }
      else // Unregister the selector
      {
        if ( !AttachedControls.TryGetValue(listBox, out var selector) )
          return;

        AttachedControls.Remove(listBox);
        selector.Unregister();
      }
    }

    private ListBoxSelector(ListBox listBox)
    {
      _listBox = listBox;

      if ( _listBox.IsLoaded )
      {
        Register();
      }
      else
      {
        // We need to wait for it to be loaded so we can find the
        // child controls.
        _listBox.Loaded += OnListBoxLoaded;
      }
    }

    /// <summary>
    /// Finds the nearest child of the specified type, or null if one wasn't found.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reference"></param>
    /// <returns></returns>
    private static T FindChild<T>(DependencyObject reference) where T : class
    {
      // Do a breadth first search.
      var queue = new Queue<DependencyObject>();
      queue.Enqueue(reference);

      while ( queue.Count > 0 )
      {
        var child = queue.Dequeue();
        T obj = child as T;

        if ( obj != null )
          return obj;

        // Add the children to the queue to search through later.
        for ( int i = 0; i < VisualTreeHelper.GetChildrenCount(child); i++ )
        {
          queue.Enqueue(VisualTreeHelper.GetChild(child, i));
        }
      }
      return null; // Not found.
    }

    private bool Register()
    {
      _scrollContent = FindChild<ScrollContentPresenter>(_listBox);

      if ( _scrollContent == null )
        return _scrollContent != null;

      _autoScroller = new AutoScroller(_listBox);
      _autoScroller.OffsetChanged += OnOffsetChanged;

      _selectionRect = new SelectionAdorner(_scrollContent);
      _scrollContent.AdornerLayer.Add(_selectionRect);

      _selector = new ItemsControlSelector(_listBox);

      // The ListBox intercepts the regular MouseLeftButtonDown event
      // to do its selection processing, so we need to handle the
      // PreviewMouseLeftButtonDown. The scroll content won't receive
      // the message if we click on a blank area so use the ListBox.
      _listBox.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
      _listBox.MouseLeftButtonUp += OnMouseLeftButtonUp;
      _listBox.MouseMove += OnMouseMove;
      _listBox.MouseLeftButtonDown += OnMouseLeftButtonDown;

      // Return success if we found the ScrollContentPresenter
      return _scrollContent != null;
    }

    private void Unregister()
    {
      StopSelection();

      // Remove all the event handlers so this instance can be reclaimed by the GC.
      _listBox.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
      _listBox.MouseLeftButtonUp -= OnMouseLeftButtonUp;
      _listBox.MouseMove -= OnMouseMove;
      _listBox.MouseLeftButtonDown -= OnMouseLeftButtonDown;

      _autoScroller.Unregister();
    }

    private void OnListBoxLoaded(object sender, EventArgs e)
    {
      if ( Register() )
        _listBox.Loaded -= OnListBoxLoaded;
    }

    private void OnOffsetChanged(object sender, OffsetChangedEventArgs e)
    {
      _selector.Scroll(e.HorizontalChange, e.VerticalChange);
      UpdateSelection();
    }

    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if ( _mouseCaptured )
      {
        _mouseCaptured = false;
        _scrollContent.ReleaseMouseCapture();
        StopSelection();
      }

      _lbdEventArgs = null;
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      var mouse = e.GetPosition(_scrollContent);

      if ( mouse.X >= 0 && mouse.X < _scrollContent.ActualWidth &&
          mouse.Y >= 0 && mouse.Y < _scrollContent.ActualHeight )
      {
        if ( (Keyboard.Modifiers & ModifierKeys.Control) == 0 &&
            (Keyboard.Modifiers & ModifierKeys.Shift) == 0 )
          // Neither the shift key or control key is pressed, so
          // clear the selection.
          _listBox.SelectedItems.Clear();
      }
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
      if ( _lbdEventArgs != null )
      {
        var e2 = _lbdEventArgs;
        _lbdEventArgs = null;

        _mouseCaptured = TryCaptureMouse(e2);
        var mouse = e2.GetPosition(_scrollContent);

        if ( _mouseCaptured )
          StartSelection(mouse);
      }

      if ( !_mouseCaptured )
        return;

      // Get the position relative to the content of the ScrollViewer.
      _end = e.GetPosition(_scrollContent);
      _autoScroller.Update(_end);
      UpdateSelection();
    }

    private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      // Check that the mouse is inside the scroll content (could be on the
      // scroll bars for example).
      var mouse = e.GetPosition(_scrollContent);

      if ( mouse.X >= 0 && mouse.X < _scrollContent.ActualWidth &&
          mouse.Y >= 0 && mouse.Y < _scrollContent.ActualHeight )
        _lbdEventArgs = e;
      else
        _lbdEventArgs = null;

      //if (mouse.X < 0 || mouse.X >= this.scrollContent.ActualWidth || mouse.Y < 0 || mouse.Y >= this.scrollContent.ActualHeight)
      //  return;

      //this.mouseCaptured = this.TryCaptureMouse (e);

      //if (this.mouseCaptured)
      //  this.StartSelection (mouse);
    }

    private bool TryCaptureMouse(MouseEventArgs e)
    {
      var position = e.GetPosition(_scrollContent);

      // Check if there is anything under the mouse.
      if ( !(_scrollContent.InputHitTest(position) is UIElement element) )
        return _scrollContent.CaptureMouse();

      // Simulate a mouse click by sending it the MouseButtonDown
      // event based on the data we received.
      var args = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left, e.StylusDevice)
      {
        RoutedEvent = Mouse.MouseDownEvent,
        Source = e.Source
      };
      element.RaiseEvent(args);

      // The ListBox will try to capture the mouse unless something
      // else captures it.
      // Either there's nothing under the mouse or the element doesn't want the mouse.
      return Equals(Mouse.Captured, _listBox) && _scrollContent.CaptureMouse();
    }

    private void StopSelection()
    {
      // Hide the selection rectangle and stop the auto scrolling.
      _selectionRect.IsEnabled = false;
      _autoScroller.IsEnabled = false;
    }

    private void StartSelection(Point location)
    {
      // We've stolen the MouseLeftButtonDown event from the ListBox
      // so we need to manually give it focus.
      if ( !_listBox.IsKeyboardFocusWithin )
        _listBox.Focus();

      _start = location;
      _end = location;

      // Do we need to start a new selection?
      if ( (Keyboard.Modifiers & ModifierKeys.Control) == 0 &&
          (Keyboard.Modifiers & ModifierKeys.Shift) == 0 )
        // Neither the shift key or control key is pressed, so
        // clear the selection.
        _listBox.SelectedItems.Clear();

      _selector.Reset();
      UpdateSelection();

      _selectionRect.IsEnabled = true;
      _autoScroller.IsEnabled = true;
    }

    private void UpdateSelection()
    {
      // Offset the start point based on the scroll offset.
      var st = _autoScroller.TranslatePoint(_start);

      // Draw the selecion rectangle.
      // Rect can't have a negative width/height...
      double x = Math.Min(st.X, _end.X);
      double y = Math.Min(st.Y, _end.Y);
      double width = Math.Abs(_end.X - st.X);
      double height = Math.Abs(_end.Y - st.Y);
      var area = new Rect(x, y, width, height);
      _selectionRect.SelectionArea = area;

      // Select the items.
      // Transform the points to be relative to the ListBox.
      var topLeft = _scrollContent.TranslatePoint(area.TopLeft, _listBox);
      var bottomRight = _scrollContent.TranslatePoint(area.BottomRight, _listBox);

      // And select the items.
      _selector.UpdateSelection(new Rect(topLeft, bottomRight));
    }

    /// <summary>
    /// Automatically scrolls an ItemsControl when the mouse is dragged outside
    /// of the control.
    /// </summary>
    private sealed class AutoScroller
    {
      private readonly DispatcherTimer _autoScroll = new DispatcherTimer();
      private readonly ItemsControl _itemsControl;
      private readonly ScrollViewer _scrollViewer;
      private readonly ScrollContentPresenter _scrollContent;
      private bool _isEnabled;
      private Point _offset;
      private Point _mouse;


      /// <summary>
      /// Initializes a new instance of the AutoScroller class.
      /// </summary>
      /// <param name="itemsControl">The ItemsControl that is scrolled.</param>
      /// <exception cref="ArgumentNullException">itemsControl is null.</exception>
      public AutoScroller(ItemsControl itemsControl)
      {
        _itemsControl = itemsControl ?? throw new ArgumentNullException(nameof(itemsControl));
        _scrollViewer = FindChild<ScrollViewer>(itemsControl);
        _scrollViewer.ScrollChanged += OnScrollChanged;
        _scrollContent = FindChild<ScrollContentPresenter>(_scrollViewer);

        _autoScroll.Tick += delegate
        {
          PreformScroll();
        };
        _autoScroll.Interval = TimeSpan.FromMilliseconds(GetRepeatRate());
      }

      /// <summary>
      /// Occurs when the scroll offset has changed.
      /// </summary>
      public event EventHandler<OffsetChangedEventArgs> OffsetChanged;

      /// <summary>
      /// Gets or sets a value indicating whether the auto-scroller is enabled
      /// or not.
      /// </summary>
      public bool IsEnabled
      {
        private get => _isEnabled;
        set
        {
          if ( _isEnabled == value )
            return;

          _isEnabled = value;

          // Reset the auto-scroller and offset.
          _autoScroll.IsEnabled = false;
          _offset = new Point();
        }
      }

      /// <summary>
      /// Translates the specified point by the current scroll offset.
      /// </summary>
      /// <param name="point">The point to translate.</param>
      /// <returns>A new point offset by the current scroll amount.</returns>
      public Point TranslatePoint(Point point) => new Point(point.X - _offset.X, point.Y - _offset.Y);

      /// <summary>
      /// Removes all the event handlers registered on the control.
      /// </summary>
      public void Unregister() => _scrollViewer.ScrollChanged -= OnScrollChanged;

      /// <summary>
      /// Updates the location of the mouse and automatically scrolls if required.
      /// </summary>
      /// <param name="msPoint">The location of the mouse, relative to the ScrollViewer's content.</param>
      public void Update(Point msPoint)
      {
        _mouse = msPoint;

        // If scrolling isn't enabled then see if it needs to be.
        if ( !_autoScroll.IsEnabled )
          PreformScroll();
      }

      /// <summary>
      /// Returns the default repeat rate in milliseconds.
      /// </summary>
      /// <returns>Returns milliseconds</returns>
      private static int GetRepeatRate()
      {
        // The RepeatButton uses the SystemParameters.KeyboardSpeed as the
        // default value for the Interval property. KeyboardSpeed returns
        // a value between 0 (400ms) and 31 (33ms).
        const double ratio = (400.0 - 33.0) / 31.0;

        return 400 - (int) (SystemParameters.KeyboardSpeed * ratio);
      }

      private double CalculateOffset(int startIndex, int endIndex)
      {
        double sum = 0;

        for ( int i = startIndex; i != endIndex; i++ )
        {
          if ( !(_itemsControl.ItemContainerGenerator.ContainerFromIndex(i) is FrameworkElement container) )
            continue;

          // Height = Actual height + margin
          sum += container.ActualHeight;
          sum += container.Margin.Top + container.Margin.Bottom;
        }
        return sum;
      }

      private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
      {
        // Do we need to update the offset?
        if ( !IsEnabled )
          return;

        double horizontal = e.HorizontalChange;
        double vertical = e.VerticalChange;

        // VerticalOffset means two seperate things based on the CanContentScroll
        // property. If this property is true then the offset is the number of
        // items to scroll; false then it's in Device Independant Pixels (DIPs).
        if ( _scrollViewer.CanContentScroll )
        {
          // We need to either increase the offset or decrease it.
          if ( e.VerticalChange < 0 )
          {
            int start = (int) e.VerticalOffset;
            int end = (int) (e.VerticalOffset - e.VerticalChange);
            vertical = -CalculateOffset(start, end);
          }
          else
          {
            int start = (int) (e.VerticalOffset - e.VerticalChange);
            int end = (int) e.VerticalOffset;
            vertical = CalculateOffset(start, end);
          }
        }

        _offset.X += horizontal;
        _offset.Y += vertical;

        OffsetChanged?.Invoke(this, new OffsetChangedEventArgs(horizontal, vertical));
      }

      private void PreformScroll()
      {
        bool scrolled = false;

        if ( _mouse.X > _scrollContent.ActualWidth )
        {
          _scrollViewer.LineRight();
          scrolled = true;
        }
        else if ( _mouse.X < 0 )
        {
          _scrollViewer.LineLeft();
          scrolled = true;
        }

        if ( _mouse.Y > _scrollContent.ActualHeight )
        {
          _scrollViewer.LineDown();
          scrolled = true;
        }
        else if ( _mouse.Y < 0 )
        {
          _scrollViewer.LineUp();
          scrolled = true;
        }

        // It's important to disable scrolling if we're inside the bounds of
        // the control so that when the user does leave the bounds we can
        // re-enable scrolling and it will have the correct initial delay.
        _autoScroll.IsEnabled = scrolled;
      }
    }

    /// <summary>
    /// Enables the selection of items by a specified rectangle.
    /// </summary>
    private sealed class ItemsControlSelector
    {
      private readonly ItemsControl _itemsControl;
      private Rect _previousArea;


      /// <summary>
      /// Initializes a new instance of the ItemsControlSelector class.
      /// </summary>
      /// <param name="itemsControl">
      /// The control that contains the items to select.
      /// </param>
      /// <exception cref="ArgumentNullException">itemsControl is null.</exception>
      public ItemsControlSelector(ItemsControl itemsControl) => _itemsControl = itemsControl ?? throw new ArgumentNullException(nameof(itemsControl));

      /// <summary>
      /// Resets the cached information, allowing a new selection to begin.
      /// </summary>
      public void Reset() => _previousArea = new Rect();

      /// <summary>
      /// Scrolls the selection area by the specified amount.
      /// </summary>
      /// <param name="x">The horizontal scroll amount.</param>
      /// <param name="y">The vertical scroll amount.</param>
      public void Scroll(double x, double y) => _previousArea.Offset(-x, -y);

      /// <summary>
      /// Updates the controls selection based on the specified area.
      /// </summary>
      /// <param name="area">
      /// The selection area, relative to the control passed in the contructor.
      /// </param>
      public void UpdateSelection(Rect area)
      {
        // Check each item to see if it intersects with the area.
        for ( int i = 0; i < _itemsControl.Items.Count; i++ )
        {
          if ( !(_itemsControl.ItemContainerGenerator.ContainerFromIndex(i) is FrameworkElement item) )
            continue;

          // Get the bounds in the parent's co-ordinates.
          var topLeft = item.TranslatePoint(new Point(0, 0), _itemsControl);
          var itemBounds = new Rect(topLeft.X, topLeft.Y, item.ActualWidth, item.ActualHeight);

          // Only change the selection if it intersects with the area
          // (or intersected i.e. we changed the value last time).
          if ( itemBounds.IntersectsWith(area) )
            Selector.SetIsSelected(item, true);
          else if ( itemBounds.IntersectsWith(_previousArea) )
          {
            // We previously changed the selection to true but it no
            // longer intersects with the area so clear the selection.
            Selector.SetIsSelected(item, false);
          }
        }

        _previousArea = area;
      }
    }

    /// <summary>
    /// The event data for the AutoScroller.OffsetChanged event.
    /// </summary>
    private sealed class OffsetChangedEventArgs : EventArgs
    {
      /// <summary>
      /// Initializes a new instance of the OffsetChangedEventArgs class.
      /// </summary>
      /// <param name="horizontal">The change in horizontal scroll.</param>
      /// <param name="vertical">The change in vertical scroll.</param>
      internal OffsetChangedEventArgs(double horizontal, double vertical)
      {
        HorizontalChange = horizontal;
        VerticalChange = vertical;
      }

      /// <summary>
      /// Gets the change in horizontal scroll position.
      /// </summary>
      public double HorizontalChange
      {
        get;
      }

      /// <summary>
      /// Gets the change in vertical scroll position.
      /// </summary>
      public double VerticalChange
      {
        get;
      }
    }

    /// <summary>
    /// Draws a selection rectangle on an AdornerLayer.
    /// </summary>
    private sealed class SelectionAdorner : Adorner
    {
      private Rect _selectionRect;


      /// <summary>
      /// Initializes a new instance of the SelectionAdorner class.
      /// </summary>
      /// <param name="parent">
      /// The UIElement which this instance will overlay.
      /// </param>
      /// <exception cref="ArgumentNullException">parent is null.</exception>
      public SelectionAdorner(UIElement parent)
        : base(parent)
      {
        // Make sure the mouse doesn't see us.
        IsHitTestVisible = false;

        // We only draw a rectangle when we're enabled.
        IsEnabledChanged += delegate
        {
          InvalidateVisual();
        };
      }

      /// <summary>
      /// Gets or sets the area of the selection rectangle.
      /// </summary>
      public Rect SelectionArea
      {
        private get => _selectionRect;
        set
        {
          _selectionRect = value;
          InvalidateVisual();
        }
      }

      /// <summary>
      /// Participates in rendering operations that are directed by the layout system.
      /// </summary>
      /// <param name="drawingContext">The drawing instructions.</param>
      protected override void OnRender(DrawingContext drawingContext)
      {
        base.OnRender(drawingContext);

        if ( !IsEnabled )
          return;

        // Make the lines snap to pixels (add half the pen width [0.5])
        double[] x = { SelectionArea.Left + 0.5, SelectionArea.Right + 0.5 };
        double[] y = { SelectionArea.Top + 0.5, SelectionArea.Bottom + 0.5 };
        drawingContext.PushGuidelineSet(new GuidelineSet(x, y));

        Brush fill = SystemColors.HighlightBrush.Clone();
        fill.Opacity = 0.4;
        drawingContext.DrawRectangle(fill, new Pen(SystemColors.HighlightBrush, 1.0), SelectionArea);
      }
    }
  }
}
