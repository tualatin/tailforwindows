using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Org.Vs.TailForWin.Core.Controllers;


namespace Org.Vs.TailForWin.UI.ValidationRules
{
  /// <summary>
  /// SMTP server address validation rule
  /// </summary>
  public class SmtpServerAddressValidation : ValidationRule
  {
    /// <summary>
    /// Validate
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="cultureInfo">Current culture</param>
    /// <returns><see cref="ValidationRule"/></returns>
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
      if ( !(value is string smtpAddress) )
        return new ValidationResult(false, Application.Current.TryFindResource("SmtpServerAddressNecessary").ToString());

      return string.IsNullOrWhiteSpace(smtpAddress) && !string.IsNullOrWhiteSpace(SettingsHelperController.CurrentSettings.SmtpSettings.FromAddress) ?
        new ValidationResult(false, Application.Current.TryFindResource("SmtpServerAddressNecessary").ToString()) :
        new ValidationResult(true, Application.Current.TryFindResource("SmtpServerAddressNecessary").ToString());
    }
  }
}
