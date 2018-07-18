using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.UI.Converters
{
  /// <summary>
  /// Log line limit to label converter
  /// </summary>
  [ValueConversion(typeof(int), typeof(string))]
  public class LogLineLimitToLabelConverter : IValueConverter
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
      if ( !(value is double number) )
        return string.Empty;

      if ( (int) number == -1 || Equals((int) number, EnvironmentContainer.UnlimitedLogLineValue) )
        return Application.Current.TryFindResource("ExtrasLogLineUnlimited");

      return $"{number:N0} {Application.Current.TryFindResource("ExtrasLogLineLines")}";
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
