using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Org.Vs.TailForWin.Business.Data;
using Org.Vs.TailForWin.Core.Data.Settings;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.LogWindowUserControl.Data;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule.LogWindowUserControl
{
  /// <summary>
  /// LogWindow control
  /// </summary>
  public class LogWindowListBox : ListBox
  {
    #region Public properties

    /// <summary>
    /// Show line number property
    /// </summary>
    public static readonly DependencyProperty ShowLineNumbersProperty = DependencyProperty.Register("ShowLineNumbers", typeof(bool), typeof(LogWindowListBox),
      new PropertyMetadata(false, OnDataTemplateChanged));

    /// <summary>
    /// Text editor show line numbers
    /// </summary>
    public bool ShowLineNumbers
    {
      get => (bool) GetValue(ShowLineNumbersProperty);
      set => SetValue(ShowLineNumbersProperty, value);
    }

    /// <summary>
    /// Line number color property
    /// </summary>
    public static readonly DependencyProperty LineNumberColorHexProperty = DependencyProperty.Register("LineNumberHexColor", typeof(string), typeof(LogWindowListBox),
      new PropertyMetadata(DefaultEnvironmentSettings.LineNumberColor));

    /// <summary>
    /// Text editor line number color
    /// </summary>
    public string LineNumberColorHex
    {
      get => (string) GetValue(LineNumberColorHexProperty);
      set => SetValue(LineNumberColorHexProperty, value);
    }

    /// <summary>
    /// Text editor line number background color
    /// </summary>
    public static readonly DependencyProperty LineNumbersBackgroundColorHexProperty = DependencyProperty.Register("LineNumberBackgroundColorHex", typeof(string), typeof(LogWindowListBox),
      new PropertyMetadata(DefaultEnvironmentSettings.LineNumberBackgroundColor));

    /// <summary>
    /// Text editor line number background color
    /// </summary>
    public string LineNumberBackgroundColorHex
    {
      get => (string) GetValue(LineNumbersBackgroundColorHexProperty);
      set => SetValue(LineNumbersBackgroundColorHexProperty, value);
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
    /// Text editor background color property
    /// </summary>
    public static readonly DependencyProperty TextEditorBackgroundColorHexProperty = DependencyProperty.Register("TextEditorBackgroundColorHex", typeof(string), typeof(LogWindowListBox),
      new PropertyMetadata(DefaultEnvironmentSettings.BackgroundColor));

    /// <summary>
    /// Text editor background color
    /// </summary>
    public string TextEditorBackgroundColorHex
    {
      get => (string) GetValue(TextEditorBackgroundColorHexProperty);
      set => SetValue(TextEditorBackgroundColorHexProperty, value);
    }

    /// <summary>
    /// Text editor forground color property
    /// </summary>
    public static readonly DependencyProperty TextEditorForegroundColorHexProperty = DependencyProperty.Register("TextEditorForegroundColorHex", typeof(string), typeof(LogWindowListBox),
      new PropertyMetadata(DefaultEnvironmentSettings.ForegroundColor));

    /// <summary>
    /// Text editor foreground color
    /// </summary>
    public string TextEditorForegroundColorHex
    {
      get => (string) GetValue(TextEditorForegroundColorHexProperty);
      set => SetValue(TextEditorForegroundColorHexProperty, value);
    }

    /// <summary>
    /// Show date time property
    /// </summary>
    public static readonly DependencyProperty ShowDateTimeProperty = DependencyProperty.Register("ShowDateTime", typeof(bool), typeof(LogWindowListBox),
      new PropertyMetadata(false, OnDataTemplateChanged));

    /// <summary>
    /// TextEditor show datetime
    /// </summary>
    public bool ShowDateTime
    {
      get => (bool) GetValue(ShowDateTimeProperty);
      set => SetValue(ShowDateTimeProperty, value);
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
    /// All log entries
    /// </summary>
    public ObservableCollection<LogEntry> LogEntries
    {
      get;
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
    public static readonly DependencyProperty FilterOnProperty = DependencyProperty.Register("FilterOn", typeof(bool), typeof(LogWindowListBox),
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
    /// Is a valid FileName entered
    /// </summary>
    public bool FileNameAvailable
    {
      get;
      set;
    }

    #endregion

    #region PropertyCallback functions

    private static void OnDataTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      if ( sender.GetType() != typeof(LogWindowListBox) )
        return;

      if ( !(sender is LogWindowListBox control) )
        return;

      control.SetTemplateState();
      control.RefreshCollectionViewSource();
    }

    private static void OnFilterOnChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if ( sender is LogWindowListBox control )
        control.RefreshCollectionViewSource();
    }

    #endregion

    private void SetTemplateState()
    {
      if ( ShowDateTime && ShowLineNumbers )
      {
        TemplateData.State = TemplateData.TemplateStates.ShowDateTimeLineNumber;
        AddDateTime = true;
      }
      else if ( ShowLineNumbers )
      {
        TemplateData.State = TemplateData.TemplateStates.ShowLineNumber;
        AddDateTime = false;
      }
      else if ( ShowDateTime )
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

    private void RefreshCollectionViewSource()
    {
      if ( ItemsSource != null )
        CollectionViewSource.GetDefaultView(ItemsSource).Refresh();
    }
  }
}
