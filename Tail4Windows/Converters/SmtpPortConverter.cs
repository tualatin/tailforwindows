using System;
using System.Windows.Data;


namespace Org.Vs.TailForWin.Converters
{
  /// <summary>
  /// SmtpPortConverter
  /// </summary>
  public class SmtpPortConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if(value is int)
      {
        int port = (int) value;

        if(port > 0)
          return port;

        return 25;
      }
      return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
