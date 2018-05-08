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
using Org.Vs.TailForWin.UI.Services;


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
    /// Lines read
    /// </summary>
    public int LinesRead
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

    /// <summary>
    /// <see cref="BackgroundWorker"/> is busy
    /// </summary>
    public bool IsBusy => _tailBackgroundWorker.IsBusy;

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
      int index = 1;
      string message = Application.Current.TryFindResource("SizeRefreshTime").ToString();

      while ( _tailBackgroundWorker != null && !_tailBackgroundWorker.CancellationPending )
      {
        Thread.Sleep((int) TailData.RefreshRate);

        GC.Collect();
        GC.WaitForPendingFinalizers();

        //if ( SettingsHelper.TailSettings.DefaultTimeFormat == ETimeFormat.HHMMd || SettingsHelper.TailSettings.DefaultTimeFormat == ETimeFormat.HHMMD )
        //  StringFormatData.StringFormat = $"{SettingsData.GetEnumDescription(SettingsHelper.TailSettings.DefaultDateFormat)} {SettingsData.GetEnumDescription(SettingsHelper.TailSettings.DefaultTimeFormat)}:ss.fff";
        //if ( SettingsHelper.TailSettings.DefaultTimeFormat == ETimeFormat.HHMMSSd || SettingsHelper.TailSettings.DefaultTimeFormat == ETimeFormat.HHMMSSD )
        //  StringFormatData.StringFormat = $"{SettingsData.GetEnumDescription(SettingsHelper.TailSettings.DefaultDateFormat)} {SettingsData.GetEnumDescription(SettingsHelper.TailSettings.DefaultTimeFormat)}.fff";


        var log = new LogEntry
        {
          Index = index,
          Message = $"Log - {index * 24}",
          DateTime = DateTime.Now
        };

        LinesRead = index;
        SizeRefreshTime = string.Format(message, $"{12 + index * 12}", DateTime.Now);
        OnLogEntryCreated?.Invoke(this, new LogEntryCreatedArgs(log, LinesRead, SizeRefreshTime));

        index++;

        LOG.Trace($"{index}");
      }
    }

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
  }
}
