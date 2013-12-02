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
using System.Linq;


namespace TailForWin.Template.TextEditor
{
  /// <summary>
  /// Interaction logic for LogViewerControl.xaml
  /// </summary>
  public partial class LogViewerControl: UserControl
  {
    private bool leftMouseButtonDown;
    private bool rightMouseButtonDown;
    private bool wordSelection;
    private bool mouseMove;
    private bool selectMouseItems;
    private Point oldMousePosition;
    private DeferredAction deferredOnMouseMove = null;
    private readonly TimeSpan onMouseMoveDelay = TimeSpan.FromMilliseconds (200.0);
    private int index;
    private string SearchText;


    public LogViewerControl ()
    {
      InitializeComponent ( );

      LogViewer.PreviewMouseLeftButtonDown += LogViewer_PreviewMouseLeftButtonDown;
      LogViewer.PreviewMouseRightButtonDown += LogViewer_PreviewMouseRightButtonDown;
      // LogViewer.AddHandler (MouseLeftButtonDownEvent, new MouseButtonEventHandler (LogViewer_MouseLeftButtonDown), true);
      // LogViewer.AddHandler (MouseRightButtonDownEvent, new MouseButtonEventHandler (LogViewer_MouseRightButtonDown), true);
      LogViewer.AddHandler (MouseDoubleClickEvent, new MouseButtonEventHandler (LogViewer_MouseDoubleClick), true);
      LogViewer.PreviewMouseLeftButtonUp += LogViewer_MouseUp;
      LogViewer.PreviewMouseRightButtonUp += LogViewer_MouseUp;
      LogViewer.MouseMove += LogViewer_MouseMove;
      LogViewer.SelectionChanged += LogViewer_SelectionChanged;

      LogViewer.DataContext = LogEntries = new ObservableCollection<LogEntry> ( );

      StringFormatData.StringFormat = "dd.MM.yyyy HH:mm:ss.fff";
    }

    #region Public properties

    public static readonly DependencyProperty ShowLineNumbersProperty = DependencyProperty.Register ("ShowLineNumbers", typeof (bool), typeof (LogViewerControl),
                              new PropertyMetadata (false, OnDataTemplateChange));

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
                              new PropertyMetadata (false, OnDataTemplateChange));

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

    public static readonly DependencyProperty LineNumberFontFamilyProperty = DependencyProperty.Register ("LineNumberFontFamily", typeof (FontFamily), typeof (LogViewerControl), 
                              new PropertyMetadata (new FontFamily ("Segoe UI")));

    [Category ("TextEditor")]
    public FontFamily LineNumberFontFamily
    {
      get
      {
        return ((FontFamily) GetValue (LineNumberFontFamilyProperty));
      }
      set
      {
        SetValue (LineNumberFontFamilyProperty, value);
      }
    }

    public static readonly DependencyProperty LineNumberFontSizeProperty = DependencyProperty.Register ("LineNumberFontSize", typeof (int), typeof (LogViewerControl), 
                              new PropertyMetadata (12));

    [Category ("TextEditor")]
    public int LineNumberFontSize
    {
      get
      {
        return ((int) GetValue (LineNumberFontSizeProperty));
      }
      set
      {
        SetValue (LineNumberFontSizeProperty, value);
      }
    }

    public static readonly DependencyProperty LineNumberFontWeightProperty = DependencyProperty.Register ("LineNumberFontWeight", typeof (FontWeight), typeof (LogViewerControl), 
                              new PropertyMetadata (FontWeights.Normal));

    [Category ("TextEditor")]
    public FontWeight LineNumberFontWeight
    {
      get
      {
        return ((FontWeight) GetValue (LineNumberFontWeightProperty));
      }
      set
      {
        SetValue (LineNumberFontWeightProperty, value);
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

    #endregion

    #region Public functions

    public void AppendText (string logText)
    {
      LogEntry entry = new LogEntry ( )
      {
        Index = ++index,
        Message = logText,
        DateTime = DateTime.Now
      };

      Dispatcher.BeginInvoke ((Action) (() => LogEntries.Add (entry)));

      if (AlwaysScrollIntoView)
        ScrollToEnd ( );
    }

    public void FindListViewItem (string searchText)
    {
      SearchText = searchText;
      FindListViewItem (LogViewer);
    }

    /// <summary>
    /// Clears all items
    /// </summary>
    public void Clear ( )
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

      LogViewer.ScrollIntoView (LogViewer.Items[LogEntries.Count - 1]);
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
    /// Get search count
    /// </summary>
    public int SearchCount
    {
      get;
      set;
    }

    /// <summary>
    /// Get count of search result
    /// </summary>
    /// <returns>Counts of find</returns>
    public int GetSearchCount ()
    {
      if (!string.IsNullOrEmpty (SearchText))
      {
        int count = 0;
        Regex regSearch = new Regex (string.Format ("({0})", SearchText), RegexOptions.IgnoreCase);

        foreach (LogEntry item in LogEntries)
        {
          string[] substrings = regSearch.Split (item.Message);

          foreach (var sub in substrings) 
          {
            if (regSearch.Match (sub).Success)
              count++;
          }
        }
        return (count);
      }
      return (-1);
    }

    #endregion

    #region PropertyCallback functions

    private static void OnDataTemplateChange (DependencyObject sender, DependencyPropertyChangedEventArgs e) 
    {
      if (sender.GetType ( ) == typeof (LogViewerControl))
      {
        LogViewerControl control = sender as LogViewerControl;

        if (control != null)
          control.SetTemplateState ( );
      }
    }

    #endregion

    #region Events

    private void LogViewer_Loaded (object sender, RoutedEventArgs e)
    {
      SetTemplateState ( );
    }

    private void LogViewer_SelectionChanged (object sender, SelectionChangedEventArgs e)
    {
      if (rightMouseButtonDown)
        return;
      if (!leftMouseButtonDown)
        return;

      var items = (sender as ListBox).SelectedItems;
      int index;
      Point mousePoint = PointToScreen (Mouse.GetPosition (this));

      if (oldMousePosition.Y != mousePoint.Y)
        mouseMove = true;

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
                  Cursor = new Cursor (info.Stream);
                  selectMouseItems = true;

                  // fullSelectionBox = true;
                  // LogViewer.ItemContainerStyle = (Style) FindResource ("FullSelectionBox");
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
#if DEBUG
        Console.WriteLine (string.Format ("{0}", ex));
#endif
      }

#if DEBUG
      LogMouseEvents ("--- SelectionChanged ---");
#endif
    }

    #endregion

    #region MouseEvents

    private void LogViewer_MouseMove (object sender, MouseEventArgs e)
    {
      if (!leftMouseButtonDown)
        return;

      if (leftMouseButtonDown && selectMouseItems)
      {
        Console.WriteLine (string.Format ("---------------- {0} --------------------", LogViewer.SelectedItems.Count));
      }
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
      selectMouseItems = false;
      // fullSelectionBox = false;

      Cursor = Cursors.Arrow;

#if DEBUG
      LogMouseEvents ("MouseUp");
#endif
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

    #region Helperfunctions

    private void FindListViewItem (DependencyObject obj)
    {
      for (int i = 0; i < VisualTreeHelper.GetChildrenCount (obj); i++)
      {
        ListBoxItem lbi = obj as ListBoxItem;

        if (lbi != null)
          HightlightText (lbi);

        FindListViewItem (VisualTreeHelper.GetChild (obj, i));
      }
    }

    private void HightlightText (Object itx)
    {
      if (itx != null)
      {
        Regex regex;

        if (itx is TextBlock)
        {
          regex = new Regex (string.Format ("({0})", SearchText), RegexOptions.IgnoreCase);
          TextBlock tb = itx as TextBlock;

          if (SearchText.Length == 0)
          {
            string str = tb.Text;
            tb.Inlines.Clear ( );
            tb.Inlines.Add (str);
            return;
          }

          string[] substrings = regex.Split (tb.Text);
          tb.Inlines.Clear ( );

          foreach (var item in substrings)
          {
            if (regex.Match (item).Success)
            {
              Brush searchHighlightOpacity = TextEditorSearchHighlightBackground;
              searchHighlightOpacity.Opacity = 0.4;

              Run runx = new Run (item) 
              { 
                Background = searchHighlightOpacity,
                Foreground = TextEditorSearchHighlightForeground
              };
              tb.Inlines.Add (runx);
            }
            else
              tb.Inlines.Add (item);
          }
          return;
        }
        else
        {
          for (int i = 0; i < VisualTreeHelper.GetChildrenCount (itx as DependencyObject); i++)
            HightlightText (VisualTreeHelper.GetChild (itx as DependencyObject, i));
        }
      }
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

    private Size GetSizeFromText (TextBlock target)
    {
      FormattedText txtSize = new FormattedText (target.Text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, 
        new Typeface (target.FontFamily, target.FontStyle, target.FontWeight, target.FontStretch), target.FontSize, Brushes.Black);

      return (new Size (txtSize.Width, txtSize.Height));
    }

    private childItem FindVisualChild<childItem> (DependencyObject obj)
        where childItem: DependencyObject
    {
      if (obj != null)
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount (obj); i++)
        {
          DependencyObject child = VisualTreeHelper.GetChild (obj, i);

          if (child != null && child is childItem)
            return ((childItem) child);
          else
          {
            childItem childOfChild = FindVisualChild<childItem> (child);

            if (childOfChild != null)
              return (childOfChild);
          }
        }
      return (null);
    }

#if DEBUG

    private void LogMouseEvents (string mouseEvent)
    {
      DateTime now = DateTime.Now;

      Console.WriteLine (string.Format ("\n{0} {1}", now.ToString ("hh:mm:ss.fff"), mouseEvent));
      Console.WriteLine (string.Format ("LeftMouseButtonDown {0} / RightMouseButtonDown {1}", leftMouseButtonDown, rightMouseButtonDown));
      Console.WriteLine (string.Format ("OldMousePosition X {0} / OldMousePosition Y {1}", oldMousePosition.X, oldMousePosition.Y));
      Console.WriteLine (string.Format ("MouseMove {0} / WordSelection {1}", mouseMove, wordSelection));
    }

#endif

    #endregion
  }
}
