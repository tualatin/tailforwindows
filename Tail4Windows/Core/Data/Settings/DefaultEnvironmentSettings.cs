using Org.Vs.TailForWin.Core.Enums;

namespace Org.Vs.TailForWin.Core.Data.Settings
{
  /// <summary>
  /// Default environment settings
  /// </summary>
  public static class DefaultEnvironmentSettings
  {
    #region Window default settings

    /// <summary>
    /// Default value Language;
    /// </summary>
    public static EUiLanguage Language = EUiLanguage.English;

    /// <summary>
    /// Default value Always on top
    /// </summary>
    public static bool AlwaysOnTop = false;

    /// <summary>
    /// Default value RestoreWindowSize
    /// </summary>
    public static bool RestoreWindowSize = false;

    #endregion

    #region StatusBar default settings

    /// <summary>
    /// Default value StatusBarInactiveBackgroundColor
    /// </summary>
    public const string StatusBarInactiveBackgroundColor = "#68217A";

    /// <summary>
    /// Default value StatusBarFileLoadedBackgroundColor
    /// </summary>
    public const string StatusBarFileLoadedBackgroundColor = "#007ACC";

    /// <summary>
    /// Default value StatusBarTailBackgroundColor
    /// </summary>
    public const string StatusBarTailBackgroundColor = "#CA5100";

    #endregion
  }
}
