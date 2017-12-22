using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Interfaces;


namespace Org.Vs.TailForWin.Core.Utils
{
  /// <summary>
  /// Control container for T4W
  /// </summary>
  public class ControlContainer
  {
    private static ControlContainer _instance;

    /// <summary>
    /// Current instance
    /// </summary>
    public static ControlContainer Instance => _instance ?? (_instance = new ControlContainer());

    private readonly ISettingsHelper _settings;

    private ControlContainer()
    {
      _settings = new SettingsHelper();
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
