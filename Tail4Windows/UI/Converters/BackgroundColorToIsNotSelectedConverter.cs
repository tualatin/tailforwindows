using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Org.Vs.TailForWin.Core.Extensions;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.UI.Converters
{
  /// <summary>
  /// Background color to is not selected converter
  /// </summary>
  [ValueConversion(typeof(Brush), typeof(Brush))]
  public class BackgroundColorToIsNotSelectedConverter : IValueConverter
  {
    private readonly SolidColorBrush _defaultIsNotSelectedColor;

    /// <summary>
    /// Standard constructor
    /// </summary>
    public BackgroundColorToIsNotSelectedConverter() => _defaultIsNotSelectedColor = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF8DA3C1"));

    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>Foreground color</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ( !(value is SolidColorBrush brush) )
        return _defaultIsNotSelectedColor;

      if ( Equals(EnvironmentContainer.ConvertMediaBrushToDrawingColor(brush).ToHexString(), "#D6DBE9") )
        return _defaultIsNotSelectedColor;

      var lighterColor = DarkerColor(EnvironmentContainer.ConvertMediaBrushToDrawingColor(brush));

      return new SolidColorBrush(Color.FromRgb(lighterColor.R, lighterColor.G, lighterColor.B));
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">TargtType</param>
    /// <param name="parameter">Paramter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>Back converted value</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    private System.Drawing.Color LighterColor(System.Drawing.Color color, float correctionfactory = 50f)
    {
      correctionfactory = correctionfactory / 100f;
      const float rgb255 = 255f;

      return System.Drawing.Color.FromArgb((int) (color.R + (rgb255 - color.R) * correctionfactory), (int) (color.G + (rgb255 - color.G) * correctionfactory), (int) (color.B + (rgb255 - color.B) * correctionfactory));
    }

    private System.Drawing.Color DarkerColor(System.Drawing.Color color, float correctionfactory = 50f)
    {
      const float hundredpercent = 100f;

      return System.Drawing.Color.FromArgb((int) (color.R / hundredpercent * correctionfactory), (int) (color.G / hundredpercent * correctionfactory), (int) (color.B / hundredpercent * correctionfactory));
    }
  }
}
