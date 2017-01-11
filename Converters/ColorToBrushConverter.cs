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

    public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        if (value.GetType() == typeof(System.Drawing.Color))
        {
          int num = -1;

          if (!int.TryParse(((System.Drawing.Color) value).Name, out num))
          {
            Color mediaColor = (Color) ColorConverter.ConvertFromString(((System.Drawing.Color) value).Name);
            Brush brush = new SolidColorBrush((Color) mediaColor);

            return (brush);
          }
          else
          {
            return (null);
          }
        }
      }
      return (null);
    }

    public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
