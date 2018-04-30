using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using log4net;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule.LogWindowUserControl.Behaviors
{
  /// <summary>
  /// GridSplitter mouse extension
  /// </summary>
  public class GridSplitterMouseExtension
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(GridSplitterMouseExtension));

    /// <summary>
    /// This stores the GridSplitterMouseExtension for each GridSplitter so we can unregister it.
    /// </summary>
    private static readonly Dictionary<GridSplitter, GridSplitterMouseExtension> AttachedControls = new Dictionary<GridSplitter, GridSplitterMouseExtension>();

    private readonly GridSplitter _gridSplitter;

    /// <summary>
    /// Identifies the IsEnabled attached property.
    /// </summary>
    public static readonly DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached("Enabled", typeof(bool), typeof(GridSplitterMouseExtension),
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
      if ( !(d is GridSplitter gridSplitter) )
        return;

      if ( (bool) e.NewValue )
      {
        AttachedControls.Add(gridSplitter, new GridSplitterMouseExtension(gridSplitter));
      }
      else
      {
        if ( !AttachedControls.TryGetValue(gridSplitter, out var mouseExtension) )
          return;

        AttachedControls.Remove(gridSplitter);
        mouseExtension.Unregister();
      }
    }

    /// <summary>
    /// IsParentControl property
    /// </summary>
    public static readonly DependencyProperty IsParentControlProperty = DependencyProperty.RegisterAttached("IsParentControl", typeof(bool), typeof(GridSplitterMouseExtension),
      new FrameworkPropertyMetadata(OnIsParentChanged));

    /// <summary>
    /// GetIsParentControl
    /// </summary>
    /// <param name="frameworkElement"><see cref="FrameworkElement"/></param>
    /// <returns><c>True</c> otherwise <c>False</c></returns>
    public static bool GetIsParentControl(FrameworkElement frameworkElement) => (bool) frameworkElement.GetValue(IsParentControlProperty);

    /// <summary>
    /// SetIsParentControl
    /// </summary>
    /// <param name="frameworkElement"><see cref="FrameworkElement"/></param>
    /// <param name="isParentControl">Set observe</param>
    public static void SetIsParentControl(FrameworkElement frameworkElement, bool isParentControl) => frameworkElement.SetValue(IsParentControlProperty, isParentControl);

    private static void OnIsParentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var frameworkElement = (FrameworkElement) d;

      if ( (bool) e.NewValue )
        frameworkElement.MouseMove += OnMouseMove;
      else
        frameworkElement.MouseMove -= OnMouseMove;
    }

    private static void OnMouseMove(object sender, MouseEventArgs e)
    {
      if ( AttachedControls.Count == 0 )
        return;

      var frameworkElement = (FrameworkElement) sender;
      var gridSplitter = FindChild<GridSplitter>(frameworkElement);

      if ( gridSplitter == null )
        return;

      var mouse = e.GetPosition(frameworkElement);
      var gridSplitterPoint = gridSplitter.TransformToAncestor(frameworkElement).Transform(new Point(0, 0));

      if ( mouse.Y >= 1 && mouse.Y <= 15 && gridSplitter.Visibility == Visibility.Collapsed && gridSplitterPoint.Y <= 0 )
        gridSplitter.Visibility = Visibility.Visible;
      else if ( gridSplitterPoint.Y <= 0 && gridSplitter.Visibility == Visibility.Visible && mouse.Y > 20 )
        gridSplitter.Visibility = Visibility.Collapsed;
      else if ( gridSplitterPoint.Y <= 0 && gridSplitter.Visibility == Visibility.Visible && mouse.Y <= 0 )
        gridSplitter.Visibility = Visibility.Collapsed;
    }

    private GridSplitterMouseExtension(GridSplitter gridSplitter)
    {
      _gridSplitter = gridSplitter;
      Register();
    }

    private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
      var mouse = e.GetPosition(_gridSplitter);

      if ( mouse.Y < 0 )
        _gridSplitter.Visibility = Visibility.Collapsed;
    }

    private void Register() => _gridSplitter.PreviewMouseLeftButtonUp += OnPreviewMouseUp;

    private void Unregister() => _gridSplitter.PreviewMouseLeftButtonUp -= OnPreviewMouseUp;

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
  }
}
