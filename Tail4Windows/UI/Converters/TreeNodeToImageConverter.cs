using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace Org.Vs.TailForWin.UI.Converters
{
  /// <summary>
  /// TreeNode to Image converter
  /// </summary>
  [ValueConversion(typeof(string), typeof(ImageSource))]
  public class TreeNodeToImageConverter : IValueConverter
  {
    private const string UriFormat = "pack://application:,,,/Resources/{0}";

    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted type</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ( value is string source)
        return new BitmapImage(new Uri(string.Format(UriFormat, source)));

      return Binding.DoNothing;
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted type</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
