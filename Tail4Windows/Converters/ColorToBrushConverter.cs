using System;
using System.Windows.Data;
using System.Windows.Media;


namespace Org.Vs.TailForWin.Converters
{
  /// <summary>
  /// ColorToBrushConverter
  /// </summary>
  class ColorToBrushConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if(value is System.Drawing.Color)
      {
        int num;

        if(!int.TryParse(((System.Drawing.Color) value).Name, out num))
        {
          var convertFromString = ColorConverter.ConvertFromString(((System.Drawing.Color) value).Name);

          if(convertFromString != null)
          {
            Color mediaColor = (Color) convertFromString;
            Brush brush = new SolidColorBrush(mediaColor);

            return brush;
          }
        }
        else
        {
          return null;
        }
      }
      return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return null;
    }

    #endregion
  }
}
