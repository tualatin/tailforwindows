using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Org.Vs.TailForWin.Business.SearchEngine.Controllers;
using Org.Vs.TailForWin.Business.SearchEngine.Interfaces;
using Org.Vs.TailForWin.Business.SmartWatchEngine.Interfaces;
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

    private readonly IFindController _findController;
    private readonly ManualResetEvent _resetEvent;

    /// <summary>
    /// Standard constructor
    /// </summary>
    public SmartWatchController()
    {
      _findController = new FindController();
      _resetEvent = new ManualResetEvent(false);
    }

    /// <summary>
    /// Get filename by pattern
    /// </summary>
    /// <param name="item"><see cref="TailData"/></param>
    /// <param name="pattern">Pattern string</param>
    /// <returns>New filename otherwise <see cref="string.Empty"/></returns>
    public async Task<string> GetFileNameByPatternAsync(TailData item, string pattern)
    {
      Arg.NotNull(item, nameof(item));

      if ( string.IsNullOrWhiteSpace(pattern) )
        return string.Empty;

      LOG.Info("Get filename by Pattern");

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
    public async Task<string> GetFileNameBySmartWatchAsync(TailData item)
    {
      Arg.NotNull(item, nameof(item));

      LOG.Info("Get filename by SmartWatch");

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
        string ptn = $@"^\d{"{"}{root.Length}{"}"}\D";
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
        Parallel.ForEach(validFileInfos, file =>
        {
          if ( file.LastWriteTime <= latestWriteTime )
            return;

          latestFile = file;
          latestWriteTime = file.LastWriteTime;
        });
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
        return !di.Exists ? null : di.GetFiles();
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
