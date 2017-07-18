namespace Org.Vs.TailForWin.Interfaces
{
  /// <summary>
  /// Settings helper interface
  /// </summary>
  public interface ISettingsHelper
  {
    /// <summary>
    /// Read app settings
    /// </summary>
    void ReadSettings();

    /// <summary>
    /// Save app settings
    /// </summary>
    void SaveSettings();

    /// <summary>
    /// Save search dialog window position
    /// </summary>
    void SaveSearchWindowPosition();

    /// <summary>
    /// Set app settings to default parameters
    /// </summary>
    void SetToDefault();

    /// <summary>
    /// Reload all TailForWindows settings
    /// </summary>
    void ReloadSettings();
  }
}