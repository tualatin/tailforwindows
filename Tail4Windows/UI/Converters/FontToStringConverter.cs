using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;

namespace Org.Vs.TailForWin.UI.Converters
{
  /// <summary>
  /// Font to string converter
  /// </summary>
  [ValueConversion(typeof(Font), typeof(string))]
  public class FontToStringConverter : IValueConverter
  {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ( value is Font font )
        return $"{font.Name} ({font.Size}) {(font.Italic ? "Italic" : string.Empty)} {(font.Bold ? "Bold" : string.Empty)}";

      return string.Empty;
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value to convert back</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
  }
}
