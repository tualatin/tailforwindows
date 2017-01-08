using System;
using System.Windows.Data;
using System.Windows;


namespace TailForWin.Converter
{
  /// <summary>
  /// Integer to bool converter
  /// </summary>
  [ValueConversion(typeof(int), typeof(bool))]
  public class IntToBoolConverter : IValueConverter
  {
    #region IValueConverter Members

    /// <summary>
    /// Int to bool
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Trim text</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      try
      {
        return (System.Convert.ToInt32(value) > 0);
      }
      catch (InvalidCastException)
      {
        return (DependencyProperty.UnsetValue);
      }
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Object</returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return (value);
    }

    #endregion
  }
}
