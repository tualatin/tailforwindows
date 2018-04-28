using System;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils;


namespace Org.Vs.TailForWin.Business.Data.Messages
{
  /// <summary>
  /// Open <see cref="TailData"/> in new <see cref="DragSupportTabItem"/>
  /// </summary>
  public class OpenTailDataAsNewTabItem : OpenTailDataMessage
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="sender">Who sends the message</param>
    /// <param name="tailData"><see cref="TailData"/></param>
    /// <param name="id"><see cref="Guid"/></param>
    public OpenTailDataAsNewTabItem(object sender, TailData tailData, Guid id)
      : base(sender, tailData, id)
    {
    }
  }
}
