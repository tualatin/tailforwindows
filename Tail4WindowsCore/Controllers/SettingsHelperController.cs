using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
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

    private static readonly object MyLock = new object();

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
      await RemovePropertiesIfExistsAsync(cts).ConfigureAwait(false);
      await AddPropertiesIfNotExistsAsync(cts).ConfigureAwait(false);
      await Task.Run(() => ReadSettings(), cts.Token).ConfigureAwait(false);
    }

    private async Task AddPropertiesIfNotExistsAsync(CancellationTokenSource cts)
    {
      var settings = new Dictionary<string, string>
      {
        { "LastViewedOptionPage", Guid.Empty.ToString() },
        { "Language", DefaultEnvironmentSettings.Language.ToString() },
        { "DeleteLogFileOlderThan", DefaultEnvironmentSettings.DeleteLogFilesOlderThan.ToString() },
        { "StatusBarInactiveBackgroundColor", DefaultEnvironmentSettings.StatusBarInactiveBackgroundColor },
        { "StatusBarFileLoadedBackgroundColor", DefaultEnvironmentSettings.StatusBarFileLoadedBackgroundColor },
        { "StatusBarTailBackgroundColor", DefaultEnvironmentSettings.StatusBarTailBackgroundColor },
        { "DragDropWindow", DefaultEnvironmentSettings.ActivateDragDropWindow.ToString() },
        { "SaveLogFileHistory", DefaultEnvironmentSettings.SaveLogFileHistory.ToString() },
        { "LogFileHistorySize", DefaultEnvironmentSettings.HistoryMaxSize.ToString() },
        { "ShowExtendedSettings", DefaultEnvironmentSettings.ShowExtendedSettings.ToString() },
        { "SplitterBackgroundColor", DefaultEnvironmentSettings.SplitterBackgroundColor },
        { "SplitterWindowBehavior", DefaultEnvironmentSettings.SplitterWindowBehavior.ToString() }
      };

      await AddNewPropertyAsync(settings, cts).ConfigureAwait(false);
    }

    private async Task RemovePropertiesIfExistsAsync(CancellationTokenSource cts)
    {
      var settings = new List<string>
      {
        "Proxy.Use",
        "InactiveBackgroundColor",
        "InactiveForegroundColor"
      };

      await RemoveObsoletePropertiesAsync(settings, cts.Token).ConfigureAwait(false);
    }

    private async Task RemoveObsoletePropertiesAsync(List<string> obsoleteSettings, CancellationToken token)
    {
      await Task.Run(
        () =>
        {
          lock ( MyLock )
          {
            LOG.Trace("Remove obsolete properties from config file");

            try
            {
              var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

              foreach ( string key in obsoleteSettings )
              {
                if ( !config.AppSettings.Settings.AllKeys.Contains(key) )
                  continue;

                config.AppSettings.Settings.Remove(key);
              }

              config.Save(ConfigurationSaveMode.Modified);
              ConfigurationManager.RefreshSection("appSettings");
            }
            catch ( ConfigurationErrorsException ex )
            {
              LOG.Error(ex, "{0} caused a(n) {1}", ex.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
          }
        },
        token).ConfigureAwait(false);
    }

    private void ReadSettings()
    {
      lock ( MyLock )
      {
        try
        {
          LOG.Trace("Read T4W settings");

          ReadWindowSettings();
          ReadStatusBarSettings();
          ReadProxySettings();
          ReadLogViewerSettings();
          ReadAlertSettings();
          ReadSmtpSettings();
          ReadSmartWatchSettings();
        }
        catch ( ConfigurationErrorsException ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", ex.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
        }
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
      lock ( MyLock )
      {
        try
        {
          LOG.Trace("Save T4W settings");

          var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

          if ( config.AppSettings.Settings.Count <= 0 )
            return;

          SaveWindowSettings(config);
          SaveStatusBarSettings(config);
          SaveProxySettings(config);
          SaveLogViewerSettings(config);
          SaveAlertSettings(config);
          SaveSmtpSettings(config);
          SaveSmartWatchSettings(config);

          config.Save(ConfigurationSaveMode.Modified);
          ConfigurationManager.RefreshSection("appSettings");
        }
        catch ( ConfigurationErrorsException ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", ex.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
        }
      }
    }

    private void SaveWindowSettings(Configuration config)
    {
      WriteValueToSetting(config, "LastViewedOptionPage", CurrentSettings.LastViewedOptionPage);
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
      WriteValueToSetting(config, "DefaultRefreshRate", CurrentSettings.DefaultRefreshRate);
      WriteValueToSetting(config, "DefaultThreadPriority", CurrentSettings.DefaultThreadPriority);
      WriteValueToSetting(config, "CurrentWindowStyle", CurrentSettings.CurrentWindowStyle);
      WriteValueToSetting(config, "TimeFormat", CurrentSettings.DefaultTimeFormat);
      WriteValueToSetting(config, "DateFormat", CurrentSettings.DefaultDateFormat);
      WriteValueToSetting(config, "FileManagerSort", CurrentSettings.DefaultFileSort);
      WriteValueToSetting(config, "LogLineLimit", CurrentSettings.LogLineLimit);
      WriteValueToSetting(config, "SmartWatch", CurrentSettings.SmartWatch);
      WriteValueToSetting(config, "Statics", CurrentSettings.Statistics);
      WriteValueToSetting(config, "DeleteLogFileOlderThan", CurrentSettings.LogFilesOlderThan);
      WriteValueToSetting(config, "DeleteLogFiles", CurrentSettings.DeleteLogFiles);
      WriteValueToSetting(config, "DragDropWindow", CurrentSettings.ActivateDragDropWindow);
      WriteValueToSetting(config, "SaveLogFileHistory", CurrentSettings.SaveLogFileHistory);
      WriteValueToSetting(config, "LogFileHistorySize", CurrentSettings.HistoryMaxSize);
      WriteValueToSetting(config, "ShowExtendedSettings", CurrentSettings.ShowExtendedSettings);
      WriteValueToSetting(config, "SplitterWindowBehavior", CurrentSettings.SplitterWindowBehavior);
    }

    private void SaveStatusBarSettings(Configuration config)
    {
      WriteValueToSetting(config, "StatusBarInactiveBackgroundColor", CurrentSettings.ColorSettings.StatusBarInactiveBackgroundColorHex);
      WriteValueToSetting(config, "StatusBarFileLoadedBackgroundColor", CurrentSettings.ColorSettings.StatusBarFileLoadedBackgroundColorHex);
      WriteValueToSetting(config, "StatusBarTailBackgroundColor", CurrentSettings.ColorSettings.StatusBarTailBackgroundColorHex);
    }

    private void SaveLogViewerSettings(Configuration config)
    {
      WriteValueToSetting(config, "ForegroundColor", CurrentSettings.ColorSettings.ForegroundColorHex);
      WriteValueToSetting(config, "BackgroundColor", CurrentSettings.ColorSettings.BackgroundColorHex);
      WriteValueToSetting(config, "FindHighlightForegroundColor", CurrentSettings.ColorSettings.FindHighlightForegroundColorHex);
      WriteValueToSetting(config, "FindHighlightBackgroundColor", CurrentSettings.ColorSettings.FindHighlightBackgroundColorHex);
      WriteValueToSetting(config, "LineNumbersColor", CurrentSettings.ColorSettings.LineNumberColorHex);
      WriteValueToSetting(config, "HighlightColor", CurrentSettings.ColorSettings.LineNumberHighlightColorHex);
      WriteValueToSetting(config, "SplitterBackgroundColor", CurrentSettings.ColorSettings.SplitterBackgroundColorHex);
    }

    private void SaveAlertSettings(Configuration config)
    {
      WriteValueToSetting(config, "Alert.BringToFront", CurrentSettings.AlertSettings.BringToFront);
      WriteValueToSetting(config, "Alert.EMailAddress", CurrentSettings.AlertSettings.MailAddress);
      WriteValueToSetting(config, "Alert.SendEMail", CurrentSettings.AlertSettings.SendMail);
      WriteValueToSetting(config, "Alert.PlaySoundFile", CurrentSettings.AlertSettings.PlaySoundFile);
      WriteValueToSetting(config, "Alert.PopupWindow", CurrentSettings.AlertSettings.PopupWnd);
      WriteValueToSetting(config, "Alert.SoundFile", CurrentSettings.AlertSettings.SoundFileNameFullPath);
    }

    private void SaveSmtpSettings(Configuration config)
    {
      WriteValueToSetting(config, "Smtp.Ssl", CurrentSettings.SmtpSettings.Ssl);
      WriteValueToSetting(config, "Smtp.Tls", CurrentSettings.SmtpSettings.Tls);
      WriteValueToSetting(config, "Smtp.FromEMail", CurrentSettings.SmtpSettings.FromAddress);
      WriteValueToSetting(config, "Smtp.Subject", CurrentSettings.SmtpSettings.Subject);
      WriteValueToSetting(config, "Smtp.Login", CurrentSettings.SmtpSettings.LoginName);
      WriteValueToSetting(config, "Smtp.Password", CurrentSettings.SmtpSettings.Password);
      WriteValueToSetting(config, "Smtp.Port", CurrentSettings.SmtpSettings.SmtpPort);
      WriteValueToSetting(config, "Smtp.Server", CurrentSettings.SmtpSettings.SmtpServerName);
    }

    private void SaveProxySettings(Configuration config)
    {
      WriteValueToSetting(config, "Proxy.UserName", CurrentSettings.ProxySettings.UserName);
      WriteValueToSetting(config, "Proxy.Password", CurrentSettings.ProxySettings.Password);
      WriteValueToSetting(config, "Proxy.Port", CurrentSettings.ProxySettings.ProxyPort.ToString(CultureInfo.InvariantCulture));
      WriteValueToSetting(config, "Proxy.Url", CurrentSettings.ProxySettings.ProxyUrl);
      WriteValueToSetting(config, "Proxy.UseSystem", CurrentSettings.ProxySettings.UseSystemSettings.ToString());
    }

    private void SaveSmartWatchSettings(Configuration config)
    {
      WriteValueToSetting(config, "SmartWatch.AutoRun", CurrentSettings.SmartWatchSettings.AutoRun);
      WriteValueToSetting(config, "SmartWatch.FilterByExtension", CurrentSettings.SmartWatchSettings.FilterByExtension);
      WriteValueToSetting(config, "SmartWatch.Mode", CurrentSettings.SmartWatchSettings.Mode);
      WriteValueToSetting(config, "SmartWatch.NewTab", CurrentSettings.SmartWatchSettings.NewTab);
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
      lock ( MyLock )
      {
        LOG.Trace("Reset T4W settings");

        SetDefaultWindowSettings();
        SetDefaultStatusBarSettings();
        SetDefaultProxySettings();
        SetDefaultLogViewerSettings();
        SetDefaultAlertSettings();
        SetDefaultSmptSettings();
        SetDefaultSmartWatchSettings();
      }
    }

    private void SetDefaultWindowSettings()
    {
      CurrentSettings.LastViewedOptionPage = Guid.Empty;
      CurrentSettings.Language = DefaultEnvironmentSettings.Language;
      CurrentSettings.CurrentWindowStyle = DefaultEnvironmentSettings.CurrentWindowStyle;
      CurrentSettings.AlwaysOnTop = DefaultEnvironmentSettings.AlwaysOnTop;
      CurrentSettings.AlwaysScrollToEnd = DefaultEnvironmentSettings.AlwaysScrollToEnd;
      CurrentSettings.CurrentWindowState = DefaultEnvironmentSettings.CurrentWindowState;
      CurrentSettings.DeleteLogFiles = DefaultEnvironmentSettings.DeleteLogFiles;
      CurrentSettings.LogFilesOlderThan = DefaultEnvironmentSettings.DeleteLogFilesOlderThan;
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
      CurrentSettings.DefaultFileSort = DefaultEnvironmentSettings.DefaultFileSort;
      CurrentSettings.LogLineLimit = DefaultEnvironmentSettings.LogLineLimit;
      CurrentSettings.SmartWatch = DefaultEnvironmentSettings.SmartWatch;
      CurrentSettings.Statistics = DefaultEnvironmentSettings.Statistics;
      CurrentSettings.ActivateDragDropWindow = DefaultEnvironmentSettings.ActivateDragDropWindow;
      CurrentSettings.SaveLogFileHistory = DefaultEnvironmentSettings.SaveLogFileHistory;
      CurrentSettings.HistoryMaxSize = DefaultEnvironmentSettings.HistoryMaxSize;
      CurrentSettings.ShowExtendedSettings = DefaultEnvironmentSettings.ShowExtendedSettings;
      CurrentSettings.SplitterWindowBehavior = DefaultEnvironmentSettings.SplitterWindowBehavior;
    }

    private void SetDefaultStatusBarSettings()
    {
      CurrentSettings.ColorSettings.StatusBarInactiveBackgroundColorHex = DefaultEnvironmentSettings.StatusBarInactiveBackgroundColor;
      CurrentSettings.ColorSettings.StatusBarFileLoadedBackgroundColorHex = DefaultEnvironmentSettings.StatusBarFileLoadedBackgroundColor;
      CurrentSettings.ColorSettings.StatusBarTailBackgroundColorHex = DefaultEnvironmentSettings.StatusBarTailBackgroundColor;
    }

    private void SetDefaultLogViewerSettings()
    {
      CurrentSettings.ColorSettings.ForegroundColorHex = DefaultEnvironmentSettings.ForegroundColor;
      CurrentSettings.ColorSettings.BackgroundColorHex = DefaultEnvironmentSettings.BackgroundColor;
      CurrentSettings.ColorSettings.FindHighlightForegroundColorHex = DefaultEnvironmentSettings.HighlightForegroundColor;
      CurrentSettings.ColorSettings.FindHighlightBackgroundColorHex = DefaultEnvironmentSettings.HighlightBackgroundColor;
      CurrentSettings.ColorSettings.LineNumberColorHex = DefaultEnvironmentSettings.LineNumberColor;
      CurrentSettings.ColorSettings.LineNumberHighlightColorHex = DefaultEnvironmentSettings.HighlightLineNumberColor;
      CurrentSettings.ColorSettings.SplitterBackgroundColorHex = DefaultEnvironmentSettings.SplitterBackgroundColor;
    }

    private void SetDefaultAlertSettings()
    {
      CurrentSettings.AlertSettings.BringToFront = DefaultEnvironmentSettings.AlertBringToFront;
      CurrentSettings.AlertSettings.PlaySoundFile = DefaultEnvironmentSettings.AlertPlaySoundFile;
      CurrentSettings.AlertSettings.SendMail = DefaultEnvironmentSettings.AlertSendMail;
      CurrentSettings.AlertSettings.PopupWnd = DefaultEnvironmentSettings.AlertPopUpWindow;
      CurrentSettings.AlertSettings.MailAddress = DefaultEnvironmentSettings.AlertMailAddress;
      CurrentSettings.AlertSettings.SoundFileNameFullPath = DefaultEnvironmentSettings.AlertSoundFile;
    }

    private void SetDefaultSmptSettings()
    {
      CurrentSettings.SmtpSettings.Ssl = DefaultEnvironmentSettings.SmtpSsl;
      CurrentSettings.SmtpSettings.Tls = DefaultEnvironmentSettings.SmtpTls;
      CurrentSettings.SmtpSettings.SmtpPort = DefaultEnvironmentSettings.SmtpPort;
      CurrentSettings.SmtpSettings.SmtpServerName = DefaultEnvironmentSettings.SmtpServer;
      CurrentSettings.SmtpSettings.LoginName = DefaultEnvironmentSettings.SmtpUsername;
      CurrentSettings.SmtpSettings.Password = DefaultEnvironmentSettings.SmtpPassword;
      CurrentSettings.SmtpSettings.FromAddress = DefaultEnvironmentSettings.SmtpFromMailAddress;
      CurrentSettings.SmtpSettings.Subject = DefaultEnvironmentSettings.SmtpSubject;
    }

    private void SetDefaultProxySettings()
    {
      CurrentSettings.ProxySettings.Password = DefaultEnvironmentSettings.ProxyPassword;
      CurrentSettings.ProxySettings.UserName = DefaultEnvironmentSettings.ProxyUserName;
      CurrentSettings.ProxySettings.ProxyPort = DefaultEnvironmentSettings.ProxyPort;
      CurrentSettings.ProxySettings.ProxyUrl = DefaultEnvironmentSettings.ProxyUrl;
      CurrentSettings.ProxySettings.UseSystemSettings = DefaultEnvironmentSettings.ProxyUseSystemSettings;
    }

    private void SetDefaultSmartWatchSettings()
    {
      CurrentSettings.SmartWatchSettings.AutoRun = DefaultEnvironmentSettings.SmartWatchAutoRun;
      CurrentSettings.SmartWatchSettings.NewTab = DefaultEnvironmentSettings.SmartWatchNewTab;
      CurrentSettings.SmartWatchSettings.Mode = DefaultEnvironmentSettings.SmartWatchMode;
      CurrentSettings.SmartWatchSettings.FilterByExtension = DefaultEnvironmentSettings.SmartWatchFilterByExension;
    }

    /// <summary>
    /// Reloads current settings
    /// </summary>
    /// <param name="cts"><see cref="CancellationTokenSource"/></param>
    /// <returns>Task</returns>
    public async Task ReloadCurrentSettingsAsync(CancellationTokenSource cts) => await Task.Run(() => ReloadCurrentSettings(), cts.Token).ConfigureAwait(false);

    private void ReloadCurrentSettings()
    {
      lock ( MyLock )
      {
        LOG.Trace("Reloads T4W settings");
        ConfigurationManager.RefreshSection("appSettings");
      }
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

      lock ( MyLock )
      {
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
    }

    #region HelperFunctions

    private void ReadWindowSettings()
    {
      CurrentSettings.LastViewedOptionPage = Guid.Parse(GetStringFromSetting("LastViewedOptionPage"));
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
      CurrentSettings.LogFilesOlderThan = GetIntFromSetting("DeleteLogFileOlderThan");
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
      CurrentSettings.DefaultFileSort = ReadFileSortFormat(GetStringFromSetting("FileManagerSort"));
      CurrentSettings.LogLineLimit = GetIntFromSetting("LogLineLimit");
      CurrentSettings.Statistics = GetBoolFromSetting("Statics");
      CurrentSettings.ActivateDragDropWindow = GetBoolFromSetting("DragDropWindow");
      CurrentSettings.SaveLogFileHistory = GetBoolFromSetting("SaveLogFileHistory");
      CurrentSettings.HistoryMaxSize = GetIntFromSetting("LogFileHistorySize");
      CurrentSettings.ShowExtendedSettings = GetBoolFromSetting("ShowExtendedSettings");
      CurrentSettings.SplitterWindowBehavior = GetBoolFromSetting("SplitterWindowBehavior");
    }

    private void ReadStatusBarSettings()
    {
      CurrentSettings.ColorSettings.StatusBarInactiveBackgroundColorHex = GetStringFromSetting("StatusBarInactiveBackgroundColor");
      CurrentSettings.ColorSettings.StatusBarFileLoadedBackgroundColorHex = GetStringFromSetting("StatusBarFileLoadedBackgroundColor");
      CurrentSettings.ColorSettings.StatusBarTailBackgroundColorHex = GetStringFromSetting("StatusBarTailBackgroundColor");
    }

    private void ReadLogViewerSettings()
    {
      CurrentSettings.ColorSettings.ForegroundColorHex = GetStringFromSetting("ForegroundColor");
      CurrentSettings.ColorSettings.BackgroundColorHex = GetStringFromSetting("BackgroundColor");
      CurrentSettings.ColorSettings.FindHighlightForegroundColorHex = GetStringFromSetting("FindHighlightForegroundColor");
      CurrentSettings.ColorSettings.FindHighlightBackgroundColorHex = GetStringFromSetting("FindHighlightBackgroundColor");
      CurrentSettings.ColorSettings.LineNumberColorHex = GetStringFromSetting("LineNumbersColor");
      CurrentSettings.ColorSettings.LineNumberHighlightColorHex = GetStringFromSetting("HighlightColor");
      CurrentSettings.ColorSettings.SplitterBackgroundColorHex = GetStringFromSetting("SplitterBackgroundColor");
    }

    private void ReadAlertSettings()
    {
      CurrentSettings.AlertSettings.BringToFront = GetBoolFromSetting("Alert.BringToFront", true);
      CurrentSettings.AlertSettings.MailAddress = GetStringFromSetting("Alert.EMailAddress");
      CurrentSettings.AlertSettings.PlaySoundFile = GetBoolFromSetting("Alert.PlaySoundFile");
      CurrentSettings.AlertSettings.PopupWnd = GetBoolFromSetting("Alert.PopupWindow");
      CurrentSettings.AlertSettings.SendMail = GetBoolFromSetting("Alert.SendEMail");
      CurrentSettings.AlertSettings.SoundFileNameFullPath = GetStringFromSetting("Alert.SoundFile");
    }

    private void ReadSmtpSettings()
    {
      CurrentSettings.SmtpSettings.Ssl = GetBoolFromSetting("Smtp.Ssl", true);
      CurrentSettings.SmtpSettings.Tls = GetBoolFromSetting("Smtp.Tls");
      CurrentSettings.SmtpSettings.FromAddress = GetStringFromSetting("Smtp.FromEMail");
      CurrentSettings.SmtpSettings.LoginName = GetStringFromSetting("Smtp.Login");
      CurrentSettings.SmtpSettings.Password = GetStringFromSetting("Smtp.Password");
      CurrentSettings.SmtpSettings.SmtpPort = GetIntFromSetting("Smtp.Port");
      CurrentSettings.SmtpSettings.SmtpServerName = GetStringFromSetting("Smtp.Server");
      CurrentSettings.SmtpSettings.Subject = GetStringFromSetting("Smtp.Subject");
    }

    private void ReadProxySettings()
    {
      CurrentSettings.ProxySettings.UseSystemSettings = GetThreeStateBoolFromSetting("Proxy.UseSystem");
      CurrentSettings.ProxySettings.ProxyPort = GetIntFromSetting("Proxy.Port");
      CurrentSettings.ProxySettings.ProxyUrl = GetStringFromSetting("Proxy.Url");
      CurrentSettings.ProxySettings.UserName = GetStringFromSetting("Proxy.UserName");
      CurrentSettings.ProxySettings.Password = GetStringFromSetting("Proxy.Password");
    }

    private void ReadSmartWatchSettings()
    {
      CurrentSettings.SmartWatchSettings.AutoRun = GetBoolFromSetting("SmartWatch.AutoRun", true);
      CurrentSettings.SmartWatchSettings.FilterByExtension = GetBoolFromSetting("SmartWatch.FilterByExtension", true);
      CurrentSettings.SmartWatchSettings.NewTab = GetBoolFromSetting("SmartWatch.NewTab", true);
      CurrentSettings.SmartWatchSettings.Mode = GetSmartWatchMode(GetStringFromSetting("SmartWatch.Mode"));
    }

    private static string GetStringFromSetting(string setting) => string.IsNullOrWhiteSpace(setting) ? string.Empty : ConfigurationManager.AppSettings[setting];

    private static int GetIntFromSetting(string setting, int defaultValue = -1) => ConfigurationManager.AppSettings[setting].ConverToInt(defaultValue);

    private static double GetDoubleFromSetting(string setting, double defaultValue = -1) => ConfigurationManager.AppSettings[setting].ConvertToDouble(defaultValue);

    private static bool GetBoolFromSetting(string setting, bool defaultValue = false) => ConfigurationManager.AppSettings[setting].ConvertToBool(defaultValue);

    private static bool? GetThreeStateBoolFromSetting(string setting) => ConfigurationManager.AppSettings[setting].ConvertToThreeStateBool();

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
      if ( string.IsNullOrEmpty(s) )
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
      if ( string.IsNullOrEmpty(s) )
        return EDateFormat.DDMMYYYY;

      foreach ( string dateFormat in Enum.GetNames(typeof(EDateFormat)) )
      {
        if ( string.Compare(s, dateFormat, StringComparison.Ordinal) != 0 )
          continue;

        Enum.TryParse(s, out EDateFormat df);
        return df;
      }
      return EDateFormat.DDMMYYYY;
    }

    private static ETimeFormat ReadTimeFormat(string s)
    {
      if ( string.IsNullOrEmpty(s) )
        return ETimeFormat.HHMMD;

      foreach ( string timeFormat in Enum.GetNames(typeof(ETimeFormat)) )
      {
        if ( string.Compare(s, timeFormat, StringComparison.Ordinal) != 0 )
          continue;

        Enum.TryParse(s, out ETimeFormat tf);
        return tf;
      }
      return ETimeFormat.HHMMD;
    }

    private static EFileSort ReadFileSortFormat(string s)
    {
      if ( string.IsNullOrEmpty(s) )
        return EFileSort.FileCreationTime;

      if ( Enum.GetNames(typeof(EFileSort)).All(p => string.Compare(s.ToLower(), p.ToLower(), StringComparison.Ordinal) != 0) )
        return EFileSort.FileCreationTime;

      Enum.TryParse(s, out EFileSort fileSort);

      return fileSort;
    }

    #endregion
  }
}
