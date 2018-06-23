using System;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils;


namespace Org.Vs.TailForWin.Data.Messages
{
  /// <summary>
  /// Open <see cref="Core.Data.TailData"/> in new <see cref="DragSupportTabItem"/>
  /// </summary>
  public class OpenTailDataAsNewTabItem
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
    /// Is SmartWatch object
    /// </summary>
    public bool IsSmartWatch
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
    /// <param name="isSmartWatch">Is SmartWatch object</param>
    public OpenTailDataAsNewTabItem(object sender, TailData tailData, Guid id, bool isSmartWatch)
    {
      Sender = sender;
      TailData = tailData;
      ParentGuid = id;
      IsSmartWatch = isSmartWatch;
    }
  }
}
