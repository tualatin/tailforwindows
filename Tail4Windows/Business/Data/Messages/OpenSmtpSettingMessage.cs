namespace Org.Vs.TailForWin.Business.Data.Messages
{
  /// <summary>
  /// Open SMTP setting message
  /// </summary>
  public class OpenSmtpSettingMessage
  {
    /// <summary>
    /// Who is sending the message
    /// </summary>
    public object Sender
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="sender">Who is sending the message</param>
    public OpenSmtpSettingMessage(object sender) => Sender = sender;
  }
}
