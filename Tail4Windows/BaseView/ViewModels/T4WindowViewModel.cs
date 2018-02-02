using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using log4net;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Base;
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
    // ReSharper disable once InconsistentNaming
    private static readonly ILog LOG = LogManager.GetLogger(typeof(T4WindowViewModel));

    /// <summary>
    /// Window title
    /// </summary>
    public string WindowTitle => EnvironmentContainer.ApplicationTitle;

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
    } = 900;

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

    /// <summary>
    /// Standard constructor
    /// </summary>
    public T4WindowViewModel()
    {
      WindowPositionX = 100;
      WindowPositionY = 100;
    }

    #region Commands

    private ICommand _goToLineCommand;

    /// <summary>
    /// Go to line xxx command
    /// </summary>
    public ICommand GoToLineCommand => _goToLineCommand ?? (_goToLineCommand = new RelayCommand(p => ExecuteGoToLineCommand()));

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
      LOG.Trace($"{WindowTitle} closing, goodbye!");
      await EnvironmentContainer.Instance.SaveSettingsAsync().ConfigureAwait(false);
    }

    private void ExecuteWndLoadedCommand()
    {
      LOG.Trace($"{WindowTitle} startup completed!");
    }

    private void ExecuteGoToLineCommand()
    {
      MessageBox.Show("Test", "Hint", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    #endregion

    #region HelperFunctions

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
          return;

        if ( !SettingsHelperController.CurrentSettings.WindowPositionY.Equals(-1) )
          WindowPositionY = SettingsHelperController.CurrentSettings.WindowPositionY;
        if ( !SettingsHelperController.CurrentSettings.WindowPositionX.Equals(-1) )
          WindowPositionX = SettingsHelperController.CurrentSettings.WindowPositionX;
      }
      else
      {
        WindowState = SettingsHelperController.CurrentSettings.CurrentWindowState;
      }
    }

    #endregion
  }
}
