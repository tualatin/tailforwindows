using System;
using Org.Vs.TailForWin.Business.Services.Data;


namespace Org.Vs.TailForWin.Business.Services.Events.Args
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
    /// <param name="sizeRefreshTime">Size refresh time</param>
    public LogEntryCreatedArgs(LogEntry log, string sizeRefreshTime)
    {
      Log = log;
      SizeRefreshTime = sizeRefreshTime;
    }
  }
}
