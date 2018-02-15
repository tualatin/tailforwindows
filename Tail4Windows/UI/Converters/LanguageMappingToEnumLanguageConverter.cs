using System;
using System.Globalization;
using System.Windows.Data;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Enums;


namespace Org.Vs.TailForWin.UI.Converters
{
  /// <summary>
  /// Language mapping to enum language converter
  /// </summary>
  [ValueConversion(typeof(LanguageMapping), typeof(EUiLanguage))]
  public class LanguageMappingToEnumLanguageConverter : IValueConverter
  {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value == null ? new LanguageMapping { Language = EUiLanguage.English } : new LanguageMapping { Language = (EUiLanguage) value };

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ( !(value is LanguageMapping language) )
        return EUiLanguage.English;

      return language.Language;
    }
  }
}
