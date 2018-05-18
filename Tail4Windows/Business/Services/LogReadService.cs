using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using log4net;
using Org.Vs.TailForWin.Business.Data;
using Org.Vs.TailForWin.Business.Events.Args;
using Org.Vs.TailForWin.Business.Events.Delegates;
using Org.Vs.TailForWin.Business.Interfaces;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Business.Services
{
  /// <summary>
  /// Log read service
  /// </summary>
  public class LogReadService : ILogReadService
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(LogReadService));

    private readonly BackgroundWorker _tailBackgroundWorker;
    private int _startOffset;

    #region Events

    /// <summary>
    /// A new <see cref="LogEntry"/> is created
    /// </summary>
    public event LogEntryCreated OnLogEntryCreated;

    #endregion

    #region Properties

    /// <summary>
    /// Size refresh time
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

    /// <summary>
    /// <see cref="BackgroundWorker"/> is busy
    /// </summary>
    public bool IsBusy => _tailBackgroundWorker.IsBusy;

    /// <summary>
    /// Current log line index
    /// </summary>
    public int Index
    {
      get;
      private set;
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public LogReadService()
    {
      _tailBackgroundWorker = new BackgroundWorker
      {
        WorkerSupportsCancellation = true
      };
      _tailBackgroundWorker.DoWork += LogReaderServiceDoWork;
      _tailBackgroundWorker.RunWorkerCompleted += LogReaderServiceRunWorkerCompleted;

      Index = 0;
      _startOffset = SettingsHelperController.CurrentSettings.LinesRead;
    }

    /// <summary>
    /// Starts tail
    /// </summary>
    public void StartTail()
    {
      LOG.Trace("Start tail...");

      Thread.CurrentThread.Priority = TailData.ThreadPriority;
      _tailBackgroundWorker.RunWorkerAsync();
    }

    private void LogReaderServiceDoWork(object sender, DoWorkEventArgs e)
    {
      string message = Application.Current.TryFindResource("SizeRefreshTime").ToString();

#if DEBUG
      if ( SettingsHelperController.CurrentSettings.DebugTailReader )
        SimulateTailReading();
#endif
    }

#if DEBUG
    private void SimulateTailReading()
    {
      string message = Application.Current.TryFindResource("SizeRefreshTime").ToString();

      while ( _tailBackgroundWorker != null && !_tailBackgroundWorker.CancellationPending )
      {
        Thread.Sleep((int) TailData.RefreshRate);

        if ( _tailBackgroundWorker.CancellationPending )
          return;

        Index++;
        var log = new LogEntry
        {
          Index = Index,
          Message = $"Log - {Index * 24} / Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.",
          DateTime = DateTime.Now
        };

        SizeRefreshTime = string.Format(message, $"{12 + Index * 12}", DateTime.Now.ToString(SettingsHelperController.CurrentSettings.CurrentStringFormat));

        if ( _tailBackgroundWorker.CancellationPending )
          return;

        OnLogEntryCreated?.Invoke(this, new LogEntryCreatedArgs(log, -1, SizeRefreshTime));
      }
    }
#endif

    private void LogReaderServiceRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      LOG.Trace("Stop finished");
    }

    /// <summary>
    /// Stop tail
    /// </summary>
    public void StopTail()
    {
      MouseService.SetBusyState();
      LOG.Trace("Stop tail.");

      _tailBackgroundWorker.CancelAsync();
    }

    /// <summary>
    /// Reset current index
    /// </summary>
    public void ResetIndex() => Index = 0;
  }
}
