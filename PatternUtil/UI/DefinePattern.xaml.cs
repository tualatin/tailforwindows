using System.Windows;
using System.Windows.Input;
using Org.Vs.TailForWin.Data;
using Org.Vs.TailForWin.PatternUtil.Events;


namespace Org.Vs.TailForWin.PatternUtil.UI
{
  /// <summary>
  /// Interaction logic for DefinePattern.xaml
  /// </summary>
  public partial class DefinePattern : Window
  {
    /// <summary>
    /// Fires, when pattern changed
    /// </summary>
    public event PatternChangedEventHandler PatternChanged;

    private Pattern pattern;


    /// <summary>
    /// Standard constructor
    /// </summary>
    public DefinePattern()
    {
      InitializeComponent();

      PreviewKeyDown += HandleEsc;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      pattern = new Pattern();
      FocusManager.SetFocusedElement(this, TextBoxPattern);
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
      if(string.IsNullOrEmpty(TextBoxPattern.Text))
      {
        pattern = null;
      }
      else
      {
        pattern.IsRegex = CheckBoxRegex.IsChecked.Value;
        pattern.PatternString = TextBoxPattern.Text;
      }

      PatternChanged?.Invoke(this, pattern);
      Close();
    }

    #region HelperFunctsion

    private void HandleEsc(object sender, KeyEventArgs e)
    {
      if(e.Key == Key.Escape)
        Close();
    }

    #endregion
  }
}
