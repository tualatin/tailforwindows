using System.Windows;
using System.Windows.Input;
using System;
using TailForWin.Controller;
using TailForWin.Data;


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
      if (!string.IsNullOrEmpty (watermarkTextBoxSmtpServer.Text))
      {
        if (string.IsNullOrEmpty (watermarkTextBoxPort.Text))
        {
          MessageBox.Show (Application.Current.FindResource ("SmtpPortNotValid").ToString ( ), LogFile.MSGBOX_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
          watermarkTextBoxPort.Focus ( );
          return;
        }
        else
        {
          if (SettingsHelper.ParseEMailAddress (watermarkTextBoxFrom.Text))
          {
            SaveSettings ( );
            OnExit ( );
          }
          else
          {
            MessageBox.Show (Application.Current.FindResource ("EMailAddressNotValid").ToString ( ), LogFile.MSGBOX_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
            watermarkTextBoxFrom.Focus ( );
            return;
          }
        }
      }
      else
      {
        SaveSettings ( );
        OnExit ( );
      }
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

    private void watermarkTextBox_GotFocus (object sender, RoutedEventArgs e)
    {
      TailForWin.Template.WatermarkTextBox.WatermarkTextBox tb = (TailForWin.Template.WatermarkTextBox.WatermarkTextBox) e.OriginalSource;
      SelectAllText (tb);
    }

    private void Window_Loaded (object sender, RoutedEventArgs e)
    {
      DataContext = SettingsHelper.TailSettings.AlertSettings.SmtpSettings;

      if (!string.IsNullOrEmpty (SettingsHelper.TailSettings.AlertSettings.SmtpSettings.Password))
        textBoxPassword.Password = TailForWin.Utils.StringEncryption.Decrypt (SettingsHelper.TailSettings.AlertSettings.SmtpSettings.Password, LogFile.ENCRYPT_PASSPHRASE);
    }

    #endregion

    #region HelperFunctions

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

    private void SaveSettings ()
    {
      if (watermarkTextBoxSmtpServer.Text.Length > 0)
        SettingsHelper.TailSettings.AlertSettings.SmtpSettings.SmtpServerName = watermarkTextBoxSmtpServer.Text;

      int port;
      
      if (!int.TryParse (watermarkTextBoxPort.Text, out port))
        port = -1;

      SettingsHelper.TailSettings.AlertSettings.SmtpSettings.SmtpPort = port;

      if (watermarkTextBoxFrom.Text.Length > 0)
        SettingsHelper.TailSettings.AlertSettings.SmtpSettings.FromAddress = watermarkTextBoxFrom.Text;

      if (watermarkTextBoxUserName.Text.Length > 0)
        SettingsHelper.TailSettings.AlertSettings.SmtpSettings.LoginName = watermarkTextBoxUserName.Text;

      if (textBoxPassword.Password.Length > 0)
        SettingsHelper.TailSettings.AlertSettings.SmtpSettings.Password = TailForWin.Utils.StringEncryption.Encrypt (textBoxPassword.Password, LogFile.ENCRYPT_PASSPHRASE);

      if (watermarkTextBoxSubject.Text.Length > 0)
        SettingsHelper.TailSettings.AlertSettings.SmtpSettings.Subject = watermarkTextBoxSubject.Text;
    }

    #endregion
  }
}
