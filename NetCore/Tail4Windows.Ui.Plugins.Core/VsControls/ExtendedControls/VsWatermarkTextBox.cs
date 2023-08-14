using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Org.Vs.TailForWin.Ui.PlugIns.VsControls.ExtendedControls
{
  /// <summary>
  /// Virtual Studios WatermarkTextBox
  /// </summary>
  public class VsWatermarkTextBox : TextBox
  {
    static VsWatermarkTextBox() => DefaultStyleKeyProperty.OverrideMetadata(typeof(VsWatermarkTextBox), new FrameworkPropertyMetadata(typeof(VsWatermarkTextBox)));

    #region Public Properties

    /// <summary>
    /// Select all on got focus property
    /// </summary>
    public static readonly DependencyProperty SelectAllOnGotFocusProperty = DependencyProperty.Register(nameof(SelectAllOnGotFocus), typeof(bool), typeof(VsWatermarkTextBox),
      new PropertyMetadata(false));

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
    public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register(nameof(Watermark), typeof(object), typeof(VsWatermarkTextBox), new UIPropertyMetadata(null));

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
    public static readonly DependencyProperty WatermarkTemplateProperty = DependencyProperty.Register(nameof(WatermarkTemplate), typeof(DataTemplate), typeof(VsWatermarkTextBox),
      new UIPropertyMetadata(null));

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
