using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;


namespace Org.Vs.TailForWin.UI.Converters
{
  /// <summary>
  /// SmartWatch interval to string converter
  /// </summary>
  [ValueConversion(typeof(double), typeof(string))]
  public class SmartWatchIntervalToString : IValueConverter
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
      if ( !(value is double i) )
        return string.Empty;

      string seconds = Application.Current.TryFindResource("SmartWatchIntervalSeconds").ToString();
      string minutes = Application.Current.TryFindResource("SmartWatchIntervalMinutes").ToString();
      string result;

      if ( i >= 60000 )
      {
        var timeSpan = TimeSpan.FromMilliseconds(i);
        result = string.Format(minutes, timeSpan.Minutes, timeSpan.Seconds);
      }
      else
      {
        result = string.Format(seconds, TimeSpan.FromMilliseconds(i).TotalSeconds);
      }
      return result;
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>Back converted value</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
  }
}
