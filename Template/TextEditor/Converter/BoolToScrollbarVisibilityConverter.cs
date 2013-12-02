﻿using System;
using System.Windows.Data;
using System.Windows.Controls;


namespace TailForWin.Template.TextEditor.Converter
{
  class BoolToScrollbarVisibilityConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value.GetType ( ) == typeof (bool))
      {
        bool wrap = (bool) value;

        if (wrap)
          return (ScrollBarVisibility.Disabled);
        else
          return (ScrollBarVisibility.Auto);
      }
      return (ScrollBarVisibility.Auto);
    }

    public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException ( );
    }

    #endregion
  }
}
