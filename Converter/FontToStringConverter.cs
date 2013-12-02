using System;
using System.Windows.Data;
using System.Drawing;


namespace TailForWin.Converter
{
  public class FontToStringConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        if (value.GetType ( ) == typeof (Font))
        {
          Font font = value as Font;

          return (string.Format ("{0} {1}", font.FontFamily.Name, font.Size));
        }
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
