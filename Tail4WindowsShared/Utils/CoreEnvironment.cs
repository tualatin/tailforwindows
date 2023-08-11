using System.IO;
using System.Windows;
using Org.Vs.Tail4Win.Shared.Controllers;

namespace Org.Vs.Tail4Win.Shared.Utils
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
    /// Current application path
    /// </summary>
    public static string ApplicationPath => Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()?.Location);

    /// <summary>
    /// TailStore path
    /// </summary>
    public static string UserSettingsPath => SettingsHelperController.CurrentAppSettings.IsPortable
      ? ApplicationPath + @"\Settings" : Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $@"\{ApplicationTitle}";

    /// <summary>
    /// Application settings file
    /// </summary>
    public static string ApplicationSettingsFile => UserSettingsPath + $@"\{ApplicationTitle}.config";

    /// <summary>
    /// Application Regex compare URL
    /// </summary>
    public static string ApplicationRegexWebUrl => "https://www.virtual-studios.de";

#if DEBUG
    /// <summary>
    /// Application Update URL
    /// </summary>
    public static string ApplicationUpdateWebUrl => "https://www.virtual-studios.de/tail4wnd/releases_debug.txt";
#else
    /// <summary>
    /// Application Update URL
    /// </summary>
    public static string ApplicationUpdateWebUrl => "https://www.virtual-studios.de/tail4wnd/releases.txt";
#endif

    /// <summary>
    /// Application donate web URL
    /// </summary>
    public static string ApplicationDonateWebUrl => "https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=M436BDAMQL7WE";
  }
}
