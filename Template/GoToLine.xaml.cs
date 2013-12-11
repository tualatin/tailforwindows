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
  public partial class GoToLine: Window
  {
    /// <summary>
    /// Go to linenumber event handler
    /// </summary>
    public event EventHandler GoToLineNumber;

    private int minLines;
    private int maxLines;
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
      if (userInput != -1) 
      {
        GoToLineData lineNumber = new GoToLineData ( ) { LineNumber = userInput };

        if (GoToLineNumber != null)
          GoToLineNumber (this, lineNumber);

        Close ( );
      }
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
        int userInput = -1;

        if (int.TryParse (textBoxLineNumber.Text, out userInput))
        {
          if (userInput < minLines)
            btnOk.IsEnabled = false;
          else if (userInput > maxLines)
            btnOk.IsEnabled = false;
          else
          {
            btnOk.IsEnabled = true;
            this.userInput = userInput;
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
