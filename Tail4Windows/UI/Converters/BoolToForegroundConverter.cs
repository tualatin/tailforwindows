using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;


namespace Org.Vs.TailForWin.UI.Converters
{
  /// <summary>
  /// Bool to 
  /// </summary>
  [ValueConversion(typeof(bool), typeof(Brush))]
  public class BoolToForegroundConverter : IValueConverter
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
      if ( !(value is bool b) )
        return Brushes.Black;

      return b ? Brushes.RoyalBlue : Brushes.Black;
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
  }
}
