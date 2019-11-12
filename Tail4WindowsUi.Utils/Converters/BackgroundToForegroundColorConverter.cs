using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;


namespace Org.Vs.TailForWin.Ui.Utils.Converters
{
  /// <summary>
  /// Background to foreground color converter
  /// </summary>
  [ValueConversion(typeof(Brush), typeof(Color))]
  public class BackgroundToForegroundColorConverter : IValueConverter
  {
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
      if ( !(value is SolidColorBrush backgroundColor) )
        return Colors.Black;

      double l = 0.2126 * backgroundColor.Color.ScR + 0.7152 * backgroundColor.Color.ScG + 0.0722 * backgroundColor.Color.ScB;
      return l > 0.5 ? Colors.Black : Color.FromRgb(240, 240, 240);
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
  }
}
