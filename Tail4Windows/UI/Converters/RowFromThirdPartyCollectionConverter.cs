using System;
using System.Globalization;
using System.Windows.Data;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.AboutOption.Data;


namespace Org.Vs.TailForWin.UI.Converters
{
  /// <summary>
  /// Row from ThirdParty collection converter
  /// </summary>
  public class RowFromThirdPartyCollectionConverter : IValueConverter
  {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      int rn = 0;

      if ( ThirdPartyCollectionViewHolder.Cv == null || value == null )
        return rn;

      rn = ThirdPartyCollectionViewHolder.Cv.IndexOf(value);

      return rn + 1;
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>Back converted value</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
  }
}
