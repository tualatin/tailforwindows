using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;


namespace Org.Vs.TailForWin.UI.ValidationRules
{
  /// <summary>
  /// E-Mail validation rule
  /// </summary>
  public class MailAddressValidation : ValidationRule
  {
    /// <summary>
    /// Validate
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="cultureInfo">Current culture</param>
    /// <returns><see cref="ValidationRule"/></returns>
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
      if ( !(value is string address) )
        return new ValidationResult(false, Application.Current.TryFindResource("EMailAddressNotValid").ToString());

      if ( string.IsNullOrWhiteSpace(address) )
        return new ValidationResult(true, Application.Current.TryFindResource("EMailAddressNotValid").ToString());

      Regex mailAddress = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");

      return new ValidationResult(mailAddress.IsMatch(address), Application.Current.TryFindResource("EMailAddressNotValid").ToString());
    }
  }
}
