using System;
using System.Windows.Data;


namespace TailForWin.Converter
{
  public class StringToIntConverter : IValueConverter
  {
    public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        if (value.GetType ( ) == typeof (int))
        {

          int port = (int) value;

          if (port < 0)
            port = 0;

          return (port);
        }
      }
      return (0);
    }

    public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException ( );
    }
  }
}
