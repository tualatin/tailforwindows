using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Core.Controllers
{
  /// <summary>
  /// E-Mail controller
  /// </summary>
  public class MailController : IMailController
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(MailController));

    private SmtpClient _mailClient;
    private MailMessage _mailMessage;


    /// <summary>
    /// Standard constructor
    /// </summary>
    public MailController() => InitializeComponents();

    private void InitializeComponents()
    {
      try
      {
        _mailClient = new SmtpClient(SettingsHelperController.CurrentSettings.SmtpSettings.SmtpServerName, SettingsHelperController.CurrentSettings.SmtpSettings.SmtpPort)
        {
          UseDefaultCredentials = false,
          Timeout = (int) TimeSpan.FromMinutes(5).TotalMilliseconds,
          DeliveryMethod = SmtpDeliveryMethod.Network,
          EnableSsl = SettingsHelperController.CurrentSettings.SmtpSettings.Ssl,
        };
        _mailClient.SendCompleted += MailSendCompleted;

        if ( SettingsHelperController.CurrentSettings.SmtpSettings.Tls )
          ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        _mailMessage = new MailMessage(SettingsHelperController.CurrentSettings.SmtpSettings.FromAddress, SettingsHelperController.CurrentSettings.AlertSettings.MailAddress)
        {
          Subject = SettingsHelperController.CurrentSettings.SmtpSettings.Subject,
          SubjectEncoding = Encoding.UTF8,
          BodyEncoding = Encoding.UTF8,
          IsBodyHtml = false
        };
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    private void MailSendCompleted(object sender, AsyncCompletedEventArgs e)
    {
      throw new NotImplementedException();
    }

    private async Task<NetworkCredential> CreateSecurePasswordAsync()
    {
      string result = await StringEncryption.DecryptAsync(SettingsHelperController.CurrentSettings.SmtpSettings.Password).ConfigureAwait(false);
      SecureString password = new SecureString();

      foreach ( char t in result )
      {
        password.AppendChar(t);
      }
      return new NetworkCredential(SettingsHelperController.CurrentSettings.SmtpSettings.LoginName, password);
    }

    /// <summary>
    /// Send E-Mail async
    /// </summary>
    /// <param name="message">Message</param>
    /// <returns>Task</returns>
    public async Task SendLogMailAsync(string message = null)
    {
      try
      {
        _mailClient.Credentials = await CreateSecurePasswordAsync().ConfigureAwait(false);

        if ( !string.IsNullOrWhiteSpace(message) )
          _mailMessage.Body = message;

        await _mailClient.SendMailAsync(_mailMessage).ConfigureAwait(false);
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }
  }
}
