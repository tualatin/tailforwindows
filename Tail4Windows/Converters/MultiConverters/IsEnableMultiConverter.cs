using System;
using System.Windows.Data;


namespace Org.Vs.TailForWin.Converters.MultiConverters
{
  /// <summary>
  /// IsEnableMultiConverter
  /// </summary>
  public class IsEnableMultiConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if ( values != null )
      {
        bool radio = (bool) values[0];
        int url = (int) values[1];

        if ( !radio )
          return true;

        if ( url > 0 )
          return true;

        return false;
      }
      return true;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
