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
    /// Lines read
    /// </summary>
    public int LinesRead
    {
      get;
    }

    /// <summary>
    /// Size and refresh time
    /// </summary>
    public string SizeRefreshTime
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="state"><see cref="EStatusbarState"/></param>
    /// <param name="encoding"><see cref="Encoding"/></param>
    /// <param name="linesRead">Lines read</param>
    /// <param name="sizeRefreshTime">Size and refresh time</param>
    public StatusChangedArgs(EStatusbarState state, Encoding encoding, int linesRead, string sizeRefreshTime)
    {
      State = state;
      LogFileEncoding = encoding;
      LinesRead = linesRead;
      SizeRefreshTime = sizeRefreshTime;
    }
  }
}
