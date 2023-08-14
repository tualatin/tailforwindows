using System;
using System.Windows;
using System.Windows.Data;


namespace Org.Vs.TailForWin.Ui.Utils.Converters
{
  /// <summary>
  /// BoolToTextWrapConverter
  /// </summary>
  [ValueConversion(typeof(bool), typeof(TextWrapping))]
  public class BoolToTextWrapConverter : IValueConverter
  {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>converted value</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if ( !(value is bool wrap) )
        return TextWrapping.NoWrap;

      return wrap ? TextWrapping.Wrap : TextWrapping.NoWrap;
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">TargtType</param>
    /// <param name="parameter">Paramter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>converted value</returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
  }
}
