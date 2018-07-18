using System;
using System.Globalization;
using System.Windows.Data;
using Org.Vs.TailForWin.Core.Enums;


namespace Org.Vs.TailForWin.UI.Converters
{
  /// <summary>
  /// Statusbar state to bool converter
  /// </summary>
  [ValueConversion(typeof(EStatusbarState), typeof(bool))]
  public class StatusBarStateToBoolConverter : IValueConverter
  {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>Converted value</returns>

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ( !(value is EStatusbarState state) )
        return false;

      return state != EStatusbarState.Busy;
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">TargtType</param>
    /// <param name="parameter">Paramter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>Converted value</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
  }
}
