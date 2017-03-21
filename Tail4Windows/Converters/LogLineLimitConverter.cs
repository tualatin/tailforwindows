using System;
using System.Globalization;
using System.Windows.Data;


namespace Org.Vs.TailForWin.Converters
{
  /// <summary>
  /// Log line limit converter
  /// </summary>
  public class LogLineLimitConverter : IValueConverter
  {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Parameters</param>
    /// <param name="culture">Culture</param>
    /// <returns>True or False</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if(value is int)
      {
        if((int) value > -1)
          return ((int) value);
      }
      return (400000);
    }

    /// <summary>
    /// Convert to object
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Parameters</param>
    /// <param name="culture">Culture</param>
    /// <returns>Object</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if(value is double)
      {
        if((double) value < 400000)
          return (value);
      }
      return (-1);
    }
  }
}
