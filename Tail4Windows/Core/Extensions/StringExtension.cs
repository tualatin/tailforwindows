using System.Globalization;


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
    public static bool ConvertToBool(this string value, bool defaultValue = false)
    {
      return !bool.TryParse(value, out var result) ? defaultValue : result;
    }

    /// <summary>
    /// Converts a string to int
    /// </summary>
    /// <param name="value">String to convert</param>
    /// <param name="defaultValue">Default value is <c>-1</c></param>
    /// <returns>Real integer value, otherwise defaultValue</returns>
    public static int ConverToInt(this string value, int defaultValue = -1)
    {
      return !int.TryParse(value, out var result) ? defaultValue : result;
    }

    /// <summary>
    /// Converts a string to double
    /// </summary>
    /// <param name="value">String to convert</param>
    /// <param name="defaultValue">Default value is <c>double.NaN</c></param>
    /// <returns>Real double value, otherwise defaultValue</returns>
    public static double ConvertToDouble(this string value, double defaultValue = double.NaN)
    {
      return !double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) ? defaultValue : result;
    }

    /// <summary>
    /// Converts a string to float
    /// </summary>
    /// <param name="value">String to convert</param>
    /// <param name="defaultValue">Default value is <c>float.NaN</c></param>
    /// <returns>Real float value, otherwise defaultValue</returns>
    public static float ConvertToFloat(this string value, float defaultValue = float.NaN)
    {
      return !float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) ? defaultValue : result;
    }

    /// <summary>
    /// Converts a string to decimal
    /// </summary>
    /// <param name="value">String to convert</param>
    /// <param name="defaultValue">Default value is <c>decimal.MinValue</c></param>
    /// <returns>Real decimal value, otherwise defaultValue</returns>
    public static decimal ConvertToDecimal(this string value, decimal defaultValue = decimal.MinValue)
    {
      return !decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) ? defaultValue : result;
    }
  }
}
