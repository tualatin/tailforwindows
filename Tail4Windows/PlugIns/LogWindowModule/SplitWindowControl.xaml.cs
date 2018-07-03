﻿using System;
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
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using log4net;
using Org.Vs.TailForWin.Business.SearchEngine.Controllers;
using Org.Vs.TailForWin.Business.SearchEngine.Interfaces;
using Org.Vs.TailForWin.Business.Services.Data;
using Org.Vs.TailForWin.Business.Services.Events.Args;
using Org.Vs.TailForWin.Business.Services.Interfaces;
using Org.Vs.TailForWin.Business.SmartWatchEngine.Controlleres;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Business.Utils.Interfaces;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
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
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.Data.Messages;
using Org.Vs.TailForWin.Data.Messages.FindWhat;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Interfaces;
using Org.Vs.TailForWin.PlugIns.SmartWatchPopupModule;
using Org.Vs.TailForWin.UI.Extensions;
using Org.Vs.TailForWin.UI.UserControls;
using Org.Vs.TailForWin.UI.Utils;


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
    private readonly IFindController _searchController;
    private readonly IPreventMessageFlood _preventMessageFlood;
    private readonly IFindController _findController;
    private readonly List<LogEntry> _findWhatResults;
    private readonly IPlaySoundFile _playSoundFile;
    private LogEntry _findNextResult;

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

    /// <summary>
    /// Search result
    /// </summary>
    public List<string> SearchResult
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
    /// <see cref="ObservableCollection{T}"/> of <see cref="LogEntry"/>
    /// </summary>
    public ObservableCollection<LogEntry> LogEntries
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

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public SplitWindowControl()
    {
      InitializeComponent();

      DataContext = this;
      FloodData = new List<MessageFloodData>();
      LogEntries = new ObservableCollection<LogEntry>();
      _findWhatResults = new List<LogEntry>();
      CacheManager = new CacheManager();
      _playSoundFile = new PlaySoundFile();
      _soundFileExists = _playSoundFile.InitSoundPlay(SettingsHelperController.CurrentSettings.AlertSettings.SoundFileNameFullPath);

      CollectionView = (ListCollectionView) new CollectionViewSource { Source = LogEntries }.View;
      CollectionView.Filter = DynamicFilter;

      _searchController = new FindController();
      _preventMessageFlood = new PreventMessageFlood();
      _findController = new FindController();
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
              CurrenTailData = CurrentTailData,
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

      if ( e.NewTabWindow )
      {
        logWindow.First().ExecuteStopTailCommand();
        EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenTailDataAsNewTabItem(this, smartWatchObject, logWindow.First().ParentWindowId, true));
      }
      else
      {
        EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenTailDataMessage(this, smartWatchObject, logWindow.First().ParentWindowId, true));
      }
    }

    private void OnLogEntryCreated(object sender, LogEntryCreatedArgs e)
    {
      if ( !(sender is ILogReadService) )
        return;

      Dispatcher.InvokeAsync(
        () =>
        {
          if ( LogEntries == null )
            return;

          if ( SettingsHelperController.CurrentSettings.LogLineLimit != -1 && LogEntries.Count >= SettingsHelperController.CurrentSettings.LogLineLimit && _splitterPosition <= 0 )
            LogEntries.RemoveAt(0);

          SetupCache();
          LogEntries.Add(e.Log);

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

        SearchResult = null;
        CollectionView.Filter = DynamicFilter;

        OnPropertyChanged(nameof(SearchResult));
        LogWindowMainElement.ScrollToEnd();
        break;

      case "Wrap":

        LogWindowMainElement.ScrollToEnd();
        break;
      }
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

    #endregion

    #region Command functions

    private void ExecuteClearItemsCommand()
    {
      if ( LogEntries == null )
        LogEntries = new ObservableCollection<LogEntry>();

      LOG.Trace("Clear items and cache");

      LogEntries.Clear();
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

      await CacheManager.PrintCacheSizeAsync(_cts.Token).ConfigureAwait(false);
    }

    private void ExecuteUnloadedCommand()
    {
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<StartSearchAllMessage>(OnStartSearchAll);
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<JumpToSelectedLogEntryMessage>(OnJumpToSelectedLogEntry);
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<StartSearchCountMessage>(OnStartSearchCount);
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<StartSearchFindNextMessage>(OnStartSearchFindNext);

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

    private void OnStartSearchFindNext(StartSearchFindNextMessage args)
    {
      if ( !IsRightWindow(args.WindowGuid) )
        return;

      _notifyTaskCompletion = NotifyTaskCompletion.Create(StartSearchingFindNexdAsync(args.FindData, args.SearchText));
      _notifyTaskCompletion.PropertyChanged += FindNextPropertyChanged;
    }

    private void FindNextPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !(sender is NotifyTaskCompletion) || !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      _notifyTaskCompletion.PropertyChanged -= FindWhatCountPropertyChanged;
      _notifyTaskCompletion = null;
    }

    private void OnStartSearchCount(StartSearchCountMessage args)
    {
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

      _notifyTaskCompletion.PropertyChanged -= FindWhatCountPropertyChanged;
      _notifyTaskCompletion = null;
    }

    private void OnStartSearchAll(StartSearchAllMessage args)
    {
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

      _notifyTaskCompletion.PropertyChanged -= FindWhatPropertyChanged;
      _notifyTaskCompletion = null;
    }

    private async Task StartSearchingFindNexdAsync(FindData findData, string searchText)
    {
      MouseService.SetBusyState();
      _findWhatResults.Clear();

      double startIndex = GetCurrentLogWindowIndex();
      double endIndex = SplitterPosition <= 0 ? LogWindowMainElement.GetViewportHeight() : LogWindowSplitElement.GetViewportHeight();

      while ( true )
      {
        // I.)
        // Look into visible items
        FindNextResult result = await SearchInVisibleItemsAsync(startIndex, startIndex + endIndex, findData, searchText).ConfigureAwait(false);

        if ( result.Result )
          break;

        // II.)
        // Look into hidden items
        result = await SearchInHiddentemsAsync(result.EndIndex, findData, searchText).ConfigureAwait(false);

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
      }
    }

    private async Task<FindNextResult> SearchInVisibleItemsAsync(double start, double end, FindData findData, string searchText) =>
      await SearchInItemsAsync(start, end, findData, searchText).ConfigureAwait(false);

    private async Task<FindNextResult> SearchInHiddentemsAsync(double start, FindData findData, string searchText) =>
      await SearchInItemsAsync(start, LogEntries.Count, findData, searchText).ConfigureAwait(false);

    private async Task<FindNextResult> SearchInItemsAsync(double start, double end, FindData findData, string searchText)
    {
      FindNextResult findNext = null;
      double stop = -1;
      int countTo = LogEntries.Count < (int) Math.Round(end) ? LogEntries.Count : (int) Math.Round(end);

      if ( start < 0 )
        return new FindNextResult(false, stop);

      if ( !findData.SearchBookmarks )
      {
        for ( var i = (int) Math.Round(start); i < countTo; i++ )
        {
          LogEntry log = LogEntries[i];

          // If list is filtered
          if ( !CollectionView.Contains(log) )
            continue;

          stop = i;
          var result = await _findController.MatchTextAsync(findData, log.Message, searchText).ConfigureAwait(false);

          if ( result == null || result.Count == 0 )
            continue;

          _findNextResult = log;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
          Dispatcher.InvokeAsync(() =>
          {
            ScrollToSelectedItem(_findNextResult);
          }, DispatcherPriority.Normal);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

          if ( findData.MarkLineAsBookmark )
            SetBookmarkFromFindWhat(log);

          findNext = new FindNextResult(true, log.Index);
          break;
        }
      }
      else
      {
        await Task.Run(() =>
        {
          for ( var i = (int) Math.Round(start); i < countTo; i++ )
          {
            LogEntry log = LogEntries[i];

            // If list is filtered
            if ( !CollectionView.Contains(log) )
              continue;

            stop = i;

            if ( log.BookmarkPoint == null )
              continue;

            _findNextResult = log;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Dispatcher.InvokeAsync(() =>
            {
              ScrollToSelectedItem(_findNextResult);
            }, DispatcherPriority.Normal);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            findNext = new FindNextResult(true, log.Index);
            break;
          }
        }, new CancellationTokenSource(TimeSpan.FromMinutes(2)).Token).ConfigureAwait(false);
      }
      return findNext ?? new FindNextResult(false, stop);
    }

    private double GetCurrentLogWindowIndex()
    {
      if ( SplitterPosition <= 0 )
        return _findNextResult?.Index ?? LogWindowMainElement.GetScrollViewerVerticalOffset();

      return _findNextResult?.Index ?? LogWindowSplitElement.GetScrollViewerVerticalOffset();
    }

    private async Task StartAllSearchingAsync(FindData findData, string searchText)
    {
      MouseService.SetBusyState();
      _findWhatResults.Clear();

      if ( !findData.SearchBookmarks )
      {
        // ReSharper disable once ForCanBeConvertedToForeach
        for ( int i = 0; i < LogEntries.Count; i++ )
        {
          LogEntry log = LogEntries[i];

          // If list is filtered
          if ( !CollectionView.Contains(log) )
            continue;

          var result = await _findController.MatchTextAsync(findData, log.Message, searchText).ConfigureAwait(false);

          if ( result == null || result.Count == 0 )
            continue;

          if ( findData.MarkLineAsBookmark )
            SetBookmarkFromFindWhat(log);

          _findWhatResults.Add(log);
        }

        for ( int i = 0; i < CacheManager.GetCacheData().Count; i++ )
        {
          LogEntry log = CacheManager.GetCacheData()[i];
          var result = await _findController.MatchTextAsync(findData, log.Message, searchText).ConfigureAwait(false);

          if ( result == null || result.Count == 0 )
            continue;

          if ( findData.MarkLineAsBookmark )
            SetBookmarkFromFindWhat(log);

          _findWhatResults.Add(log);
        }
      }
      else
      {
        await Task.Run(() =>
        {
          var result = LogEntries.Where(p => p.BookmarkPoint != null).ToList();

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

    private bool DynamicFilter(object item)
    {
      if ( CurrentTailData.ListOfFilter == null || CurrentTailData.ListOfFilter.Count == 0 || !CurrentTailData.FilterState )
        return true;

      if ( !(item is LogEntry logEntry) )
        return false;

      var result = false;
      var filterSource = CurrentTailData.ListOfFilter.Where(p => p.FilterSource).ToList();
      var highlightSource = CurrentTailData.ListOfFilter.Where(p => p.IsHighlight).ToList();

      // If no FilterSource is defined, we assume only Highlighting is active
      if ( filterSource.Count == 0 )
        result = true;

      foreach ( var filterData in filterSource )
      {
        try
        {
          var sr = _searchController.MatchTextAsync(filterData.FindSettingsData, logEntry.Message, filterData.Filter).GetAwaiter().GetResult();

          if ( sr == null || sr.Count == 0 )
            continue;

          HandleAlertSettings(filterData, sr, logEntry);

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
          var sr = _searchController.MatchTextAsync(filterData.FindSettingsData, logEntry.Message, filterData.Filter).GetAwaiter().GetResult();

          if ( sr == null || sr.Count == 0 )
            continue;

          // If no FilterSource is defined, handle all alert settings here
          if ( filterSource.Count == 0 )
            HandleAlertSettings(filterData, sr, logEntry);
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        }
      }
      return true;
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
    public void ClearItems() => ExecuteClearItemsCommand();

    private void SetupCache()
    {
      if ( !CacheManager.HaveToCache(LogEntries.Count) )
        return;

      LogEntry logEntry;

      if ( _splitterPosition <= 0 )
      {
        _index = 0;
        logEntry = LogEntries.First();
      }
      else
      {
        logEntry = LogEntries[_index];
        _index++;
      }

      CacheManager.SetupCache(LogEntries.Count, logEntry, _splitterPosition);

      if ( _splitterPosition <= 0 )
        LogEntries.RemoveAt(0);
    }

    private void SetSplitWindowItemSource()
    {
      if ( _splitterPosition > 0 && LogWindowSplitElement.ItemsSource == null )
      {
        LogWindowSplitElement.ItemsSource = LogEntries;

        if ( _lastSeenEntry == null )
        {
          if ( LogEntries.Count < LastVisibleLogEntryIndex )
            LastVisibleLogEntryIndex = LogEntries.Count - 1;

          _lastSeenEntry = LogEntries[LastVisibleLogEntryIndex];
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
        var result = CacheManager.GetIntersectData(LogEntries);

        foreach ( var logEntry in result )
        {
          LogEntries.Remove(logEntry);
        }

        if ( SettingsHelperController.CurrentSettings.LogLineLimit == -1 || LogEntries.Count < SettingsHelperController.CurrentSettings.LogLineLimit )
          return;

        int count = LogEntries.Count - SettingsHelperController.CurrentSettings.LogLineLimit;

        for ( int i = 0; i < count; i++ )
        {
          LogEntries.RemoveAt(i);
        }

        CacheManager.FixCacheSize(LogEntries.Count);
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    private void SetBookmarkFromFindWhat(LogEntry log)
    {
      Dispatcher.InvokeAsync(() =>
      {
        BitmapImage bp = BusinessHelper.CreateBitmapIcon("/T4W;component/Resources/Boomark.png");
        log.BookmarkPoint = bp;
      }, DispatcherPriority.Normal);
    }

    private bool IsRightWindow(Guid windowGuid)
    {
      var logWindow = this.Ancestors().OfType<ILogWindowControl>().ToList();
      return logWindow.Count != 0 && logWindow.First().WindowId == windowGuid;
    }

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
  }
}
