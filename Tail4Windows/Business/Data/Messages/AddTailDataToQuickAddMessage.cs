using Org.Vs.TailForWin.Core.Data;


namespace Org.Vs.TailForWin.Business.Data.Messages
{
  /// <summary>
  /// AddTailDataToQuickAdd message
  /// </summary>
  public class AddTailDataToQuickAddMessage
  {
    /// <summary>
    /// <see cref="TailData"/>
    /// </summary>
    public TailData TailData
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
    /// Standarc constructor
    /// </summary>
    /// <param name="sender">Who sends the message</param>
    /// <param name="tailData"><see cref="TailData"/></param>
    public AddTailDataToQuickAddMessage(object sender, TailData tailData)
    {
      TailData = tailData;
      Sender = sender;
    }
  }
}
