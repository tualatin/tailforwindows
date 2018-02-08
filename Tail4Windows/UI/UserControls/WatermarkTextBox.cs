using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Org.Vs.TailForWin.UI.UserControls
{
  /// <summary>
  /// WatermakTextBox
  /// </summary>
  public class WatermarkTextBox : TextBox
  {
    static WatermarkTextBox() => DefaultStyleKeyProperty.OverrideMetadata(typeof(WatermarkTextBox), new FrameworkPropertyMetadata(typeof(WatermarkTextBox)));

    #region Public Properties

    /// <summary>
    /// Select all on got focus property
    /// </summary>
    public static readonly DependencyProperty SelectAllOnGotFocusProperty = DependencyProperty.Register("SelectAllOnGotFocus", typeof(bool), typeof(WatermarkTextBox), new PropertyMetadata(false));

    /// <summary>
    /// Select all on got focus
    /// </summary>
    public bool SelectAllOnGotFocus
    {
      get => (bool) GetValue(SelectAllOnGotFocusProperty);
      set => SetValue(SelectAllOnGotFocusProperty, value);
    }

    /// <summary>
    /// Set watermark property
    /// </summary>
    public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register("Watermark", typeof(object), typeof(WatermarkTextBox), new UIPropertyMetadata(null));

    /// <summary>
    /// Set watermark
    /// </summary>
    public object Watermark
    {
      get => GetValue(WatermarkProperty);
      set => SetValue(WatermarkProperty, value);
    }

    /// <summary>
    /// Watermark template property
    /// </summary>
    public static readonly DependencyProperty WatermarkTemplateProperty = DependencyProperty.Register("WatermarkTemplate", typeof(DataTemplate), typeof(WatermarkTextBox), new UIPropertyMetadata(null));

    /// <summary>
    /// Watermark template
    /// </summary>
    public DataTemplate WatermarkTemplate
    {
      get => (DataTemplate) GetValue(WatermarkTemplateProperty);
      set => SetValue(WatermarkTemplateProperty, value);
    }

    #endregion

    /// <summary>
    /// On got keyboard focus event
    /// </summary>
    /// <param name="e">KeyboadFocuesChangedEvent arguments</param>
    protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
      base.OnGotKeyboardFocus(e);

      if ( SelectAllOnGotFocus )
        SelectAll();
    }

    /// <summary>
    /// On preview mouse left button down event
    /// </summary>
    /// <param name="e">MouseButtonEvent arguments</param>
    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      if ( !IsKeyboardFocused && SelectAllOnGotFocus )
      {
        e.Handled = true;
        Focus();
      }

      base.OnPreviewMouseLeftButtonDown(e);
    }
  }
}
