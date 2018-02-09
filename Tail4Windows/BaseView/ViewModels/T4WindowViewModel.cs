using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using log4net;
using Org.Vs.TailForWin.Business.Data.Messages;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.UI.Commands;
using Org.Vs.TailForWin.UI.Interfaces;


namespace Org.Vs.TailForWin.BaseView.ViewModels
{
  /// <inheritdoc />
  /// <summary>
  /// T4Window view model
  /// </summary>
  public class T4WindowViewModel : NotifyMaster
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(T4WindowViewModel));

    private readonly NotifyTaskCompletion _notifyTaskCompletion;

    #region Events

    #endregion

    #region Properties

    /// <summary>
    /// Default width
    /// </summary>
    public double DefaultWidth
    {
      get;
    } = 800;

    /// <summary>
    /// Default height
    /// </summary>
    public double DefaultHeight
    {
      get;
    } = 600;

    /// <summary>
    /// Default window position X
    /// </summary>
    public double DefaultWindowPositionX
    {
      get;
    } = 100;

    /// <summary>
    /// Default window position Y
    /// </summary>
    public double DefaultWindowPositionY
    {
      get;
    } = 100;

    private double _width;

    /// <summary>
    /// Width of main window
    /// </summary>
    public double Width
    {
      get => _width;
      set
      {
        _width = value;
        OnPropertyChanged(nameof(Width));
      }
    }

    private double _height;

    /// <summary>
    /// Height of main window
    /// </summary>
    public double Height
    {
      get => _height;
      set
      {
        _height = value;
        OnPropertyChanged(nameof(Height));
      }
    }

    private WindowState _windowState;

    /// <summary>
    /// Window state
    /// </summary>
    public WindowState WindowState
    {
      get => _windowState;
      set
      {
        _windowState = value;
        OnPropertyChanged(nameof(WindowState));
      }
    }

    private double _windowPositionX;

    /// <summary>
    /// Window X position
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
    /// Window Y position
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

    private Style _t4WindowsStyle;

    /// <summary>
    /// Window style
    /// </summary>
    public Style T4WindowsStyle
    {
      get => _t4WindowsStyle;
      set
      {
        _t4WindowsStyle = value;
        OnPropertyChanged(nameof(T4WindowsStyle));
      }
    }

    private Brush _mainWindowStatusBarBackgroundColor;

    /// <summary>
    /// MainWindow StatusBar background color
    /// </summary>
    public Brush MainWindowStatusBarBackgroundColor
    {
      get => _mainWindowStatusBarBackgroundColor;
      set
      {
        _mainWindowStatusBarBackgroundColor = value;
        OnPropertyChanged(nameof(MainWindowStatusBarBackgroundColor));
      }
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public T4WindowViewModel()
    {
      _notifyTaskCompletion = NotifyTaskCompletion.Create(StartUpAsync());
      _notifyTaskCompletion.PropertyChanged += TaskPropertyChanged;
    }

    private void TaskPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !(sender is NotifyTaskCompletion) || !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      _notifyTaskCompletion.PropertyChanged -= TaskPropertyChanged;
    }

    private async Task StartUpAsync()
    {
      await EnvironmentContainer.Instance.ReadSettingsAsync().ConfigureAwait(false);

      SetDefaultWindowSettings();
      MoveIntoView();
      RestoreWindowSizeAndPosition();
    }

    #region Commands

    private ICommand _goToLineCommand;

    /// <summary>
    /// Go to line xxx command
    /// </summary>
    public ICommand GoToLineCommand => _goToLineCommand ?? (_goToLineCommand = new RelayCommand(p => ExecuteGoToLineCommand()));

    private ICommand _quickSearchCommand;

    /// <summary>
    /// Quick search command
    /// </summary>
    public ICommand QuickSearchCommand => _quickSearchCommand ?? (_quickSearchCommand = new RelayCommand(p => ExecuteQuickSearchCommand()));

    private ICommand _wndLoadedCommand;

    /// <summary>
    /// Window loaded command
    /// </summary>
    public ICommand WndLoadedCommand => _wndLoadedCommand ?? (_wndLoadedCommand = new RelayCommand(p => ExecuteWndLoadedCommand()));

    private IAsyncCommand _wndClosingCommand;

    /// <summary>
    /// Window closing command
    /// </summary>
    public IAsyncCommand WndClosingCommand => _wndClosingCommand ?? (_wndClosingCommand = AsyncCommand.Create(p => ExecuteWndClosingCommandAsync()));

    #endregion

    #region Command functions

    private async Task ExecuteWndClosingCommandAsync()
    {
      LOG.Trace($"{EnvironmentContainer.ApplicationTitle} closing, goodbye!");

      if ( SettingsHelperController.CurrentSettings.DeleteLogFiles )
        await DeleteLogFilesAsync().ConfigureAwait(false);

      await EnvironmentContainer.Instance.SaveSettingsAsync().ConfigureAwait(false);
    }

    private void ExecuteWndLoadedCommand()
    {
      LOG.Trace($"{EnvironmentContainer.ApplicationTitle} startup completed!");
    }

    private void ExecuteQuickSearchCommand()
    {
      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new QuickSearchTextBoxGetFocusMessage(this, true));
    }

    private void ExecuteGoToLineCommand()
    {
      MessageBox.Show("Test", "Hint", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    #endregion

    #region HelperFunctions

    private void SetDefaultWindowSettings()
    {
      MainWindowStatusBarBackgroundColor = SettingsHelperController.CurrentSettings.StatusBarInactiveBackgroundColor;
      MainWindowStatusBarBackgroundColor.Freeze();

      switch ( SettingsHelperController.CurrentSettings.CurrentWindowStyle )
      {
      case EWindowStyle.ModernLightWindowStyle:

        T4WindowsStyle = (Style) Application.Current.TryFindResource("Tail4LightWindowStyle");
        break;

      case EWindowStyle.ModernBlueWindowStyle:

        T4WindowsStyle = (Style) Application.Current.TryFindResource("Tail4BlueWindowStyle");
        break;

      default:

        T4WindowsStyle = (Style) Application.Current.TryFindResource("Tail4LightWindowStyle");
        SettingsHelperController.CurrentSettings.CurrentWindowStyle = EWindowStyle.ModernLightWindowStyle;
        break;
      }
    }

    private void MoveIntoView()
    {
      LOG.Trace($"Move {EnvironmentContainer.ApplicationTitle} into view, if required.");

      if ( SettingsHelperController.CurrentSettings.WindowPositionY + SettingsHelperController.CurrentSettings.WindowHeight / 2 > SystemParameters.VirtualScreenHeight )
        SettingsHelperController.CurrentSettings.WindowPositionY = SystemParameters.VirtualScreenHeight - SettingsHelperController.CurrentSettings.WindowHeight;

      if ( SettingsHelperController.CurrentSettings.WindowPositionX + SettingsHelperController.CurrentSettings.WindowWidth / 2 > SystemParameters.VirtualScreenWidth )
        SettingsHelperController.CurrentSettings.WindowPositionX = SystemParameters.VirtualScreenWidth - SettingsHelperController.CurrentSettings.WindowWidth;

      if ( SettingsHelperController.CurrentSettings.WindowPositionY < 0 )
        SettingsHelperController.CurrentSettings.WindowPositionY = 0;

      if ( SettingsHelperController.CurrentSettings.WindowPositionX < 0 )
        SettingsHelperController.CurrentSettings.WindowPositionX = 0;
    }

    private void RestoreWindowSizeAndPosition()
    {

      if ( SettingsHelperController.CurrentSettings.CurrentWindowState == WindowState.Normal )
      {
        if ( SettingsHelperController.CurrentSettings.RestoreWindowSize )
        {
          Width = !SettingsHelperController.CurrentSettings.WindowWidth.Equals(-1) ? SettingsHelperController.CurrentSettings.WindowWidth : DefaultWidth;
          Height = !SettingsHelperController.CurrentSettings.WindowHeight.Equals(-1) ? SettingsHelperController.CurrentSettings.WindowHeight : DefaultHeight;
        }
        else
        {
          Width = DefaultWidth;
          Height = DefaultHeight;
        }

        if ( !SettingsHelperController.CurrentSettings.SaveWindowPosition )
        {
          WindowPositionX = DefaultWindowPositionX;
          WindowPositionY = DefaultWindowPositionY;
          return;
        }

        WindowPositionY = !SettingsHelperController.CurrentSettings.WindowPositionY.Equals(-1) ? SettingsHelperController.CurrentSettings.WindowPositionY : DefaultWindowPositionY;
        WindowPositionX = !SettingsHelperController.CurrentSettings.WindowPositionX.Equals(-1) ? SettingsHelperController.CurrentSettings.WindowPositionX : DefaultWindowPositionX;
      }
      else
      {
        WindowState = SettingsHelperController.CurrentSettings.CurrentWindowState;
      }
    }

    private async Task DeleteLogFilesAsync()
    {
      LOG.Trace($"Delete log files older than {EnvironmentContainer.DeleteLogFilesOlderThan} days...");

      if ( !Directory.Exists("logs") )
        return;

      await Task.Run(
        () =>
        {
          try
          {
            var files = new DirectoryInfo("logs").GetFiles("*.log");

            Parallel.ForEach(files.Where(p => DateTime.Now - p.LastWriteTimeUtc > TimeSpan.FromDays(EnvironmentContainer.DeleteLogFilesOlderThan)), item =>
            {
              try
              {
                item.Delete();
              }
              catch ( Exception ex )
              {
                LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
              }
            });
          }
          catch ( Exception ex )
          {
            LOG.Error(ex, "{0} caused a(n) {1}", ex.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
          }
        }).ConfigureAwait(false);
    }

    #endregion
  }
}
