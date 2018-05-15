using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using Org.Vs.TailForWin.UI.Extensions;


namespace Org.Vs.TailForWin.UI.Behaviors
{
  /// <summary>
  /// SynchronizeOffset behavior
  /// </summary>
  public class SynchronizeOffsetBehavior : Behavior<FrameworkElement>
  {
    private ScrollViewer _scrollViewer;

    /// <summary>
    /// Called after the behavior is attached to an AssociatedObject.
    /// </summary>
    /// <remarks>
    /// Override this to hook up functionality to the AssociatedObject.
    /// </remarks>
    protected override void OnAttached()
    {
      base.OnAttached();

      AssociatedObject.Loaded += OnLoaded;
      AssociatedObject.Unloaded += OnUnloaded;
    }

    /// <summary>
    /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
    /// </summary>
    /// <remarks>
    /// Override this to unhook functionality from the AssociatedObject.
    /// </remarks>
    protected override void OnDetaching()
    {
      base.OnDetaching();

      if ( AssociatedObject == null )
        return;

      AssociatedObject.Loaded -= OnLoaded;
      AssociatedObject.Unloaded -= OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      if ( sender is FrameworkElement frameworkElement )
        _scrollViewer = frameworkElement.Descendents().OfType<ScrollViewer>().FirstOrDefault();

      AssociatedObject?.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(OnScrollChanged));
    }

    private void OnUnloaded(object sender, RoutedEventArgs e) => AssociatedObject?.RemoveHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(OnScrollChanged));

    /// <summary>
    /// Dependency-Property TargetElement
    /// </summary>
    public static readonly DependencyProperty TargetElementProperty = DependencyProperty.Register(nameof(TargetElement), typeof(FrameworkElement), typeof(SynchronizeOffsetBehavior),
      new PropertyMetadata(null));

    /// <summary>
    /// Gets / sets target element
    /// </summary>
    public FrameworkElement TargetElement
    {
      get => (FrameworkElement) GetValue(TargetElementProperty);
      set => SetValue(TargetElementProperty, value);
    }

    private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
    {
      if ( _scrollViewer == null )
        return;

      if ( e.HorizontalOffset <= 16 && e.HorizontalChange > 0 )
        _scrollViewer.ScrollToHorizontalOffset(e.HorizontalOffset * 2);
      else if ( e.HorizontalOffset <= 16 && e.HorizontalChange < 0 )
        _scrollViewer.ScrollToHorizontalOffset(0);

      _scrollViewer.Padding = _scrollViewer.HorizontalOffset <= 0 ? new Thickness(0, 0, 0, 0) : new Thickness(21, 0, 0, 0);
    }
  }
}
