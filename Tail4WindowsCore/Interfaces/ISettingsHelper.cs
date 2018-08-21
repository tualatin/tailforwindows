using System.Collections.Generic;
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
    /// Reads current settings
    /// </summary>
    /// <param name="cts"><see cref="CancellationTokenSource"/></param>
    /// <returns>Task</returns>
    Task ReadSettingsAsync(CancellationTokenSource cts);

    /// <summary>
    /// Writes current settings
    /// </summary>
    /// <returns>Task</returns>
    Task SaveSettingsAsync();

    /// <summary>
    /// Writes current settings
    /// </summary>
    /// <param name="cts"><see cref="CancellationTokenSource"/></param>
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
    /// <param name="cts"><see cref="CancellationTokenSource"/></param>
    /// <returns>Task</returns>
    Task SetDefaultSettingsAsync(CancellationTokenSource cts);

    /// <summary>
    /// Reset current color settings
    /// </summary>
    /// <param name="cts"><see cref="CancellationTokenSource"/></param>
    /// <returns>Task</returns>
    Task SetDefaultColorsAsync(CancellationTokenSource cts);

    /// <summary>
    /// Reloads current settings
    /// </summary>
    /// <param name="cts"><see cref="CancellationTokenSource"/></param>
    /// <returns>Task</returns>
    Task ReloadCurrentSettingsAsync(CancellationTokenSource cts);

    /// <summary>
    /// Adds a new property to config file
    /// </summary>
    /// <param name="newSettings">List of configuration pair</param>
    /// <param name="cts"><see cref="CancellationTokenSource"/></param>
    /// <returns>Task</returns>
    Task AddNewPropertyAsync(Dictionary<string, string> newSettings, CancellationTokenSource cts);
  }
}
