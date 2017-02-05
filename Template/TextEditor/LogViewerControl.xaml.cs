using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using log4net;
using Org.Vs.TailForWin.Data;
using Org.Vs.TailForWin.Data.Events;
using Org.Vs.TailForWin.Template.TextEditor.Data;
using Org.Vs.TailForWin.Template.TextEditor.Utils;


namespace Org.Vs.TailForWin.Template.TextEditor
{
  /// <summary>
  /// Interaction logic for LogViewerControl.xaml
  /// </summary>
  public partial class LogViewerControl
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(LogViewerControl));

    #region EventHandler

    /// <summary>
    /// Alert event handler
    /// </summary>
    public event EventHandler Alert;

    #endregion

    private bool leftMouseButtonDown;
    private bool rightMouseButtonDown;
    private bool wordSelection;
    private bool mouseMove;
    private Point oldMousePosition;
    //private readonly DeferredAction deferredOnMouseMove;
    //private readonly TimeSpan onMouseMoveDelay = TimeSpan.FromMilliseconds (200.0);
    private ScrollViewer logViewScrollViewer;
    private bool newSearch;
    private TextBox readOnlyEditor;
    private TextBlock showLineEditor;

    /// <summary>
    /// Index for LogEntries and line numbers
    /// </summary>
    private int index;

    /// <summary>
    /// Pattern from search box what we explore
    /// </summary>
    private string searchText;

    /// <summary>
    /// Search bookmarks only
    /// </summary>
    private bool searchBookmark;

    /// <summary>
    /// Collection of found items
    /// </summary>
    private readonly List<LogEntry> foundItems;

    /// <summary>
    /// Collection of words what we want to find
    /// </summary>
    private ObservableCollection<FilterData> filters;

    /// <summary>
    /// CollectionViewSource to filter data, grouping etc.
    /// </summary>
    private readonly CollectionViewSource collectionViewSource;

    /// <summary>
    /// Mouse left button down counter, don't know, why this event fires twice
    /// </summary>
    private int mouseLeftButtonDownCounter;

    // TODO CodeFolding


    /// <summary>
    /// Standard constructor
    /// </summary>
    public LogViewerControl()
    {
      InitializeComponent();

      LogViewer.PreviewMouseLeftButtonDown += LogViewer_PreviewMouseLeftButtonDown;
      LogViewer.PreviewMouseRightButtonDown += LogViewer_PreviewMouseRightButtonDown;
      LogViewer.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(LogViewer_MouseLeftButtonDown), true);
      LogViewer.AddHandler(MouseRightButtonDownEvent, new MouseButtonEventHandler(LogViewer_MouseRightButtonDown), true);
      LogViewer.AddHandler(MouseDoubleClickEvent, new MouseButtonEventHandler(LogViewer_MouseDoubleClick), true);
      LogViewer.PreviewMouseLeftButtonUp += LogViewer_MouseUp;
      LogViewer.PreviewMouseRightButtonUp += LogViewer_MouseUp;
      LogViewer.MouseMove += LogViewer_MouseMove;
      LogViewer.SelectionChanged += LogViewer_SelectionChanged;

      LogEntries = new ObservableCollection<LogEntry>();
      collectionViewSource = new CollectionViewSource
      {
        Source = LogEntries
      };
      collectionViewSource.Filter += CollectionViewSourceFilter;
      LogViewer.DataContext = collectionViewSource;
      filters = new ObservableCollection<FilterData>();
      foundItems = new List<LogEntry>();

      StringFormatData.StringFormat = "dd.MM.yyyy HH:mm:ss.fff";
      newSearch = true;
    }

    #region Public properties

    /// <summary>
    /// Show line number property
    /// </summary>
    public static readonly DependencyProperty ShowLineNumbersProperty = DependencyProperty.Register("ShowLineNumbers", typeof(bool), typeof(LogViewerControl),
                              new PropertyMetadata(false, OnDataTemplateChanged));

    /// <summary>
    /// Text editor show line numbers
    /// </summary>
    [Category("TextEditor")]
    public bool ShowLineNumbers
    {
      get
      {
        return ((bool) GetValue(ShowLineNumbersProperty));
      }
      set
      {
        SetValue(ShowLineNumbersProperty, value);
      }
    }

    /// <summary>
    /// Line number color property
    /// </summary>
    public static readonly DependencyProperty LineNumbersColorProperty = DependencyProperty.Register("LineNumbersColor", typeof(Brush), typeof(LogViewerControl),
                              new PropertyMetadata(Brushes.Gray));

    /// <summary>
    /// Text editor line number color
    /// </summary>
    [Category("TextEditor")]
    public Brush LineNumbersColor
    {
      get
      {
        return ((Brush) GetValue(LineNumbersColorProperty));
      }
      set
      {
        SetValue(LineNumbersColorProperty, value);
      }
    }

    public static readonly DependencyProperty LineNumbersBackgroundColorProperty = DependencyProperty.Register("LineNumbersBackgroundColor", typeof(Brush), typeof(LogViewerControl),
                              new PropertyMetadata(Brushes.LightGray));

    /// <summary>
    /// Text editor line number background color
    /// </summary>
    [Category("TextEditor")]
    public Brush LineNumbersBackgroundColor
    {
      get
      {
        return ((Brush) GetValue(LineNumbersBackgroundColorProperty));
      }
      set
      {
        SetValue(LineNumbersBackgroundColorProperty, value);
      }
    }

    /// <summary>
    /// Word wrap property
    /// </summary>
    public static readonly DependencyProperty WordWrappingProperty = DependencyProperty.Register("WordWrapping", typeof(bool), typeof(LogViewerControl),
                              new PropertyMetadata(false));

    /// <summary>
    /// Text editor word wrapping
    /// </summary>
    [Category("TextEditor")]
    public bool WordWrapping
    {
      get
      {
        return ((bool) GetValue(WordWrappingProperty));
      }
      set
      {
        SetValue(WordWrappingProperty, value);
      }
    }

    /// <summary>
    /// Vertical scroll bar visibility property
    /// </summary>
    public static readonly DependencyProperty VerticalScrollBarVisibleProperty = DependencyProperty.Register("VerticalScrollBarVisible", typeof(ScrollBarVisibility), typeof(LogViewerControl), new PropertyMetadata(ScrollBarVisibility.Auto));

    /// <summary>
    /// TextEditor vertical scroll visible
    /// </summary>
    [Category("TextEditor")]
    public ScrollBarVisibility VerticalScrollbarVisible
    {
      get
      {
        return ((ScrollBarVisibility) GetValue(VerticalScrollBarVisibleProperty));
      }
      set
      {
        SetValue(VerticalScrollBarVisibleProperty, value);
      }
    }

    /// <summary>
    /// Text editor background color property
    /// </summary>
    public static readonly DependencyProperty TextEditorBackGroundColorProperty = DependencyProperty.Register("TextEditorBackgroundColor", typeof(Brush), typeof(LogViewerControl),
                              new PropertyMetadata(Brushes.White));

    /// <summary>
    /// Text editor background color
    /// </summary>
    [Category("TextEditor")]
    public Brush TextEditorBackgroundColor
    {
      get
      {
        return ((Brush) GetValue(TextEditorBackGroundColorProperty));
      }
      set
      {
        SetValue(TextEditorBackGroundColorProperty, value);
      }
    }

    /// <summary>
    /// Text editor forground color property
    /// </summary>
    public static readonly DependencyProperty TextEditorForegroundColorProperty = DependencyProperty.Register("TextEditorForegroundColor", typeof(Brush), typeof(LogViewerControl),
                              new PropertyMetadata(Brushes.Black));

    /// <summary>
    /// Text editor foreground color
    /// </summary>
    [Category("TextEditor")]
    public Brush TextEditorForegroundColor
    {
      get
      {
        return ((Brush) GetValue(TextEditorForegroundColorProperty));
      }
      set
      {
        SetValue(TextEditorForegroundColorProperty, value);
      }
    }

    /// <summary>
    /// Show date time property
    /// </summary>
    public static readonly DependencyProperty ShowDateTimeProperty = DependencyProperty.Register("ShowDateTime", typeof(bool), typeof(LogViewerControl),
                              new PropertyMetadata(false, OnDataTemplateChanged));

    /// <summary>
    /// TextEditor show datetime
    /// </summary>
    [Category("TextEditor")]
    public bool ShowDateTime
    {
      get
      {
        return ((bool) GetValue(ShowDateTimeProperty));
      }
      set
      {
        SetValue(ShowDateTimeProperty, value);
      }
    }

    /// <summary>
    /// Text editor font style property
    /// </summary>
    public static readonly DependencyProperty TextEditorFontStyleProperty = DependencyProperty.Register("TextEditorFontStyle", typeof(FontStyle), typeof(LogViewerControl),
                              new PropertyMetadata(FontStyles.Normal));

    /// <summary>
    /// Text editor font style
    /// </summary>
    [Category("TextEditor")]
    public FontStyle TextEditorFontStyle
    {
      get
      {
        return ((FontStyle) GetValue(TextEditorFontStyleProperty));
      }
      set
      {
        SetValue(TextEditorFontStyleProperty, value);
      }
    }

    /// <summary>
    /// Text editor font family property
    /// </summary>
    public static readonly DependencyProperty TextEditorFontFamilyProperty = DependencyProperty.Register("TextEditorFontFamily", typeof(FontFamily), typeof(LogViewerControl),
                              new PropertyMetadata(new FontFamily("Segoe UI")));

    /// <summary>
    /// Text editor font family
    /// </summary>
    [Category("TextEditor")]
    public FontFamily TextEditorFontFamily
    {
      get
      {
        return ((FontFamily) GetValue(TextEditorFontFamilyProperty));
      }
      set
      {
        SetValue(TextEditorFontFamilyProperty, value);
      }
    }

    /// <summary>
    /// Text editor font weight property
    /// </summary>
    public static readonly DependencyProperty TextEditorFontWeightProperty = DependencyProperty.Register("TextEditorFontWeight", typeof(FontWeight), typeof(LogViewerControl),
                              new PropertyMetadata(FontWeights.Normal));

    /// <summary>
    /// Text editor font weight
    /// </summary>
    [Category("TextEditor")]
    public FontWeight TextEditorFontWeight
    {
      get
      {
        return ((FontWeight) GetValue(TextEditorFontWeightProperty));
      }
      set
      {
        SetValue(TextEditorFontWeightProperty, value);
      }
    }

    /// <summary>
    /// Text editor font size property
    /// </summary>
    public static readonly DependencyProperty TextEditorFontSizeProperty = DependencyProperty.Register("TextEditorFontSize", typeof(int), typeof(LogViewerControl),
                              new PropertyMetadata(12));

    /// <summary>
    /// Text editor font size
    /// </summary>
    [Category("TextEditor")]
    public int TextEditorFontSize
    {
      get
      {
        return ((int) GetValue(TextEditorFontSizeProperty));
      }
      set
      {
        SetValue(TextEditorFontSizeProperty, value);
      }
    }

    /// <summary>
    /// Text editor selection color porperty
    /// </summary>
    public static readonly DependencyProperty TextEditorSelectionColorProperty = DependencyProperty.Register("TextEditorSelectionColor", typeof(Color),
                              typeof(LogViewerControl), new PropertyMetadata(Colors.Blue));

    /// <summary>
    /// Text editor selection color
    /// </summary>
    [Category("TextEditor")]
    public Color TextEditorSelectionColor
    {
      get
      {
        return ((Color) GetValue(TextEditorSelectionColorProperty));
      }
      set
      {
        SetValue(TextEditorSelectionColorProperty, value);
      }
    }

    /// <summary>
    /// Always scroll into view property
    /// </summary>
    public static readonly DependencyProperty AlwaysScrollIntoViewProperty = DependencyProperty.Register("AlwaysScrollIntoView", typeof(bool), typeof(LogViewerControl),
                          new PropertyMetadata(true));

    /// <summary>
    /// Text editor scroll into view
    /// </summary>
    [Category("TextEditor")]
    public bool AlwaysScrollIntoView
    {
      get
      {
        return ((bool) GetValue(AlwaysScrollIntoViewProperty));
      }
      set
      {
        SetValue(AlwaysScrollIntoViewProperty, value);
      }
    }

    /// <summary>
    /// Text editor search highlight background property
    /// </summary>
    public static readonly DependencyProperty TextEditorSearchHighlightBackgroundProperty = DependencyProperty.Register("TextEditorSearchHighlightBackground", typeof(Brush), typeof(LogViewerControl),
                          new PropertyMetadata(Brushes.Red));

    /// <summary>
    /// Text editor search highlight background
    /// </summary>
    [Category("TextEditor")]
    public Brush TextEditorSearchHighlightBackground
    {
      get
      {
        return ((Brush) GetValue(TextEditorSearchHighlightBackgroundProperty));
      }
      set
      {
        SetValue(TextEditorSearchHighlightBackgroundProperty, value);
      }
    }

    /// <summary>
    /// Text eidtor search highlight foreground property
    /// </summary>
    public static readonly DependencyProperty TextEditorSearchHighlightForegroundProperty = DependencyProperty.Register("TextEditorSearchHighlightForeground", typeof(Brush), typeof(LogViewerControl), new PropertyMetadata(Brushes.WhiteSmoke));

    /// <summary>
    /// Text editor search highlight foreground
    /// </summary>
    [Category("TextEditor")]
    public Brush TextEditorSearchHighlightForeground
    {
      get
      {
        return ((Brush) GetValue(TextEditorSearchHighlightForegroundProperty));
      }
      set
      {
        SetValue(TextEditorSearchHighlightForegroundProperty, value);
      }
    }

    /// <summary>
    /// All log entries
    /// </summary>
    public ObservableCollection<LogEntry> LogEntries
    {
      get;
      set;
    }

    private static readonly DependencyProperty AddDateTimeProperty = DependencyProperty.Register("AddDateTime", typeof(bool), typeof(LogViewerControl),
                      new PropertyMetadata(true));

    private bool AddDateTime
    {
      get
      {
        return ((bool) GetValue(AddDateTimeProperty));
      }
      set
      {
        SetValue(AddDateTimeProperty, value);
      }
    }

    /// <summary>
    /// Filter on property
    /// </summary>
    public static readonly DependencyProperty FilterOnProperty = DependencyProperty.Register("FilterOn", typeof(bool), typeof(LogViewerControl),
                      new PropertyMetadata(false, OnFilterOnChanged));

    /// <summary>
    /// Set filter on
    /// </summary>
    [Category("TextEditor")]
    public bool FilterOn
    {
      get
      {
        return ((bool) GetValue(FilterOnProperty));
      }
      set
      {
        SetValue(FilterOnProperty, value);
      }
    }

    /// <summary>
    /// Is control visible yes/no
    /// </summary>
    public bool IsActiv
    {
      get;
      set;
    }

    /// <summary>
    /// Searchbox open yes/no
    /// </summary>
    public bool IsSearching
    {
      get;
      set;
    }

    /// <summary>
    /// NextSearch counter
    /// </summary>
    public LogEntry NextSearch
    {
      get;
      set;
    }

    /// <summary>
    /// Wrap around while searching
    /// </summary>
    public bool WrapAround
    {
      get;
      set;
    }

    private bool bookmarLine;

    /// <summary>
    /// Bookmark lines
    /// </summary>
    public bool BookmarkLine
    {
      get
      {
        return (bookmarLine);
      }
      set
      {
        bookmarLine = value;

        if(bookmarLine)
          FindWhatTextChanged();
      }
    }

    /// <summary>
    /// Is a valid FileName entered
    /// </summary>
    public bool FileNameAvailable
    {
      get;
      set;
    }

    #endregion

    #region Public functions

    /// <summary>
    /// Append text to TextEditor
    /// </summary>
    /// <param name="logText">Text to insert</param>
    public void AppendText(string logText)
    {
      LogEntry entry = new LogEntry
      {
        Index = ++index,
        Message = logText,
        DateTime = DateTime.Now
      };

      Dispatcher.BeginInvoke((Action) (() => LogEntries.Add(entry)));

      if(AlwaysScrollIntoView)
        ScrollToEnd();
    }

    /// <summary>
    /// Find next serach item
    /// </summary>
    /// <param name="sd"></param>
    public void FindNextItem(SearchData sd)
    {
      searchBookmark = sd.SearchBookmarks;
      searchText = sd.WordToFind;

      if(sd.Count)
        SearchItemsNow(sd.Count);

      // Dispatcher.BeginInvoke allows the runtime to finish init tab control
      Dispatcher.BeginInvoke(new Action(() =>
      {
        if(newSearch)
        {
          RemoveAllSearchHighlights();
          SearchItemsNow(sd.Count);
        }

        if(sd.Count)
          return;

        FindNextSearchItem();
        LogViewer.SelectedItem = NextSearch;
        LogViewer.ScrollIntoView(NextSearch);
        SearchNextHightlightItem(FindMessageTextBox(NextSearch));
      }), DispatcherPriority.DataBind);
    }

    /// <summary>
    /// Remove the search highlights
    /// </summary>
    public void RemoveSearchHighlight()
    {
      RemoveAllSearchHighlights();
      newSearch = true;
      NextSearch = null;
      IsSearching = false;
    }

    /// <summary>
    /// Find text changed
    /// </summary>
    public void FindWhatTextChanged()
    {
      if(foundItems.Count != 0)
      {
        RemoveAllSearchHighlights();
        foundItems.Clear();
        newSearch = true;
        NextSearch = null;
      }

      if(BookmarkLine)
        RemoveAllBookmarks_Click(this, null);
    }

    /// <summary>
    /// Clears all items
    /// </summary>
    public void Clear()
    {
      index = 0;
      LogEntries.Clear();
    }

    /// <summary>
    /// Scroll into view
    /// </summary>
    public void ScrollToEnd()
    {
      if(LogViewer.Items.Count == 0)
        return;

      if(logViewScrollViewer != null)
        logViewScrollViewer.ScrollToEnd();
    }

    /// <summary>
    /// Get lines count
    /// </summary>
    public int LineCount
    {
      get
      {
        return (LogEntries.Count);
      }
    }

    /// <summary>
    /// Get search result count
    /// </summary>
    public int SearchResultCount
    {
      get
      {
        return (foundItems.Count);
      }
    }

    /// <summary>
    /// Update filters
    /// </summary>
    /// <param name="newFilter">Dictionary with filters</param>
    public void UpdateFilters(ObservableCollection<FilterData> newFilter)
    {
      filters = newFilter;
    }

    /// <summary>
    /// Go to linenumber
    /// </summary>
    /// <param name="line">linenumber</param>
    public void GoToLineNumber(int line)
    {
      logViewScrollViewer.ScrollToVerticalOffset(line - 1);
      var item = LogEntries.FirstOrDefault(x => x.Index == line);
      LogViewer.SelectedItem = item;
    }

    #endregion

    #region PropertyCallback functions

    private static void OnDataTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      if(sender.GetType() == typeof(LogViewerControl))
      {
        LogViewerControl control = sender as LogViewerControl;

        if(control != null)
        {
          control.SetTemplateState();
          control.RefreshCollectionViewSource();
        }
      }
    }

    private static void OnFilterOnChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if(sender is LogViewerControl)
      {
        LogViewerControl control = sender as LogViewerControl;

        if(control != null)
          control.RefreshCollectionViewSource();
      }
    }

    #endregion

    #region Events

    private void LogViewer_Loaded(object sender, RoutedEventArgs e)
    {
      SetTemplateState();
      logViewScrollViewer = FindVisualChild<ScrollViewer>(LogViewer);
      logViewScrollViewer.ScrollChanged += ScrollViewerChanged;
    }

    private void ScrollViewerChanged(object sender, ScrollChangedEventArgs e)
    {
      if(foundItems.Count != 0 && IsSearching && IsActiv)
        HighlightAllMatches();
    }

    private void LogViewer_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      e.Handled = true;

      //if (rightMouseButtonDown)
      //  return;
      //if (!leftMouseButtonDown)
      //  return;

      var listBox = sender as ListBox;

      if(listBox != null)
      {
        var items = listBox.SelectedItems;
        int index;
        Point mousePoint = PointToScreen(Mouse.GetPosition(this));

        if(oldMousePosition.Y != mousePoint.Y)
          mouseMove = true;

        // TODO: review me
        if(readOnlyEditor != null)
          readOnlyEditor.Visibility = System.Windows.Visibility.Collapsed;
        if(showLineEditor != null)
          showLineEditor.Visibility = System.Windows.Visibility.Visible;

        try
        {
          foreach(LogEntry item in items)
          {
            index = items.IndexOf(item);

            if(index == 0)
            {
              ListBoxItem myListBoxItem = (ListBoxItem) (LogViewer.ItemContainerGenerator.ContainerFromItem(item));
              ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(myListBoxItem);

              if(myContentPresenter == null)
                continue;

              DataTemplate myDataTemplate = myContentPresenter.ContentTemplateSelector.SelectTemplate(myListBoxItem, LogViewer);

              if(!mouseMove && leftMouseButtonDown)
              {
                if(TemplateData.State == TemplateData.TemplateStates.ShowDateTimeLineNumber || TemplateData.State == TemplateData.TemplateStates.ShowLineNumber)
                {
                  TextBlock target = (TextBlock) myDataTemplate.FindName("txtBoxLineNumbers", myContentPresenter);
                  Point relativePoint = target.PointToScreen(new Point(0, 0));
                  Size s = GetSizeFromText(target);
                  s.Width += 10; // see XAML Margin style from txtBoxLineNumbers! Margin (5, 0, 5, 0)

                  relativePoint.X = relativePoint.X - (10 / 2);
                  System.Drawing.Rectangle rcTextBox = new System.Drawing.Rectangle((int) relativePoint.X, (int) relativePoint.Y, (int) s.Width, (int) s.Height);

                  if(rcTextBox.Contains((int) mousePoint.X, (int) mousePoint.Y) && leftMouseButtonDown)
                  {
                    System.Windows.Resources.StreamResourceInfo info = Application.GetResourceStream(new Uri("/TailForWin;component/Template/TextEditor/Res/RightArrow.cur", UriKind.Relative));

                    if(info != null)
                      Cursor = new Cursor(info.Stream);

                    // fullSelectionBox = true;
                    // LogViewer.ItemContainerStyle = (Style) FindResource ("FullSelectionBox");
                  }
                }
              }
            }
          }
        }
        catch(Exception ex)
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        }
      }

      LogMouseEvents("--- SelectionChanged ---");
    }

    private void CollectionViewSourceFilter(object sender, FilterEventArgs e)
    {
      if(FilterOn)
      {
        LogEntry logEntry = e.Item as LogEntry;

        if(logEntry != null)
        {
          if(TriggerAlert(logEntry))
            e.Accepted = true;
          else
            e.Accepted = false;
        }
      }
    }

    #endregion

    #region MouseEvents

    private void LogViewer_MouseMove(object sender, MouseEventArgs e)
    {
      LogMouseEvents("LogViewer_MouseMove");
    }

    private void LogViewer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if(sender.GetType() != typeof(ListBox))
        return;
      if(!FileNameAvailable)
        return;
      if(!leftMouseButtonDown)
        return;

      ListBox lb = sender as ListBox;
      var item = lb.SelectedItem as LogEntry;
      //Point mousePoint = PointToScreen (Mouse.GetPosition (this));

      if(item == null)
        return;
      if(string.IsNullOrEmpty(item.Message))
        return;

      ListBoxItem myListBoxItem = (ListBoxItem) (LogViewer.ItemContainerGenerator.ContainerFromItem(item));
      ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(myListBoxItem);

      if(myContentPresenter == null)
        return;

      DataTemplate myDataTemplate = myContentPresenter.ContentTemplateSelector.SelectTemplate(myListBoxItem, LogViewer);
      showLineEditor = (TextBlock) myDataTemplate.FindName("txtBoxMessage", myContentPresenter);
      readOnlyEditor = (TextBox) myDataTemplate.FindName("txtEditMessage", myContentPresenter);

      if(showLineEditor == null)
        return;
      if(readOnlyEditor == null)
        return;

      showLineEditor.Visibility = System.Windows.Visibility.Hidden;
      readOnlyEditor.Visibility = System.Windows.Visibility.Visible;

      //Point relativePoint = target.PointToScreen (new Point (0, 0));
      //Size s = GetSizeFromText (target);
    }

    private void LogViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if(rightMouseButtonDown)
        return;

      leftMouseButtonDown = true;
      oldMousePosition = PointToScreen(Mouse.GetPosition(this));
    }

    private void LogViewer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      mouseLeftButtonDownCounter++;

      if(mouseLeftButtonDownCounter >= 2)
        return;
      if(rightMouseButtonDown)
        return;
      if(sender.GetType() != typeof(ListBox))
        return;

      LogViewer.ContextMenu = null;
      ListBox lb = sender as ListBox;
      LogEntry item = lb.SelectedItem as LogEntry;
      Point mousePoint = PointToScreen(Mouse.GetPosition(this));

      if(item == null)
        return;

      System.Drawing.Rectangle? rcBreakpoint = MouseButtonDownHelper(item);

      if(rcBreakpoint == null)
        return;
      if(!(((System.Drawing.Rectangle) rcBreakpoint).Contains((int) mousePoint.X, (int) mousePoint.Y) && leftMouseButtonDown))
        return;

      System.Windows.Media.Imaging.BitmapImage bp = new System.Windows.Media.Imaging.BitmapImage();
      bp.BeginInit();
      bp.UriSource = new Uri("/TailForWin;component/Template/TextEditor/Res/breakpoint.png", UriKind.Relative);
      bp.EndInit();

      RenderOptions.SetBitmapScalingMode(bp, BitmapScalingMode.NearestNeighbor);
      RenderOptions.SetEdgeMode(bp, EdgeMode.Aliased);

      if(item.BookmarkPoint == null)
        item.BookmarkPoint = bp;
      else
        item.BookmarkPoint = null;
    }

    private void LogViewer_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      if(leftMouseButtonDown)
        return;

      rightMouseButtonDown = true;
    }

    private void LogViewer_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      if(leftMouseButtonDown)
        return;
      if(sender.GetType() != typeof(ListBox))
        return;

      LogViewer.ContextMenu = null;
      ListBox lb = sender as ListBox;
      var items = lb.SelectedItems;
      Point mousePoint = PointToScreen(Mouse.GetPosition(this));

      if(items != null)
      {
        foreach(LogEntry item in items)
        {
          if(item.BookmarkPoint == null)
            continue;

          System.Drawing.Rectangle? rcBreakpoint = MouseButtonDownHelper(item);

          if(rcBreakpoint == null)
            continue;
          if(!(((System.Drawing.Rectangle) rcBreakpoint).Contains((int) mousePoint.X, (int) mousePoint.Y) && rightMouseButtonDown))
            continue;

          System.Windows.Media.Imaging.BitmapImage icon = new System.Windows.Media.Imaging.BitmapImage(new Uri("/TailForWin;component/Template/TextEditor/Res/trash-icon.png", UriKind.Relative));
          RenderOptions.SetBitmapScalingMode(icon, BitmapScalingMode.NearestNeighbor);
          RenderOptions.SetEdgeMode(icon, EdgeMode.Aliased);

          ContextMenu menu = new ContextMenu();
          MenuItem menuItem = new MenuItem
          {
            Header = "Delete all Bookmarks",
            Icon = new Image
            {
              Source = icon
            }
          };

          menuItem.Click += RemoveAllBookmarks_Click;
          menu.Items.Add(menuItem);

          LogViewer.ContextMenu = menu;
          break;
        }
      }
    }

    private void LogViewer_MouseUp(object sender, MouseButtonEventArgs e)
    {
      leftMouseButtonDown = false;
      rightMouseButtonDown = false;
      mouseMove = false;
      wordSelection = false;
      mouseLeftButtonDownCounter = 0;
      Cursor = Cursors.Arrow;
    }

    #endregion

    #region ClickEvents

    private void RemoveAllBookmarks_Click(object sender, RoutedEventArgs e)
    {
      if(sender.GetType() != typeof(LogViewerControl))
      {
        if(sender.GetType() != typeof(MenuItem))
          return;
      }
      if(sender.GetType() != typeof(MenuItem))
      {
        if(sender.GetType() != typeof(LogViewerControl))
          return;
      }

      var bookmarkItems = LogEntries.Where(item => item.BookmarkPoint != null);

      foreach(LogEntry bookmarkItem in bookmarkItems)
      {
        bookmarkItem.BookmarkPoint = null;
      }

      LogViewer.ContextMenu = null;

      if(e == null)
        return;

      e.Handled = true;
    }

    #endregion

    #region Deferred Action

    //    private void DeferredMouseMove ()
    //    {
    //      mouseMove = true;

    //#if DEBUG
    //      LogMouseEvents ("DeferedMouseMove");
    //#endif
    //    }

    #endregion

    #region Searching

    private void FindNextSearchItem()
    {
      int start;
      int end = (int) Math.Round(logViewScrollViewer.ViewportHeight);
      int stop;

      if(NextSearch == null)
        start = (int) Math.Round(logViewScrollViewer.VerticalOffset) + 1;
      else
        start = NextSearch.Index;

      // I.)
      // Look into visible items and highlight first hit
      if(ScrollIntoVisibleSearchText(start, end, out stop))
        return;

      // II.)
      // Look into hidden items and highlight first hit
      if(ScrollIntoHiddenSearchText(stop))
        return;

      if(!WrapAround)
        return;

      logViewScrollViewer.ScrollToHome();
      HightlightText(FindMessageTextBox(NextSearch));
      NextSearch = null;
      ScrollIntoHiddenSearchText(0);
    }

    private bool ScrollIntoVisibleSearchText(int start, int end, out int stop)
    {
      int counter = 0;
      stop = -1;

      for(int i = start; i <= LogEntries.Count; i++)
      {
        try
        {
          foreach(LogEntry item in foundItems.Where(item => item.Index == i))
          {
            if(NextSearch != null && item.Index == NextSearch.Index)
            {
              stop = i;
              continue;
            }

            HightlightText(FindMessageTextBox(NextSearch));
            NextSearch = item;

            return (true);
          }

          counter++;

          if(counter > end)
            break;
        }
        catch(Exception ex)
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
          continue;
        }
      }
      return (false);
    }

    private bool ScrollIntoHiddenSearchText(int start)
    {
      for(int i = start; i <= LogEntries.Count; i++)
      {
        try
        {
          foreach(LogEntry item in foundItems.Where(item => item.Index == i).Where(item => NextSearch == null || item.Index != NextSearch.Index))
          {
            HightlightText(FindMessageTextBox(NextSearch));
            NextSearch = item;

            return (true);
          }
        }
        catch(Exception ex)
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
          continue;
        }
      }
      return (false);
    }

    private void SearchItemsNow(bool count = false)
    {
      try
      {
        if(!string.IsNullOrEmpty(searchText))
        {
          foundItems.Clear();
          Regex regSearch = new Regex($"({searchText})", RegexOptions.IgnoreCase);

          foreach(LogEntry item in from item in LogEntries let substrings = regSearch.Split(item.Message) from sub in substrings where regSearch.Match(sub).Success select item)
          {
            if(BookmarkLine)
            {
              System.Windows.Media.Imaging.BitmapImage bp = new System.Windows.Media.Imaging.BitmapImage();
              bp.BeginInit();
              bp.UriSource = new Uri("/TailForWin;component/Template/TextEditor/Res/breakpoint.png", UriKind.Relative);
              bp.EndInit();

              RenderOptions.SetBitmapScalingMode(bp, BitmapScalingMode.NearestNeighbor);
              RenderOptions.SetEdgeMode(bp, EdgeMode.Aliased);

              item.BookmarkPoint = bp;
            }

            foundItems.Add(item);

            if(count)
              newSearch = true;
            else
              newSearch = false;
          }

          if(!count)
            HighlightAllMatches();
        }
        else if(searchBookmark)
        {
          foundItems.Clear();

          foreach(LogEntry item in LogEntries.Where(item => item.BookmarkPoint != null))
          {
            foundItems.Add(item);

            if(count)
              newSearch = true;
            else
              newSearch = false;
          }
        }
      }
      catch(Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    private void HighlightAllMatches()
    {
      if(foundItems.Count == 0)
        return;

      try
      {
        foreach(LogEntry item in foundItems.Where(item => NextSearch == null || item.Index != NextSearch.Index))
        {
          HightlightText(FindMessageTextBox(item));
        }
      }
      catch(Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    private void RemoveAllSearchHighlights()
    {
      if(foundItems.Count != 0)
        foundItems.ForEach(item => RemoveHightlightText(FindMessageTextBox(item)));
    }

    private TextBlock FindMessageTextBox(LogEntry item)
    {
      TextBlock target = null;

      ListBoxItem myListBoxItem = (ListBoxItem) (LogViewer.ItemContainerGenerator.ContainerFromItem(item));
      ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(myListBoxItem);

      if(myContentPresenter != null)
      {
        DataTemplate myDataTemplate = myContentPresenter.ContentTemplateSelector.SelectTemplate(myListBoxItem, LogViewer);
        target = (TextBlock) myDataTemplate.FindName("txtBoxMessage", myContentPresenter);

        return (target);
      }
      return (target);
    }

    private void HightlightText(TextBlock tb)
    {
      if(tb == null)
        return;

      Regex regex = new Regex(string.Format("({0})", searchText), RegexOptions.IgnoreCase);

      if(searchText.Length == 0)
      {
        string str = tb.Text;
        tb.Inlines.Clear();
        tb.Inlines.Add(str);
        return;
      }

      string[] substrings = regex.Split(tb.Text);
      tb.Inlines.Clear();

      Array.ForEach(substrings, item =>
       {
         if(regex.Match(item).Success)
         {
           Brush searchHighlightOpacity = TextEditorSearchHighlightBackground.Clone();
           searchHighlightOpacity.Opacity = 0.5;
           Run runx = new Run(item)
           {
             Background = searchHighlightOpacity,
             Foreground = TextEditorSearchHighlightForeground
           };
           tb.Inlines.Add(runx);
         }
         else
         {
           tb.Inlines.Add(item);
         }
       });
    }

    private void RemoveHightlightText(TextBlock tb)
    {
      if(tb == null)
        return;

      Regex regex = new Regex($"({searchText})", RegexOptions.IgnoreCase);

      if(searchText.Length == 0)
      {
        string str = tb.Text;
        tb.Inlines.Clear();
        tb.Inlines.Add(str);
        return;
      }

      string[] substrings = regex.Split(tb.Text);
      tb.Inlines.Clear();

      Array.ForEach(substrings, tb.Inlines.Add);
    }

    private void SearchNextHightlightItem(TextBlock tb)
    {
      if(tb == null)
        return;

      if(searchBookmark)
        return;

      Regex regex = new Regex($"({searchText})", RegexOptions.IgnoreCase);

      if(searchText.Length == 0)
      {
        string str = tb.Text;
        tb.Inlines.Clear();
        tb.Inlines.Add(str);
      }

      string[] substrings = regex.Split(tb.Text);
      tb.Inlines.Clear();

      Array.ForEach(substrings, item =>
                                {
                                  if(regex.Match(item).Success)
                                  {
                                    Run runx = new Run(item)
                                    {
                                      Background = TextEditorSearchHighlightBackground,
                                      Foreground = TextEditorSearchHighlightForeground
                                    };
                                    tb.Inlines.Add(runx);
                                  }
                                  else
                                    tb.Inlines.Add(item);
                                });
    }

    #endregion

    #region Helperfunctions

    private System.Drawing.Rectangle? MouseButtonDownHelper(LogEntry item)
    {
      ListBoxItem myListBoxItem = (ListBoxItem) (LogViewer.ItemContainerGenerator.ContainerFromItem(item));
      ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(myListBoxItem);

      if(myContentPresenter == null)
        return (null);

      DataTemplate myDataTemplate = myContentPresenter.ContentTemplateSelector.SelectTemplate(myListBoxItem, LogViewer);

      TextBlock text = (TextBlock) myDataTemplate.FindName("txtBoxMessage", myContentPresenter);
      Image target = (Image) myDataTemplate.FindName("txtBoxBreakpoint", myContentPresenter);
      Point relativePoint = target.PointToScreen(new Point(0, 0));

      Size s = new Size(16, 16);
      Size textSize = GetSizeFromText(text);

      if(textSize.Height >= 16)
        s.Height = textSize.Height;

      // very strange behaviour! when image is shown, no correction is needed, otherwise it is needed??? WTF!
      if(item.BookmarkPoint != null)
        return (new System.Drawing.Rectangle((int) relativePoint.X, (int) relativePoint.Y, (int) s.Width, (int) s.Height));

      relativePoint.X = relativePoint.X - (s.Width / 2);
      relativePoint.Y = relativePoint.Y - (s.Height / 2);

      return (new System.Drawing.Rectangle((int) relativePoint.X, (int) relativePoint.Y, (int) s.Width, (int) s.Height));
    }

    private void RefreshCollectionViewSource()
    {
      if(LogViewer.ItemsSource != null)
        CollectionViewSource.GetDefaultView(LogViewer.ItemsSource).Refresh();
    }

    private bool TriggerAlert(LogEntry newItem)
    {
      if(filters.Count == 0)
        return (false);

      bool success = false;

      foreach(FilterData pair in filters)
      {
        Regex regSearch = new Regex($"({pair.Filter})", RegexOptions.IgnoreCase);
        string[] substrings = regSearch.Split(newItem.Message);

        Array.ForEach(substrings, sub =>
         {
           if(!regSearch.Match(sub).Success)
             return;

           AlertTriggerEventArgs triggerData = new AlertTriggerEventArgs(newItem);
           Alert?.Invoke(this, triggerData);

           success = true;
         });
      }
      return (success);
    }

    private void SetTemplateState()
    {
      if(ShowDateTime && ShowLineNumbers)
      {
        TemplateData.State = TemplateData.TemplateStates.ShowDateTimeLineNumber;
        AddDateTime = true;
      }
      else if(ShowLineNumbers)
      {
        TemplateData.State = TemplateData.TemplateStates.ShowLineNumber;
        AddDateTime = false;
      }
      else if(ShowDateTime)
      {
        TemplateData.State = TemplateData.TemplateStates.ShowDateTime;
        AddDateTime = true;
      }
      else
      {
        TemplateData.State = TemplateData.TemplateStates.ShowDefault;
        AddDateTime = false;
      }
    }

    private static Size GetSizeFromText(TextBlock target)
    {
      FormattedText txtSize = new FormattedText(target.Text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
        new Typeface(target.FontFamily, target.FontStyle, target.FontWeight, target.FontStretch), target.FontSize, Brushes.Black);

      return (new Size(txtSize.Width, txtSize.Height));
    }

    private static childItem FindVisualChild<childItem>(DependencyObject obj)
      where childItem : DependencyObject
    {
      if(obj == null)
        return (null);

      for(int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
      {
        DependencyObject child = VisualTreeHelper.GetChild(obj, i);

        if(child is childItem)
          return ((childItem) child);

        childItem childOfChild = FindVisualChild<childItem>(child);

        if(childOfChild != null)
          return (childOfChild);
      }
      return (null);
    }

    private void LogMouseEvents(string mouseEvent)
    {
      LOG.Debug("{0}", mouseEvent);
      //LOG.Debug("LeftMouseButtonDown {0} / RightMouseButtonDown {1}", leftMouseButtonDown, rightMouseButtonDown);
      //LOG.Debug("OldMousePosition X {0} / OldMousePosition Y {1}", oldMousePosition.X, oldMousePosition.Y);
      //LOG.Debug("MouseMove {0} / WordSelection {1}", mouseMove, wordSelection);
    }

    #endregion
  }
}
