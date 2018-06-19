using System;
using System.Collections.Generic;
using System.IO;
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
      var files = GetFilesByDirectory(item);

      return string.Empty;
    }

    #region HelperFunctions

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
