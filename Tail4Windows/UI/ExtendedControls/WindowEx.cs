using System.Windows;


namespace Org.Vs.TailForWin.UI.ExtendedControls
{
  /// <summary>
  /// WindowEx class
  /// </summary>
  public class WindowEx : Window
  {
    /// <summary>
    /// Dialog can close
    /// </summary>
    public static readonly DependencyProperty CanCloseProperty = DependencyProperty.Register("CanClose", typeof(bool), typeof(WindowEx), new PropertyMetadata(false, CanCloseChanged));

    private static void CanCloseChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      if ( e.Property != CanCloseProperty || !(e.NewValue is bool canClose) )
        return;

      if ( canClose && sender is WindowEx wnd )
        wnd.Close();
    }

    /// <summary>
    /// Can close
    /// </summary>
    public bool CanClose
    {
      get => (bool) GetValue(CanCloseProperty);
      set => SetValue(CanCloseProperty, value);
    }
  }
}
