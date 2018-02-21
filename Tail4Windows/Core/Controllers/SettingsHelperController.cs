using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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
    public async Task ReadSettingsAsync() => await ReadSettingsAsync(new CancellationTokenSource(TimeSpan.FromMinutes(5))).ConfigureAwait(false);

    /// <summary>
    /// Reads current settings
    /// </summary>
    /// <param name="cts"><see cref="CancellationTokenSource"/></param>
    /// <returns>Task</returns>
    public async Task ReadSettingsAsync(CancellationTokenSource cts)
    {
      await AddPropertiesIfNotExistsAsync(cts).ConfigureAwait(false);
      await Task.Run(() => ReadSettings(), cts.Token).ConfigureAwait(false);
    }

    private async Task AddPropertiesIfNotExistsAsync(CancellationTokenSource cts)
    {
      var settings = new Dictionary<string, string>
      {
        { "Language", DefaultEnvironmentSettings.Language.ToString() },
        { "StatusBarInactiveBackgroundColor", DefaultEnvironmentSettings.StatusBarInactiveBackgroundColor },
        { "StatusBarFileLoadedBackgroundColor", DefaultEnvironmentSettings.StatusBarFileLoadedBackgroundColor },
        { "StatusBarTailBackgroundColor", DefaultEnvironmentSettings.StatusBarTailBackgroundColor }
      };

      await AddNewPropertyAsync(settings, cts).ConfigureAwait(false);
    }

    private void ReadSettings()
    {
      LOG.Trace("Read T4W settings");

      try
      {
        ReadWindowSettings();
        ReadStatusBarSettings();
        ReadProxySettings();
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
    /// <param name="cts"><see cref="CancellationTokenSource"/></param>
    /// <returns>Task</returns>
    public async Task SaveSettingsAsync(CancellationTokenSource cts) => await Task.Run(() => SaveSettings(), cts.Token).ConfigureAwait(false);

    private void SaveSettings()
    {
      LOG.Trace("Save T4W settings");

      try
      {
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        if ( config.AppSettings.Settings.Count <= 0 )
          return;

        SaveWindowSettings(config);
        SaveStatusBarSettings(config);
        SaveProxySettings(config);

        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("appSettings");
      }
      catch ( ConfigurationErrorsException ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", ex.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
      }
    }

    private void SaveWindowSettings(Configuration config)
    {
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
      WriteValueToSetting(config, "Language", CurrentSettings.Language);
      WriteValueToSetting(config, "AlwaysScrollToEnd", CurrentSettings.AlwaysScrollToEnd);
      WriteValueToSetting(config, "ShowNLineAtStart", CurrentSettings.ShowNumberLineAtStart);
      WriteValueToSetting(config, "ShowLineNumbers", CurrentSettings.ShowLineNumbers);
      WriteValueToSetting(config, "LinesRead", CurrentSettings.LinesRead);
      WriteValueToSetting(config, "GroupByCategory", CurrentSettings.GroupByCategory);
      WriteValueToSetting(config, "AutoUpdate", CurrentSettings.AutoUpdate);
      WriteValueToSetting(config, "DefaultRefreshRate", CurrentSettings.DefaultRefreshRate.ToString());
      WriteValueToSetting(config, "DefaultThreadPriority", CurrentSettings.DefaultThreadPriority.ToString());
      WriteValueToSetting(config, "CurrentWindowStyle", CurrentSettings.CurrentWindowStyle.ToString());
      WriteValueToSetting(config, "TimeFormat", CurrentSettings.DefaultTimeFormat.ToString());
      WriteValueToSetting(config, "DateFormat", CurrentSettings.DefaultDateFormat.ToString());
    }

    private void SaveStatusBarSettings(Configuration config)
    {
      WriteValueToSetting(config, "StatusBarInactiveBackgroundColor", EnvironmentContainer.ConvertMediaBrushToDrawingColor(CurrentSettings.StatusBarInactiveBackgroundColor).ToHexString());
      WriteValueToSetting(config, "StatusBarFileLoadedBackgroundColor", EnvironmentContainer.ConvertMediaBrushToDrawingColor(CurrentSettings.StatusBarFileLoadedBackgroundColor).ToHexString());
      WriteValueToSetting(config, "StatusBarTailBackgroundColor", EnvironmentContainer.ConvertMediaBrushToDrawingColor(CurrentSettings.StatusBarTailBackgroundColor).ToHexString());
    }

    private void SaveProxySettings(Configuration config)
    {

    }

    /// <summary>
    /// Reset current settings
    /// </summary>
    /// <returns>Task</returns>
    public async Task SetDefaultSettingsAsync() => await SetDefaultSettingsAsync(new CancellationTokenSource(TimeSpan.FromMinutes(5)));

    /// <summary>
    /// Reset current settings
    /// </summary>
    /// <param name="cts"><see cref="CancellationTokenSource"/></param>
    /// <returns>Task</returns>
    public async Task SetDefaultSettingsAsync(CancellationTokenSource cts) => await Task.Run(() => SetDefaultSettings(), cts.Token).ConfigureAwait(false);

    private void SetDefaultSettings()
    {
      LOG.Trace("Reset T4W settings");

      SetDefaultWindowSettings();
      SetDefaultStatusBarSettings();
      SetDefaultProxySettings();
    }

    private void SetDefaultWindowSettings()
    {
      CurrentSettings.Language = DefaultEnvironmentSettings.Language;
      CurrentSettings.CurrentWindowStyle = DefaultEnvironmentSettings.CurrentWindowStyle;
      CurrentSettings.AlwaysOnTop = DefaultEnvironmentSettings.AlwaysOnTop;
      CurrentSettings.AlwaysScrollToEnd = DefaultEnvironmentSettings.AlwaysScrollToEnd;
      CurrentSettings.CurrentWindowState = DefaultEnvironmentSettings.CurrentWindowState;
      CurrentSettings.DeleteLogFiles = DefaultEnvironmentSettings.DeleteLogFiles;
      CurrentSettings.ExitWithEscape = DefaultEnvironmentSettings.ExitWithEscape;
      CurrentSettings.LinesRead = DefaultEnvironmentSettings.LinesRead;
      CurrentSettings.RestoreWindowSize = DefaultEnvironmentSettings.RestoreWindowSize;
      CurrentSettings.SaveWindowPosition = DefaultEnvironmentSettings.SaveWindowPosition;
      CurrentSettings.ShowLineNumbers = DefaultEnvironmentSettings.ShowLineNumbers;
      CurrentSettings.ShowNumberLineAtStart = DefaultEnvironmentSettings.ShowNumberLineAtStart;
      CurrentSettings.WindowPositionY = DefaultEnvironmentSettings.WindowPositionY;
      CurrentSettings.WindowPositionX = DefaultEnvironmentSettings.WindowPositionX;
      CurrentSettings.WindowHeight = DefaultEnvironmentSettings.WindowHeight;
      CurrentSettings.WindowWidth = DefaultEnvironmentSettings.WindowWidth;
      CurrentSettings.GroupByCategory = DefaultEnvironmentSettings.GroupByCategory;
      CurrentSettings.AutoUpdate = DefaultEnvironmentSettings.AutoUpdate;
      CurrentSettings.DefaultRefreshRate = DefaultEnvironmentSettings.DefaultRefreshRate;
      CurrentSettings.DefaultThreadPriority = DefaultEnvironmentSettings.DefaultThreadPriority;
      CurrentSettings.DefaultTimeFormat = DefaultEnvironmentSettings.DefaultTimeFormat;
      CurrentSettings.DefaultDateFormat = DefaultEnvironmentSettings.DefaultDateFormat;
    }

    private void SetDefaultStatusBarSettings()
    {
      CurrentSettings.StatusBarInactiveBackgroundColor = EnvironmentContainer.ConvertHexStringToBrush(DefaultEnvironmentSettings.StatusBarInactiveBackgroundColor);
      CurrentSettings.StatusBarFileLoadedBackgroundColor = EnvironmentContainer.ConvertHexStringToBrush(DefaultEnvironmentSettings.StatusBarFileLoadedBackgroundColor);
      CurrentSettings.StatusBarTailBackgroundColor = EnvironmentContainer.ConvertHexStringToBrush(DefaultEnvironmentSettings.StatusBarTailBackgroundColor);
    }

    private void SetDefaultProxySettings()
    {
      CurrentSettings.ProxySettings.Password = DefaultEnvironmentSettings.ProxyPassword;
      CurrentSettings.ProxySettings.UserName = DefaultEnvironmentSettings.ProxyUserName;
      CurrentSettings.ProxySettings.ProxyPort = DefaultEnvironmentSettings.ProxyPort;
      CurrentSettings.ProxySettings.ProxyUrl = DefaultEnvironmentSettings.ProxyUrl;
      CurrentSettings.ProxySettings.UseSystemSettings = DefaultEnvironmentSettings.ProxyUseSystemSettings;
      CurrentSettings.ProxySettings.UseProxy = DefaultEnvironmentSettings.ProxyUseProxy;
    }

    /// <summary>
    /// Reloads current settings
    /// </summary>
    /// <param name="cts"><see cref="CancellationTokenSource"/></param>
    /// <returns>Task</returns>
    public async Task ReloadCurrentSettingsAsync(CancellationTokenSource cts) => await Task.Run(() => ReloadCurrentSettings(), cts.Token).ConfigureAwait(false);

    private void ReloadCurrentSettings()
    {
      LOG.Trace("Reloads T4W settings");
      ConfigurationManager.RefreshSection("appSettings");
    }

    /// <summary>
    /// Adds a new property to config file
    /// </summary>
    /// <param name="settings">List of configuration pair</param>
    /// <param name="cts"><see cref="CancellationTokenSource"/></param>
    /// <returns>Task</returns>
    public async Task AddNewPropertyAsync(Dictionary<string, string> settings, CancellationTokenSource cts) => await Task.Run(() => AddNewProperty(settings), cts.Token).ConfigureAwait(false);

    private void AddNewProperty(Dictionary<string, string> settings)
    {
      if ( settings == null || settings.Count == 0 )
        return;

      LOG.Trace("Add missing config properties");

      try
      {
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        foreach ( var pair in settings )
        {
          if ( config.AppSettings.Settings.AllKeys.Contains(pair.Key) )
            continue;

          config.AppSettings.Settings.Add(pair.Key, pair.Value);
        }

        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("appSettings");
      }
      catch ( ConfigurationErrorsException ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", ex.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
      }
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
      CurrentSettings.Language = GetUiLanguage(GetStringFromSetting("Language"));
      CurrentSettings.AlwaysScrollToEnd = GetBoolFromSetting("AlwaysScrollToEnd");
      CurrentSettings.ShowNumberLineAtStart = GetBoolFromSetting("ShowNLineAtStart");
      CurrentSettings.ShowLineNumbers = GetBoolFromSetting("ShowLineNumbers");
      CurrentSettings.LinesRead = GetIntFromSetting("LinesRead");
      CurrentSettings.GroupByCategory = GetBoolFromSetting("GroupByCategory");
      CurrentSettings.AutoUpdate = GetBoolFromSetting("AutoUpdate");
      CurrentSettings.DefaultRefreshRate = GetRefreshRate(GetStringFromSetting("DefaultRefreshRate"));
      CurrentSettings.DefaultThreadPriority = GetThreadPriority(GetStringFromSetting("DefaultThreadPriority"));
      CurrentSettings.DefaultTimeFormat = ReadTimeFormat(GetStringFromSetting("TimeFormat"));
      CurrentSettings.DefaultDateFormat = ReadDateFormat(GetStringFromSetting("DateFormat"));
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

    private void ReadProxySettings()
    {

    }

    private static string GetStringFromSetting(string setting) => string.IsNullOrWhiteSpace(setting) ? string.Empty : ConfigurationManager.AppSettings[setting];

    private static int GetIntFromSetting(string setting, int defaultValue = -1) => ConfigurationManager.AppSettings[setting].ConverToInt(defaultValue);

    private static double GetDoubleFromSetting(string setting, double defaultValue = -1) => ConfigurationManager.AppSettings[setting].ConvertToDouble(defaultValue);

    private static bool GetBoolFromSetting(string setting, bool defaultValue = false) => ConfigurationManager.AppSettings[setting].ConvertToBool(defaultValue);

    /// <summary>
    /// Get current window state from Enum
    /// </summary>
    /// <param name="s">Enum value as string</param>
    /// <returns>Enum of <see cref="WindowState"/></returns>
    private static WindowState GetWindowState(string s)
    {
      if ( string.IsNullOrEmpty(s) )
        return WindowState.Normal;

      if ( Enum.GetNames(typeof(WindowState)).All(w => string.Compare(s.ToLower(), w.ToLower(), StringComparison.Ordinal) != 0) )
        return WindowState.Normal;

      Enum.TryParse(s, out WindowState wndState);

      return wndState;
    }

    private static void WriteValueToSetting(Configuration config, string setting, object value)
    {
      Arg.NotNull(config, "Configuration");
      config.AppSettings.Settings[setting].Value = value.ToString();
    }

    /// <summary>
    /// Gets current window style from Enum
    /// </summary>
    /// <param name="s">Enum value as string</param>
    /// <returns>Enum of <see cref="EWindowStyle"/></returns>
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
    /// Gets all Enum SmartWatch modes
    /// </summary>
    /// <param name="s">Reference of SmartWatch mode string</param>
    /// <returns>Enum of <see cref="ESmartWatchMode"/></returns>
    private static ESmartWatchMode GetSmartWatchMode(string s)
    {
      if ( string.IsNullOrEmpty(s) )
        return ESmartWatchMode.Manual;

      if ( Enum.GetNames(typeof(ESmartWatchMode)).All(m => string.Compare(s.ToLower(), m.ToLower(), StringComparison.Ordinal) != 0) )
        return ESmartWatchMode.Manual;

      Enum.TryParse(s, out ESmartWatchMode mode);

      return mode;
    }

    /// <summary>
    /// Get all Enum ThreadPriorities
    /// </summary>
    /// <param name="s">Reference of thread priority string</param>
    /// <returns>Enum from thread priority</returns>
    private static ThreadPriority GetThreadPriority(string s)
    {
      if ( s == null )
        return ThreadPriority.Normal;

      if ( Enum.GetNames(typeof(ThreadPriority)).All(priorityName => string.Compare(s.ToLower(), priorityName.ToLower(), StringComparison.Ordinal) != 0) )
        return ThreadPriority.Normal;

      Enum.TryParse(s, out ThreadPriority tp);

      return tp;
    }

    /// <summary>
    /// Get all Enum RefreshRates
    /// </summary>
    /// <param name="s">Reference of refresh rate string</param>
    /// <returns>Enum of ETailRefreshRate</returns>
    private static ETailRefreshRate GetRefreshRate(string s)
    {
      if ( string.IsNullOrEmpty(s) )
        return ETailRefreshRate.Normal;

      if ( Enum.GetNames(typeof(ETailRefreshRate)).All(refreshName => string.Compare(s.ToLower(), refreshName.ToLower(), StringComparison.Ordinal) != 0) )
        return ETailRefreshRate.Normal;

      Enum.TryParse(s, out ETailRefreshRate trr);

      return trr;
    }

    /// <summary>
    /// Gets current language from Enum
    /// </summary>
    /// <param name="s">Enum value as string</param>
    /// <returns>Enum of <see cref="EUiLanguage"/></returns>
    private static EUiLanguage GetUiLanguage(string s)
    {
      if ( string.IsNullOrEmpty(s) )
        return EUiLanguage.English;

      if ( Enum.GetNames(typeof(EUiLanguage)).All(p => string.Compare(s.ToLower(), p.ToLower(), StringComparison.Ordinal) != 0) )
        return EUiLanguage.English;

      Enum.TryParse(s, out EUiLanguage language);

      return language;
    }

    private static EDateFormat ReadDateFormat(string s)
    {
      if ( s == null )
        return EDateFormat.DDMMYYYY;

      foreach ( string dateFormat in Enum.GetNames(typeof(EDateFormat)) )
      {
        if ( string.Compare(s, dateFormat, StringComparison.Ordinal) != 0 )
          return EDateFormat.DDMMYYYY;

        Enum.TryParse(s, out EDateFormat df);
        return df;
      }
      return EDateFormat.DDMMYYYY;
    }

    private static ETimeFormat ReadTimeFormat(string s)
    {
      if ( s == null )
        return ETimeFormat.HHMMD;

      foreach ( string timeFormat in Enum.GetNames(typeof(ETimeFormat)) )
      {
        if ( string.Compare(s, timeFormat, StringComparison.Ordinal) != 0 )
          return ETimeFormat.HHMMD;

        Enum.TryParse(s, out ETimeFormat tf);
        return tf;
      }
      return ETimeFormat.HHMMD;
    }

    #endregion
  }
}
