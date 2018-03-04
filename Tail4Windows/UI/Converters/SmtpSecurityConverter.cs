using System;
using System.Windows.Data;
using Org.Vs.TailForWin.Core.Controllers;


namespace Org.Vs.TailForWin.UI.Converters
{
  /// <summary>
  /// SmtpPortConverter
  /// </summary>
  public class SmtpSecurityConverter : IValueConverter
  {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>Converted value</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if ( !(value is bool ssl) )
        return 0;

      if ( !ssl )
        return SettingsHelperController.CurrentSettings.SmtpSettings.Tls ? 2 : 0;

      return 1;
    }

    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>Converted value</returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if ( !(value is int index) )
        return false;

      switch ( index )
      {
      case 0:

        SettingsHelperController.CurrentSettings.SmtpSettings.Tls = false;
        return false;

      case 1:

        SettingsHelperController.CurrentSettings.SmtpSettings.Tls = false;
        return true;

      case 2:

        SettingsHelperController.CurrentSettings.SmtpSettings.Tls = true;
        return false;

      default:

        throw new IndexOutOfRangeException();
      }
    }
  }
}
