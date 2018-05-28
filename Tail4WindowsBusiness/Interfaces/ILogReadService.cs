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
    /// A new <see cref="Data.LogEntry"/> is created
    /// </summary>
    event LogEntryCreated OnLogEntryCreated;

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
    /// <see cref="System.ComponentModel.BackgroundWorker"/> is busy
    /// </summary>
    bool IsBusy
    {
      get;
    }

    /// <summary>
    /// Current log line index
    /// </summary>
    int LineIndex
    {
      get;
    }

    /// <summary>
    /// Starts tail
    /// </summary>
    void StartTail();

    /// <summary>
    /// Stop tail
    /// </summary>
    void StopTail();

    /// <summary>
    /// Reset current index
    /// </summary>
    void ResetIndex();

    /// <summary>
    /// Set current index to special value
    /// </summary>
    /// <param name="index">Index</param>
    void SetIndex(int index);
  }
}
