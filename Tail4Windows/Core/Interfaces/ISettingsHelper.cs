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
    Task ReadSettingsAsync();

    /// <summary>
    /// Writes current settings
    /// </summary>
    Task SaveSettingsAsync();

    /// <summary>
    /// Reset current settings
    /// </summary>
    Task SetDefaultSettingsAsync();

    /// <summary>
    /// Reloads current settings
    /// </summary>
    Task ReloadCurrentSettingsAsync();
  }
}
