using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Windows.Media;
using System;
using System.Windows.Input;
using System.Globalization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using TailForWin.Template.TextEditor.Utils;
using TailForWin.Template.TextEditor.Data;
using TailForWin.Template.TextEditor.Converter;
using TailForWin.Data;
using System.Windows.Threading;
using System.Windows.Data;
using System.Linq;


namespace TailForWin.Template.TextEditor
{
  /// <summary>
  /// Interaction logic for LogViewerControl.xaml
  /// </summary>
  public partial class LogViewerControl
  {
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
    private bool selectMouseItems;
    private Point oldMousePosition;
    //private readonly DeferredAction deferredOnMouseMove;
    //private readonly TimeSpan onMouseMoveDelay = TimeSpan.FromMilliseconds (200.0);
    private ScrollViewer logViewScrollViewer;
    private bool newSearch;
    private TextBox readOnlyEditor;
    private TextBlock showLineEditor;
    private readonly List<object> selectedItems = new List<object> ( );
    private object anchor, lead;

    /// <summary>
    /// Index for LogEntries and linenumbers
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


    public LogViewerControl ()
    {
      InitializeComponent ( );

      LogViewer.PreviewMouseLeftButtonDown += LogViewer_PreviewMouseLeftButtonDown;
      LogViewer.PreviewMouseRightButtonDown += LogViewer_PreviewMouseRightButtonDown;
      LogViewer.AddHandler (MouseLeftButtonDownEvent, new MouseButtonEventHandler (LogViewer_MouseLeftButtonDown), true);
      LogViewer.AddHandler (MouseRightButtonDownEvent, new MouseButtonEventHandler (LogViewer_MouseRightButtonDown), true);
      LogViewer.AddHandler (MouseDoubleClickEvent, new MouseButtonEventHandler (LogViewer_MouseDoubleClick), true);
      LogViewer.PreviewMouseLeftButtonUp += LogViewer_MouseUp;
      LogViewer.PreviewMouseRightButtonUp += LogViewer_MouseUp;
      LogViewer.MouseMove += LogViewer_MouseMove;
      LogViewer.SelectionChanged += LogViewer_SelectionChanged;

      LogEntries = new ObservableCollection<LogEntry> ( );
      collectionViewSource = new CollectionViewSource
      {
        Source = LogEntries
      };
      collectionViewSource.Filter += CollectionViewSourceFilter;
      LogViewer.DataContext = collectionViewSource;
      filters = new ObservableCollection<FilterData> ( );
      foundItems = new List<LogEntry> ( );

      StringFormatData.StringFormat = "dd.MM.yyyy HH:mm:ss.fff";
      newSearch = true;
    }

    #region Public properties

    public static readonly DependencyProperty ShowLineNumbersProperty = DependencyProperty.Register ("ShowLineNumbers", typeof (bool), typeof (LogViewerControl),
                              new PropertyMetadata (false, OnDataTemplateChanged));

    [Category ("TextEditor")]
    public bool ShowLineNumbers
    {
      get
      {
        return ((bool) GetValue (ShowLineNumbersProperty));
      }
      set
      {
        SetValue (ShowLineNumbersProperty, value);
      }
    }

    public static readonly DependencyProperty LineNumbersColorProperty = DependencyProperty.Register ("LineNumbersColor", typeof (Brush), typeof (LogViewerControl),
                              new PropertyMetadata (Brushes.Gray));

    [Category ("TextEditor")]
    public Brush LineNumbersColor
    {
      get
      {
        return ((Brush) GetValue (LineNumbersColorProperty));
      }
      set
      {
        SetValue (LineNumbersColorProperty, value);
      }
    }

    public static readonly DependencyProperty LineNumbersBackgroundColorProperty = DependencyProperty.Register ("LineNumbersBackgroundColor", typeof (Brush), typeof (LogViewerControl),
                              new PropertyMetadata (Brushes.LightGray));

    [Category ("TextEditor")]
    public Brush LineNumbersBackgroundColor
    {
      get
      {
        return ((Brush) GetValue (LineNumbersBackgroundColorProperty));
      }
      set
      {
        SetValue (LineNumbersBackgroundColorProperty, value);
      }
    }

    public static readonly DependencyProperty WordWrappingProperty = DependencyProperty.Register ("WordWrapping", typeof (bool), typeof (LogViewerControl),
                              new PropertyMetadata (false));

    [Category ("TextEditor")]
    public bool WordWrapping
    {
      get
      {
        return ((bool) GetValue (WordWrappingProperty));
      }
      set
      {
        SetValue (WordWrappingProperty, value);
      }
    }

    public static readonly DependencyProperty VerticalScrollBarVisibleProperty = DependencyProperty.Register ("VerticalScrollBarVisible", typeof (ScrollBarVisibility), typeof (LogViewerControl),
                              new PropertyMetadata (ScrollBarVisibility.Auto));

    [Category ("TextEditor")]
    public ScrollBarVisibility VerticalScrollbarVisible
    {
      get
      {
        return ((ScrollBarVisibility) GetValue (VerticalScrollBarVisibleProperty));
      }
      set
      {
        SetValue (VerticalScrollBarVisibleProperty, value);
      }
    }

    public static readonly DependencyProperty TextEditorBackGroundColorProperty = DependencyProperty.Register ("TextEditorBackgroundColor", typeof (Brush), typeof (LogViewerControl),
                              new PropertyMetadata (Brushes.White));

    [Category ("TextEditor")]
    public Brush TextEditorBackgroundColor
    {
      get
      {
        return ((Brush) GetValue (TextEditorBackGroundColorProperty));
      }
      set
      {
        SetValue (TextEditorBackGroundColorProperty, value);
      }
    }

    public static readonly DependencyProperty TextEditorForegroundColorProperty = DependencyProperty.Register ("TextEditorForegroundColor", typeof (Brush), typeof (LogViewerControl),
                              new PropertyMetadata (Brushes.Black));

    [Category ("TextEditor")]
    public Brush TextEditorForegroundColor
    {
      get
      {
        return ((Brush) GetValue (TextEditorForegroundColorProperty));
      }
      set
      {
        SetValue (TextEditorForegroundColorProperty, value);
      }
    }

    public static readonly DependencyProperty ShowDateTimeProperty = DependencyProperty.Register ("ShowDateTime", typeof (bool), typeof (LogViewerControl),
                              new PropertyMetadata (false, OnDataTemplateChanged));

    [Category ("TextEditor")]
    public bool ShowDateTime
    {
      get
      {
        return ((bool) GetValue (ShowDateTimeProperty));
      }
      set
      {
        SetValue (ShowDateTimeProperty, value);
      }
    }

    public static readonly DependencyProperty TextEditorFontStyleProperty = DependencyProperty.Register ("TextEditorFontStyle", typeof (FontStyle), typeof (LogViewerControl),
                              new PropertyMetadata (FontStyles.Normal));

    [Category ("TextEditor")]
    public FontStyle TextEditorFontStyle
    {
      get
      {
        return ((FontStyle) GetValue (TextEditorFontStyleProperty));
      }
      set
      {
        SetValue (TextEditorFontStyleProperty, value);
      }
    }

    public static readonly DependencyProperty TextEditorFontFamilyProperty = DependencyProperty.Register ("TextEditorFontFamily", typeof (FontFamily), typeof (LogViewerControl),
                              new PropertyMetadata (new FontFamily ("Segoe UI")));

    [Category ("TextEditor")]
    public FontFamily TextEditorFontFamily
    {
      get
      {
        return ((FontFamily) GetValue (TextEditorFontFamilyProperty));
      }
      set
      {
        SetValue (TextEditorFontFamilyProperty, value);
      }
    }

    public static readonly DependencyProperty TextEditorFontWeightProperty = DependencyProperty.Register ("TextEditorFontWeight", typeof (FontWeight), typeof (LogViewerControl),
                              new PropertyMetadata (FontWeights.Normal));

    [Category ("TextEditor")]
    public FontWeight TextEditorFontWeight
    {
      get
      {
        return ((FontWeight) GetValue (TextEditorFontWeightProperty));
      }
      set
      {
        SetValue (TextEditorFontWeightProperty, value);
      }
    }

    public static readonly DependencyProperty TextEditorFontSizeProperty = DependencyProperty.Register ("TextEditorFontSize", typeof (int), typeof (LogViewerControl),
                              new PropertyMetadata (12));

    [Category ("TextEditor")]
    public int TextEditorFontSize
    {
      get
      {
        return ((int) GetValue (TextEditorFontSizeProperty));
      }
      set
      {
        SetValue (TextEditorFontSizeProperty, value);
      }
    }

    public static readonly DependencyProperty TextEditorSelectionColorProperty = DependencyProperty.Register ("TextEditorSelectionColor", typeof (Color),
                              typeof (LogViewerControl), new PropertyMetadata (Colors.Blue));

    [Category ("TextEditor")]
    public Color TextEditorSelectionColor
    {
      get
      {
        return ((Color) GetValue (TextEditorSelectionColorProperty));
      }
      set
      {
        SetValue (TextEditorSelectionColorProperty, value);
      }
    }

    public static readonly DependencyProperty AlwaysScrollIntoViewProperty = DependencyProperty.Register ("AlwaysScrollIntoView", typeof (bool), typeof (LogViewerControl),
                          new PropertyMetadata (true));

    [Category ("TextEditor")]
    public bool AlwaysScrollIntoView
    {
      get
      {
        return ((bool) GetValue (AlwaysScrollIntoViewProperty));
      }
      set
      {
        SetValue (AlwaysScrollIntoViewProperty, value);
      }
    }

    public static readonly DependencyProperty TextEditorSearchHighlightBackgroundProperty = DependencyProperty.Register ("TextEditorSearchHighlightBackground", typeof (Brush), typeof (LogViewerControl),
                          new PropertyMetadata (Brushes.Red));

    [Category ("TextEditor")]
    public Brush TextEditorSearchHighlightBackground
    {
      get
      {
        return ((Brush) GetValue (TextEditorSearchHighlightBackgroundProperty));
      }
      set
      {
        SetValue (TextEditorSearchHighlightBackgroundProperty, value);
      }
    }

    public static readonly DependencyProperty TextEditorSearchHighlightForegroundProperty = DependencyProperty.Register ("TextEditorSearchHighlightForeground", typeof (Brush), typeof (LogViewerControl),
                      new PropertyMetadata (Brushes.WhiteSmoke));

    [Category ("TextEditor")]
    public Brush TextEditorSearchHighlightForeground
    {
      get
      {
        return ((Brush) GetValue (TextEditorSearchHighlightForegroundProperty));
      }
      set
      {
        SetValue (TextEditorSearchHighlightForegroundProperty, value);
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

    private static readonly DependencyProperty AddDateTimeProperty = DependencyProperty.Register ("AddDateTime", typeof (bool), typeof (LogViewerControl),
                      new PropertyMetadata (true));

    private bool AddDateTime
    {
      get
      {
        return ((bool) GetValue (AddDateTimeProperty));
      }
      set
      {
        SetValue (AddDateTimeProperty, value);
      }
    }

    public static readonly DependencyProperty FilterOnProperty = DependencyProperty.Register ("FilterOn", typeof (bool), typeof (LogViewerControl),
                      new PropertyMetadata (false, OnFilterOnChanged));

    [Category ("TextEditor")]
    public bool FilterOn
    {
      get
      {
        return ((bool) GetValue (FilterOnProperty));
      }
      set
      {
        SetValue (FilterOnProperty, value);
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

        if (bookmarLine)
          FindWhatTextChanged ( );
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
    public void AppendText (string logText)
    {
      LogEntry entry = new LogEntry
      {
        Index = ++index,
        Message = logText,
        DateTime = DateTime.Now
      };

      Dispatcher.BeginInvoke ((Action) (() => LogEntries.Add (entry)));

      if (AlwaysScrollIntoView)
        ScrollToEnd ( );
    }

    /// <summary>
    /// Find next serach item
    /// </summary>
    /// <param name="sd"></param>
    public void FindNextItem (SearchData sd)
    {
      searchBookmark = sd.SearchBookmarks;
      searchText = sd.WordToFind;

      if (sd.Count)
        SearchItemsNow (sd.Count);

      // Dispatcher.BeginInvoke allows the runtime to finish init tab control
      Dispatcher.BeginInvoke (new Action (() =>
      {
        if (newSearch)
        {
          RemoveAllSearchHighlights ( );
          SearchItemsNow (sd.Count);
        }

        if (sd.Count)
          return;

        FindNextSearchItem ( );
        LogViewer.SelectedItem = NextSearch;
        LogViewer.ScrollIntoView (NextSearch);
        SearchNextHightlightItem (FindMessageTextBox (NextSearch));
      }), DispatcherPriority.DataBind);
    }

    /// <summary>
    /// Remove the search highlights
    /// </summary>
    public void RemoveSearchHighlight ()
    {
      RemoveAllSearchHighlights ( );
      newSearch = true;
      NextSearch = null;
      IsSearching = false;
    }

    /// <summary>
    /// Find text changed
    /// </summary>
    public void FindWhatTextChanged ()
    {
      if (foundItems.Count != 0)
      {
        RemoveAllSearchHighlights ( );
        foundItems.Clear ( );
        newSearch = true;
        NextSearch = null;
      }

      if (BookmarkLine)
        RemoveAllBookmarks_Click (null, null);
    }

    /// <summary>
    /// Clears all items
    /// </summary>
    public void Clear ()
    {
      index = 0;
      LogEntries.Clear ( );
    }

    /// <summary>
    /// Scroll into view
    /// </summary>
    public void ScrollToEnd ()
    {
      if (LogViewer.Items.Count == 0)
        return;

      if (logViewScrollViewer != null)
        logViewScrollViewer.ScrollToEnd ( );
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
    public void UpdateFilters (ObservableCollection<FilterData> newFilter)
    {
      filters = newFilter;
    }

    /// <summary>
    /// Go to linenumber
    /// </summary>
    /// <param name="line">linenumber</param>
    public void GoToLineNumber (int line)
    {
      logViewScrollViewer.ScrollToVerticalOffset (line - 1);
      var item = LogEntries.FirstOrDefault (x => x.Index == line);
      LogViewer.SelectedItem = item;
    }

    #endregion

    #region PropertyCallback functions

    private static void OnDataTemplateChanged (DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      if (sender.GetType ( ) == typeof (LogViewerControl))
      {
        LogViewerControl control = sender as LogViewerControl;

        if (control != null)
        {
          control.SetTemplateState ( );
          control.RefreshCollectionViewSource ( );
        }
      }
    }

    private static void OnFilterOnChanged (object sender, DependencyPropertyChangedEventArgs e)
    {
      if (sender.GetType ( ) == typeof (LogViewerControl))
      {
        LogViewerControl control = sender as LogViewerControl;

        if (control != null)
          control.RefreshCollectionViewSource ( );
      }
    }

    #endregion

    #region Events

    private void LogViewer_Loaded (object sender, RoutedEventArgs e)
    {
      SetTemplateState ( );
      logViewScrollViewer = FindVisualChild<ScrollViewer> (LogViewer);
      logViewScrollViewer.ScrollChanged += ScrollViewerChanged;
    }

    private void ScrollViewerChanged (object sender, ScrollChangedEventArgs e)
    {
      if (foundItems.Count != 0 && IsSearching && IsActiv)
        HighlightAllMatches ( );
    }

    private void LogViewer_SelectionChanged (object sender, SelectionChangedEventArgs e)
    {
      e.Handled = true;

      if (rightMouseButtonDown)
        return;
      if (!leftMouseButtonDown)
        return;

      var listBox = sender as ListBox;

      if (listBox != null)
      {
        var items = listBox.SelectedItems;
        int index;
        Point mousePoint = PointToScreen (Mouse.GetPosition (this));

        if (oldMousePosition.Y != mousePoint.Y)
          mouseMove = true;

        // TODO: review me
        if (readOnlyEditor != null)
          readOnlyEditor.Visibility = System.Windows.Visibility.Collapsed;
        if (showLineEditor != null)
          showLineEditor.Visibility = System.Windows.Visibility.Visible;

        try
        {
          foreach (LogEntry item in items)
          {
            index = items.IndexOf (item);

            if (index == 0)
            {
              ListBoxItem myListBoxItem = (ListBoxItem) (LogViewer.ItemContainerGenerator.ContainerFromItem (item));
              ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter> (myListBoxItem);

              if (myContentPresenter == null)
                return;

              DataTemplate myDataTemplate = myContentPresenter.ContentTemplateSelector.SelectTemplate (myListBoxItem, LogViewer);

              if (!mouseMove && leftMouseButtonDown)
              {
                if (TemplateData.State == TemplateData.TemplateStates.ShowDateTimeLineNumber || TemplateData.State == TemplateData.TemplateStates.ShowLineNumber)
                {
                  TextBlock target = (TextBlock) myDataTemplate.FindName ("txtBoxLineNumbers", myContentPresenter);
                  Point relativePoint = target.PointToScreen (new Point (0, 0));
                  Size s = GetSizeFromText (target);
                  s.Width += 10; // see XAML Margin style from txtBoxLineNumbers! Margin (5, 0, 5, 0)

                  relativePoint.X = relativePoint.X - (10 / 2);
                  System.Drawing.Rectangle rcTextBox = new System.Drawing.Rectangle ((int) relativePoint.X, (int) relativePoint.Y, (int) s.Width, (int) s.Height);

                  if (rcTextBox.Contains ((int) mousePoint.X, (int) mousePoint.Y) && leftMouseButtonDown)
                  {
                    System.Windows.Resources.StreamResourceInfo info = Application.GetResourceStream (new Uri ("/TailForWin;component/Template/TextEditor/RightArrow.cur", UriKind.Relative));

                    if (info != null)
                      Cursor = new Cursor (info.Stream);

                    // fullSelectionBox = true;
                    LogViewer.ItemContainerStyle = (Style) FindResource ("FullSelectionBox");
                  }
                }
              }
            }
          }
        }
        catch (Exception ex)
        {
          TailForWin.Utils.ErrorLog.WriteLog (TailForWin.Utils.ErrorFlags.Error, GetType ( ).Name, string.Format ("{0}, exception: {1}", System.Reflection.MethodBase.GetCurrentMethod ( ).Name, ex));
        }
      }

#if DEBUG
      LogMouseEvents ("--- SelectionChanged ---");
#endif
    }

    private void CollectionViewSourceFilter (object sender, FilterEventArgs e)
    {
      if (FilterOn)
      {
        LogEntry logEntry = e.Item as LogEntry;

        if (logEntry != null)
        {
          if (TriggerAlert (logEntry))
            e.Accepted = true;
          else
            e.Accepted = false;
        }
      }
    }

    #endregion

    #region MouseEvents

    private void LogViewer_MouseMove (object sender, MouseEventArgs e)
    {
      DateTime now = DateTime.Now;

      if (!leftMouseButtonDown)
        return;
      if (!selectMouseItems)
        return;
      if (sender.GetType ( ) != typeof (ListBox))
        return;

      ListBox lb = sender as ListBox;
      var item = lb.SelectedItem as LogEntry;

      if (item != null)
      {
        ListBoxItem listBoxItem = (ListBoxItem)(LogViewer.ItemContainerGenerator.ContainerFromItem (item));

        if (listBoxItem != null)
        {
          if (lead != listBoxItem)
          {
            var last = lead;
            lead = listBoxItem;

            if (selectedItems.Contains (lead))
              selectedItems.Remove (last);
            else
              selectedItems.Add (lead);
          }
        }
      }

      selectedItems.ForEach (lbi => ((ListBoxItem) lbi).IsSelected = true);

      DateTime complete = DateTime.Now;
      TimeSpan duration = complete.Subtract (now);
      Console.WriteLine (@"Duration in millisec {0}", duration.Milliseconds);

      //if (mouseMove && leftMouseButtonDown)
      //{
      //  LogViewer.ItemContainerStyle = (Style) FindResource ("LeftAligned");
      //  fullSelectionBox = false;
      //}

      //if (!fullSelectionBox)
      //{
      //  Point mousePosition = PointToScreen (Mouse.GetPosition (this));

      //  if (oldMousePosition.X != mousePosition.X)
      //    wordSelection = true;
      //}

      //if (e.LeftButton != MouseButtonState.Pressed)
      //{
      //  LogViewer_MouseUp (null, null);
      //}

      //if (deferredOnMouseMove == null)
      //  deferredOnMouseMove = DeferredAction.Create (() => DeferredMouseMove ( ));

      //deferredOnMouseMove.Defer (onMouseMoveDelay);

#if DEBUG
      LogMouseEvents ("MouseMove");
#endif
    }

    private void LogViewer_MouseDoubleClick (object sender, MouseButtonEventArgs e)
    {
      if (sender.GetType ( ) != typeof (ListBox))
        return;
      if (!FileNameAvailable)
        return;

      ListBox lb = sender as ListBox;
      var item = lb.SelectedItem as LogEntry;
      //Point mousePoint = PointToScreen (Mouse.GetPosition (this));

      if (string.IsNullOrEmpty (item.Message))
        return;

      if (item != null)
      {
        ListBoxItem myListBoxItem = (ListBoxItem) (LogViewer.ItemContainerGenerator.ContainerFromItem (item));
        ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter> (myListBoxItem);

        if (myContentPresenter == null)
          return;

        DataTemplate myDataTemplate = myContentPresenter.ContentTemplateSelector.SelectTemplate (myListBoxItem, LogViewer);
        showLineEditor = (TextBlock) myDataTemplate.FindName ("txtBoxMessage", myContentPresenter);
        readOnlyEditor = (TextBox) myDataTemplate.FindName ("txtEditMessage", myContentPresenter);

        if (showLineEditor == null)
          return;
        if (readOnlyEditor == null)
          return;

        showLineEditor.Visibility = System.Windows.Visibility.Hidden;
        readOnlyEditor.Visibility = System.Windows.Visibility.Visible;

        //Point relativePoint = target.PointToScreen (new Point (0, 0));
        //Size s = GetSizeFromText (target);
      }

#if DEBUG
      LogMouseEvents ("MouseDoubleClick");
#endif
    }

    private void LogViewer_PreviewMouseLeftButtonDown (object sender, MouseButtonEventArgs e)
    {
      if (rightMouseButtonDown)
        return;

      leftMouseButtonDown = true;
      oldMousePosition = PointToScreen (Mouse.GetPosition (this));
#if DEBUG
      LogMouseEvents ("PreviewMouseLeftButtonDown");
#endif
    }

    private void LogViewer_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
    {
      if (rightMouseButtonDown)
        return;
      if (sender.GetType ( ) != typeof (ListBox))
        return;

      LogViewer.ContextMenu = null;
      ListBox lb = sender as ListBox;
      LogEntry item = lb.SelectedItem as LogEntry;
      Point mousePoint = PointToScreen (Mouse.GetPosition (this));

      if (item == null)
        return;

      System.Drawing.Rectangle? rcBreakpoint = MouseButtonDownHelper (item);

      if (rcBreakpoint != null)
      {
        if (((System.Drawing.Rectangle) rcBreakpoint).Contains ((int) mousePoint.X, (int) mousePoint.Y) && leftMouseButtonDown)
        {
          System.Windows.Media.Imaging.BitmapImage bp = new System.Windows.Media.Imaging.BitmapImage ( );
          bp.BeginInit ( );
          bp.UriSource = new Uri ("/TailForWin;component/Template/TextEditor/breakpoint.gif", UriKind.Relative);
          bp.EndInit ( );

          if (item.BookmarkPoint == null)
            item.BookmarkPoint = bp;
          else
            item.BookmarkPoint = null;
        }
      }

      ListBoxItem listboxItem = (ListBoxItem) (LogViewer.ItemContainerGenerator.ContainerFromItem (item));

      if (listboxItem == null)
        return;

      selectedItems.Clear ( );
      selectedItems.Add (listboxItem);
      selectMouseItems = true;
      
#if DEBUG
      LogMouseEvents ("MouseLeftButtonDown");
#endif
    }

    private void LogViewer_PreviewMouseRightButtonDown (object sender, MouseButtonEventArgs e)
    {
      if (leftMouseButtonDown)
        return;

      rightMouseButtonDown = true;

#if DEBUG
      LogMouseEvents ("PreviewMouseRightButtonDown");
#endif
    }

    private void LogViewer_MouseRightButtonDown (object sender, MouseButtonEventArgs e)
    {
      if (leftMouseButtonDown)
        return;
      if (sender.GetType ( ) != typeof (ListBox))
        return;

      LogViewer.ContextMenu = null;
      ListBox lb = sender as ListBox;
      LogEntry item = lb.SelectedItem as LogEntry;
      Point mousePoint = PointToScreen (Mouse.GetPosition (this));

      if (item != null)
      {
        if (item.BookmarkPoint != null)
        {
          System.Drawing.Rectangle? rcBreakpoint = MouseButtonDownHelper (item);

          if (rcBreakpoint != null)
          {
            if (((System.Drawing.Rectangle) rcBreakpoint).Contains ((int) mousePoint.X, (int) mousePoint.Y) && rightMouseButtonDown)
            {
              System.Windows.Media.Imaging.BitmapImage icon = new System.Windows.Media.Imaging.BitmapImage (new Uri ("/TailForWin;component/Template/TextEditor/breakpoint_delete.gif", UriKind.Relative));
              RenderOptions.SetBitmapScalingMode (icon, BitmapScalingMode.NearestNeighbor);
              RenderOptions.SetEdgeMode (icon, EdgeMode.Aliased);

              ContextMenu menu = new ContextMenu ( );
              MenuItem menuItem = new MenuItem
              {
                Header = "Delete all Bookmarks",
                Icon = new Image
                {
                  Source = icon
                }
              };

              menuItem.Click += RemoveAllBookmarks_Click;
              menu.Items.Add (menuItem);

              LogViewer.ContextMenu = menu;
            }
          }
        }
      }

#if DEBUG
      LogMouseEvents ("MouseRightButtonDown");
#endif
    }

    private void LogViewer_MouseUp (object sender, MouseButtonEventArgs e)
    {
      leftMouseButtonDown = false;
      rightMouseButtonDown = false;
      mouseMove = false;
      wordSelection = false;
      // fullSelectionBox = false;

      selectMouseItems = false;
      selectedItems.Clear ( );
      lead = null;
      anchor = null;

      Cursor = Cursors.Arrow;

#if DEBUG
      LogMouseEvents ("MouseUp");
#endif
    }

    #endregion

    #region ClickEvents

    private void RemoveAllBookmarks_Click (object sender, RoutedEventArgs e)
    {
      var bookmarkItems = LogEntries.Where (item => item.BookmarkPoint != null);

      foreach (LogEntry bookmarkItem in bookmarkItems)
      {
        bookmarkItem.BookmarkPoint = null;
      }

      LogViewer.ContextMenu = null;
    }

    #endregion

    #region Deferred Action

    private void DeferredMouseMove ()
    {
      mouseMove = true;

#if DEBUG
      LogMouseEvents ("DeferedMouseMove");
#endif
    }

    #endregion

    #region Searching

    private void FindNextSearchItem ()
    {
      int start;
      int end = (int) Math.Round (logViewScrollViewer.ViewportHeight);
      int stop;

      if (NextSearch == null)
        start = (int) Math.Round (logViewScrollViewer.VerticalOffset) + 1;
      else
        start = NextSearch.Index;

      // I.)
      // Look into visible items and highlight first hit
      if (ScrollIntoVisibleSearchText (start, end, out stop))
        return;

      // II.)
      // Look into hidden items and highlight first hit
      if (ScrollIntoHiddenSearchText (stop))
        return;

      if (!WrapAround)
        return;

      logViewScrollViewer.ScrollToHome ( );
      HightlightText (FindMessageTextBox (NextSearch));
      NextSearch = null;
      ScrollIntoHiddenSearchText (0);
    }

    private bool ScrollIntoVisibleSearchText (int start, int end, out int stop)
    {
      int counter = 0;
      stop = -1;

      for (int i = start; i <= LogEntries.Count; i++)
      {
        foreach (LogEntry item in foundItems.Where (item => item.Index == i))
        {
          if (NextSearch != null && item.Index == NextSearch.Index)
          {
            stop = i;
            continue;
          }

          HightlightText (FindMessageTextBox (NextSearch));
          NextSearch = item;

          return (true);
        }

        counter++;

        if (counter > end)
          break;
      }
      return (false);
    }

    private bool ScrollIntoHiddenSearchText (int start)
    {
      for (int i = start; i <= LogEntries.Count; i++)
      {
        foreach (LogEntry item in foundItems.Where (item => item.Index == i).Where (item => NextSearch == null || item.Index != NextSearch.Index))
        {
          HightlightText (FindMessageTextBox (NextSearch));
          NextSearch = item;

          return (true);
        }
      }
      return (false);
    }

    private void SearchItemsNow (bool count = false)
    {
      if (!string.IsNullOrEmpty (searchText))
      {
        foundItems.Clear ( );
        Regex regSearch = new Regex (string.Format ("({0})", searchText), RegexOptions.IgnoreCase);

        foreach (LogEntry item in from item in LogEntries let substrings = regSearch.Split (item.Message) from sub in substrings where regSearch.Match (sub).Success select item)
        {
          if (BookmarkLine)
          {
            System.Windows.Media.Imaging.BitmapImage bp = new System.Windows.Media.Imaging.BitmapImage ( );
            bp.BeginInit ( );
            bp.UriSource = new Uri ("/TailForWin;component/Template/TextEditor/breakpoint.gif", UriKind.Relative);
            bp.EndInit ( );

            item.BookmarkPoint = bp;
          }

          foundItems.Add (item);

          if (count)
            newSearch = true;
          else
            newSearch = false;
        }

        if (!count)
          HighlightAllMatches ( );
      }
      else if (searchBookmark)
      {
        foundItems.Clear ( );

        foreach (LogEntry item in LogEntries.Where (item => item.BookmarkPoint != null))
        {
          foundItems.Add (item);

          if (count)
            newSearch = true;
          else
            newSearch = false;
        }
      }
    }

    private void HighlightAllMatches ()
    {
      if (foundItems.Count == 0)
        return;

      foreach (LogEntry item in foundItems.Where (item => NextSearch == null || item.Index != NextSearch.Index))
      {
        HightlightText (FindMessageTextBox (item));
      }
    }

    private void RemoveAllSearchHighlights ()
    {
      if (foundItems.Count != 0)
      {
        foundItems.ForEach (item => RemoveHightlightText (FindMessageTextBox (item)));
      }
    }

    private TextBlock FindMessageTextBox (LogEntry item)
    {
      TextBlock target = null;

      ListBoxItem myListBoxItem = (ListBoxItem) (LogViewer.ItemContainerGenerator.ContainerFromItem (item));
      ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter> (myListBoxItem);

      if (myContentPresenter != null)
      {
        DataTemplate myDataTemplate = myContentPresenter.ContentTemplateSelector.SelectTemplate (myListBoxItem, LogViewer);
        target = (TextBlock) myDataTemplate.FindName ("txtBoxMessage", myContentPresenter);

        return (target);
      }
      return (target);
    }

    private void HightlightText (TextBlock tb)
    {
      if (tb == null)
        return;

      Regex regex = new Regex (string.Format ("({0})", searchText), RegexOptions.IgnoreCase);

      if (searchText.Length == 0)
      {
        string str = tb.Text;
        tb.Inlines.Clear ( );
        tb.Inlines.Add (str);
        return;
      }

      string[] substrings = regex.Split (tb.Text);
      tb.Inlines.Clear ( );

      Array.ForEach (substrings, item =>
        {
          if (regex.Match (item).Success)
          {
            Brush searchHighlightOpacity = TextEditorSearchHighlightBackground.Clone ( );
            searchHighlightOpacity.Opacity = 0.5;
            Run runx = new Run (item)
            {
              Background = searchHighlightOpacity,
              Foreground = TextEditorSearchHighlightForeground
            };
            tb.Inlines.Add (runx);
          }
          else
            tb.Inlines.Add (item);
        });
    }

    private void RemoveHightlightText (TextBlock tb)
    {
      if (tb == null)
        return;

      Regex regex = new Regex (string.Format ("({0})", searchText), RegexOptions.IgnoreCase);

      if (searchText.Length == 0)
      {
        string str = tb.Text;
        tb.Inlines.Clear ( );
        tb.Inlines.Add (str);
        return;
      }

      string[] substrings = regex.Split (tb.Text);
      tb.Inlines.Clear ( );

      Array.ForEach (substrings, tb.Inlines.Add);
    }

    private void SearchNextHightlightItem (TextBlock tb)
    {
      if (tb == null)
        return;

      if (searchBookmark)
        return;

      Regex regex = new Regex (string.Format ("({0})", searchText), RegexOptions.IgnoreCase);

      if (searchText.Length == 0)
      {
        string str = tb.Text;
        tb.Inlines.Clear ( );
        tb.Inlines.Add (str);
      }

      string[] substrings = regex.Split (tb.Text);
      tb.Inlines.Clear ( );

      Array.ForEach (substrings, item =>
                                 {
                                   if (regex.Match (item).Success)
                                   {
                                     Run runx = new Run (item)
                                                {
                                                  Background = TextEditorSearchHighlightBackground,
                                                  Foreground = TextEditorSearchHighlightForeground
                                                };
                                     tb.Inlines.Add (runx);
                                   }
                                   else
                                     tb.Inlines.Add (item);
                                 });
    }

    #endregion

    #region Helperfunctions

    private System.Drawing.Rectangle? MouseButtonDownHelper (LogEntry item)
    {
      ListBoxItem myListBoxItem = (ListBoxItem) (LogViewer.ItemContainerGenerator.ContainerFromItem (item));
      ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter> (myListBoxItem);

      if (myContentPresenter == null)
        return (null);

      DataTemplate myDataTemplate = myContentPresenter.ContentTemplateSelector.SelectTemplate (myListBoxItem, LogViewer);

      TextBlock text = (TextBlock) myDataTemplate.FindName ("txtBoxMessage", myContentPresenter);
      Image target = (Image) myDataTemplate.FindName ("txtBoxBreakpoint", myContentPresenter);
      Point relativePoint = target.PointToScreen (new Point (0, 0));

      Size s = new Size (16, 16);
      Size textSize = GetSizeFromText (text);

      if (textSize.Height >= 16)
        s.Height = textSize.Height;

      // very strange behaviour! when image is shown, no correction is needed, otherwise it is needed??? WTF!
      if (item.BookmarkPoint != null)
        return (new System.Drawing.Rectangle ((int) relativePoint.X, (int) relativePoint.Y, (int) s.Width, (int) s.Height));

      relativePoint.X = relativePoint.X - (s.Width / 2);
      relativePoint.Y = relativePoint.Y - (s.Height / 2);

      return (new System.Drawing.Rectangle ((int) relativePoint.X, (int) relativePoint.Y, (int) s.Width, (int) s.Height));
    }

    private void RefreshCollectionViewSource ()
    {
      if (LogViewer.ItemsSource != null)
        CollectionViewSource.GetDefaultView (LogViewer.ItemsSource).Refresh ( );
    }

    private bool TriggerAlert (LogEntry newItem)
    {
      if (filters.Count == 0)
        return (false);

      bool success = false;

      foreach (FilterData pair in filters)
      {
        Regex regSearch = new Regex (string.Format ("({0})", pair.Filter), RegexOptions.IgnoreCase);
        string[] substrings = regSearch.Split (newItem.Message);

        Array.ForEach (substrings, sub =>
          {
            if (!regSearch.Match (sub).Success)
              return;

            AlertTriggerEventArgs triggerData = new AlertTriggerEventArgs (newItem);

            if (Alert != null)
              Alert (this, triggerData);

            success = true;
          });
      }
      return (success);
    }

    private void SetTemplateState ()
    {
      if (ShowDateTime && ShowLineNumbers)
      {
        TemplateData.State = TemplateData.TemplateStates.ShowDateTimeLineNumber;
        AddDateTime = true;
      }
      else if (ShowLineNumbers)
      {
        TemplateData.State = TemplateData.TemplateStates.ShowLineNumber;
        AddDateTime = false;
      }
      else if (ShowDateTime)
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

    private static Size GetSizeFromText (TextBlock target)
    {
      FormattedText txtSize = new FormattedText (target.Text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
        new Typeface (target.FontFamily, target.FontStyle, target.FontWeight, target.FontStretch), target.FontSize, Brushes.Black);

      return (new Size (txtSize.Width, txtSize.Height));
    }

    private static childItem FindVisualChild<childItem> (DependencyObject obj)
        where childItem : DependencyObject
    {
      if (obj == null)
        return (null);

      for (int i = 0; i < VisualTreeHelper.GetChildrenCount (obj); i++)
      {
        DependencyObject child = VisualTreeHelper.GetChild (obj, i);

        if (child is childItem)
          return ((childItem) child);

        childItem childOfChild = FindVisualChild<childItem> (child);

        if (childOfChild != null)
          return (childOfChild);
      }
      return (null);
    }

#if DEBUG

    private void LogMouseEvents (string mouseEvent)
    {
      DateTime now = DateTime.Now;

      Console.WriteLine (@"{0} {1}", now.ToString ("hh:mm:ss.fff"), mouseEvent);
      Console.WriteLine (@"LeftMouseButtonDown {0} / RightMouseButtonDown {1}", leftMouseButtonDown, rightMouseButtonDown);
      Console.WriteLine (@"OldMousePosition X {0} / OldMousePosition Y {1}", oldMousePosition.X, oldMousePosition.Y);
      Console.WriteLine (@"MouseMove {0} / WordSelection {1}", mouseMove, wordSelection);
    }

#endif

    #endregion
  }
}
