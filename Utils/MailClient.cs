using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Windows;
using log4net;
using Org.Vs.TailForWin.Controller;
using Org.Vs.TailForWin.Data;


namespace Org.Vs.TailForWin.Utils
{
  /// <summary>
  /// MailClient
  /// </summary>
  public class MailClient : IDisposable
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(MailClient));

    private SmtpClient mailClient;
    private MailMessage mailMessage;
    private System.Timers.Timer emailTimer;
    private List<string> messageCollection;

    /// <summary>
    /// Send E-Mail complete event handler
    /// </summary>
    public event EventHandler SendMailComplete;


    /// <summary>
    /// Free up unused objects
    /// </summary>
    public void Dispose ()
    {
      if (mailClient != null)
      {
        mailClient.Dispose();
        mailClient = null;
      }

      if (mailMessage != null)
      {
        mailMessage.Dispose();
        mailMessage = null;
      }

      if (emailTimer == null)
        return;

      emailTimer.Dispose();
      emailTimer = null;
    }

    /// <summary>
    /// Init E-Mail client engine
    /// </summary>
    public void InitClient ()
    {
      InitSucces = false;

      if (string.IsNullOrEmpty(SettingsHelper.TailSettings.AlertSettings.SmtpSettings.SmtpServerName))
        return;
      if (SettingsHelper.TailSettings.AlertSettings.SmtpSettings.SmtpPort <= 0)
        return;

      try
      {
        mailClient = new SmtpClient(SettingsHelper.TailSettings.AlertSettings.SmtpSettings.SmtpServerName, SettingsHelper.TailSettings.AlertSettings.SmtpSettings.SmtpPort)
        {
          UseDefaultCredentials = false,
          Timeout = 30000,
          DeliveryMethod = SmtpDeliveryMethod.Network
        };
        string decryptPassword = StringEncryption.Decrypt(SettingsHelper.TailSettings.AlertSettings.SmtpSettings.Password, LogFile.ENCRYPT_PASSPHRASE);
        NetworkCredential authInfo = new NetworkCredential(SettingsHelper.TailSettings.AlertSettings.SmtpSettings.LoginName, decryptPassword);
        mailClient.Credentials = authInfo;
        mailClient.SendCompleted += SendCompleted;

        if (SettingsHelper.TailSettings.AlertSettings.SmtpSettings.SSL)
          mailClient.EnableSsl = true;
        if (SettingsHelper.TailSettings.AlertSettings.SmtpSettings.TLS)
          ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

        MailAddress from = new MailAddress(SettingsHelper.TailSettings.AlertSettings.SmtpSettings.FromAddress);
        MailAddress to = new MailAddress(SettingsHelper.TailSettings.AlertSettings.EMailAddress);

        mailMessage = new MailMessage(from, to)
        {
          Subject = SettingsHelper.TailSettings.AlertSettings.SmtpSettings.Subject,
          SubjectEncoding = System.Text.Encoding.UTF8,
          BodyEncoding = System.Text.Encoding.UTF8,
          IsBodyHtml = false
        };

        emailTimer = new System.Timers.Timer(300000)
        {
          Enabled = false
        };

        emailTimer.Elapsed += EMailTimerEvent;

        messageCollection = new List<string>();

        InitSucces = true;
      }
      catch (Exception ex)
      {
        MessageBox.Show(Application.Current.FindResource("SmtpSettingsNotValid") as string, LogFile.MSGBOX_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    /// <summary>
    /// Send E-Mail
    /// </summary>
    /// <param name="userToken">User token</param>
    /// <param name="bodyMessage">Message to be send</param>
    /// <exception cref="ArgumentException">If userToken is null or empty</exception>
    public void SendMail (string userToken, string bodyMessage = null)
    {
      Arg.NotNull(userToken, "UserToken");

      try
      {
        string userState = userToken;

        if (bodyMessage != null)
          mailMessage.Body = bodyMessage;

        if (string.Compare(userState, "testMessage", StringComparison.Ordinal) == 0)
          mailMessage.Body = string.Format("Testmail from {0}", LogFile.APPLICATION_CAPTION);

        if (!EMailTimer.Enabled)
          mailClient.SendAsync(mailMessage, userToken);
        else
        {
          CollectMessages(bodyMessage);
          return;
        }

        if (string.Compare(userState, "testMessage", StringComparison.Ordinal) == 0)
          return;

        emailTimer.Enabled = true;
        Console.WriteLine(@"Timer start {0}", DateTime.Now);
      }
      catch (Exception ex)
      {
        MessageBox.Show(Application.Current.FindResource("MailCannotSend") as string, LogFile.MSGBOX_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
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
        MessageBox.Show(string.Format("{0}\n\"{1}\"", Application.Current.FindResource("MailCannotSend"), token), LogFile.MSGBOX_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);

        if (SendMailComplete != null)
          SendMailComplete(sender, EventArgs.Empty);
        return;
      }

      MessageBox.Show(e.Error != null ? string.Format("{0}\n\"{1}\"", e.Error, token) : "Complete!", LogFile.MSGBOX_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);

      if (SendMailComplete != null)
        SendMailComplete(sender, EventArgs.Empty);
    }

    private void EMailTimerEvent (object sender, System.Timers.ElapsedEventArgs e)
    {
      try
      {
        emailTimer.Enabled = false;
        string body = string.Empty;

        messageCollection.ForEach(message =>
         {
           body += string.Format("{0}\n", message);
         });

        messageCollection.Clear();

        SendMail("AlertTrigger", body);
        Console.WriteLine(@"Timer end {0}", DateTime.Now);
      }
      catch (Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    private void CollectMessages (string message)
    {
      messageCollection.Add(message);
      LOG.Debug("{0} add message ...", System.Reflection.MethodBase.GetCurrentMethod().Name);
    }
  }
}
