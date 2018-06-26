using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;


namespace Org.Vs.TailForWin.UI.Converters
{
  /// <summary>
  /// Inverse bool to <see cref="Visibility"/> converter
  /// </summary>
  [ValueConversion(typeof(bool), typeof(Visibility))]
  public class InverseBooleanToVisibilityConverter : IValueConverter
  {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Visibility value</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value != null && (bool) value ? Visibility.Collapsed : Visibility.Visible;

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Back converted value</returns>

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
  }
}
