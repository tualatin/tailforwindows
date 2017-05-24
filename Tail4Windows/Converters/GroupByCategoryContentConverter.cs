using System;
using System.Globalization;
using System.Windows.Data;


namespace Org.Vs.TailForWin.Converters
{
  /// <summary>
  /// Group by category context menu header text converter
  /// </summary>
  public class GroupByCategoryContentConverter : IValueConverter
  {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>Converted object</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if(value is bool bValue)
      {
        if(bValue)
          return "Ungroup";

      }
      return "Group by Category";
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value to convert back</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Paremeter</param>
    /// <param name="culture">Curent culture</param>
    /// <returns>Converted object</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
