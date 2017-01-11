using System;
using System.Windows.Data;


namespace Org.Vs.TailForWin.Converters
{
  /// <summary>
  /// Text trim converter
  /// </summary>
  [ValueConversion(typeof(string), typeof(string))]
  public class TextTrimConverter : IValueConverter
  {
    #region IValueConverter Members

    /// <summary>
    /// Trim a text string
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Trim text</returns>
    public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
        return (null);

      if (string.IsNullOrEmpty(value as string))
        return (string.Empty);

      string s = (value as string).Trim();

      return (s);
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Object</returns>
    public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return (value);
    }

    #endregion
  }
}
