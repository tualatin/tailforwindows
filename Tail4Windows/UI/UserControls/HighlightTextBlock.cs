using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Data;
using Org.Vs.TailForWin.UI.Converters;


namespace Org.Vs.TailForWin.UI.UserControls
{
  /// <summary>
  /// Highlight <see cref="TextBlock"/>
  /// </summary>
  public class HighlightTextBlock : TextBlock
  {
    private readonly StringToWindowMediaBrushConverter _stringToBrushConverter;

    private new string Text
    {
      set
      {
        if ( HighlightText == null || HighlightText.Count == 0 )
        {
          base.Text = value;
          return;
        }

        string words = string.Join("|", HighlightText.Select(p => p.Text).ToList());
        var regex = new Regex($@"(?i)(\b{words}\b)");
        var splits = regex.Split(value);

        Inlines.Clear();

        foreach ( var item in splits )
        {
          string hexColor = HighlightText.FirstOrDefault(p => string.Compare(p.Text, item, StringComparison.CurrentCultureIgnoreCase) == 0)?.TextHighlightColorHex;

          if ( regex.Match(item).Success )
          {
            var run = new Run(item)
            {
              Foreground = _stringToBrushConverter.Convert(hexColor, typeof(Brush), null, null) as Brush
            };
            Inlines.Add(run);
          }
          else
          {
            Inlines.Add(item);
          }
        }
      }
    }

    #region Dependency properties

    /// <summary>
    /// Highlight foreground property
    /// </summary>
    public static readonly DependencyProperty HighlightForegroundProperty = DependencyProperty.Register(nameof(HighlightForeground), typeof(Brush), typeof(HighlightTextBlock));

    /// <summary>
    /// Highlight foreground
    /// </summary>
    public Brush HighlightForeground
    {
      get => (Brush) GetValue(HighlightForegroundProperty);
      set => SetValue(HighlightForegroundProperty, value);
    }

    /// <summary>
    /// <see cref="List{T}"/> of <see cref="string"/> to be highlighted
    /// </summary>
    public static readonly DependencyProperty HighlightTextroperty = DependencyProperty.Register(nameof(HighlightText), typeof(List<TextHighlightData>), typeof(HighlightTextBlock),
      new PropertyMetadata(null, HighlightTextChanged));

    /// <summary>
    /// Highlight text
    /// </summary>
    public List<TextHighlightData> HighlightText
    {
      get => (List<TextHighlightData>) GetValue(HighlightTextroperty);
      set => SetValue(HighlightTextroperty, value);
    }

    /// <summary>
    /// Using a DependencyProperty as the backing store for HighlightableText.  This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty HighlightableTextProperty = DependencyProperty.Register(nameof(HighlightableText), typeof(string), typeof(HighlightTextBlock),
      new PropertyMetadata(HighlightableTextChanged));

    /// <summary>
    /// Highlightablbe text
    /// </summary>
    public string HighlightableText
    {
      get => (string) GetValue(HighlightableTextProperty);
      set => SetValue(HighlightableTextProperty, value);
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public HighlightTextBlock() => _stringToBrushConverter = new StringToWindowMediaBrushConverter();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="inline"><see cref="Inline"/></param>
    public HighlightTextBlock(Inline inline)
    : base(inline) => _stringToBrushConverter = new StringToWindowMediaBrushConverter();

    #region Property callbacks

    private static void HighlightableTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs inArgs)
    {
      if ( sender is HighlightTextBlock tb )
        tb.Text = tb.HighlightableText;
    }

    private static void HighlightTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      if ( !(sender is HighlightTextBlock tb) )
        return;

      tb.UpdateText();
    }

    #endregion

    private void UpdateText() => Text = base.Text;
  }
}
