using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;


namespace Org.Vs.TailForWin.Ui.Utils.Converters
{
  /// <summary>
  /// Count to foreground converter
  /// </summary>
  [ValueConversion(typeof(string), typeof(Brush))]
  public class CountToForegroundConverter : IValueConverter
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
      if ( !(value is string s) || string.IsNullOrWhiteSpace(s) )
        return Brushes.Black;

      try
      {
        var split = s.Split(':');
        var digitArray = split.Last().Where(char.IsDigit).ToArray();
        int result = System.Convert.ToInt32(digitArray[0].ToString());

        return result == 0 ? Brushes.IndianRed : Brushes.RoyalBlue;
      }
      catch
      {
        return Brushes.Black;
      }
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
