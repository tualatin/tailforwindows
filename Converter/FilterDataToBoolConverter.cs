using System;
using System.Windows.Data;
using TailForWin.Data;


namespace TailForWin.Converter
{
  public class FilterDataToBoolConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        if (value.GetType ( ) == typeof (FilterData))
          return (true);
        else
          return (false);
      }
      return (false);
    }

    public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException ( );
    }

    #endregion
  }
}
