namespace Org.Vs.TailForWin.Business.Data.Messages
{
  /// <summary>
  /// Disable quick add flag in <see cref="Core.Data.TailData"/>
  /// </summary>
  public class DisableQuickAddInTailDataMessage
  {
    /// <summary>
    /// Is loaded by XML
    /// </summary>
    public bool OpenFromFileManager
    {
      get;
    }

    /// <summary>
    /// Who sends the message
    /// </summary>
    public object Sender
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="sender">Who sends the message</param>
    /// <param name="openFromFileManager">Is loaded by XML</param>
    public DisableQuickAddInTailDataMessage(object sender, bool openFromFileManager)
    {
      OpenFromFileManager = openFromFileManager;
      Sender = sender;
    }
  }
}
