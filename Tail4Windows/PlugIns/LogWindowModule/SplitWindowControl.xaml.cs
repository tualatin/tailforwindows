﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
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


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule
{
  /// <summary>
  /// Interaction logic for SplitWindowControl.xaml
  /// </summary>
  public partial class SplitWindowControl : INotifyPropertyChanged, ISplitWindowControl
  {
    private const double Offset = 5;

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

          if ( SettingsHelperController.CurrentSettings.LogLineLimit != -1 && LogEntries.Count >= SettingsHelperController.CurrentSettings.LogLineLimit )
            LogEntries.RemoveAt(0);

          LogEntries.Add(e.Log);
          RaiseEvent(new LinesRefreshTimeChangedArgs(LinesRefreshTimeChangedRoutedEvent, LinesRead, e.SizeRefreshTime));
        }, DispatcherPriority.Normal);
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

    private ICommand _loadedCommand;

    /// <summary>
    /// Loaded command
    /// </summary>
    public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new RelayCommand(p => ExecuteLoadedCommand()));

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

      LogEntries.Clear();
    }

    private void ExecuteLoadedCommand()
    {
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
