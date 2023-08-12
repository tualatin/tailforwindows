using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using log4net;
using Org.Vs.Tail4Win.Business.Services.Data;
using Org.Vs.Tail4Win.Business.Services.Events.Args;
using Org.Vs.Tail4Win.Business.Services.Events.Delegates;
using Org.Vs.Tail4Win.Business.Services.Interfaces;
using Org.Vs.Tail4Win.Business.SmartWatchEngine.Controllers;
using Org.Vs.Tail4Win.Business.SmartWatchEngine.Interfaces;
using Org.Vs.Tail4Win.Business.StatisticEngine.Data.Messages;
using Org.Vs.Tail4Win.Business.Utils;
using Org.Vs.Tail4Win.Core.Controllers;
using Org.Vs.Tail4Win.Core.Data;
using Org.Vs.Tail4Win.Core.Data.Base;
using Org.Vs.Tail4Win.Core.Logging;
using Org.Vs.Tail4Win.Core.Utils;
using Application = System.Windows.Application;

namespace Org.Vs.Tail4Win.Business.Services
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
    private readonly Stopwatch _sw;
    private CancellationTokenSource _cts;
    private DateTime? _lastWrittenTime;

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

    /// <summary>
    /// Elapsed time
    /// </summary>
    public TimeSpan ElapsedTime => _sw.Elapsed;

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
      LogReaderId = Guid.NewGuid();
      SmartWatch = new SmartWatchController();
      _resetEvent = new ManualResetEvent(false);
      _sw = new Stopwatch();
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
      _sw.Start();

      NotifyTaskCompletion.Create(UpdateStatisticsAsync);
      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new StatisticChangeReaderMessage(LogReaderId, Index, TailData.FileName, TailData.IsWindowsEvent));
    }

    /// <summary>
    /// Stops tail
    /// </summary>
    public void StopTail()
    {
      MouseService.SetBusyState();
      LOG.Info("Stop tail.");

      _tailBackgroundWorker.CancelAsync();
      _resetEvent?.Set();
      SmartWatch.SuspendSmartWatch();
      _cts?.Cancel();
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
    /// Sets current file offset to zero
    /// </summary>
    public void SetFileOffsetZero() => _fileOffset = 0;

    /// <summary>
    /// Gets current file offset
    /// </summary>
    /// <returns>Current file offset</returns>
    public long GetFileOffset() => _fileOffset;

    /// <summary>
    /// Sets current file offset
    /// </summary>
    /// <param name="offset">Offset</param>
    public void SetFileOffset(long offset) => _fileOffset = offset;

    /// <summary>
    /// Get <see cref="ObservableCollection{T}"/> of <see cref="WindowsEventCategory"/> with Windows events categories
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>Task</returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<ObservableCollection<WindowsEventCategory>> GetCategoriesAsync(CancellationToken token) => throw new NotImplementedException();

    /// <summary>
    /// Auto save into Database if Statistics is enabled
    /// </summary>
    /// <returns><see cref="Task{TResult}"/></returns>
    private async Task UpdateStatisticsAsync()
    {
      _cts?.Dispose();
      _cts = new CancellationTokenSource();

      while ( _tailBackgroundWorker != null && !_tailBackgroundWorker.CancellationPending )
      {
        await Task.Delay(TimeSpan.FromMinutes(30), _cts.Token).ConfigureAwait(false);
        EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new StatisticUpdateReaderMessage(LogReaderId, Index, TailData.FileName, _sw.Elapsed));
      }
    }

    private void LogReaderServiceDoWork(object sender, DoWorkEventArgs e)
    {

#if DEBUG
      if ( SettingsHelperController.CurrentAppSettings.DebugTailReader )
        SimulateTailReading(e);
#endif

      if ( SettingsHelperController.CurrentAppSettings.DebugTailReader )
        return;

      try
      {
        if ( SettingsHelperController.CurrentSettings.ShowNumberLineAtStart && Index == 0 && _lastFileInfo == null )
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

            if ( SettingsHelperController.CurrentSettings.ClearLogWindowIfLogIsCleared )
              OnLogCleared?.Invoke(this, EventArgs.Empty);
            continue;
          }

          if ( fileInfo.Length != _lastFileInfo.Length || fileInfo.LastWriteTimeUtc != _lastFileInfo.LastWriteTimeUtc || fileInfo.Length > 0 && _fileOffset == 0 )
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
      catch ( InvalidOperationException ex )
      {
        InteractionService.ShowErrorMessageBox(ex.Message);
      }
    }

    private void ReadFileLines()
    {
      try
      {
        if ( IsInvalidChars(TailData.FileName) )
          throw new InvalidOperationException("Invalid characters found in path or file name.");

        using ( var fs = new FileStream(TailData.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite) )
        {
          using ( var sr = new StreamReader(fs, TailData.FileEncoding) )
          {
            // file is suddenly empty
            if ( sr.BaseStream.Length < _fileOffset )
            {
              _fileOffset = sr.BaseStream.Length;

              if ( SettingsHelperController.CurrentSettings.ClearLogWindowIfLogIsCleared )
                OnLogCleared?.Invoke(this, EventArgs.Empty);
            }

            sr.BaseStream.Seek(_fileOffset, SeekOrigin.Begin);
            ReadFile(sr);

            // update the last offset
            _fileOffset = sr.BaseStream.Position;

            sr.Close();
          }

          fs.Close();
        }
      }
      catch ( InvalidOperationException )
      {
        throw;
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
      }
    }

    private void RewindLinesInFile()
    {
      LOG.Debug($"Rewind {_startOffset} lines in file");

      try
      {
        if ( IsInvalidChars(TailData.FileName) )
          throw new InvalidOperationException("Invalid characters found in path or file name.");

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
      catch ( InvalidOperationException )
      {
        throw;
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
      }
    }

    private void ReadFile(StreamReader sr)
    {
      string line;
      var entries = new List<LogEntry>();

      while ( (line = sr.ReadLine()) != null )
      {
        if ( _tailBackgroundWorker.CancellationPending )
          break;

        if ( TailData.RemoveSpace || Index == 0 )
        {
          if ( string.IsNullOrWhiteSpace(line) )
            continue;

          if ( SettingsHelperController.CurrentSettings.ContinuedScroll )
            SendLogEntryEvent(CreateLogEntry(line), sr.BaseStream.Length);
          else
            entries.Add(CreateLogEntry(line));
        }
        else
        {
          if ( SettingsHelperController.CurrentSettings.ContinuedScroll )
            SendLogEntryEvent(CreateLogEntry(line), sr.BaseStream.Length);
          else
            entries.Add(CreateLogEntry(line));
        }
      }

      if ( !SettingsHelperController.CurrentSettings.ContinuedScroll )
        SendLogEntryEvent(entries, sr.BaseStream.Length);
    }

    private static bool IsInvalidChars(string fileName)
    {
      var invalidFileNameChars = Path.GetInvalidFileNameChars();
      var invalidPathChars = Path.GetInvalidPathChars();
      string path = Path.GetFullPath(fileName);

      return fileName.IndexOfAny(invalidFileNameChars) < 0 && path.IndexOfAny(invalidPathChars) < 0;
    }

    private LogEntry CreateLogEntry(string line)
    {
      Index++;
      var dateTime = DateTime.Now;
      var entry = new LogEntry
      {
        Index = Index,
        Message = line,
        DateTime = dateTime,
        TimeDelta = dateTime - _lastWrittenTime
      };
      _lastWrittenTime = dateTime;

      return entry;
    }

    private void SendLogEntryEvent(LogEntry entry, long fileSize)
    {
      string message = Application.Current.TryFindResource("SizeRefreshTime").ToString();

      if ( string.IsNullOrWhiteSpace(message) )
        return;

      FileSizeTotalEvents = fileSize / 1024d;
      SizeRefreshTime = string.Format(message, FileSizeTotalEvents.ToString("#,0.000", SettingsHelperController.CurrentAppSettings.CurrentCultureInfo),
        DateTime.Now.ToString(SettingsHelperController.CurrentSettings.CurrentStringFormat));

      OnLogEntryCreated?.Invoke(this, new LogEntryCreatedArgs(new List<LogEntry> { entry }, SizeRefreshTime));
    }

    private void SendLogEntryEvent(List<LogEntry> entries, long fileSize)
    {
      string message = Application.Current.TryFindResource("SizeRefreshTime").ToString();

      if ( string.IsNullOrWhiteSpace(message) )
        return;

      FileSizeTotalEvents = fileSize / 1024d;
      SizeRefreshTime = string.Format(message, FileSizeTotalEvents.ToString("#,0.000", SettingsHelperController.CurrentAppSettings.CurrentCultureInfo),
        DateTime.Now.ToString(SettingsHelperController.CurrentSettings.CurrentStringFormat));

      OnLogEntryCreated?.Invoke(this, new LogEntryCreatedArgs(entries, SizeRefreshTime));
    }

#if DEBUG
    private void SimulateTailReading(CancelEventArgs e)
    {
      string message = Application.Current.TryFindResource("SizeRefreshTime").ToString();

      if ( string.IsNullOrWhiteSpace(message) )
        return;

      while ( _tailBackgroundWorker != null && !_tailBackgroundWorker.CancellationPending )
      {
        if ( _tailBackgroundWorker.CancellationPending )
          break;

        Index++;
        int mod = Index % 2;
        LogEntry log = mod == 0
          ? new LogEntry
          {
            Index = Index,
            Message = $"Log - {Index * 24} / Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.",
            DateTime = DateTime.Now
          }
          : new LogEntry
          {
            Index = Index,
            Message = "Christabel strips and slips like a dream breaking ice with arms that gleam with pain disdain... She throws her head and glides against the stream throwing me her bravest smile defiant glittering shivering guile",
            DateTime = DateTime.Now
          };

        SizeRefreshTime = string.Format(message, $"{24 + Index * 12}", DateTime.Now.ToString(SettingsHelperController.CurrentSettings.CurrentStringFormat));

        if ( _tailBackgroundWorker.CancellationPending )
          break;

        OnLogEntryCreated?.Invoke(this, new LogEntryCreatedArgs(new List<LogEntry> { log }, SizeRefreshTime));
        _resetEvent?.WaitOne((int) TailData.RefreshRate);
      }

      e.Cancel = true;
    }
#endif

    private void LogReaderServiceRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      _sw.Stop();
      _resetEvent?.Reset();

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new StatisticUpdateReaderMessage(LogReaderId, Index, TailData.FileName, _sw.Elapsed));
      LOG.Info($"Stop finished, tail was running about {_sw.ElapsedMilliseconds:N0} ms");
    }
  }
}
