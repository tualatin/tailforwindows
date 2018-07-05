using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Org.Vs.TailForWin.Business.Services.Data;
using Org.Vs.TailForWin.Business.Utils.Interfaces;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Business.Utils
{
  /// <summary>
  /// CacheManager
  /// </summary>
  public class CacheManager : ICacheManager
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(CacheManager));

    /// <summary>
    /// Max capacity
    /// </summary>
    private const int MaxCapacity = 10000;

    private readonly List<LogEntry> _cacheData;


    /// <summary>
    /// Standard constructor
    /// </summary>
    public CacheManager()
    {
      _cacheData = new List<LogEntry>();
      LOG.Debug($"Current max logline capacity is {MaxCapacity}");
    }

    /// <summary>
    /// Print current cache size (for logging only)
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns><see cref="Task"/></returns>
    public async Task PrintCacheSizeAsync(CancellationToken token)
    {
      while ( !token.IsCancellationRequested )
      {
        await Task.Delay(TimeSpan.FromMinutes(30), token).ConfigureAwait(false);
        LOG.Trace($"Current cache size is {_cacheData.Count}");
      }
    }

    /// <summary>
    /// Gets current cache data
    /// </summary>
    /// <returns><see cref="List{T}"/> of <see cref="LogEntry"/></returns>
    public List<LogEntry> GetCacheData() => _cacheData;

    /// <summary>
    /// Sets new cache data
    /// </summary>
    /// <param name="data"><see cref="List{T}"/> of <see cref="LogEntry"/></param>
    public void SetCacheData(List<LogEntry> data)
    {
      if ( data == null || data.Count == 0 )
        return;

      _cacheData.AddRange(data);
    }

    /// <summary>
    /// Clears current cache data
    /// </summary>
    public void ClearCacheData() => _cacheData.Clear();

    /// <summary>
    /// Have to build the internal cache
    /// </summary>
    /// <param name="count">Count of items</param>
    /// <returns><c>True</c> if cache is necessary, otherwise <c>False</c></returns>
    public bool HaveToCache(int count) => count > MaxCapacity;

    /// <summary>
    /// Setup cache data
    /// </summary>
    /// <param name="count">Count of items</param>
    /// <param name="entry"><see cref="LogEntry"/></param>
    /// <param name="position">Position of <see cref="double"/></param>
    public void SetupCache(int count, LogEntry entry, double position)
    {
      if ( count <= MaxCapacity )
        return;

      if ( _cacheData.Count == 0 )
        LOG.Debug("Start caching...");

      if ( SettingsHelperController.CurrentSettings.LogLineLimit != -1 && _cacheData.Count + MaxCapacity >= SettingsHelperController.CurrentSettings.LogLineLimit
                                                                       && position <= 0 )
      {
        _cacheData.RemoveAt(0);
      }

      entry.IsCacheData = true;
      _cacheData.Add(entry);
    }

    /// <summary>
    /// Fix current cache size
    /// </summary>
    /// <param name="count">Count of items</param>
    public void FixCacheSize(int count)
    {
      if ( count == MaxCapacity )
      {
        _cacheData.Clear();
        return;
      }

      int index = SettingsHelperController.CurrentSettings.LogLineLimit - count;

      for ( var i = 0; i < index; i++ )
      {
        _cacheData.RemoveAt(i);
      }
    }

    /// <summary>
    /// Produces the set intersection of two sequences by using the default equality comparer to compare values.
    /// </summary>
    /// <param name="other">An <see cref="IEnumerable{T}" /> whose distinct elements that also appear in the first sequence will be returned.</param>
    /// <returns>A sequence that contains the elements that form the set intersection of two sequences.</returns>
    /// <exception cref="ArgumentException">If <c>other</c> is null</exception>
    public IEnumerable<LogEntry> GetIntersectData(IEnumerable<LogEntry> other)
    {
      // ReSharper disable once PossibleMultipleEnumeration
      Arg.NotNull(other, nameof(other));
      // ReSharper disable once PossibleMultipleEnumeration
      return _cacheData.Intersect(other);
    }
  }
}
