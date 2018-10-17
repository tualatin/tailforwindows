using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Org.Vs.TailForWin.Business.Services.Data;


namespace Org.Vs.TailForWin.Business.Utils.Interfaces
{
  /// <summary>
  /// CacheManager interface
  /// </summary>
  public interface ICacheManager : IEnumerable, IEnumerator
  {
    /// <summary>
    /// Gets <see cref="LogEntry"/> by index
    /// </summary>
    /// <param name="index">Index</param>
    /// <returns><see cref="LogEntry"/></returns>
    /// <exception cref="InvalidOperationException"></exception> if index is not in range
    LogEntry this[int index]
    {
      get;
    }

    /// <summary>
    /// Gets current cache data
    /// </summary>
    /// <returns><see cref="List{T}"/> of <see cref="LogEntry"/></returns>
    List<LogEntry> GetCacheData();

    /// <summary>
    /// Adds <see cref="List{T}"/> of <see cref="LogEntry"/> to CacheData
    /// </summary>
    /// <param name="data"><see cref="List{T}"/> of <see cref="LogEntry"/></param>
    void AddToCacheData(List<LogEntry> data);

    /// <summary>
    /// Sets new cache data
    /// </summary>
    /// <param name="data"><see cref="IEnumerable{T}"/> of <see cref="LogEntry"/></param>
    void SetCacheData(List<LogEntry> data);

    /// <summary>
    /// Clears current cache data
    /// </summary>
    void ClearCacheData();

    /// <summary>
    /// Print current cache size (for logging only)
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns><see cref="Task"/></returns>
    Task PrintCacheSizeAsync(CancellationToken token);

    /// <summary>
    /// Produces the set intersection of two sequences by using the default equality comparer to compare values.
    /// </summary>
    /// <param name="other">An <see cref="IEnumerable{T}" /> whose distinct elements that also appear in the first sequence will be returned.</param>
    /// <returns>A sequence that contains the elements that form the set intersection of two sequences.</returns>
    IEnumerable<LogEntry> GetIntersectData(IEnumerable<LogEntry> other);

    /// <summary>
    /// Setup cache data
    /// </summary>
    /// <param name="count">Count of items</param>
    /// <param name="entry"><see cref="LogEntry"/></param>
    /// <param name="position">Position of <see cref="double"/></param>
    void SetupCache(int count, LogEntry entry, double position);

    /// <summary>
    /// Have to build the internal cache
    /// </summary>
    /// <param name="count">Count of items</param>
    /// <returns><c>True</c> if cache is necessary, otherwise <c>False</c></returns>
    bool HaveToCache(int count);

    /// <summary>
    /// Fix current cache size
    /// </summary>
    /// <param name="count">Count of items</param>
    void FixCacheSize(int count);
  }
}
