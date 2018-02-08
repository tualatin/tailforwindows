using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;


namespace Org.Vs.TailForWin.UI.Converters
{
  /// <summary>
  /// Convert a foreground color controlled by background color
  /// </summary>
  public class BackgroundToForegroundConverter : IValueConverter
  {
    private readonly SolidColorBrush _defaultForegroundColor;
    private readonly SolidColorBrush _foregroundColor;

    /// <summary>
    /// Standard constructor
    /// </summary>
    public BackgroundToForegroundConverter()
    {
      _defaultForegroundColor = new SolidColorBrush(Colors.Black);
      _foregroundColor = new SolidColorBrush(Color.FromRgb(240, 240, 240));
    }

    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>Foreground color</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ( !(value is SolidColorBrush backgroundColor) )
        return _defaultForegroundColor;

      double l = 0.2126 * backgroundColor.Color.ScR + 0.7152 * backgroundColor.Color.ScG + 0.0722 * backgroundColor.Color.ScB;
      return l > 0.5 ? _defaultForegroundColor : _foregroundColor;
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">TargtType</param>
    /// <param name="parameter">Paramter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>Back converted value</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
