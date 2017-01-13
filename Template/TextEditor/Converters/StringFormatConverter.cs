using System;
using System.Windows.Data;
using Org.Vs.TailForWin.Template.TextEditor.Data;


namespace Org.Vs.TailForWin.Template.TextEditor.Converters
{
  /// <summary>
  /// StringFormatConverter
  /// </summary>
  class StringFormatConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if(value == null)
        return (null);

      if(value.GetType() == typeof(DateTime))
      {
        DateTime dt = (DateTime) value;
        return (dt.ToString(StringFormatData.StringFormat));
      }
      return (null);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
