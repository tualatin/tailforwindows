using System;
using System.Globalization;
using System.Windows.Data;
using Org.Vs.TailForWin.Data;


namespace Org.Vs.TailForWin.Converters.MultiConverters
{
  /// <summary>
  /// EnableOpenMultiConverter
  /// </summary>
  class EnableOpenMultiConverter : IMultiValueConverter
  {
    #region IMultiValueConverter Members

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if(values != null)
      {
        bool isEnable = values[0] is FileManagerData;
        return isEnable;
      }
      return false;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
