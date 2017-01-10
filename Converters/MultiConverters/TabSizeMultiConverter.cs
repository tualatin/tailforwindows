using System;
using System.Windows.Data;
using System.Windows.Controls;


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
      TabControl tabControl = values[0] as TabControl;
      double width = tabControl.ActualWidth / tabControl.Items.Count;

      return ((width <= 1) ? 0 : (width - 1));
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
