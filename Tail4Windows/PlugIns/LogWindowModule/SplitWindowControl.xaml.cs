using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using log4net;
using Org.Vs.TailForWin.Business.Data;
using Org.Vs.TailForWin.Business.Events.Args;
using Org.Vs.TailForWin.Business.Interfaces;
using Org.Vs.TailForWin.Business.Services;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Events.Args;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Events.Delegates;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Interfaces;
using Org.Vs.TailForWin.UI.Commands;
using Org.Vs.TailForWin.UI.Interfaces;
using Org.Vs.TailForWin.UI.Services;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule
{
  /// <summary>
  /// Interaction logic for SplitWindowControl.xaml
  /// </summary>
  public partial class SplitWindowControl : INotifyPropertyChanged, ISplitWindowControl
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(SplitWindowControl));

    private int _index;
    private CancellationTokenSource _cts;

    /// <summary>
    /// Splitter offset
    /// </summary>
    private const double Offset = 5;

    /// <summary>
    /// Max capacity of <see cref="LogEntries"/>
    /// </summary>
    private const int MaxCapacity = 6500;

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
    } = new ObservableCollection<LogEntry>();

    /// <summary>
    /// <see cref="CollectionViewSource"/> of <see cref="LogEntry"/>
    /// </summary>
    private CollectionViewSource CollectionView
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
    /// <see cref="LogEntry"/> cache
    /// </summary>
    public List<LogEntry> EntryCache
    {
      get;
      set;
    } = new List<LogEntry>();

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public SplitWindowControl()
    {
      InitializeComponent();

      DataContext = this;
      CollectionView = new CollectionViewSource
      {
        Source = LogEntries
      };

      LOG.Debug($"Current max logline capacity is {MaxCapacity}");
    }

    #region Dependency properties

    /// <summary>
    /// LogReaderService property
    /// </summary>
    public static readonly DependencyProperty LogReaderServiceProperty = DependencyProperty.Register(nameof(LogReaderService), typeof(LogReadService), typeof(SplitWindowControl),
      new PropertyMetadata(null, OnLogReaderServiceChanged));

    private static void OnLogReaderServiceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if ( !(d is SplitWindowControl sender) )
        return;

      sender.LogReaderService = e.NewValue as LogReadService;

      if ( sender.LogReaderService == null )
        return;

      sender.LogReaderService.OnLogEntryCreated += sender.OnLogEntryCreated;
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

    /// <summary>
    /// LogReaderService
    /// </summary>
    public LogReadService LogReaderService
    {
      get => (LogReadService) GetValue(LogReaderServiceProperty);
      set => SetValue(LogReaderServiceProperty, value);
    }

    /// <summary>
    /// <see cref="TailData"/> property
    /// </summary>
    public static readonly DependencyProperty CurrentTailDataProperty = DependencyProperty.Register(nameof(CurrentTailData), typeof(TailData), typeof(SplitWindowControl),
      new PropertyMetadata(new TailData()));

    /// <summary>
    /// Current <see cref="TailData"/>
    /// </summary>
    public TailData CurrentTailData
    {
      get => (TailData) GetValue(CurrentTailDataProperty);
      set => SetValue(CurrentTailDataProperty, value);
    }

    #endregion

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
      if ( EntryCache == null )
        EntryCache = new List<LogEntry>();

      LOG.Trace("Clear items and cache");

      LogEntries.Clear();
      EntryCache.Clear();
    }

    private async Task ExecuteLoadedCommandAsync()
    {
      _cts?.Dispose();
      _cts = new CancellationTokenSource();

      await PrintCacheSizeAsync().ConfigureAwait(false);
    }

    private async Task PrintCacheSizeAsync()
    {
      while ( !_cts.IsCancellationRequested )
      {
        await Task.Delay(TimeSpan.FromMinutes(10), _cts.Token).ConfigureAwait(false);
        LOG.Trace($"Current cache size is {EntryCache.Count}");
      }
    }

    private void ExecuteUnloadedCommand() => _cts.Cancel();

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

    private void SetupCache()
    {
      if ( LogEntries.Count <= MaxCapacity )
        return;

      if ( EntryCache.Count == 0 )
        LOG.Debug("Start caching...");

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

      if ( SettingsHelperController.CurrentSettings.LogLineLimit != -1 && EntryCache.Count + MaxCapacity >= SettingsHelperController.CurrentSettings.LogLineLimit
                                                                       && _splitterPosition <= 0 )
      {
        EntryCache.RemoveAt(0);
      }

      EntryCache.Add(logEntry);

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

        MouseService.SetBusyState();

        try
        {
          var result = EntryCache.Intersect(LogEntries);

          foreach ( var logEntry in result )
          {
            LogEntries.Remove(logEntry);
          }
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", ex.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
        }
      }
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
