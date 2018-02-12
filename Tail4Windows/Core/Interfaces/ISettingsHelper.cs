using System.Threading;
using System.Threading.Tasks;


namespace Org.Vs.TailForWin.Core.Interfaces
{
  /// <summary>
  /// Settings helper interface
  /// </summary>
  public interface ISettingsHelper
  {
    /// <summary>
    /// Reads current settings
    /// </summary>
    /// <returns>Task</returns>
    Task ReadSettingsAsync();

    /// <summary>
    /// Writes current settings
    /// </summary>
    /// <returns>Task</returns>
    Task SaveSettingsAsync();

    /// <summary>
    /// Writes current settings
    /// </summary>
    /// <param name="cts">CancellationTokenSource</param>
    /// <returns>Task</returns>
    Task SaveSettingsAsync(CancellationTokenSource cts);

    /// <summary>
    /// Reset current settings
    /// </summary>
    /// <returns>Task</returns>
    Task SetDefaultSettingsAsync();

    /// <summary>
    /// Reset current settings
    /// </summary>
    /// <param name="cts">CancellationTokenSource</param>
    /// <returns>Task</returns>
    Task SetDefaultSettingsAsync(CancellationTokenSource cts);

    /// <summary>
    /// Reloads current settings
    /// </summary>
    /// <returns>Task</returns>
    Task ReloadCurrentSettingsAsync();
  }
}
