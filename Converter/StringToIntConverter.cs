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
        int port;

        if (int.TryParse (value as string, out port))
          port = 0;

        if (port < 0)
          port = 0;

        return (port);
      }
      return (null);
    }

    public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException ( );
    }
  }
}
