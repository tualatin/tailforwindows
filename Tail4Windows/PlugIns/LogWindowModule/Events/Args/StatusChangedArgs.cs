using System;
using Org.Vs.TailForWin.Core.Enums;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule.Events.Args
{
  /// <summary>
  /// StatusChanged args
  /// </summary>
  public class StatusChangedArgs : EventArgs
  {
    /// <summary>
    /// State <see cref="EStatusbarState"/>
    /// </summary>
    public EStatusbarState State
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="state"><see cref="EStatusbarState"/></param>
    public StatusChangedArgs(EStatusbarState state)
    {
      State = state;
    }
  }
}
