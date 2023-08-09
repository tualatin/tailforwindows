using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Core.Extensions;


namespace Org.Vs.TailForWin.Ui.Utils.Converters
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
    // ReSharper disable once PossibleNullReferenceException
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

      var darkerColor = DarkerColor(EnvironmentContainer.ConvertMediaBrushToDrawingColor(brush), 75F);

      return new SolidColorBrush(Color.FromRgb(darkerColor.R, darkerColor.G, darkerColor.B));
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>Back converted value</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    /// <summary>
    /// Lighten an color
    /// </summary>
    /// <param name="color"></param>
    /// <param name="correctionFactory"></param>
    /// <returns></returns>
    public static System.Drawing.Color LighterColor(System.Drawing.Color color, float correctionFactory = 50f)
    {
      correctionFactory /= 100f;
      const float rgb255 = 255f;

      return System.Drawing.Color.FromArgb(
        (int) (color.R + (rgb255 - color.R) * correctionFactory),
        (int) (color.G + (rgb255 - color.G) * correctionFactory),
        (int) (color.B + (rgb255 - color.B) * correctionFactory));
    }

    /// <summary>
    /// Darken an color
    /// </summary>
    /// <param name="color"></param>
    /// <param name="correctionFactory"></param>
    /// <returns></returns>
    public static System.Drawing.Color DarkerColor(System.Drawing.Color color, float correctionFactory = 50f)
    {
      const float hundredPercent = 100f;

      return System.Drawing.Color.FromArgb((
        int) (color.R / hundredPercent * correctionFactory),
        (int) (color.G / hundredPercent * correctionFactory),
        (int) (color.B / hundredPercent * correctionFactory));
    }
  }
}
