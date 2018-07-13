using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using log4net;
using Org.Vs.TailForWin.Business.Services.Data;
using Org.Vs.TailForWin.Business.Services.Events.Args;
using Org.Vs.TailForWin.Business.Services.Events.Delegates;
using Org.Vs.TailForWin.Business.Services.Interfaces;
using Org.Vs.TailForWin.Business.SmartWatchEngine.Interfaces;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;


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

    #region Events

    /// <summary>
    /// A new <see cref="LogEntry"/> is created
    /// </summary>
    public event LogEntryCreated OnLogEntryCreated;

    #endregion

    #region Properties

    /// <summary>
    /// <see cref="ISmartWatchController"/> current SmartWatch
    /// </summary>
    public ISmartWatchController SmartWatch => null;

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
    /// Sets current fileoffset to zero
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void SetFileOffsetZero() => throw new NotImplementedException();

    /// <summary>
    /// Gets current file offset
    /// </summary>
    /// <returns>Current file offset</returns>
    /// <exception cref="NotImplementedException"></exception>
    public long GetFileOffset() => throw new NotImplementedException();

    /// <summary>
    /// Sets current file offset
    /// </summary>
    /// <param name="offset">Offset</param>
    /// <exception cref="NotImplementedException"></exception>
    public void SetFileOffset(long offset) => throw new NotImplementedException();

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public WindowsEventReadService()
    {
      Index = 0;
      _startOffset = SettingsHelperController.CurrentSettings.LinesRead;
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
          lastItems.Add(CreateLogEntryByWindowsEvent(_logReader.Entries[i]));
        }

        // Reverse list
        lastItems.Reverse();
        lastItems.ForEach(p =>
        {
          SizeRefreshTime = string.Format(_message, _logReader.Entries.Count, p.DateTime.ToString(SettingsHelperController.CurrentSettings.CurrentStringFormat));
          OnLogEntryCreated?.Invoke(this, new LogEntryCreatedArgs(p, SizeRefreshTime));
        });

        Index = _startOffset;
      }

      _sw.Start();
      IsBusy = true;
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

      LOG.Trace($"Stop tail, tail was running about {_sw.ElapsedMilliseconds:N0} ms");
    }

    private void LogReaderEntryWritten(object sender, EntryWrittenEventArgs e)
    {
      Index++;
      SizeRefreshTime = string.Format(_message, _logReader.Entries.Count, e.Entry.TimeWritten.ToString(SettingsHelperController.CurrentSettings.CurrentStringFormat));
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
            string category;

            if ( string.Compare(eventLog.Log, "application", StringComparison.OrdinalIgnoreCase) == 0 ||
                 string.Compare(eventLog.Log, "security", StringComparison.OrdinalIgnoreCase) == 0 ||
                 string.Compare(eventLog.Log, "system", StringComparison.OrdinalIgnoreCase) == 0 )
              category = windowsLogs;
            else
              category = applicationLogs;

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

    private LogEntry CreateLogEntryByWindowsEvent(EventLogEntry e)
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

      return log;
    }

    #endregion
  }
}
