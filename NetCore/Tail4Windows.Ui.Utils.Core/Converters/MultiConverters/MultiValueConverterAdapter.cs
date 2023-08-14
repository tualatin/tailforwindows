using System;
using System.Windows.Data;
using System.Windows.Markup;


namespace Org.Vs.TailForWin.Ui.Utils.Converters.MultiConverters
{
  /// <summary>
  /// Multi value converter adapter
  /// </summary>
  [ContentProperty("Converter")]
  public class MultiValueConverterAdapter : IMultiValueConverter
  {
    private object _lastParameter;
    private IValueConverter _lastConverter;

    /// <summary>
    /// <see cref="IValueConverter"/>
    /// </summary>
    public IValueConverter Converter
    {
      get;
      set;
    }

    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="values">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      _lastConverter = Converter;

      if ( values.Length > 1 )
        _lastParameter = values[1];

      if ( values.Length > 2 )
        _lastConverter = (IValueConverter) values[2];

      return Converter == null ? values[0] : Converter.Convert(values[0], targetType, _lastParameter, culture);
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetTypes">TargetType</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>Back converted value</returns>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
      => _lastConverter == null ? new[] { value } : new[] { _lastConverter.ConvertBack(value, targetTypes[0], _lastParameter, culture) };
  }
}
