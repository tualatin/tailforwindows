using System;
using System.Text;
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
    /// Current logfile <see cref="Encoding"/>
    /// </summary>
    public Encoding LogFileEncoding
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="state"><see cref="EStatusbarState"/></param>
    /// <param name="encoding"><see cref="Encoding"/></param>
    public StatusChangedArgs(EStatusbarState state, Encoding encoding)
    {
      State = state;
      LogFileEncoding = encoding;
    }
  }
}
