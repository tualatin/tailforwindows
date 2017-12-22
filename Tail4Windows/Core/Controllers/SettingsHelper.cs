using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.Data.Settings;


namespace Org.Vs.TailForWin.Core.Controllers
{
  /// <inheritdoc />
  /// <summary>
  /// Holds settings from enviroment
  /// </summary>
  public class SettingsHelper : ISettingsHelper
  {
    // ReSharper disable once InconsistentNaming
    private static readonly ILog LOG = LogManager.GetLogger(typeof(SettingsHelper));

    /// <summary>
    /// Current T4W settings
    /// </summary>
    public static EnvironmentSettings CurrentSettings
    {
      get;
    } = new EnvironmentSettings();

    /// <inheritdoc />
    /// <summary>
    /// Reads current settings
    /// </summary>
    public async Task ReadSettingsAsync()
    {
      LOG.Debug("Read T4W settings");
      await ReadConfigurationAsync().ConfigureAwait(false);

    }

    private Task ReadConfigurationAsync()
    {
      return Task.Factory.StartNew(
        () =>
        {
          try
          {
            CurrentSettings.RestoreWindowSize = GetBoolFromSetting("RestoreWindowSize");
            CurrentSettings.AlwaysOnTop = GetBoolFromSetting("AlwaysOnTop");
            CurrentSettings.RestoreWindowSize = GetBoolFromSetting("RestoreWindowSize");
            CurrentSettings.WindowWidth = GetDoubleFromSetting("WndWidth");
            CurrentSettings.WindowHeight = GetDoubleFromSetting("WndHeight");
            CurrentSettings.WindowPositionX = GetDoubleFromSetting("WndXPos");
            CurrentSettings.WindowPositionY = GetDoubleFromSetting("WndYPos");
            CurrentSettings.SaveWindowPosition = GetBoolFromSetting("SaveWindowPosition");
            CurrentSettings.ExitWithEscape = GetBoolFromSetting("ExitWithEsc");
            CurrentSettings.CurrentWindowState = GetWindowState(GetStringFromSetting("WindowState"));
          }
          catch( ConfigurationErrorsException ex )
          {
            LOG.Error(ex, "{0} caused a(n) {1}", ex.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
          }
        });
    }

    /// <inheritdoc />
    /// <summary>
    /// Writes current settings
    /// </summary>
    public async Task SaveSettingsAsync()
    {
      LOG.Debug("Save T4W settings");
      await SaveConfigurationAsync().ConfigureAwait(false);
    }

    private Task SaveConfigurationAsync()
    {
      return Task.Factory.StartNew(
        () =>
        {
          try
          {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if( config.AppSettings.Settings.Count <= 0 )
              return;

            WriteValueToSetting(config, "RestoreWindowSize", CurrentSettings.RestoreWindowSize);
            WriteValueToSetting(config, "AlwaysOnTop", CurrentSettings.AlwaysOnTop);
            WriteValueToSetting(config, "RestoreWindowSize", CurrentSettings.RestoreWindowSize);
            WriteValueToSetting(config, "WndWidth", CurrentSettings.WindowWidth);
            WriteValueToSetting(config, "WndHeight", CurrentSettings.WindowHeight);
            WriteValueToSetting(config, "WndXPos", CurrentSettings.WindowPositionX);
            WriteValueToSetting(config, "WndYPos", CurrentSettings.WindowPositionY);
            WriteValueToSetting(config, "SaveWindowPosition", CurrentSettings.SaveWindowPosition);
            WriteValueToSetting(config, "ExitWithEsc", CurrentSettings.ExitWithEscape);
            WriteValueToSetting(config, "WindowState", CurrentSettings.CurrentWindowState);

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
          }
          catch( ConfigurationErrorsException ex )
          {
            LOG.Error(ex, "{0} caused a(n) {1}", ex.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
          }
        });
    }

    /// <inheritdoc />
    /// <summary>
    /// Reset current settings
    /// </summary>
    public async Task SetDefaultSettingsAsync()
    {
      LOG.Debug("Reset T4W settings");
    }

    /// <inheritdoc />
    /// <summary>
    /// Reloads current settings
    /// </summary>
    public async Task ReloadCurrentSettingsAsync()
    {
      LOG.Debug("Reloads T4W settings");
      await Task.Factory.StartNew(
        () =>
        {
          ConfigurationManager.RefreshSection("appSettings");

        }).ConfigureAwait(false);
    }

    #region HelperFunctions

    private static string GetStringFromSetting(string setting)
    {
      return string.IsNullOrWhiteSpace(setting) ? string.Empty : ConfigurationManager.AppSettings[setting];
    }

    private static int GetIntFromSetting(string setting, int defaultValue = -1)
    {
      if( !int.TryParse(ConfigurationManager.AppSettings[setting], out int intSetting) )
        intSetting = defaultValue;

      return intSetting;
    }

    private static double GetDoubleFromSetting(string setting, double defaultValue = -1)
    {
      if( !double.TryParse(ConfigurationManager.AppSettings[setting], out double doubleSetting) )
        doubleSetting = defaultValue;

      return doubleSetting;
    }

    private static bool GetBoolFromSetting(string setting, bool defaultValue = false)
    {
      if( !bool.TryParse(ConfigurationManager.AppSettings[setting], out bool boolSetting) )
        boolSetting = defaultValue;

      return boolSetting;
    }

    /// <summary>
    /// Get current window state from Enum
    /// </summary>
    /// <param name="s">Enum value as string</param>
    /// <returns>Enum of System.Windows.WindowState</returns>
    private static System.Windows.WindowState GetWindowState(string s)
    {
      if( string.IsNullOrEmpty(s) )
        return System.Windows.WindowState.Normal;

      if( Enum.GetNames(typeof(System.Windows.WindowState)).All(w => string.Compare(s.ToLower(), w.ToLower(), StringComparison.Ordinal) != 0) )
        return System.Windows.WindowState.Normal;

      Enum.TryParse(s, out System.Windows.WindowState wndState);

      return wndState;
    }

    private static void WriteValueToSetting(Configuration config, string setting, object value)
    {
      Arg.NotNull(config, "Configuration");
      config.AppSettings.Settings[setting].Value = value.ToString();
    }

    #endregion
  }
}
