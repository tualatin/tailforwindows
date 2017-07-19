using System;
using System.Windows.Controls;
using System.Windows.Data;


namespace Org.Vs.TailForWin.Converters.MultiConverters
{
  /// <summary>
  /// TabSizeMultiConverter
  /// </summary>
  class TabSizeMultiConverter : IMultiValueConverter
  {
    #region IMultiValueConverter Members

    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      TabControl tabControl = values?[0] as TabControl;

      if ( tabControl != null )
      {
        double width = tabControl.ActualWidth / tabControl.Items.Count;

        return width <= 1 ? 0 : width - 1;
      }
      return 0;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
