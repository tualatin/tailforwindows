using System;
using System.Globalization;
using System.Windows.Data;


namespace Org.Vs.TailForWin.Ui.Utils.Converters.MultiConverters
{
  /// <summary>
  /// Bool multi converter
  /// </summary>
  public class BoolMultiConverter : IMultiValueConverter
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
      var result = false;
      var index = 0;

      foreach ( var value in values )
      {
        if ( !(value is bool b) )
          return false;

        if ( index > 0 && !b )
          result = true;
        else
          result = false;

        index++;
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
