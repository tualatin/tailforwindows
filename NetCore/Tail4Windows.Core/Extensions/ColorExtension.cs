using System.Drawing;

namespace Org.Vs.TailForWin.Core.Extensions
{
  /// <summary>
  /// Color extension
  /// </summary>
  public static class ColorExtension
  {
    /// <summary>
    /// Converts <see cref="Color"/> to hex string
    /// </summary>
    /// <param name="color">Color to convert</param>
    /// <returns>Color as hex string</returns>
    public static string ToHexString(this Color color) => $"#{color.R:X2}{color.G:X2}{color.B:X2}";

    /// <summary>
    /// Converts <see cref="System.Windows.Media.Color"/> to hex string
    /// </summary>
    /// <param name="color">Color to convert</param>
    /// <returns>Color as hex string</returns>
    public static string ToHexString(this System.Windows.Media.Color color) => $"#{color.R:X2}{color.G:X2}{color.B:X2}";
  }
}
