using System.Windows;
using System.Windows.Input;
using System;


namespace TailForWin.Template
{
  /// <summary>
  /// Interaction logic for SmtpSettings.xaml
  /// </summary>
  public partial class SmtpSettings: Window
  {
    public SmtpSettings ()
    {
      InitializeComponent ( );

      PreviewKeyDown += HandleEsc;
    }

    #region ClickEvents

    private void btnOK_Click (object sender, RoutedEventArgs e)
    {
      OnExit ( );
    }

    private void btnCancel_Click (object sender, RoutedEventArgs e)
    {
      OnExit ( );
    }

    #endregion

    #region Events

    private void HandleEsc (object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        OnExit ( );
    }

    private void watermarkTextBoxUserName_GotFocus (object sender, RoutedEventArgs e)
    {
      TailForWin.Template.WatermarkTextBox.WatermarkTextBox tb = (TailForWin.Template.WatermarkTextBox.WatermarkTextBox) e.OriginalSource;
      SelectAllText (tb);
    }

    private void textBoxPassword_GotFocus (object sender, RoutedEventArgs e)
    {
      TailForWin.Template.WatermarkTextBox.WatermarkTextBox tb = (TailForWin.Template.WatermarkTextBox.WatermarkTextBox) e.OriginalSource;
      SelectAllText (tb);
    }

    private void watermarkTextBoxSubject_GotFocus (object sender, RoutedEventArgs e)
    {
      TailForWin.Template.WatermarkTextBox.WatermarkTextBox tb = (TailForWin.Template.WatermarkTextBox.WatermarkTextBox) e.OriginalSource;
      SelectAllText (tb);
    }

    #endregion

    private void OnExit ()
    {
      Close ( );
    }

    private void SelectAllText (TailForWin.Template.WatermarkTextBox.WatermarkTextBox textBox)
    {
      textBox.Dispatcher.BeginInvoke (new Action (delegate
      {
        textBox.SelectAll ( );
      }), System.Windows.Threading.DispatcherPriority.Input);
    }
  }
}
