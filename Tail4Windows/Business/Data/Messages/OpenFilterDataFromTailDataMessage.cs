using Org.Vs.TailForWin.Core.Data;


namespace Org.Vs.TailForWin.Business.Data.Messages
{
  /// <summary>
  /// Open <see cref="FilterData"/> from <see cref="TailData"/>
  /// </summary>
  public class OpenFilterDataFromTailDataMessage : AddTailDataToQuickAddMessage
  {
    /// <summary>
    /// Standarc constructor
    /// </summary>
    /// <param name="sender">Who sends the message</param>
    /// <param name="tailData"><see cref="TailData"/></param>
    public OpenFilterDataFromTailDataMessage(object sender, TailData tailData)
      : base(sender, tailData)
    {
    }
  }
}
