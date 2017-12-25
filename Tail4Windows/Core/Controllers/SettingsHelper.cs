using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Org.Vs.TailForWin.Core.Data.Settings;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Extensions;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;


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
          catch ( ConfigurationErrorsException ex )
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

            if ( config.AppSettings.Settings.Count <= 0 )
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
          catch ( ConfigurationErrorsException ex )
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

    #region Public helpers

    /// <summary>
    /// Get all Enum RefreshRates
    /// </summary>
    /// <param name="s">Reference of refresh rate string</param>
    /// <returns>Enum of ETailRefreshRate</returns>
    public static ETailRefreshRate GetRefreshRate(string s)
    {
      if ( string.IsNullOrWhiteSpace(s) )
        return ETailRefreshRate.Normal;

      if ( Enum.GetNames(typeof(ETailRefreshRate)).All(refreshName => string.Compare(s.ToLower(), refreshName.ToLower(), StringComparison.Ordinal) != 0) )
        return ETailRefreshRate.Normal;

      Enum.TryParse(s, out ETailRefreshRate trr);

      return trr;
    }

    /// <summary>
    /// Get all Enum ThreadPriorities
    /// </summary>
    /// <param name="s">Reference of thread priority string</param>
    /// <returns>Enum from thread priority</returns>
    public static ThreadPriority GetThreadPriority(string s)
    {
      if ( string.IsNullOrWhiteSpace(s) )
        return ThreadPriority.Normal;

      if ( Enum.GetNames(typeof(ThreadPriority)).All(priorityName => String.Compare(s.ToLower(), priorityName.ToLower(), StringComparison.Ordinal) != 0) )
        return ThreadPriority.Normal;

      Enum.TryParse(s, out ThreadPriority tp);

      return tp;
    }

    #endregion

    #region HelperFunctions

    private static string GetStringFromSetting(string setting)
    {
      return string.IsNullOrWhiteSpace(setting) ? string.Empty : ConfigurationManager.AppSettings[setting];
    }

    private static int GetIntFromSetting(string setting, int defaultValue = -1)
    {
      return ConfigurationManager.AppSettings[setting].ConverToInt(defaultValue);
    }

    private static double GetDoubleFromSetting(string setting, double defaultValue = -1)
    {
      return ConfigurationManager.AppSettings[setting].ConvertToDouble(defaultValue);
    }

    private static bool GetBoolFromSetting(string setting, bool defaultValue = false)
    {
      return ConfigurationManager.AppSettings[setting].ConvertToBool(defaultValue);
    }

    /// <summary>
    /// Get current window state from Enum
    /// </summary>
    /// <param name="s">Enum value as string</param>
    /// <returns>Enum of System.Windows.WindowState</returns>
    private static System.Windows.WindowState GetWindowState(string s)
    {
      if ( string.IsNullOrEmpty(s) )
        return System.Windows.WindowState.Normal;

      if ( Enum.GetNames(typeof(System.Windows.WindowState)).All(w => string.Compare(s.ToLower(), w.ToLower(), StringComparison.Ordinal) != 0) )
        return System.Windows.WindowState.Normal;

      Enum.TryParse(s, out System.Windows.WindowState wndState);

      return wndState;
    }

    private static void WriteValueToSetting(Configuration config, string setting, object value)
    {
      Arg.NotNull(config, "Configuration");
      config.AppSettings.Settings[setting].Value = value.ToString();
    }

    /// <summary>
    /// Get current window style from Enum
    /// </summary>
    /// <param name="s">Enum value as string</param>
    /// <returns>Enum of EWindowStyle</returns>
    private static EWindowStyle GetWindowStyle(string s)
    {
      if ( string.IsNullOrEmpty(s) )
        return EWindowStyle.ModernBlueWindowStyle;

      if ( Enum.GetNames(typeof(EWindowStyle)).All(w => string.Compare(s.ToLower(), w.ToLower(), StringComparison.Ordinal) != 0) )
        return EWindowStyle.ModernBlueWindowStyle;

      Enum.TryParse(s, out EWindowStyle wnd);

      return wnd;
    }

    /// <summary>
    /// Get all Enum SmartWatch modes
    /// </summary>
    /// <param name="s">Reference of SmartWatch mode string</param>
    /// <returns>Enum of ESmartWatchMode</returns>
    private static ESmartWatchMode GetSmartWatchMode(string s)
    {
      if ( string.IsNullOrEmpty(s) )
        return ESmartWatchMode.Manual;

      if ( Enum.GetNames(typeof(ESmartWatchMode)).All(m => string.Compare(s.ToLower(), m.ToLower(), StringComparison.Ordinal) != 0) )
        return ESmartWatchMode.Manual;

      Enum.TryParse(s, out ESmartWatchMode mode);

      return mode;
    }

    #endregion
  }
}
