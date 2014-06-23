using System;
using System.Windows.Data;
using TailForWin.Data;


namespace TailForWin.Converter
{
  class EnableOpenMultiConverter: IMultiValueConverter
  {
    #region IMultiValueConverter Members

    public object Convert (object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (values != null)
      {
        bool isEnable = false;
        bool isOpenFromFileManager = false;

        if (values[0] != null && values[0].GetType ( ) == typeof (FileManagerData))
          isEnable = true;
        if (values[1] != null && values[1].GetType ( ) == typeof (FileManagerData))
          isOpenFromFileManager = (values[1] as FileManagerData).OpenFromFileManager;

        if (isOpenFromFileManager)
          return (false);
        else
          return (isEnable);
      }
      return (false);
    }

    public object[] ConvertBack (object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException ( );
    }

    #endregion
  }
}
