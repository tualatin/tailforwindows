using System;
using System.Windows.Data;


namespace TailForWin.Converter
{
  public class SmtpPortConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
        return (null);

      if (value.GetType() == typeof(int))
      {
        int port = (int)value;

        if (port > 0)
          return (port);
        else
          return (25);
      }
      return (null);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
