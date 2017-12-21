using System.Globalization;
using System.Threading;
using Org.Vs.TailForWin.Data.Base;


namespace Org.Vs.TailForWin.Data.Settings
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
        if( Equals(_cultureInfo, value) )
          return;

        _cultureInfo = value;
        Thread.CurrentThread.CurrentUICulture = value;
        Thread.CurrentThread.CurrentCulture = value;

        OnPropertyChanged(nameof(CultureInfo));
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
  }
}
