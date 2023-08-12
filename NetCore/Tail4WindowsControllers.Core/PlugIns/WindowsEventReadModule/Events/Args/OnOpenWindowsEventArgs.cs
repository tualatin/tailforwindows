using Org.Vs.Tail4Win.Core.Data;

namespace Org.Vs.Tail4Win.Controllers.PlugIns.WindowsEventReadModule.Events.Args
{
  /// <summary>
  /// Open Windows event args
  /// </summary>
  public class OnOpenWindowsEventArgs : EventArgs
  {
    /// <summary>
    /// <see cref="Core.Data.TailData"/>
    /// </summary>
    public TailData TailData
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="tailData"><see cref="Core.Data.TailData"/></param>
    public OnOpenWindowsEventArgs(TailData tailData) => TailData = tailData;
  }
}
