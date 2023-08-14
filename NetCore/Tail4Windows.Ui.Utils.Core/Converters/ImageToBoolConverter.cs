using System;
using System.Globalization;
using System.Windows.Data;
using Org.Vs.TailForWin.Business.Utils;


namespace Org.Vs.TailForWin.Ui.Utils.Converters
{
  /// <summary>
  /// 
  /// </summary>
  [ValueConversion(typeof(System.Windows.Media.ImageSource), typeof(bool))]
  public class ImageToBoolConverter : IValueConverter
  {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is System.Windows.Media.ImageSource;

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
      !(value is bool b) ? null : (b ? BusinessHelper.CreateBitmapIcon("/T4W;component/Resources/Bookmark.png") : null);
  }
}
