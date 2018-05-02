using System.Threading;
using log4net;
using Org.Vs.TailForWin.Business.Data;
using Org.Vs.TailForWin.Business.Events.Delegates;
using Org.Vs.TailForWin.Business.Interfaces;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.UI.Services;


namespace Org.Vs.TailForWin.Business.Services
{
  /// <summary>
  /// Log read service
  /// </summary>
  public class LogReadService : ILogReadService
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(LogReadService));

    private CancellationToken _token;

    #region Events

    /// <summary>
    /// A new <see cref="LogEntry"/> is created
    /// </summary>
    public event LogEntryCreated OnLogEntryCreated;

    #endregion

    #region Properties

    /// <summary>
    /// Lines read
    /// </summary>
    public string LinesRead
    {
      get;
      private set;
    }

    /// <summary>
    /// Size and refresh time
    /// </summary>
    public string SizeRefreshTime
    {
      get;
      private set;
    }

    /// <summary>
    /// <see cref="Core.Data.TailData"/>
    /// </summary>
    public TailData TailData
    {
      get;
      set;
    }

    #endregion

    /// <summary>
    /// Starts tail
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    public void StartTail(CancellationToken token)
    {
      LOG.Trace("Start tail...");

      _token = token;
    }

    /// <summary>
    /// Stop tail
    /// </summary>
    public void StopTail()
    {
      MouseService.SetBusyState();

      LOG.Trace("Stop tail.");
    }
  }
}
