using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Org.Vs.TailForWin.Data;
using Org.Vs.TailForWin.Utils;


namespace Org.Vs.TailForWin.Template
{
  /// <summary>
  /// Interaction logic for GoToLine.xaml
  /// </summary>
  public partial class GoToLine
  {
    /// <summary>
    /// Go to linenumber event handler
    /// </summary>
    public event EventHandler GoToLineNumber;

    private readonly int minLines;
    private readonly int maxLines;
    private int userInput;


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="minLines">Min lines</param>
    /// <param name="maxLines">Max lines</param>
    public GoToLine(int minLines, int maxLines)
    {
      InitializeComponent();

      PreviewKeyDown += HandleEsc;
      this.minLines = minLines;
      this.maxLines = maxLines;
      userInput = -1;

      textBoxLineNumber.Focus();
      var findResource = Application.Current.FindResource("GoToLineNumber");

      if(findResource != null)
        labelLineNumber.Content = string.Format(findResource.ToString(), this.minLines, this.maxLines);
    }

    /// <summary>
    /// On source initialized
    /// </summary>
    /// <param name="e">EventArgs</param>
    protected override void OnSourceInitialized(EventArgs e)
    {
      IconHelper.RemoveIcon(this);
    }

    private void btnOK_Click(object sender, RoutedEventArgs e)
    {
      if (userInput == -1)
        return;

      var lineNumber = new GoToLineData { LineNumber = userInput };

      GoToLineNumber?.Invoke(this, lineNumber);
      Close();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void HandleEsc(object sender, KeyEventArgs e)
    {
      if(e.Key == Key.Escape)
        btnCancel_Click(sender, e);
      if(e.Key == Key.Enter)
        btnOK_Click(sender, e);
    }

    private void textBoxLineNumber_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
      var regex = new Regex(@"\d");

      if(regex.IsMatch(textBoxLineNumber.Text))
      {
        int usrInput;

        if(int.TryParse(textBoxLineNumber.Text, out usrInput))
        {
          if(usrInput < minLines)
            btnOk.IsEnabled = false;
          else if(usrInput > maxLines)
            btnOk.IsEnabled = false;
          else
          {
            btnOk.IsEnabled = true;
            userInput = usrInput;
          }
        }
        else
          btnOk.IsEnabled = false;

        return;
      }

      btnOk.IsEnabled = false;
    }
  }
}
