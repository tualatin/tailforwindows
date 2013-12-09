using System;
using System.Windows.Data;
using System.Windows;


namespace TailForWin.Template.TextEditor.Converter
{
  public class BoolToTextWrapConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value.GetType ( ) == typeof (bool))
      {
        bool wrap = (bool) value;

        if (wrap)
          return (TextWrapping.Wrap);
        else
          return (TextWrapping.NoWrap);
      }
      return (TextWrapping.NoWrap);
    }

    public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException ( );
    }
    
    #endregion
  }
}
