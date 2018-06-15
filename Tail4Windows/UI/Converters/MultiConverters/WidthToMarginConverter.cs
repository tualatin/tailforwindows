using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


namespace Org.Vs.TailForWin.UI.Converters.MultiConverters
{
  /// <summary>
  /// <see cref="DataGridLength"/> to <see cref="Thickness"/> converter
  /// </summary>
  [ValueConversion(typeof(DataGridLength), typeof(Thickness))]
  public class WidthToMarginMultiConverter : IMultiValueConverter
  {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
    {
      if ( !(value.First() is DataGridLength gridColumnFileName) || !(value.Last() is DataGridLength gridColumnNo) )
        return new Thickness(0, 0, 0, 0);

      double offset = gridColumnNo.DisplayValue + gridColumnFileName.DisplayValue;

      return new Thickness(offset, 0, 0, 0);
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>Back converted value</returns>
    public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
  }
}
