using System.Windows;


namespace Org.Vs.TailForWin.Core.Utils
{
  /// <summary>
  /// CoreEnvironment
  /// </summary>
  public class CoreEnvironment
  {
    /// <summary>
    /// Application title
    /// </summary>
    public static string ApplicationTitle => Application.Current.TryFindResource("ApplicationTitle").ToString();

    /// <summary>
    /// Application Regex compare URL
    /// </summary>
    public static string ApplicationRegexWebUrl => "https://www.virtual-studios.de";

    /// <summary>
    /// Application Update URL
    /// </summary>
    public static string ApplicationUpdateWebUrl => "https://www.virtual-studios.de/tail4wnd/releases.txt";

    /// <summary>
    /// Application donate web URL
    /// </summary>
    public static string ApplicationDonateWebUrl => "https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=M436BDAMQL7WE";
  }
}
