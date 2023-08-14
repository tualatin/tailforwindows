using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Org.Vs.TailForWin.Core.Data;


namespace Org.Vs.TailForWin.Ui.Utils.Converters
{
  /// <summary>
  /// <see cref="FontType"/> to string converter
  /// </summary>
  [ValueConversion(typeof(FontType), typeof(string))]
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
      if ( !(value is FontType font) )
        return string.Empty;

      var ftf = new FamilyTypeface
      {
        Stretch = font.FontStretch,
        Weight = font.FontWeight,
        Style = font.FontStyle
      };
      return $"{font.FontFamily.Source} ({font.FontSize:0.#})";
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
