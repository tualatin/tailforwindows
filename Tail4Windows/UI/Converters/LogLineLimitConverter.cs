using System;
using System.Globalization;
using System.Windows.Data;
using Org.Vs.TailForWin.Business.Utils;


namespace Org.Vs.TailForWin.UI.Converters
{
  /// <summary>
  /// Log line limit converter
  /// </summary>
  [ValueConversion(typeof(int), typeof(int))]
  public class LogLineLimitConverter : IValueConverter
  {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Parameters</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ( !(value is int iValue) )
        return EnvironmentContainer.UnlimitedLogLineValue;

      return iValue > -1 ? iValue : EnvironmentContainer.UnlimitedLogLineValue;
    }

    /// <summary>
    /// Convert to object
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Parameters</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ( value as double? < EnvironmentContainer.UnlimitedLogLineValue )
        return value;

      return -1;
    }
  }
}
