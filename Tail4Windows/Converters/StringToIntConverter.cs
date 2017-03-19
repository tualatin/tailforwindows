﻿using System;
using System.Windows.Data;


namespace Org.Vs.TailForWin.Converters
{
  /// <summary>
  /// StringToIntConverter
  /// </summary>
  public class StringToIntConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if(value != null)
      {
        if(value.GetType() == typeof(int))
        {
          int port = (int) value;

          if(port < 0)
            port = 0;

          return (port);
        }
      }
      return (null);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}