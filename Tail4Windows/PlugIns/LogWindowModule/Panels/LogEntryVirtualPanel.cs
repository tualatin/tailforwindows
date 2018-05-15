using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using log4net;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule.Panels
{
  /// <summary>
  /// LogEntry virtual panel
  /// </summary>
  public class LogEntryVirtualPanel : VirtualizingPanel, IScrollInfo
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(LogEntryVirtualPanel));

    private readonly TranslateTransform _translateTransform;
    private Size _extentView;
    private Size _viewPort;
    private Point _offset;

    private ItemsControl _itemsControl;


    private const double ChildSize = 1;

    /// <summary>
    /// Standard constructor
    /// </summary>
    public LogEntryVirtualPanel()
    {
      Loaded += OnLoaded;

      _extentView = new Size(0, 0);
      _viewPort = new Size(0, 0);
      _translateTransform = new TranslateTransform();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      LOG.Trace("LogEntryVirtualPanel loaded...");

      _itemsControl = ItemsControl.GetItemsOwner(this);
      RenderTransform = _translateTransform;

      Loaded -= OnLoaded;
    }

    /// <summary>
    /// Measure the children
    /// </summary>
    /// <param name="availableSize">Size available</param>
    /// <returns>Size desired</returns>
    protected override Size MeasureOverride(Size availableSize)
    {
      if ( _itemsControl == null )
        return availableSize;

      UpdateScrollInfo(availableSize);

      // Figure out range that's visible based on layout algorithm
      GetVisibleRange(out int firstVisibleItemIndex, out int lastVisibleItemIndex);

      // We need to access InternalChildren before the generator to work around a bug
      var children = InternalChildren;
      var generator = ItemContainerGenerator;

      // Get the generator position of the first visible data item
      var startPos = generator.GeneratorPositionFromIndex(firstVisibleItemIndex);

      // Get index where we'd insert the child for this position. If the item is realized
      // (position.Offset == 0), it's just position.Index, otherwise we have to add one to
      // insert after the corresponding child
      int childIndex = (startPos.Offset == 0) ? startPos.Index : startPos.Index + 1;

      using ( generator.StartAt(startPos, GeneratorDirection.Forward, true) )
      {
        for ( int itemIndex = firstVisibleItemIndex; itemIndex <= lastVisibleItemIndex; ++itemIndex, ++childIndex )
        {
          // Get or create the child
          var child = generator.GenerateNext(out bool newlyRealized) as UIElement;

          if ( newlyRealized )
          {
            // Figure out if we need to insert the child at the end or somewhere in the middle
            if ( childIndex >= children.Count )
              AddInternalChild(child);
            else
              InsertInternalChild(childIndex, child);

            generator.PrepareItemContainer(child);
          }

          // Measurements will depend on layout algorithm
          child.Measure(new Size(100, 16));
        }
      }

      // Note: this could be deferred to idle time for efficiency
      CleanUpItems(firstVisibleItemIndex, lastVisibleItemIndex);

      return availableSize;
    }

    /// <summary>
    /// Arrange the children
    /// </summary>
    /// <param name="finalSize">Size available</param>
    /// <returns>Size used</returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
      var generator = ItemContainerGenerator;

      UpdateScrollInfo(finalSize);

      for ( int i = 0; i < Children.Count; i++ )
      {
        var child = Children[i];
        int itemIndex = generator.IndexFromGeneratorPosition(new GeneratorPosition(i, 0));

        ArrangeChild(itemIndex, child, finalSize);
      }
      return finalSize;
    }

    /// <summary>
    /// When items are removed, remove the corresponding UI if necessary
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
    {
      switch ( args.Action )
      {
      case NotifyCollectionChangedAction.Remove:
      case NotifyCollectionChangedAction.Replace:
      case NotifyCollectionChangedAction.Move:

        RemoveInternalChildRange(args.Position.Index, args.ItemUICount);
        break;
      }
    }

    #region HelperFunctions

    private double GetControlTextHeight(Control control) => Math.Ceiling(control.FontSize * control.FontFamily.LineSpacing);

    private void UpdateScrollInfo(Size availableSize)
    {
      if ( _itemsControl == null )
        return;

      int itemCount = _itemsControl.HasItems ? _itemsControl.Items.Count : 0;
      var extent = CalculateExtent(availableSize, itemCount);

      if ( extent != _extentView )
      {
        _extentView = extent;
        ScrollOwner?.InvalidateScrollInfo();
      }

      if ( availableSize == _viewPort )
        return;

      _viewPort = availableSize;
      ScrollOwner?.InvalidateScrollInfo();
    }

    private Size CalculateExtent(Size availableSize, int itemCount)
    {
      const int childrenPerRow = 1;
      return new Size(childrenPerRow * ChildSize, GetControlTextHeight(_itemsControl) * Math.Ceiling((double) itemCount / childrenPerRow));
    }

    private void ArrangeChild(int itemIndex, UIElement child, Size finalSize)
    {
      const int childrenPerRow = 1;
      int row = itemIndex / childrenPerRow;
      int column = itemIndex % childrenPerRow;

      child.Arrange(new Rect(column * ChildSize, row * ChildSize, ChildSize, ChildSize));
    }

    /// <summary>
    /// Get the range of children that are visible
    /// </summary>
    /// <param name="firstVisibleItemIndex">The item index of the first visible item</param>
    /// <param name="lastVisibleItemIndex">The item index of the last visible item</param>
    private void GetVisibleRange(out int firstVisibleItemIndex, out int lastVisibleItemIndex)
    {
      int childrenPerRow = 1;

      firstVisibleItemIndex = (int) Math.Floor(_offset.Y / ChildSize) * childrenPerRow;
      lastVisibleItemIndex = (int) Math.Ceiling((_offset.Y + _viewPort.Height) / ChildSize) * childrenPerRow - 1;
      int itemCount = _itemsControl.HasItems ? _itemsControl.Items.Count : 0;

      if ( lastVisibleItemIndex >= itemCount )
        lastVisibleItemIndex = itemCount - 1;
    }

    /// <summary>
    /// Revirtualize items that are no longer visible
    /// </summary>
    /// <param name="minDesiredGenerated">first item index that should be visible</param>
    /// <param name="maxDesiredGenerated">last item index that should be visible</param>
    private void CleanUpItems(int minDesiredGenerated, int maxDesiredGenerated)
    {
      var children = InternalChildren;
      var generator = ItemContainerGenerator;

      for ( int i = children.Count - 1; i >= 0; i-- )
      {
        var childGeneratorPos = new GeneratorPosition(i, 0);
        int itemIndex = generator.IndexFromGeneratorPosition(childGeneratorPos);

        if ( itemIndex >= minDesiredGenerated && itemIndex <= maxDesiredGenerated )
          continue;

        generator.Remove(childGeneratorPos, 1);
        RemoveInternalChildRange(i, 1);
      }
    }

    #endregion

    /// <summary>
    /// Can vertically scroll
    /// </summary>
    public bool CanVerticallyScroll
    {
      get;
      set;
    }

    /// <summary>
    /// Can horizontally scroll
    /// </summary>
    public bool CanHorizontallyScroll
    {
      get;
      set;
    }

    /// <summary>
    /// Extent width
    /// </summary>
    public double ExtentWidth => _extentView.Width;

    /// <summary>
    /// Extent height
    /// </summary>
    public double ExtentHeight => _extentView.Height;

    /// <summary>
    /// ViewPort width
    /// </summary>
    public double ViewportWidth => _viewPort.Width;

    /// <summary>
    /// ViewPort height
    /// </summary>
    public double ViewportHeight => _viewPort.Height;

    /// <summary>
    /// Horizontal offset
    /// </summary>
    public double HorizontalOffset => _offset.X;

    /// <summary>
    /// Vertical offset
    /// </summary>
    public double VerticalOffset => _offset.Y;

    /// <summary>
    /// ScrollViewer
    /// </summary>
    public ScrollViewer ScrollOwner
    {
      get;
      set;
    }

    /// <summary>
    /// Line down
    /// </summary>
    public void LineDown() => SetVerticalOffset(VerticalOffset + 10);

    public void LineLeft()
    {
    }

    public void LineRight()
    {
    }

    /// <summary>
    /// Line up
    /// </summary>
    public void LineUp() => SetVerticalOffset(VerticalOffset - 10);

    public Rect MakeVisible(Visual visual, Rect rectangle)
    {
      return new Rect();
    }

    /// <summary>
    /// MouseWheel down
    /// </summary>
    public void MouseWheelDown() => SetVerticalOffset(VerticalOffset + 10);

    public void MouseWheelLeft()
    {
    }

    public void MouseWheelRight()
    {
    }

    /// <summary>
    /// MouseWheel up
    /// </summary>
    public void MouseWheelUp() => SetVerticalOffset(VerticalOffset - 10);

    /// <summary>
    /// Page down
    /// </summary>
    public void PageDown() => SetVerticalOffset(VerticalOffset + _viewPort.Height);

    public void PageLeft()
    {
    }

    public void PageRight()
    {
    }

    /// <summary>
    /// Page up
    /// </summary>
    public void PageUp() => SetVerticalOffset(VerticalOffset - _viewPort.Height);

    /// <summary>
    /// Set horizontal offset
    /// </summary>
    /// <param name="offset">Offset</param>
    public void SetHorizontalOffset(double offset)
    {
    }

    /// <summary>
    /// Set vertical offset
    /// </summary>
    /// <param name="offset">Offset</param>
    public void SetVerticalOffset(double offset)
    {
      if ( offset < 0 || _viewPort.Height >= _extentView.Height )
      {
        offset = 0;
      }
      else
      {
        if ( offset + _viewPort.Height >= _extentView.Height )
          offset = _extentView.Height - _viewPort.Height;
      }

      _offset.Y = offset;
      ScrollOwner?.InvalidateScrollInfo();

      _translateTransform.Y = -offset;

      // Force us to realize the correct children
      InvalidateMeasure();
    }
  }
}
