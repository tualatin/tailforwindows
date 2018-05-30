using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using log4net;


namespace Org.Vs.TailForWin.UI.UserControls
{
  /// <summary>
  /// Highlight <see cref="TextBlock"/>
  /// </summary>
  public class HighlightTextBlock : TextBlock
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(HighlightTextBlock));

    private new string Text
    {
      set
      {
        var words = (List<string>) HighlightText;

        if ( words == null || words.Count == 0 )
        {
          base.Text = value;
          return;
        }

        Inlines.Clear();
        var regex = new Regex($"({string.Join("|", words)})");
        var splits = regex.Split(value);

        foreach ( var item in splits )
        {
          if ( regex.Match(item).Success )
          {
            var run = new Run(item)
            {
              Foreground = HighlightForeground
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
    public static readonly DependencyProperty HighlightTextroperty = DependencyProperty.Register(nameof(HighlightText), typeof(object), typeof(HighlightTextBlock),
      new PropertyMetadata(null, HighlightTextChanged));

    /// <summary>
    /// Highlight text
    /// </summary>
    public object HighlightText
    {
      get => GetValue(HighlightTextroperty);
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
    public HighlightTextBlock()
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="inline"><see cref="Inline"/></param>
    public HighlightTextBlock(Inline inline)
    : base(inline)
    {
    }

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
