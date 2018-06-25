using System;
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

    private static readonly object MyLock = new object();
    private string _machine;
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
    public string SizeRefreshTime => string.Empty;

    /// <summary>
    /// <see cref="Core.Data.TailData"/>
    /// </summary>
    public TailData TailData
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
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
    /// <param name="machine">Name of mache</param>
    public WindowsEventReadService(string machine = ".")
    {
      _machine = machine;
      Index = 0;
    }

    /// <summary>
    /// Changes current machine name
    /// </summary>
    /// <param name="machineName">Name of machine</param>
    public void SetMachineName(string machineName) => _machine = machineName;

    /// <summary>
    /// Starts tail
    /// </summary>
    public void StartTail() => StartTail("System");

    /// <summary>
    /// Starts tail
    /// </summary>
    /// <param name="category">Category of Windows event</param>
    public void StartTail(string category)
    {
      if ( _logReader != null )
      {
        _logReader.EntryWritten -= LogReaderEntryWritten;
        _logReader = null;
      }

      LOG.Trace($"Start read Windows events by category {category}, on machine {_machine}");

      _logReader = new EventLog(category, _machine);
      _logReader.EntryWritten += LogReaderEntryWritten;

      var lastItems = new List<LogEntry>(SettingsHelperController.CurrentSettings.LinesRead);

      for ( int i = _logReader.Entries.Count - 1; i >= _logReader.Entries.Count - SettingsHelperController.CurrentSettings.LinesRead; i-- )
      {
        Index++;
        lastItems.Add(CreateLogEntryByWindowsEvent(_logReader.Entries[i]));
      }

      // Reverse the list
      lastItems.Reverse();
      lastItems.ForEach(p =>
      {
        OnLogEntryCreated?.Invoke(this, new LogEntryCreatedArgs(p, string.Empty));
      });
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

      IsBusy = false;
      _logReader.EntryWritten -= LogReaderEntryWritten;
      _logReader = null;
    }

    private void LogReaderEntryWritten(object sender, EntryWrittenEventArgs e)
    {
      Index++;
      OnLogEntryCreated?.Invoke(this, new LogEntryCreatedArgs(CreateLogEntryByWindowsEvent(e.Entry), string.Empty));
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
        lock ( MyLock )
        {
          LOG.Trace("Get Windows event categories");

          foreach ( EventLog eventLog in EventLog.GetEventLogs(string.IsNullOrWhiteSpace(_machine) ? "." : _machine) )
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
      string machineName = string.Format(Application.Current.TryFindResource("WindowsEventMachine").ToString(), e.MachineName);
      string message = string.Format(Application.Current.TryFindResource("WindowdEventMessage").ToString(), e.Message);
      var log = new LogEntry
      {
        Index = Index,
        DateTime = e.TimeWritten,
        Message = $"{category} - {machineName} *** {message}"
      };

      return log;
    }

    #endregion
  }
}
