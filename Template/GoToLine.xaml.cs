using System;
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;
using TailForWin.Data;


namespace TailForWin.Template
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


    public GoToLine (int minLines, int maxLines)
    {
      InitializeComponent ( );

      PreviewKeyDown += HandleEsc;
      this.minLines = minLines;
      this.maxLines = maxLines;
      userInput = -1;

      textBoxLineNumber.Focus ( );
      labelLineNumber.Content = string.Format (Application.Current.FindResource ("GoToLineNumber").ToString ( ), this.minLines, this.maxLines);
    }

    private void btnOK_Click (object sender, RoutedEventArgs e)
    {
      if (userInput == -1)
        return;

      GoToLineData lineNumber = new GoToLineData { LineNumber = userInput };

      if (GoToLineNumber != null)
        GoToLineNumber (this, lineNumber);

      Close ( );
    }

    private void btnCancel_Click (object sender, RoutedEventArgs e)
    {
      Close ( );
    }

    private void HandleEsc (object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        btnCancel_Click (sender, e);
      if (e.Key == Key.Enter)
        btnOK_Click (sender, e);
    }

    private void textBoxLineNumber_TextChanged (object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
      Regex regex = new Regex (@"\d");

      if (regex.IsMatch (textBoxLineNumber.Text))
      {
        int usrInput;

        if (int.TryParse (textBoxLineNumber.Text, out usrInput))
        {
          if (usrInput < minLines)
            btnOk.IsEnabled = false;
          else if (usrInput > maxLines)
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
