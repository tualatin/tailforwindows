namespace Org.Vs.TailForWin.Data.Messages
{
  /// <summary>
  /// Reset data message
  /// </summary>
  public class ResetDataMessage : OpenSmtpSettingMessage
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="sender">Whos sends the message</param>
    public ResetDataMessage(object sender) : base(sender) { }
  }
}
