using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace Org.Vs.TailForWin.Business.Utils
{  /// <summary>
   /// BusinessHelper
   /// </summary>
  public static class BusinessHelper
  {
    private static readonly object MyLock = new object();

    /// <summary>
    /// Create icon of type <see cref="BitmapImage"/>
    /// </summary>
    /// <param name="url">Url</param>
    /// <returns><see cref="BitmapImage"/></returns>
    public static BitmapImage CreateBitmapIcon(string url)
    {
      lock ( MyLock )
      {
        var icon = new BitmapImage();
        icon.BeginInit();
        icon.UriSource = new Uri(url, UriKind.Relative);
        icon.EndInit();

        RenderOptions.SetBitmapScalingMode(icon, BitmapScalingMode.HighQuality);
        RenderOptions.SetEdgeMode(icon, EdgeMode.Aliased);

        return icon;
      }
    }

    /// <summary>
    /// Creates a valid <see cref="Regex"/> pattern
    /// </summary>
    /// <param name="keyWords"><see cref="List{T}"/> of keywords</param>
    /// <returns>Valid <see cref="Regex"/></returns>
    public static Regex GetValidRegexPattern(List<string> keyWords)
    {
      string words = string.Empty;
      keyWords.ForEach(p =>
      {
        if ( !string.IsNullOrWhiteSpace(words) )
          words += "|";

        words += $@"\b{p}\b";
      });
      var regex = new Regex($@"({words})");

      return regex;
    }
  }
}
