using System;
using System.Windows.Data;


namespace TailForWin.Converter
{
  public class MailAddressConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        if (value.GetType ( ) == typeof (string))
        {
          string eMailAddress = value as string;

          if (eMailAddress.CompareTo ("NoMail") == 0)
            return (string.Empty);
          else
            return (eMailAddress);
        }
        else
          return (string.Empty);
      }
      return (string.Empty);
    }

    public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException ( );
    }

    #endregion
  }
}
