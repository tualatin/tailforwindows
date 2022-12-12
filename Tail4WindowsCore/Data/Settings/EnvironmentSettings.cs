using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Extensions;


namespace Org.Vs.TailForWin.Core.Data.Settings
{
  /// <summary>
  /// Environment settings object
  /// </summary>
  public partial class EnvironmentSettings : NotifyMaster
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public EnvironmentSettings()
    {
      ProxySettings = new ProxySetting();
      AlertSettings = new AlertSetting();
      SmartWatchSettings = new SmartWatchSetting();
      ColorSettings = new EnvironmentColorSettings();
      SmtpSettings = new SmtpSetting();
    }

    /// <summary>
    /// Current date format
    /// </summary>
    [JsonIgnore]
    public string CurrentDateFormat
    {
      get
      {
        string currentFormat = string.Empty;

        switch ( DefaultTimeFormat )
        {
        case ETimeFormat.HHMMd:
        case ETimeFormat.HHMMD:

          currentFormat = $"{DefaultDateFormat.GetEnumDescription()}";
          break;

        case ETimeFormat.HHMMSSd:
        case ETimeFormat.HHMMSSD:

          currentFormat = $"{DefaultDateFormat.GetEnumDescription()}";
          break;
        }
        return currentFormat;
      }
    }

    /// <summary>
    /// Current string format
    /// </summary>
    [JsonIgnore]
    public string CurrentStringFormat
    {
      get
      {
        string currentFormat = string.Empty;

        switch ( DefaultTimeFormat )
        {
        case ETimeFormat.HHMMd:
        case ETimeFormat.HHMMD:

          currentFormat = $"{DefaultDateFormat.GetEnumDescription()} {DefaultTimeFormat.GetEnumDescription()}";
          break;

        case ETimeFormat.HHMMSSd:
        case ETimeFormat.HHMMSSD:

          currentFormat = $"{DefaultDateFormat.GetEnumDescription()} {DefaultTimeFormat.GetEnumDescription()}.fff";
          break;
        }
        return currentFormat;
      }
    }

    private EUiLanguage _language;

    /// <summary>
    /// Current UI language
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.Language)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public EUiLanguage Language
    {
      get => _language;
      set
      {
        if ( value == _language )
          return;

        _language = value;
        OnPropertyChanged();
      }
    }

    private string _editorPath;

    /// <summary>
    /// Editor path
    /// </summary>
    [DefaultValue("")]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public string EditorPath
    {
      get => _editorPath;
      set
      {
        if ( Equals(value, _editorPath) )
          return;

        _editorPath = value;
        OnPropertyChanged();
      }
    }

    private EExportFormat _exportFormat;

    /// <summary>
    /// Last used export format
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.ExportFormat)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public EExportFormat ExportFormat
    {
      get => _exportFormat;
      set
      {
        if ( Equals(value, _exportFormat) )
          return;

        _exportFormat = value;
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Editor without path and extension
    /// </summary>
    public string Editor => string.IsNullOrWhiteSpace(_editorPath) ? string.Empty : Path.GetFileNameWithoutExtension(Path.GetFileName(_editorPath));

    #region Window settings

    private bool _restoreWindowSize;

    /// <summary>
    /// Restore window size at startup
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.RestoreWindowSize)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool RestoreWindowSize
    {
      get => _restoreWindowSize;
      set
      {
        if ( value == _restoreWindowSize )
          return;

        _restoreWindowSize = value;
        OnPropertyChanged();
      }
    }

    private bool _saveWindowPosition;

    /// <summary>
    /// Save window position
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.SaveWindowPosition)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool SaveWindowPosition
    {
      get => _saveWindowPosition;
      set
      {
        if ( value == _saveWindowPosition )
          return;

        _saveWindowPosition = value;
        OnPropertyChanged();
      }
    }

    private double _windowPositionX;

    /// <summary>
    /// X window position
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.WindowPositionX)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public double WindowPositionX
    {
      get => _windowPositionX;
      set
      {
        _windowPositionX = value;
        OnPropertyChanged();
      }
    }

    private double _windowPositionY;

    /// <summary>
    /// Y window position
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.WindowPositionY)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public double WindowPositionY
    {
      get => _windowPositionY;
      set
      {
        _windowPositionY = value;
        OnPropertyChanged();
      }
    }

    private double _optionWindowPositionX;

    /// <summary>
    /// X option window position
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.WindowPositionX)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public double OptionWindowPositionX
    {
      get => _optionWindowPositionX;
      set
      {
        _optionWindowPositionX = value;
        OnPropertyChanged();
      }
    }

    private double _optionWindowPositionY;

    /// <summary>
    /// Y option window position
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.WindowPositionY)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public double OptionWindowPositionY
    {
      get => _optionWindowPositionY;
      set
      {
        _optionWindowPositionY = value;
        OnPropertyChanged();
      }
    }

    private double _findResultPositionX;

    /// <summary>
    /// FindResult window X position
    /// </summary>
    [JsonIgnore]
    public double FindResultPositionX
    {
      get => _findResultPositionX;
      set
      {
        _findResultPositionX = value;
        OnPropertyChanged();
      }
    }

    private double _findResultPositionY;

    /// <summary>
    /// FindResult window Y position
    /// </summary>
    [JsonIgnore]
    public double FindResultPositionY
    {
      get => _findResultPositionY;
      set
      {
        _findResultPositionY = value;
        OnPropertyChanged();
      }
    }

    private double _findResultHeight;

    /// <summary>
    /// FindResult window height
    /// </summary>
    [JsonIgnore]
    public double FindResultHeight
    {
      get => _findResultHeight;
      set
      {
        _findResultHeight = value;
        OnPropertyChanged();
      }
    }

    private double _findResultWidth;

    /// <summary>
    /// FindResult window width
    /// </summary>
    [JsonIgnore]
    public double FindResultWidth
    {
      get => _findResultWidth;
      set
      {
        _findResultWidth = value;
        OnPropertyChanged();
      }
    }

    private double _findDialogPositionX;

    /// <summary>
    /// FindDialog window X position
    /// </summary>
    [JsonIgnore]
    public double FindDialogPositionX
    {
      get => _findDialogPositionX;
      set
      {
        _findDialogPositionX = value;
        OnPropertyChanged();
      }
    }

    private double _findDialogPositionY;

    /// <summary>
    /// FindDialog window Y position
    /// </summary>
    [JsonIgnore]
    public double FindDialogPositionY
    {
      get => _findDialogPositionY;
      set
      {
        _findDialogPositionY = value;
        OnPropertyChanged();
      }
    }

    private double _bookmarkOverviewPositionX;

    /// <summary>
    /// Bookmark overview window X position
    /// </summary>
    [JsonIgnore]
    public double BookmarkOverviewPositionX
    {
      get => _bookmarkOverviewPositionX;
      set
      {
        _bookmarkOverviewPositionX = value;
        OnPropertyChanged();
      }
    }

    private double _bookmarkOverviewPositionY;

    /// <summary>
    /// Bookmark overview window Y position
    /// </summary>
    [JsonIgnore]
    public double BookmarkOverviewPositionY
    {
      get => _bookmarkOverviewPositionY;
      set
      {
        _bookmarkOverviewPositionY = value;
        OnPropertyChanged();
      }
    }

    private double _bookmarkOverviewHeight;

    /// <summary>
    /// Bookmark overview window height
    /// </summary>
    [JsonIgnore]
    public double BookmarkOverviewHeight
    {
      get => _bookmarkOverviewHeight;
      set
      {
        _bookmarkOverviewHeight = value;
        OnPropertyChanged();
      }
    }

    private double _bookmarkOverviewWidth;

    /// <summary>
    /// Bookmark overview window width
    /// </summary>
    [JsonIgnore]
    public double BookmarkOverviewWidth
    {
      get => _bookmarkOverviewWidth;
      set
      {
        _bookmarkOverviewWidth = value;
        OnPropertyChanged();
      }
    }

    private double _windowHeight;

    /// <summary>
    /// Window height
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.WindowHeight)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public double WindowHeight
    {
      get => _windowHeight;
      set
      {
        _windowHeight = value;
        OnPropertyChanged();
      }
    }

    private double _windowWidth;

    /// <summary>
    /// Window width
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.WindowWidth)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public double WindowWidth
    {
      get => _windowWidth;
      set
      {
        _windowWidth = value;
        OnPropertyChanged();
      }
    }

    private double _optionWindowHeight;

    /// <summary>
    /// Option window height
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.WindowHeight)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public double OptionWindowHeight
    {
      get => _optionWindowHeight;
      set
      {
        _optionWindowHeight = value;
        OnPropertyChanged();
      }
    }

    private double _optionWindowWidth;

    /// <summary>
    /// Option window width
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.WindowWidth)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public double OptionWindowWidth
    {
      get => _optionWindowWidth;
      set
      {
        _optionWindowWidth = value;
        OnPropertyChanged();
      }
    }

    private System.Windows.WindowState _currentWindowState;

    /// <summary>
    /// Current window state
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.CurrentWindowState)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public System.Windows.WindowState CurrentWindowState
    {
      get => _currentWindowState;
      set
      {
        if ( value == _currentWindowState )
          return;

        _currentWindowState = value;
        OnPropertyChanged();
      }
    }

    private bool _activateDragDropWindow;

    /// <summary>
    /// Activate Drag'n'Drop window behavior
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.ActivateDragDropWindow)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool ActivateDragDropWindow
    {
      get => _activateDragDropWindow;
      set
      {
        if ( value == _activateDragDropWindow )
          return;

        _activateDragDropWindow = value;
        OnPropertyChanged();
      }
    }

    private bool _saveLogFileHistory;

    /// <summary>
    /// Save current log file history
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.SaveLogFileHistory)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool SaveLogFileHistory
    {
      get => _saveLogFileHistory;
      set
      {
        if ( value == _saveLogFileHistory )
          return;

        _saveLogFileHistory = value;
        OnPropertyChanged();
      }
    }

    private int _historyMaxSize;

    /// <summary>
    /// LogFile history max size
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.HistoryMaxSize)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public int HistoryMaxSize
    {
      get => _historyMaxSize;
      set
      {
        if ( value == _historyMaxSize )
          return;

        _historyMaxSize = value;
        OnPropertyChanged();
      }
    }

    private bool _splitterWindowBehavior;

    /// <summary>
    /// SplitterWindow behavior
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.SplitterWindowBehavior)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool SplitterWindowBehavior
    {
      get => _splitterWindowBehavior;
      set
      {
        if ( value == _splitterWindowBehavior )
          return;

        _splitterWindowBehavior = value;
        OnPropertyChanged();
      }
    }

    #endregion

    #region StatisticWindow settings

    private double _statisticWindowLeft;

    /// <summary>
    /// Statistic window left
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.StatisticWindowLeft)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public double StatisticWindowLeft
    {
      get => _statisticWindowLeft;
      set
      {
        _statisticWindowLeft = value;
        OnPropertyChanged();
      }
    }

    private double _statisticWindowTop;

    /// <summary>
    /// Statistic window top
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.StatisticWindowTop)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public double StatisticWindowTop
    {
      get => _statisticWindowTop;
      set
      {
        _statisticWindowTop = value;
        OnPropertyChanged();
      }
    }

    private double _statisticWindowHeight;

    /// <summary>
    /// Statistic window height
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.StatisticWindowHeight)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public double StatisticWindowHeight
    {
      get => _statisticWindowHeight;
      set
      {
        _statisticWindowHeight = value;
        OnPropertyChanged();
      }
    }

    private double _statisticWindowWidth;

    /// <summary>
    /// Statistic window width
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.StatisticWindowWidth)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public double StatisticWindowWidth
    {
      get => _statisticWindowWidth;
      set
      {
        _statisticWindowWidth = value;
        OnPropertyChanged();
      }
    }

    #endregion

    private bool _singleInstance;

    /// <summary>
    /// Single instance
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.SingleInstance)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool SingleInstance
    {
      get => _singleInstance;
      set
      {
        if ( value == _singleInstance )
          return;

        _singleInstance = value;
        OnPropertyChanged();
      }
    }

    private bool _exitWithEscape;

    /// <summary>
    /// Close/exist T4W by pressing Escape key
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.ExitWithEscape)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool ExitWithEscape
    {
      get => _exitWithEscape;
      set
      {
        if ( value == _exitWithEscape )
          return;

        _exitWithEscape = value;
        OnPropertyChanged();
      }
    }

    private bool _alwaysOnTop;

    /// <summary>
    /// T4W window always on top
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.AlwaysOnTop)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool AlwaysOnTop
    {
      get => _alwaysOnTop;
      set
      {
        if ( value == _alwaysOnTop )
          return;

        _alwaysOnTop = value;
        OnPropertyChanged();
      }
    }

    private bool _deleteLogFiles;

    /// <summary>
    /// Delete old T4W log files
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.DeleteLogFiles)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool DeleteLogFiles
    {
      get => _deleteLogFiles;
      set
      {
        if ( value == _deleteLogFiles )
          return;

        _deleteLogFiles = value;
        OnPropertyChanged();
      }
    }

    private int _logFilesOlderThan;

    /// <summary>
    /// Log files older than xxx days
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.DeleteLogFilesOlderThan)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public int LogFilesOlderThan
    {
      get => _logFilesOlderThan;
      set
      {
        if ( value == _logFilesOlderThan )
          return;

        _logFilesOlderThan = value;
        OnPropertyChanged();
      }
    }

    private EWindowStyle _currentWindowStyle;

    /// <summary>
    /// Current window style
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.CurrentWindowStyle)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public EWindowStyle CurrentWindowStyle
    {
      get => _currentWindowStyle;
      set
      {
        if ( value == _currentWindowStyle )
          return;

        _currentWindowStyle = value;
        OnPropertyChanged();
      }
    }

    private bool _alwaysScrollToEnd;

    /// <summary>
    /// Always scroll to end
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.AlwaysScrollToEnd)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool AlwaysScrollToEnd
    {
      get => _alwaysScrollToEnd;
      set
      {
        if ( value == _alwaysScrollToEnd )
          return;

        _alwaysScrollToEnd = value;
        OnPropertyChanged();
      }
    }

    private bool _continuedScroll;

    /// <summary>
    /// ContinuedScroll or push method
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.ContinuedScroll)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool ContinuedScroll
    {
      get => _continuedScroll;
      set
      {
        if ( value == _continuedScroll )
          return;

        _continuedScroll = value;
        OnPropertyChanged();
      }
    }

    private bool _showLineNumbers;

    /// <summary>
    /// Shows line numbers
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.ShowLineNumbers)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool ShowLineNumbers
    {
      get => _showLineNumbers;
      set
      {
        if ( value == _showLineNumbers )
          return;

        _showLineNumbers = value;
        OnPropertyChanged();
      }
    }

    private bool _showNumberLineAtStart;

    /// <summary>
    /// Show n numbers at start
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.ShowNumberLineAtStart)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool ShowNumberLineAtStart
    {
      get => _showNumberLineAtStart;
      set
      {
        if ( value == _showNumberLineAtStart )
          return;

        _showNumberLineAtStart = value;
        OnPropertyChanged();
      }
    }

    private int _linesRead;

    /// <summary>
    /// Lines read at start
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.LinesRead)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public int LinesRead
    {
      get => _linesRead;
      set
      {
        if ( value == _linesRead )
          return;

        _linesRead = value;
        OnPropertyChanged();
      }
    }

    private bool _groupByCategory;

    /// <summary>
    /// DataGrid group by category
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.GroupByCategory)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool GroupByCategory
    {
      get => _groupByCategory;
      set
      {
        if ( value == _groupByCategory )
          return;

        _groupByCategory = value;
        OnPropertyChanged();
      }
    }

    private bool _autoUpdate;

    /// <summary>
    /// AutoUpdate
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.AutoUpdate)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool AutoUpdate
    {
      get => _autoUpdate;
      set
      {
        if ( value == _autoUpdate )
          return;

        _autoUpdate = value;
        OnPropertyChanged();
      }
    }

    private ETailRefreshRate _defaultRefreshRate;

    /// <summary>
    /// Default refresh rate
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.DefaultRefreshRate)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public ETailRefreshRate DefaultRefreshRate
    {
      get => _defaultRefreshRate;
      set
      {
        if ( value == _defaultRefreshRate )
          return;

        _defaultRefreshRate = value;
        OnPropertyChanged();
      }
    }

    private ThreadPriority _defaultThreadPriority;

    /// <summary>
    /// Default thread priority
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.DefaultThreadPriority)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public ThreadPriority DefaultThreadPriority
    {
      get => _defaultThreadPriority;
      set
      {
        if ( value == _defaultThreadPriority )
          return;

        _defaultThreadPriority = value;
        OnPropertyChanged();
      }
    }

    private ETimeFormat _defaultTimeFormat;

    /// <summary>
    /// Default time format
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.DefaultTimeFormat)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public ETimeFormat DefaultTimeFormat
    {
      get => _defaultTimeFormat;
      set
      {
        if ( value == _defaultTimeFormat )
          return;

        _defaultTimeFormat = value;
        OnPropertyChanged();
      }
    }

    private EDateFormat _defaultDateFormat;

    /// <summary>
    /// Default date format
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.DefaultDateFormat)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public EDateFormat DefaultDateFormat
    {
      get => _defaultDateFormat;
      set
      {
        if ( value == _defaultDateFormat )
          return;


        _defaultDateFormat = value;
        OnPropertyChanged();
      }
    }

    private EFileSort _defaultFileSort;

    /// <summary>
    /// Default file sort
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.DefaultFileSort)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public EFileSort DefaultFileSort
    {
      get => _defaultFileSort;
      set
      {
        if ( value == _defaultFileSort )
          return;

        _defaultFileSort = value;
        OnPropertyChanged();
      }
    }

    private int _logLineLimit;

    /// <summary>
    /// Log line limitation
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.LogLineLimit)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public int LogLineLimit
    {
      get => _logLineLimit;
      set
      {
        if ( value == _logLineLimit )
          return;

        _logLineLimit = value;
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Last viewed option page
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public Guid LastViewedOptionPage
    {
      get;
      set;
    }

    private bool _smartWatch;

    /// <summary>
    /// Enable SmartWatch
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.SmartWatch)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool SmartWatch
    {
      get => _smartWatch;
      set
      {
        if ( value == _smartWatch )
          return;

        _smartWatch = value;
        OnPropertyChanged();
      }
    }

    private bool _statistics;

    /// <summary>
    /// Statistics for nerds
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.Statistics)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool Statistics
    {
      get => _statistics;
      set
      {
        if ( value == _statistics )
          return;

        _statistics = value;
        OnPropertyChanged();
      }
    }

    private bool _mouseHover;

    /// <summary>
    /// MouseHover in LogWindow
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.MouseHover)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool MouseHover
    {
      get => _mouseHover;
      set
      {
        if ( value == _mouseHover )
          return;

        _mouseHover = value;
        OnPropertyChanged();
      }
    }

    private bool _clearLogWindowIfLogIsCleared;

    /// <summary>
    /// Clear LogWindow when the log file is cleared or deleted
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.ClearLogWindowIfLogIsCleared)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool ClearLogWindowIfLogIsCleared
    {
      get => _clearLogWindowIfLogIsCleared;
      set
      {
        if ( value == _clearLogWindowIfLogIsCleared )
          return;

        _clearLogWindowIfLogIsCleared = value;
        OnPropertyChanged();
      }
    }

    private bool _showExtendedSettings;

    /// <summary>
    /// Show extended settings
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.ShowExtendedSettings)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool ShowExtendedSettings
    {
      get => _showExtendedSettings;
      set
      {
        if ( value == _showExtendedSettings )
          return;

        _showExtendedSettings = value;
        OnPropertyChanged();
      }
    }

    #region Proxy settings

    /// <summary>
    /// Current proxy settings
    /// </summary>
    public ProxySetting ProxySettings
    {
      get;
      set;
    }

    #endregion

    #region Alert settings

    /// <summary>
    /// Current alert settings
    /// </summary>
    public AlertSetting AlertSettings
    {
      get;
      set;
    }

    #endregion

    #region SmartWatch settings

    /// <summary>
    /// Current SmartWatch settings
    /// </summary>
    public SmartWatchSetting SmartWatchSettings
    {
      get;
      set;
    }

    #endregion

    #region EnvironmentColor settings

    /// <summary>
    /// Environment color settings
    /// </summary>
    public EnvironmentColorSettings ColorSettings
    {
      get;
      set;
    }

    #endregion

    #region SMTP settings

    /// <summary>
    /// Current SMTP settings
    /// </summary>
    public SmtpSetting SmtpSettings
    {
      get;
      set;
    }

    #endregion
  }
}
