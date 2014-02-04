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
    private System.Timers.Timer emailTimer;


    /// <summary>
    /// Free up unused objects
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

      if (emailTimer != null)
      {
        emailTimer.Dispose ( );
        emailTimer = null;
      }
    }

    /// <summary>
    /// Init E-Mail client engine
    /// </summary>
    public void InitClient ()
    {
      InitSucces = false;

      if (string.IsNullOrEmpty (SettingsHelper.TailSettings.AlertSettings.SmtpSettings.SmtpServerName))
        return;
      if (SettingsHelper.TailSettings.AlertSettings.SmtpSettings.SmtpPort <= 0)
        return;

      try
      {
        mailClient = new SmtpClient (SettingsHelper.TailSettings.AlertSettings.SmtpSettings.SmtpServerName, SettingsHelper.TailSettings.AlertSettings.SmtpSettings.SmtpPort)
        {
          UseDefaultCredentials = false,
          Timeout = 30000,
          DeliveryMethod = SmtpDeliveryMethod.Network
        };
        string decryptPassword = StringEncryption.Decrypt (SettingsHelper.TailSettings.AlertSettings.SmtpSettings.Password, LogFile.ENCRYPT_PASSPHRASE);
        NetworkCredential authInfo = new NetworkCredential (SettingsHelper.TailSettings.AlertSettings.SmtpSettings.LoginName, decryptPassword);
        mailClient.Credentials = authInfo;
        mailClient.SendCompleted += SendCompleted;

        if (SettingsHelper.TailSettings.AlertSettings.SmtpSettings.SSL)
          mailClient.EnableSsl = true;
        if (SettingsHelper.TailSettings.AlertSettings.SmtpSettings.TLS)
          ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

        MailAddress from = new MailAddress (SettingsHelper.TailSettings.AlertSettings.SmtpSettings.FromAddress);
        MailAddress to = new MailAddress (SettingsHelper.TailSettings.AlertSettings.EMailAddress);
        
        mailMessage = new MailMessage (from, to)
        {
          Subject = SettingsHelper.TailSettings.AlertSettings.SmtpSettings.Subject,
          SubjectEncoding = System.Text.Encoding.UTF8,
          BodyEncoding = System.Text.Encoding.UTF8,
          IsBodyHtml = false,
        };

        emailTimer = new System.Timers.Timer (300000)
        {
          Enabled = false
        };

        emailTimer.Elapsed += EMailTimerEvent;

        InitSucces = true;
      }
      catch (Exception ex)
      {
        MessageBox.Show (Application.Current.FindResource ("SmtpSettingsNotValid").ToString ( ), LogFile.MSGBOX_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
        ErrorLog.WriteLog (ErrorFlags.Error, "MailClient", string.Format ("InitClient exception: {0}", ex));
      }
    }

    /// <summary>
    /// Send E-Mail
    /// </summary>
    /// <param name="userToken">User token</param>
    /// <param name="bodyMessage">Message to be send</param>
    public void SendMail (string userToken, string bodyMessage = null)
    {
      try
      {
        string userState = userToken;
        mailMessage.Body = bodyMessage;
        mailClient.SendAsync (mailMessage, userState);

        if (userState.CompareTo ("testMessage") != 0)
        {
          emailTimer.Enabled = true;
          Console.WriteLine (string.Format ("Timer start {0}", DateTime.Now));
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show (Application.Current.FindResource ("MailCannotSend").ToString ( ), LogFile.MSGBOX_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
        ErrorLog.WriteLog (ErrorFlags.Error, "MailClient", string.Format ("SendMail exception: {0}", ex));
      }
    }

    /// <summary>
    /// SMTP client initialisation success
    /// </summary>
    public bool InitSucces
    {
      get;
      set;
    }

    /// <summary>
    /// E-Mail timer
    /// </summary>
    public System.Timers.Timer EMailTimer
    {
      get
      {
        return (emailTimer);
      }
    }

    private void SendCompleted (object sender, AsyncCompletedEventArgs e)
    {
      string token = (string) e.UserState;

      if (e.Cancelled)
      {
        MessageBox.Show (string.Format ("{0}\n\"{1}\"", Application.Current.FindResource ("MailCannotSend"), token), LogFile.MSGBOX_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }
      if (e.Error != null)
        MessageBox.Show (string.Format ("{0}\n\"{1}\"", e.Error, token), LogFile.MSGBOX_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
      else
        MessageBox.Show ("Complete!", LogFile.MSGBOX_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private void EMailTimerEvent (object sender, System.Timers.ElapsedEventArgs e)
    {
      try
      {
        emailTimer.Enabled = false;
        Console.WriteLine (string.Format ("Timer end {0}", DateTime.Now));
      }
      catch (Exception ex)
      {
        ErrorLog.WriteLog (ErrorFlags.Error, "MailClient", string.Format ("EMailTimerEvent, exception: {0}", ex));
      }
    }
  }
}
