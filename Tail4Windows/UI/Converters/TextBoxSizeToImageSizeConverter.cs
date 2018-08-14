using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Org.Vs.TailForWin.Core.Extensions;
using Org.Vs.TailForWin.UI.Extensions;
using Org.Vs.TailForWin.UI.UserControls;


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
      if ( !(value is Grid grid) )
        return 15;

      Grid templateGrid = grid.Ancestors().OfType<Grid>().FirstOrDefault(p => p.Name == "DataTemplateGrid");
      HighlightTextBlock textBlock = templateGrid?.Descendents().OfType<HighlightTextBlock>().FirstOrDefault(p => p.Name == "TextBoxMessage");

      if ( textBlock == null )
        return 15;

      Size textSize = textBlock.Text.GetMeasureTextSize(new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch), textBlock.FontSize);

      double height;

      if ( textSize.Height > 16 )
        height = 16;
      else if ( textSize.Height < 10 )
        height = 10;
      else
        height = textSize.Height;

      return height - 1;
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
