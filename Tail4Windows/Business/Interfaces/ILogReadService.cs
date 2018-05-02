using System.Threading;
using Org.Vs.TailForWin.Business.Data;
using Org.Vs.TailForWin.Business.Events.Delegates;
using Org.Vs.TailForWin.Core.Data;


namespace Org.Vs.TailForWin.Business.Interfaces
{
  /// <summary>
  /// Log read service interface
  /// </summary>
  public interface ILogReadService
  {
    /// <summary>
    /// A new <see cref="LogEntry"/> is created
    /// </summary>
    event LogEntryCreated OnLogEntryCreated;

    /// <summary>
    /// Lines read
    /// </summary>
    string LinesRead
    {
      get;
    }

    /// <summary>
    /// Size refresh time
    /// </summary>
    string SizeRefreshTime
    {
      get;
    }

    /// <summary>
    /// <see cref="Core.Data.TailData"/>
    /// </summary>
    TailData TailData
    {
      get;
      set;
    }

    /// <summary>
    /// Starts tail
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    void StartTail(CancellationToken token);

    /// <summary>
    /// Stop tail
    /// </summary>
    void StopTail();
  }
}
