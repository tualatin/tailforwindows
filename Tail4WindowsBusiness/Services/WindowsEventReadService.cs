using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

    private readonly int _startOffset;
    private EventLog _logReader;

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

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public WindowsEventReadService()
    {
      Index = 0;
      _startOffset = SettingsHelperController.CurrentSettings.LinesRead;
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

      if ( Index == 0 )
      {
        var lastItems = new List<LogEntry>(SettingsHelperController.CurrentSettings.LinesRead);
        int index = _startOffset;

        for ( int i = _logReader.Entries.Count - 1; i >= _logReader.Entries.Count - _startOffset; i-- )
        {
          Index = index;
          index--;
          lastItems.Add(CreateLogEntryByWindowsEvent(_logReader.Entries[i]));
        }

        // Reverse the list
        lastItems.Reverse();
        lastItems.ForEach(p =>
        {
          SizeRefreshTime = p.DateTime.ToString(SettingsHelperController.CurrentSettings.CurrentStringFormat);
          OnLogEntryCreated?.Invoke(this, new LogEntryCreatedArgs(p, SizeRefreshTime));
        });

        Index = _startOffset;
      }

      IsBusy = true;
    }

    /// <summary>
    /// Stops tail
    /// </summary>
    public void StopTail()
    {
      if ( _logReader == null )
        return;

      LOG.Trace("Stop tail");

      _logReader.EntryWritten -= LogReaderEntryWritten;
      _logReader = null;
      IsBusy = false;
    }

    private void LogReaderEntryWritten(object sender, EntryWrittenEventArgs e)
    {
      Index++;
      SizeRefreshTime = e.Entry.TimeWritten.ToString(SettingsHelperController.CurrentSettings.CurrentStringFormat);
      OnLogEntryCreated?.Invoke(this, new LogEntryCreatedArgs(CreateLogEntryByWindowsEvent(e.Entry), SizeRefreshTime));
    }

    /// <summary>
    /// Get <see cref="ObservableCollection{T}"/> of <see cref="WindowsEventCategory"/> with Windows events categories
    /// </summary>
    /// <returns>Task</returns>
    public async Task<ObservableCollection<WindowsEventCategory>> GetCategoriesAsync()
    {
      var result = new ObservableCollection<WindowsEventCategory>();

      await Task.Run(() =>
      {
        LOG.Trace("Get Windows event categories");

        foreach ( EventLog eventLog in EventLog.GetEventLogs(TailData.WindowsEvent.Machine) )
        {
          try
          {
            result.Add(new WindowsEventCategory
            {
              Log = eventLog.Log,
              LogDisplayName = eventLog.LogDisplayName
            });
          }
          catch
          {
            // Nothing
          }
        }
      }).ConfigureAwait(false);

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
