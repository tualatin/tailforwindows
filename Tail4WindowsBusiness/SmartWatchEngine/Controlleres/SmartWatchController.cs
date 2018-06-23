using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Org.Vs.TailForWin.Business.SearchEngine.Controllers;
using Org.Vs.TailForWin.Business.SearchEngine.Interfaces;
using Org.Vs.TailForWin.Business.SmartWatchEngine.Events.Delegates;
using Org.Vs.TailForWin.Business.SmartWatchEngine.Interfaces;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Business.SmartWatchEngine.Controlleres
{
  /// <summary>
  /// SmartWatch controller
  /// </summary>
  public class SmartWatchController : ISmartWatchController
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(SmartWatchController));

    #region Events

    /// <summary>
    /// SmartWatch file changed event
    /// </summary>
    public event SmartWatchFileChangedEventHandler SmartWatchFileChanged;

    #endregion

    private readonly IFindController _findController;
    private readonly ManualResetEvent _resetEvent;
    private readonly ManualResetEvent _waitResetEvent;
    private BackgroundWorker _smartWatchWorker;
    private List<FileInfo> _currentFiles;

    #region Properties

    /// <summary>
    /// SmartWatch is busy
    /// </summary>
    public bool IsBusy => _smartWatchWorker != null && _smartWatchWorker.IsBusy;

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public SmartWatchController()
    {
      _findController = new FindController();
      _resetEvent = new ManualResetEvent(false);
      _waitResetEvent = new ManualResetEvent(false);
    }

    #region SmartWatch worker

    /// <summary>
    /// Resume smart watch
    /// </summary>
    private void ResumeSmartWatch()
    {
      if ( !_smartWatchWorker.IsBusy )
        return;

      LOG.Trace("Resume SmartWatch");

      _resetEvent?.Set();
      _waitResetEvent?.Reset();
    }

    private void SmartWatchWorkerDoWork(object sender, DoWorkEventArgs e)
    {
      if ( !(e.Argument is TailData data) )
        return;

      while ( _smartWatchWorker != null && !_smartWatchWorker.CancellationPending )
      {
        _waitResetEvent?.WaitOne(TimeSpan.FromMilliseconds(SettingsHelperController.CurrentSettings.SmartWatchSettings.SmartWatchInterval));
        _resetEvent?.WaitOne();

        if ( _smartWatchWorker.CancellationPending )
          break;

        var dirFiles = GetFilesByDirectory(data);

        if ( dirFiles == null )
          continue;

        var files = dirFiles as List<FileInfo> ?? dirFiles.ToList();

        if ( _currentFiles.Count < files.Count )
        {
          if ( _smartWatchWorker.CancellationPending )
            break;

          LOG.Trace("SmartWatch files changed! Current {0} new {1}", _currentFiles.Count, files.Count);

          foreach ( FileInfo file in files )
          {
            if ( _smartWatchWorker.CancellationPending )
              break;

            if ( _currentFiles.Any(p => p.FullName == file.FullName) )
              continue;

            if ( data.UsePattern )
            {
              string match = GetFileNameByPatternAsync(data, data.PatternString).GetAwaiter().GetResult();
              NewFileDetected(match, file.FullName);
            }
            else
            {
              string match = GetFileNameBySmartWatchAsync(data).GetAwaiter().GetResult();
              NewFileDetected(match, file.FullName);
            }
          }

          _currentFiles = files.ToList();
        }
        else if ( _currentFiles.Count > files.Count )
        {
          if ( _smartWatchWorker.CancellationPending )
            break;

          LOG.Trace("SmartWatch some files deleted! Current {0} new {1}", _currentFiles.Count, files.Count);
          _currentFiles = files;
        }
      }

      e.Cancel = true;
    }

    private void NewFileDetected(string match, string fileName)
    {
      if ( !Equals(match, fileName) )
        return;

      LOG.Info("SmartWatch file changed...");
      SmartWatchFileChanged?.Invoke(this, match);
    }

    private void SmartWatchWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      LOG.Trace("Stop finished");

      _resetEvent?.Close();
      _waitResetEvent?.Close();
    }

    #endregion

    /// <summary>
    /// Starts SmartWatch
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="ArgumentException">If item is null</exception>
    public void StartSmartWatch(TailData item)
    {
      Arg.NotNull(item, nameof(item));

      if ( !SettingsHelperController.CurrentSettings.SmartWatch || !item.SmartWatch )
        return;

      if ( _smartWatchWorker == null )
      {
        _smartWatchWorker = new BackgroundWorker
        {
          WorkerSupportsCancellation = true
        };
        _smartWatchWorker.DoWork += SmartWatchWorkerDoWork;
        _smartWatchWorker.RunWorkerCompleted += SmartWatchWorkerRunWorkerCompleted;
      }

      var files = GetFilesByDirectory(item);
      _currentFiles = files as List<FileInfo> ?? files.ToList();

      if ( _smartWatchWorker.IsBusy )
      {
        ResumeSmartWatch();
        return;
      }

      LOG.Trace("Start SmartWatch");
      LOG.Trace($"SmartWatch interval is {SettingsHelperController.CurrentSettings.SmartWatchSettings.SmartWatchInterval} ms");

      _smartWatchWorker.RunWorkerAsync(item);
      _resetEvent?.Set();
    }

    /// <summary>
    /// Suspend smart watch
    /// </summary>
    public void SuspendSmartWatch()
    {
      if ( _smartWatchWorker == null )
        return;

      if ( !_smartWatchWorker.IsBusy )
        return;

      LOG.Trace("Suspend SmartWatch");
      _resetEvent?.Reset();
      _waitResetEvent?.Set();
    }

    /// <summary>
    /// Stops current SmartWatch
    /// </summary>
    public void StopSmartWatch()
    {
      if ( _smartWatchWorker == null )
        return;

      if ( !_smartWatchWorker.IsBusy )
        return;

      _resetEvent.Set();
      _smartWatchWorker.CancelAsync();
    }

    /// <summary>
    /// Get filename by pattern
    /// </summary>
    /// <param name="item"><see cref="TailData"/></param>
    /// <param name="pattern">Pattern string</param>
    /// <returns>New filename otherwise <see cref="string.Empty"/></returns>
    /// <exception cref="ArgumentException">If item is null</exception>
    public async Task<string> GetFileNameByPatternAsync(TailData item, string pattern)
    {
      Arg.NotNull(item, nameof(item));

      LOG.Info("SmartWatch get file name by pattern");

      if ( string.IsNullOrWhiteSpace(pattern) )
        return string.Empty;

      var files = GetFilesByDirectory(item);
      List<FileInfo> validFileInfos = new List<FileInfo>();

      foreach ( var file in files )
      {
        var result = await _findController.MatchTextAsync(item.FindSettings, file.Name, pattern).ConfigureAwait(false);

        if ( result == null || result.Count == 0 )
          continue;

        validFileInfos.Add(file);
      }
      return validFileInfos.Count == 0 ? string.Empty : await GetLatestFileNameAsync(validFileInfos).ConfigureAwait(false);
    }

    /// <summary>
    /// Get filename by SmartWatch logic
    /// </summary>
    /// <param name="item"><see cref="TailData"/></param>
    /// <returns>New filename otherwise <see cref="string.Empty"/></returns>
    /// <exception cref="ArgumentException">If item is null</exception>
    public async Task<string> GetFileNameBySmartWatchAsync(TailData item)
    {
      Arg.NotNull(item, nameof(item));

      LOG.Info("SmartWatch get file name by internal logic");

      List<FileInfo> validFileInfos = new List<FileInfo>();
      var files = GetFilesByDirectory(item);
      var fileInfos = files as FileInfo[] ?? files.ToArray();

      if ( !fileInfos.Any() )
        return string.Empty;

      var root = GetRootString(item.File, out bool isNumeric);
      var pattern = CreatePattern(item.File, string.Empty);

      if ( !isNumeric )
      {
        fileInfos = fileInfos.Where(p => p.Extension == Path.GetExtension(item.FileName) && p.Name.StartsWith(root)).ToArray();
      }
      else
      {
        string ptn = $@"^\d{"{"}{root.Length},{root.Length}{"}"}\D";
        var regex = new Regex(ptn);
        fileInfos = fileInfos.Where(p => p.Extension == Path.GetExtension(item.FileName) && regex.IsMatch(p.Name)).ToArray();
      }

      foreach ( var file in fileInfos )
      {
        var result = await _findController.MatchTextAsync(new FindData { UseRegex = true, WholeWord = true }, file.Name, pattern).ConfigureAwait(false);

        if ( result == null || result.Count == 0 )
          continue;

        validFileInfos.Add(file);
      }
      return validFileInfos.Count == 0 ? string.Empty : await GetLatestFileNameAsync(validFileInfos).ConfigureAwait(false);
    }

    #region HelperFunctions

    private string CreatePattern(string text, string pattern)
    {
      var root = GetRootString(text, out bool isNumeric);
      var temp = isNumeric ? $@"^\d{"{"}{root.Length}{"}"}" : root;

      if ( !string.IsNullOrWhiteSpace(pattern) )
      {
        if ( temp.StartsWith("^") )
          temp = temp.Remove(0, 1);
      }

      pattern += temp;
      return root.Length < text.Length ? CreatePattern(text.Substring(root.Length, text.Length - root.Length), pattern) : pattern;
    }

    private string GetRootString(string text, out bool isNumeric)
    {
      string root = string.Empty;
      isNumeric = false;

      var regex = new Regex(@"^\d+", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
      var match = regex.Match(text);

      if ( match.Success )
      {
        isNumeric = true;
        return match.Value;
      }

      regex = new Regex(@"^[a-zA-Z]+", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
      match = regex.Match(text);

      if ( match.Success )
        return match.Value;

      regex = new Regex(@"^(\D|[^a-zA-Z])", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
      match = regex.Match(text);

      if ( match.Success )
        root = match.Value;

      return root;
    }

    private async Task<string> GetLatestFileNameAsync(IEnumerable<FileInfo> validFileInfos)
    {
      FileInfo latestFile = null;

      await Task.Run(() =>
      {
        DateTime latestWriteTime = DateTime.MinValue;

        foreach ( FileInfo file in validFileInfos )
        {
          if ( file.LastWriteTime <= latestWriteTime )
            continue;

          latestFile = file;
          latestWriteTime = file.LastWriteTime;
        }
      }).ConfigureAwait(false);

      return latestFile?.FullName;
    }

    private IEnumerable<FileInfo> GetFilesByDirectory(TailData item)
    {
      Arg.NotNull(item, nameof(item));

      try
      {
        var path = Path.GetDirectoryName(item.FileName);

        if ( string.IsNullOrWhiteSpace(path) )
          return null;

        DirectoryInfo di = new DirectoryInfo(path);
        return !di.Exists ? null : di.GetFiles(SettingsHelperController.CurrentSettings.SmartWatchSettings.FilterByExtension ? $"*{Path.GetExtension(item.FileName)}" : "*.*", SearchOption.TopDirectoryOnly);
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
      return null;
    }

    #endregion
  }
}
