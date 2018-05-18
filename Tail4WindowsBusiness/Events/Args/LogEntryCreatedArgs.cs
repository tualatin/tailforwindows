using System;
using Org.Vs.TailForWin.Business.Data;


namespace Org.Vs.TailForWin.Business.Events.Args
{
  /// <summary>
  /// <see cref="LogEntry"/> created <see cref="EventArgs"/>
  /// </summary>
  public class LogEntryCreatedArgs : EventArgs
  {
    /// <summary>
    /// <see cref="LogEntry"/>
    /// </summary>
    public LogEntry Log
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
    /// Size refresh time
    /// </summary>
    public string SizeRefreshTime
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="log"><see cref="LogEntry"/></param>
    /// <param name="linesRead">Lines read</param>
    /// <param name="sizeRefreshTime">Size refresh time</param>
    public LogEntryCreatedArgs(LogEntry log, int linesRead, string sizeRefreshTime)
    {
      Log = log;
      LinesRead = linesRead;
      SizeRefreshTime = sizeRefreshTime;
    }
  }
}
