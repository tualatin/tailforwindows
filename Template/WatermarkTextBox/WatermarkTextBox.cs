using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Org.Vs.TailForWin.Template.WatermarkTextBox
{
  /// <summary>
  /// WatermakTextBox
  /// </summary>
  public class WatermarkTextBox : TextBox
  {
    static WatermarkTextBox()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(WatermarkTextBox), new FrameworkPropertyMetadata(typeof(WatermarkTextBox)));
    }

    #region Public Properties

    public static readonly DependencyProperty SelectAllOnGotFocusProperty = DependencyProperty.Register("SelectAllOnGotFocus", typeof(bool), typeof(WatermarkTextBox), new PropertyMetadata(false));

    public bool SelectAllOnGotFocus
    {
      get
      {
        return ((bool) GetValue(SelectAllOnGotFocusProperty));
      }
      set
      {
        SetValue(SelectAllOnGotFocusProperty, value);
      }
    }

    public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register("Watermark", typeof(object), typeof(WatermarkTextBox), new UIPropertyMetadata(null));

    public object Watermark
    {
      get
      {
        return (GetValue(WatermarkProperty));
      }
      set
      {
        SetValue(WatermarkProperty, value);
      }
    }

    public static readonly DependencyProperty WatermarkTemplateProperty = DependencyProperty.Register("WatermarkTemplate", typeof(DataTemplate), typeof(WatermarkTextBox), new UIPropertyMetadata(null));

    public DataTemplate WatermarkTemplate
    {
      get
      {
        return ((DataTemplate) GetValue(WatermarkTemplateProperty));
      }
      set
      {
        SetValue(WatermarkTemplateProperty, value);
      }
    }

    #endregion

    protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
      base.OnGotKeyboardFocus(e);

      if(SelectAllOnGotFocus)
        SelectAll();
    }

    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      if(!IsKeyboardFocused && SelectAllOnGotFocus)
      {
        e.Handled = true;
        Focus();
      }

      base.OnPreviewMouseLeftButtonDown(e);
    }
  }
}
