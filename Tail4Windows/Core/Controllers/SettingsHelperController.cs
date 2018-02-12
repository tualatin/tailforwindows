﻿using System;
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
  public class SettingsHelperController : ISettingsHelper
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(SettingsHelperController));

    /// <summary>
    /// Current T4W settings
    /// </summary>
    public static EnvironmentSettings CurrentSettings
    {
      get;
    } = new EnvironmentSettings();

    /// <summary>
    /// Reads current settings
    /// </summary>
    /// <returns>Task</returns>
    public async Task ReadSettingsAsync()
    {
      LOG.Trace("Read T4W settings");
      await Task.Run(() => ReadSettings()).ConfigureAwait(false);
    }

    private void ReadSettings()
    {
      try
      {
        ReadWindowSettings();
        ReadStatusBarSettings();
      }
      catch ( ConfigurationErrorsException ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", ex.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
      }
    }

    /// <summary>
    /// Writes current settings
    /// </summary>
    /// <returns>Task</returns>
    public async Task SaveSettingsAsync() => await SaveSettingsAsync(new CancellationTokenSource(TimeSpan.FromMinutes(5)));

    /// <summary>
    /// Writes current settings
    /// </summary>
    /// <param name="cts">CancellationTokenSource</param>
    /// <returns>Task</returns>
    public async Task SaveSettingsAsync(CancellationTokenSource cts)
    {
      LOG.Trace("Save T4W settings");
      await Task.Run(() => SaveSettings(), cts.Token).ConfigureAwait(false);
    }

    private void SaveSettings()
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
    }

    /// <summary>
    /// Reset current settings
    /// </summary>
    /// <returns>Task</returns>
    public async Task SetDefaultSettingsAsync() => await SetDefaultSettingsAsync(new CancellationTokenSource(TimeSpan.FromMinutes(5)));

    /// <summary>
    /// Reset current settings
    /// </summary>
    /// <param name="cts">CancellationTokenSource</param>
    /// <returns>Task</returns>
    public async Task SetDefaultSettingsAsync(CancellationTokenSource cts)
    {
      LOG.Trace("Reset T4W settings");
      await Task.Run(() => SetDefaultSettings(), cts.Token).ConfigureAwait(false);
    }

    private void SetDefaultSettings()
    {
    }

    /// <summary>
    /// Reloads current settings
    /// </summary>
    /// <returns>Task</returns>
    public async Task ReloadCurrentSettingsAsync()
    {
      LOG.Trace("Reloads T4W settings");
      await Task.Run(() => ReloadCurrentSettings()).ConfigureAwait(false);
    }

    private void ReloadCurrentSettings()
    {
      ConfigurationManager.RefreshSection("appSettings");
    }

    #region HelperFunctions

    private void ReadWindowSettings()
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
      CurrentSettings.DeleteLogFiles = GetBoolFromSetting("DeleteLogFiles");
      CurrentSettings.CurrentWindowStyle = GetWindowStyle(GetStringFromSetting("CurrentWindowStyle"));
    }

    private void ReadStatusBarSettings()
    {
      CurrentSettings.StatusBarInactiveBackgroundColor = EnvironmentContainer.ConvertHexStringToBrush(GetStringFromSetting("StatusBarInactiveBackgroundColor"),
        EnvironmentContainer.ConvertHexStringToBrush(DefaultEnvironmentSettings.StatusBarInactiveBackgroundColor));
      CurrentSettings.StatusBarInactiveBackgroundColor.Freeze();

      CurrentSettings.StatusBarFileLoadedBackgroundColor = EnvironmentContainer.ConvertHexStringToBrush(GetStringFromSetting("StatusBarFileLoadedBackgroundColor"),
        EnvironmentContainer.ConvertHexStringToBrush(DefaultEnvironmentSettings.StatusBarFileLoadedBackgroundColor));
      CurrentSettings.StatusBarFileLoadedBackgroundColor.Freeze();

      CurrentSettings.StatusBarTailBackgroundColor = EnvironmentContainer.ConvertHexStringToBrush(GetStringFromSetting("StatusBarTailBackgroundColor"),
        EnvironmentContainer.ConvertHexStringToBrush(DefaultEnvironmentSettings.StatusBarTailBackgroundColor));
      CurrentSettings.StatusBarTailBackgroundColor.Freeze();
    }

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
