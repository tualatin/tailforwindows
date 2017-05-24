using System;
using System.Windows.Data;


namespace Org.Vs.TailForWin.Converters
{
  /// <summary>
  /// InverseBooleanConverter
  /// </summary>
  public class InverseBooleanConverter : IValueConverter
  {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>Converted object</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      //if(targetType != typeof(bool))
      //  throw new InvalidOperationException("The target must be a boolean");

      return value != null && !(bool) value;
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value to convert back</param>
    /// <param name="targetType">TargtType</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>Converted object</returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return value != null && !(bool) value;
    }
  }
}
