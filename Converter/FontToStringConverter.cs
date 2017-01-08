﻿using System;
using System.Windows.Data;
using System.Drawing;


namespace Org.Vs.TailForWin.Converter
{
  public class FontToStringConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        if (value.GetType() == typeof(Font))
        {
          Font font = value as Font;

          return (string.Format("{0} ({1}) {2} {3}", font.Name, font.Size, font.Italic ? "Italic" : string.Empty, font.Bold ? "Bold" : string.Empty));
        }
      }
      return (string.Empty);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
