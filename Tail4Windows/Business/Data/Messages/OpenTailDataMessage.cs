using System;
using Org.Vs.TailForWin.Core.Data;


namespace Org.Vs.TailForWin.Business.Data.Messages
{
  /// <summary>
  /// Open <see cref="TailData"/>
  /// </summary>
  public class OpenTailDataMessage : AddTailDataToQuickAddMessage
  {
    /// <summary>
    /// Parent Guid
    /// </summary>
    public Guid ParentGuid
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="sender">Who sends the message</param>
    /// <param name="tailData"><see cref="TailData"/></param>
    /// <param name="id"><see cref="Guid"/></param>
    public OpenTailDataMessage(object sender, TailData tailData, Guid id)
    : base(sender, tailData) => ParentGuid = id;
  }
}
