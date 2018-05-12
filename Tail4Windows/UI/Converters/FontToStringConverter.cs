using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using Org.Vs.TailForWin.Core.Data;


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
      if ( value is FontType font )
        return $"{font.FontFamily.Source} ({font.FontSize:0.#})";

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
