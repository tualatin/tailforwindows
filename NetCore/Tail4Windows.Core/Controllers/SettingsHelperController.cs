using System.Configuration;
using System.IO;
using log4net;
using Newtonsoft.Json;
using Org.Vs.TailForWin.Core.Data.Settings;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Logging;
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
    private readonly SemaphoreSlim _semaphore;

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
    /// Standard constructor
    /// </summary>
    public SettingsHelperController() => _semaphore = new SemaphoreSlim(1, 1);

    /// <summary>
    /// Reads current settings
    /// </summary>
    /// <returns>Task</returns>
    public Task ReadSettingsAsync() => ReadSettingsAsync(new CancellationTokenSource(TimeSpan.FromMinutes(5)));

    /// <summary>
    /// Reads current settings
    /// </summary>
    /// <param name="cts"><see cref="CancellationTokenSource"/></param>
    /// <returns>Task</returns>
    public async Task ReadSettingsAsync(CancellationTokenSource cts)
    {
      await _semaphore.WaitAsync(cts.Token).ConfigureAwait(false);

      try
      {
        var t1 = AddPropertiesIfNotExistsAsync(cts);
        var t2 = Task.Run(() => ReadUserSettings(), cts.Token);

        await Task.WhenAll(t1, t2).ConfigureAwait(false);
      }
      finally
      {
        _semaphore.Release();
      }
    }

    private Task AddPropertiesIfNotExistsAsync(CancellationTokenSource cts)
    {
      var settings = new Dictionary<string, string>
      {
        {"Portable", DefaultEnvironmentSettings.IsPortable.ToString()}
      };
      return AddNewPropertyAsync(settings, cts);
    }

    private static void ReadUserSettings()
    {
      try
      {
        LOG.Trace($"Read {CoreEnvironment.ApplicationTitle} user settings");

        if ( !Directory.Exists(CoreEnvironment.UserSettingsPath) )
          Directory.CreateDirectory(CoreEnvironment.UserSettingsPath);

        // To remove as soon as possible
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

        using ( var sr = File.OpenText(CoreEnvironment.ApplicationSettingsFile) )
        {
          var serializer = new JsonSerializer();
          CurrentSettings = (EnvironmentSettings) serializer.Deserialize(sr, typeof(EnvironmentSettings));
        }
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
      }
    }

    /// <summary>
    /// Writes current settings
    /// </summary>
    /// <returns>Task</returns>
    public Task SaveSettingsAsync()
    {
      using ( var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2)) )
      {
        return SaveSettingsAsync(cts);
      }
    }

    /// <summary>
    /// Writes current settings
    /// </summary>
    /// <param name="cts"><see cref="CancellationTokenSource"/></param>
    /// <returns>Task</returns>
    public async Task SaveSettingsAsync(CancellationTokenSource cts)
    {
      await _semaphore.WaitAsync(cts.Token).ConfigureAwait(false);

      try
      {
        await Task.Run(() => SaveUserSettings(), cts.Token).ConfigureAwait(false);
      }
      finally
      {
        _semaphore.Release();
      }
    }

    /// <summary>
    /// Reset current color settings
    /// </summary>
    /// <param name="cts"><see cref="CancellationTokenSource"/></param>
    /// <returns>Task</returns>
    public Task SetDefaultColorsAsync(CancellationTokenSource cts)
    {
      SetDefaultLogViewerSettings();
      SetDefaultStatusBarSettings();

      return SaveSettingsAsync(cts);
    }

    private static void SaveUserSettings()
    {
      try
      {
        LOG.Trace($"Save {CoreEnvironment.ApplicationTitle} user settings");

        using ( var fs = File.Open(CoreEnvironment.ApplicationSettingsFile, FileMode.OpenOrCreate) )
        using ( var sw = new StreamWriter(fs) )
        using ( JsonWriter jw = new JsonTextWriter(sw) )
        {
          jw.Formatting = Formatting.Indented;
          var serializer = new JsonSerializer();
          serializer.Serialize(jw, CurrentSettings);
        }
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
      }
    }

    /// <summary>
    /// Reset current settings
    /// </summary>
    /// <returns>Task</returns>
    public Task SetDefaultSettingsAsync()
    {
      using ( var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5)) )
      {
        return SetDefaultSettingsAsync(cts);
      }
    }

    /// <summary>
    /// Reset current settings
    /// </summary>
    /// <param name="cts"><see cref="CancellationTokenSource"/></param>
    /// <returns>Task</returns>
    public Task SetDefaultSettingsAsync(CancellationTokenSource cts) => Task.Run(() => SetDefaultSettings(), cts.Token);

    private static void SetDefaultSettings()
    {
      LOG.Trace($"Reset {CoreEnvironment.ApplicationTitle} settings");

      SetDefaultWindowSettings();
      SetDefaultStatusBarSettings();
      SetDefaultProxySettings();
      SetDefaultLogViewerSettings();
      SetDefaultAlertSettings();
      SetDefaultSmtpSettings();
      SetDefaultSmartWatchSettings();
    }

    private static void SetDefaultWindowSettings()
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
      CurrentSettings.OptionWindowPositionY = DefaultEnvironmentSettings.WindowPositionY;
      CurrentSettings.OptionWindowPositionX = DefaultEnvironmentSettings.WindowPositionX;
      CurrentSettings.OptionWindowHeight = DefaultEnvironmentSettings.WindowHeight;
      CurrentSettings.OptionWindowWidth = DefaultEnvironmentSettings.WindowWidth;
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
      CurrentSettings.MouseHover = DefaultEnvironmentSettings.MouseHover;
      CurrentSettings.ClearLogWindowIfLogIsCleared = DefaultEnvironmentSettings.ClearLogWindowIfLogIsCleared;
      CurrentSettings.ActivateDragDropWindow = DefaultEnvironmentSettings.ActivateDragDropWindow;
      CurrentSettings.SaveLogFileHistory = DefaultEnvironmentSettings.SaveLogFileHistory;
      CurrentSettings.HistoryMaxSize = DefaultEnvironmentSettings.HistoryMaxSize;
      CurrentSettings.ShowExtendedSettings = DefaultEnvironmentSettings.ShowExtendedSettings;
      CurrentSettings.SplitterWindowBehavior = DefaultEnvironmentSettings.SplitterWindowBehavior;
      CurrentSettings.EditorPath = string.Empty;
      CurrentSettings.ExportFormat = DefaultEnvironmentSettings.ExportFormat;
      CurrentSettings.StatisticWindowHeight = DefaultEnvironmentSettings.StatisticWindowHeight;
      CurrentSettings.StatisticWindowLeft = DefaultEnvironmentSettings.StatisticWindowLeft;
      CurrentSettings.StatisticWindowTop = DefaultEnvironmentSettings.StatisticWindowTop;
      CurrentSettings.StatisticWindowTop = DefaultEnvironmentSettings.StatisticWindowTop;
    }

    private static void SetDefaultStatusBarSettings()
    {
      CurrentSettings.ColorSettings.StatusBarInactiveBackgroundColorHex = DefaultEnvironmentSettings.StatusBarInactiveBackgroundColor;
      CurrentSettings.ColorSettings.StatusBarFileLoadedBackgroundColorHex = DefaultEnvironmentSettings.StatusBarFileLoadedBackgroundColor;
      CurrentSettings.ColorSettings.StatusBarTailBackgroundColorHex = DefaultEnvironmentSettings.StatusBarTailBackgroundColor;
    }

    private static void SetDefaultLogViewerSettings()
    {
      CurrentSettings.ColorSettings.ForegroundColorHex = DefaultEnvironmentSettings.ForegroundColor;
      CurrentSettings.ColorSettings.BackgroundColorHex = DefaultEnvironmentSettings.BackgroundColor;
      CurrentSettings.ColorSettings.SelectionBackgroundColorHex = DefaultEnvironmentSettings.SelectionBackgroundColor;
      CurrentSettings.ColorSettings.FindHighlightForegroundColorHex = DefaultEnvironmentSettings.SearchHighlightForegroundColor;
      CurrentSettings.ColorSettings.FindHighlightBackgroundColorHex = DefaultEnvironmentSettings.SearchHighlightBackgroundColor;
      CurrentSettings.ColorSettings.LineNumberColorHex = DefaultEnvironmentSettings.LineNumberColor;
      CurrentSettings.ColorSettings.LineNumberHighlightColorHex = DefaultEnvironmentSettings.HighlightLineNumberColor;
      CurrentSettings.ColorSettings.SplitterBackgroundColorHex = DefaultEnvironmentSettings.SplitterBackgroundColor;
      CurrentSettings.ColorSettings.MouseHoverColorHex = DefaultEnvironmentSettings.MouseHoverColor;
    }

    private static void SetDefaultAlertSettings()
    {
      CurrentSettings.AlertSettings.BringToFront = DefaultEnvironmentSettings.AlertBringToFront;
      CurrentSettings.AlertSettings.PlaySoundFile = DefaultEnvironmentSettings.AlertPlaySoundFile;
      CurrentSettings.AlertSettings.SendMail = DefaultEnvironmentSettings.AlertSendMail;
      CurrentSettings.AlertSettings.PopupWnd = DefaultEnvironmentSettings.AlertPopUpWindow;
      CurrentSettings.AlertSettings.MailAddress = DefaultEnvironmentSettings.AlertMailAddress;
      CurrentSettings.AlertSettings.SoundFileNameFullPath = DefaultEnvironmentSettings.AlertSoundFile;
    }

    private static void SetDefaultSmtpSettings()
    {
      CurrentSettings.SmtpSettings.Ssl = DefaultEnvironmentSettings.SmtpSsl;
      CurrentSettings.SmtpSettings.Tls = DefaultEnvironmentSettings.SmtpTls;
      CurrentSettings.SmtpSettings.SmtpPort = DefaultEnvironmentSettings.SmtpPort;
      CurrentSettings.SmtpSettings.SmtpServerName = DefaultEnvironmentSettings.SmtpServer;
      CurrentSettings.SmtpSettings.LoginName = DefaultEnvironmentSettings.SmtpUsername;
      CurrentSettings.SmtpSettings.FromAddress = DefaultEnvironmentSettings.SmtpFromMailAddress;
      CurrentSettings.SmtpSettings.Subject = DefaultEnvironmentSettings.SmtpSubject;
    }

    private static void SetDefaultProxySettings()
    {
      CurrentSettings.ProxySettings.UserName = DefaultEnvironmentSettings.ProxyUserName;
      CurrentSettings.ProxySettings.ProxyPort = DefaultEnvironmentSettings.ProxyPort;
      CurrentSettings.ProxySettings.ProxyUrl = DefaultEnvironmentSettings.ProxyUrl;
      CurrentSettings.ProxySettings.UseSystemSettings = DefaultEnvironmentSettings.ProxyUseSystemSettings;
    }

    private static void SetDefaultSmartWatchSettings()
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
    public async Task ReloadCurrentSettingsAsync(CancellationTokenSource cts)
    {
      await _semaphore.WaitAsync(cts.Token).ConfigureAwait(false);

      try
      {
        await Task.Run(() => ReloadCurrentSettings(), cts.Token).ConfigureAwait(false);
      }
      finally
      {
        _semaphore.Release();
      }
    }

    private void ReloadCurrentSettings()
    {
      LOG.Trace($"Reload {CoreEnvironment.ApplicationTitle} settings");
      ConfigurationManager.RefreshSection("appSettings");

      ReadUserSettings();
    }

    /// <summary>
    /// Adds a new property to config file
    /// </summary>
    /// <param name="settings">List of configuration pair</param>
    /// <param name="cts"><see cref="CancellationTokenSource"/></param>
    /// <returns>Task</returns>
    public Task AddNewPropertyAsync(Dictionary<string, string> settings, CancellationTokenSource cts) =>
      Task.Run(() => AddNewProperty(settings), cts.Token);

    private static void AddNewProperty(Dictionary<string, string> settings)
    {
      if ( settings == null || settings.Count == 0 )
        return;

      LOG.Trace("Add missing config properties");

      try
      {
        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

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
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
      }
    }
  }
}
