﻿using System.Windows;
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
    /// Default value IsPortable
    /// </summary>
    public const bool IsPortable = false;

    /// <summary>
    /// Default value DeleteLogFiles
    /// </summary>
    public const bool DeleteLogFiles = true;

    /// <summary>
    /// Default value DeleteLogFilesOlderThan
    /// </summary>
    public const int DeleteLogFilesOlderThan = 5;

    /// <summary>
    /// Default value AlwaysScrollToEnd
    /// </summary>
    public const bool AlwaysScrollToEnd = true;

    /// <summary>
    /// Default value ContinuedScroll
    /// </summary>
    public const bool ContinuedScroll = false;

    /// <summary>
    /// Default value ActivateDragDropWindow
    /// </summary>
    public const bool ActivateDragDropWindow = true;

    /// <summary>
    /// Default value AlwaysOnTop
    /// </summary>
    public const bool AlwaysOnTop = false;

    /// <summary>
    /// Default value RestoreWindowSize
    /// </summary>
    public const bool RestoreWindowSize = false;

    /// <summary>
    /// Default value SingleInstance
    /// </summary>
    public const bool SingleInstance = false;

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
    public const bool ShowNumberLineAtStart = true;

    /// <summary>
    /// Default value LinesRead
    /// </summary>
    public const int LinesRead = 10;

    /// <summary>
    /// Default value GroupByCategory
    /// </summary>
    public const bool GroupByCategory = false;

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

    /// <summary>
    /// Default value MouseHover for LogWindow
    /// </summary>
    public const bool MouseHover = false;

    /// <summary>
    /// Default value clear LogWindow when the log file is cleared or deleted
    /// </summary>
    public const bool ClearLogWindowIfLogIsCleared = false;

    /// <summary>
    /// Default value SaveLogFileHistory
    /// </summary>
    public const bool SaveLogFileHistory = true;

    /// <summary>
    /// Default value HistoryMaxSize
    /// </summary>
    public const int HistoryMaxSize = 15;

    /// <summary>
    /// Default value ShowExtendedSettings
    /// </summary>
    public const bool ShowExtendedSettings = false;

    /// <summary>
    /// Default value SplitterWindowBehavior
    /// </summary>
    public const bool SplitterWindowBehavior = true;

    #endregion

    /// <summary>
    /// Default value Export format
    /// </summary>
    public const EExportFormat ExportFormat = EExportFormat.Csv;

    /// <summary>
    /// Default value Filter font color
    /// </summary>
    public const string FilterFontColor = "#DC143C";

    #region Statistic window default settings

    /// <summary>
    /// Default value StatisticWindowLeft
    /// </summary>
    public const double StatisticWindowLeft = 0;

    /// <summary>
    /// Default value StatisticWindowTop
    /// </summary>
    public const double StatisticWindowTop = 0;

    /// <summary>
    /// Default value StatisticWindowHeight
    /// </summary>
    public const double StatisticWindowHeight = 450;

    /// <summary>
    /// Default value StatisticWindowWidth
    /// </summary>
    public const double StatisticWindowWidth = 800;

    #endregion

    #region StatusBar default settings

    /// <summary>
    /// Default value StatusBarInactiveBackgroundColor
    /// </summary>
    public const string StatusBarInactiveBackgroundColor = "#FF68217A";

    /// <summary>
    /// Default value StatusBarFileLoadedBackgroundColor
    /// </summary>
    public const string StatusBarFileLoadedBackgroundColor = "#FF007ACC";

    /// <summary>
    /// Default value StatusBarTailBackgroundColor
    /// </summary>
    public const string StatusBarTailBackgroundColor = "#FFCA5100";

    #endregion

    #region TabItemHeader

    /// <summary>
    /// Default value TabItemHeaderBackgroundColor
    /// </summary>
    public const string TabItemHeaderBackgroundColor = "#FFD6DBE9";

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
    /// Default value selection background color
    /// </summary>
    public const string SelectionBackgroundColor = "#FFA4DCFF";

    /// <summary>
    /// Default value line number color
    /// </summary>
    public const string LineNumberColor = "#808080";

    /// <summary>
    /// Default value highlight line number color
    /// </summary>
    public const string HighlightLineNumberColor = "#FF0000FF";

    /// <summary>
    /// Default value search hightlight background color
    /// </summary>
    public const string SearchHighlightBackgroundColor = "#FFCC00";

    /// <summary>
    /// Default value search hightlight foreground color
    /// </summary>
    public const string SearchHighlightForegroundColor = "#000000";

    /// <summary>
    /// Default value Splitter background color
    /// </summary>
    public const string SplitterBackgroundColor = "#FF293955";

    /// <summary>
    /// Default MouseHover color
    /// </summary>
    public const string MouseHoverColor = "#A3C8DB";

    #endregion

    #region Proxy settings

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

    #region Alert settings

    /// <summary>
    /// Default value alert bring to front
    /// </summary>
    public const bool AlertBringToFront = true;

    /// <summary>
    /// Default value alert play sound file
    /// </summary>
    public const bool AlertPlaySoundFile = false;

    /// <summary>
    /// Default value alert send E-Mail
    /// </summary>
    public const bool AlertSendMail = false;

    /// <summary>
    /// Default value alert PopUp window
    /// </summary>
    public const bool AlertPopUpWindow = false;

    /// <summary>
    /// Default value E-Mail address
    /// </summary>
    public const string AlertMailAddress = "NoMail";

    /// <summary>
    /// Default value sound file
    /// </summary>
    public const string AlertSoundFile = "NoFile";

    #endregion

    #region SMTP settings

    /// <summary>
    /// Default value SMTP SSL
    /// </summary>
    public const bool SmtpSsl = true;

    /// <summary>
    /// Default value SMTP TLS
    /// </summary>
    public const bool SmtpTls = false;

    /// <summary>
    /// Default value SMTP port
    /// </summary>
    public const int SmtpPort = 587;

    /// <summary>
    /// Default value SMTP server
    /// </summary>
    public const string SmtpServer = "";

    /// <summary>
    /// Default value SMTP login name
    /// </summary>
    public const string SmtpUsername = "";

    /// <summary>
    /// Default value SMTP password
    /// </summary>
    public const string SmtpPassword = "";

    /// <summary>
    /// Default value SMTP from E-Mail address
    /// </summary>
    public const string SmtpFromMailAddress = "";

    /// <summary>
    /// Default value SMTP E-Mail subject
    /// </summary>
    public const string SmtpSubject = "";

    #endregion

    #region SmartWatch settings

    /// <summary>
    /// Default value AutoRun
    /// </summary>
    public const bool SmartWatchAutoRun = true;

    /// <summary>
    /// Default value open in new tab
    /// </summary>
    public const bool SmartWatchNewTab = true;

    /// <summary>
    /// Default value <see cref="ESmartWatchMode"/>
    /// </summary>
    public const ESmartWatchMode SmartWatchMode = ESmartWatchMode.Manual;

    /// <summary>
    /// Default value filter by extension
    /// </summary>
    public const bool SmartWatchFilterByExension = true;

    /// <summary>
    /// Default value SmartWatch interval
    /// </summary>
    public const int SmartWatchInterval = 2000;

    #endregion
  }
}
