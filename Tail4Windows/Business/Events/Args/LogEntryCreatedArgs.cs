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
    private LogEntry Log
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="log"><see cref="LogEntry"/></param>
    public LogEntryCreatedArgs(LogEntry log) => Log = log;
  }
}
