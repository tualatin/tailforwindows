using System;
using System.Windows.Data;


namespace Org.Vs.TailForWin.Converters
{
  /// <summary>
  /// MailAddress converter
  /// </summary>
  public class MailAddressConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if(value == null)
        return (string.Empty);

      if(!(value is string))
        return (string.Empty);

      string eMailAddress = value as string;

      return (string.Compare(eMailAddress, "NoMail", StringComparison.Ordinal) == 0 ? (string.Empty) : (eMailAddress));
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return (value);
    }

    #endregion
  }
}
