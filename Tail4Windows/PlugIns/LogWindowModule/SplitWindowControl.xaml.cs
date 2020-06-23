using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using log4net;
using Org.Vs.TailForWin.BaseView.ViewModels;
using Org.Vs.TailForWin.Business.BookmarkEngine.Events.Args;
using Org.Vs.TailForWin.Business.BookmarkEngine.Interfaces;
using Org.Vs.TailForWin.Business.SearchEngine.Controllers;
using Org.Vs.TailForWin.Business.SearchEngine.Interfaces;
using Org.Vs.TailForWin.Business.Services.Data;
using Org.Vs.TailForWin.Business.Services.Events.Args;
using Org.Vs.TailForWin.Business.Services.Interfaces;
using Org.Vs.TailForWin.Business.SmartWatchEngine.Controllers;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Business.Utils.Interfaces;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.FindModule;
using Org.Vs.TailForWin.Controllers.PlugIns.FindModule.Data;
using Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Data;
using Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Events.Args;
using Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Events.Delegates;
using Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Utils;
using Org.Vs.TailForWin.Controllers.PlugIns.SmartWatchPopupModule.Events.Args;
using Org.Vs.TailForWin.Core.Collections.FilterCollections;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.Data.Messages;
using Org.Vs.TailForWin.Data.Messages.FindWhat;
using Org.Vs.TailForWin.Data.Messages.Keybindings;
using Org.Vs.TailForWin.PlugIns.BookmarkCommentModule;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Interfaces;
using Org.Vs.TailForWin.PlugIns.SmartWatchPopupModule;
using Org.Vs.TailForWin.Ui.PlugIns.VsControls;
using Org.Vs.TailForWin.Ui.Utils.Extensions;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule
{
  /// <summary>
  /// Interaction logic for SplitWindowControl.xaml
  /// </summary>
  public partial class SplitWindowControl : INotifyPropertyChanged, ISplitWindowControl
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(SplitWindowControl));

    private NotifyTaskCompletion _notifyTaskCompletion;
    private int _index;
    private CancellationTokenSource _cts;

    private readonly IHistory<HistoryData> _searchHistoryController;
    private readonly IFindController _searchController;
    private readonly IPreventMessageFlood _preventMessageFlood;
    private readonly IFindController _findController;
    private readonly List<LogEntry> _findWhatResults;
    private readonly IPlaySoundFile _playSoundFile;
    private LogEntry _findNextResult;
    private readonly List<Predicate<LogEntry>> _criteria = new List<Predicate<LogEntry>>();

    /// <summary>
    /// Configured sound file exists
    /// </summary>
    private readonly bool _soundFileExists;

    /// <summary>
    /// Splitter offset
    /// </summary>
    private const double Offset = 5;

    /// <summary>
    /// Last seen <see cref="LogEntry"/>
    /// </summary>
    private LogEntry _lastSeenEntry;

    /// <summary>
    /// Old StatusBar message
    /// </summary>
    private string _oldStatusBarMessage;

    #region RoutedEvents

    /// <summary>
    /// Clears ItemsSource event handler
    /// </summary>
    private static readonly RoutedEvent LinesRefreshTimeChangedRoutedEvent = EventManager.RegisterRoutedEvent(nameof(LinesRefreshTimeChangedRoutedEvent), RoutingStrategy.Bubble,
      typeof(LinesRefreshTimeChangedEventHandler), typeof(SplitWindowControl));

    /// <summary>
    /// Clears ItemsSource event
    /// </summary>
    public event LinesRefreshTimeChangedEventHandler LinesRefreshTimeChangedEvent
    {
      add => AddHandler(LinesRefreshTimeChangedRoutedEvent, value);
      remove => RemoveHandler(LinesRefreshTimeChangedRoutedEvent, value);
    }

    private static readonly RoutedEvent SelectedLinesChangedRoutedEvent = EventManager.RegisterRoutedEvent(nameof(SelectedLinesChangedEvent), RoutingStrategy.Bubble,
      typeof(RoutedEventHandler), typeof(SplitWindowControl));

    /// <summary>
    /// Selected lines changed event
    /// </summary>
    public event RoutedEventHandler SelectedLinesChangedEvent
    {
      add => AddHandler(SelectedLinesChangedRoutedEvent, value);
      remove => RemoveHandler(SelectedLinesChangedRoutedEvent, value);
    }

    #endregion

    #region Properties

    private HistoryData _searchHistory;

    /// <summary>
    /// Search history
    /// </summary>
    public HistoryData SearchHistory
    {
      get => _searchHistory;
      set
      {
        if ( value == _searchHistory )
          return;

        _searchHistory = value;
        OnPropertyChanged();
      }
    }

    private string _selectedSplitSearchItem;

    /// <summary>
    /// SelectedSplitSearch item
    /// </summary>
    public string SelectedSplitSearchItem
    {
      get => _selectedSplitSearchItem;
      set
      {
        _selectedSplitSearchItem = value;
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Highlight data <see cref="List{T}"/> of <see cref="TextHighlightData"/>
    /// </summary>
    public List<TextHighlightData> HighlightData
    {
      get;
      set;
    }

    private double _splitterPosition;

    /// <summary>
    /// Current splitter height
    /// </summary>
    public double SplitterPosition
    {
      get => _splitterPosition;
      set
      {
        if ( Equals(value, _splitterPosition) )
          return;

        if ( value + (Offset - 1) > CurrentHeight )
          return;

        _splitterPosition = value;
        OnPropertyChanged();
        SetSplitWindowItemSource();
      }
    }

    /// <summary>
    /// Current height
    /// </summary>
    public double CurrentHeight
    {
      get;
      set;
    }

    /// <summary>
    /// LogCollectionView <see cref="VsCollectionView{T}"/>
    /// </summary>
    public IVsCollectionView<LogEntry> LogCollectionView
    {
      get;
      set;
    }

    /// <summary>
    /// <see cref="ObservableCollection{T}"/> of <see cref="LogEntry"/>
    /// </summary>
    public ObservableCollection<LogEntry> FindWhatResults
    {
      get;
      private set;
    }

    /// <summary>
    /// <see cref="LogWindowSplitElement"/> <see cref="ListCollectionView"/> of <see cref="LogEntry"/>
    /// </summary>
    public ListCollectionView SplitCollectionView
    {
      get;
      set;
    }

    /// <summary>
    /// Lines read
    /// </summary>
    public int LinesRead => LogReaderService?.Index ?? 0;

    /// <summary>
    /// Selected lines
    /// </summary>
    public int SelectedLines => SplitterPosition <= 0 ? LogWindowMainElement.SelectedItems.Count : LogWindowSplitElement.SelectedItems.Count;

    /// <summary>
    /// Last visible <see cref="LogEntry"/> index
    /// </summary>
    public int LastVisibleLogEntryIndex
    {
      get;
      set;
    }

    private LogEntry _selectedItem;

    /// <summary>
    /// Selected <see cref="LogEntry"/> item
    /// </summary>
    public LogEntry SelectedItem
    {
      get => _selectedItem;
      set
      {
        if ( value == null )
          return;

        _selectedItem = value;
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// CacheManager interface
    /// </summary>
    public ICacheManager CacheManager
    {
      get;
      set;
    }

    /// <summary>
    /// <see cref="List{T}"/> of <see cref="MessageFloodData"/>
    /// </summary>
    public List<MessageFloodData> FloodData
    {
      get;
      set;
    }

    /// <summary>
    /// SelectedText
    /// </summary>
    public string SelectedText
    {
      get;
      set;
    }

    private Visibility _extendedToolbarVisibility;

    /// <summary>
    /// Extended toolbar visibility
    /// </summary>
    public Visibility ExtendedToolbarVisibility
    {
      get => _extendedToolbarVisibility;
      set
      {
        if ( value == _extendedToolbarVisibility )
          return;

        _extendedToolbarVisibility = value;
        OnPropertyChanged();

        SplitElementFilterText = string.Empty;
        TextBoxHasFocus = _extendedToolbarVisibility == Visibility.Visible;
      }
    }

    private bool _textBoxHasFocus;

    /// <summary>
    /// TextBox has focus
    /// </summary>
    public bool TextBoxHasFocus
    {
      get => _textBoxHasFocus;
      set
      {
        if ( value == _textBoxHasFocus )
          return;

        _textBoxHasFocus = value;
        OnPropertyChanged();
      }
    }

    private bool _splitElementFilterByBookmark;

    /// <summary>
    /// <see cref="LogWindowSplitElement"/> filtered by Bookmark
    /// </summary>
    public bool SplitElementFilterByBookmark
    {
      get => _splitElementFilterByBookmark;
      set
      {
        if ( value == _splitElementFilterByBookmark )
          return;

        _splitElementFilterByBookmark = value;
        SplitCollectionView.Filter = SplitElementDynamicFilter;
        OnPropertyChanged();
      }
    }

    private string _splitElementFilterText;

    /// <summary>
    /// <see cref="LogWindowSplitElement"/> filtered by Text
    /// </summary>
    public string SplitElementFilterText
    {
      get => _splitElementFilterText;
      set
      {
        if ( Equals(value, _splitElementFilterText) )
          return;

        _splitElementFilterText = value;
        OnPropertyChanged();

        if ( SplitCollectionView == null )
          return;

        _criteria.Clear();

        if ( string.IsNullOrWhiteSpace(_splitElementFilterText) )
        {
          SplitCollectionView.Filter = SplitElementDynamicFilter;
          return;
        }

        _criteria.Add(p => !string.IsNullOrWhiteSpace(p.Message) && p.Message.ToLower().Contains(_splitElementFilterText));
        SplitCollectionView.Filter = SplitElementDynamicFilter;
      }
    }

    /// <summary>
    /// Gets current Bookmark count
    /// </summary>
    public int BookmarkCount => LogWindowMainElement.BookmarkCount;

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public SplitWindowControl()
    {
      InitializeComponent();

      DataContext = this;
      FloodData = new List<MessageFloodData>();
      LogCollectionView = new VsCollectionView<LogEntry>();

      _findWhatResults = new List<LogEntry>();
      CacheManager = new CacheManager();
      _playSoundFile = new PlaySoundFile();
      _soundFileExists = _playSoundFile.InitSoundPlay(SettingsHelperController.CurrentSettings.AlertSettings.SoundFileNameFullPath);

      InitCollectionView();

      _searchHistoryController = new HistoryController();
      _searchController = new FindController();
      _preventMessageFlood = new PreventMessageFlood();
      _findController = new FindController();

      ExtendedToolbarVisibility = Visibility.Collapsed;

      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<FindWhatChangedClosedMessage>(OnFindWhatChangedOrClosed);

      ((AsyncCommand<object>) LoadedCommand).PropertyChanged += LoadedPropertyChanged;
      ((AsyncCommand<object>) SplitSearchKeyDownCommand).PropertyChanged += OnSplitSearchKeyDownCommandPropertyChanged;
    }

    /// <summary>
    /// Initialize the CollectionView
    /// </summary>
    public void InitCollectionView()
    {
      LogCollectionView.FilteringStarted += OnFilteringStarted;
      LogCollectionView.FilteringCompleted += OnFilteringCompleted;
      LogCollectionView.FilteringErrorOccurred += OnFilteringErrorOccurred;

      LogWindowMainElement.ItemsSource = LogCollectionView.Items;
      LogCollectionView.Filter = DynamicFilterAsync;
    }

    private void OnFilteringStarted(object sender, EventArgs e) =>
      LOG.Debug("Filtering startet");

    private void OnFilteringCompleted(object sender, Core.Collections.FilterCollections.FilterEventArgs e) =>
      LOG.Debug($"Filtering completed {e.IsCompleted}, elapsed time {e.ElapsedTime} ms");

    private void OnFilteringErrorOccurred(object sender, Core.Collections.FilterCollections.FilterEventArgs e)
    {
      InteractionService.ShowErrorMessageBox(e.Exception.Message);
      LOG.Error(e.Exception, "Filtering caused a(n) {0}, elapsed time {1}", e.Exception.GetType().Name, e.ElapsedTime);
    }

    #region Dependency properties

    /// <summary>
    /// LogReaderService property
    /// </summary>
    public static readonly DependencyProperty LogReaderServiceProperty = DependencyProperty.Register(nameof(LogReaderService), typeof(ILogReadService), typeof(SplitWindowControl),
      new PropertyMetadata(null, OnLogReaderServiceChanged));

    /// <summary>
    /// LogReaderService
    /// </summary>
    public ILogReadService LogReaderService
    {
      get => (ILogReadService) GetValue(LogReaderServiceProperty);
      set => SetValue(LogReaderServiceProperty, value);
    }

    private TailData _currentTailData;

    /// <summary>
    /// Current <see cref="TailData"/>
    /// </summary>
    public TailData CurrentTailData
    {
      get => _currentTailData;
      private set
      {
        if ( _currentTailData != null )
          _currentTailData.PropertyChanged -= OnTailDataPropertyChanged;

        _currentTailData = value;
        LogWindowMainElement.CurrentTailData = value;
        LogWindowSplitElement.CurrentTailData = value;

        _currentTailData.PropertyChanged += OnTailDataPropertyChanged;
        OnPropertyChanged();
      }
    }

    #endregion

    #region Callback functions

    private static void OnLogReaderServiceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if ( !(d is SplitWindowControl sender) )
        return;

      if ( e.OldValue is ILogReadService reader )
      {
        reader.OnLogEntryCreated -= sender.OnLogEntryCreated;

        if ( reader.SmartWatch != null )
          reader.SmartWatch.SmartWatchFileChanged -= sender.OnSmartWatchFileChanged;
      }

      if ( sender.LogReaderService == null )
        return;

      sender.LogReaderService.OnLogEntryCreated += sender.OnLogEntryCreated;

      if ( sender.LogReaderService.SmartWatch != null )
        sender.LogReaderService.SmartWatch.SmartWatchFileChanged += sender.OnSmartWatchFileChanged;
    }

    private void OnSmartWatchFileChanged(object sender, string file)
    {
      if ( !(sender is SmartWatchController) || Dispatcher == null )
        return;

      Dispatcher.Invoke(() =>
      {
        switch ( SettingsHelperController.CurrentSettings.SmartWatchSettings.Mode )
        {
        case ESmartWatchMode.Auto:

          OnSmartWatchWindowClosed(this, new SmartWatchWindowClosedEventArgs(SettingsHelperController.CurrentSettings.SmartWatchSettings.NewTab, file));
          break;

        case ESmartWatchMode.Manual:

          var windows = this.Ancestors().OfType<Window>().ToList();
          var smartWatchPopup = new SmartWatchPopup
          {
            CurrentTailData = CurrentTailData,
            FileName = file,
            ShouldClose = true,
            MainWindow = windows.FirstOrDefault()
          };
          smartWatchPopup.SmartWatchPopupViewModel.SmartWatchWindowClosed += OnSmartWatchWindowClosed;
          smartWatchPopup.Show();
          break;

        default:

          throw new ArgumentOutOfRangeException();
        }
      });
    }

    private void OnSmartWatchWindowClosed(object sender, SmartWatchWindowClosedEventArgs e)
    {
      var logWindow = this.Ancestors().OfType<ILogWindowControl>().ToList();

      if ( logWindow.Count == 0 || !(CurrentTailData.Clone() is TailData smartWatchObject) )
        return;

      smartWatchObject.FileName = e.FileName;
      smartWatchObject.CommitChanges();

      if ( e.NewTabWindow )
      {
        logWindow.First().ExecuteStopTailCommand();
        EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenTailDataAsNewTabItem(this, smartWatchObject, logWindow.First().ParentWindowId,
          logWindow.First().WindowId, true));
      }
      else
      {
        EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenTailDataMessage(this, smartWatchObject, logWindow.First().ParentWindowId,
          logWindow.First().WindowId, true));
      }
    }

    private void OnLogEntryCreated(object sender, LogEntryCreatedArgs e)
    {
      if ( !(sender is ILogReadService) || Dispatcher == null )
        return;

      Dispatcher.Invoke(() =>
      {
        if ( LogCollectionView.Items == null )
          return;

        SetupCache();
        LogCollectionView.AddRange(e.Log);

        if ( SettingsHelperController.CurrentSettings.LogLineLimit != -1 &&
             LogCollectionView.Count >= SettingsHelperController.CurrentSettings.LogLineLimit &&
             _splitterPosition <= 0 )
        {
          var toRemove = LogCollectionView.Items.Take(LogCollectionView.Count - SettingsHelperController.CurrentSettings.LogLineLimit).ToArray();

          for ( int i = toRemove.Length - 1; i >= 0; i-- )
          {
            var item = toRemove[i];
            LogCollectionView.Remove(item);
          }
        }

        RaiseEvent(new LinesRefreshTimeChangedArgs(LinesRefreshTimeChangedRoutedEvent, LinesRead, e.SizeRefreshTime));
      }, DispatcherPriority.Background);
    }

    #endregion

    /// <summary>
    /// Updates LogWindowListBox"
    /// </summary>
    /// <param name="tailData"><see cref="TailData"/></param>
    public void UpdateTailData(TailData tailData) => CurrentTailData = tailData;

    /// <summary>
    /// <see cref="TailData"/> property changed
    /// </summary>
    /// <param name="sender">Who sends the event</param>
    /// <param name="e"><see cref="PropertyChangedEventArgs"/></param>
    private void OnTailDataPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      switch ( e.PropertyName )
      {
      case "FilterState":
      case "FilterItem":
      case "ListOfFilter":

        MouseService.SetBusyState();

        // I know, I will break the current T4W convention, that only the T4WindowViewModel is responsible for the StatusBar
        _oldStatusBarMessage = BaseWindowStatusbarViewModel.Instance.CurrentBusyState;
        BaseWindowStatusbarViewModel.Instance.CurrentBusyState = Application.Current.TryFindResource("Busy").ToString();

        // ReSharper disable once ObjectCreationAsStatement
        new DispatcherTimer(TimeSpan.FromSeconds(0),
          DispatcherPriority.ApplicationIdle,
          DispatcherTimerTick,
          Application.Current.Dispatcher ?? throw new InvalidOperationException());

        HighlightData = null;
        OnPropertyChanged(nameof(HighlightData));

        new ThrottledExecution().InMs(15).Do(() =>
          {
            Dispatcher?.Invoke(() =>
            {
              LogWindowMainElement.RemoveAllBookmarks();
              EnvironmentContainer.Instance.BookmarkManager.ClearBookmarkDataSource();

              LogCollectionView.Filter = DynamicFilterAsync;
              LogWindowMainElement.ScrollToEnd();
            });
          });
        break;

      case "Wrap":

        LogWindowMainElement.ScrollToEnd();
        break;
      }
    }

    private void DispatcherTimerTick(object sender, EventArgs e)
    {
      if ( !(sender is DispatcherTimer dispatcherTimer) )
        return;

      dispatcherTimer.Stop();
      BaseWindowStatusbarViewModel.Instance.CurrentBusyState = _oldStatusBarMessage;
      _oldStatusBarMessage = string.Empty;
    }

    #region Commands

    private IAsyncCommand _loadedCommand;

    /// <summary>
    /// Loaded command
    /// </summary>
    public IAsyncCommand LoadedCommand => _loadedCommand ?? (_loadedCommand = AsyncCommand.Create((p, t) => ExecuteLoadedCommandAsync()));

    private ICommand _unloadedCommand;

    /// <summary>
    /// Unloaded command
    /// </summary>
    public ICommand UnloadedCommand => _unloadedCommand ?? (_unloadedCommand = new RelayCommand(p => ExecuteUnloadedCommand()));

    private ICommand _sizeChangedCommand;

    /// <summary>
    /// Size changed command
    /// </summary>
    public ICommand SizeChangedCommand => _sizeChangedCommand ?? (_sizeChangedCommand = new RelayCommand(p => ExecuteSizeChangedCommand((SizeChangedEventArgs) p)));

    private ICommand _clearItemsCommand;

    /// <summary>
    /// Clear items
    /// </summary>
    public ICommand ClearItemsCommand => _clearItemsCommand ?? (_clearItemsCommand = new RelayCommand(p => ExecuteClearItemsCommand()));

    private ICommand _addBookmarkCommentCommand;

    /// <summary>
    /// Add bookmark comment command
    /// </summary>
    public ICommand AddBookmarkCommentCommand => _addBookmarkCommentCommand ?? (_addBookmarkCommentCommand = new RelayCommand(ExecuteAddBookmarkCommentCommand));

    private ICommand _closeExtendedToolbarCommand;

    /// <summary>
    /// Close extended toolbar command
    /// </summary>
    public ICommand CloseExtendedToolbarCommand => _closeExtendedToolbarCommand ?? (_closeExtendedToolbarCommand = new RelayCommand(p => ExecuteCloseExtendedToolbarCommand()));

    private IAsyncCommand _splitSearchKeyDownCommand;

    /// <summary>
    /// SplitSearchKeyDown command
    /// </summary>
    public IAsyncCommand SplitSearchKeyDownCommand =>
      _splitSearchKeyDownCommand ?? (_splitSearchKeyDownCommand = AsyncCommand.Create((p, t) => ExecuteSplitSearchKeyDownCommandAsync(p)));

    private ICommand _selectedLinesChangedInSpitControlCommand;

    /// <summary>
    /// Selected lines changed in split control
    /// </summary>
    public ICommand SelectedLinesChangedInSplitControlCommand =>
      _selectedLinesChangedInSpitControlCommand ?? (_selectedLinesChangedInSpitControlCommand = new RelayCommand(ExecuteSelectedLinesChangedInSplitControlCommand));

    private ICommand _selectedLinesChangedCommand;

    /// <summary>
    /// Selected lines changed command
    /// </summary>
    public ICommand SelectedLinesChangedCommand =>
      _selectedLinesChangedCommand ?? (_selectedLinesChangedCommand = new RelayCommand(ExecuteSelectedLinesChangedCommand));

    #endregion

    #region Command functions

    private void ExecuteSelectedLinesChangedInSplitControlCommand(object args)
    {
      if ( SplitterPosition <= 0 )
        return;

      if ( !(args is RoutedEventArgs e) )
        return;

      RaiseEvent(new RoutedEventArgs(SelectedLinesChangedRoutedEvent, (int) e.OriginalSource));
    }

    private void ExecuteSelectedLinesChangedCommand(object args)
    {
      if ( !(args is RoutedEventArgs e) )
        return;

      RaiseEvent(new RoutedEventArgs(SelectedLinesChangedRoutedEvent, (int) e.OriginalSource));
    }

    private async Task ExecuteSplitSearchKeyDownCommandAsync(object param)
    {
      if ( !(param is KeyEventArgs e) )
        return;

      if ( string.IsNullOrWhiteSpace(SplitElementFilterText) || e.Key != Key.Enter )
        return;

      MouseService.SetBusyState();

      await _searchHistoryController.UpdateHistoryAsync(_searchHistory, SplitElementFilterText, _cts.Token).ContinueWith(p =>
      {
        if ( !p.Result )
        {
          InteractionService.ShowErrorMessageBox(Application.Current.TryFindResource("HistoryUpdateError").ToString());
        }

        _searchHistory = _searchHistoryController.ReadHistoryAsync(_cts.Token).Result;
      }, TaskContinuationOptions.OnlyOnRanToCompletion).ConfigureAwait(false);
    }

    private void ExecuteCloseExtendedToolbarCommand()
    {
      SplitElementCheckBoxScrollToItemsEnd.IsChecked = false;
      ExtendedToolbarVisibility = Visibility.Collapsed;
    }

    private void ExecuteAddBookmarkCommentCommand(object args)
    {
      if ( !(args is RoutedEventArgs e) )
        return;

      if ( !(e.OriginalSource is LogEntry item) )
        return;

      var addBookmarkCommentPopup = new AddBookmarkComment
      {
        Owner = Window.GetWindow(this),
        Comment = item.BookmarkToolTip
      };
      addBookmarkCommentPopup.ShowDialog();

      item.BookmarkToolTip = addBookmarkCommentPopup.Comment;
      item.BookmarkPoint = BusinessHelper.CreateBitmapIcon(string.IsNullOrWhiteSpace(item.BookmarkToolTip) ?
        "/T4W;component/Resources/Bookmark.png" :
        "/T4W;component/Resources/Bookmark_Info.png");
    }

    private void ExecuteClearItemsCommand()
    {
      if ( LogCollectionView == null )
        LogCollectionView = new VsCollectionView<LogEntry>();

      LOG.Info("Clear items and cache");

      LogCollectionView.Clear();
      CacheManager.ClearCacheData();
      _findWhatResults.Clear();
      FindWhatResults?.Clear();
    }

    private async Task ExecuteLoadedCommandAsync()
    {
      _cts?.Dispose();
      _cts = new CancellationTokenSource();

      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<StartSearchAllMessage>(OnStartSearchAll);
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<JumpToSelectedLogEntryMessage>(OnJumpToSelectedLogEntry);
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<StartSearchCountMessage>(OnStartSearchCount);
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<StartSearchFindNextMessage>(OnStartSearchFindNext);
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<ShowExtendedToolbarMessage>(OnShowExtendedToolbar);

      EnvironmentContainer.Instance.BookmarkManager.OnIdChanged += OnBookmarkManagerIdChanged;
      EnvironmentContainer.Instance.BookmarkManager.OnBookmarkDataSourceChanged += OnBookmarkManagerDataSourceChanged;

      BookmarkManagerIdChanged(EnvironmentContainer.Instance.BookmarkManager.GetCurrentWindowId());
      OnBookmarkManagerDataSourceChanged(this, new IdChangedEventArgs(EnvironmentContainer.Instance.BookmarkManager.GetCurrentWindowId()));

      try
      {
        _searchHistory = await _searchHistoryController.ReadHistoryAsync(_cts.Token).ConfigureAwait(false);
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    private void ExecuteUnloadedCommand()
    {
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<StartSearchAllMessage>(OnStartSearchAll);
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<JumpToSelectedLogEntryMessage>(OnJumpToSelectedLogEntry);
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<StartSearchCountMessage>(OnStartSearchCount);
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<StartSearchFindNextMessage>(OnStartSearchFindNext);
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<ShowExtendedToolbarMessage>(OnShowExtendedToolbar);

      EnvironmentContainer.Instance.BookmarkManager.OnIdChanged -= OnBookmarkManagerIdChanged;
      EnvironmentContainer.Instance.BookmarkManager.OnBookmarkDataSourceChanged -= OnBookmarkManagerDataSourceChanged;

      _cts?.Cancel();
    }

    private void ExecuteSizeChangedCommand(SizeChangedEventArgs e)
    {
      // Calculate the distance position of GridSplitter
      double result = Math.Abs(SplitterPosition + Offset);

      if ( (int) result == (int) Offset )
        return;

      double percentage = (result * 100) / e.PreviousSize.Height;

      if ( percentage >= 100 )
      {
        SplitterPosition = CurrentHeight - Offset;
        return;
      }

      double distance = e.PreviousSize.Height - result;
      double newPosition = CurrentHeight - distance;

      if ( newPosition - Offset < 0 )
      {
        SplitterPosition = 0;
        return;
      }

      SplitterPosition = newPosition - Offset;
    }

    #endregion

    private void OnJumpToSelectedLogEntry(JumpToSelectedLogEntryMessage args)
    {
      if ( !IsRightWindow(args.WindowGuid) )
        return;

      ScrollToSelectedItem(args.SelectedLogEntry);
    }

    private void ScrollToSelectedItem(LogEntry logEntry)
    {
      if ( _splitterPosition <= 0 )
      {
        LogWindowMainElement.SelectedItem = logEntry;
        LogWindowMainElement.ScrollIntoView(logEntry);
        return;
      }

      LogWindowSplitElement.SelectedItem = logEntry;
      LogWindowSplitElement.ScrollIntoView(logEntry);
    }

    private void OnFindWhatChangedOrClosed(FindWhatChangedClosedMessage args) => RemoveFindWhatResultFromHighlightData();

    private void OnShowExtendedToolbar(ShowExtendedToolbarMessage args)
    {
      if ( !IsRightWindow(args.WindowGuid) || _splitterPosition <= 0 || ExtendedToolbarVisibility == Visibility.Visible )
        return;

      ExtendedToolbarVisibility = Visibility.Visible;
    }

    private void OnStartSearchFindNext(StartSearchFindNextMessage args)
    {
      if ( !IsRightWindow(args.WindowGuid) )
        return;

      _notifyTaskCompletion = NotifyTaskCompletion.Create(StartSearchingFindNextAsync(args.FindData, args.SearchText));
      _notifyTaskCompletion.PropertyChanged += FindNextPropertyChanged;
    }

    private void FindNextPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !(sender is NotifyTaskCompletion) || !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      UpdateHighlighting();

      if ( _notifyTaskCompletion == null )
        return;

      _notifyTaskCompletion.PropertyChanged -= FindWhatCountPropertyChanged;
      _notifyTaskCompletion = null;
    }

    private void OnStartSearchCount(StartSearchCountMessage args)
    {
      RemoveFindWhatResultFromHighlightData();

      if ( !IsRightWindow(args.WindowGuid) )
        return;

      _notifyTaskCompletion = NotifyTaskCompletion.Create(StartAllSearchingAsync(args.FindData, args.SearchText));
      _notifyTaskCompletion.PropertyChanged += FindWhatCountPropertyChanged;
    }

    private void FindWhatCountPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !(sender is NotifyTaskCompletion) || !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      var logWindow = this.Ancestors().OfType<ILogWindowControl>().ToList();

      if ( logWindow.Count == 0 )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new FindWhatCountResponseMessage(logWindow.First().WindowId, _findWhatResults.Count));

      if ( _notifyTaskCompletion == null )
        return;

      _notifyTaskCompletion.PropertyChanged -= FindWhatCountPropertyChanged;
      _notifyTaskCompletion = null;

      RemoveFindWhatResultFromHighlightData();
    }

    private void OnStartSearchAll(StartSearchAllMessage args)
    {
      RemoveFindWhatResultFromHighlightData();

      if ( !IsRightWindow(args.WindowGuid) )
        return;

      _notifyTaskCompletion = NotifyTaskCompletion.Create(StartAllSearchingAsync(args.FindData, args.SearchText));
      _notifyTaskCompletion.PropertyChanged += FindWhatPropertyChanged;
    }

    private void FindWhatPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !(sender is NotifyTaskCompletion) || !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      var logWindow = this.Ancestors().OfType<ILogWindowControl>().ToList();

      if ( logWindow.Count == 0 )
        return;

      FindWhatResults?.Clear();
      FindWhatResults = new ObservableCollection<LogEntry>(_findWhatResults);
      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenFindWhatResultWindowMessage(FindWhatResults, logWindow.First().WindowId));

      UpdateHighlighting();

      if ( _notifyTaskCompletion == null )
        return;

      _notifyTaskCompletion.PropertyChanged -= FindWhatPropertyChanged;
      _notifyTaskCompletion = null;
    }

    private async Task StartSearchingFindNextAsync(FindData findData, string searchText)
    {
      MouseService.SetBusyState();
      _findWhatResults.Clear();

      double startIndex = GetCurrentLogWindowIndex();
      double endIndex = SplitterPosition <= 0 ? LogWindowMainElement.GetViewportHeight() : LogWindowSplitElement.GetViewportHeight();
      var count = 0;

      while ( true )
      {
        // Nothing found, a complete loop run finished, break
        if ( count > 1 )
          break;

        // I.)
        // Look into visible items
        var result = await SearchInVisibleItemsAsync(startIndex, startIndex + endIndex, findData, searchText).ConfigureAwait(false);

        if ( result.Result )
          break;

        // II.)
        // Look into hidden items
        result = await SearchInHiddenItemsAsync(result.EndIndex, findData, searchText).ConfigureAwait(false);

        if ( result.Result )
          break;

        if ( !findData.Wrap )
          break;

        _findNextResult = null;

        if ( Dispatcher != null )
        {
          await Dispatcher?.InvokeAsync(() =>
          {
            if ( SplitterPosition <= 0 )
              LogWindowMainElement.ScrollToHome();
            else
              LogWindowSplitElement.ScrollToHome();
          }, DispatcherPriority.Normal);
        }

        startIndex = 0;
        endIndex = SplitterPosition <= 0 ? LogWindowMainElement.GetViewportHeight() : LogWindowSplitElement.GetViewportHeight();
        count++;
      }
    }

    private async Task<FindNextResult> SearchInVisibleItemsAsync(double start, double end, FindData findData, string searchText) =>
      await SearchInItemsAsync(start, end, findData, searchText).ConfigureAwait(false);

    private async Task<FindNextResult> SearchInHiddenItemsAsync(double start, FindData findData, string searchText) =>
      await SearchInItemsAsync(start, LogCollectionView.Count, findData, searchText).ConfigureAwait(false);

    private async Task<FindNextResult> SearchInItemsAsync(double start, double end, FindData findData, string searchText)
    {
      FindNextResult findNext = null;
      double stop = -1;
      int countTo = LogCollectionView.Count < (int) Math.Round(end) ? LogCollectionView.Count : (int) Math.Round(end);

      if ( start < 0 )
        return new FindNextResult(false, stop);

      if ( !findData.SearchBookmarks )
      {
        for ( var i = (int) Math.Round(start); i < countTo; i++ )
        {
          var log = LogCollectionView[i];

          // If list is filtered
          if ( !LogCollectionView.Contains(log) )
            continue;

          stop = i;
          string message = findData.SearchBookmarkComments ? log.BookmarkToolTip : log.Message;
          var result = await _findController.MatchTextAsync(findData, message, searchText).ConfigureAwait(false);

          if ( result == null || result.Count == 0 )
            continue;

          _findNextResult = log;
          findNext = new FindNextResult(true, log.Index);

          if ( findData.MarkLineAsBookmark )
            SetBookmarkFromFindWhat(log);

          AddFindWhatResultToHighlightData(result);

          if ( Dispatcher != null )
          {
#pragma warning disable _MissingConfigureAwait // Consider using .ConfigureAwait(false).
            await Dispatcher.InvokeAsync(() =>
            {
              ScrollToSelectedItem(_findNextResult);
            }, DispatcherPriority.Normal);
#pragma warning restore _MissingConfigureAwait // Consider using .ConfigureAwait(false).
          }
          break;
        }
      }
      else
      {
        var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        await Task.Run(() =>
        {
          for ( var i = (int) Math.Round(start); i < countTo; i++ )
          {
            var log = LogCollectionView[i];

            // If list is filtered
            if ( !LogCollectionView.Contains(log) )
              continue;

            stop = i;

            if ( log.BookmarkPoint == null )
              continue;

            _findNextResult = log;
            findNext = new FindNextResult(true, log.Index);

            Dispatcher?.Invoke(() =>
            {
              ScrollToSelectedItem(_findNextResult);
            }, DispatcherPriority.Normal, cts.Token);

            break;
          }
        }, cts.Token).ConfigureAwait(false);
      }
      return findNext ?? new FindNextResult(false, stop);
    }

    private double GetCurrentLogWindowIndex() =>
      SplitterPosition <= 0 ? _findNextResult?.Index ?? LogWindowMainElement.GetScrollViewerVerticalOffset() :
      _findNextResult?.Index ?? LogWindowSplitElement.GetScrollViewerVerticalOffset();

    private async Task StartAllSearchingAsync(FindData findData, string searchText)
    {
      MouseService.SetBusyState();
      _findWhatResults.Clear();

      if ( !findData.SearchBookmarks )
      {
        // ReSharper disable once ForCanBeConvertedToForeach
        for ( var i = 0; i < LogCollectionView.Count; i++ )
        {
          var log = LogCollectionView[i];

          // If list is filtered
          if ( !LogCollectionView.Contains(log) )
            continue;

          string message = findData.SearchBookmarkComments ? log.BookmarkToolTip : log.Message;
          var result = await _findController.MatchTextAsync(findData, message, searchText).ConfigureAwait(false);

          if ( result == null || result.Count == 0 )
            continue;

          if ( findData.MarkLineAsBookmark )
            SetBookmarkFromFindWhat(log);

          _findWhatResults.Add(log);
          AddFindWhatResultToHighlightData(result);
        }

        for ( int i = CacheManager.GetCacheData().Count - 1; i >= 0; i-- )
        {
          var log = CacheManager[i];

          if ( log == null )
            continue;

          string message = findData.SearchBookmarkComments ? log.BookmarkToolTip : log.Message;
          var result = await _findController.MatchTextAsync(findData, message, searchText).ConfigureAwait(false);

          if ( result == null || result.Count == 0 )
            continue;

          if ( findData.MarkLineAsBookmark )
            SetBookmarkFromFindWhat(log);

          _findWhatResults.Add(log);
          AddFindWhatResultToHighlightData(result);
        }
      }
      else
      {
        using ( var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5)) )
        {
          await Task.Run(() =>
          {
            var result = LogCollectionView.Items.Where(p => p.BookmarkPoint != null).ToList();

            // If list is filtered
            result = result.Where(p => LogCollectionView.Contains(p)).ToList();

            if ( result.Count > 0 )
              _findWhatResults.AddRange(result);

            result = CacheManager.GetCacheData().Where(p => p.BookmarkPoint != null).ToList();

            if ( result.Count > 0 )
              _findWhatResults.AddRange(result);

          }, cts.Token).ConfigureAwait(false);
        }
      }

      LOG.Trace($"Find all result count {_findWhatResults.Count}");
    }

    private void AddFindWhatResultToHighlightData(List<string> values)
    {
      if ( HighlightData == null )
        HighlightData = new List<TextHighlightData>();

      string words = string.Join("|", values);
      var inside = HighlightData.Where(i => Equals(i.Text, words) && !i.IsFindWhat).ToList();

      if ( inside.Count > 0 )
      {
        // Item is in HighlightData, backup old highlight color
        inside.ForEach(p =>
        {
          p.IsFindWhat = true;
          p.OldTextHighlightColorHex = p.TextHighlightColorHex;
          p.TextBackgroundColorHex = SettingsHelperController.CurrentSettings.ColorSettings.FindHighlightBackgroundColorHex;
          p.TextHighlightColorHex = SettingsHelperController.CurrentSettings.ColorSettings.FindHighlightForegroundColorHex;
          p.Opacity = 0.6;
        });
        return;
      }

      inside = HighlightData.Where(p => Equals(p.Text, words) && p.IsFindWhat).ToList();

      if ( inside.Count > 0 )
        return;

      HighlightData.Add(new TextHighlightData
      {
        IsFindWhat = true,
        Opacity = 0.6,
        Text = words,
        TextBackgroundColorHex = SettingsHelperController.CurrentSettings.ColorSettings.FindHighlightBackgroundColorHex,
        TextHighlightColorHex = SettingsHelperController.CurrentSettings.ColorSettings.FindHighlightForegroundColorHex
      });
    }

    private void RemoveFindWhatResultFromHighlightData()
    {
      if ( HighlightData == null || HighlightData.Count == 0 )
        return;

      var recover = HighlightData.Where(p => !string.IsNullOrWhiteSpace(p.OldTextHighlightColorHex) && p.IsFindWhat).ToList();

      // recover is empty, remove all FindWhat results from list
      if ( recover.Count == 0 )
      {
        HighlightData.ForEach(p =>
        {
          if ( !p.IsFindWhat )
            return;

          p.IsFindWhat = false;
          p.TextBackgroundColorHex = SettingsHelperController.CurrentSettings.ColorSettings.BackgroundColorHex;
          p.TextHighlightColorHex = SettingsHelperController.CurrentSettings.ColorSettings.ForegroundColorHex;
        });
        UpdateHighlighting();

        // Remove FindWhat data
        HighlightData.RemoveAll(p => p.IsFindWhat);
        OnPropertyChanged(nameof(HighlightData));
        UpdateHighlighting();
        return;
      }

      // Recover old highlight color
      recover.ForEach(p =>
      {
        p.IsFindWhat = false;
        p.TextHighlightColorHex = p.OldTextHighlightColorHex;
        p.OldTextHighlightColorHex = string.Empty;
        p.TextBackgroundColorHex = SettingsHelperController.CurrentSettings.ColorSettings.BackgroundColorHex;
        p.Opacity = 1;
      });

      // Remove FindWhat data
      HighlightData.RemoveAll(p => p.IsFindWhat);
      OnPropertyChanged(nameof(HighlightData));
      UpdateHighlighting();
    }

    private void UpdateHighlighting()
    {
      LogWindowMainElement.UpdateHighlighting(HighlightData);
      LogWindowSplitElement.UpdateHighlighting(HighlightData);
    }

    private bool SplitElementDynamicFilter(object item)
    {
      if ( !SplitElementFilterByBookmark && _criteria.Count == 0 )
        return true;

      if ( !(item is LogEntry logEntry) )
        return false;

      var result = false;

      if ( SplitElementFilterByBookmark )
      {
        if ( logEntry.BookmarkPoint != null )
          result = true;
        else
          return false;
      }

      if ( _criteria.Count == 0 )
        return result;

      if ( SplitElementFilterByBookmark )
        result &= _criteria.TrueForAll(p => p(logEntry));
      else
        result = _criteria.TrueForAll(p => p(logEntry));

      return result;
    }

    private async Task<bool> DynamicFilterAsync(object item, CancellationToken token)
    {
      if ( CurrentTailData?.ListOfFilter == null || CurrentTailData.ListOfFilter.Count == 0 || !CurrentTailData.FilterState )
        return true;

      if ( !(item is LogEntry logEntry) )
        return false;

      await LogCollectionView.CollectionViewLock.WaitAsync(token).ConfigureAwait(false);

      var result = false;
      var filterSource = CurrentTailData.ListOfFilter.Where(p => p.FilterSource && p.IsEnabled).ToList();
      var highlightSource = CurrentTailData.ListOfFilter.Where(p => p.IsHighlight && p.IsEnabled).ToList();

      // If no FilterSource is defined, we assume only Highlighting is active
      if ( filterSource.Count == 0 )
        result = true;

      try
      {
        foreach ( var filterData in filterSource )
        {
          try
          {
            var sr = await _searchController.MatchTextAsync(filterData.FindSettingsData, logEntry.Message, filterData.Filter).ConfigureAwait(false);
            token.ThrowIfCancellationRequested();

            if ( sr == null || sr.Count == 0 )
              continue;

            // Handle alert settings
            if ( filterData.UseNotification )
            {
              HandleAlertSettings(filterData, sr, logEntry);
            }

            token.ThrowIfCancellationRequested();

            // Handle AutoBookmark
            if ( filterData.IsAutoBookmark )
            {
              HandleAutoBookmark(filterData, logEntry);
            }

            token.ThrowIfCancellationRequested();
            result = true;
            break;
          }
          catch ( Exception ex )
          {
            LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
          }
        }

        // If result is false OR no highlighting is defined, return the current result
        if ( !result || highlightSource.Count == 0 )
          return result;

        foreach ( var filterData in highlightSource )
        {
          try
          {
            var sr = await _searchController
              .MatchTextAsync(filterData.FindSettingsData, logEntry.Message, filterData.Filter).ConfigureAwait(false);
            token.ThrowIfCancellationRequested();

            if ( sr == null || sr.Count == 0 )
              continue;

            // If no FilterSource is defined, handle alert settings here
            if ( filterSource.Count == 0 && filterData.UseNotification )
              HandleAlertSettings(filterData, sr, logEntry);

            token.ThrowIfCancellationRequested();

            // If not FilterSource is defined, handle AutoBookmark here
            if ( filterSource.Count == 0 && filterData.IsAutoBookmark )
            {
              HandleAutoBookmark(filterData, logEntry);
            }

            token.ThrowIfCancellationRequested();

            if ( HighlightData == null )
              HighlightData = new List<TextHighlightData>();

            token.ThrowIfCancellationRequested();

            if ( !filterData.IsEnabled )
            {
              // Remove disabled items from highlight list
              var toRemove = HighlightData
                .Where(p => string.Compare(p.Text, string.Join("|", sr), StringComparison.CurrentCultureIgnoreCase) == 0 && !p.IsFindWhat)
                .ToList();

              token.ThrowIfCancellationRequested();

              if ( toRemove.Count > 0 )
                HighlightData.RemoveAll(p => toRemove.Contains(p));

              continue;
            }

            // Is already inside highlight list?
            var inside = HighlightData
              .Where(p => string.Compare(p.Text, string.Join("|", sr), StringComparison.CurrentCultureIgnoreCase) == 0 && !p.IsFindWhat)
              .ToList();

            if ( inside.Count > 0 )
            {
              // Color changed?
              if ( inside.Where(p => Equals(p.TextHighlightColorHex, filterData.FilterColorHex)).ToList().Count > 0 )
                continue;

              token.ThrowIfCancellationRequested();
              HighlightData.RemoveAll(p => inside.Contains(p));
            }

            HighlightData
              .Add(new TextHighlightData { FilterFontType = filterData.FontType, TextHighlightColorHex = filterData.FilterColorHex, Text = string.Join("|", sr) });
          }
          catch ( Exception ex )
          {
            LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
          }
        }

        OnPropertyChanged(nameof(HighlightData));

        return true;
      }
      finally
      {
        LogCollectionView.CollectionViewLock.Release();
      }
    }

    private void HandleAutoBookmark(FilterData filterData, LogEntry item)
    {
      LOG.Debug("* * * * * * * * HandleAutoBookmark * * * * * * * *");

      Dispatcher?.Invoke(() =>
      {
        item.BookmarkPoint = BusinessHelper.CreateBitmapIcon("/T4W;component/Resources/Auto_Bookmark.png");
        item.BookmarkToolTip = string.IsNullOrWhiteSpace(filterData.AutoBookmarkComment) ? "Auto Bookmark" : filterData.AutoBookmarkComment;
        item.IsAutoBookmark = true;

        EnvironmentContainer.Instance.BookmarkManager.AddBookmarkItemsToSource(GetLogWindow().WindowId, item);
      }, DispatcherPriority.Background);
    }

    private void HandleAlertSettings(FilterData filter, IReadOnlyCollection<string> stringResult, LogEntry item)
    {
      if ( stringResult == null || stringResult.Count == 0 )
        return;

      // Jump out, if no alert settings set
      if ( !SettingsHelperController.CurrentSettings.AlertSettings.PopupWnd && !SettingsHelperController.CurrentSettings.AlertSettings.BringToFront &&
          !SettingsHelperController.CurrentSettings.AlertSettings.SendMail && !SettingsHelperController.CurrentSettings.AlertSettings.PlaySoundFile )
        return;

      if ( _preventMessageFlood.IsBusy )
      {
        FloodData.Add(new MessageFloodData
        {
          Filter = filter,
          LogEntry = item,
          Results = stringResult.ToList()
        });
        return;
      }

      if ( FloodData.Count > 0 )
      {
        HandleClearingFloodData();
        return;
      }

      if ( filter.UseNotification && SettingsHelperController.CurrentSettings.AlertSettings.PopupWnd )
        HandleNotification(item.DateTime, stringResult.ToArray());

      if ( SettingsHelperController.CurrentSettings.AlertSettings.BringToFront )
        EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new BringMainWindowToFrontMessage(this));

      if ( SettingsHelperController.CurrentSettings.AlertSettings.SendMail )
        HandleMailSend(stringResult.ToArray(), item);

      if ( SettingsHelperController.CurrentSettings.AlertSettings.PlaySoundFile && _soundFileExists && !_playSoundFile.IsPlaying() )
        _playSoundFile.Play(false);

      _preventMessageFlood.UpdateBusyState();
    }

    private void HandleClearingFloodData()
    {
      LOG.Info("Clearing flood data messages");

      if ( FloodData.First().Filter.UseNotification && SettingsHelperController.CurrentSettings.AlertSettings.PopupWnd )
        HandleNotification(FloodData.First().LogEntry.DateTime, FloodData.First().Results.ToArray());

      if ( SettingsHelperController.CurrentSettings.AlertSettings.PlaySoundFile && _soundFileExists && !_playSoundFile.IsPlaying() )
        _playSoundFile.Play(false);

      if ( SettingsHelperController.CurrentSettings.AlertSettings.BringToFront )
        EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new BringMainWindowToFrontMessage(this));

      if ( SettingsHelperController.CurrentSettings.AlertSettings.SendMail )
      {
        string messageTitle = Application.Current.TryFindResource("FilterManagerSendMailMessage").ToString();
        var msgBuild = new StringBuilder();

        foreach ( var flood in FloodData )
        {
          string detail = Application.Current.TryFindResource("FilterManagerSendMailDetail").ToString();
          msgBuild.Append(string.Format(detail, flood.LogEntry.Index, flood.LogEntry.Message, string.Join("\n\t", flood.Results)));
        }

        string mailMessage = string.Format(messageTitle, msgBuild);
        NotifyTaskCompletion.Create(HandleSendMailAsync(mailMessage));
      }

      FloodData.Clear();
      _preventMessageFlood.UpdateBusyState();
    }

    private void HandleMailSend(string[] notifications, LogEntry item)
    {
      if ( notifications == null || notifications.Length == 0 )
        return;

      string messageTitle = Application.Current.TryFindResource("FilterManagerSendMailMessage").ToString();
      string detail = Application.Current.TryFindResource("FilterManagerSendMailDetail").ToString();
      string mailMessage = string.Format(messageTitle, string.Format(detail, item.Index, item.Message, string.Join("\n\t", notifications)));

      NotifyTaskCompletion.Create(HandleSendMailAsync(mailMessage));
    }

    private Task HandleSendMailAsync(string message)
    {
      IMailController mailController = new MailController();
      return mailController.SendLogMailAsync(message);
    }

    /// <summary>
    /// Sets the notification message
    /// </summary>
    /// <param name="time"><see cref="DateTime"/> of message</param>
    /// <param name="notifications">List of matches</param>
    private void HandleNotification(DateTime time, string[] notifications)
    {
      if ( notifications == null || notifications.Length == 0 )
        return;

      string message = Application.Current.TryFindResource("FilterManagerNotificationInformation").ToString();

      Dispatcher?.Invoke(() =>
      {
        var alertPopUp = new FancyNotificationPopUp
        {
          Height = 100,
          Width = 300,
          PopUpAlert = CurrentTailData.File,
          PopUpAlertDetail = string.Format(message, time.ToString(SettingsHelperController.CurrentSettings.CurrentStringFormat), string.Join("\n\t", notifications))
        };
        EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new ShowNotificationPopUpMessage(alertPopUp));
      });
    }

    /// <summary>
    /// Unregister FindWhat changed or closed message
    /// </summary>
    public void UnregisterFindWhatChanged() => EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<FindWhatChangedClosedMessage>(OnFindWhatChangedOrClosed);

    /// <summary>
    /// GoToLine
    /// </summary>
    /// <param name="index">Index</param>
    public void GoToLine(int index)
    {
      if ( SplitterPosition <= 0 )
      {
        LogWindowMainElement.GoToItemByIndex(index);
        return;
      }

      LogWindowSplitElement.GoToItemByIndex(index);
    }

    /// <summary>
    /// Clears current items
    /// </summary>
    public void ClearItems()
    {
      ExecuteClearItemsCommand();
      LogReaderService.ResetIndex();
      RaiseEvent(new LinesRefreshTimeChangedArgs(LinesRefreshTimeChangedRoutedEvent, LinesRead, string.Empty));
    }

    private void SetupCache()
    {
      if ( !CacheManager.HaveToCache(LogCollectionView.Count) )
        return;

      LogEntry logEntry;

      if ( _splitterPosition <= 0 )
      {
        _index = 0;
        logEntry = LogCollectionView.Items.First();
      }
      else
      {
        logEntry = LogCollectionView[_index];
        _index++;
      }

      CacheManager.SetupCache(LogCollectionView.Count, logEntry, _splitterPosition);

      if ( _splitterPosition <= 0 )
        LogCollectionView.RemoveAt(0);
    }

    private void SetSplitWindowItemSource()
    {
      if ( _splitterPosition > 0 && LogWindowSplitElement.ItemsSource == null )
      {
        SplitCollectionView = (ListCollectionView) new CollectionViewSource { Source = LogCollectionView.Items }.View;
        LogWindowSplitElement.ItemsSource = SplitCollectionView;

        if ( _lastSeenEntry == null )
        {
          if ( LogCollectionView.Count < LastVisibleLogEntryIndex )
            LastVisibleLogEntryIndex = LogCollectionView.Count - 1;
          _lastSeenEntry = LogCollectionView[LastVisibleLogEntryIndex];
        }
        else
        {
          if ( SelectedItem != null )
            _lastSeenEntry = SelectedItem;
        }

        LogWindowSplitElement.ScrollIntoView(_lastSeenEntry);
      }
      else if ( _splitterPosition <= 0 && LogWindowSplitElement.ItemsSource != null )
      {
        LogWindowSplitElement.ItemsSource = null;
        FixLogEntries();
        RaiseEvent(new RoutedEventArgs(SelectedLinesChangedRoutedEvent, SelectedLines));
      }
    }

    private void FixLogEntries()
    {
      MouseService.SetBusyState();

      try
      {
        var result = CacheManager.GetIntersectData(LogCollectionView.Items);

        foreach ( var logEntry in result )
        {
          LogCollectionView.Remove(logEntry);
        }

        if ( SettingsHelperController.CurrentSettings.LogLineLimit == -1 || LogCollectionView.Count < SettingsHelperController.CurrentSettings.LogLineLimit )
          return;

        int count = LogCollectionView.Count - SettingsHelperController.CurrentSettings.LogLineLimit;

        for ( var i = 0; i < count; i++ )
        {
          LogCollectionView.RemoveAt(i);
        }

        CacheManager.FixCacheSize(LogCollectionView.Count);
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    private void SetBookmarkFromFindWhat(LogEntry log) =>
      Dispatcher?.Invoke(() =>
      {
        if ( log.BookmarkPoint != null )
          return;

        var bp = BusinessHelper.CreateBitmapIcon("/T4W;component/Resources/Bookmark.png");
        log.BookmarkPoint = bp;

        EnvironmentContainer.Instance.BookmarkManager.AddBookmarkItemsToSource(GetLogWindow().WindowId, log);
      }, DispatcherPriority.Normal);

    private ILogWindowControl GetLogWindow()
    {
      var logWindow = this.Ancestors().OfType<ILogWindowControl>().ToList();
      return logWindow.Count == 0 ? null : logWindow.FirstOrDefault();
    }

    private bool IsRightWindow(Guid windowGuid)
    {
      var window = GetLogWindow();
      return window != null && window.WindowId == windowGuid;
    }

    private void LoadedPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals(nameof(NotifyTaskCompletion.IsSuccessfullyCompleted)) )
        return;

      OnPropertyChanged(nameof(SearchHistory));
      NotifyTaskCompletion.Create(CacheManager.PrintCacheSizeAsync(_cts.Token));
    }

    private void OnSplitSearchKeyDownCommandPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals(nameof(NotifyTaskCompletion.IsSuccessfullyCompleted)) )
        return;

      OnPropertyChanged(nameof(SearchHistory));
    }

    #region BookmarkManager events

    private void OnBookmarkManagerIdChanged(object sender, IdChangedEventArgs e)
    {
      if ( !(sender is IBookmarkManager) )
        return;

      BookmarkManagerIdChanged(e.WindowId);
    }

    private void OnBookmarkManagerDataSourceChanged(object sender, IdChangedEventArgs e)
    {
      if ( !IsRightWindow(e.WindowId) )
        return;

      LogWindowSplitElement.BookmarkCount = EnvironmentContainer.Instance.BookmarkManager.BookmarkDataSource == null
        ? 0
        : EnvironmentContainer.Instance.BookmarkManager.Count;
      LogWindowMainElement.BookmarkCount = EnvironmentContainer.Instance.BookmarkManager.BookmarkDataSource == null
        ? 0
        : EnvironmentContainer.Instance.BookmarkManager.Count;
    }

    #endregion

    private void BookmarkManagerIdChanged(Guid windowId)
    {
      if ( !IsRightWindow(windowId) || LogCollectionView.Items == null )
        return;

      EnvironmentContainer.Instance.BookmarkManager.AddBookmarkItemsToSource(windowId, LogCollectionView.Items.Where(p => p?.BookmarkPoint != null).ToList());
    }

    #region PropertyChanged

    /// <summary>
    /// Declare the event
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// OnPropertyChanged
    /// </summary>
    /// <param name="name">Name of property</param>
    private void OnPropertyChanged([CallerMemberName] string name = null)
    {
      var handler = PropertyChanged;
      handler?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    #endregion
  }
}
