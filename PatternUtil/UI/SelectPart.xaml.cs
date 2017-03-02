using System.Windows;
using System.Windows.Input;
using Org.Vs.TailForWin.Data;
using Org.Vs.TailForWin.PatternUtil.Events;


namespace Org.Vs.TailForWin.PatternUtil.UI
{
  /// <summary>
  /// Interaction logic for SelectPart.xaml
  /// </summary>
  public partial class SelectPart : Window
  {
    /// <summary>
    /// Fires, when part has changed
    /// </summary>
    public event PartChangedEventHandler PartChanged;

    private bool leftMouseDown;
    private Part part;


    /// <summary>
    /// Text to be parted
    /// </summary>
    public string Text
    {
      get;
      set;
    }


    /// <summary>
    /// Standard constructor
    /// </summary>
    public SelectPart()
    {
      InitializeComponent();

      PreviewKeyDown += HandleEsc;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      part = new Part();
      TextBoxSelectPart.Text = Text;

      TextBoxSelectPart.SelectAll();
      SetLabelContent(TextBoxSelectPart.SelectionStart, TextBoxSelectPart.SelectionLength);
      FocusManager.SetFocusedElement(this, TextBoxSelectPart);
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
      if(TextBoxSelectPart.SelectionLength == 0)
      {
        part = null;
      }
      else
      {
        part.Begin = TextBoxSelectPart.SelectionStart;
        part.End = TextBoxSelectPart.SelectionLength;
      }

      if(PartChanged != null)
        PartChanged(this, part);

      Close();
    }

    private void TextBoxSelectPart_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      SetLabelContent(TextBoxSelectPart.SelectionStart, TextBoxSelectPart.SelectionLength);
    }

    private void TextBoxSelectPart_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      leftMouseDown = true;
    }

    private void TextBoxSelectPart_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      leftMouseDown = false;
    }

    private void TextBoxSelectPart_PreviewMouseMove(object sender, MouseEventArgs e)
    {
      if(leftMouseDown)
        SetLabelContent(TextBoxSelectPart.SelectionStart, TextBoxSelectPart.SelectionLength);
    }

    #region HelperFunctions

    private void HandleEsc(object sender, KeyEventArgs e)
    {
      if(e.Key == Key.Escape)
        Close();
    }

    private void SetLabelContent(int start, int length)
    {
      switch(length)
      {
      case 0:

        LabelPartCount.Content = null;
        break;

      case 1:

        LabelPartCount.Content = length;
        break;

      default:

        LabelPartCount.Content = string.Format("{0}-{1}", start, length);
        break;
      }
    }

    #endregion
  }
}
