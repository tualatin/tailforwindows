using System;
using System.Windows.Data;
using System.Windows;
using System.Collections.ObjectModel;
using TailForWin.Data;


namespace TailForWin.Converter
{
  public class FilterCountToStringConverter: IValueConverter
  {
    #region IValueConverter Members

    public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      string title = Application.Current.FindResource ("FiltersLoaded").ToString ( );

      if (value != null)
        return (string.Format (title, ((ObservableCollection<FilterData>) value).Count));
      return (string.Format (title, 0));
    }

    public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException ( );
    }

    #endregion
  }
}
