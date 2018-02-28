using System.Threading;
using System.Windows;
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
    /// Default value CurrentWindowState
    /// </summary>
    public const WindowState CurrentWindowState = WindowState.Normal;

    /// <summary>
    /// Default value CurrentWindowStyle
    /// </summary>
    public const EWindowStyle CurrentWindowStyle = EWindowStyle.ModernBlueWindowStyle;

    /// <summary>
    /// Default value DeleteLogFiles
    /// </summary>
    public const bool DeleteLogFiles = true;

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
    public const double WindowWidth = -1;

    /// <summary>
    /// Default value WindowHeight
    /// </summary>
    public const double WindowHeight = -1;

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

    /// <summary>
    /// Default value GroupByCategory
    /// </summary>
    public const bool GroupByCategory = true;

    /// <summary>
    /// Default value AutoUpdate
    /// </summary>
    public const bool AutoUpdate = false;

    /// <summary>
    /// Default value DefaultRefreshRate
    /// </summary>
    public const ETailRefreshRate DefaultRefreshRate = ETailRefreshRate.Normal;

    /// <summary>
    /// Default value DefaultThreadPriority
    /// </summary>
    public const ThreadPriority DefaultThreadPriority = ThreadPriority.Normal;

    /// <summary>
    /// Default value DefaultTimeFormat
    /// </summary>
    public const ETimeFormat DefaultTimeFormat = ETimeFormat.HHMMD;

    /// <summary>
    /// Default value DefaultDateFormat
    /// </summary>
    public const EDateFormat DefaultDateFormat = EDateFormat.DDMMYYYY;

    /// <summary>
    /// Default value DefaultFileSort
    /// </summary>
    public const EFileSort DefaultFileSort = EFileSort.FileCreationTime;

    /// <summary>
    /// Default value LogLineLimit
    /// </summary>
    public const int LogLineLimit = -1;

    /// <summary>
    /// Default value SmartWatch
    /// </summary>
    public const bool SmartWatch = false;

    /// <summary>
    /// Default value Statistics
    /// </summary>
    public const bool Statistics = false;

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

    #region Log viewer default settings

    /// <summary>
    /// Default value foreground color
    /// </summary>
    public const string ForegroundColor = "#000000";

    /// <summary>
    /// Default value background color
    /// </summary>
    public const string BackgroundColor = "#FFFFFF";

    /// <summary>
    /// Default value inactive foreground color
    /// </summary>
    public const string InactiveForegroundColor = "#000000";

    /// <summary>
    /// Default value inactive background color
    /// </summary>
    public const string InactiveBackgroundColor = "#FFFCFAF5";

    /// <summary>
    /// Default value highlight foreground color
    /// </summary>
    public const string HighlightForegroundColor = "#000000";

    /// <summary>
    /// Default value highlight background color
    /// </summary>
    public const string HighlightBackgroundColor = "#FFCC00";

    /// <summary>
    /// Default value line number color
    /// </summary>
    public const string LineNumberColor = "#808080";

    /// <summary>
    /// Default value highlight line number color
    /// </summary>
    public const string HighlightLineNumberColor = "#FF0000FF";

    #endregion

    #region ProxySettings

    /// <summary>
    /// Default value proxy use system settings
    /// </summary>
    public const bool ProxyUseSystemSettings = true;

    /// <summary>
    /// Default value proxy port
    /// </summary>
    public const int ProxyPort = 8080;

    /// <summary>
    /// Default value proxy URL
    /// </summary>
    public const string ProxyUrl = "";

    /// <summary>
    /// Default value proxy Username
    /// </summary>
    public const string ProxyUserName = "";

    /// <summary>
    /// Default value proxy password
    /// </summary>
    public const string ProxyPassword = "";


    #endregion
  }
}
