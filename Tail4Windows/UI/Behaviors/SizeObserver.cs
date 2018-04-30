using System.Windows;


namespace Org.Vs.TailForWin.UI.Behaviors
{
  /// <summary>
  /// SizeObserver
  /// </summary>
  public static class SizeObserver
  {
    /// <summary>
    /// Observe property
    /// </summary>
    public static readonly DependencyProperty ObserveProperty = DependencyProperty.RegisterAttached("Observe", typeof(bool), typeof(SizeObserver),
      new FrameworkPropertyMetadata(OnObserveChanged));

    /// <summary>
    /// GetObserve
    /// </summary>
    /// <param name="frameworkElement"><see cref="FrameworkElement"/></param>
    /// <returns><c>True</c> otherwise <c>False</c></returns>
    public static bool GetObserve(FrameworkElement frameworkElement) => (bool) frameworkElement.GetValue(ObserveProperty);

    /// <summary>
    /// SetObserve
    /// </summary>
    /// <param name="frameworkElement"><see cref="FrameworkElement"/></param>
    /// <param name="observe">Set observe</param>
    public static void SetObserve(FrameworkElement frameworkElement, bool observe) => frameworkElement.SetValue(ObserveProperty, observe);

    /// <summary>
    /// ObserveWidth property
    /// </summary>
    public static readonly DependencyProperty ObservedWidthProperty = DependencyProperty.RegisterAttached("ObservedWidth", typeof(double), typeof(SizeObserver));

    /// <summary>
    /// Get ObserveWidth
    /// </summary>
    /// <param name="frameworkElement"><see cref="FrameworkElement"/></param>
    /// <returns><see cref="double"/> value</returns>
    public static double GetObservedWidth(FrameworkElement frameworkElement) => (double) frameworkElement.GetValue(ObservedWidthProperty);

    /// <summary>
    /// Set ObserveWidth
    /// </summary>
    /// <param name="frameworkElement"><see cref="FrameworkElement"/></param>
    /// <param name="observedWidth">Set ObserveWidth</param>
    public static void SetObservedWidth(FrameworkElement frameworkElement, double observedWidth) => frameworkElement.SetValue(ObservedWidthProperty, observedWidth);

    /// <summary>
    /// ObserveHeight property
    /// </summary>
    public static readonly DependencyProperty ObservedHeightProperty = DependencyProperty.RegisterAttached("ObservedHeight", typeof(double), typeof(SizeObserver));

    /// <summary>
    /// Get ObserveHeight
    /// </summary>
    /// <param name="frameworkElement"><see cref="FrameworkElement"/></param>
    /// <returns><see cref="double"/> value</returns>
    public static double GetObservedHeight(FrameworkElement frameworkElement) => (double) frameworkElement.GetValue(ObservedHeightProperty);

    /// <summary>
    /// Set ObserveHeight
    /// </summary>
    /// <param name="frameworkElement"><see cref="FrameworkElement"/></param>
    /// <param name="observedHeight">Set ObserveHeight</param>
    public static void SetObservedHeight(FrameworkElement frameworkElement, double observedHeight) => frameworkElement.SetValue(ObservedHeightProperty, observedHeight);

    private static void OnObserveChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
      var frameworkElement = (FrameworkElement) dependencyObject;

      if ( (bool) e.NewValue )
      {
        frameworkElement.SizeChanged += OnFrameworkElementSizeChanged;
        UpdateObservedSizesForFrameworkElement(frameworkElement);
      }
      else
      {
        frameworkElement.SizeChanged -= OnFrameworkElementSizeChanged;
      }
    }

    private static void OnFrameworkElementSizeChanged(object sender, SizeChangedEventArgs e) => UpdateObservedSizesForFrameworkElement((FrameworkElement) sender);

    private static void UpdateObservedSizesForFrameworkElement(FrameworkElement frameworkElement)
    {
      frameworkElement.SetCurrentValue(ObservedWidthProperty, frameworkElement.ActualWidth);
      frameworkElement.SetCurrentValue(ObservedHeightProperty, frameworkElement.ActualHeight);
    }
  }
}
