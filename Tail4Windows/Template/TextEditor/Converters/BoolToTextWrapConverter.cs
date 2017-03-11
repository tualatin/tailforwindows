using System;
using System.Windows;
using System.Windows.Data;


namespace Org.Vs.TailForWin.Template.TextEditor.Converters
{
  /// <summary>
  /// BoolToTextWrapConverter
  /// </summary>
  public class BoolToTextWrapConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if(value.GetType() == typeof(bool))
      {
        bool wrap = (bool) value;

        if(wrap)
          return (TextWrapping.Wrap);
        else
          return (TextWrapping.NoWrap);
      }
      return (TextWrapping.NoWrap);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
