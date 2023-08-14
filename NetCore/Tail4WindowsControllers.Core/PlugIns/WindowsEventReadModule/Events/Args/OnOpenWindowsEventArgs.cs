using Org.Vs.TailForWin.Core.Data;

namespace Org.Vs.TailForWin.Controllers.PlugIns.WindowsEventReadModule.Events.Args
{
  /// <summary>
  /// Open Windows event args
  /// </summary>
  public class OnOpenWindowsEventArgs : EventArgs
  {
    /// <summary>
    /// <see cref="TailForWin.Core.Data.TailData"/>
    /// </summary>
    public TailData TailData
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="tailData"><see cref="TailForWin.Core.Data.TailData"/></param>
    public OnOpenWindowsEventArgs(TailData tailData) => TailData = tailData;
  }
}
