using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Extensions;


namespace Org.Vs.TailForWin.UI.Converters
{
  /// <summary>
  /// <see cref="FontType"/> to <see cref="double"/> converter
  /// </summary>
  [ValueConversion(typeof(FontType), typeof(double))]
  public class FontTypeToImageSizeConverter : IValueConverter
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
      if ( !(value is FontType fontType) )
        return 16;

      Size textSize = "1".GetMeasureTextSize(new Typeface(fontType.FontFamily, fontType.FontStyle, fontType.FontWeight, fontType.FontStretch), fontType.FontSize);
      double height = textSize.Height > 16 ? 16 : (textSize.Height < 10 ? 10 : textSize.Height);

      return height - 2;
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
  }
}
