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
using Org.Vs.TailForWin.Business.DbEngine.Controllers;
using Org.Vs.TailForWin.Business.DbEngine.Interfaces;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Data.Settings;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.Data.Messages;
using Org.Vs.TailForWin.Data.Messages.FindWhat;
using Org.Vs.TailForWin.Data.Messages.Keybindings;
using Org.Vs.TailForWin.Data.Messages.QuickSearchbar;
using Org.Vs.TailForWin.PlugIns.FindModule;
using Org.Vs.TailForWin.PlugIns.LogWindowModule;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Events.Args;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Interfaces;
using Org.Vs.TailForWin.UI;
using Org.Vs.TailForWin.UI.Commands;
using Org.Vs.TailForWin.UI.Interfaces;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils;
using Org.Vs.TailForWin.UI.Utils;


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
    private readonly FindWhatResult _findWhatResultWindow;
    private FindWhat _findWhatWindow;
    private readonly ISettingsDbController _dbSettingsController;

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

        EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new ChangeWindowGuiMessage(content.WindowId));
        OnFindWhatWindowTitleChanged(new DragWindowTabItemChangedMessage(this, _selectedTabItem.HeaderContent, content.WindowId));
        SetCurrentBusinessData();
      }
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public T4WindowViewModel()
    {
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<AddTabItemMessage>(OnAddTabItemFromMainWindow);
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<OpenFindWhatWindowMessage>(OnOpenFindWhatWindow);
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<DragWindowTabItemChangedMessage>(OnFindWhatWindowTitleChanged);
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<OpenFindWhatResultWindowMessage>(OnOpenFindWhatResultWindow);

      _cts = new CancellationTokenSource();
      _findWhatResultWindow = new FindWhatResult();
      _dbSettingsController = SettingsDbController.Instance;
      _currentStatusbarState = EStatusbarState.Default;
      _notifyTaskCompletion = NotifyTaskCompletion.Create(StartUpAsync());
      _notifyTaskCompletion.PropertyChanged += TaskPropertyChanged;
      BusinessHelper.TabItemList.CollectionChanged += RegisteredTabItemsSourceCollectionChanged;

      _baseWindowStatusbarViewModel = BaseWindowStatusbarViewModel.Instance;
      _baseWindowStatusbarViewModel.FileEncodingChanged += OnFileEncodingChanged;
      ((AsyncCommand<object>) WndClosingCommand).PropertyChanged += WndClosingCommandChanged;

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
          Icon = new Image { Source = BusinessHelper.CreateBitmapIcon(@"../../../Resources/transparent.png") },
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
            TabItem = item as DragSupportTabItem,
            Icon = new Image
            {
              Source = BusinessHelper.CreateBitmapIcon(@"../../../Resources/transparent.png")
            }
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

    private void WndClosingCommandChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      _findWhatResultWindow.ShouldClose = true;
      _findWhatResultWindow.Close();

      if ( _findWhatWindow != null && _findWhatWindow.Visibility == Visibility.Visible )
        _findWhatWindow.Close();
    }

    private async Task StartUpAsync()
    {
      await EnvironmentContainer.Instance.ReadSettingsAsync().ConfigureAwait(false);

      SetUiLanguage();
      SetDefaultWindowSettings();
      MoveIntoView();
      RestoreWindowSizeAndPosition();

      await MoveUserFilesToTailStoreAsync().ConfigureAwait(false);
      await _dbSettingsController.ReadDbSettingsAsync().ConfigureAwait(false);
      await AutoUpdateAsync().ConfigureAwait(false);
    }

    private async Task CleanGarbageCollectorAsync()
    {
      while ( !_cts.IsCancellationRequested )
      {
        await Task.Delay(TimeSpan.FromMinutes(15), _cts.Token).ConfigureAwait(false);

        LOG.Info("CleanUp GC..");
        LOG.Trace($"TotalMemory before clean up: {GC.GetTotalMemory(true)}");

        GC.Collect();
        GC.WaitForFullGCComplete();
        GC.WaitForPendingFinalizers();

        LOG.Trace($"TotalMemory after clean up: {GC.GetTotalMemory(true)}");
      }
    }

    /// <summary>
    /// Move some user files to new TailStore
    /// </summary>
    /// <see cref="Task"/>
    public async Task MoveUserFilesToTailStoreAsync()
    {
      LOG.Info("Try to move old user settings");

      await Task.Run(
        () =>
        {
          try
          {
            if ( !Directory.Exists(EnvironmentContainer.TailStorePath) )
              Directory.CreateDirectory(EnvironmentContainer.TailStorePath);

            string fileManager = EnvironmentContainer.ApplicationPath + @"\FileManager.xml";

            if ( File.Exists(fileManager) )
              File.Move(fileManager, EnvironmentContainer.TailStorePath + @"\FileManager.xml");

            string history = EnvironmentContainer.ApplicationPath + @"\History.xml";

            if ( !File.Exists(history) )
              return;

            File.Move(history, EnvironmentContainer.TailStorePath + @"\History.xml");
          }
          catch ( Exception ex )
          {
            LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
          }
        }).ConfigureAwait(false);
    }

    #region Commands

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

    private ICommand _activatedCommand;

    /// <summary>
    /// Close a <see cref="DragSupportTabItem"/>
    /// </summary>
    public ICommand ActivatedCommand => _activatedCommand ?? (_activatedCommand = new RelayCommand(p => ExecuteActivatedCommand()));

    private ICommand _deactivatedCommand;

    /// <summary>
    /// Close a <see cref="DragSupportTabItem"/>
    /// </summary>
    public ICommand DeactivatedCommand => _deactivatedCommand ?? (_deactivatedCommand = new RelayCommand(p => ExecuteDeactivatedCommand()));

    #endregion

    #region Command functions

    private void ExecuteActivatedCommand() => EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new SetFloatingTopmostFlagMessage(true));

    private void ExecuteDeactivatedCommand() => EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new SetFloatingTopmostFlagMessage(false));

    private void ExecuteExitApplication()
    {
      new ThrottledExecution().InMs(220).Do(() =>
      {
        Application.Current.Dispatcher.Invoke(() =>
        {
          if ( Application.Current.MainWindow != null )
            Application.Current.MainWindow.Close();
        });
      });
    }

    private void ExecuteCloseTabItemCommand()
    {
      if ( SelectedTabItem == null )
        return;

      CloseTabItem(SelectedTabItem);
    }

    private void ExecuteAddNewTabItemCommand() => AddTabItem($"{Application.Current.TryFindResource("NoFile")}", $"{Application.Current.TryFindResource("NoFile")}", Visibility.Collapsed);

    private void ExecutePreviewKeyDownCommand(object parameter)
    {
      if ( !(parameter is KeyEventArgs e) )
        return;

      switch ( e.Key )
      {
      case Key.Escape:

        if ( SettingsHelperController.CurrentSettings.ExitWithEscape )
          Application.Current.Shutdown(0);

        e.Handled = true;
        break;
      }
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

    #endregion

    #region KeyBinding commands

    private ICommand _toggleAlwaysOnTopCommand;

    /// <summary>
    /// Toggle always on top command
    /// </summary>
    public ICommand ToggleAlwaysOnTopCommand => _toggleAlwaysOnTopCommand ?? (_toggleAlwaysOnTopCommand = new RelayCommand(p => ExecuteToggleAlwaysOnTopCommand()));

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

    private ICommand _findWhatCommand;

    /// <summary>
    /// FindWhat command
    /// </summary>
    public ICommand FindWhatCommand => _findWhatCommand ?? (_findWhatCommand = new RelayCommand(p => ExecuteFindWhatCommand()));

    private ICommand _findWhatResultCommand;

    /// <summary>
    /// FindWhatResult command
    /// </summary>
    public ICommand FindWhatResultCommand => _findWhatResultCommand ?? (_findWhatResultCommand = new RelayCommand(p => ExecuteFindWhatResultCommand()));

    private ICommand _openFileManagerCommand;

    /// <summary>
    /// OpenFileManager command
    /// </summary>
    public ICommand OpenFileManagerCommand => _openFileManagerCommand ?? (_openFileManagerCommand = new RelayCommand(p => ExecuteOpenFileManagerCommand()));

    private ICommand _openFilterManagerCommand;

    /// <summary>
    /// OpenFilterManager command
    /// </summary>
    public ICommand OpenFilterManagerCommand => _openFilterManagerCommand ?? (_openFilterManagerCommand = new RelayCommand(p => ExecuteOpenFilterManagerCommand()));

    private ICommand _toggleFilterCommand;

    /// <summary>
    /// ToggleFilter command
    /// </summary>
    public ICommand ToggleFilterCommand => _toggleFilterCommand ?? (_toggleFilterCommand = new RelayCommand(p => ExecuteToggleFilterCommand()));

    private ICommand _quickSaveCommand;

    /// <summary>
    /// Quick save command
    /// </summary>
    public ICommand QuickSaveCommand => _quickSaveCommand ?? (_quickSaveCommand = new RelayCommand(p => ExecuteQuickSaveCommand()));

    private ICommand _openFileCommand;

    /// <summary>
    /// Open file command
    /// </summary>
    public ICommand OpenFileCommand => _openFileCommand ?? (_openFileCommand = new RelayCommand(p => ExecuteOpenFileCommand()));

    private ICommand _clearTailLogCommand;

    /// <summary>
    /// Clear tail log command
    /// </summary>
    public ICommand ClearTailLogCommand => _clearTailLogCommand ?? (_clearTailLogCommand = new RelayCommand(p => ExecuteClearTailLogCommand()));

    private ICommand _startTailCommand;

    /// <summary>
    /// Start tail command
    /// </summary>
    public ICommand StartTailCommand => _startTailCommand ?? (_startTailCommand = new RelayCommand(p => ExecuteStartTailCommand()));

    private ICommand _stopTailCommand;

    /// <summary>
    /// Stop tail command
    /// </summary>
    public ICommand StopTailCommand => _stopTailCommand ?? (_stopTailCommand = new RelayCommand(p => ExecuteStopTailCommand()));

    #endregion

    #region KeyBinding command functions

    private void ExecuteStopTailCommand()
    {
      if ( !(SelectedTabItem.Content is ILogWindowControl control) )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new StopTailMessage(control.WindowId));
    }

    private void ExecuteStartTailCommand()
    {
      if ( !(SelectedTabItem.Content is ILogWindowControl control) )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new StartTailMessage(control.WindowId));
    }

    private void ExecuteClearTailLogCommand()
    {
      if ( !(SelectedTabItem.Content is ILogWindowControl control) )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new ClearTailLogMessage(control.WindowId));
    }

    private void ExecuteOpenFileCommand()
    {
      if ( !(SelectedTabItem.Content is ILogWindowControl control) )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenFileMessage(control.WindowId));
    }

    private void ExecuteQuickSaveCommand()
    {
      if ( !(SelectedTabItem.Content is ILogWindowControl control) )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new QuickSaveMessage(control.WindowId));
    }

    private void ExecuteToggleFilterCommand()
    {
      if ( !(SelectedTabItem.Content is ILogWindowControl control) )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new ToggleFilterMessage(control.WindowId));
    }

    private void ExecuteOpenFilterManagerCommand()
    {
      if ( !(SelectedTabItem.Content is ILogWindowControl control) )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenFilterManagerMessage(control.WindowId));
    }

    private void ExecuteOpenFileManagerCommand()
    {
      if ( !(SelectedTabItem.Content is ILogWindowControl control) )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenTailManagerMessage(control.WindowId));
    }

    private void ExecuteFindWhatResultCommand()
    {
      if ( !(SelectedTabItem.Content is ILogWindowControl control) )
        return;

      OnOpenFindWhatResultWindow(new OpenFindWhatResultWindowMessage(control.SplitWindow.FindWhatResults, control.WindowId));
    }

    private void ExecuteFindWhatCommand()
    {
      if ( !(SelectedTabItem.Content is ILogWindowControl control) )
        return;

      string findWhat = control.SplitWindow.SelectedText;
      OnOpenFindWhatWindow(new OpenFindWhatWindowMessage(this, SelectedTabItem.HeaderContent, control.WindowId, findWhat));
    }

    private void ExecuteQuickSearchCommand() => EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new QuickSearchTextBoxGetFocusMessage(this, true));

    private void ExecuteGoToLineCommand()
    {
      if ( SelectedTabItem == null )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenGoToLineDialogMessage(((ILogWindowControl) SelectedTabItem.Content).ParentWindowId));
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
      DragWindow.CreateTabWindow(WindowPositionX + 10, WindowPositionY + 10, Width, height: Height, tabItem: SelectedTabItem);
      CloseTabItem(SelectedTabItem, true);
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

    private void OnOpenFindWhatWindow(OpenFindWhatWindowMessage args)
    {
      if ( _findWhatWindow != null && _findWhatWindow.IsVisible )
      {
        _findWhatWindow.SearchText = !string.IsNullOrWhiteSpace(args.FindWhat) ? args.FindWhat : null;
        _findWhatWindow.WindowGuid = args.WindowGuid;
        return;
      }

      _findWhatWindow = new FindWhat
      {
        ShouldClose = true,
        SearchText = !string.IsNullOrWhiteSpace(args.FindWhat) ? args.FindWhat : null,
        WindowGuid = args.WindowGuid
      };

      if ( !string.IsNullOrWhiteSpace(args.Title) )
        _findWhatWindow.DialogTitle = args.Title;

      _findWhatWindow.Show();
      _findWhatWindow.Focus();
    }

    private void OnFindWhatWindowTitleChanged(DragWindowTabItemChangedMessage args)
    {
      if ( args.Sender == null || _findWhatWindow == null )
        return;

      if ( !_findWhatWindow.IsVisible )
      {
        _findWhatWindow.Activate();
        _findWhatWindow.Focus();
        return;
      }

      _findWhatWindow.DialogTitle = args.NewTitle;
      _findWhatWindow.WindowGuid = args.WindowGuid;
    }

    private void OnOpenFindWhatResultWindow(OpenFindWhatResultWindowMessage args)
    {
      if ( !(SelectedTabItem.Content is ILogWindowControl control) )
        return;

      if ( _findWhatResultWindow.IsVisible )
      {
        EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new FindWhatResultMessage(args.FindWhatResults, control.WindowId));

        _findWhatResultWindow.Activate();
        _findWhatResultWindow.Focus();
        return;
      }

      _findWhatResultWindow.Show();
      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new FindWhatResultMessage(args.FindWhatResults, control.WindowId));
    }

    private void CloseTabItem(DragSupportTabItem item, bool tabItemDoubleClick = false)
    {
      if ( !TabItemsSource.Contains(item) )
        return;

      if ( item.TabItemBusyIndicator == Visibility.Visible && !tabItemDoubleClick )
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
      var result = await updateController.UpdateNecessaryAsync(new CancellationTokenSource(TimeSpan.FromMinutes(2)).Token,
        System.Reflection.Assembly.GetExecutingAssembly().GetName().Version).ConfigureAwait(false);

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
            LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
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
