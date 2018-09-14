using System;
using Org.Vs.TailForWin.Core.Data;

namespace Org.Vs.TailForWin.Data.Messages
{
  /// <summary>
  /// Change selected tab item message
  /// </summary>
  public class ChangeSelectedTabItemMessage
  {
    /// <summary>
    /// Parent ID
    /// </summary>
    public Guid ParentId
    {
      get;
    }

    /// <summary>
    /// Current Window ID
    /// </summary>
    public Guid WindowId
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
    /// Standard constructor
    /// </summary>
    /// <param name="parentId">Parent ID</param>
    /// <param name="windowId">Current Window ID</param>
    /// <param name="tailData"><see cref="TailData"/></param>
    public ChangeSelectedTabItemMessage(Guid parentId, Guid windowId, TailData tailData)
    {
      ParentId = parentId;
      WindowId = windowId;
      TailData = tailData;
    }
  }
}
