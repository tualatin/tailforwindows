using System;
using System.Windows;
using System.Windows.Data;


namespace Org.Vs.TailForWin.Ui.Utils.Converters
{
  /// <summary>
  /// FilterCountToStringConverter
  /// </summary>
  public class FilterCountToStringConverter : IValueConverter
  {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>Converted value</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if ( !(value is int i) )
        return string.Format(Application.Current.TryFindResource("FiltersLoaded").ToString(), 0);

      string title = i == 1 ? Application.Current.TryFindResource("FilterLoaded").ToString() : Application.Current.TryFindResource("FiltersLoaded").ToString();

      return string.Format(title, i);
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>Back converted value</returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => null;
  }
}
