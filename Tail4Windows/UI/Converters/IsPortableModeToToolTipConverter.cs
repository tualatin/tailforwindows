using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;


namespace Org.Vs.TailForWin.UI.Converters
{
  /// <summary>
  /// Is portable mode to ToolTip converter
  /// </summary>
  public class IsPortableModeToToolTipConverter : IValueConverter
  {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
      !(value is bool b) ?
      Application.Current.TryFindResource("StatusbarUserModeToolTip").ToString() :
      b ?
        Application.Current.TryFindResource("StatusbarPortableModeToolTip").ToString() :
        Application.Current.TryFindResource("StatusbarUserModeToolTip").ToString();

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
