using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
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
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Extensions;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.Data.Messages;
using Org.Vs.TailForWin.Data.Messages.FindWhat;
using Org.Vs.TailForWin.Data.Messages.Keybindings;
using Org.Vs.TailForWin.PlugIns.BookmarkCommentModule;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Interfaces;
using Org.Vs.TailForWin.PlugIns.SmartWatchPopupModule;
using Org.Vs.TailForWin.UI.Extensions;
using Org.Vs.TailForWin.UI.UserControls;


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

    private readonly IXmlSearchHistory<IObservableDictionary<string, string>> _searchHistoryController;
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

    #endregion

    #region Properties

    private IObservableDictionary<string, string> _searchHistory;

    /// <summary>
    /// Search history
    /// </summary>
    public IObservableDictionary<string, string> SearchHistory
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

    private KeyValuePair<string, string> _selectedSplitSearchItem;

    /// <summary>
    /// SelectedSplitSearch item
    /// </summary>
    public KeyValuePair<string, string> SelectedSplitSearchItem
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
    public VsCollectionView<LogEntry> LogCollectionView
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
    /// <see cref="ListCollectionView"/> of <see cref="LogEntry"/>
    /// </summary>
    public ListCollectionView CollectionView
    {
      get;
      set;
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
      LogCollectionView = new VsCollectionView<LogEntry>();
      FloodData = new List<MessageFloodData>();

      _findWhatResults = new List<LogEntry>();
      CacheManager = new CacheManager();
      _playSoundFile = new PlaySoundFile();
      _soundFileExists = _playSoundFile.InitSoundPlay(SettingsHelperController.CurrentSettings.AlertSettings.SoundFileNameFullPath);

      InitCollectionView();

      _searchHistoryController = new XmlSearchHistoryController();
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
      CollectionView = (ListCollectionView) new CollectionViewSource { Source = LogCollectionView.Collection }.View;
      CollectionView.Filter = DynamicFilter;
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

    /// <summary>
    /// <see cref="TailData"/> property
    /// </summary>
    public static readonly DependencyProperty CurrentTailDataProperty = DependencyProperty.Register(nameof(CurrentTailData), typeof(TailData), typeof(SplitWindowControl),
      new PropertyMetadata(new TailData(), TailDataOnChanged));

    /// <summary>
    /// Current <see cref="TailData"/>
    /// </summary>
    public TailData CurrentTailData
    {
      get => (TailData) GetValue(CurrentTailDataProperty);
      set
      {
        SetValue(CurrentTailDataProperty, value);
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
      if ( !(sender is SmartWatchController) )
        return;

      Dispatcher.InvokeAsync(
        () =>
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
      if ( !(sender is ILogReadService) )
        return;

      Dispatcher.InvokeAsync(
        () =>
        {
          if ( LogCollectionView.Collection == null )
            return;

          if ( SettingsHelperController.CurrentSettings.LogLineLimit != -1 &&
               LogCollectionView.Collection.Count >= SettingsHelperController.CurrentSettings.LogLineLimit &&
               _splitterPosition <= 0 )
          {
            LogCollectionView.Collection.RemoveAt(0);
          }

          SetupCache();
          LogCollectionView.Collection.AddRange(e.Log);

          RaiseEvent(new LinesRefreshTimeChangedArgs(LinesRefreshTimeChangedRoutedEvent, LinesRead, e.SizeRefreshTime));
        }, DispatcherPriority.Background);
    }

    #endregion

    private static void TailDataOnChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      if ( !(sender is SplitWindowControl control) )
        return;

      if ( e.OldValue is TailData oldValue )
        oldValue.PropertyChanged -= control.CurrentTailDataChanged;

      if ( !(e.NewValue is TailData newValue) )
        return;

      control.LogWindowSplitElement.CurrentTailData = newValue;
      control.LogWindowMainElement.CurrentTailData = newValue;

      control.CurrentTailData.PropertyChanged += control.CurrentTailDataChanged;
    }

    private void CurrentTailDataChanged(object sender, PropertyChangedEventArgs e)
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
        new DispatcherTimer(TimeSpan.FromSeconds(0), DispatcherPriority.ApplicationIdle, DispatcherTimerTick, Application.Current.Dispatcher);

        HighlightData = null;
        OnPropertyChanged(nameof(HighlightData));

        new ThrottledExecution().InMs(15).Do(() =>
        {
          Dispatcher.InvokeAsync(() =>
          {
            LogWindowMainElement.RemoveAllBookmarks();
            EnvironmentContainer.Instance.BookmarkManager.ClearBookmarkDataSource();

            CollectionView.Filter = DynamicFilter;
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
    public IAsyncCommand SplitSearchKeyDownCommand => _splitSearchKeyDownCommand ?? (_splitSearchKeyDownCommand = AsyncCommand.Create((p, t) =>
                                                        ExecuteSplitSearchKeyDownCommandAsync(p)));

    #endregion

    #region Command functions

    private async Task ExecuteSplitSearchKeyDownCommandAsync(object param)
    {
      if ( !(param is KeyEventArgs e) )
        return;

      if ( string.IsNullOrWhiteSpace(SplitElementFilterText) || e.Key != Key.Enter )
        return;

      MouseService.SetBusyState();

      if ( !SearchHistory.ContainsKey(SplitElementFilterText.Trim()) )
      {
        _searchHistory.Add(new KeyValuePair<string, string>(SplitElementFilterText.Trim(), SplitElementFilterText.Trim()));
        await _searchHistoryController.SaveSearchHistoryAsync(SplitElementFilterText).ConfigureAwait(false);
      }
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
        "/T4W;component/Resources/Bookmark.png" : "/T4W;component/Resources/Bookmark_Info.png");
    }

    private void ExecuteClearItemsCommand()
    {
      if ( LogCollectionView == null )
        LogCollectionView = new VsCollectionView<LogEntry>();

      LOG.Trace("Clear items and cache");

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
        _searchHistory = await _searchHistoryController.ReadXmlFileAsync().ConfigureAwait(false);
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

      LogWindowMainElement.UpdateHighlighting(HighlightData);
      LogWindowSplitElement.UpdateHighlighting(HighlightData);

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

      LogWindowMainElement.UpdateHighlighting(HighlightData);
      LogWindowSplitElement.UpdateHighlighting(HighlightData);

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
        FindNextResult result = await SearchInVisibleItemsAsync(startIndex, startIndex + endIndex, findData, searchText).ConfigureAwait(false);

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

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        Dispatcher.InvokeAsync(() =>
        {
          if ( SplitterPosition <= 0 )
            LogWindowMainElement.ScrollToHome();
          else
            LogWindowSplitElement.ScrollToHome();
        }, DispatcherPriority.Normal);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        startIndex = 0;
        endIndex = SplitterPosition <= 0 ? LogWindowMainElement.GetViewportHeight() : LogWindowSplitElement.GetViewportHeight();
        count++;
      }
    }

    private async Task<FindNextResult> SearchInVisibleItemsAsync(double start, double end, FindData findData, string searchText) =>
      await SearchInItemsAsync(start, end, findData, searchText).ConfigureAwait(false);

    private async Task<FindNextResult> SearchInHiddenItemsAsync(double start, FindData findData, string searchText) =>
      await SearchInItemsAsync(start, LogCollectionView.Collection.Count, findData, searchText).ConfigureAwait(false);

    private async Task<FindNextResult> SearchInItemsAsync(double start, double end, FindData findData, string searchText)
    {
      FindNextResult findNext = null;
      double stop = -1;
      int countTo = LogCollectionView.Collection.Count < (int) Math.Round(end) ? LogCollectionView.Collection.Count : (int) Math.Round(end);

      if ( start < 0 )
        return new FindNextResult(false, stop);

      if ( !findData.SearchBookmarks )
      {
        for ( var i = (int) Math.Round(start); i < countTo; i++ )
        {
          LogEntry log = LogCollectionView.Collection[i];

          // If list is filtered
          if ( !CollectionView.Contains(log) )
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

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
          Dispatcher.InvokeAsync(() =>
          {
            ScrollToSelectedItem(_findNextResult);
          }, DispatcherPriority.Normal);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

          break;
        }
      }
      else
      {
        await Task.Run(() =>
        {
          for ( var i = (int) Math.Round(start); i < countTo; i++ )
          {
            LogEntry log = LogCollectionView.Collection[i];

            // If list is filtered
            if ( !CollectionView.Contains(log) )
              continue;

            stop = i;

            if ( log.BookmarkPoint == null )
              continue;

            _findNextResult = log;
            findNext = new FindNextResult(true, log.Index);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Dispatcher.InvokeAsync(() =>
            {
              ScrollToSelectedItem(_findNextResult);
            }, DispatcherPriority.Normal);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            break;
          }
        }, new CancellationTokenSource(TimeSpan.FromMinutes(2)).Token).ConfigureAwait(false);
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
        for ( int i = 0; i < LogCollectionView.Collection.Count; i++ )
        {
          LogEntry log = LogCollectionView.Collection[i];

          // If list is filtered
          if ( !CollectionView.Contains(log) )
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

        for ( int i = 0; i < CacheManager.GetCacheData().Count; i++ )
        {
          LogEntry log = CacheManager.GetCacheData()[i];
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
        await Task.Run(() =>
        {
          var result = LogCollectionView.Collection.Where(p => p.BookmarkPoint != null).ToList();

          // If list is filtered
          result = result.Where(p => CollectionView.Contains(p)).ToList();

          if ( result.Count > 0 )
            _findWhatResults.AddRange(result);

          result = CacheManager.GetCacheData().Where(p => p.BookmarkPoint != null).ToList();

          if ( result.Count > 0 )
            _findWhatResults.AddRange(result);

        }, new CancellationTokenSource(TimeSpan.FromMinutes(2)).Token).ConfigureAwait(false);
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
        // Remove FindWhat data
        HighlightData.RemoveAll(p => p.IsFindWhat);
        OnPropertyChanged(nameof(HighlightData));

        LogWindowMainElement.UpdateHighlighting(HighlightData);
        LogWindowSplitElement.UpdateHighlighting(HighlightData);
        return;
      }

      // Recover old highlight color
      recover.ForEach(p =>
      {
        p.IsFindWhat = false;
        p.TextHighlightColorHex = p.OldTextHighlightColorHex;
        p.OldTextHighlightColorHex = string.Empty;
        p.TextBackgroundColorHex = null;
        p.Opacity = 1;
      });

      // Remove FindWhat data
      HighlightData.RemoveAll(p => p.IsFindWhat);
      OnPropertyChanged(nameof(HighlightData));

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

    private bool DynamicFilter(object item)
    {
      if ( CurrentTailData.ListOfFilter == null || CurrentTailData.ListOfFilter.Count == 0 || !CurrentTailData.FilterState )
        return true;

      if ( !(item is LogEntry logEntry) )
        return false;

      var result = false;
      var filterSource = CurrentTailData.ListOfFilter.Where(p => p.FilterSource && p.IsEnabled).ToList();
      var highlightSource = CurrentTailData.ListOfFilter.Where(p => p.IsHighlight && p.IsEnabled).ToList();

      // If no FilterSource is defined, we assume only Highlighting is active
      if ( filterSource.Count == 0 )
        result = true;

      foreach ( FilterData filterData in filterSource )
      {
        try
        {
          var sr = _searchController.MatchTextAsync(filterData.FindSettingsData, logEntry.Message, filterData.Filter).GetAwaiter().GetResult();

          if ( sr == null || sr.Count == 0 )
            continue;

          // Handle alert settings
          if ( filterData.UseNotification )
            HandleAlertSettings(filterData, sr, logEntry);

          // Handle AutoBookmark
          if ( filterData.IsAutoBookmark )
            HandleAutoBookmark(filterData, logEntry);

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

#if DEBUG
      var sw = new Stopwatch();
      sw.Start();
#endif

      foreach ( FilterData filterData in highlightSource )
      {
        try
        {
          var sr = _searchController.MatchTextAsync(filterData.FindSettingsData, logEntry.Message, filterData.Filter).GetAwaiter().GetResult();

          if ( sr == null || sr.Count == 0 )
            continue;

          // If no FilterSource is defined, handle alert settings here
          if ( filterSource.Count == 0 && filterData.UseNotification )
            HandleAlertSettings(filterData, sr, logEntry);

          // If not FilterSource is defined, handle AutoBookmark here
          if ( filterSource.Count == 0 && filterData.IsAutoBookmark )
            HandleAutoBookmark(filterData, logEntry);

          if ( HighlightData == null )
            HighlightData = new List<TextHighlightData>();

          if ( !filterData.IsEnabled )
          {
            // Remove disabled items from highlight list
            var toRemove = HighlightData.Where(p => string.Compare(p.Text, string.Join("|", sr), StringComparison.CurrentCultureIgnoreCase) == 0 && !p.IsFindWhat).ToList();

            if ( toRemove.Count > 0 )
              HighlightData.RemoveAll(p => toRemove.Contains(p));

            continue;
          }

          // Is already inside highlight list?
          var inside = HighlightData.Where(p => string.Compare(p.Text, string.Join("|", sr), StringComparison.CurrentCultureIgnoreCase) == 0 && !p.IsFindWhat).ToList();

          if ( inside.Count > 0 )
          {
            // Color changed?
            if ( inside.Where(p => Equals(p.TextHighlightColorHex, filterData.FilterColorHex)).ToList().Count > 0 )
              continue;

            HighlightData.RemoveAll(p => inside.Contains(p));
          }

          HighlightData.Add(new TextHighlightData
          {
            FilterFontType = filterData.FontType,
            TextHighlightColorHex = filterData.FilterColorHex,
            Text = string.Join("|", sr)
          });
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        }
      }

#if DEBUG
      sw.Stop();
      //LOG.Debug($"Elapsed time after highlighting {sw.ElapsedTicks} ticks / {sw.ElapsedMilliseconds} ms");
#endif
      OnPropertyChanged(nameof(HighlightData));

      return true;
    }

    private void HandleAutoBookmark(FilterData filterData, LogEntry item)
    {
      item.BookmarkPoint = BusinessHelper.CreateBitmapIcon("/T4W;component/Resources/Auto_Bookmark.png");
      item.BookmarkToolTip = string.IsNullOrWhiteSpace(filterData.AutoBookmarkComment) ? "Auto Bookmark" : filterData.AutoBookmarkComment;

      EnvironmentContainer.Instance.BookmarkManager.AddBookmarkItemsToSource(GetLogWindow().WindowId, item);
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

        foreach ( MessageFloodData flood in FloodData )
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

    private async Task HandleSendMailAsync(string message)
    {
      IMailController mailController = new MailController();
      await mailController.SendLogMailAsync(message).ConfigureAwait(false);
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
      var alertPopUp = new FancyNotificationPopUp
      {
        Height = 100,
        Width = 300,
        PopUpAlert = CurrentTailData.File,
        PopUpAlertDetail = string.Format(message, time.ToString(SettingsHelperController.CurrentSettings.CurrentStringFormat), string.Join("\n\t", notifications))
      };
      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new ShowNotificationPopUpMessage(alertPopUp));
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
      if ( !CacheManager.HaveToCache(LogCollectionView.Collection.Count) )
        return;

      LogEntry logEntry;

      if ( _splitterPosition <= 0 )
      {
        _index = 0;
        logEntry = LogCollectionView.Collection.First();
      }
      else
      {
        logEntry = LogCollectionView.Collection[_index];
        _index++;
      }

      CacheManager.SetupCache(LogCollectionView.Collection.Count, logEntry, _splitterPosition);

      if ( _splitterPosition <= 0 )
        LogCollectionView.Collection.RemoveAt(0);
    }

    private void SetSplitWindowItemSource()
    {
      if ( _splitterPosition > 0 && LogWindowSplitElement.ItemsSource == null )
      {
        SplitCollectionView = (ListCollectionView) new CollectionViewSource { Source = LogCollectionView.Collection }.View;
        LogWindowSplitElement.ItemsSource = SplitCollectionView;

        if ( _lastSeenEntry == null )
        {
          if ( LogCollectionView.Collection.Count < LastVisibleLogEntryIndex )
            LastVisibleLogEntryIndex = LogCollectionView.Collection.Count - 1;

          _lastSeenEntry = LogCollectionView.Collection[LastVisibleLogEntryIndex];
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
      }
    }

    private void FixLogEntries()
    {
      MouseService.SetBusyState();

      try
      {
        var result = CacheManager.GetIntersectData(LogCollectionView.Collection);

        foreach ( var logEntry in result )
        {
          LogCollectionView.Collection.Remove(logEntry);
        }

        if ( SettingsHelperController.CurrentSettings.LogLineLimit == -1 || LogCollectionView.Collection.Count < SettingsHelperController.CurrentSettings.LogLineLimit )
          return;

        int count = LogCollectionView.Collection.Count - SettingsHelperController.CurrentSettings.LogLineLimit;

        for ( int i = 0; i < count; i++ )
        {
          LogCollectionView.Collection.RemoveAt(i);
        }

        CacheManager.FixCacheSize(LogCollectionView.Collection.Count);
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    private void SetBookmarkFromFindWhat(LogEntry log) =>
      Dispatcher.InvokeAsync(() =>
      {
        if ( log.BookmarkPoint != null )
          return;

        BitmapImage bp = BusinessHelper.CreateBitmapIcon("/T4W;component/Resources/Bookmark.png");
        log.BookmarkPoint = bp;
      }, DispatcherPriority.Normal);

    private ILogWindowControl GetLogWindow()
    {
      var logWindow = this.Ancestors().OfType<ILogWindowControl>().ToList();
      return logWindow.Count == 0 ? null : logWindow.FirstOrDefault();
    }

    private bool IsRightWindow(Guid windowGuid)
    {
      ILogWindowControl window = GetLogWindow();
      return window != null && window.WindowId == windowGuid;
    }

    private void LoadedPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      OnPropertyChanged(nameof(SearchHistory));
      NotifyTaskCompletion.Create(CacheManager.PrintCacheSizeAsync(_cts.Token));
    }

    private void OnSplitSearchKeyDownCommandPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
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

      LogWindowSplitElement.BookmarkCount = EnvironmentContainer.Instance.BookmarkManager.BookmarkDataSource == null ? 0 :
        EnvironmentContainer.Instance.BookmarkManager.BookmarkDataSource.Count;
      LogWindowMainElement.BookmarkCount = EnvironmentContainer.Instance.BookmarkManager.BookmarkDataSource == null ? 0 :
        EnvironmentContainer.Instance.BookmarkManager.BookmarkDataSource.Count;
    }

    #endregion

    private void BookmarkManagerIdChanged(Guid windowId)
    {
      if ( !IsRightWindow(windowId) || LogCollectionView.Collection == null )
        return;

      EnvironmentContainer.Instance.BookmarkManager.AddBookmarkItemsToSource(windowId, LogCollectionView.Collection.Where(p => p?.BookmarkPoint != null).ToList());
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
      PropertyChangedEventHandler handler = PropertyChanged;
      handler?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    #endregion
  }
}
