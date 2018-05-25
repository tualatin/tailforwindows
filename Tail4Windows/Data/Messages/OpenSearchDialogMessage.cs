namespace Org.Vs.TailForWin.Data.Messages
{
  /// <summary>
  /// Open search dialog message
  /// </summary>
  public class OpenSearchDialogMessage
  {
    /// <summary>
    /// Who sends the message
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public object Sender
    {
      // ReSharper disable once UnusedAutoPropertyAccessor.Global
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="sender">Who sends the message</param>
    public OpenSearchDialogMessage(object sender) => Sender = sender;
  }
}
