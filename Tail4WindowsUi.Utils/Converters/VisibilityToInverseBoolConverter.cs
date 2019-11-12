using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;


namespace Org.Vs.TailForWin.Ui.Utils.Converters
{
  /// <summary>
  /// Visibility to inverse bool converter
  /// </summary>
  [ValueConversion(typeof(Visibility), typeof(bool))]
  public class VisibilityToInverseBoolConverter : IValueConverter
  {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is Visibility visibility && visibility != Visibility.Collapsed;

    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var flag = false;

      if ( value != null )
      {
        var boolType = value.GetType();

        if ( boolType == typeof(bool) )
        {
          flag = (bool) value;
        }
        else if ( boolType == typeof(bool?) )
        {
          var nullable = (bool?) value;
          flag = nullable.Value;
        }
      }
      else
      {
        flag = true;
      }
      return flag ? Visibility.Visible : Visibility.Collapsed;
    }
  }
}
