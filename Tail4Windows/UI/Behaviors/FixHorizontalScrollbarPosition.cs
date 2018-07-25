using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Org.Vs.TailForWin.UI.Behaviors.Base;
using Org.Vs.TailForWin.UI.Extensions;
using Org.Vs.TailForWin.UI.UserControls;


namespace Org.Vs.TailForWin.UI.Behaviors
{
  /// <summary>
  /// Fix horizontal scrollbar position
  /// </summary>
  public class FixHorizontalScrollbarPosition : BehaviorBase<FrameworkElement>
  {
    private Grid _horizontalScrollBarGrid;

    /// <summary>
    /// Dependency-Property FixedMargin
    /// </summary>
    public static readonly DependencyProperty FixedMarginProperty = DependencyProperty.Register(nameof(FixedMargin), typeof(Thickness), typeof(FixHorizontalScrollbarPosition),
      new PropertyMetadata(new Thickness(0, 0, 0, 0)));

    /// <summary>
    /// Gets / sets fixed margin
    /// </summary>
    public Thickness FixedMargin
    {
      get => (Thickness) GetValue(FixedMarginProperty);
      set => SetValue(FixedMarginProperty, value);
    }

    /// <summary>
    /// Setup <see cref="BehaviorBase{T}"/>
    /// </summary>
    protected override void OnSetup()
    {
      AssociatedObject.Loaded += OnLoaded;
      AssociatedObject.Unloaded += OnUnloaded;
    }

    /// <summary>
    /// Release all resource used by <see cref="BehaviorBase{T}"/>
    /// </summary>
    protected override void OnCleanup()
    {
      if ( AssociatedObject == null )
        return;

      AssociatedObject.Loaded -= OnLoaded;
      AssociatedObject.Unloaded -= OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e) => AssociatedObject?.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(OnScrollChanged));

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
      _horizontalScrollBarGrid = null;
      AssociatedObject?.RemoveHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(OnScrollChanged));
    }

    private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
    {
      if ( _horizontalScrollBarGrid != null )
        return;

      if ( !(sender is FrameworkElement frameworkElement) )
        return;
      
      var scrollViewer = frameworkElement.Descendents().OfType<ScrollViewer>().FirstOrDefault();

      if ( scrollViewer == null )
        return;

      var scrollView = e.OriginalSource as ScrollViewer;

      if ( !Equals(scrollViewer, scrollView) )
        return;

      _horizontalScrollBarGrid = VsDataGrid.GetHorizontalScrollBarGrid(scrollViewer);

      if ( _horizontalScrollBarGrid == null )
        return;

      _horizontalScrollBarGrid.Margin = FixedMargin;
    }
  }
}
