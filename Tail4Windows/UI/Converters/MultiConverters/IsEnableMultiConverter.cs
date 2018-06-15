using System;
using System.Globalization;
using System.Windows.Data;


namespace Org.Vs.TailForWin.UI.Converters.MultiConverters
{
  /// <summary>
  /// IsEnable multi converter
  /// </summary>
  public class IsEnableMultiConverter : IMultiValueConverter
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
      bool result = true;

      foreach ( var value in values )
      {
        if ( !(value is bool b1) )
          result = false;
        else
          result &= b1;
      }
      return result;
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetTypes">TargetType</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>Back converted value</returns>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
  }
}
