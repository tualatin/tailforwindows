using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interactivity;
using System.Windows.Media;


namespace Org.Vs.TailForWin.UI.UserControls.Behaviors
{
  /// <summary>
  /// WatermarkBehavior for <see cref="ComboBox"/>
  /// </summary>
  public class WatermarkBehavior : Behavior<ComboBox>
  {
    private WaterMarkAdorner _adorner;

    /// <summary>
    /// Watermark text
    /// </summary>
    public string Text
    {
      get => (string) GetValue(TextProperty);
      set => SetValue(TextProperty, value);
    }

    /// <summary>
    ///  Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(WatermarkBehavior), new PropertyMetadata("Watermark"));

    /// <summary>
    /// FontSize
    /// </summary>
    public double FontSize
    {
      get => (double) GetValue(FontSizeProperty);
      set => SetValue(FontSizeProperty, value);
    }

    /// <summary>
    /// Using a DependencyProperty as the backing store for FontSize.  This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(WatermarkBehavior), new PropertyMetadata(12.0));

    /// <summary>
    /// Foreground
    /// </summary>
    public Brush Foreground
    {
      get => (Brush) GetValue(ForegroundProperty);
      set => SetValue(ForegroundProperty, value);
    }

    /// <summary>
    /// Using a DependencyProperty as the backing store for Foreground.  This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register("Foreground", typeof(Brush), typeof(WatermarkBehavior), new PropertyMetadata(Brushes.Black));

    /// <summary>
    /// FontFamily
    /// </summary>
    public string FontFamily
    {
      get => (string) GetValue(FontFamilyProperty);
      set => SetValue(FontFamilyProperty, value);
    }

    /// <summary>
    /// Using a DependencyProperty as the backing store for FontFamily.  This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register("FontFamily", typeof(string), typeof(WatermarkBehavior), new PropertyMetadata("Segoe UI"));

    /// <summary>
    /// Called after the behavior is attached to an AssociatedObject.
    /// </summary>
    protected override void OnAttached()
    {
      _adorner = new WaterMarkAdorner(AssociatedObject, Text, FontSize, FontFamily, Foreground);

      AssociatedObject.Loaded += OnLoaded;
      AssociatedObject.GotFocus += OnFocus;
      AssociatedObject.LostFocus += OnLostFocus;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      if ( AssociatedObject.IsFocused )
        return;

      if ( !string.IsNullOrEmpty(AssociatedObject.Text) )
        return;

      try
      {
        var layer = AdornerLayer.GetAdornerLayer(AssociatedObject);

        if ( layer == null )
          return;

        layer.Remove(_adorner);
        layer.Add(_adorner);
      }
      catch
      {
        // Nothing
      }

    }

    private void OnLostFocus(object sender, RoutedEventArgs e)
    {
      if ( !string.IsNullOrEmpty(AssociatedObject.Text) )
        return;

      try
      {
        var layer = AdornerLayer.GetAdornerLayer(AssociatedObject);

        if ( layer == null )
          return;

        layer.Remove(_adorner);
        layer.Add(_adorner);
      }
      catch
      {
        // Nothing
      }
    }

    private void OnFocus(object sender, RoutedEventArgs e)
    {
      try
      {
        var layer = AdornerLayer.GetAdornerLayer(AssociatedObject);

        if ( layer == null )
          return;

        layer.Remove(_adorner);
      }
      catch
      {
        // Nothing
      }
    }

    /// <summary>
    /// WaterMarkAdorner
    /// </summary>
    private class WaterMarkAdorner : Adorner
    {
      private readonly string _text;
      private readonly double _fontSize;
      private readonly string _fontFamily;
      private readonly Brush _foreground;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="element"><see cref="UIElement"/></param>
      /// <param name="text">Text</param>
      /// <param name="fontsize">Fontsize</param>
      /// <param name="font">Font</param>
      /// <param name="foreground">Foreground <see cref="Brush"/></param>
      public WaterMarkAdorner(UIElement element, string text, double fontsize, string font, Brush foreground)
          : base(element)
      {
        IsHitTestVisible = false;
        Opacity = 1;
        _text = text;
        _fontSize = fontsize;
        _fontFamily = font;
        _foreground = foreground;
      }

      /// <summary>
      /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. 
      /// The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
      /// </summary>
      /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
      protected override void OnRender(DrawingContext drawingContext)
      {
        base.OnRender(drawingContext);

        var text = new FormattedText(
                _text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(new FontFamily(_fontFamily), FontStyles.Italic, FontWeights.Normal, FontStretches.Normal),
                _fontSize,
                _foreground);

        drawingContext.DrawText(text, new Point(8, 3));
      }
    }
  }
}
