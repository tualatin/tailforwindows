using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;


namespace Org.Vs.TailForWin.UI.Converters.MultiConverters
{
  /// <summary>
  /// Log file length / FileManager multi converter
  /// </summary>
  public class LogFileFileManagerMultiConverter : IMultiValueConverter
  {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="values">Value to convert</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if ( !(values.First() is bool i) )
        return false;
      if ( !(values.Last() is bool b) )
        return false;

      return i && !b;
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value to convert back</param>
    /// <param name="targetTypes">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
  }
}
