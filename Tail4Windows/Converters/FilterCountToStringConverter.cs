using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using Org.Vs.TailForWin.Data;


namespace Org.Vs.TailForWin.Converters
{
  /// <summary>
  /// FilterCountToStringConverter
  /// </summary>
  public class FilterCountToStringConverter : IValueConverter
  {
    #region IValueConverter Members

    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>Converted value</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if ( value != null )
      {
        int count = ((ObservableCollection<FilterData>) value).Count;
        string title = count == 1 ? Application.Current.FindResource("FilterLoaded").ToString() : Application.Current.FindResource("FiltersLoaded").ToString();

        return string.Format(title, count);
      }
      return string.Format(Application.Current.FindResource("FiltersLoaded").ToString(), 0);
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>Back converted value</returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return null;
    }

    #endregion
  }
}
