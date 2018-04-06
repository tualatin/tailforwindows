using System.Windows;


namespace Org.Vs.TailForWin.UI.UserControls.Behaviors
{
  /// <summary>
  /// Focus behavior
  /// </summary>
  public class FocusBehavior
  {
    /// <summary>
    /// Get IsFocused
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool GetIsFocused(DependencyObject obj) => (bool) obj.GetValue(IsFocusedProperty);

    /// <summary>
    /// Set IsFocused
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetIsFocused(DependencyObject obj, bool value) => obj.SetValue(IsFocusedProperty, value);

    /// <summary>
    /// IsFocused property
    /// </summary>
    public static readonly DependencyProperty IsFocusedProperty = DependencyProperty.RegisterAttached("IsFocused", typeof(bool), typeof(FocusBehavior), new UIPropertyMetadata(false, OnIsFocusedPropertyChanged));

    private static void OnIsFocusedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var uie = (UIElement) d;

      if ( (bool) e.NewValue )
        uie.Focus(); // Don't care about false values.
    }
  }
}
