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
        bool isEnable = false;
        bool isOpenFromFileManager = false;

        if(values[0] != null && values[0] is FileManagerData fmData)
        {
          isEnable = true;
          isOpenFromFileManager = fmData.OpenFromFileManager;
        }

        if(isOpenFromFileManager)
          return (false);
        else
          return (isEnable);
      }
      return (false);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
