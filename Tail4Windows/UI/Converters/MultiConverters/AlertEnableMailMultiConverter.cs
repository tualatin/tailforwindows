using System;
using System.Globalization;
using System.Windows.Data;


namespace Org.Vs.TailForWin.UI.Converters.MultiConverters
{
  /// <summary>
  /// Alert enable E-Mail multi converter
  /// </summary>
  public class AlertEnableMailMultiConverter : IMultiValueConverter
  {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="values">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      foreach ( var value in values )
      {
        if ( !(value is string s) )
          return false;

        if ( string.IsNullOrWhiteSpace(s) )
          return false;
      }
      return true;
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetTypes">TargetType</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>Back converted value</returns>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
