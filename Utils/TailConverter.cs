using System.Windows.Data;
using TailForWin.Data;


namespace TailForWin.Utils
{
  /// <summary>
  /// DataGrid selectedItem converter to bool
  /// </summary>
  public class TailConverter : IValueConverter
  {
    #region IValueConverter Members

    /// <summary>
    /// Convert to bool
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Parameters</param>
    /// <param name="culture">Culture</param>
    /// <returns>True or False</returns>
    public object Convert (object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        if (value.GetType ( ) == typeof (FileManagerData))
          return (true);
        return (false);
      }
      else
        return (false);
    }

    /// <summary>
    /// Convert to object
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Parameters</param>
    /// <param name="culture">Culture</param>
    /// <returns>Object</returns>
    public object ConvertBack (object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new System.NotImplementedException ( );
    }

    #endregion
  }
}
