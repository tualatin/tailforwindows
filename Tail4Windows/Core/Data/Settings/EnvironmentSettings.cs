using System.Globalization;
using System.Threading;
using System.Windows.Media;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Enums;


namespace Org.Vs.TailForWin.Core.Data.Settings
{
  /// <summary>
  /// Enviroment settings object
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
    }

    private CultureInfo _cultureInfo;

    /// <summary>
    /// Current culture info
    /// </summary>
    public CultureInfo CurrentCultureInfo
    {
      get => _cultureInfo ?? Thread.CurrentThread.CurrentUICulture;
      private set
      {
        if ( Equals(_cultureInfo, value) )
          return;

        _cultureInfo = value;
        Thread.CurrentThread.CurrentUICulture = value;
        Thread.CurrentThread.CurrentCulture = value;

        OnPropertyChanged(nameof(CurrentCultureInfo));
      }
    }

    private EUiLanguage _language;

    /// <summary>
    /// Current UI language
    /// </summary>
    public EUiLanguage Language
    {
      get => _language;
      set
      {
        if ( value == _language )
          return;

        _language = value;
        OnPropertyChanged(nameof(Language));
      }
    }

    #region Window settings

    private bool _restoreWindowSize;

    /// <summary>
    /// Restore window size at startup
    /// </summary>
    public bool RestoreWindowSize
    {
      get => _restoreWindowSize;
      set
      {
        if ( value == _restoreWindowSize )
          return;

        _restoreWindowSize = value;
        OnPropertyChanged(nameof(RestoreWindowSize));
      }
    }

    private bool _saveWindowPosition;

    /// <summary>
    /// Save window position
    /// </summary>
    public bool SaveWindowPosition
    {
      get => _saveWindowPosition;
      set
      {
        if ( value == _saveWindowPosition )
          return;

        _saveWindowPosition = value;
        OnPropertyChanged(nameof(SaveWindowPosition));
      }
    }

    private double _windowPositionX;

    /// <summary>
    /// X window position
    /// </summary>
    public double WindowPositionX
    {
      get => _windowPositionX;
      set
      {
        _windowPositionX = value;
        OnPropertyChanged(nameof(WindowPositionX));
      }
    }

    private double _windowPositionY;

    /// <summary>
    /// Y window position
    /// </summary>
    public double WindowPositionY
    {
      get => _windowPositionY;
      set
      {
        _windowPositionY = value;
        OnPropertyChanged(nameof(WindowPositionY));
      }
    }

    private double _windowHeight;

    /// <summary>
    /// Window height
    /// </summary>
    public double WindowHeight
    {
      get => _windowHeight;
      set
      {
        _windowHeight = value;
        OnPropertyChanged(nameof(WindowHeight));
      }
    }

    private double _windowWidth;

    /// <summary>
    /// Window width
    /// </summary>
    public double WindowWidth
    {
      get => _windowWidth;
      set
      {
        _windowWidth = value;
        OnPropertyChanged(nameof(WindowWidth));
      }
    }

    private System.Windows.WindowState _currentWindowState;

    /// <summary>
    /// Current window state
    /// </summary>
    public System.Windows.WindowState CurrentWindowState
    {
      get => _currentWindowState;
      set
      {
        if ( value == _currentWindowState )
          return;

        _currentWindowState = value;
        OnPropertyChanged(nameof(CurrentWindowState));
      }
    }

    #endregion

    #region StatusBar settings

    private Brush _statusBarInactiveBackgroundColor;

    /// <summary>
    /// StatusBar inactive background color
    /// </summary>
    public Brush StatusBarInactiveBackgroundColor
    {
      get => _statusBarInactiveBackgroundColor;
      set
      {
        if ( Equals(value, _statusBarInactiveBackgroundColor) )
          return;

        _statusBarInactiveBackgroundColor = value;
        OnPropertyChanged(nameof(StatusBarInactiveBackgroundColor));
      }
    }

    private Brush _statusBarFileLoadedBackgroundColor;

    /// <summary>
    /// StatusBar file loaded background color
    /// </summary>
    public Brush StatusBarFileLoadedBackgroundColor
    {
      get => _statusBarFileLoadedBackgroundColor;
      set
      {
        if ( Equals(value, _statusBarFileLoadedBackgroundColor) )
          return;

        _statusBarFileLoadedBackgroundColor = value;
        OnPropertyChanged(nameof(StatusBarFileLoadedBackgroundColor));
      }
    }

    private Brush _statusBarTailBackgroundColor;

    /// <summary>
    /// StatusBar tail background color
    /// </summary>
    public Brush StatusBarTailBackgroundColor
    {
      get => _statusBarTailBackgroundColor;
      set
      {
        if ( Equals(value, _statusBarTailBackgroundColor) )
          return;

        _statusBarTailBackgroundColor = value;
        OnPropertyChanged(nameof(StatusBarTailBackgroundColor));
      }
    }

    #endregion

    private bool _exitWithEscape;

    /// <summary>
    /// Close/exist T4W by pressing Escape key
    /// </summary>
    public bool ExitWithEscape
    {
      get => _exitWithEscape;
      set
      {
        if ( value == _exitWithEscape )
          return;

        _exitWithEscape = value;
        OnPropertyChanged(nameof(ExitWithEscape));
      }
    }

    private bool _alwaysOnTop;

    /// <summary>
    /// T4W window always on top
    /// </summary>
    public bool AlwaysOnTop
    {
      get => _alwaysOnTop;
      set
      {
        if ( value == _alwaysOnTop )
          return;

        _alwaysOnTop = value;
        OnPropertyChanged(nameof(AlwaysOnTop));
      }
    }

    private bool _deleteLogFiles;

    /// <summary>
    /// Delete old T4W log files
    /// </summary>
    public bool DeleteLogFiles
    {
      get => _deleteLogFiles;
      set
      {
        if ( value == _deleteLogFiles )
          return;

        _deleteLogFiles = value;
        OnPropertyChanged(nameof(DeleteLogFiles));
      }
    }

    private EWindowStyle _currentWindowStyle;

    /// <summary>
    /// Current window style
    /// </summary>
    public EWindowStyle CurrentWindowStyle
    {
      get => _currentWindowStyle;
      set
      {
        if ( value == _currentWindowStyle )
          return;

        _currentWindowStyle = value;
        OnPropertyChanged(nameof(CurrentWindowStyle));
      }
    }

    private bool _alwaysScrollToEnd;

    /// <summary>
    /// Always scroll to end
    /// </summary>
    public bool AlwaysScrollToEnd
    {
      get => _alwaysScrollToEnd;
      set
      {
        if ( value == _alwaysScrollToEnd )
          return;

        _alwaysScrollToEnd = value;
        OnPropertyChanged(nameof(AlwaysScrollToEnd));
      }
    }

    private bool _showLineNumbers;

    /// <summary>
    /// Show line numbers
    /// </summary>
    public bool ShowLineNumbers
    {
      get => _showLineNumbers;
      set
      {
        if ( value == _showLineNumbers )
          return;

        _showLineNumbers = value;
        OnPropertyChanged(nameof(ShowLineNumbers));
      }
    }

    private bool _showNumberLineAtStart;

    /// <summary>
    /// Show n numbers at start
    /// </summary>
    public bool ShowNumberLineAtStart
    {
      get => _showNumberLineAtStart;
      set
      {
        if ( value == _showNumberLineAtStart )
          return;

        _showNumberLineAtStart = value;
        OnPropertyChanged(nameof(ShowNumberLineAtStart));
      }
    }

    private int _linesRead;

    /// <summary>
    /// Lines read at start
    /// </summary>
    public int LinesRead
    {
      get => _linesRead;
      set
      {
        if ( value == _linesRead )
          return;

        _linesRead = value;
        OnPropertyChanged(nameof(LinesRead));
      }
    }

    private bool _groupByCategory;

    /// <summary>
    /// DataGrid group by category
    /// </summary>
    public bool GroupByCategory
    {
      get => _groupByCategory;
      set
      {
        if ( value == _groupByCategory )
          return;

        _groupByCategory = value;
        OnPropertyChanged(nameof(GroupByCategory));
      }
    }

    private bool _autoUpdate;

    /// <summary>
    /// AutoUpdate
    /// </summary>
    public bool AutoUpdate
    {
      get => _autoUpdate;
      set
      {
        if ( value == _autoUpdate )
          return;

        _autoUpdate = value;
        OnPropertyChanged(nameof(AutoUpdate));
      }
    }

    private ETailRefreshRate _defaultRefreshRate;

    /// <summary>
    /// Default refresh rate
    /// </summary>
    public ETailRefreshRate DefaultRefreshRate
    {
      get => _defaultRefreshRate;
      set
      {
        if ( value == _defaultRefreshRate )
          return;

        _defaultRefreshRate = value;
        OnPropertyChanged(nameof(DefaultRefreshRate));
      }
    }

    private ThreadPriority _defaulThreadPriority;

    /// <summary>
    /// Default thread priority
    /// </summary>
    public ThreadPriority DefaultThreadPriority
    {
      get => _defaulThreadPriority;
      set
      {
        if ( value == _defaulThreadPriority )
          return;

        _defaulThreadPriority = value;
        OnPropertyChanged(nameof(DefaultThreadPriority));
      }
    }

    private ETimeFormat _defaultTimeFormat;

    /// <summary>
    /// Default time format
    /// </summary>
    public ETimeFormat DefaultTimeFormat
    {
      get => _defaultTimeFormat;
      set
      {
        if ( value == _defaultTimeFormat )
          return;

        _defaultTimeFormat = value;
        OnPropertyChanged(nameof(DefaultTimeFormat));
      }
    }

    private EDateFormat _defaultDateFormat;

    /// <summary>
    /// Default date format
    /// </summary>
    public EDateFormat DefaultDateFormat
    {
      get => _defaultDateFormat;
      set
      {
        if ( value == _defaultDateFormat )
          return;

        _defaultDateFormat = value;
        OnPropertyChanged(nameof(DefaultDateFormat));
      }
    }

    #region ProxySettings

    /// <summary>
    /// Current proxy settings
    /// </summary>
    public ProxySetting ProxySettings
    {
      get;
      set;
    }

    #endregion

    #region AlertSettings

    /// <summary>
    /// Current alert settings
    /// </summary>
    public AlertSetting AlertSettings
    {
      get;
      set;
    }

    #endregion

    #region SmartWatchSettings

    /// <summary>
    /// Current SmartWatch settings
    /// </summary>
    public SmartWatchSetting SmartWatchSettings
    {
      get;
      set;
    }

    #endregion
  }
}
