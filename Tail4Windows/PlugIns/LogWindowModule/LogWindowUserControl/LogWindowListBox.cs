﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using log4net;
using Org.Vs.TailForWin.BaseView;
using Org.Vs.TailForWin.Business.Services.Data;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Data;
using Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.LogWindowUserControl.Interfaces;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Extensions;
using Org.Vs.TailForWin.Data.Messages;
using Org.Vs.TailForWin.Data.Messages.FindWhat;
using Org.Vs.TailForWin.Data.Messages.Keybindings;
using Org.Vs.TailForWin.PlugIns.FileManagerModule;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Interfaces;
using Org.Vs.TailForWin.Ui.Utils.Converters;
using Org.Vs.TailForWin.Ui.Utils.Extensions;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule.LogWindowUserControl
{
  /// <summary>
  /// LogWindow control
  /// </summary>
  public class LogWindowListBox : ListBox, INotifyPropertyChanged, ILogWindowListBox
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(LogWindowListBox));

    private ScrollViewer _scrollViewer;
    private Grid _splitGripControl;
    private bool _mouseDown;
    private bool _isMouseLeftDownClick;
    private bool _isMouseRightDownClick;
    private bool _isMouseDoubleClick;

    private TextBlock _defaultTextMessage;
    private TextBox _readOnlyTextMessage;

    private ContextMenu _readOnlyTextBoxContextMenu;
    private readonly StringToWindowMediaBrushConverter _stringToBrushConverter;

    #region Public properties

    /// <summary>
    /// Bookmark image size property
    /// </summary>
    public static readonly DependencyProperty BookmarkImageSizeProperty =
      DependencyProperty.Register(nameof(BookmarkImageSize), typeof(double), typeof(LogWindowListBox), new PropertyMetadata(16d));

    /// <summary>
    /// Bookmark image size
    /// </summary>
    public double BookmarkImageSize
    {
      get => (double) GetValue(BookmarkImageSizeProperty);
      set => SetValue(BookmarkImageSizeProperty, value);
    }

    /// <summary>
    /// ShowGridSplitControl property
    /// </summary>
    public static readonly DependencyProperty ShowGridSplitControlProperty =
      DependencyProperty.Register(nameof(ShowGridSplitControl), typeof(bool), typeof(LogWindowListBox), new PropertyMetadata(false));

    /// <summary>
    /// ShowGridSplitControl
    /// </summary>
    public bool ShowGridSplitControl
    {
      get => (bool) GetValue(ShowGridSplitControlProperty);
      set => SetValue(ShowGridSplitControlProperty, value);
    }

    /// <summary>
    /// ScrollToItemsEnd property
    /// </summary>
    public static readonly DependencyProperty ScrollToItemsEndProperty =
      DependencyProperty.Register(nameof(ScrollToItemsEnd), typeof(bool), typeof(LogWindowListBox), new PropertyMetadata(false));

    /// <summary>
    /// ScrollToItemsEnd
    /// </summary>
    public bool ScrollToItemsEnd
    {
      get => (bool) GetValue(ScrollToItemsEndProperty);
      set => SetValue(ScrollToItemsEndProperty, value);
    }

    /// <summary>
    /// AddDateTime property
    /// </summary>
    public static readonly DependencyProperty AddDateTimeProperty =
      DependencyProperty.Register(nameof(AddDateTime), typeof(bool), typeof(LogWindowListBox), new PropertyMetadata(true));

    /// <summary>
    /// AddDateTime
    /// </summary>
    public bool AddDateTime
    {
      get => (bool) GetValue(AddDateTimeProperty);
      set => SetValue(AddDateTimeProperty, value);
    }

    /// <summary>
    /// Last visible <see cref="LogEntry"/> index property
    /// </summary>
    public static readonly DependencyProperty LastVisibleLogEntryIndexProperty =
      DependencyProperty.Register(nameof(LastVisibleLogEntryIndex), typeof(int), typeof(LogWindowListBox), new PropertyMetadata(0));

    /// <summary>
    /// Last visible <see cref="LogEntry"/> index property
    /// </summary>
    public int LastVisibleLogEntryIndex
    {
      get => (int) GetValue(LastVisibleLogEntryIndexProperty);
      set => SetValue(LastVisibleLogEntryIndexProperty, value);
    }

    private TailData _currentTailData;

    /// <summary>
    /// <see cref="CurrentTailData"/>
    /// </summary>
    public TailData CurrentTailData
    {
      get => _currentTailData;
      set
      {
        _currentTailData = value;
        OnPropertyChanged();

        if ( _currentTailData.OpenFromSmartWatch )
          return;

        RaiseEvent(new RoutedEventArgs(ClearItemsRoutedEvent, this));
      }
    }

    /// <summary>
    /// Highlight data result <see cref="List{T}"/> of <see cref="TextHighlightData"/> property
    /// </summary>
    public static readonly DependencyProperty HighlightDataResultProperty =
      DependencyProperty.Register(nameof(HighlightDataResult), typeof(List<TextHighlightData>),
        typeof(LogWindowListBox), new PropertyMetadata(null));

    /// <summary>
    /// Highlight data result <see cref="List{T}"/> of <see cref="TextHighlightData"/>
    /// </summary>
    public List<TextHighlightData> HighlightDataResult
    {
      get => (List<TextHighlightData>) GetValue(HighlightDataResultProperty);
      set => SetValue(HighlightDataResultProperty, value);
    }

    /// <summary>
    /// SelectedText property
    /// </summary>
    public static readonly DependencyProperty SelectedTextProperty =
      DependencyProperty.Register(nameof(SelectedText), typeof(string), typeof(LogWindowListBox), new PropertyMetadata(null));

    /// <summary>
    /// SelectedText
    /// </summary>
    public string SelectedText
    {
      get => (string) GetValue(SelectedTextProperty);
      set
      {
        SetValue(SelectedTextProperty, value);
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// BookmarkCount property
    /// </summary>
    public static readonly DependencyProperty BookmarkCountProperty =
      DependencyProperty.Register(nameof(BookmarkCount), typeof(int), typeof(LogWindowListBox),
        new PropertyMetadata(0, OnBookmarkCountPropertyChangedCallback));

    /// <summary>
    /// Bookmark count
    /// </summary>
    public int BookmarkCount
    {
      get => (int) GetValue(BookmarkCountProperty);
      set => SetValue(BookmarkCountProperty, value);
    }

    #endregion

    #region RoutedEvents

    /// <summary>
    /// Clears ItemsSource event handler
    /// </summary>
    private static readonly RoutedEvent ClearItemsRoutedEvent =
      EventManager.RegisterRoutedEvent(nameof(ClearItemsRoutedEvent), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LogWindowListBox));

    /// <summary>
    /// Clears ItemsSource event
    /// </summary>
    public event RoutedEventHandler ClearItemsEvent
    {
      add => AddHandler(ClearItemsRoutedEvent, value);
      remove => RemoveHandler(ClearItemsRoutedEvent, value);
    }

    private static readonly RoutedEvent AddBookmarkCommentRoutedEvent =
      EventManager.RegisterRoutedEvent(nameof(AddBookmarkCommentEvent), RoutingStrategy.Bubble, typeof(RoutedEvent), typeof(LogWindowListBox));

    /// <summary>
    /// Add Bookmark comment event
    /// </summary>
    public event RoutedEventHandler AddBookmarkCommentEvent
    {
      add => AddHandler(AddBookmarkCommentRoutedEvent, value);
      remove => RemoveHandler(AddBookmarkCommentRoutedEvent, value);
    }

    private static readonly RoutedEvent SelectedLinesChangedRoutedEvent =
      EventManager.RegisterRoutedEvent(nameof(SelectedLinesChangedEvent), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LogWindowListBox));

    /// <summary>
    /// Selection of lines changed event
    /// </summary>
    public event RoutedEventHandler SelectedLinesChangedEvent
    {
      add => AddHandler(SelectedLinesChangedRoutedEvent, value);
      remove => RemoveHandler(SelectedLinesChangedRoutedEvent, value);
    }

    #endregion

    static LogWindowListBox() => DefaultStyleKeyProperty.OverrideMetadata(typeof(LogWindowListBox), new FrameworkPropertyMetadata(typeof(LogWindowListBox)));

    /// <summary>
    /// Standard constructor
    /// </summary>
    public LogWindowListBox()
    {
      _stringToBrushConverter = new StringToWindowMediaBrushConverter();

      Loaded += LogWindowListBoxOnLoaded;
      Unloaded += LogWindowListBoxOnUnloaded;
    }

    private void LogWindowListBoxOnLoaded(object sender, RoutedEventArgs e)
    {
      CreateReadOnlyTextBoxContextMenu();

      PreviewMouseLeftButtonDown += LogWindowListBoxOnPreviewMouseLeftButtonDown;
      PreviewMouseLeftButtonUp += LogWindowListBoxOnPreviewMouseLeftButtonUp;

      PreviewMouseRightButtonDown += LogWindowListBoxOnPreviewMouseRightButtonDown;
      PreviewMouseRightButtonUp += LogWindowListBoxOnPreviewMouseRightButtonUp;

      PreviewMouseDoubleClick += LogWindowListBoxOnPreviewMouseDoubleClick;

      SelectionChanged += LogWindowListBoxOnSelectionChanged;
    }

    private void LogWindowListBoxOnUnloaded(object sender, RoutedEventArgs e)
    {
      PreviewMouseLeftButtonDown -= LogWindowListBoxOnPreviewMouseLeftButtonDown;
      PreviewMouseLeftButtonUp -= LogWindowListBoxOnPreviewMouseLeftButtonUp;

      PreviewMouseRightButtonDown -= LogWindowListBoxOnPreviewMouseRightButtonDown;
      PreviewMouseRightButtonUp -= LogWindowListBoxOnPreviewMouseRightButtonUp;

      PreviewMouseDoubleClick -= LogWindowListBoxOnPreviewMouseDoubleClick;

      SelectionChanged -= LogWindowListBoxOnSelectionChanged;
    }

    /// <summary>
    /// Update highlighting in <see cref="TextBlock"/>
    /// </summary>
    /// <param name="result"></param>
    public void UpdateHighlighting(List<TextHighlightData> result)
    {
      if ( ItemsSource == null || result == null || result.Count == 0 )
        return;

      var items = ItemsSource.Cast<LogEntry>().ToList();

      // ReSharper disable once ForCanBeConvertedToForeach
      for ( var i = 0; i < items.Count; i++ )
      {
        var logEntry = items[i];
        var tb = FindDataTemplate<TextBlock>(logEntry, "TextBoxMessage");

        if ( tb == null )
          continue;

        var regex = BusinessHelper.GetValidRegexPattern(result.Select(p => p.Text).ToList());
        var splits = regex.Split(tb.Text);

        tb.Inlines.Clear();

        foreach ( string item in splits )
        {
          var highlightData = result.FirstOrDefault(p => string.Compare(p.Text, item, StringComparison.CurrentCultureIgnoreCase) == 0);

          if ( regex.Match(item).Success && highlightData != null )
          {
            var run = new Run(item)
            {
              Foreground = _stringToBrushConverter.Convert(highlightData.TextHighlightColorHex, typeof(Brush), null, null) as Brush
            };

            if ( !string.IsNullOrWhiteSpace(highlightData.TextBackgroundColorHex) && highlightData.IsFindWhat )
            {
              run.Background = _stringToBrushConverter.Convert(highlightData.TextBackgroundColorHex, typeof(Brush), null, null) as Brush;

              if ( run.Background != null )
                run.Background.Opacity = highlightData.Opacity;
            }

            tb.Inlines.Add(run);
          }
          else
          {
            tb.Inlines.Add(item);
          }
        }
      }
    }

    /// <summary>
    /// Go to item by index
    /// </summary>
    /// <param name="index">Index to go</param>
    public void GoToItemByIndex(int index)
    {
      try
      {
        _scrollViewer?.ScrollToVerticalOffset(index - 1);
        object item = Items.GetItemAt(index - 1);
        SelectedItem = item;
      }
      catch
      {
        // Nothing
      }
    }

    /// <summary>
    /// Scroll to the beginning of list
    /// </summary>
    public void ScrollToHome() => _scrollViewer?.ScrollToHome();

    /// <summary>
    /// Scroll to end of list
    /// </summary>
    public void ScrollToEnd() => _scrollViewer?.ScrollToEnd();

    /// <summary>
    /// Get ViewPort height
    /// </summary>
    /// <returns>ViewPort height, otherwise <see cref="double.NaN"/></returns>
    public double GetViewportHeight() => _scrollViewer?.ViewportHeight ?? double.NaN;

    /// <summary>
    /// Gets scroll viewer VerticalOffset
    /// </summary>
    /// <returns>ScrollViewer VerticalOffset, otherwise <see cref="double.NaN"/></returns>
    public double GetScrollViewerVerticalOffset() => _scrollViewer?.VerticalOffset ?? double.NaN;

    #region Mouse events

    private void LogWindowListBoxOnPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if ( Items.Count == 0 )
        return;

      var item = SelectedItem as LogEntry;

      if ( string.IsNullOrWhiteSpace(item?.Message) )
        return;

      _defaultTextMessage = FindDataTemplate<TextBlock>(item, "TextBoxMessage");
      _readOnlyTextMessage = FindDataTemplate<TextBox>(item, "TextBoxReadOnly");

      if ( _defaultTextMessage == null || _readOnlyTextMessage == null )
        return;

      _isMouseDoubleClick = true;
      _defaultTextMessage.Visibility = Visibility.Collapsed;

      _readOnlyTextMessage.Visibility = Visibility.Visible;
      _readOnlyTextMessage.ContextMenu = _readOnlyTextBoxContextMenu;
      _readOnlyTextMessage.SelectionChanged += ReadOnlyTextMessageOnSelectionChanged;
    }

    private void ReadOnlyTextMessageOnSelectionChanged(object sender, RoutedEventArgs e)
    {
      if ( _readOnlyTextMessage == null )
      {
        SelectedText = string.Empty;
        return;
      }

      SelectedText = _readOnlyTextMessage.SelectedText;
    }

    private void LogWindowListBoxOnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if ( e.ClickCount == 2 || _isMouseRightDownClick )
        return;

      _isMouseLeftDownClick = true;
      var mousePoint = PointToScreen(Mouse.GetPosition(this));
      ContextMenu = null;
      LogEntry item = null;

      switch ( e.OriginalSource )
      {
      case Border myBorder:

        if ( myBorder.DataContext is LogEntry entry )
          item = entry;
        break;

      case Image img:

        if ( img.DataContext is LogEntry dc )
          item = dc;
        break;
      }

      if ( item == null )
        return;

      var rcBookmarkPoint = MouseButtonDownHelper(item);

      if ( rcBookmarkPoint == null )
        return;

      if ( !rcBookmarkPoint.Value.Contains((int) mousePoint.X, (int) mousePoint.Y) && _isMouseLeftDownClick )
        return;

      if ( item.BookmarkPoint == null )
      {
        item.BookmarkPoint = BusinessHelper.CreateBitmapIcon("/T4W;component/Resources/Bookmark.png");

        if ( !IsRightWindow() )
          return;

        EnvironmentContainer.Instance.BookmarkManager.AddBookmarkItemsToSource(GetLogWindow().WindowId, item);
      }
      else
      {
        item.BookmarkPoint = null;
        item.BookmarkToolTip = string.Empty;

        if ( !IsRightWindow() )
          return;

        EnvironmentContainer.Instance.BookmarkManager.RemoveFromBookmarkDataSource(item);
      }
    }

    private void LogWindowListBoxOnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      _isMouseDoubleClick = false;
      _isMouseLeftDownClick = false;
    }

    private void LogWindowListBoxOnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      if ( _isMouseLeftDownClick )
        return;

      _isMouseRightDownClick = true;

      if ( !(e.OriginalSource is Image image) )
        return;
      if ( !(image.DataContext is LogEntry item) )
        return;
      if ( item.BookmarkPoint == null )
        return;

      var rcBookmarkPoint = MouseButtonDownHelper(item);
      var mousePoint = PointToScreen(Mouse.GetPosition(this));

      if ( rcBookmarkPoint == null )
        return;

      if ( !rcBookmarkPoint.Value.Contains((int) mousePoint.X, (int) mousePoint.Y) && _isMouseLeftDownClick )
        return;

      var contentContextMenu = new ContextMenu();

      var icon = BusinessHelper.CreateBitmapIcon("/T4W;component/Resources/bubble.png");
      var menuItem = CreateMenuItem(Application.Current.TryFindResource("AddCommentToBookmark").ToString(), icon);
      menuItem.Command = AddBookmarkCommentCommand;
      menuItem.CommandParameter = item;
      contentContextMenu.Items.Add(menuItem);

      icon = BusinessHelper.CreateBitmapIcon("/T4W;component/Resources/Delete_Bookmark.png");
      menuItem = CreateMenuItem(Application.Current.TryFindResource("DeleteBookmarks").ToString(), icon, new Size(14, 14));
      menuItem.Command = RemoveBookmarksCommand;
      contentContextMenu.Items.Add(menuItem);

      ContextMenu = contentContextMenu;
    }

    private void LogWindowListBoxOnPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
      _isMouseDoubleClick = false;
      _isMouseRightDownClick = false;
    }

    #endregion

    #region  Events

    private void LogWindowListBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var eventArgs = new RoutedEventArgs(SelectedLinesChangedRoutedEvent, SelectedItems.Count);
      RaiseEvent(eventArgs);

      if ( _defaultTextMessage == null || _readOnlyTextMessage == null || _isMouseDoubleClick )
        return;

      _defaultTextMessage.Visibility = Visibility.Visible;
      _readOnlyTextMessage.Visibility = Visibility.Collapsed;
      _readOnlyTextMessage.ContextMenu = null;
      _readOnlyTextMessage.SelectionChanged -= ReadOnlyTextMessageOnSelectionChanged;

      SelectedText = null;
    }

    #endregion

    /// <summary>
    /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      _scrollViewer = this.Descendents().OfType<ScrollViewer>().FirstOrDefault();

      if ( _scrollViewer == null )
        return;

      _scrollViewer.ScrollChanged += OnScrollChanged;
    }

    private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
    {
      if ( _splitGripControl == null && ShowGridSplitControl && SettingsHelperController.CurrentSettings.SplitterWindowBehavior )
        LoadSplitGripControl();

      LastVisibleLogEntryIndex = ((int) _scrollViewer.ViewportHeight + (int) _scrollViewer.VerticalOffset) - 1;
      OnPropertyChanged(nameof(LastVisibleLogEntryIndex));
    }

    private void LoadSplitGripControl()
    {
      var scrollBars = _scrollViewer.Descendents().OfType<ScrollBar>().Where(p => p.Visibility == Visibility.Visible);

      foreach ( var scrollBar in scrollBars )
      {
        _splitGripControl = scrollBar.Descendents().OfType<Grid>().FirstOrDefault(p => p.Name == "PART_SplitGripControl");

        if ( _splitGripControl == null || !ShowGridSplitControl )
          continue;

        _splitGripControl.Visibility = Visibility.Visible;
        _splitGripControl.MouseLeftButtonDown += SplitGripControlOnMouseLeftButtonDown;
        _splitGripControl.MouseLeftButtonUp += SplitGripControlOnMouseLeftButtonUp;
        _splitGripControl.MouseMove += SplitGripControlOnMouseMove;
        break;
      }
    }

    private void SplitGripControlOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      LOG.Trace("Mouse left button down");
      _mouseDown = true;

      _splitGripControl.CaptureMouse();
    }

    private void SplitGripControlOnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      LOG.Trace("Mouse left button up");
      _mouseDown = false;

      _splitGripControl.ReleaseMouseCapture();
    }

    private void SplitGripControlOnMouseMove(object sender, MouseEventArgs e)
    {
      if ( !_mouseDown )
        return;

      var splitWindow = _scrollViewer.Ancestors().OfType<SplitWindowControl>().FirstOrDefault();
      var gridSplitter = splitWindow?.Descendents().OfType<GridSplitter>().FirstOrDefault();

      if ( gridSplitter == null )
        return;

      var mouse = e.GetPosition(splitWindow);
      //LOG.Debug($"Current mouse position {mouse.Y}");

      if ( mouse.Y <= 5 )
      {
        gridSplitter.Visibility = Visibility.Collapsed;
        splitWindow.SplitterPosition = 0;
      }
      else
      {
        gridSplitter.Visibility = Visibility.Visible;
        splitWindow.SplitterPosition = mouse.Y;
      }
    }

    /// <summary>
    /// Updates the current selection when an item in the <see cref="T:System.Windows.Controls.Primitives.Selector" /> has changed
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
      base.OnItemsChanged(e);

      switch ( e.Action )
      {
      case NotifyCollectionChangedAction.Add:

        if ( SettingsHelperController.CurrentSettings.AlwaysScrollToEnd && ShowGridSplitControl || ScrollToItemsEnd )
          _scrollViewer?.ScrollToEnd();

        break;

      case NotifyCollectionChangedAction.Move:

        break;
      }
    }

    #region PropertyCallback functions

    private static void OnBookmarkCountPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if ( !(d is LogWindowListBox listBox) )
        return;

      var bookmarkCount = listBox.Descendents().OfType<Label>().FirstOrDefault(p => p.Name == "BookmarkCountLabel");

      if ( bookmarkCount != null )
        bookmarkCount.Content = listBox.BookmarkCount.ToString();

      ((RelayCommand) listBox.BookmarkOverviewCommand).RaiseCanExecuteChanged();
    }

    #endregion

    #region Commands

    private ICommand _undoCommand;

    /// <summary>
    /// Undo command
    /// </summary>
    public ICommand UndoCommand => _undoCommand ?? (_undoCommand = new RelayCommand(p => CanExecuteUndoCommand(), p => ExecuteUndoCommand()));

    private ICommand _addToFilterCommand;

    /// <summary>
    /// Add to filter command
    /// </summary>
    private ICommand AddToFilterCommand => _addToFilterCommand ?? (_addToFilterCommand = new RelayCommand(p => CanExecuteAddToFilterCommand(), p => ExecuteAddToFilterCommand()));

    private ICommand _addToGlobalFilterCommand;

    /// <summary>
    /// Add to filter command
    /// </summary>
    private ICommand AddToGlobalFilterCommand => _addToGlobalFilterCommand ?? (_addToGlobalFilterCommand = new RelayCommand(p => CanExecuteAddToFilterCommand(), p => ExecuteAddToGlobalFilterCommand()));

    private ICommand _addToFindWhatCommand;

    /// <summary>
    /// Add to find what command
    /// </summary>
    private ICommand AddToFindWhatCommand => _addToFindWhatCommand ?? (_addToFindWhatCommand = new RelayCommand(p => CanExecuteAddToFindWhatCommand(),
                                               p => ExecuteAddToFindWhatCommand()));

    private ICommand _removeBookmarksCommand;

    /// <summary>
    /// Remove bookmarks command
    /// </summary>
    private ICommand RemoveBookmarksCommand => _removeBookmarksCommand ?? (_removeBookmarksCommand = new RelayCommand(p => CanExecuteRemoveBookmarksCommand(),
                                                 p => ExecuteRemoveBookmarksCommand()));

    private ICommand _addBookmarkCommentCommand;

    /// <summary>
    /// Add bookmark comment command
    /// </summary>
    private ICommand AddBookmarkCommentCommand => _addBookmarkCommentCommand ?? (_addBookmarkCommentCommand = new RelayCommand(ExecuteAddBookmarkCommentCommand));

    private ICommand _bookmarkOverviewCommand;

    /// <summary>
    /// Bookmark overview command
    /// </summary>
    public ICommand BookmarkOverviewCommand => _bookmarkOverviewCommand ?? (_bookmarkOverviewCommand = new RelayCommand(p => CanExecuteBookmarkOverviewCommand(),
                                                 p => ExecuteBookmarkOverviewCommand()));

    #endregion

    #region Command functions

    private bool CanExecuteBookmarkOverviewCommand()
    {
      if ( ItemsSource == null )
        return false;

      var items = ItemsSource.Cast<LogEntry>().ToList();

      return items.Any(p => p.BookmarkPoint != null);
    }

    private void ExecuteBookmarkOverviewCommand()
    {
      var logWindow = GetLogWindow();

      if ( logWindow == null )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new ShowBookmarkOverviewMessage(logWindow.WindowId));
    }

    private bool CanExecuteUndoCommand() => CurrentTailData != null && CurrentTailData.OpenFromFileManager && CurrentTailData.CanUndo;

    private void ExecuteUndoCommand() => CurrentTailData.Undo();

    private void ExecuteAddBookmarkCommentCommand(object args)
    {
      if ( !(args is LogEntry item) )
        return;

      var eventArgs = new RoutedEventArgs(AddBookmarkCommentRoutedEvent, item);
      RaiseEvent(eventArgs);
    }

    private bool CanExecuteAddToFilterCommand() =>
      _readOnlyTextMessage != null && _readOnlyTextMessage.Visibility != Visibility.Collapsed && _readOnlyTextMessage.SelectionLength > 0;

    private void ExecuteAddToFilterCommand()
    {
      var filterManager = new FilterManager
      {
        Owner = Window.GetWindow(this)
      };
      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenFilterDataFromTailDataMessage(this, CurrentTailData, _readOnlyTextMessage.SelectedText));
      filterManager.ShowDialog();

      OnPropertyChanged(nameof(CurrentTailData));
    }

    private void ExecuteAddToGlobalFilterCommand()
    {
      var options = new Options
      {
        Owner = Window.GetWindow(this)
      };
      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenGlobalHighlightSettingMessage(this, _readOnlyTextMessage.SelectedText));
      options.ShowDialog();
    }

    private bool CanExecuteAddToFindWhatCommand() =>
      _readOnlyTextMessage != null && _readOnlyTextMessage.Visibility != Visibility.Collapsed && _readOnlyTextMessage.SelectionLength > 0;

    private void ExecuteAddToFindWhatCommand()
    {
      var logWindow = GetLogWindow();

      if ( logWindow == null )
        return;

#if DEBUG
      LOG.Trace($"Selected word is {_readOnlyTextMessage.SelectedText}");
#endif

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenFindWhatWindowMessage(this, CurrentTailData.File, logWindow.WindowId, _readOnlyTextMessage.SelectedText));
    }

    private bool CanExecuteRemoveBookmarksCommand() => Items.Count > 0;

    private void ExecuteRemoveBookmarksCommand() => RemoveAllBookmarks(true);

    private void RemoveAllBookmarks(bool isCommand)
    {
      if ( Items.Count == 0 )
        return;

      var items = ItemsSource.Cast<LogEntry>().ToList();
      items.ForEach(p =>
      {
        if ( p.BookmarkPoint != null )
        {
          p.BookmarkPoint = null;
          p.BookmarkToolTip = string.Empty;
        }

        if ( isCommand && IsRightWindow() )
          EnvironmentContainer.Instance.BookmarkManager.RemoveFromBookmarkDataSource(p);
      });

      ContextMenu = null;
    }

    /// <summary>
    /// Removes all bookmarks from data source
    /// </summary>
    public void RemoveAllBookmarks() => RemoveAllBookmarks(false);

    #endregion

    private bool IsRightWindow()
    {
      var logWindow = GetLogWindow();
      return logWindow != null && Equals(EnvironmentContainer.Instance.BookmarkManager.GetCurrentWindowId(), logWindow.WindowId);
    }

    private ILogWindowControl GetLogWindow()
    {
      var logWindow = this.Ancestors().OfType<ILogWindowControl>().ToList();
      return logWindow.Count == 0 ? null : logWindow.FirstOrDefault();
    }

    private void CreateReadOnlyTextBoxContextMenu()
    {
      var icon = BusinessHelper.CreateBitmapIcon("/T4W;component/Resources/transparent.png");
      _readOnlyTextBoxContextMenu = new ContextMenu();
      var menuItem = CreateMenuItem(Application.Current.TryFindResource("AddToFilter").ToString(), icon);
      menuItem.Command = AddToFilterCommand;

      _readOnlyTextBoxContextMenu.Items.Add(menuItem);

      menuItem = CreateMenuItem(Application.Current.TryFindResource("AddToGlobalFilter").ToString(), icon);
      menuItem.Command = AddToGlobalFilterCommand;

      _readOnlyTextBoxContextMenu.Items.Add(menuItem);

      var separator = new Separator();
      _readOnlyTextBoxContextMenu.Items.Add(separator);

      menuItem = CreateMenuItem(Application.Current.TryFindResource("AddToFindWhat").ToString(), icon);
      menuItem.Command = AddToFindWhatCommand;
      menuItem.InputGestureText = Application.Current.TryFindResource("FindWhatInputGesture").ToString();

      _readOnlyTextBoxContextMenu.Items.Add(menuItem);
    }

    private static MenuItem CreateMenuItem(string header, ImageSource image = null, Size? iconSize = null)
    {
      if ( string.IsNullOrWhiteSpace(header) )
        throw new ArgumentNullException(nameof(header));

      var menuItem = new MenuItem
      {
        Header = header,
        Icon = image == null ? null : new Image
        {
          Source = image,
          Width = iconSize?.Width ?? 16,
          Height = iconSize?.Height ?? 16
        },
        FontWeight = SystemFonts.MenuFontWeight,
        FontStyle = SystemFonts.MenuFontStyle,
        FontSize = SystemFonts.MenuFontSize,
        FontStretch = FontStretches.Normal,
        FontFamily = SystemFonts.MenuFontFamily,
        Foreground = Brushes.Black
      };
      return menuItem;
    }

    private T FindDataTemplate<T>(LogEntry item, string templateName) where T : FrameworkElement
    {
      var myListBoxItem = (ListBoxItem) ItemContainerGenerator.ContainerFromItem(item);

      if ( myListBoxItem == null )
        return null;

      var myContentPresenter = myListBoxItem.Descendents().OfType<ContentPresenter>().FirstOrDefault();
      var myDataTemplate = myContentPresenter?.ContentTemplate;
      var control = (T) myDataTemplate?.FindName(templateName, myContentPresenter);

      return control;
    }

    private System.Drawing.Rectangle? MouseButtonDownHelper(LogEntry item)
    {
      var textBlock = FindDataTemplate<TextBlock>(item, "TextBoxMessage");
      var target = FindDataTemplate<Image>(item, "TextBoxBookmarkPoint");

      if ( target == null || textBlock == null )
        return null;

      var lines = GetLines(textBlock);
      var linesEnumerable = lines as string[] ?? lines.ToArray();
      var relativePoint = target.PointToScreen(new Point(0, 0));
      var s = new Size(16, 16);
      var textSize = textBlock.Text.GetMeasureTextSize(new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch), textBlock.FontSize);

      if ( textSize.Height * linesEnumerable.Length >= 16 )
        s.Height = textSize.Height * linesEnumerable.Length;

      // very strange behaviour! when image is shown, no correction is needed, otherwise it is needed??? WTF!
      if ( item.BookmarkPoint != null )
      {
#if DEBUG
        LOG.Debug($"BookmarkPoint is null X {relativePoint.X} Y {relativePoint.Y} Width {s.Width} Height {s.Height}");
#endif
        return new System.Drawing.Rectangle((int) relativePoint.X, (int) relativePoint.Y, (int) s.Width, (int) s.Height);
      }

      relativePoint.X -= s.Width / 2;
      relativePoint.Y -= s.Height / 2;

#if DEBUG
      LOG.Debug($"BookmarkPoint is not null X {relativePoint.X} Y {relativePoint.Y} Width {s.Width} Height {s.Height}");
#endif
      return new System.Drawing.Rectangle((int) relativePoint.X, (int) relativePoint.Y, (int) s.Width, (int) s.Height);
    }

    private IEnumerable<string> GetLines(TextBlock source)
    {
      string text = source.Text;
      var offset = 0;
      var lineStart = source.ContentStart.GetPositionAtOffset(1, LogicalDirection.Forward);

      do
      {
        var lineEnd = lineStart?.GetLineStartPosition(1);
        int length = lineEnd != null ? lineStart.GetOffsetToPosition(lineEnd) : text.Length - offset;

        yield return text.Substring(offset, length);

        offset += length;
        lineStart = lineEnd;
      }
      while ( lineStart != null );
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
