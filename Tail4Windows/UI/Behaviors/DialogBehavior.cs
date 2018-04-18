using System.Windows;


namespace Org.Vs.TailForWin.UI.Behaviors
{
  /// <summary>
  /// Dialog behavior
  /// </summary>
  public class DialogBehavior
  {
    /// <summary>
    /// Dialog visible property
    /// </summary>
    public static readonly DependencyProperty DialogVisibleProperty = DependencyProperty.RegisterAttached("DialogVisible", typeof(bool), typeof(DialogBehavior),
      new PropertyMetadata(false, OnDialogVisibleChanged));

    /// <summary>
    /// Get current dialog visibility
    /// </summary>
    /// <param name="source">Source</param>
    public static bool GetDialogVisible(DependencyObject source) => (bool) source.GetValue(DialogVisibleProperty);

    /// <summary>
    /// Set dialog visibility
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="value">Visible value</param>
    public static void SetDialogVisible(DependencyObject source, bool value) => source.SetValue(DialogVisibleProperty, value);

    private static void OnDialogVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if ( !(d is Window) )
        return;

      if ( e.NewValue == null )
        return;

      var resource = Application.Current.TryFindResource(e.NewValue.GetType());

      if ( resource == null )
        return;

      if ( !(resource is Window wnd) )
        return;

      wnd.DataContext = e.NewValue;
      wnd.ShowDialog();
    }
  }
}
