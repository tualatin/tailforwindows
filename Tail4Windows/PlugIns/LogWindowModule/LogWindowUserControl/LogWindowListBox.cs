using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using log4net;
using Org.Vs.TailForWin.Business.Data;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Settings;
using Org.Vs.TailForWin.Core.Extensions;
using Org.Vs.TailForWin.UI.Converters;
using Org.Vs.TailForWin.UI.Extensions;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule.LogWindowUserControl
{
  /// <summary>
  /// LogWindow control
  /// </summary>
  public class LogWindowListBox : ListBox, INotifyPropertyChanged
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

    private readonly StringToWindowMediaBrushConverter _stringToWindowMediaBrushConverter;
    private Brush _textHighlightColorBrush;

    #region Public properties

    /// <summary>
    /// ShowGridSplitControl property
    /// </summary>
    public static readonly DependencyProperty ShowGridSplitControlProperty = DependencyProperty.Register(nameof(ShowGridSplitControl), typeof(bool), typeof(LogWindowListBox),
      new PropertyMetadata(false));

    /// <summary>
    /// ShowGridSplitControl
    /// </summary>
    public bool ShowGridSplitControl
    {
      get => (bool) GetValue(ShowGridSplitControlProperty);
      set => SetValue(ShowGridSplitControlProperty, value);
    }

    /// <summary>
    /// Text editor selection color porperty
    /// </summary>
    public static readonly DependencyProperty TextEditorSelectionColorHexProperty = DependencyProperty.Register(nameof(TextEditorSelectionColorHex), typeof(string), typeof(LogWindowListBox),
      new PropertyMetadata(DefaultEnvironmentSettings.HighlightLineNumberColor));

    /// <summary>
    /// Text editor selection color
    /// </summary>
    public string TextEditorSelectionColorHex
    {
      get => (string) GetValue(TextEditorSelectionColorHexProperty);
      set => SetValue(TextEditorSelectionColorHexProperty, value);
    }

    /// <summary>
    /// Text editor search highlight background property
    /// </summary>
    public static readonly DependencyProperty TextEditorSearchHighlightBackgroundHexProperty = DependencyProperty.Register(nameof(TextEditorSearchHighlightBackgroundHex), typeof(string),
      typeof(LogWindowListBox), new PropertyMetadata(DefaultEnvironmentSettings.SearchHighlightBackgroundColor));

    /// <summary>
    /// Text editor search highlight background
    /// </summary>
    public string TextEditorSearchHighlightBackgroundHex
    {
      get => (string) GetValue(TextEditorSearchHighlightBackgroundHexProperty);
      set => SetValue(TextEditorSearchHighlightBackgroundHexProperty, value);
    }

    /// <summary>
    /// Text editor search highlight foreground property
    /// </summary>
    public static readonly DependencyProperty TextEditorSearchHighlightForegroundHexProperty = DependencyProperty.Register(nameof(TextEditorSearchHighlightForegroundHex), typeof(string),
      typeof(LogWindowListBox), new PropertyMetadata(DefaultEnvironmentSettings.SearchHighlightForegroundColor));

    /// <summary>
    /// Text editor search highlight foreground
    /// </summary>
    public string TextEditorSearchHighlightForegroundHex
    {
      get => (string) GetValue(TextEditorSearchHighlightForegroundHexProperty);
      set => SetValue(TextEditorSearchHighlightForegroundHexProperty, value);
    }

    /// <summary>
    /// Text editor highlight foreground property
    /// </summary>
    public static readonly DependencyProperty TextEditorHighlightForegroundHexProperty = DependencyProperty.Register(nameof(TextEditorHighlightForegroundHex), typeof(string),
      typeof(LogWindowListBox), new PropertyMetadata(DefaultEnvironmentSettings.HighlightForegroundColor));

    /// <summary>
    /// Text editor highlight foreground
    /// </summary>
    public string TextEditorHighlightForegroundHex
    {
      get => (string) GetValue(TextEditorHighlightForegroundHexProperty);
      set => SetValue(TextEditorHighlightForegroundHexProperty, value);
    }


    /// <summary>
    /// AddDateTime property
    /// </summary>
    public static readonly DependencyProperty AddDateTimeProperty = DependencyProperty.Register(nameof(AddDateTime), typeof(bool), typeof(LogWindowListBox),
      new PropertyMetadata(true));

    /// <summary>
    /// AddDateTime
    /// </summary>
    public bool AddDateTime
    {
      get => (bool) GetValue(AddDateTimeProperty);
      set => SetValue(AddDateTimeProperty, value);
    }

    /// <summary>
    /// Filter on property
    /// </summary>
    public static readonly DependencyProperty FilterOnProperty = DependencyProperty.Register(nameof(FilterOn), typeof(bool), typeof(LogWindowListBox),
      new PropertyMetadata(false, OnFilterOnChanged));

    /// <summary>
    /// Set filter on
    /// </summary>
    public bool FilterOn
    {
      get => (bool) GetValue(FilterOnProperty);
      set => SetValue(FilterOnProperty, value);
    }

    /// <summary>
    /// Last visible <see cref="LogEntry"/> index property
    /// </summary>
    public static readonly DependencyProperty LastVisibleLogEntryIndexProperty = DependencyProperty.Register(nameof(LastVisibleLogEntryIndex), typeof(int), typeof(LogWindowListBox),
      new PropertyMetadata(0));

    /// <summary>
    /// Last visible <see cref="LogEntry"/> index property
    /// </summary>
    public int LastVisibleLogEntryIndex
    {
      get => (int) GetValue(LastVisibleLogEntryIndexProperty);
      set => SetValue(LastVisibleLogEntryIndexProperty, value);
    }

    /// <summary>
    /// Current <see cref="CurrentTailData"/> property
    /// </summary>
    public static readonly DependencyProperty CurrentTailDataProperty = DependencyProperty.Register(nameof(CurrentTailData), typeof(TailData), typeof(LogWindowListBox),
      new PropertyMetadata(null, OnTailDataChanged));

    /// <summary>
    /// <see cref="CurrentTailData"/>
    /// </summary>
    public TailData CurrentTailData
    {
      get => (TailData) GetValue(CurrentTailDataProperty);
      set => SetValue(CurrentTailDataProperty, value);
    }

    #endregion

    #region Events

    /// <summary>
    ///Item visible changed event, that fires, when the new item is visible in UI
    /// </summary>
    public event EventHandler ItemVisibleChanged;

    #endregion

    #region RoutedEvents

    /// <summary>
    /// Clears ItemsSource event handler
    /// </summary>
    private static readonly RoutedEvent ClearItemsRoutedEvent = EventManager.RegisterRoutedEvent(nameof(ClearItemsRoutedEvent), RoutingStrategy.Bubble,
      typeof(RoutedEventHandler), typeof(LogWindowListBox));

    /// <summary>
    /// Clears ItemsSource event
    /// </summary>
    public event RoutedEventHandler ClearItemsEvent
    {
      add => AddHandler(ClearItemsRoutedEvent, value);
      remove => RemoveHandler(ClearItemsRoutedEvent, value);
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public LogWindowListBox()
    {
      PreviewMouseLeftButtonDown += LogWindowListBoxOnPreviewMouseLeftButtonDown;
      PreviewMouseLeftButtonUp += LogWindowListBoxOnPreviewMouseLeftButtonUp;

      PreviewMouseRightButtonDown += LogWindowListBoxOnPreviewMouseRightButtonDown;
      PreviewMouseRightButtonUp += LogWindowListBoxOnPreviewMouseRightButtonUp;

      PreviewMouseDoubleClick += LogWindowListBoxOnPreviewMouseDoubleClick;

      SelectionChanged += LogWindowListBoxOnSelectionChanged;
      _stringToWindowMediaBrushConverter = new StringToWindowMediaBrushConverter();
    }

    /// <summary>
    /// Go to item by index
    /// </summary>
    /// <param name="index">Index to go</param>
    public void GoToItemByIndex(int index)
    {
      _scrollViewer?.ScrollToVerticalOffset(index - 1);
      var item = Items.GetItemAt(index - 1);
      SelectedItem = item;
    }

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
    }

    private void LogWindowListBoxOnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if ( e.ClickCount == 2 || _isMouseRightDownClick )
        return;

      _isMouseLeftDownClick = true;
      var mousePoint = PointToScreen(Mouse.GetPosition(this));
      Image image = null;
      ContextMenu = null;

      switch ( e.OriginalSource )
      {
      case Grid myGrid:

        var enumerator = myGrid.Children.GetEnumerator();

        if ( enumerator.MoveNext() )
          image = enumerator.Current as Image;

        break;

      case Image img:

        image = img;
        break;
      }

      if ( !(image?.DataContext is LogEntry item) )
        return;

      System.Drawing.Rectangle? rcBookmarkpoint = MouseButtonDownHelper(item);

      if ( rcBookmarkpoint == null )
        return;

      if ( !rcBookmarkpoint.Value.Contains((int) mousePoint.X, (int) mousePoint.Y) && _isMouseLeftDownClick )
        return;

      System.Windows.Media.Imaging.BitmapImage bp = new System.Windows.Media.Imaging.BitmapImage();
      bp.BeginInit();
      bp.UriSource = new Uri("/T4W;component/Resources/Boomark.png", UriKind.Relative);
      bp.EndInit();

      RenderOptions.SetBitmapScalingMode(bp, BitmapScalingMode.NearestNeighbor);
      RenderOptions.SetEdgeMode(bp, EdgeMode.Aliased);

      item.BookmarkPoint = item.BookmarkPoint == null ? bp : null;
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

      var rcBookmarkpoint = MouseButtonDownHelper(item);
      var mousePoint = PointToScreen(Mouse.GetPosition(this));

      if ( rcBookmarkpoint == null )
        return;

      if ( !rcBookmarkpoint.Value.Contains((int) mousePoint.X, (int) mousePoint.Y) && _isMouseLeftDownClick )
        return;

      var icon = new System.Windows.Media.Imaging.BitmapImage();
      icon.BeginInit();
      icon.UriSource = new Uri("/T4W;component/Resources/Delete_Bookmark.png", UriKind.Relative);
      icon.EndInit();

      RenderOptions.SetBitmapScalingMode(icon, BitmapScalingMode.NearestNeighbor);
      RenderOptions.SetEdgeMode(icon, EdgeMode.Aliased);

      var contenContextMenu = new ContextMenu();
      var menuItem = new MenuItem
      {
        Header = Application.Current.TryFindResource("DeleteBookmarks").ToString(),
        Icon = new Image
        {
          Source = icon
        },
        FontWeight = SystemFonts.MenuFontWeight,
        FontStyle = SystemFonts.MenuFontStyle,
        FontSize = SystemFonts.MenuFontSize,
        FontStretch = FontStretches.Normal,
        FontFamily = SystemFonts.MenuFontFamily,
        Foreground = Brushes.Black
      };

      menuItem.Click += OnRemoveBookmarks;
      contenContextMenu.Items.Add(menuItem);

      ContextMenu = contenContextMenu;
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
      if ( _defaultTextMessage == null || _readOnlyTextMessage == null || _isMouseDoubleClick )
        return;

      _defaultTextMessage.Visibility = Visibility.Visible;
      _readOnlyTextMessage.Visibility = Visibility.Collapsed;
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
      ItemVisibleChanged?.Invoke(this, EventArgs.Empty);
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

        if ( SettingsHelperController.CurrentSettings.AlwaysScrollToEnd && ShowGridSplitControl )
          _scrollViewer?.ScrollToEnd();

        break;

      case NotifyCollectionChangedAction.Move:

        break;
      }
    }

    #region PropertyCallback functions

    private static void OnTailDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if ( !(d is LogWindowListBox control) )
        return;

      control.RaiseEvent(new RoutedEventArgs(ClearItemsRoutedEvent, control));
    }

    private static void OnFilterOnChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if ( sender is LogWindowListBox control )
        control.RefreshCollectionViewSource();
    }

    #endregion

    private void OnRemoveBookmarks(object sender, RoutedEventArgs e)
    {
      var enumerator = ItemsSource.GetEnumerator();

      while ( enumerator.MoveNext() )
      {
        if ( !(enumerator.Current is LogEntry logEntry) )
          continue;

        if ( logEntry.BookmarkPoint == null )
          continue;

        logEntry.BookmarkPoint = null;
      }

      ContextMenu = null;
      e.Handled = true;
    }

    /// <summary>
    /// Highlights certain words in Control
    /// </summary>
    /// <param name="logEntry"><see cref="LogEntry"/></param>
    /// <param name="words"><see cref="List{T}"/> of words</param>
    public void SetHighlightInTextBlock(LogEntry logEntry, List<string> words)
    {
      if ( words == null || words.Count == 0 )
        return;

      var tb = FindDataTemplate<TextBlock>(logEntry, "TextBoxMessage");

      if ( _textHighlightColorBrush == null )
        _textHighlightColorBrush = (Brush) _stringToWindowMediaBrushConverter.Convert(TextEditorHighlightForegroundHex, typeof(Brush), null, CultureInfo.InvariantCulture);

      if ( _textHighlightColorBrush == null )
        return;

      HighlightTextInTextBlock(tb, words, _textHighlightColorBrush);
    }

    private void HighlightTextInTextBlock(TextBlock tb, IEnumerable<string> words, Brush highlightForegroundColor, Brush highlightBackgroundColor = null)
    {
      if ( tb == null )
        return;

      var regex = new Regex($"({string.Join("|", words)})");
      var splits = regex.Split(tb.Text);

      tb.Inlines.Clear();

      foreach ( string item in splits )
      {
        if ( regex.Match(item).Success )
        {
          var run = new Run(item)
          {
            Foreground = highlightForegroundColor,
            Background = highlightBackgroundColor ?? Brushes.Transparent
          };
          tb.Inlines.Add(run);
        }
        else
        {
          tb.Inlines.Add(item);
        }
      }
    }

    private void RemoveHighlightTextInTextBlock(TextBlock tb)
    {
      if ( tb == null )
        return;

      string text = tb.Text;
      tb.Inlines.Clear();
      tb.Inlines.Add(text);

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

      var relativePoint = target.PointToScreen(new Point(0, 0));
      var s = new Size(16, 16);
      var textSize = textBlock.Text.GetMeasureTextSize(new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch), textBlock.FontSize);

      if ( textSize.Height >= 16 )
        s.Height = textSize.Height;

      // very strange behaviour! when image is shown, no correction is needed, otherwise it is needed??? WTF!
      if ( item.BookmarkPoint != null )
      {
        LOG.Debug($"BookmarkPoint is null X {relativePoint.X} Y {relativePoint.Y} Width {s.Width} Height {s.Height}");
        return new System.Drawing.Rectangle((int) relativePoint.X, (int) relativePoint.Y, (int) s.Width, (int) s.Height);
      }

      relativePoint.X = relativePoint.X - s.Width / 2;
      relativePoint.Y = relativePoint.Y - s.Height / 2;
      LOG.Debug($"BookmarkPoint is not null X {relativePoint.X} Y {relativePoint.Y} Width {s.Width} Height {s.Height}");

      return new System.Drawing.Rectangle((int) relativePoint.X, (int) relativePoint.Y, (int) s.Width, (int) s.Height);
    }

    private void RefreshCollectionViewSource()
    {
      if ( ItemsSource != null )
        CollectionViewSource.GetDefaultView(ItemsSource).Refresh();
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
