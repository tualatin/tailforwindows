using System;
using System.Globalization;
using System.Windows.Data;


namespace Org.Vs.TailForWin.UI.Converters
{
  /// <summary>
  /// Time delta to string format converter
  /// </summary>
  public class TimeDeltaStringFormatConverter : IValueConverter
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
      if ( value == null )
        return "0 ms";

      if ( !(value is TimeSpan ts) )
        return "0 ms";

      var delta = TimeSpan.FromMilliseconds(Math.Abs(ts.TotalMilliseconds));

      if ( delta >= TimeSpan.FromDays(value: 1) )
        return $"+{ts.TotalDays:N0} d";

      if ( delta >= TimeSpan.FromHours(value: 1) )
        return $"+{ts.TotalHours:N0} h";

      if ( delta >= TimeSpan.FromMinutes(value: 1) )
        return $"+{ts.TotalMinutes:N0} m";

      if ( delta >= TimeSpan.FromSeconds(value: 1) )
        return $"+{ts.TotalSeconds:N0} s";

      if ( Equals(ts.TotalMilliseconds, 0d) )
        return $"{ts.TotalMilliseconds:N0} ms";

      return $"+{ts.TotalMilliseconds:N0} ms";
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
  }
}
