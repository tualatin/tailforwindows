using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using log4net;
using Org.Vs.TailForWin.Business.Services.Data;
using Org.Vs.TailForWin.Business.Services.Events.Args;
using Org.Vs.TailForWin.Business.Services.Events.Delegates;
using Org.Vs.TailForWin.Business.Services.Interfaces;
using Org.Vs.TailForWin.Business.SmartWatchEngine.Interfaces;
using Org.Vs.TailForWin.Business.StatisticEngine.Data.Messages;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Base;


namespace Org.Vs.TailForWin.Business.Services
{
  /// <summary>
  /// Windows event reader
  /// </summary>
  public class WindowsEventReadService : ILogReadService
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(WindowsEventReadService));

    private int _startOffset;
    private EventLog _logReader;
    private readonly string _message;
    private readonly Stopwatch _sw;
    private CancellationTokenSource _cts;

    #region Events

    /// <summary>
    /// A new <see cref="LogEntry"/> is created
    /// </summary>
    public event LogEntryCreated OnLogEntryCreated;

    /// <summary>
    /// Log file is cleared or deleted
    /// </summary>
    public event EventHandler OnLogCleared;

    #endregion

    #region Properties

    /// <summary>
    /// <see cref="ISmartWatchController"/> current SmartWatch
    /// </summary>
    public ISmartWatchController SmartWatch => null;

    /// <summary>
    /// LogReader Id
    /// </summary>
    public Guid LogReaderId
    {
      get;
    }

    /// <summary>
    /// Size refresh time
    /// </summary>
    public string SizeRefreshTime
    {
      get;
      private set;
    }

    /// <summary>
    /// File size or total events
    /// </summary>
    public double FileSizeTotalEvents
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
    /// <see cref="System.ComponentModel.BackgroundWorker"/> is busy
    /// </summary>
    public bool IsBusy
    {
      get;
      private set;
    }

    /// <summary>
    /// Current log line index
    /// </summary>
    public int Index
    {
      get;
      private set;
    }

    /// <summary>
    /// Elapsed time
    /// </summary>
    public TimeSpan ElapsedTime => _sw.Elapsed;

    /// <summary>
    /// Sets current file offset to zero
    /// </summary>
    public void SetFileOffsetZero() => LOG.Debug("Not implemented!");

    /// <summary>
    /// Gets current file offset
    /// </summary>
    /// <returns>Current file offset</returns>
    public long GetFileOffset() => -1;

    /// <summary>
    /// Sets current file offset
    /// </summary>
    /// <param name="offset">Offset</param>
    public void SetFileOffset(long offset) => LOG.Debug("Not implemented!");

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public WindowsEventReadService()
    {
      Index = 0;
      _startOffset = SettingsHelperController.CurrentSettings.LinesRead;
      LogReaderId = Guid.NewGuid();
      _message = Application.Current.TryFindResource("WindowsEventTotalEvents").ToString();
      _sw = new Stopwatch();
    }

    /// <summary>
    /// Starts tail
    /// </summary>
    public void StartTail()
    {
      if ( _logReader != null )
      {
        _logReader.EntryWritten -= LogReaderEntryWritten;
        _logReader = null;
      }

      LOG.Trace($"Start read Windows events by category {TailData.WindowsEvent.Category}, on machine {TailData.WindowsEvent.Machine}");

      _logReader = new EventLog(TailData.WindowsEvent.Category, TailData.WindowsEvent.Machine)
      {
        EnableRaisingEvents = true
      };
      _logReader.EntryWritten += LogReaderEntryWritten;

      if ( Index == 0 && _logReader.Entries.Count > 0 )
      {
        var lastItems = new List<LogEntry>(SettingsHelperController.CurrentSettings.LinesRead);

        if ( _startOffset > _logReader.Entries.Count )
          _startOffset = _logReader.Entries.Count;

        int index = _startOffset;

        for ( int i = _logReader.Entries.Count - 1; i >= _logReader.Entries.Count - _startOffset; i-- )
        {
          Index = index;
          index--;
          lastItems.Add(CreateLogEntryByWindowsEvent(_logReader.Entries[i]).First());
        }

        // Reverse list
        lastItems.Reverse();
        lastItems.ForEach(p =>
        {
          SizeRefreshTime = string.Format(_message, _logReader.Entries.Count, p.DateTime.ToString(SettingsHelperController.CurrentSettings.CurrentStringFormat));
          FileSizeTotalEvents = _logReader.Entries.Count;

          OnLogEntryCreated?.Invoke(this, new LogEntryCreatedArgs(new List<LogEntry> { p }, SizeRefreshTime));
        });

        Index = _startOffset;
      }

      _sw.Start();
      IsBusy = true;

      NotifyTaskCompletion.Create(UpdateStatisticsAsync);
      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new StatisticChangeReaderMessage(LogReaderId, Index, TailData.WindowsEvent.Category, TailData.IsWindowsEvent));
    }

    /// <summary>
    /// Stops tail
    /// </summary>
    public void StopTail()
    {
      if ( _logReader == null )
        return;

      _logReader.EntryWritten -= LogReaderEntryWritten;
      _logReader = null;
      IsBusy = false;

      _sw.Stop();
      _cts.Cancel();

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new StatisticUpdateReaderMessage(LogReaderId, Index, TailData.WindowsEvent.Category, _sw.Elapsed));
      LOG.Trace($"Stop tail, tail was running about {_sw.ElapsedMilliseconds:N0} ms");
    }

    /// <summary>
    /// Auto save into Database if Statistics is enabled
    /// </summary>
    /// <returns><see cref="Task{TResult}"/></returns>
    private async Task UpdateStatisticsAsync()
    {
      _cts?.Dispose();
      _cts = new CancellationTokenSource();

      while ( !_cts.IsCancellationRequested )
      {
        await Task.Delay(TimeSpan.FromMinutes(30), _cts.Token).ConfigureAwait(false);
        EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new StatisticUpdateReaderMessage(LogReaderId, Index, TailData.FileName, _sw.Elapsed));
      }
    }

    private void LogReaderEntryWritten(object sender, EntryWrittenEventArgs e)
    {
      Index++;
      SizeRefreshTime = string.Format(_message, _logReader.Entries.Count, e.Entry.TimeWritten.ToString(SettingsHelperController.CurrentSettings.CurrentStringFormat));
      FileSizeTotalEvents = _logReader.Entries.Count;

      OnLogEntryCreated?.Invoke(this, new LogEntryCreatedArgs(CreateLogEntryByWindowsEvent(e.Entry), SizeRefreshTime));
    }

    /// <summary>
    /// Get <see cref="ObservableCollection{T}"/> of <see cref="WindowsEventCategory"/> with Windows events categories
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>Task</returns>
    public async Task<ObservableCollection<WindowsEventCategory>> GetCategoriesAsync(CancellationToken token)
    {
      var result = new ObservableCollection<WindowsEventCategory>();
      string windowsLogs = Application.Current.TryFindResource("WindowsEventWindowsLogs").ToString();
      string applicationLogs = Application.Current.TryFindResource("WindowsEventApplicationLogs").ToString();

      await Task.Run(() =>
      {
        LOG.Trace("Get Windows event categories");

        foreach ( EventLog eventLog in EventLog.GetEventLogs(TailData.WindowsEvent.Machine) )
        {
          try
          {
            var category = string.Compare(eventLog.Log, "application", StringComparison.OrdinalIgnoreCase) == 0 ||
                 string.Compare(eventLog.Log, "security", StringComparison.OrdinalIgnoreCase) == 0 ||
                 string.Compare(eventLog.Log, "system", StringComparison.OrdinalIgnoreCase) == 0
              ? windowsLogs
              : applicationLogs;

            result.Add(new WindowsEventCategory
            {
              Category = category,
              Log = eventLog.Log,
              LogDisplayName = eventLog.LogDisplayName
            });
          }
          catch
          {
            // Nothing
          }
        }
      }, token).ConfigureAwait(false);

      return result;
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

    #region HelperFunctions

    private List<LogEntry> CreateLogEntryByWindowsEvent(EventLogEntry e)
    {
      string category = string.Format(Application.Current.TryFindResource("WindowsEventCategory").ToString(), e.EntryType.ToString());
      string source = string.Format(Application.Current.TryFindResource("WindowsEventSource").ToString(), e.Source);
      string machineName = string.Format(Application.Current.TryFindResource("WindowsEventMachine").ToString(), e.MachineName);
      string message = string.Format(Application.Current.TryFindResource("WindowsEventMessage").ToString(), e.Message);
      var log = new LogEntry
      {
        Index = Index,
        DateTime = e.TimeWritten,
        Message = $"{category} -> {source} -> {machineName} -> {message}"
      };

      return new List<LogEntry> { log };
    }

    #endregion
  }
}
