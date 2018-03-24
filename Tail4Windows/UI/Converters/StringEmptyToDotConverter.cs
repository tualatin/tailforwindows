using System;
using System.Globalization;
using System.Windows.Data;


namespace Org.Vs.TailForWin.UI.Converters
{
  /// <summary>
  /// Strint empty to dot converter
  /// </summary>
  public class StringEmptyToDotConverter : IValueConverter
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
      if ( !(value is string text) )
        return string.Empty;

      return string.IsNullOrWhiteSpace(text) ? "..." : text;
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
