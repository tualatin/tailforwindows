using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Org.Vs.TailForWin.Controllers.PlugIns.StatisticAnalysis.Data.Enums;


namespace Org.Vs.TailForWin.Ui.Utils.Converters
{
  /// <summary>
  /// Analysis enum to visibility converter <see cref="EAnalysisOf"/> to <see cref="Visibility"/>
  /// </summary>
  [ValueConversion(typeof(EAnalysisOf), typeof(Visibility))]
  public class AnalysisEnumToVisibilityConverter : IValueConverter
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
      if ( !(value is EAnalysisOf enumValue) )
        return Visibility.Collapsed;

      if ( !(parameter is EAnalysisOf param) )
        return Visibility.Collapsed;

      return enumValue == param ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>Back converted value</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
  }
}
