using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule.LogWindowUserControl.Behaviors
{
  /// <summary>
  /// GridSplitter mouse extension
  /// </summary>
  public class GridSplitterMouseExtension
  {
    /// <summary>
    /// This stores the GridSplitterMouseExtension for each GridSplitter so we can unregister it.
    /// </summary>
    private static readonly Dictionary<GridSplitter, GridSplitterMouseExtension> AttachedControls = new Dictionary<GridSplitter, GridSplitterMouseExtension>();

    private readonly GridSplitter _gridSplitter;

    /// <summary>
    /// Identifies the IsEnabled attached property.
    /// </summary>
    public static readonly DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached("Enabled", typeof(bool), typeof(GridSplitterMouseExtension),
      new UIPropertyMetadata(false, IsEnabledChanged));

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

    private static void IsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
      new FrameworkPropertyMetadata());

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
  }
}
