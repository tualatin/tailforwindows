using System;
using System.Globalization;
using System.Windows.Data;


namespace Org.Vs.TailForWin.UI.Converters.MultiConverters
{
  /// <summary>
  /// Use proxy multi converter
  /// </summary>
  public class UseProxyMultiConverter : IMultiValueConverter
  {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="values">List of values</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetTypes">List of target types</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
