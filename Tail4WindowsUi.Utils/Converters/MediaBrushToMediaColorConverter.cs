using System;
using System.Globalization;
using System.Windows.Data;


namespace Org.Vs.TailForWin.Ui.Utils.Converters
{
  /// <summary>
  /// Media brush to media color converter
  /// </summary>
  [ValueConversion(typeof(System.Windows.Media.Brush), typeof(System.Windows.Media.Color))]
  public class MediaBrushToMediaColorConverter : IValueConverter
  {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ( !(value is System.Windows.Media.SolidColorBrush brush) )
        return System.Windows.Media.Colors.Crimson;

      brush.Freeze();
      return brush.Color;
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value to convert back</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ( !(value is System.Windows.Media.Color color) )
        return System.Windows.Media.Brushes.Crimson;

      return new System.Windows.Media.SolidColorBrush(color);
    }
  }
}
