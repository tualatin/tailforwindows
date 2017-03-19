﻿using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Org.Vs.TailForWin.Controller;
using Org.Vs.TailForWin.Data;


namespace Org.Vs.TailForWin.Template.TabOptions
{
  /// <summary>
  /// Interaction logic for SmtpSettings.xaml
  /// </summary>
  public partial class SmtpSettings
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public SmtpSettings()
    {
      InitializeComponent();

      PreviewKeyDown += HandleEsc;
    }

    #region ClickEvents

    private void btnOK_Click(object sender, RoutedEventArgs e)
    {
      if(!string.IsNullOrEmpty(watermarkTextBoxSmtpServer.Text))
      {
        if(string.IsNullOrEmpty(watermarkTextBoxPort.Text))
        {
          MessageBox.Show(Application.Current.FindResource("SmtpPortNotValid") as string, LogFile.MSGBOX_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
          watermarkTextBoxPort.Focus();
        }
        else
        {
          if(SettingsHelper.ParseEMailAddress(watermarkTextBoxFrom.Text))
          {
            SaveSettings();
            OnExit();
          }
          else
          {
            MessageBox.Show(Application.Current.FindResource("EMailAddressNotValid") as string, LogFile.MSGBOX_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
            watermarkTextBoxFrom.Focus();
          }
        }
      }
      else
      {
        SaveSettings();
        OnExit();
      }
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      OnExit();
    }

    private void checkBoxSameLogin_Click(object sender, RoutedEventArgs e)
    {
      if(checkBoxSameLogin.IsChecked == true)
        watermarkTextBoxFrom.Text = watermarkTextBoxUserName.Text;
      else
        watermarkTextBoxFrom.Text = string.Empty;
    }

    #endregion

    #region Events

    private void HandleEsc(object sender, KeyEventArgs e)
    {
      if(e.Key == Key.Escape)
        OnExit();
    }

    private void watermarkTextBox_GotFocus(object sender, RoutedEventArgs e)
    {
      WatermarkTextBox.WatermarkTextBox tb = (WatermarkTextBox.WatermarkTextBox) e.OriginalSource;
      SelectAllText(tb);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      DataContext = SettingsHelper.TailSettings.AlertSettings.SmtpSettings;

      if(!string.IsNullOrEmpty(SettingsHelper.TailSettings.AlertSettings.SmtpSettings.Password))
        textBoxPassword.Password = Utils.StringEncryption.Decrypt(SettingsHelper.TailSettings.AlertSettings.SmtpSettings.Password, LogFile.ENCRYPT_PASSPHRASE);

      if(SettingsHelper.TailSettings.AlertSettings.SmtpSettings.SSL)
        comboBoxSecurity.SelectedIndex = 1;
      else if(SettingsHelper.TailSettings.AlertSettings.SmtpSettings.TLS)
        comboBoxSecurity.SelectedIndex = 2;
      else
        comboBoxSecurity.SelectedIndex = 0;
    }

    private void watermarkTextBoxUserName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
      if(checkBoxSameLogin.IsChecked == true)
        watermarkTextBoxFrom.Text = watermarkTextBoxUserName.Text;
    }

    #endregion

    #region HelperFunctions

    private void OnExit()
    {
      Close();
    }

    private static void SelectAllText(TextBoxBase textBox)
    {
      textBox.Dispatcher.BeginInvoke(new Action(textBox.SelectAll), System.Windows.Threading.DispatcherPriority.Input);
    }

    private void SaveSettings()
    {
      if(watermarkTextBoxSmtpServer.Text.Length > 0)
        SettingsHelper.TailSettings.AlertSettings.SmtpSettings.SmtpServerName = watermarkTextBoxSmtpServer.Text;

      if(!int.TryParse(watermarkTextBoxPort.Text, out int port))
        port = -1;

      SettingsHelper.TailSettings.AlertSettings.SmtpSettings.SmtpPort = port;

      if(watermarkTextBoxFrom.Text.Length > 0)
        SettingsHelper.TailSettings.AlertSettings.SmtpSettings.FromAddress = watermarkTextBoxFrom.Text;

      if(watermarkTextBoxUserName.Text.Length > 0)
        SettingsHelper.TailSettings.AlertSettings.SmtpSettings.LoginName = watermarkTextBoxUserName.Text;

      if(textBoxPassword.Password.Length > 0)
        SettingsHelper.TailSettings.AlertSettings.SmtpSettings.Password = Utils.StringEncryption.Encrypt(textBoxPassword.Password, LogFile.ENCRYPT_PASSPHRASE);

      if(watermarkTextBoxSubject.Text.Length > 0)
        SettingsHelper.TailSettings.AlertSettings.SmtpSettings.Subject = watermarkTextBoxSubject.Text;

      switch(comboBoxSecurity.SelectedIndex)
      {
      case 0:

        SettingsHelper.TailSettings.AlertSettings.SmtpSettings.SSL = false;
        SettingsHelper.TailSettings.AlertSettings.SmtpSettings.TLS = false;
        break;

      case 1:

        SettingsHelper.TailSettings.AlertSettings.SmtpSettings.SSL = true;
        SettingsHelper.TailSettings.AlertSettings.SmtpSettings.TLS = false;
        break;

      case 2:

        SettingsHelper.TailSettings.AlertSettings.SmtpSettings.SSL = false;
        SettingsHelper.TailSettings.AlertSettings.SmtpSettings.TLS = true;
        break;
      }

      SettingsHelper.SaveSettings();
    }

    #endregion
  }
}