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
    public const EUiLanguage Language = EUiLanguage.English;

    /// <summary>
    /// Default value AlwaysScrollToEnd
    /// </summary>
    public const bool AlwaysScrollToEnd = false;

    /// <summary>
    /// Default value AlwaysOnTop
    /// </summary>
    public const bool AlwaysOnTop = false;

    /// <summary>
    /// Default value RestoreWindowSize
    /// </summary>
    public const bool RestoreWindowSize = false;

    /// <summary>
    /// Default value ExitWithEscape
    /// </summary>
    public const bool ExitWithEscape = false;

    /// <summary>
    /// Default value SaveWindowPositon
    /// </summary>
    public const bool SaveWindowPosition = false;

    /// <summary>
    /// Default value WindowWidth
    /// </summary>
    public static double WindowWidth = -1;

    /// <summary>
    /// Default value WindowHeight
    /// </summary>
    public static double WindowHeight = -1;

    /// <summary>
    /// Default value WindowPosition X
    /// </summary>
    public const double WindowPositionX = -1;

    /// <summary>
    /// Default value WindowPosition Y
    /// </summary>
    public const double WindowPositionY = -1;

    /// <summary>
    /// Default value ShowLineNumbers
    /// </summary>
    public const bool ShowLineNumbers = false;

    /// <summary>
    /// Default value ShowNumberLineAtStart
    /// </summary>
    public const bool ShowNumberLineAtStart = false;

    /// <summary>
    /// Default value LinesRead
    /// </summary>
    public const int LinesRead = 10;

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
