using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using log4net;
using Newtonsoft.Json;
using Org.Vs.TailForWin.Core.Data.Settings;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Extensions;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Core.Controllers
{
  /// <inheritdoc />
  /// <summary>
  /// Holds settings from environment
  /// </summary>
  public class SettingsHelperController : ISettingsHelper
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(SettingsHelperController));
    private static readonly object MyLock = new object();

    /// <summary>
    /// Current lock time span in milliseconds
    /// </summary>
    private const int LockTimeSpanIsMs = 200;

    /// <summary>
    /// Current T4W settings
    /// </summary>
    public static EnvironmentSettings CurrentSettings
    {
      get;
      private set;
    } = new EnvironmentSettings();

    /// <summary>
    /// Current T4W app settings
    /// </summary>
    public static AppSettings CurrentAppSettings
    {
      get;
    } = new AppSettings();

    /// <summary>
    /// Reads current settings
    /// </summary>
    /// <returns>Task</returns>
    public async Task ReadSettingsAsync() => await ReadSettingsAsync(new CancellationTokenSource(TimeSpan.FromMinutes(5)));

    /// <summary>
    /// Reads current settings
    /// </summary>
    /// <param name="cts"><see cref="CancellationTokenSource"/></param>
    /// <returns>Task</returns>
    public async Task ReadSettingsAsync(CancellationTokenSource cts)
    {
      await AddPropertiesIfNotExistsAsync(cts);

      await Task.Run(() => ReadSettings(), cts.Token);
      await Task.Run(() => ReadUserSettings(), cts.Token);

      await RemovePropertiesIfExistsAsync(cts);
    }

    private async Task AddPropertiesIfNotExistsAsync(CancellationTokenSource cts)
    {
      var settings = new Dictionary<string, string>
      {
        { "IsPortable", DefaultEnvironmentSettings.IsPortable.ToString() }
      };

      await AddNewPropertyAsync(settings, cts);
    }

    private async Task RemovePropertiesIfExistsAsync(CancellationTokenSource cts)
    {
      var settings = new[]
      {
        "LastViewedOptionPage",
        "Language",
        "DragDropWindow",
        "SplitterWindowBehavior",
        "LinesRead",
        "LogLineLimit",
        "AlwaysOnTop",
        "ShowNLineAtStart",
        "AlwaysScrollToEnd",
        "RestoreWindowSize",
        "InactiveBackgroundColor",
        "InactiveForegroundColor",
         "WndWidth",
        "WndHeight",
        "SaveWindowPosition",
        "WindowState",
        "WndXPos",
        "WndYPos",
        "DefaultRefreshRate",
        "DefaultThreadPriority",
        "ExitWithEsc" ,
        "TimeFormat",
        "DateFormat",
        "SearchwndYPos",
        "SearchwndXPos",
        "ForegroundColor",
        "SingleInstance" ,
        "ContinuedScroll" ,
        "LastUsedExportFormat",
        "BackgroundColor",
        "SelectionBackgroundColor",
        "FindHighlightForegroundColor",
        "FindHighlightBackgroundColor",
        "StatusBarInactiveBackgroundColor",
        "StatusBarFileLoadedBackgroundColor",
        "StatusBarTailBackgroundColor",
        "SplitterBackgroundColor",
        "FileManagerSort",
        "ShowLineNumbers",
        "LineNumbersColor",
        "HighlightColor",
        "AutoUpdate" ,
        "SmartWatch",
        "Statics",
        "ShowExtendedSettings",
        "GroupByCategory",
        "SaveLogFileHistory",
        "LogFileHistorySize",
        "CurrentWindowStyle",
        "DeleteLogFiles",
        "DeleteLogFileOlderThan",
        "EditorPath",
        "Alert.BringToFront",
        "Alert.PlaySoundFile",
        "Alert.SoundFile",
        "Alert.SendEMail",
        "Alert.EMailAddress",
        "Alert.PopupWindow",
        "Proxy.Port" ,
        "Proxy.Url" ,
        "Proxy.UseSystem" ,
        "Proxy.UserName" ,
        "Proxy.Use",
        "Proxy.Password",
        "Smtp.Server" ,
        "Smtp.Port" ,
        "Smtp.Login" ,
        "Smtp.FromEMail" ,
        "Smtp.Subject",
        "Smtp.Ssl",
        "Smtp.Tls",
        "Smtp.Password",
        "SmartWatch.FilterByExtension" ,
        "SmartWatch.NewTab" ,
        "SmartWatch.Mode" ,
        "SmartWatch.AutoRun" ,
        "SmartWatch.SmartWatchInterval"
      };

      await RemoveObsoletePropertiesAsync(settings, cts.Token);
    }

    private async Task RemoveObsoletePropertiesAsync(IReadOnlyCollection<string> obsoleteSettings, CancellationToken token) => await Task.Run(() =>
    {
      if ( Monitor.TryEnter(MyLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
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
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        }
        finally
        {
          Monitor.Exit(MyLock);
        }
      }
      else
      {
        LOG.Error("Can not lock!");
      }
    }, token);

    private void ReadSettings()
    {
      if ( Monitor.TryEnter(MyLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
      {
        try
        {
          LOG.Trace($"Read {CoreEnvironment.ApplicationTitle} settings");

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
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        }
        finally
        {
          Monitor.Exit(MyLock);
        }
      }
      else
      {
        LOG.Error("Can not lock!");
      }
    }

    private void ReadUserSettings()
    {
      if ( Monitor.TryEnter(MyLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
      {
        try
        {
          LOG.Trace($"Read {CoreEnvironment.ApplicationTitle} user settings");

          if ( !Directory.Exists(CoreEnvironment.UserSettingsPath) )
            Directory.CreateDirectory(CoreEnvironment.UserSettingsPath);

          if ( !File.Exists(CoreEnvironment.ApplicationSettingsFile) )
          {
            if ( CurrentSettings.LastViewedOptionPage != Guid.Empty )
            {
              // Convert old settings to JSON
              SaveUserSettings();
            }
            else
            {
              SetDefaultSettings();
              SaveUserSettings();
            }
            return;
          }

          using ( StreamReader sr = File.OpenText(CoreEnvironment.ApplicationSettingsFile) )
          {
            JsonSerializer serializer = new JsonSerializer();
            CurrentSettings = (EnvironmentSettings) serializer.Deserialize(sr, typeof(EnvironmentSettings));
          }
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        }
        finally
        {
          Monitor.Exit(MyLock);
        }
      }
      else
      {
        LOG.Error("Can not lock!");
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
    public async Task SaveSettingsAsync(CancellationTokenSource cts)
    {
      await Task.Run(() => SaveSettings(), cts.Token);
      await Task.Run(() => SaveUserSettings(), cts.Token);
    }

    /// <summary>
    /// Reset current color settings
    /// </summary>
    /// <param name="cts"><see cref="CancellationTokenSource"/></param>
    /// <returns>Task</returns>
    public async Task SetDefaultColorsAsync(CancellationTokenSource cts)
    {
      SetDefaultLogViewerSettings();
      SetDefaultStatusBarSettings();

      await SaveSettingsAsync(cts);
    }

    private void SaveSettings()
    {
      if ( Monitor.TryEnter(MyLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
      {
        try
        {
          LOG.Trace($"Save {CoreEnvironment.ApplicationTitle} settings");

          Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

          if ( config.AppSettings.Settings.Count <= 0 )
            return;

          SaveWindowSettings(config);

          config.Save(ConfigurationSaveMode.Modified);
          ConfigurationManager.RefreshSection("appSettings");
        }
        catch ( ConfigurationErrorsException ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        }
        finally
        {
          Monitor.Exit(MyLock);
        }
      }
      else
      {
        LOG.Error("Can not lock!");
      }
    }

    private void SaveUserSettings()
    {
      if ( Monitor.TryEnter(MyLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
      {
        try
        {
          LOG.Trace($"Save {CoreEnvironment.ApplicationTitle} user settings");

          using ( FileStream fs = File.Open(CoreEnvironment.ApplicationSettingsFile, FileMode.OpenOrCreate) )
          using ( StreamWriter sw = new StreamWriter(fs) )
          using ( JsonWriter jw = new JsonTextWriter(sw) )
          {
            jw.Formatting = Formatting.Indented;
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(jw, CurrentSettings);
          }
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        }
        finally
        {
          Monitor.Exit(MyLock);
        }
      }
      else
      {
        LOG.Error("Can not lock!");
      }
    }

    private void SaveWindowSettings(Configuration config) => WriteValueToSetting(config, "IsPortable", CurrentAppSettings.IsPortable);

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
      if ( Monitor.TryEnter(MyLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
      {
        try
        {
          LOG.Trace($"Reset {CoreEnvironment.ApplicationTitle} settings");

          SetDefaultWindowSettings();
          SetDefaultStatusBarSettings();
          SetDefaultProxySettings();
          SetDefaultLogViewerSettings();
          SetDefaultAlertSettings();
          SetDefaultSmptSettings();
          SetDefaultSmartWatchSettings();
        }
        finally
        {
          Monitor.Exit(MyLock);
        }
      }
      else
      {
        LOG.Error("Can not lock!");
      }
    }

    private void SetDefaultWindowSettings()
    {
      CurrentSettings.LastViewedOptionPage = Guid.Empty;
      CurrentSettings.Language = DefaultEnvironmentSettings.Language;
      CurrentSettings.CurrentWindowStyle = DefaultEnvironmentSettings.CurrentWindowStyle;
      CurrentSettings.AlwaysOnTop = DefaultEnvironmentSettings.AlwaysOnTop;
      CurrentSettings.AlwaysScrollToEnd = DefaultEnvironmentSettings.AlwaysScrollToEnd;
      CurrentSettings.ContinuedScroll = DefaultEnvironmentSettings.ContinuedScroll;
      CurrentSettings.CurrentWindowState = DefaultEnvironmentSettings.CurrentWindowState;
      CurrentSettings.DeleteLogFiles = DefaultEnvironmentSettings.DeleteLogFiles;
      CurrentSettings.LogFilesOlderThan = DefaultEnvironmentSettings.DeleteLogFilesOlderThan;
      CurrentSettings.ExitWithEscape = DefaultEnvironmentSettings.ExitWithEscape;
      CurrentSettings.SingleInstance = DefaultEnvironmentSettings.SingleInstance;
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
      CurrentSettings.EditorPath = string.Empty;
      CurrentSettings.ExportFormat = DefaultEnvironmentSettings.ExportFormat;
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
      CurrentSettings.ColorSettings.SelectionBackgroundColorHex = DefaultEnvironmentSettings.SelectionBackgroundColor;
      CurrentSettings.ColorSettings.FindHighlightForegroundColorHex = DefaultEnvironmentSettings.SearchHighlightForegroundColor;
      CurrentSettings.ColorSettings.FindHighlightBackgroundColorHex = DefaultEnvironmentSettings.SearchHighlightBackgroundColor;
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
      CurrentSettings.SmtpSettings.FromAddress = DefaultEnvironmentSettings.SmtpFromMailAddress;
      CurrentSettings.SmtpSettings.Subject = DefaultEnvironmentSettings.SmtpSubject;
    }

    private void SetDefaultProxySettings()
    {
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
      CurrentSettings.SmartWatchSettings.SmartWatchInterval = DefaultEnvironmentSettings.SmartWatchInterval;
    }

    /// <summary>
    /// Reloads current settings
    /// </summary>
    /// <param name="cts"><see cref="CancellationTokenSource"/></param>
    /// <returns>Task</returns>
    public async Task ReloadCurrentSettingsAsync(CancellationTokenSource cts) => await Task.Run(() => ReloadCurrentSettings(), cts.Token);

    private void ReloadCurrentSettings()
    {
      if ( Monitor.TryEnter(MyLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
      {
        try
        {
          LOG.Trace($"Reload {CoreEnvironment.ApplicationTitle} settings");
          ConfigurationManager.RefreshSection("appSettings");

          ReadUserSettings();
        }
        finally
        {
          Monitor.Exit(MyLock);
        }
      }
      else
      {
        LOG.Error("Can not lock!");
      }
    }

    /// <summary>
    /// Adds a new property to config file
    /// </summary>
    /// <param name="settings">List of configuration pair</param>
    /// <param name="cts"><see cref="CancellationTokenSource"/></param>
    /// <returns>Task</returns>
    public async Task AddNewPropertyAsync(Dictionary<string, string> settings, CancellationTokenSource cts) => await Task.Run(() => AddNewProperty(settings), cts.Token);

    private void AddNewProperty(Dictionary<string, string> settings)
    {
      if ( settings == null || settings.Count == 0 )
        return;

      if ( Monitor.TryEnter(MyLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
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
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        }
        finally
        {
          Monitor.Exit(MyLock);
        }
      }
      else
      {
        LOG.Error("Can not lock!");
      }
    }

    #region HelperFunctions

    private void ReadWindowSettings()
    {
      CurrentAppSettings.IsPortable = GetBoolFromSetting("IsPortable");

      string guid = GetStringFromSetting("LastViewedOptionPage");

      if ( !string.IsNullOrWhiteSpace(guid) )
        CurrentSettings.LastViewedOptionPage = Guid.Parse(guid);

      CurrentSettings.RestoreWindowSize = GetBoolFromSetting("RestoreWindowSize");
      CurrentSettings.AlwaysOnTop = GetBoolFromSetting("AlwaysOnTop");
      CurrentSettings.RestoreWindowSize = GetBoolFromSetting("RestoreWindowSize");
      CurrentSettings.WindowWidth = GetDoubleFromSetting("WndWidth");
      CurrentSettings.WindowHeight = GetDoubleFromSetting("WndHeight");
      CurrentSettings.WindowPositionX = GetDoubleFromSetting("WndXPos");
      CurrentSettings.WindowPositionY = GetDoubleFromSetting("WndYPos");
      CurrentSettings.SaveWindowPosition = GetBoolFromSetting("SaveWindowPosition");
      CurrentSettings.SingleInstance = GetBoolFromSetting("SingleInstance");
      CurrentSettings.ExitWithEscape = GetBoolFromSetting("ExitWithEsc");
      CurrentSettings.CurrentWindowState = GetWindowState(GetStringFromSetting("WindowState"));
      CurrentSettings.DeleteLogFiles = GetBoolFromSetting("DeleteLogFiles");
      CurrentSettings.LogFilesOlderThan = GetIntFromSetting("DeleteLogFileOlderThan");
      CurrentSettings.CurrentWindowStyle = GetWindowStyle(GetStringFromSetting("CurrentWindowStyle"));
      CurrentSettings.Language = GetUiLanguage(GetStringFromSetting("Language"));
      CurrentSettings.AlwaysScrollToEnd = GetBoolFromSetting("AlwaysScrollToEnd");
      CurrentSettings.ContinuedScroll = GetBoolFromSetting("ContinuedScroll");
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
      CurrentSettings.SmartWatch = GetBoolFromSetting("SmartWatch");
      CurrentSettings.EditorPath = GetStringFromSetting("EditorPath");
      CurrentSettings.ExportFormat = GetExportFormat(GetStringFromSetting("LastUsedExportFormat"));
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
      CurrentSettings.ColorSettings.SelectionBackgroundColorHex = GetStringFromSetting("SelectionBackgroundColor");
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
    }

    private void ReadSmartWatchSettings()
    {
      CurrentSettings.SmartWatchSettings.AutoRun = GetBoolFromSetting("SmartWatch.AutoRun", true);
      CurrentSettings.SmartWatchSettings.FilterByExtension = GetBoolFromSetting("SmartWatch.FilterByExtension", true);
      CurrentSettings.SmartWatchSettings.NewTab = GetBoolFromSetting("SmartWatch.NewTab", true);
      CurrentSettings.SmartWatchSettings.Mode = GetSmartWatchMode(GetStringFromSetting("SmartWatch.Mode"));
      CurrentSettings.SmartWatchSettings.SmartWatchInterval = GetIntFromSetting("SmartWatch.SmartWatchInterval", 2000);
    }

    private static string GetStringFromSetting(string setting) => string.IsNullOrWhiteSpace(setting) ? string.Empty : ConfigurationManager.AppSettings[setting];

    private static int GetIntFromSetting(string setting, int defaultValue = -1) => ConfigurationManager.AppSettings[setting].ConvertToInt(defaultValue);

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
    /// Gets last used export format from Enum
    /// </summary>
    /// <param name="s">Enum value as string</param>
    /// <returns>Enum of <see cref="EExportFormat"/></returns>
    private static EExportFormat GetExportFormat(string s)
    {
      if ( string.IsNullOrWhiteSpace(s) )
        return EExportFormat.Csv;

      if ( !Enum.TryParse(s, out EExportFormat format) )
        format = EExportFormat.Csv;

      return format;
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
