using System;
using System.Windows.Data;
using Org.Vs.TailForWin.Data;


namespace Org.Vs.TailForWin.Converters
{
  /// <summary>
  /// SoundFileName converter
  /// </summary>
  public class SoundFileNameConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if(value == null)
        return (string.Empty);

      if(value is string)
        return (String.Compare(((string) value), LogFile.ALERT_SOUND_FILENAME, StringComparison.Ordinal) == 0 ? (string.Empty) : (value));

      return (string.Empty);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return (value);
    }

    #endregion
  }
}
