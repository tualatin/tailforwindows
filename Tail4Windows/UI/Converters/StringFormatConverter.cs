using System;
using System.Windows.Data;
using Org.Vs.TailForWin.Core.Controllers;


namespace Org.Vs.TailForWin.UI.Converters
{
  /// <summary>
  /// StringFormatConverter
  /// </summary>
  public class StringFormatConverter : IValueConverter
  {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if ( !(value is DateTime dt) )
        return null;

      return dt.ToString(SettingsHelperController.CurrentSettings.CurrentStringFormat);
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
  }
}
