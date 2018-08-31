using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Org.Vs.TailForWin.Core.Controllers;


namespace Org.Vs.TailForWin.Core.Extensions
{
  /// <summary>
  /// String extension class
  /// </summary>
  public static class StringExtension
  {
    /// <summary>
    /// Converts a string to bool
    /// </summary>
    /// <param name="value">String to convert</param>
    /// <param name="defaultValue">Default value is <c>False</c></param>
    /// <returns><c>True</c> or <c>defaultValue</c></returns>
    public static bool ConvertToBool(this string value, bool defaultValue = false) => !bool.TryParse(value, out bool result) ? defaultValue : result;

    /// <summary>
    /// Converts a string to three state bool
    /// </summary>
    /// <param name="value">String to convert</param>
    /// <returns><c>True</c>, <c>False</c> or <c>Null</c></returns>
    public static bool? ConvertToThreeStateBool(this string value) => !bool.TryParse(value, out bool result) ? null : (bool?) result;

    /// <summary>
    /// Converts a string to int
    /// </summary>
    /// <param name="value">String to convert</param>
    /// <param name="defaultValue">Default value is <c>-1</c></param>
    /// <returns>Real integer value, otherwise defaultValue</returns>
    public static int ConvertToInt(this string value, int defaultValue = -1) => !int.TryParse(value, out int result) ? defaultValue : result;

    /// <summary>
    /// Converts a string to double
    /// </summary>
    /// <param name="value">String to convert</param>
    /// <param name="defaultValue">Default value is <c>double.NaN</c></param>
    /// <returns>Real double value, otherwise defaultValue</returns>
    public static double ConvertToDouble(this string value, double defaultValue = double.NaN) => !double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double result) ? defaultValue : result;

    /// <summary>
    /// Converts a string to float
    /// </summary>
    /// <param name="value">String to convert</param>
    /// <param name="defaultValue">Default value is <c>float.NaN</c></param>
    /// <returns>Real float value, otherwise defaultValue</returns>
    public static float ConvertToFloat(this string value, float defaultValue = float.NaN) => !float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float result) ? defaultValue : result;

    /// <summary>
    /// Converts a string to decimal
    /// </summary>
    /// <param name="value">String to convert</param>
    /// <param name="defaultValue">Default value is <c>decimal.MinValue</c></param>
    /// <returns>Real decimal value, otherwise defaultValue</returns>
    public static decimal ConvertToDecimal(this string value, decimal defaultValue = decimal.MinValue) => !decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal result) ? defaultValue : result;

    /// <summary>
    /// Measure a certain text
    /// </summary>
    /// <param name="value">String to measure</param>
    /// <param name="typeface"><see cref="Typeface"/></param>
    /// <param name="fontSize">Font size</param>
    /// <param name="maxsize">Max pixel size</param>
    /// <returns>A cut string if necessary</returns>
    public static string MeasureTextAndCutIt(this string value, Typeface typeface, double fontSize, int maxsize)
    {
      var size = GetMeasureTextSize(value, typeface, fontSize);

      if ( size.Width < maxsize )
        return value;

      string cuttext = string.Empty;

      foreach ( char c in value )
      {
        cuttext += c;
        size = GetMeasureTextSize(cuttext, typeface, fontSize);

        if ( !(size.Width >= maxsize) )
          continue;

        cuttext = cuttext.Substring(0, cuttext.Length > 3 ? cuttext.Length - 3 : cuttext.Length).Trim();
        cuttext += "...";
        break;
      }
      return cuttext;
    }

    /// <summary>
    /// Get size of a text
    /// </summary>
    /// <param name="value">String to measure</param>
    /// <param name="typeface"><see cref="Typeface"/></param>
    /// <param name="fontSize">Font size</param>
    /// <returns><see cref="Size"/></returns>
    public static Size GetMeasureTextSize(this string value, Typeface typeface, double fontSize)
    {
      var formattedText = new FormattedText(
        value,
        SettingsHelperController.CurrentSettings.CurrentCultureInfo,
        FlowDirection.LeftToRight,
        typeface,
        fontSize,
        Brushes.Black,
        new NumberSubstitution(),
        TextFormattingMode.Display);

      var size = new Size(formattedText.Width, formattedText.Height);
      return size;
    }

    /// <summary>
    /// <see cref="List{T}"/> of strings to delimited string
    /// </summary>
    /// <param name="list"><see cref="List{T}"/> of strings</param>
    /// <param name="delimiter">Delimiter</param>
    /// <param name="insertSpaces">Insert spaces</param>
    /// <param name="qualifier">Qualifier</param>
    /// <returns>Flatten string</returns>
    public static string ToDelimitedString(this List<string> list, string delimiter = ":", bool insertSpaces = false, string qualifier = "")
    {
      var result = new StringBuilder();

      for ( var i = 0; i < list.Count; i++ )
      {
        string initialString = list[i];
        result.Append(qualifier == string.Empty ? initialString : $"{qualifier}{initialString}{qualifier}");

        if ( i >= list.Count - 1 )
          continue;

        result.Append(delimiter);

        if ( insertSpaces )
          result.Append(' ');
      }
      return result.ToString();
    }
  }
}
