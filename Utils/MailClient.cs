using System.Net.Mail;
using TailForWin.Controller;
using System.Net;
using TailForWin.Data;
using System.Windows;
using System;
using System.ComponentModel;


namespace TailForWin.Utils
{
  public class MailClient: IDisposable
  {
    private SmtpClient mailClient;
    private MailMessage mailMessage;


    /// <summary>
    /// Free up unused object
    /// </summary>
    public void Dispose ()
    {
      if (mailClient != null)
      {
        mailClient.Dispose ( );
        mailClient = null;
      }

      if (mailMessage != null)
      {
        mailMessage.Dispose ( );
        mailMessage = null;
      }
    }

    /// <summary>
    /// Init E-Mail client engine
    /// </summary>
    public void InitClient ()
    {
      try
      {
        mailClient = new SmtpClient (SettingsHelper.TailSettings.AlertSettings.SmtpSettings.SmtpServerName, SettingsHelper.TailSettings.AlertSettings.SmtpSettings.SmtpPort)
        {
          UseDefaultCredentials = false,
          Timeout = 120000
        };
        string decryptPassword = StringEncryption.Decrypt (SettingsHelper.TailSettings.AlertSettings.SmtpSettings.Password, LogFile.ENCRYPT_PASSPHRASE);
        NetworkCredential authInfo = new NetworkCredential (SettingsHelper.TailSettings.AlertSettings.SmtpSettings.LoginName, decryptPassword);
        mailClient.Credentials = authInfo;
        mailClient.SendCompleted += SendCompleted;

        MailAddress from = new MailAddress (SettingsHelper.TailSettings.AlertSettings.SmtpSettings.FromAddress);
        MailAddress to = new MailAddress (SettingsHelper.TailSettings.AlertSettings.EMailAddress);
        
        mailMessage = new MailMessage (from, to)
        {
          Subject = SettingsHelper.TailSettings.AlertSettings.SmtpSettings.Subject,
          SubjectEncoding = System.Text.Encoding.UTF8,
          IsBodyHtml = false,
        };
      }
      catch (Exception ex)
      {
        MessageBox.Show (Application.Current.FindResource ("SmtpSettingsNotValid").ToString ( ), LogFile.MSGBOX_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
        ErrorLog.WriteLog (ErrorFlags.Error, "MailClient", string.Format ("InitClient exception: {0}", ex));
      }
    }

    /// <summary>
    /// Send a E-Mail
    /// </summary>
    public void SendMail ( )
    {
      try
      {
        string userState = "test message";
        mailClient.SendAsync (mailMessage, userState);
      }
      catch (Exception ex)
      {
        MessageBox.Show (Application.Current.FindResource ("MailCannotSend").ToString ( ), LogFile.MSGBOX_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
        ErrorLog.WriteLog (ErrorFlags.Error, "MailClient", string.Format ("SendMail exception: {0}", ex));
      }
    }

    private void SendCompleted (object sender, AsyncCompletedEventArgs e)
    {
      string token = (string) e.UserState;

      if (e.Cancelled)
      {
        MessageBox.Show (Application.Current.FindResource ("MailCannotSend").ToString ( ), LogFile.MSGBOX_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }
      if (e.Error != null)
        MessageBox.Show (e.Error.ToString ( ), LogFile.MSGBOX_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
      else
        MessageBox.Show ("Complete!", LogFile.MSGBOX_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
    }
  }
}
