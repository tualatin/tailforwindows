using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using log4net;
using Org.Vs.TailForWin.Business.Services.Data;
using Org.Vs.TailForWin.Business.Services.Events.Args;
using Org.Vs.TailForWin.Business.Services.Events.Delegates;
using Org.Vs.TailForWin.Business.Services.Interfaces;
using Org.Vs.TailForWin.Business.SmartWatchEngine.Controlleres;
using Org.Vs.TailForWin.Business.SmartWatchEngine.Interfaces;
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
    private readonly ManualResetEvent _resetEvent;
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

    /// <summary>
    /// <see cref="ISmartWatchController"/> current SmartWatch
    /// </summary>
    public ISmartWatchController SmartWatch
    {
      get;
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
      SmartWatch = new SmartWatchController();
      _resetEvent = new ManualResetEvent(false);
    }

    /// <summary>
    /// Starts tail
    /// </summary>
    public void StartTail()
    {
      LOG.Trace("Start tail...");

      Thread.CurrentThread.Priority = TailData.ThreadPriority;

      _tailBackgroundWorker.RunWorkerAsync();
      _resetEvent?.Reset();
      SmartWatch.StartSmartWatch(TailData);
    }

    private void LogReaderServiceDoWork(object sender, DoWorkEventArgs e)
    {
      string message = Application.Current.TryFindResource("SizeRefreshTime").ToString();

#if DEBUG
      if ( SettingsHelperController.CurrentSettings.DebugTailReader )
        SimulateTailReading(e);
#endif

      if ( SettingsHelperController.CurrentSettings.DebugTailReader )
        return;

      while ( _tailBackgroundWorker != null && !_tailBackgroundWorker.CancellationPending )
      {
        if ( _tailBackgroundWorker.CancellationPending )
          return;

        // TODO real log file reader here

        _resetEvent?.WaitOne((int) TailData.RefreshRate);
      }
    }

#if DEBUG
    private void SimulateTailReading(DoWorkEventArgs e)
    {
      string message = Application.Current.TryFindResource("SizeRefreshTime").ToString();

      while ( _tailBackgroundWorker != null && !_tailBackgroundWorker.CancellationPending )
      {
        if ( _tailBackgroundWorker.CancellationPending )
          break;

        Index++;
        LogEntry log;
        int mod = Index % 2;

        if ( mod == 0 )
        {
          log = new LogEntry
          {
            Index = Index,
            Message = $"Log - {Index * 24} / Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.",
            DateTime = DateTime.Now
          };
        }
        else
        {
          log = new LogEntry
          {
            Index = Index,
            Message = "Christabel strips and slips like a dream breaking ice with arms that gleam with pain disdain... She throws her head and glides against the stream throwing me her bravest smile defiant glittering shivering guile",
            DateTime = DateTime.Now
          };
        }

        SizeRefreshTime = string.Format(message, $"{24 + Index * 12}", DateTime.Now.ToString(SettingsHelperController.CurrentSettings.CurrentStringFormat));

        if ( _tailBackgroundWorker.CancellationPending )
          break;

        OnLogEntryCreated?.Invoke(this, new LogEntryCreatedArgs(log, SizeRefreshTime));
        _resetEvent?.WaitOne((int) TailData.RefreshRate);
      }

      e.Cancel = true;
    }
#endif

    private void LogReaderServiceRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      LOG.Trace("Stop finished");

      _resetEvent?.Reset();
    }

    /// <summary>
    /// Stop tail
    /// </summary>
    public void StopTail()
    {
      MouseService.SetBusyState();
      LOG.Trace("Stop tail.");

      _tailBackgroundWorker.CancelAsync();
      _resetEvent?.Set();
      SmartWatch.SuspendSmartWatch();
    }

    /// <summary>
    /// Reset current index
    /// </summary>
    public void ResetIndex() => Index = 0;

    /// <summary>
    /// Set current index to special value
    /// </summary>
    /// <param name="index">Index</param>
    public void SetIndex(int index) => Index = index;
  }
}
