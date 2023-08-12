using Org.Vs.Tail4Win.Business.Services.Data;

namespace Org.Vs.Tail4Win.Business.Services.Events.Args
{
  /// <summary>
  /// <see cref="LogEntry"/> created <see cref="EventArgs"/>
  /// </summary>
  public class LogEntryCreatedArgs : EventArgs
  {
    /// <summary>
    /// <see cref="LogEntry"/>
    /// </summary>
    public List<LogEntry> Log
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
    /// <param name="log"><see cref="List{T}"/> of <see cref="LogEntry"/></param>
    /// <param name="sizeRefreshTime">Size refresh time</param>
    public LogEntryCreatedArgs(List<LogEntry> log, string sizeRefreshTime)
    {
      Log = log;
      SizeRefreshTime = sizeRefreshTime;
    }
  }
}
