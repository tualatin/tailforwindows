using System;
using System.Windows.Data;


namespace TailForWin.Template.TextEditor.Converter
{
  public static class StringFormatData
  {
    /// <summary>
    /// Set application wide string format for DateTime
    /// </summary>
    public static string StringFormat
    {
      get;
      set;
    }
  }

  class StringFormatConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
        return (null);

      if (value.GetType() == typeof(DateTime))
      {
        DateTime dt = (DateTime)value;
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
