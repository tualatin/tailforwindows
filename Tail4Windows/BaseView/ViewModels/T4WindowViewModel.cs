using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using log4net;
using Org.Vs.TailForWin.Business.Controllers;
using Org.Vs.TailForWin.Business.Data.Messages;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.UI;
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

    /// <summary>
    /// Tray icon items source
    /// </summary>
    public ObservableCollection<MenuItem> TrayIconItemsSource
    {
      get;
      set;
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public T4WindowViewModel()
    {
      _notifyTaskCompletion = NotifyTaskCompletion.Create(StartUpAsync());
      _notifyTaskCompletion.PropertyChanged += TaskPropertyChanged;

      TrayIconItemsSource = new ObservableCollection<MenuItem>();
      TrayIconItemsSource.CollectionChanged += TrayIconItemsSourceCollectionChanged;
    }

    private void TrayIconItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => OnPropertyChanged(nameof(TrayIconItemsSource));

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
      SetUiLanguage();

      await AutoUpdateAsync().ConfigureAwait(false);
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
    public IAsyncCommand WndClosingCommand => _wndClosingCommand ?? (_wndClosingCommand = AsyncCommand.Create(ExecuteWndClosingCommandAsync));

    private ICommand _toggleAlwaysOnTopCommand;

    /// <summary>
    /// Toggle always on top command
    /// </summary>
    public ICommand ToggleAlwaysOnTopCommand => _toggleAlwaysOnTopCommand ?? (_toggleAlwaysOnTopCommand = new RelayCommand(p => ExecuteToggleAlwaysOnTopCommand()));

    private ICommand _trayContextMenuOpenCommand;

    /// <summary>
    /// TrayContextMenuOpen command
    /// </summary>
    public ICommand TrayContextMenuOpenCommand => _trayContextMenuOpenCommand ?? (_trayContextMenuOpenCommand = new RelayCommand(p => ExecuteTrayContextMenuOpenCommand()));

    private ICommand _previewTrayContextMenuOpenCommand;

    /// <summary>
    /// PreviewTrayContextMenuOpen command
    /// </summary>
    public ICommand PreviewTrayContextMenuOpenCommand => _previewTrayContextMenuOpenCommand ?? (_previewTrayContextMenuOpenCommand = new RelayCommand(ExecutePreviewTrayContextMenuOpenCommand));

    private ICommand _previewKeyDownCommand;

    /// <summary>
    /// PreviewKeyDown command
    /// </summary>
    public ICommand PreviewKeyDownCommand => _previewKeyDownCommand ?? (_previewKeyDownCommand = new RelayCommand(ExecutePreviewKeyDownCommand));

    #endregion

    #region Command functions

    private void ExecutePreviewKeyDownCommand(object parameter)
    {
      if ( !(parameter is KeyEventArgs args) )
        return;

      switch ( args.Key )
      {
      case Key.Escape:

        if ( SettingsHelperController.CurrentSettings.ExitWithEscape )
          Application.Current.Shutdown(0);

        args.Handled = true;
        break;
      }
    }

    private void ExecuteTrayContextMenuOpenCommand()
    {
      LOG.Trace("Tray context menu open command");
    }

    private void ExecutePreviewTrayContextMenuOpenCommand(object parameter)
    {
      LOG.Trace("Preview tray context menu open command");
    }

    private async Task ExecuteWndClosingCommandAsync()
    {
      LOG.Trace($"{EnvironmentContainer.ApplicationTitle} closing, goodbye!");

      if ( SettingsHelperController.CurrentSettings.DeleteLogFiles )
        await DeleteLogFilesAsync().ConfigureAwait(false);

      await EnvironmentContainer.Instance.SaveSettingsAsync(new CancellationTokenSource(TimeSpan.FromMinutes(2))).ConfigureAwait(false);
    }

    private void ExecuteWndLoadedCommand() => LOG.Trace($"{EnvironmentContainer.ApplicationTitle} startup completed!");

    private void ExecuteQuickSearchCommand() => EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new QuickSearchTextBoxGetFocusMessage(this, true));

    private void ExecuteGoToLineCommand()
    {
      MessageBox.Show("Test", "Hint", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void ExecuteToggleAlwaysOnTopCommand() => SettingsHelperController.CurrentSettings.AlwaysOnTop = !SettingsHelperController.CurrentSettings.AlwaysOnTop;

    #endregion

    #region HelperFunctions

    private void SetDefaultWindowSettings()
    {
      BusinessMainWndToMainWndStatusBarController.CurrentBusinessData.CurrentStatusBarBackgroundColorHex = SettingsHelperController.CurrentSettings.ColorSettings.StatusBarInactiveBackgroundColorHex;
      BusinessMainWndToMainWndStatusBarController.CurrentBusinessData.CurrentBusyState = Application.Current.TryFindResource("TrayIconReady").ToString();

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

    private void SetUiLanguage()
    {
      ResourceDictionary dictionary = new ResourceDictionary();

      switch ( SettingsHelperController.CurrentSettings.Language )
      {
      case EUiLanguage.English:

        dictionary.Source = new Uri(@"..\Language\en-EN.xaml", UriKind.Relative);
        break;

      case EUiLanguage.German:

        dictionary.Source = new Uri(@"..\Language\de-DE.xaml", UriKind.Relative);
        break;

      default:

        dictionary.Source = new Uri(@"..\Language\en-EN.xaml", UriKind.Relative);
        break;
      }

      Application.Current.Resources.MergedDictionaries.Add(dictionary);
    }

    private async Task AutoUpdateAsync()
    {
      if ( !SettingsHelperController.CurrentSettings.AutoUpdate )
        return;

      var updateController = new UpdateController(new WebDataController());
      var result = await updateController.UpdateNecessaryAsync(new CancellationTokenSource(TimeSpan.FromMinutes(2)).Token).ConfigureAwait(false);

      if ( !result.Update )
        return;

      new ThrottledExecution().InMs(5000).Do(
        () =>
        {
          var staThread = new Thread(() =>
          {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
              var updateDialog = new AutoUpdatePopUp
              {
                ApplicationVersion = result.ApplicationVersion.ToString(),
                WebVersion = result.WebVersion.ToString(),
                UpdateHint = Application.Current.TryFindResource("UpdateControlUpdateExits").ToString()
              };

              var wnd = new Window
              {
                Visibility = Visibility.Hidden,
                WindowState = WindowState.Minimized,
                ShowInTaskbar = false
              };

              wnd.Show();
              updateDialog.Owner = wnd;
              updateDialog.ShowDialog();
            }, DispatcherPriority.Normal);
          })
          {
            Name = $"{EnvironmentContainer.ApplicationTitle}_AutoUpdateThread",
            IsBackground = true
          };

          staThread.SetApartmentState(ApartmentState.STA);
          staThread.Start();
          staThread.Join();
        });
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
      LOG.Trace($"Delete log files older than {SettingsHelperController.CurrentSettings.LogFilesOlderThan} days...");

      if ( !Directory.Exists("logs") )
        return;

      await Task.Run(
        () =>
        {
          try
          {
            var files = new DirectoryInfo("logs").GetFiles("*.log");

            Parallel.ForEach(files.Where(p => DateTime.Now - p.LastWriteTimeUtc > TimeSpan.FromDays(SettingsHelperController.CurrentSettings.LogFilesOlderThan)), item =>
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
