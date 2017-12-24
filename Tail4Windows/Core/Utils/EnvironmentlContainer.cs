using System.Threading.Tasks;
using System.Windows;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Interfaces;


namespace Org.Vs.TailForWin.Core.Utils
{
  /// <summary>
  /// Environment container for T4W
  /// </summary>
  public class EnvironmentlContainer
  {
    private static EnvironmentlContainer instance;

    /// <summary>
    /// Current instance
    /// </summary>
    public static EnvironmentlContainer Instance => instance ?? (instance = new EnvironmentlContainer());

    private readonly ISettingsHelper _settings;
    private readonly IXmlReader _xmlReader;


    private EnvironmentlContainer()
    {
      _settings = new SettingsHelper();
      _xmlReader = new XmlConfigReadController();
    }

    /// <summary>
    /// Application title
    /// </summary>
    public string ApplicationTitle => Application.Current.TryFindResource("ApplicationTitle").ToString();

    /// <summary>
    /// Read current settings
    /// </summary>
    async public Task ReadSettings()
    {
      await _settings.ReadSettingsAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Save current settings
    /// </summary>
    /// <returns></returns>
    async public Task SaveSettings()
    {
      await _settings.SaveSettingsAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Reload settings from config file
    /// </summary>
    /// <returns></returns>
    async public Task ReloadSettings()
    {
      await _settings.ReloadCurrentSettingsAsync().ConfigureAwait(false);
    }
  }
}
