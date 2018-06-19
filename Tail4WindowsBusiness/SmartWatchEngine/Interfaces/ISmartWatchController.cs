using System.Threading.Tasks;
using Org.Vs.TailForWin.Core.Data;


namespace Org.Vs.TailForWin.Business.SmartWatchEngine.Interfaces
{
  /// <summary>
  /// SmartWatch interface
  /// </summary>
  public interface ISmartWatchController
  {
    /// <summary>
    /// Get filename by pattern
    /// </summary>
    /// <param name="item"><see cref="TailData"/></param>
    /// <param name="pattern">Pattern string</param>
    /// <returns>New filename otherwise <see cref="string.Empty"/></returns>
    Task<string> GetFileNameByPatternAsync(TailData item, string pattern);

    /// <summary>
    /// Get filename by SmartWatch logic
    /// </summary>
    /// <param name="item"><see cref="TailData"/></param>
    /// <returns>New filename otherwise <see cref="string.Empty"/></returns>
    Task<string> GetFileNameBySmartWatchAsync(TailData item);
  }
}
