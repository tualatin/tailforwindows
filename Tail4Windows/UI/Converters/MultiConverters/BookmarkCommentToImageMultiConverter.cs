using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Org.Vs.TailForWin.Business.Utils;


namespace Org.Vs.TailForWin.UI.Converters.MultiConverters
{
  /// <summary>
  /// Bookmark comment to image multi converter
  /// </summary>
  public class BookmarkCommentToImageMultiConverter : IMultiValueConverter
  {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="values">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if ( values.Length != 2 )
        return null;

      if ( values[0] == null )
        return null;

      if ( !(values[1] is string s) )
        return values.First();

      var image = BusinessHelper.CreateBitmapIcon("/T4W;component/Resources/Bookmark_Info.png");

      if ( !string.IsNullOrWhiteSpace(s) )
        values[0] = image;

      return values.First();
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetTypes">TargetType</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>Back converted value</returns>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
  }
}
