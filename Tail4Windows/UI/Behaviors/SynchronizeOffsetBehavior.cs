using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Org.Vs.TailForWin.UI.Behaviors.Base;
using Org.Vs.TailForWin.UI.Extensions;


namespace Org.Vs.TailForWin.UI.Behaviors
{
  /// <summary>
  /// SynchronizeOffset behavior
  /// </summary>
  public class SynchronizeOffsetBehavior : BehaviorBase<FrameworkElement>
  {
    private ScrollViewer _scrollViewer;

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

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      if ( sender is FrameworkElement frameworkElement )
        _scrollViewer = frameworkElement.Descendents().OfType<ScrollViewer>().FirstOrDefault();

      AssociatedObject?.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(OnScrollChanged));
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
      _scrollViewer = null;
      AssociatedObject?.RemoveHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(OnScrollChanged));
    }

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
