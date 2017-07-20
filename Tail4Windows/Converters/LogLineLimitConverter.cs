using System;
using System.Globalization;
using System.Windows.Data;
using Org.Vs.TailForWin.Data;


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
      if ( !(value is int iValue) )
        return CentralManager.UNLIMITED_LOG_LINE_VALUE;

      return iValue > -1 ? iValue : CentralManager.UNLIMITED_LOG_LINE_VALUE;
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
      if ( value as double? < CentralManager.UNLIMITED_LOG_LINE_VALUE )
        return value;

      return -1;
    }
  }
}
