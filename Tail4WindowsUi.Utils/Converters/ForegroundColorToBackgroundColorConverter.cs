using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Core.Data.Settings;

namespace Org.Vs.TailForWin.Ui.Utils.Converters
{
  /// <summary>
  /// Foreground color to background color converter
  /// </summary>
  [ValueConversion(typeof(Brush), typeof(Brush))]
  public class ForegroundColorToBackgroundColorConverter : IValueConverter
  {
    private readonly SolidColorBrush _defaultIsNotSelectedColor;

    /// <summary>
    /// Standard constructor
    /// </summary>
    // ReSharper disable once PossibleNullReferenceException
    public ForegroundColorToBackgroundColorConverter() => _defaultIsNotSelectedColor = new SolidColorBrush((Color) ColorConverter.ConvertFromString(DefaultEnvironmentSettings.TabItemHeaderBackgroundColor));

    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Is the background color</param>
    /// <param name="targetType"></param>
    /// <param name="parameter">Is the color to be set lighter or darker</param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ( !(value is SolidColorBrush backgroundColor) )
        backgroundColor = _defaultIsNotSelectedColor;

      if ( !(parameter is SolidColorBrush defaultColor) )
        defaultColor = Brushes.White;

      double l = 0.2126 * backgroundColor.Color.ScR + 0.7152 * backgroundColor.Color.ScG + 0.0722 * backgroundColor.Color.ScB;

      var newColor = l > 0.4
        ? BackgroundColorToIsNotSelectedConverter.DarkerColor(EnvironmentContainer.ConvertMediaBrushToDrawingColor(defaultColor), 95f)
        : BackgroundColorToIsNotSelectedConverter.LighterColor(EnvironmentContainer.ConvertMediaBrushToDrawingColor(defaultColor));

      return new SolidColorBrush(Color.FromRgb(newColor.R, newColor.G, newColor.B));
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
  }
}
