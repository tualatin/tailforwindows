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
  /// TextBox size to image size converter
  /// </summary>
  public class TextBoxSizeToImageSizeConverter : IValueConverter
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
        return 15;

      Size textSize = "123".GetMeasureTextSize(new Typeface(fontType.FontFamily, fontType.FontStyle, fontType.FontWeight, fontType.FontStretch), fontType.FontSize);

      double height;

      if ( textSize.Height > 16 )
        height = 16;
      else if ( textSize.Height < 10 )
        height = 10;
      else
        height = textSize.Height;

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
