using System;
using System.Globalization;
using System.Windows.Data;
using Org.Vs.TailForWin.Data.Enums;


namespace Org.Vs.TailForWin.Converters
{
  /// <summary>
  /// SmartWatch enum mode to bool converter
  /// </summary>
  public class SmartWatchModeToBoolConverter : IValueConverter
  {
    /// <summary>
    /// Convert to bool
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Parameters</param>
    /// <param name="culture">Culture</param>
    /// <returns>True or False</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ( value is ESmartWatchMode mode )
      {
        if ( mode == ESmartWatchMode.Manual )
          return false;

        return true;
      }
      return false;
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
      throw new NotImplementedException();
    }
  }
}
