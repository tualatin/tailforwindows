using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
    private readonly int _startOffset;
    private FileInfo _lastFileInfo;
    private long _fileOffset;

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
      if ( _tailBackgroundWorker.IsBusy )
        return;

      LOG.Info("Start tail...");

      Thread.CurrentThread.Priority = TailData.ThreadPriority;

      _tailBackgroundWorker.RunWorkerAsync();
      _resetEvent?.Reset();
      SmartWatch.StartSmartWatch(TailData);
    }

    /// <summary>
    /// Stops tail
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

    /// <summary>
    /// Get <see cref="ObservableCollection{T}"/> of <see cref="WindowsEventCategory"/> with Windows events categories
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>Task</returns>
    public Task<ObservableCollection<WindowsEventCategory>> GetCategoriesAsync(CancellationToken token) => throw new NotImplementedException();

    private void LogReaderServiceDoWork(object sender, DoWorkEventArgs e)
    {

#if DEBUG
      if ( SettingsHelperController.CurrentSettings.DebugTailReader )
        SimulateTailReading(e);
#endif

      if ( SettingsHelperController.CurrentSettings.DebugTailReader )
        return;

      if ( SettingsHelperController.CurrentSettings.ShowNumberLineAtStart && Index == 0 )
        RewindLinesInFile();

      _lastFileInfo = new FileInfo(TailData.FileName);

      while ( _tailBackgroundWorker != null && !_tailBackgroundWorker.CancellationPending )
      {
        if ( _tailBackgroundWorker.CancellationPending )
          break;

        var fileInfo = new FileInfo(TailData.FileName);

        if ( !fileInfo.Exists )
        {
          _resetEvent?.WaitOne((int) TailData.RefreshRate);
          continue;
        }

        if ( fileInfo.Length != _lastFileInfo.Length || fileInfo.LastWriteTimeUtc != _lastFileInfo.LastWriteTimeUtc )
        {
          if ( _tailBackgroundWorker.CancellationPending )
            break;

          ReadFileLines();

          if ( _tailBackgroundWorker.CancellationPending )
            break;

          _lastFileInfo = fileInfo;
        }

        _resetEvent?.WaitOne((int) TailData.RefreshRate);
      }

      e.Cancel = true;
    }

    private void ReadFileLines()
    {
      try
      {
        using ( var fs = new FileStream(TailData.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite) )
        {
          using ( var sr = new StreamReader(fs, TailData.FileEncoding) )
          {
            // file is suddenly empty
            if ( sr.BaseStream.Length < _fileOffset )
              _fileOffset = sr.BaseStream.Length;

            sr.BaseStream.Seek(_fileOffset, SeekOrigin.Begin);
            ReadFile(sr);

            // update the last offset
            _fileOffset = sr.BaseStream.Position;

            sr.Close();
          }

          fs.Close();
        }
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    private void RewindLinesInFile()
    {
      LOG.Debug($"Rewind {_startOffset} lines in file");

      try
      {
        using ( var fs = new FileStream(TailData.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite) )
        {
          using ( var sr = new StreamReader(fs, TailData.FileEncoding) )
          {
            // Scroll to end of file
            sr.BaseStream.Seek(0, SeekOrigin.End);
            var linesRead = 0;

            while ( linesRead < _startOffset + 1 && sr.BaseStream.Position > 0 )
            {
              if ( _tailBackgroundWorker.CancellationPending )
                break;

              sr.BaseStream.Position--;
              int c = sr.BaseStream.ReadByte();

              if ( sr.BaseStream.Position > 0 )
                sr.BaseStream.Position--;
              if ( c == Convert.ToInt32('\n') )
                linesRead++;
            }

            ReadFile(sr);
            _fileOffset = sr.BaseStream.Length;

            sr.Close();
          }

          fs.Close();
        }
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    private void ReadFile(StreamReader sr)
    {
      string line;

      while ( (line = sr.ReadLine()) != null )
      {
        if ( _tailBackgroundWorker.CancellationPending )
          break;

        if ( TailData.RemoveSpace || Index == 0 )
        {
          if ( !string.IsNullOrWhiteSpace(line) )
            SendLogEntryEvent(line, sr.BaseStream.Length);
        }
        else
        {
          SendLogEntryEvent(line, sr.BaseStream.Length);
        }
      }
    }

    private void SendLogEntryEvent(string line, long fileSize)
    {
      Index++;
      var entry = new LogEntry
      {
        Index = Index,
        Message = line,
        DateTime = DateTime.Now
      };
      string message = Application.Current.TryFindResource("SizeRefreshTime").ToString();
      SizeRefreshTime = string.Format(message, (fileSize / 1024d).ToString("#,0.000", SettingsHelperController.CurrentSettings.CurrentCultureInfo),
        DateTime.Now.ToString(SettingsHelperController.CurrentSettings.CurrentStringFormat));
      OnLogEntryCreated?.Invoke(this, new LogEntryCreatedArgs(entry, SizeRefreshTime));
    }

#if DEBUG
    private void SimulateTailReading(CancelEventArgs e)
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
      LOG.Info("Stop finished");
      _resetEvent?.Reset();
    }
  }
}
