using System;
using System.Globalization;
using System.Windows.Data;
using Org.Vs.TailForWin.Data;


namespace Org.Vs.TailForWin.Converters.MultiConverters
{
  /// <summary>
  /// Enable use pattern CheckBox multi converter
  /// </summary>
  public class EnableUsePatternCheckBoxMultiConverter : IMultiValueConverter
  {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="values">Values to convert</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Curent culture</param>
    /// <returns>Converted type</returns>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if(values != null)
      {
        bool patternSet = false;
        bool dataGridSelection = values[0] is FileManagerData;

        var pattern = values[1] as string;

        if(!string.IsNullOrEmpty(pattern))
          patternSet = true;

        return (patternSet & dataGridSelection);
      }
      return (false);
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetTypes">TargetTypes</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>Converted types</returns>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
