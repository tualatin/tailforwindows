using System.Windows;
using TailForWin.Controller;
using System.Windows.Input;
using TailForWin.Utils;
using TailForWin.Data;
using System;


namespace TailForWin.Template.TabOptions
{
  /// <summary>
  /// Interaktionslogik für ProxyServer.xaml
  /// </summary>
  public partial class ProxyServer
  {
    public ProxyServer()
    {
      InitializeComponent();

      PreviewKeyDown += HandleEsc;

      InitRadios();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      DataContext = SettingsHelper.TailSettings.ProxySettings;
    }

    private void btnOK_Click(object sender, RoutedEventArgs e)
    {
      SaveSettings();
      OnExit();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      OnExit();
    }

    private void HandleEsc(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        OnExit();
    }

    private void OnExit()
    {
      Close();
    }

    #region HelperFunctions

    private void SaveSettings()
    {
      if (textBoxPassword.Password.Length > 0)
        SettingsHelper.TailSettings.ProxySettings.Password = StringEncryption.Encrypt(textBoxPassword.Password, LogFile.ENCRYPT_PASSPHRASE);
      if (watermarkTextBoxUserName.Text.Length > 0)
        SettingsHelper.TailSettings.ProxySettings.UserName = watermarkTextBoxUserName.Text;
      if (watermarkTextBoxUrl.Text.Length > 0)
        SettingsHelper.TailSettings.ProxySettings.ProxyUrl = watermarkTextBoxUrl.Text;

      int port;

      if (!int.TryParse(watermarkTextBoxPort.Text, out port))
        port = -1;

      SettingsHelper.TailSettings.ProxySettings.ProxyPort = port;

      if (radioButtonManualProxy.IsChecked == true)
        SettingsHelper.TailSettings.ProxySettings.UseProxy = true;
      else
        SettingsHelper.TailSettings.ProxySettings.UseProxy = false;

      if (radioButtonSystemSettings.IsChecked == true)
        SettingsHelper.TailSettings.ProxySettings.UseSystemSettings = true;
      else
        SettingsHelper.TailSettings.ProxySettings.UseSystemSettings = false;

      if (radioButtonNoProxy.IsChecked == true)
      {
        SettingsHelper.TailSettings.ProxySettings.UserName = string.Empty;
        SettingsHelper.TailSettings.ProxySettings.Password = string.Empty;
        SettingsHelper.TailSettings.ProxySettings.ProxyUrl = string.Empty;
        SettingsHelper.TailSettings.ProxySettings.ProxyPort = -1;
      }

      SettingsHelper.SaveSettings();
    }

    private void InitRadios()
    {
      if (!SettingsHelper.TailSettings.ProxySettings.UseSystemSettings && !SettingsHelper.TailSettings.ProxySettings.UseProxy)
        radioButtonNoProxy.IsChecked = true;
      else
        radioButtonNoProxy.IsChecked = false;

      if (!string.IsNullOrEmpty(SettingsHelper.TailSettings.ProxySettings.Password))
        textBoxPassword.Password = StringEncryption.Decrypt(SettingsHelper.TailSettings.ProxySettings.Password, LogFile.ENCRYPT_PASSPHRASE);
    }

    private void SelectAllText(WatermarkTextBox.WatermarkTextBox textBox)
    {
      textBox.Dispatcher.BeginInvoke(new Action(delegate
      {
        textBox.SelectAll();
      }), System.Windows.Threading.DispatcherPriority.Input);
    }

    #endregion

    private void watermarkTextBox_GotFocus(object sender, RoutedEventArgs e)
    {
      WatermarkTextBox.WatermarkTextBox tb = (WatermarkTextBox.WatermarkTextBox)e.OriginalSource;
      SelectAllText(tb);
    }
  }
}
