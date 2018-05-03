using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows;
using Org.Vs.TailForWin.Business.Data;
using Org.Vs.TailForWin.Core.Data.Settings;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule
{
  /// <summary>
  /// Interaction logic for SplitWindowControl.xaml
  /// </summary>
  public partial class SplitWindowControl
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public SplitWindowControl() => InitializeComponent();


    #region Public properties

    /// <summary>
    /// LogEntries property
    /// </summary>
    public static readonly DependencyProperty LogEntriesProperty = DependencyProperty.Register("LogEntries", typeof(ObservableCollection<LogEntry>), typeof(SplitWindowControl),
      new PropertyMetadata(new ObservableCollection<LogEntry>()));

    /// <summary>
    /// LogEntries
    /// </summary>
    public ObservableCollection<LogEntry> LogEntries
    {
      get => (ObservableCollection<LogEntry>) GetValue(LogEntriesProperty);
      set => SetValue(LogEntriesProperty, value);
    }

    /// <summary>
    /// Word wrap property
    /// </summary>
    public static readonly DependencyProperty WordWrappingProperty = DependencyProperty.Register("WordWrapping", typeof(bool), typeof(SplitWindowControl),
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
    /// Text editor font style property
    /// </summary>
    public static readonly DependencyProperty TextEditorFontStyleProperty = DependencyProperty.Register("TextEditorFontStyle", typeof(System.Drawing.FontStyle), typeof(SplitWindowControl),
      new PropertyMetadata(System.Drawing.FontStyle.Regular));

    /// <summary>
    /// Text editor font style
    /// </summary>
    public System.Drawing.FontStyle TextEditorFontStyle
    {
      get => (System.Drawing.FontStyle) GetValue(TextEditorFontStyleProperty);
      set => SetValue(TextEditorFontStyleProperty, value);
    }

    /// <summary>
    /// Text editor font family property
    /// </summary>
    public static readonly DependencyProperty TextEditorFontFamilyProperty = DependencyProperty.Register("TextEditorFontFamily", typeof(FontFamily), typeof(SplitWindowControl),
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
    public static readonly DependencyProperty TextEditorFontWeightProperty = DependencyProperty.Register("TextEditorFontWeight", typeof(FontWeight), typeof(SplitWindowControl),
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
    public static readonly DependencyProperty TextEditorFontSizeProperty = DependencyProperty.Register("TextEditorFontSize", typeof(int), typeof(SplitWindowControl),
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
    public static readonly DependencyProperty TextEditorSelectionColorHexProperty = DependencyProperty.Register("TextEditorSelectionColorHex", typeof(string), typeof(SplitWindowControl),
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
    public static readonly DependencyProperty TextEditorSearchHighlightBackgroundHexProperty = DependencyProperty.Register("TextEditorSearchHighlightBackgroundHex", typeof(string),
      typeof(SplitWindowControl), new PropertyMetadata(DefaultEnvironmentSettings.SearchHighlightBackgroundColor));

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
      typeof(SplitWindowControl), new PropertyMetadata(DefaultEnvironmentSettings.SearchHighlightForegroundColor));

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
    public static readonly DependencyProperty AddDateTimeProperty = DependencyProperty.Register("AddDateTime", typeof(bool), typeof(SplitWindowControl),
      new PropertyMetadata(true));

    /// <summary>
    /// AddDateTime
    /// </summary>
    public bool AddDateTime
    {
      get => (bool) GetValue(AddDateTimeProperty);
      set => SetValue(AddDateTimeProperty, value);
    }

    #endregion
  }
}
