using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;


namespace Org.Vs.TailForWin.UI.Converters
{
  /// <summary>
  /// Color to solid color brush converter
  /// </summary>
  [ValueConversion(typeof(Color), typeof(SolidColorBrush))]
  public class ColorToSolidColorBrushConverter : IValueConverter
  {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns></returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ( !(value is Color color) )
        return null;

      return new SolidColorBrush(color);
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value to convert back</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns></returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
