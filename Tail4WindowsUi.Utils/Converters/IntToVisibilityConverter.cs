using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;


namespace Org.Vs.TailForWin.Ui.Utils.Converters
{
  /// <summary>
  /// <see cref="int"/> to <see cref="Visibility"/> converter
  /// </summary>
  [ValueConversion(typeof(int), typeof(Visibility))]
  public class IntToVisibilityConverter : IValueConverter
  {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ( !(value is int i) )
        return Visibility.Collapsed;

      if ( parameter is string s )
      {
        if ( string.Compare("inverse", s, StringComparison.InvariantCultureIgnoreCase) == 0 )
          return i == 0 ? Visibility.Visible : Visibility.Collapsed;
      }
      return i <= 0 ? Visibility.Collapsed : Visibility.Visible;
    }

    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
  }
}
