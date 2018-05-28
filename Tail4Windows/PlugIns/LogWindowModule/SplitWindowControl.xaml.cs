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
using System.Windows.Threading;
using log4net;
using Org.Vs.TailForWin.Business.Data;
using Org.Vs.TailForWin.Business.Events.Args;
using Org.Vs.TailForWin.Business.Interfaces;
using Org.Vs.TailForWin.Business.SearchEngine.Controllers;
using Org.Vs.TailForWin.Business.SearchEngine.Interfaces;
using Org.Vs.TailForWin.Business.Services;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.Data.Messages;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Data;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Events.Args;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Events.Delegates;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Interfaces;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Utils;
using Org.Vs.TailForWin.UI.Commands;
using Org.Vs.TailForWin.UI.Interfaces;
using Org.Vs.TailForWin.UI.UserControls;


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
    private readonly IFindController _searchController;
    private readonly IPreventMessageFlood _preventMessageFlood;

    private MessageFloodData _highlight;

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
    /// <see cref="ListCollectionView"/> of <see cref="LogEntry"/>
    /// </summary>
    public ListCollectionView CollectionView
    {
      get;
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

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public SplitWindowControl()
    {
      InitializeComponent();

      DataContext = this;

      FloodData = new List<MessageFloodData>();
      CollectionView = (ListCollectionView) new CollectionViewSource { Source = LogEntries }.View;
      CollectionView.Filter = DynamicFilter;
      CacheManager = new CacheManager();

      _searchController = new FindController();
      _preventMessageFlood = new PreventMessageFlood();

      LogWindowMainElement.ItemVisibleChanged += OnNewItemIsVisibleChanged;
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

      LOG.Trace("Clear items and cache");

      LogEntries.Clear();
      CacheManager.ClearCacheData();
    }

    private async Task ExecuteLoadedCommandAsync()
    {
      _cts?.Dispose();
      _cts = new CancellationTokenSource();

      await CacheManager.PrintCacheSizeAsync(_cts.Token).ConfigureAwait(false);
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

    private bool DynamicFilter(object item)
    {
      if ( CurrentTailData.ListOfFilter == null || CurrentTailData.ListOfFilter.Count == 0 || !CurrentTailData.FilterState )
        return true;

      if ( !(item is LogEntry logEntry) )
        return false;

      bool result = false;

      foreach ( var filterData in CurrentTailData.ListOfFilter )
      {
        try
        {
          var stringResult = _searchController.MatchTextAsync(filterData.FindSettingsData, logEntry.Message, filterData.Filter, _cts.Token).GetAwaiter().GetResult();

          if ( (stringResult == null || stringResult.Count == 0) && !filterData.IsHighlight )
            continue;

          if ( filterData.IsHighlight )
          {
            _highlight = new MessageFloodData
            {
              Filter = filterData,
              Results = stringResult,
              LogEntry = logEntry
            };
          }

          HandleAlertSettings(filterData, stringResult, logEntry);

          result = true;
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        }
      }
      return result;
    }

    private void HandleAlertSettings(FilterData filter, IReadOnlyCollection<string> stringResult, LogEntry item)
    {
      if ( stringResult == null || stringResult.Count == 0 )
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
      else if ( FloodData.Count > 0 )
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

      _preventMessageFlood.UpdateBusyState();
    }

    private void HandleClearingFloodData()
    {
      if ( FloodData.First().Filter.UseNotification && SettingsHelperController.CurrentSettings.AlertSettings.PopupWnd )
        HandleNotification(FloodData.First().LogEntry.DateTime, FloodData.First().Results.ToArray());

      if ( SettingsHelperController.CurrentSettings.AlertSettings.BringToFront )
        EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new BringMainWindowToFrontMessage(this));

      string messageTitle = Application.Current.TryFindResource("FilterManagerSendMailMessage").ToString();
      var msgBuild = new StringBuilder();

      foreach ( var flood in FloodData )
      {
        string detail = Application.Current.TryFindResource("FilterManagerSendMailDetail").ToString();
        msgBuild.Append(string.Format(detail, flood.LogEntry.Index, flood.LogEntry.Message, string.Join("\n\t", flood.Results)));
      }

      string mailMessage = string.Format(messageTitle, msgBuild);

      NotifyTaskCompletion.Create(HandleSendMailAsync(mailMessage));

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

    private void OnNewItemIsVisibleChanged(object sender, EventArgs e)
    {
      if ( _highlight == null )
        return;

      if ( _highlight.Filter.IsHighlight )
        LogWindowMainElement.SetHighlightInTextBlock(_highlight.LogEntry, _highlight.Results);

      _highlight = null;
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
