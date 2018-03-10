using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;


namespace Org.Vs.TailForWin.UI.Converters
{
  /// <summary>
  /// String to Windows Media Color converter
  /// </summary>
  public class StringToWindowsMediaColorConverter : IValueConverter
  {
    /// <summary>
    /// Converts
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>Converted value</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ( !(value is string hexString) )
        return Colors.White;

      var convertFromString = ColorConverter.ConvertFromString(hexString);
      return (Color?) convertFromString ?? Colors.Transparent;
    }

    /// <summary>
    /// Converts back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>Converted value</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ( !(value is Color color) )
        return "#FFFFFF";

      return Color.FromArgb(color.A, color.R, color.G, color.B);
    }
  }
}
