using System.Drawing;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using log4net;
using Org.Vs.Tail4Win.Core.Utils;

namespace Org.Vs.Tail4Win.Business.Utils
{  /// <summary>
   /// BusinessHelper
   /// </summary>
  public static class BusinessHelper
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(BusinessHelper));
    private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

    /// <summary>
    /// Create icon of type <see cref="BitmapImage"/>
    /// </summary>
    /// <param name="url">Url</param>
    /// <returns><see cref="BitmapImage"/></returns>
    /// <exception cref="ArgumentException">If url is null or empty</exception>
    public static BitmapImage CreateBitmapIcon(string url)
    {
      Arg.NotNull(url, nameof(url));
      LOG.Debug($"CreateBitmapIcon bitmap URL: {url}");
      Semaphore.Wait();

      try
      {
        var icon = new BitmapImage();
        icon.BeginInit();
        icon.UriSource = new Uri(url, UriKind.Relative);
        icon.EndInit();

        RenderOptions.SetBitmapScalingMode(icon, BitmapScalingMode.HighQuality);
        RenderOptions.SetEdgeMode(icon, EdgeMode.Aliased);

        return icon;
      }
      finally
      {
        Semaphore.Release();
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

    /// <summary>
    /// Get <see cref="Icon"/> from <see cref="Assembly"/>
    /// </summary>
    /// <param name="path">Path of assembly</param>
    /// <returns><see cref="Icon"/></returns>
    /// <exception cref="ArgumentException">If path is null or empty</exception>
    public static BitmapSource GetAssemblyIcon(string path)
    {
      Arg.NotNull(path, nameof(path));
      Semaphore.Wait();

      try
      {
        Icon appIcon = Icon.ExtractAssociatedIcon(path);

        if ( appIcon == null )
          return CreateBitmapIcon("/T4W;component/Resources/notepad.ico");

        BitmapSource bitmapSrc = Imaging.CreateBitmapSourceFromHIcon(appIcon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

        return bitmapSrc;
      }
      finally
      {
        Semaphore.Release();
      }
    }
  }
}
