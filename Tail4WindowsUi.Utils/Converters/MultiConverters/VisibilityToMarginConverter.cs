using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;


namespace Org.Vs.TailForWin.Ui.Utils.Converters.MultiConverters
{
  /// <summary>
  /// Visibility to margin converter
  /// </summary>
  [ValueConversion(typeof(Visibility), typeof(Thickness))]
  public class VisibilityToMarginMultiConverter : IMultiValueConverter
  {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="values">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if ( !(values.First() is Visibility) || !(values.Last() is Visibility) )
        return new Thickness(5, 0, 0, 0);

      var first = (Visibility) values.First();
      var last = (Visibility) values.Last();

      switch ( parameter )
      {
      case "DateTimeEditor":

        if ( first == Visibility.Visible )
          return new Thickness(10, 0, 0, 0);

        return new Thickness(5, 0, 0, 0);

      case "TextBoxMessage":

        if ( first == Visibility.Visible && last == Visibility.Visible )
          return new Thickness(5, 0, 0, 0);
        else if ( first == Visibility.Visible && last == Visibility.Collapsed )
          return new Thickness(10, 0, 0, 0);

        return new Thickness(5, 0, 0, 0);

      default:

        return new Thickness(5, 0, 0, 0);
      }
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetTypes">TargetType</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>Back converted value</returns>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
  }
}
