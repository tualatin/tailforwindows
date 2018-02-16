﻿namespace Org.Vs.TailForWin.Core.Extensions
{
  /// <summary>
  /// Color extension
  /// </summary>
  public static class ColorExtension
  {
    /// <summary>
    /// Convert color to hex string
    /// </summary>
    /// <param name="color">Color to convert</param>
    /// <returns>Color as hex string</returns>
    public static string ToHexString(this System.Drawing.Color color) => $"#{color.R:X2}{color.G:X2}{color.B:X2}";
  }
}