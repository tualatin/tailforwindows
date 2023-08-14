using Org.Vs.TailForWin.Business.SmartWatchEngine.Events.Delegates;
using Org.Vs.TailForWin.Core.Data;

namespace Org.Vs.TailForWin.Business.SmartWatchEngine.Interfaces
{
  /// <summary>
  /// SmartWatch interface
  /// </summary>
  public interface ISmartWatchController
  {
    #region Events

    /// <summary>
    /// SmartWatch file changed event
    /// </summary>
    event SmartWatchFileChangedEventHandler SmartWatchFileChanged;

    #endregion

    /// <summary>
    /// SmartWatch is busy
    /// </summary>
    bool IsBusy
    {
      get;
    }

    /// <summary>
    /// SmartWatch is suspended
    /// </summary>
    bool IsSuspended
    {
      get;
    }

    /// <summary>
    /// Starts SmartWatch
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="ArgumentException">If item is null</exception>
    void StartSmartWatch(TailData item);

    /// <summary>
    /// Stops current SmartWatch
    /// </summary>
    void StopSmartWatch();

    /// <summary>
    /// Suspend smart watch
    /// </summary>
    void SuspendSmartWatch();

    /// <summary>
    /// Get filename by pattern
    /// </summary>
    /// <param name="item"><see cref="TailData"/></param>
    /// <param name="pattern">Pattern string</param>
    /// <returns>New filename otherwise <see cref="string.Empty"/></returns>
    /// <exception cref="ArgumentException">If item is null</exception>
    Task<string> GetFileNameByPatternAsync(TailData item, string pattern);

    /// <summary>
    /// Get filename by SmartWatch logic
    /// </summary>
    /// <param name="item"><see cref="TailData"/></param>
    /// <returns>New filename otherwise <see cref="string.Empty"/></returns>
    /// <exception cref="ArgumentException">If item is null</exception>
    Task<string> GetFileNameBySmartWatchAsync(TailData item);
  }
}
