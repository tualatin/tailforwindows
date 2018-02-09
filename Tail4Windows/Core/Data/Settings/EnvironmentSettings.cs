using System.Globalization;
using System.Threading;
using System.Windows.Media;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Enums;


namespace Org.Vs.TailForWin.Core.Data.Settings
{
  /// <inheritdoc />
  /// <summary>
  /// Enviroment settings object
  /// </summary>
  public class EnvironmentSettings : NotifyMaster
  {
    private CultureInfo _cultureInfo;

    /// <summary>
    /// Current culture info
    /// </summary>
    public CultureInfo CurrentCultureInfo
    {
      get => _cultureInfo ?? Thread.CurrentThread.CurrentUICulture;
      set
      {
        if ( Equals(_cultureInfo, value) )
          return;

        _cultureInfo = value;
        Thread.CurrentThread.CurrentUICulture = value;
        Thread.CurrentThread.CurrentCulture = value;

        OnPropertyChanged(nameof(CurrentCultureInfo));
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
        _currentWindowStyle = value;
        OnPropertyChanged(nameof(CurrentWindowStyle));
      }
    }
  }
}
