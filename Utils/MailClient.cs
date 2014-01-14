using System.Net.Mail;
using TailForWin.Controller;
using System.Net;
using TailForWin.Data;


namespace TailForWin.Utils
{
  public class MailClient
  {
    public void InitClient ( )
    {
      SmtpClient myClient = new SmtpClient (SettingsHelper.TailSettings.AlertSettings.SmtpSettings.SmtpServerName, SettingsHelper.TailSettings.AlertSettings.SmtpSettings.SmtpPort)
      {
        UseDefaultCredentials = false
      };
      string decryptPassword = StringEncryption.Decrypt (SettingsHelper.TailSettings.AlertSettings.SmtpSettings.Password, LogFile.ENCRYPT_PASSPHRASE);
      NetworkCredential authInfo = new NetworkCredential (SettingsHelper.TailSettings.AlertSettings.SmtpSettings.LoginName, decryptPassword);
      myClient.Credentials = authInfo;

      MailAddress from = new MailAddress (SettingsHelper.TailSettings.AlertSettings.EMailAddress);
    }
  }
}
