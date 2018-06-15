using System.Windows;
using System.Windows.Controls;


namespace Org.Vs.TailForWin.UI.Behaviors
{
  /// <summary>
  /// <see cref="TextBox"/> selection behavior
  /// </summary>
  public class TextBoxSelectionBehavior
  {
    /// <summary>
    /// Get selected text
    /// </summary>
    /// <param name="obj"><see cref="DependencyObject"/></param>
    /// <returns>Selected text</returns>
    public static string GetSelectedText(DependencyObject obj) => (string) obj.GetValue(SelectedTextProperty);

    /// <summary>
    /// Set selected text
    /// </summary>
    /// <param name="obj"><see cref="DependencyObject"/></param>
    /// <param name="value">Text</param>
    public static void SetSelectedText(DependencyObject obj, string value) => obj.SetValue(SelectedTextProperty, value);

    /// <summary>
    /// Using a DependencyProperty as the backing store for SelectedText.  This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty SelectedTextProperty = DependencyProperty.RegisterAttached("SelectedText", typeof(string), typeof(TextBoxSelectionBehavior),
      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedTextChanged));

    private static void SelectedTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      if ( !(obj is TextBox tb) )
        return;

      if ( e.OldValue == null && e.NewValue != null )
        tb.SelectionChanged += TextBoxSelectionChanged;
      else if ( e.OldValue != null && e.NewValue == null )
        tb.SelectionChanged -= TextBoxSelectionChanged;

      if ( e.NewValue is string newValue && newValue != tb.SelectedText )
        tb.SelectedText = newValue;
    }

    private static void TextBoxSelectionChanged(object sender, RoutedEventArgs e)
    {
      if ( sender is TextBox tb )
        SetSelectedText(tb, tb.SelectedText);
    }
  }
}
