using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using log4net;
using Org.Vs.TailForWin.BaseView.Events.Args;
using Org.Vs.TailForWin.BaseView.Interfaces;
using Org.Vs.TailForWin.Business.Data.Messages;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Data.Settings;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.PlugIns.LogWindowModule;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Events.Args;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Interfaces;
using Org.Vs.TailForWin.UI;
using Org.Vs.TailForWin.UI.Commands;
using Org.Vs.TailForWin.UI.Interfaces;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils;


namespace Org.Vs.TailForWin.BaseView.ViewModels
{
  /// <summary>
  /// T4Window view model
  /// </summary>
  public class T4WindowViewModel : NotifyMaster, IT4WindowViewModel
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(T4WindowViewModel));

    private readonly NotifyTaskCompletion _notifyTaskCompletion;
    private EStatusbarState _currentStatusbarState;
    private Encoding _currentEncoding;
    private int _currentLinesRead;
    private string _currentSizeRefreshTime;
    private readonly IBaseWindowStatusbarViewModel _baseWindowStatusbarViewModel;
    private readonly CancellationTokenSource _cts;

    #region Events

    #endregion

    #region Properties

    /// <summary>
    /// Default width
    /// </summary>
    private double DefaultWidth
    {
      get;
    } = 820;

    /// <summary>
    /// Default height
    /// </summary>
    private double DefaultHeight
    {
      get;
    } = 600;

    /// <summary>
    /// Default window position X
    /// </summary>
    private double DefaultWindowPositionX
    {
      get;
    } = 100;

    /// <summary>
    /// Default window position Y
    /// </summary>
    private double DefaultWindowPositionY
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
    public ObservableCollection<DragSupportMenuItem> TrayIconItemsSource
    {
      get;
      set;
    }

    /// <summary>
    /// Tab item source
    /// </summary>
    public ObservableCollection<DragSupportTabItem> TabItemsSource
    {
      get;
      set;
    }

    private DragSupportTabItem _selectedTabItem;

    /// <summary>
    /// Selected <see cref="DragSupportTabItem"/>
    /// </summary>
    public DragSupportTabItem SelectedTabItem
    {
      get => _selectedTabItem;
      set
      {
        if ( _selectedTabItem?.Content != null )
        {
          ((ILogWindowControl) _selectedTabItem.Content).OnStatusChanged -= OnStatusChangedCurrentLogWindow;
          ((ILogWindowControl) _selectedTabItem.Content).OnLinesTimeChanged -= OnLinesRefreshTimeChangedCurrentLogWindow;
        }

        _selectedTabItem = value;

        if ( _selectedTabItem == null )
          return;

        var content = (ILogWindowControl) _selectedTabItem.Content;
        content.OnStatusChanged += OnStatusChangedCurrentLogWindow;
        content.OnLinesTimeChanged += OnLinesRefreshTimeChangedCurrentLogWindow;

        _currentLinesRead = content.LinesRead;
        _currentSizeRefreshTime = content.TailReader.SizeRefreshTime;
        _currentStatusbarState = content.LogWindowState;
        _currentEncoding = content.CurrentTailData?.FileEncoding;

        SetCurrentBusinessData();
      }
    }

    /// <summary>
    /// Close action
    /// </summary>
    public Action CloseAction
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
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<AddTabItemMessage>(OnAddTabItemFromMainWindow);

      _cts = new CancellationTokenSource();
      _currentStatusbarState = EStatusbarState.Default;
      _notifyTaskCompletion = NotifyTaskCompletion.Create(StartUpAsync());
      _notifyTaskCompletion.PropertyChanged += TaskPropertyChanged;
      BusinessHelper.TabItemList.CollectionChanged += RegisteredTabItemsSourceCollectionChanged;

      _baseWindowStatusbarViewModel = BaseWindowStatusbarViewModel.Instance;
      _baseWindowStatusbarViewModel.FileEncodingChanged += OnFileEncodingChanged;

      CreateTrayIconSystemMenu();

      TabItemsSource = new ObservableCollection<DragSupportTabItem>();
    }

    private void CreateTrayIconSystemMenu()
    {
      TrayIconItemsSource = new ObservableCollection<DragSupportMenuItem>
      {
        new DragSupportMenuItem
        {
          HeaderContent = Application.Current.TryFindResource("ApplicationExit").ToString(),
          Command = new RelayCommand(p => ExecuteExitApplication())
        }
      };
    }

    private void OnFileEncodingChanged(object sender, FileEncodingArgs e)
    {
      if ( !(sender is BaseWindowStatusbarViewModel) || SelectedTabItem == null )
        return;

      var content = (ILogWindowControl) SelectedTabItem.Content;

      if ( content.CurrentTailData == null )
        return;

      if ( Equals(content.CurrentTailData.FileEncoding, e.Encoding) )
        return;

      content.CurrentTailData.FileEncoding = e.Encoding;
    }

    private void RegisteredTabItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      switch ( e.Action )
      {
      case NotifyCollectionChangedAction.Add:

        foreach ( var item in e.NewItems )
        {
          TrayIconItemsSource.Insert(0, new DragSupportMenuItem
          {
            TabItem = item as DragSupportTabItem
          });
        }
        break;

      case NotifyCollectionChangedAction.Remove:

        foreach ( var item in e.OldItems )
        {
          var toRemove = TrayIconItemsSource.SingleOrDefault(p => p.TabItem != null && p.TabItem.TabItemId == ((DragSupportTabItem) item).TabItemId);
          TrayIconItemsSource.Remove(toRemove);
        }
        break;

      case NotifyCollectionChangedAction.Reset:

        TrayIconItemsSource.Clear();
        break;
      }

      OnPropertyChanged(nameof(TrayIconItemsSource));
    }

    private void TaskPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !(sender is NotifyTaskCompletion) || !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      ExecuteAddNewTabItemCommand();

      SettingsHelperController.CurrentSettings.ColorSettings.PropertyChanged += ColorSettingsPropertyChanged;
      _notifyTaskCompletion.PropertyChanged -= TaskPropertyChanged;
    }

    private async Task StartUpAsync()
    {
      await EnvironmentContainer.Instance.ReadSettingsAsync().ConfigureAwait(false);

      SetUiLanguage();
      SetDefaultWindowSettings();
      MoveIntoView();
      RestoreWindowSizeAndPosition();

      await AutoUpdateAsync().ConfigureAwait(false);
    }

    private async Task CleanGarbageCollectorAsync()
    {
      while ( !_cts.IsCancellationRequested )
      {
        await Task.Delay(TimeSpan.FromMinutes(5), _cts.Token).ConfigureAwait(false);

        LOG.Trace("CleanUp GC..");
        LOG.Trace($"TotalMemory before clean up: {GC.GetTotalMemory(true)}");

        GC.Collect();
        GC.WaitForPendingFinalizers();

        LOG.Trace($"TotalMemory after clean up: {GC.GetTotalMemory(true)}");
      }
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

    private IAsyncCommand _loadedCommand;

    /// <summary>
    /// Window loaded command
    /// </summary>
    public IAsyncCommand LoadedCommand => _loadedCommand ?? (_loadedCommand = AsyncCommand.Create((p, t) => ExecuteWndLoadedCommandAsync()));

    /// <summary>
    /// Unloaded command
    /// </summary>
    public ICommand UnloadedCommand => throw new NotImplementedException();

    private IAsyncCommand _wndClosingCommand;

    /// <summary>
    /// Window closing command
    /// </summary>
    public IAsyncCommand WndClosingCommand => _wndClosingCommand ?? (_wndClosingCommand = AsyncCommand.Create((p, t) => ExecuteWndClosingCommandAsync(p)));

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

    private ICommand _addNewTabItemCommand;

    /// <summary>
    /// Adds a new <see cref="TabItem"/> to <see cref="DragSupportTabControl"/>
    /// </summary>
    public ICommand AddNewTabItemCommand => _addNewTabItemCommand ?? (_addNewTabItemCommand = new RelayCommand(p => ExecuteAddNewTabItemCommand()));

    private ICommand _closeTabItemCommand;

    /// <summary>
    /// Close a <see cref="DragSupportTabItem"/>
    /// </summary>
    public ICommand CloseTabItemCommand => _closeTabItemCommand ?? (_closeTabItemCommand = new RelayCommand(p => ExecuteCloseTabItemCommand()));

    #endregion

    #region Command functions

    private void ExecuteExitApplication() => CloseAction();

    private void ExecuteCloseTabItemCommand()
    {
      if ( SelectedTabItem == null )
        return;

      CloseTabItem(SelectedTabItem);
    }

    private void ExecuteAddNewTabItemCommand() => AddTabItem($"{Application.Current.TryFindResource("NoFile")}", $"{Application.Current.TryFindResource("NoFile")}", Visibility.Collapsed);

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

    private async Task ExecuteWndClosingCommandAsync(object param)
    {
      if ( !(param is CancelEventArgs e) )
        return;

      if ( e.Cancel )
        return;

      LOG.Trace($"{EnvironmentContainer.ApplicationTitle} closing, goodbye!");

      _cts.Cancel();
      TrayIconItemsSource.Clear();
      TabItemsSource.Clear();

      if ( SettingsHelperController.CurrentSettings.DeleteLogFiles )
        await DeleteLogFilesAsync().ConfigureAwait(false);

      await EnvironmentContainer.Instance.SaveSettingsAsync(new CancellationTokenSource(TimeSpan.FromMinutes(2))).ConfigureAwait(false);
    }

    private async Task ExecuteWndLoadedCommandAsync()
    {
      await Task.Run(() =>
      {
        LOG.Trace($"{EnvironmentContainer.ApplicationTitle} startup completed!");
      }).ConfigureAwait(false);

      await CleanGarbageCollectorAsync().ConfigureAwait(false);
    }

    private void ExecuteQuickSearchCommand() => EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new QuickSearchTextBoxGetFocusMessage(this, true));

    private void ExecuteGoToLineCommand()
    {
      LOG.Trace("Go to certain line...");
    }

    private void ExecuteToggleAlwaysOnTopCommand() => SettingsHelperController.CurrentSettings.AlwaysOnTop = !SettingsHelperController.CurrentSettings.AlwaysOnTop;

    #endregion

    #region Events

    private void TabItemCloseTabWindow(object sender, RoutedEventArgs e)
    {
      if ( e.Source is DragSupportTabItem item )
        CloseTabItem(item);
    }

    private void TabItemDoubleClick(object sender, RoutedEventArgs e)
    {
      LOG.Trace("MouseDoubleClick");
    }

    private void OnStatusChangedCurrentLogWindow(object sender, StatusChangedArgs e)
    {
      if ( !(sender is LogWindowControl) )
        return;

      _currentStatusbarState = e.State;
      _currentEncoding = e.LogFileEncoding;
      _currentLinesRead = e.LinesRead;
      _currentSizeRefreshTime = e.SizeRefreshTime;

      SetCurrentBusinessData();
    }

    private void OnLinesRefreshTimeChangedCurrentLogWindow(object sender, LinesRefreshTimeChangedArgs e)
    {
      if ( !(sender is LogWindowControl) )
        return;

      _baseWindowStatusbarViewModel.SizeRefreshTime = _currentSizeRefreshTime = e.SizeRefreshTime;
      _baseWindowStatusbarViewModel.LinesRead = _currentLinesRead = e.LinesRead;
    }

    #endregion

    #region HelperFunctions

    private void AddTabItem(string header, string toolTip, Visibility busyIndicator, ILogWindowControl content = null, string backgroundColor = DefaultEnvironmentSettings.TabItemHeaderBackgroundColor)
    {
      var tabItem = BusinessHelper.CreateDragSupportTabItem(header, toolTip, busyIndicator, content, backgroundColor);

      tabItem.CloseTabWindow += TabItemCloseTabWindow;
      tabItem.TabHeaderDoubleClick += TabItemDoubleClick;

      TabItemsSource.Add(tabItem);
    }

    private void OnAddTabItemFromMainWindow(AddTabItemMessage args)
    {
      if ( !(args?.Sender is T4Window) )
        return;

      AddTabItem(args.TabItem.HeaderContent, args.TabItem.HeaderToolTip, args.TabItem.TabItemBusyIndicator, (LogWindowControl) args.TabItem.Content, args.TabItem.TabItemBackgroundColorStringHex);
    }

    private void CloseTabItem(DragSupportTabItem item)
    {
      if ( !TabItemsSource.Contains(item) )
        return;

      if ( item.TabItemBusyIndicator == Visibility.Visible )
      {
        string message = $"{Application.Current.TryFindResource("QRemoveTab")} \n {item.HeaderFullText}";

        if ( InteractionService.ShowQuestionMessageBox(message) == MessageBoxResult.No )
          return;
      }

      item.TabHeaderDoubleClick -= TabItemDoubleClick;
      item.CloseTabWindow -= TabItemCloseTabWindow;

      BusinessHelper.UnregisterTabItem(item);
      TabItemsSource.Remove(item);

      if ( TabItemsSource.Count == 0 )
        ExecuteAddNewTabItemCommand();
    }

    private void SetDefaultWindowSettings()
    {
      SetCurrentBusinessData();

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
      switch ( SettingsHelperController.CurrentSettings.Language )
      {
      case EUiLanguage.English:

        LanguageSelector.SetLanguageResourceDictionary(EnvironmentContainer.ApplicationPath + @"\Language\en-EN.xaml");
        break;

      case EUiLanguage.German:

        LanguageSelector.SetLanguageResourceDictionary(EnvironmentContainer.ApplicationPath + @"\Language\de-DE.xaml");
        break;

      default:

        LanguageSelector.SetLanguageResourceDictionary(EnvironmentContainer.ApplicationPath + @"\Language\en-EN.xaml");
        break;
      }
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

    private void ColorSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("StatusBarInactiveBackgroundColorHex") && !e.PropertyName.Equals("StatusBarFileLoadedBackgroundColorHex")
                                                                         && !e.PropertyName.Equals("StatusBarTailBackgroundColorHex") && !e.PropertyName.Equals("RaiseOnPropertyChanged") )
        return;

      SetCurrentBusinessData();
    }

    private void SetCurrentBusinessData()
    {
      switch ( _currentStatusbarState )
      {
      case EStatusbarState.FileLoaded:

        _baseWindowStatusbarViewModel.CurrentStatusBarBackgroundColorHex = SettingsHelperController.CurrentSettings.ColorSettings.StatusBarFileLoadedBackgroundColorHex;
        _baseWindowStatusbarViewModel.CurrentBusyState = Application.Current.TryFindResource("TrayIconReady").ToString();
        break;

      case EStatusbarState.Busy:

        _baseWindowStatusbarViewModel.CurrentStatusBarBackgroundColorHex = SettingsHelperController.CurrentSettings.ColorSettings.StatusBarTailBackgroundColorHex;
        _baseWindowStatusbarViewModel.CurrentBusyState = Application.Current.TryFindResource("Record").ToString();
        break;

      case EStatusbarState.Default:

        _baseWindowStatusbarViewModel.CurrentStatusBarBackgroundColorHex = SettingsHelperController.CurrentSettings.ColorSettings.StatusBarInactiveBackgroundColorHex;
        _baseWindowStatusbarViewModel.CurrentBusyState = Application.Current.TryFindResource("TrayIconReady").ToString();
        break;

      default:

        throw new NotImplementedException();
      }

      _baseWindowStatusbarViewModel.SizeRefreshTime = _currentSizeRefreshTime;
      _baseWindowStatusbarViewModel.LinesRead = _currentLinesRead;
      _baseWindowStatusbarViewModel.CurrentEncoding = _currentEncoding;
    }

    #endregion
  }
}
