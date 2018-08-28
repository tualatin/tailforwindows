using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Core.Extensions;


namespace Org.Vs.TailForWin.UI.Converters
{
  /// <summary>
  /// Hex string to <see cref="Brush"/> converter
  /// </summary>
  [ValueConversion(typeof(string), typeof(Brush))]
  public class StringToWindowMediaBrushConverter : IValueConverter
  {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ( !(value is string hexString) )
        return Brushes.Transparent;

      var convertFromString = ColorConverter.ConvertFromString(hexString);
      return convertFromString == null ? Brushes.Transparent : new SolidColorBrush((Color) convertFromString);
    }

    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ( !(value is Brush brush) )
        return "#FFFFFF";

      return EnvironmentContainer.ConvertMediaBrushToDrawingColor(brush).ToHexString();
    }
  }
}
