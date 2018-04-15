using Org.Vs.TailForWin.Core.Data;


namespace Org.Vs.TailForWin.Business.Data.Messages
{
  /// <summary>
  /// Open <see cref="TailData"/>
  /// </summary>
  public class OpenTailDataMessage
  {
    /// <summary>
    /// Who sends the message
    /// </summary>
    public object Sender
    {
      get;
    }

    /// <summary>
    /// <see cref="TailData"/> to open
    /// </summary>
    public TailData TailData
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="sender">Who sends the message</param>
    /// <param name="tailData"><see cref="TailData"/></param>
    public OpenTailDataMessage(object sender, TailData tailData)
    {
      Sender = sender;
      TailData = tailData;
    }
  }
}
