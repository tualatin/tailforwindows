using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using log4net;
using Org.Vs.TailForWin.Business.Data;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Settings;
using Org.Vs.TailForWin.UI.Extensions;
using Org.Vs.TailForWin.UI.Utils;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule.LogWindowUserControl
{
  /// <summary>
  /// LogWindow control
  /// </summary>
  public class LogWindowListBox : ListBox
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(LogWindowListBox));

    private ScrollViewer _scrollViewer;
    private Grid _splitGripControl;
    private bool _mouseDown;

    #region Public properties

    /// <summary>
    /// ShowGridSplitControl property
    /// </summary>
    public static readonly DependencyProperty ShowGridSplitControlProperty = DependencyProperty.Register("ShowGridSplitControl", typeof(bool), typeof(LogWindowListBox),
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
    /// Word wrap property
    /// </summary>
    public static readonly DependencyProperty WordWrappingProperty = DependencyProperty.Register("WordWrapping", typeof(bool), typeof(LogWindowListBox),
      new PropertyMetadata(false));

    /// <summary>
    /// Text editor word wrapping
    /// </summary>
    public bool WordWrapping
    {
      get => (bool) GetValue(WordWrappingProperty);
      set => SetValue(WordWrappingProperty, value);
    }

    /// <summary>
    /// Vertical scroll bar visibility property
    /// </summary>
    public static readonly DependencyProperty VerticalScrollbarVisibleProperty = DependencyProperty.Register("VerticalScrollbarVisible", typeof(ScrollBarVisibility), typeof(LogWindowListBox),
      new PropertyMetadata(ScrollBarVisibility.Auto));

    /// <summary>
    /// TextEditor vertical scroll visible
    /// </summary>
    public ScrollBarVisibility VerticalScrollbarVisible
    {
      get => (ScrollBarVisibility) GetValue(VerticalScrollbarVisibleProperty);
      set => SetValue(VerticalScrollbarVisibleProperty, value);
    }

    /// <summary>
    /// Text editor font style property
    /// </summary>
    public static readonly DependencyProperty TextEditorFontStyleProperty = DependencyProperty.Register("TextEditorFontStyle", typeof(FontStyle), typeof(LogWindowListBox),
      new PropertyMetadata(FontStyles.Normal));

    /// <summary>
    /// Text editor font style
    /// </summary>
    public FontStyle TextEditorFontStyle
    {
      get => (FontStyle) GetValue(TextEditorFontStyleProperty);
      set => SetValue(TextEditorFontStyleProperty, value);
    }

    /// <summary>
    /// Text editor font family property
    /// </summary>
    public static readonly DependencyProperty TextEditorFontFamilyProperty = DependencyProperty.Register("TextEditorFontFamily", typeof(FontFamily), typeof(LogWindowListBox),
      new PropertyMetadata(new FontFamily("Segoe UI")));

    /// <summary>
    /// Text editor font family
    /// </summary>
    public FontFamily TextEditorFontFamily
    {
      get => (FontFamily) GetValue(TextEditorFontFamilyProperty);
      set => SetValue(TextEditorFontFamilyProperty, value);
    }

    /// <summary>
    /// Text editor font weight property
    /// </summary>
    public static readonly DependencyProperty TextEditorFontWeightProperty = DependencyProperty.Register("TextEditorFontWeight", typeof(FontWeight), typeof(LogWindowListBox),
      new PropertyMetadata(FontWeights.Normal));

    /// <summary>
    /// Text editor font weight
    /// </summary>
    public FontWeight TextEditorFontWeight
    {
      get => (FontWeight) GetValue(TextEditorFontWeightProperty);
      set => SetValue(TextEditorFontWeightProperty, value);
    }

    /// <summary>
    /// Text editor font size property
    /// </summary>
    public static readonly DependencyProperty TextEditorFontSizeProperty = DependencyProperty.Register("TextEditorFontSize", typeof(int), typeof(LogWindowListBox),
      new PropertyMetadata(12));

    /// <summary>
    /// Text editor font size
    /// </summary>
    public int TextEditorFontSize
    {
      get => (int) GetValue(TextEditorFontSizeProperty);
      set => SetValue(TextEditorFontSizeProperty, value);
    }

    /// <summary>
    /// Text editor selection color porperty
    /// </summary>
    public static readonly DependencyProperty TextEditorSelectionColorHexProperty = DependencyProperty.Register("TextEditorSelectionColorHex", typeof(string), typeof(LogWindowListBox),
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
    /// Always scroll into view property
    /// </summary>
    public static readonly DependencyProperty AlwaysScrollIntoViewProperty = DependencyProperty.Register("AlwaysScrollIntoView", typeof(bool), typeof(LogWindowListBox),
      new PropertyMetadata(true));

    /// <summary>
    /// Text editor scroll into view
    /// </summary>
    public bool AlwaysScrollIntoView
    {
      get => (bool) GetValue(AlwaysScrollIntoViewProperty);
      set => SetValue(AlwaysScrollIntoViewProperty, value);
    }

    /// <summary>
    /// Text editor search highlight background property
    /// </summary>
    public static readonly DependencyProperty TextEditorSearchHighlightBackgroundHexProperty = DependencyProperty.Register("TextEditorSearchHighlightBackgroundHex", typeof(string),
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
    public static readonly DependencyProperty TextEditorSearchHighlightForegroundHexProperty = DependencyProperty.Register("TextEditorSearchHighlightForegroundHex", typeof(string),
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
    /// AddDateTime property
    /// </summary>
    public static readonly DependencyProperty AddDateTimeProperty = DependencyProperty.Register("AddDateTime", typeof(bool), typeof(LogWindowListBox),
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
    [Category("TextEditor")]
    public bool FilterOn
    {
      get => (bool) GetValue(FilterOnProperty);
      set => SetValue(FilterOnProperty, value);
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

    private bool _bookmarLine;

    /// <summary>
    /// Bookmark lines
    /// </summary>
    public bool BookmarkLine
    {
      get => _bookmarLine;
      set
      {
        _bookmarLine = value;

        //if ( _bookmarLine )
        //  FindWhatTextChanged();
      }
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
    /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      //Loaded += OnLoaded;
      _scrollViewer = UiHelpers.GetChildOfType<ScrollViewer>(this);

      if ( _scrollViewer == null )
        return;

      _scrollViewer.ScrollChanged += OnScrollChanged;
    }

    private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
    {
      if ( _splitGripControl == null && ShowGridSplitControl && SettingsHelperController.CurrentSettings.SplitterWindowBehavior )
        LoadSplitGripControl();
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

      LOG.Trace($"Current mouse position {mouse.Y}");
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

    private void RefreshCollectionViewSource()
    {
      if ( ItemsSource != null )
        CollectionViewSource.GetDefaultView(ItemsSource).Refresh();
    }
  }
}
