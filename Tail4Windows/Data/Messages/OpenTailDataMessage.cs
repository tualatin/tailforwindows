using System;
using Org.Vs.TailForWin.Core.Data;


namespace Org.Vs.TailForWin.Data.Messages
{
  /// <summary>
  /// Open <see cref="TailData"/>
  /// </summary>
  public class OpenTailDataMessage
  {
    /// <summary>
    /// Parent Guid
    /// </summary>
    public Guid ParentGuid
    {
      get;
    }

    /// <summary>
    /// <see cref="TailData"/>
    /// </summary>
    public TailData TailData
    {
      get;
    }

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
    /// <param name="sender">Who sends the message</param>
    /// <param name="tailData"><see cref="TailData"/></param>
    /// <param name="id"><see cref="Guid"/></param>
    public OpenTailDataMessage(object sender, TailData tailData, Guid id)
    {
      Sender = sender;
      TailData = tailData;
      ParentGuid = id;
    }
  }
}
