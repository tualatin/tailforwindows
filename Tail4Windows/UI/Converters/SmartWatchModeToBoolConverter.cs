using System;
using System.Globalization;
using System.Windows.Data;
using Org.Vs.TailForWin.Core.Enums;


namespace Org.Vs.TailForWin.UI.Converters
{
  /// <summary>
  /// SmartWatch enum mode to bool converter
  /// </summary>
  [ValueConversion(typeof(ESmartWatchMode), typeof(bool))]
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
      if ( !(value is ESmartWatchMode mode) )
        return false;

      return mode != ESmartWatchMode.Manual;
    }

    /// <summary>
    /// Convert to object
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Parameters</param>
    /// <param name="culture">Culture</param>
    /// <returns>Object</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
  }
}
