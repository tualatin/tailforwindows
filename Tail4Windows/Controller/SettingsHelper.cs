using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using log4net;
using Org.Vs.TailForWin.Data;
using Org.Vs.TailForWin.Data.Enums;


namespace Org.Vs.TailForWin.Controller
{
  /// <summary>
  /// SettingsHelper
  /// </summary>
  public class SettingsHelper
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(SettingsHelper));


    /// <summary>
    /// Read app settings
    /// </summary>
    public static void ReadSettings()
    {
      LOG.Debug("Read TailForWindows settings");

      try
      {
        AddMissingSettingsToConfigFile();

        TailSettings.LinesRead = GetIntFromSetting("LinesRead", 10);
        TailSettings.AlwaysOnTop = GetBoolFromSetting("AlwaysOnTop");
        TailSettings.ShowNLineAtStart = GetBoolFromSetting("ShowNLineAtStart", true);
        TailSettings.ShowLineNumbers = GetBoolFromSetting("ShowLineNumbers");
        TailSettings.AlwaysScrollToEnd = GetBoolFromSetting("AlwaysScrollToEnd", true);
        TailSettings.RestoreWindowSize = GetBoolFromSetting("RestoreWindowSize");
        TailSettings.WndWidth = GetDoubleFromSetting("WndWidth");
        TailSettings.WndHeight = GetDoubleFromSetting("WndHeight");
        TailSettings.SaveWindowPosition = GetBoolFromSetting("SaveWindowPosition");
        TailSettings.WndXPos = GetDoubleFromSetting("WndXPos");
        TailSettings.WndYPos = GetDoubleFromSetting("WndYPos");
        TailSettings.ExitWithEscape = GetBoolFromSetting("ExitWithEsc");
        TailSettings.AutoUpdate = GetBoolFromSetting("AutoUpdate");
        TailSettings.CurrentWindowState = GetWindowState(GetStringFromSetting("WindowState"));

        ReadThreadPriorityEnum(GetStringFromSetting("DefaultThreadPriority"));
        ReadThreadRefreshRateEnum(GetStringFromSetting("DefaultRefreshRate"));

        TailSettings.SmartWatch = GetBoolFromSetting("SmartWatch");
        TailSettings.GroupByCategory = GetBoolFromSetting("GroupByCategory", true);
        TailSettings.DeleteLogFiles = GetBoolFromSetting("DeleteLogFiles", true);
        TailSettings.CurrentWindowStyle = GetWindowStyle(GetStringFromSetting("CurrentWindowStyle"));

        ReadTimeFormatEnum(GetStringFromSetting("TimeFormat"));
        ReadDateFormatEnum(GetStringFromSetting("DateFormat"));
        CheckHexColorValue(GetStringFromSetting("ForegroundColor"), ETailLogColorTypes.ForegroundColor);
        CheckHexColorValue(GetStringFromSetting("BackgroundColor"), ETailLogColorTypes.BackgroundColor);
        CheckHexColorValue(GetStringFromSetting("InactiveForegroundColor"), ETailLogColorTypes.InactiveForegroundColor);
        CheckHexColorValue(GetStringFromSetting("InactiveBackgroundColor"), ETailLogColorTypes.InactiveBackgroundColor);
        CheckHexColorValue(GetStringFromSetting("FindHighlightForegroundColor"), ETailLogColorTypes.FindHighlightForegroundColor);
        CheckHexColorValue(GetStringFromSetting("FindHighlightBackgroundColor"), ETailLogColorTypes.FindHighlightBackgroundColor);
        CheckHexColorValue(GetStringFromSetting("LineNumbersColor"), ETailLogColorTypes.LineNumbersColor);
        CheckHexColorValue(GetStringFromSetting("HighlightColor"), ETailLogColorTypes.HighlightColor);
        SetSearchWindowPosition(GetStringFromSetting("SearchwndXPos"));
        SetSearchWindowPosition(GetStringFromSetting("SearchwndYPos"), false);
        ReadFileSortEnum(GetStringFromSetting("FileManagerSort"));

        TailSettings.LogLineLimit = GetIntFromSetting("LogLineLimit");

        ReadAlertSettings();
        ReadProxySettings();
        ReadSmtpSettings();
        ReadSmartWatchSettings();
      }
      catch (ConfigurationErrorsException ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    private static void AddMissingSettingsToConfigFile()
    {
      if (ConfigurationManager.AppSettings["WindowState"] == null)
        AddNewProperties_IntoConfigFile("WindowState", System.Windows.WindowState.Normal.ToString());

      if (ConfigurationManager.AppSettings["SmartWatch"] == null)
        AddNewProperties_IntoConfigFile("SmartWatch", false.ToString());

      if (ConfigurationManager.AppSettings["GroupByCategory"] == null)
        AddNewProperties_IntoConfigFile("GroupByCategory", true.ToString());

      if (ConfigurationManager.AppSettings["DeleteLogFiles"] == null)
        AddNewProperties_IntoConfigFile("DeleteLogFiles", true.ToString());

      if (ConfigurationManager.AppSettings["CurrentWindowStyle"] == null)
        AddNewProperties_IntoConfigFile("CurrentWindowStyle", EWindowStyle.ModernBlueWindowStyle.ToString());

      if (ConfigurationManager.AppSettings["LogLineLimit"] == null)
        AddNewProperties_IntoConfigFile("LogLineLimit", "-1");
      
      #region SmartWatch settings
      
      if (ConfigurationManager.AppSettings["SmartWatch.FilterByExtension"] == null)
        AddNewProperties_IntoConfigFile("SmartWatch.FilterByExtension", true.ToString());

      if (ConfigurationManager.AppSettings["SmartWatch.NewTab"] == null)
        AddNewProperties_IntoConfigFile("SmartWatch.NewTab", true.ToString());

      if (ConfigurationManager.AppSettings["SmartWatch.Mode"] == null)
        AddNewProperties_IntoConfigFile("SmartWatch.Mode", ESmartWatchMode.Manual.ToString());

      if (ConfigurationManager.AppSettings["SmartWatch.AutoRun"] == null)
        AddNewProperties_IntoConfigFile("SmartWatch.AutoRun", true.ToString());
      
      #endregion
      
      if (ConfigurationManager.AppSettings["Alert.PopupWindow"] == null)
        AddNewProperties_IntoConfigFile("Alert.PopupWindow", "false");
    }

    /// <summary>
    /// Save app settings
    /// </summary>
    public static void SaveSettings()
    {
      try
      {
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        if (config.AppSettings.Settings.Count <= 0)
          return;

        WriteValueToSetting(config, "LinesRead", TailSettings.LinesRead.ToString(CultureInfo.InvariantCulture));
        WriteValueToSetting(config, "LogLineLimit", TailSettings.LogLineLimit.ToString(CultureInfo.InvariantCulture));
        WriteValueToSetting(config, "AlwaysOnTop", TailSettings.AlwaysOnTop.ToString());
        WriteValueToSetting(config, "ShowNLineAtStart", TailSettings.ShowNLineAtStart.ToString());
        WriteValueToSetting(config, "AlwaysScrollToEnd", TailSettings.AlwaysScrollToEnd.ToString());
        WriteValueToSetting(config, "RestoreWindowSize", TailSettings.RestoreWindowSize.ToString());
        WriteValueToSetting(config, "WndWidth", TailSettings.WndWidth.ToString(CultureInfo.InvariantCulture));
        WriteValueToSetting(config, "WndHeight", TailSettings.WndHeight.ToString(CultureInfo.InvariantCulture));
        WriteValueToSetting(config, "SaveWindowPosition", TailSettings.RestoreWindowSize.ToString());
        WriteValueToSetting(config, "WndXPos", TailSettings.WndXPos.ToString(CultureInfo.InvariantCulture));
        WriteValueToSetting(config, "WndYPos", TailSettings.WndYPos.ToString(CultureInfo.InvariantCulture));
        WriteValueToSetting(config, "DefaultThreadPriority", TailSettings.DefaultThreadPriority.ToString());
        WriteValueToSetting(config, "DefaultRefreshRate", TailSettings.DefaultRefreshRate.ToString());
        WriteValueToSetting(config, "ExitWithEsc", TailSettings.ExitWithEscape.ToString());
        WriteValueToSetting(config, "TimeFormat", TailSettings.DefaultTimeFormat.ToString());
        WriteValueToSetting(config, "DateFormat", TailSettings.DefaultDateFormat.ToString());
        WriteValueToSetting(config, "ForegroundColor", TailSettings.DefaultForegroundColor);
        WriteValueToSetting(config, "BackgroundColor", TailSettings.DefaultBackgroundColor);
        WriteValueToSetting(config, "InactiveForegroundColor", TailSettings.DefaultInactiveForegroundColor);
        WriteValueToSetting(config, "InactiveBackgroundColor", TailSettings.DefaultInactiveBackgroundColor);
        WriteValueToSetting(config, "FindHighlightForegroundColor", TailSettings.DefaultHighlightForegroundColor);
        WriteValueToSetting(config, "FindHighlightBackgroundColor", TailSettings.DefaultHighlightBackgroundColor);
        WriteValueToSetting(config, "SearchwndXPos", TailSettings.SearchWndXPos.ToString(CultureInfo.InvariantCulture));
        WriteValueToSetting(config, "SearchwndYPos", TailSettings.SearchWndYPos.ToString(CultureInfo.InvariantCulture));
        WriteValueToSetting(config, "FileManagerSort", TailSettings.DefaultFileSort.ToString());
        WriteValueToSetting(config, "ShowLineNumbers", TailSettings.ShowLineNumbers.ToString());
        WriteValueToSetting(config, "LineNumbersColor", TailSettings.DefaultLineNumbersColor);
        WriteValueToSetting(config, "HighlightColor", TailSettings.DefaultHighlightColor);
        WriteValueToSetting(config, "AutoUpdate", TailSettings.AutoUpdate.ToString());
        WriteValueToSetting(config, "SmartWatch", TailSettings.SmartWatch.ToString());
        WriteValueToSetting(config, "GroupByCategory", TailSettings.GroupByCategory.ToString());
        WriteValueToSetting(config, "CurrentWindowStyle", TailSettings.CurrentWindowStyle.ToString());
        WriteValueToSetting(config, "WindowState", TailSettings.CurrentWindowState.ToString());
        WriteValueToSetting(config, "DeleteLogFiles", TailSettings.DeleteLogFiles.ToString());

        SaveAlertSettings(config);
        SaveProxySettings(config);
        SaveSmptSettings(config);
        SaveSmartWatchSettings(config);

        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("appSettings");
      }
      catch (ConfigurationErrorsException ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    /// <summary>
    /// Save search dialog window position
    /// </summary>
    public static void SaveSearchWindowPosition()
    {
      try
      {
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        if (config.AppSettings.Settings.Count <= 0)
          return;
        
        WriteValueToSetting(config, "SearchwndXPos", TailSettings.SearchWndXPos.ToString(CultureInfo.InvariantCulture));
        WriteValueToSetting(config, "SearchwndYPos", TailSettings.SearchWndYPos.ToString(CultureInfo.InvariantCulture));

        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("appSettings");
      }
      catch (ConfigurationErrorsException ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    /// <summary>
    /// Set app settings to default parameters
    /// </summary>
    public static void SetToDefault()
    {
      LOG.Info("Set all settings to default values");

      TailSettings.AlwaysOnTop = false;
      TailSettings.AlwaysScrollToEnd = true;
      TailSettings.LinesRead = 10;
      TailSettings.RestoreWindowSize = false;
      TailSettings.SaveWindowPosition = false;
      TailSettings.ShowNLineAtStart = true;
      TailSettings.WndHeight = -1;
      TailSettings.WndWidth = -1;
      TailSettings.WndXPos = -1;
      TailSettings.WndYPos = -1;
      TailSettings.DefaultThreadPriority = ThreadPriority.Normal;
      TailSettings.DefaultRefreshRate = ETailRefreshRate.Normal;
      TailSettings.ExitWithEscape = false;
      TailSettings.DefaultTimeFormat = ETimeFormat.HHMMD;
      TailSettings.DefaultDateFormat = EDateFormat.DDMMYYYY;
      TailSettings.DefaultBackgroundColor = LogFile.DEFAULT_BACKGROUND_COLOR;
      TailSettings.DefaultForegroundColor = LogFile.DEFAULT_FOREGROUND_COLOR;
      TailSettings.DefaultInactiveForegroundColor = LogFile.DEFAULT_INACTIVE_FOREGROUND_COLOR;
      TailSettings.DefaultInactiveBackgroundColor = LogFile.DEFAULT_INACTIVE_BACKGROUND_COLOR;
      TailSettings.DefaultHighlightForegroundColor = LogFile.DEFAULT_FIND_HIGHLIGHT_FOREGROUND_COLOR;
      TailSettings.DefaultHighlightBackgroundColor = LogFile.DEFAULT_FIND_HIGHLIGHT_BACKGROUND_COLOR;
      TailSettings.DefaultLineNumbersColor = LogFile.DEFAULT_LINE_NUMBERS_COLOR;
      TailSettings.DefaultHighlightColor = LogFile.DEFAULT_HIGHLIGHT_COLOR;
      TailSettings.SearchWndXPos = -1;
      TailSettings.SearchWndYPos = -1;
      TailSettings.DefaultFileSort = EFileSort.Nothing;
      TailSettings.ShowLineNumbers = false;
      TailSettings.AutoUpdate = false;
      TailSettings.SmartWatch = false;
      TailSettings.GroupByCategory = true;
      TailSettings.CurrentWindowStyle = EWindowStyle.ModernBlueWindowStyle;
      TailSettings.CurrentWindowState = System.Windows.WindowState.Normal;
      TailSettings.LogLineLimit = -1;
      TailSettings.DeleteLogFiles = true;

      ResetAlertSettings();
      ResetProxySettings();
      ResetSmtpSettings();
      ResetSmartWatchSettings();

      SaveSettings();
    }

    /// <summary>
    /// Reload all TailForWindows settings
    /// </summary>
    public static void ReloadSettings()
    {
      ConfigurationManager.RefreshSection("appSettings");
    }

    /// <summary>
    /// Get tail settings
    /// </summary>
    public static SettingsData TailSettings { get; } = new SettingsData();

    #region HelperFunctions

    private static void ResetSmartWatchSettings()
    {
      TailSettings.SmartWatchData.FilterByExtension = true;
      TailSettings.SmartWatchData.NewTab = true;
      TailSettings.SmartWatchData.Mode = ESmartWatchMode.Manual;
      TailSettings.SmartWatchData.AutoRun = true;
    }

    /// <summary>
    /// Reset SMTP settings
    /// </summary>
    private static void ResetSmtpSettings()
    {
      TailSettings.AlertSettings.SmtpSettings.FromAddress = string.Empty;
      TailSettings.AlertSettings.SmtpSettings.LoginName = string.Empty;
      TailSettings.AlertSettings.SmtpSettings.Password = string.Empty;
      TailSettings.AlertSettings.SmtpSettings.SmtpPort = -1;
      TailSettings.AlertSettings.SmtpSettings.SmtpServerName = string.Empty;
      TailSettings.AlertSettings.SmtpSettings.Subject = string.Empty;
      TailSettings.AlertSettings.SmtpSettings.SSL = true;
      TailSettings.AlertSettings.SmtpSettings.TLS = false;
    }

    /// <summary>
    /// Reset Proxy settings
    /// </summary>
    private static void ResetProxySettings()
    {
      TailSettings.ProxySettings.Password = string.Empty;
      TailSettings.ProxySettings.ProxyPort = -1;
      TailSettings.ProxySettings.ProxyUrl = string.Empty;
      TailSettings.ProxySettings.UseProxy = false;
      TailSettings.ProxySettings.UseSystemSettings = true;
      TailSettings.ProxySettings.UserName = string.Empty;
    }

    /// <summary>
    /// Reset Alert settings
    /// </summary>
    private static void ResetAlertSettings()
    {
      TailSettings.AlertSettings.BringToFront = true;
      TailSettings.AlertSettings.SendEMail = false;
      TailSettings.AlertSettings.PlaySoundFile = false;
      TailSettings.AlertSettings.SoundFileNameFullPath = LogFile.ALERT_SOUND_FILENAME;
      TailSettings.AlertSettings.EMailAddress = LogFile.ALERT_EMAIL_ADDRESS;
      TailSettings.AlertSettings.PopupWnd = false;
    }

    /// <summary>
    /// Save Alert settings to config
    /// </summary>
    /// <param name="config">Reference to config file</param>
    private static void SaveAlertSettings(Configuration config)
    {
      WriteValueToSetting(config, "Alert.BringToFront", TailSettings.AlertSettings.BringToFront.ToString());
      WriteValueToSetting(config, "Alert.PlaySoundFile", TailSettings.AlertSettings.PlaySoundFile.ToString());
      WriteValueToSetting(config, "Alert.SendEMail", TailSettings.AlertSettings.SendEMail.ToString());
      WriteValueToSetting(config, "Alert.EMailAddress", TailSettings.AlertSettings.EMailAddress);
      WriteValueToSetting(config, "Alert.SoundFile", TailSettings.AlertSettings.SoundFileNameFullPath);
      WriteValueToSetting(config, "Alert.PopupWindow", TailSettings.AlertSettings.PopupWnd.ToString());
    }

    /// <summary>
    /// Savve Proxy settings to config
    /// </summary>
    /// <param name="config">Reference to config file</param>
    private static void SaveProxySettings(Configuration config)
    {
      WriteValueToSetting(config, "Proxy.UserName", TailSettings.ProxySettings.UserName);
      WriteValueToSetting(config, "Proxy.Password", TailSettings.ProxySettings.Password);
      WriteValueToSetting(config, "Proxy.Use", TailSettings.ProxySettings.UseProxy.ToString());
      WriteValueToSetting(config, "Proxy.Port", TailSettings.ProxySettings.ProxyPort.ToString(CultureInfo.InvariantCulture));
      WriteValueToSetting(config, "Proxy.Url", TailSettings.ProxySettings.ProxyUrl);
      WriteValueToSetting(config, "Proxy.UseSystem", TailSettings.ProxySettings.UseSystemSettings.ToString());
    }

    /// <summary>
    /// Save SMTP settings to config
    /// </summary>
    /// <param name="config">Reference to config file</param>
    private static void SaveSmptSettings(Configuration config)
    {
      WriteValueToSetting(config, "Smtp.Server", TailSettings.AlertSettings.SmtpSettings.SmtpServerName);
      WriteValueToSetting(config, "Smtp.Port", TailSettings.AlertSettings.SmtpSettings.SmtpPort.ToString(CultureInfo.InvariantCulture));
      WriteValueToSetting(config, "Smtp.Login", TailSettings.AlertSettings.SmtpSettings.LoginName);
      WriteValueToSetting(config, "Smtp.Password", TailSettings.AlertSettings.SmtpSettings.Password);
      WriteValueToSetting(config, "Smtp.FromEMail", TailSettings.AlertSettings.SmtpSettings.FromAddress);
      WriteValueToSetting(config, "Smtp.Subject", TailSettings.AlertSettings.SmtpSettings.Subject);
      WriteValueToSetting(config, "Smtp.Ssl", TailSettings.AlertSettings.SmtpSettings.SSL.ToString());
      WriteValueToSetting(config, "Smtp.Tls", TailSettings.AlertSettings.SmtpSettings.TLS.ToString());
    }

    private static void SaveSmartWatchSettings(Configuration config)
    {
      WriteValueToSetting(config, "SmartWatch.FilterByExtension", TailSettings.SmartWatchData.FilterByExtension.ToString());
      WriteValueToSetting(config, "SmartWatch.NewTab", TailSettings.SmartWatchData.NewTab.ToString());
      WriteValueToSetting(config, "SmartWatch.Mode", TailSettings.SmartWatchData.Mode.ToString());
      WriteValueToSetting(config, "SmartWatch.AutoRun", TailSettings.SmartWatchData.AutoRun.ToString());
    }

    private static void ReadSmartWatchSettings()
    {
      try
      {
        TailSettings.SmartWatchData.FilterByExtension = GetBoolFromSetting("SmartWatch.FilterByExtension", true);
        TailSettings.SmartWatchData.NewTab = GetBoolFromSetting("SmartWatch.NewTab", true);
        TailSettings.SmartWatchData.Mode = GetSmartWatchMode(GetStringFromSetting("SmartWatch.Mode"));
        TailSettings.SmartWatchData.AutoRun = GetBoolFromSetting("SmartWatch.AutoRun", true);
      }
      catch (ConfigurationErrorsException ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    /// <summary>
    /// Read proxy config settings
    /// </summary>
    private static void ReadProxySettings()
    {
      try
      {
        TailSettings.ProxySettings.UseProxy = GetBoolFromSetting("Proxy.Use");
        TailSettings.ProxySettings.UseSystemSettings = GetBoolFromSetting("Proxy.UseSystem", true);
        TailSettings.ProxySettings.ProxyPort = GetIntFromSetting("Proxy.Port");
        TailSettings.ProxySettings.ProxyUrl = GetStringFromSetting("Proxy.Url");
        TailSettings.ProxySettings.UserName = GetStringFromSetting("Proxy.UserName");
        TailSettings.ProxySettings.Password = GetStringFromSetting("Proxy.Password");
      }
      catch (ConfigurationErrorsException ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    /// <summary>
    /// Read SMTP config settings
    /// </summary>
    private static void ReadSmtpSettings()
    {
      try
      {
        TailSettings.AlertSettings.SmtpSettings.SmtpServerName = GetStringFromSetting("Smtp.Server");
        TailSettings.AlertSettings.SmtpSettings.SmtpPort = GetIntFromSetting("Smtp.Port");
        TailSettings.AlertSettings.SmtpSettings.LoginName = GetStringFromSetting("Smtp.Login");
        TailSettings.AlertSettings.SmtpSettings.Password = GetStringFromSetting("Smtp.Password");

        string strSetting = GetStringFromSetting("Smtp.FromEMail");
        TailSettings.AlertSettings.SmtpSettings.FromAddress = ParseEMailAddress(strSetting) ? strSetting : string.Empty;
        TailSettings.AlertSettings.SmtpSettings.Subject = GetStringFromSetting("Smtp.Subject");
        TailSettings.AlertSettings.SmtpSettings.SSL = GetBoolFromSetting("Smtp.Ssl", true);

        bool boolSetting = GetBoolFromSetting("Smtp.Tls");
        TailSettings.AlertSettings.SmtpSettings.TLS = !TailSettings.AlertSettings.SmtpSettings.SSL && boolSetting;
      }
      catch (ConfigurationErrorsException ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    /// <summary>
    /// Read alert config settings
    /// </summary>
    private static void ReadAlertSettings()
    {
      try
      {
        TailSettings.AlertSettings.BringToFront = GetBoolFromSetting("Alert.BringToFront", true);
        TailSettings.AlertSettings.PlaySoundFile = GetBoolFromSetting("Alert.PlaySoundFile");
        TailSettings.AlertSettings.SendEMail = GetBoolFromSetting("Alert.SendEMail");
        TailSettings.AlertSettings.PopupWnd = GetBoolFromSetting("Alert.PopupWindow");

        ParseSoundFileName(GetStringFromSetting("Alert.SoundFile"));

        string strSetting = GetStringFromSetting("Alert.EMailAddress");
        TailSettings.AlertSettings.EMailAddress = ParseEMailAddress(strSetting) ? strSetting : LogFile.ALERT_EMAIL_ADDRESS;

      }
      catch (ConfigurationErrorsException ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    /// <summary>
    /// Get all Enum ThreadPriorities
    /// </summary>
    /// <param name="s">Reference of thread priority string</param>
    /// <returns>Enum from thread priority</returns>
    public static ThreadPriority GetThreadPriority(string s)
    {
      if (s == null)
        return ThreadPriority.Normal;

      if (Enum.GetNames(typeof(ThreadPriority)).All(priorityName => String.Compare(s.ToLower(), priorityName.ToLower(), StringComparison.Ordinal) != 0))
        return ThreadPriority.Normal;

      Enum.TryParse(s, out ThreadPriority tp);

      return tp;
    }

    private static void ReadThreadPriorityEnum(string s)
    {
      TailSettings.DefaultThreadPriority = GetThreadPriority(s);
    }

    /// <summary>
    /// Get all Enum RefreshRates
    /// </summary>
    /// <param name="s">Reference of refresh rate string</param>
    /// <returns>Enum of ETailRefreshRate</returns>
    public static ETailRefreshRate GetRefreshRate(string s)
    {
      if (string.IsNullOrEmpty(s))
        return ETailRefreshRate.Normal;

      if (Enum.GetNames(typeof(ETailRefreshRate)).All(refreshName => string.Compare(s.ToLower(), refreshName.ToLower(), StringComparison.Ordinal) != 0))
        return ETailRefreshRate.Normal;

      Enum.TryParse(s, out ETailRefreshRate trr);

      return trr;
    }

    /// <summary>
    /// Get all Enum SmartWatch modes
    /// </summary>
    /// <param name="s">Reference of SmartWatch mode string</param>
    /// <returns>Enum of ESmartWatchMode</returns>
    private static ESmartWatchMode GetSmartWatchMode(string s)
    {
      if (string.IsNullOrEmpty(s))
        return ESmartWatchMode.Manual;

      if (Enum.GetNames(typeof(ESmartWatchMode)).All(m => string.Compare(s.ToLower(), m.ToLower(), StringComparison.Ordinal) != 0))
        return ESmartWatchMode.Manual;

      Enum.TryParse(s, out ESmartWatchMode mode);

      return mode;
    }

    /// <summary>
    /// Get current window state from Enum
    /// </summary>
    /// <param name="s">Enum value as string</param>
    /// <returns>Enum of System.Windows.WindowState</returns>
    private static System.Windows.WindowState GetWindowState(string s)
    {
      if (string.IsNullOrEmpty(s))
        return System.Windows.WindowState.Normal;

      if (Enum.GetNames(typeof(System.Windows.WindowState)).All(w => string.Compare(s.ToLower(), w.ToLower(), StringComparison.Ordinal) != 0))
        return System.Windows.WindowState.Normal;

      Enum.TryParse(s, out System.Windows.WindowState wndState);

      return wndState;
    }

    /// <summary>
    /// Get current window style from Enum
    /// </summary>
    /// <param name="s">Enum value as string</param>
    /// <returns>Enum of EWindowStyle</returns>
    private static EWindowStyle GetWindowStyle(string s)
    {
      if (string.IsNullOrEmpty(s))
        return EWindowStyle.ModernBlueWindowStyle;

      if (Enum.GetNames(typeof(EWindowStyle)).All(w => string.Compare(s.ToLower(), w.ToLower(), StringComparison.Ordinal) != 0))
        return EWindowStyle.ModernBlueWindowStyle;

      Enum.TryParse(s, out EWindowStyle wnd);

      return wnd;
    }

    private static void ReadThreadRefreshRateEnum(string s)
    {
      TailSettings.DefaultRefreshRate = GetRefreshRate(s);
    }

    private static void ReadTimeFormatEnum(string s)
    {
      if (s != null)
      {
        foreach (string timeFormat in Enum.GetNames(typeof(ETimeFormat)))
        {
          if (String.Compare(s, timeFormat, StringComparison.Ordinal) == 0)
          {
            Enum.TryParse(s, out ETimeFormat tf);
            TailSettings.DefaultTimeFormat = tf;
            break;
          }

          TailSettings.DefaultTimeFormat = ETimeFormat.HHMMD;
        }
      }
      else
      {
        TailSettings.DefaultTimeFormat = ETimeFormat.HHMMD;
      }
    }

    private static void ReadDateFormatEnum(string s)
    {
      if (s != null)
      {
        foreach (string dateFormat in Enum.GetNames(typeof(EDateFormat)))
        {
          if (String.Compare(s, dateFormat, StringComparison.Ordinal) == 0)
          {
            Enum.TryParse(s, out EDateFormat df);
            TailSettings.DefaultDateFormat = df;
            break;
          }

          TailSettings.DefaultDateFormat = EDateFormat.DDMMYYYY;
        }
      }
      else
      {
        TailSettings.DefaultDateFormat = EDateFormat.DDMMYYYY;
      }
    }

    private static void ReadFileSortEnum(string s)
    {
      if (s != null)
      {
        foreach (string fileSort in Enum.GetNames(typeof(EFileSort)))
        {
          if (String.Compare(s, fileSort, StringComparison.Ordinal) == 0)
          {
            Enum.TryParse(s, out EFileSort fs);
            TailSettings.DefaultFileSort = fs;
            break;
          }

          TailSettings.DefaultFileSort = EFileSort.Nothing;
        }
      }
      else
      {
        TailSettings.DefaultFileSort = EFileSort.Nothing;
      }
    }

    private static void CheckHexColorValue(string s, ETailLogColorTypes cType)
    {
      Match match6 = Regex.Match(s, @"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$");
      Match match8 = Regex.Match(s, @"^(#)?([0-9a-fA-F]{4})([0-9a-fA-F]{4})?$");
      bool matched = match6.Success | match8.Success;

      switch (cType)
      {
        case ETailLogColorTypes.BackgroundColor:

          TailSettings.DefaultBackgroundColor = !matched ? LogFile.DEFAULT_BACKGROUND_COLOR : s;
          break;

        case ETailLogColorTypes.ForegroundColor:

          TailSettings.DefaultForegroundColor = !matched ? LogFile.DEFAULT_FOREGROUND_COLOR : s;
          break;

        case ETailLogColorTypes.InactiveForegroundColor:

          TailSettings.DefaultInactiveForegroundColor = !matched ? LogFile.DEFAULT_INACTIVE_FOREGROUND_COLOR : s;
          break;

        case ETailLogColorTypes.InactiveBackgroundColor:

          TailSettings.DefaultInactiveBackgroundColor = !matched ? LogFile.DEFAULT_INACTIVE_BACKGROUND_COLOR : s;
          break;

        case ETailLogColorTypes.FindHighlightForegroundColor:

          TailSettings.DefaultHighlightForegroundColor = !matched ? LogFile.DEFAULT_FIND_HIGHLIGHT_FOREGROUND_COLOR : s;
          break;

        case ETailLogColorTypes.FindHighlightBackgroundColor:

          TailSettings.DefaultHighlightBackgroundColor = !matched ? LogFile.DEFAULT_FIND_HIGHLIGHT_BACKGROUND_COLOR : s;
          break;

        case ETailLogColorTypes.LineNumbersColor:

          TailSettings.DefaultLineNumbersColor = !matched ? LogFile.DEFAULT_LINE_NUMBERS_COLOR : s;
          break;

        case ETailLogColorTypes.HighlightColor:

          TailSettings.DefaultHighlightColor = !matched ? LogFile.DEFAULT_HIGHLIGHT_COLOR : s;
          break;

        default:

          throw new NotImplementedException("not supported color type");
      }
    }

    private static void SetSearchWindowPosition(string s, bool xPos = true)
    {
      int pos;

      if (xPos)
      {
        if (!int.TryParse(s, out pos))
          pos = -1;

        TailSettings.SearchWndXPos = pos;
      }
      else
      {
        if (!int.TryParse(s, out pos))
          pos = -1;

        TailSettings.SearchWndYPos = pos;
      }
    }

    /// <summary>
    /// Check if a E-Mail address is valid
    /// </summary>
    /// <param name="emailAddress">E-Mail address</param>
    /// <returns>true if is correct, false if is not valid</returns>
    public static bool ParseEMailAddress(string emailAddress)
    {
      Regex regex = new Regex(LogFile.REGEX_EMAIL_ADDRESS);

      return regex.IsMatch(emailAddress);
    }

    private static void ParseSoundFileName(string fullPath)
    {
      string extension = System.IO.Path.GetExtension(fullPath);
      string fileName = System.IO.Path.GetFileName(fullPath);
      char[] reserved = System.IO.Path.GetInvalidFileNameChars();

      if (fileName != null && fileName.Length < 3)
      {
        TailSettings.AlertSettings.SoundFileNameFullPath = LogFile.ALERT_SOUND_FILENAME;
        return;
      }

      if (fileName != null && fileName.Length > 128)
      {
        TailSettings.AlertSettings.SoundFileNameFullPath = LogFile.ALERT_SOUND_FILENAME;
        return;
      }

      if (reserved.Any(c => fileName != null && fileName.Contains(c.ToString(CultureInfo.InvariantCulture))))
      {
        TailSettings.AlertSettings.SoundFileNameFullPath = LogFile.ALERT_SOUND_FILENAME;
        return;
      }

      Regex regex = new Regex(LogFile.REGEX_SOUNDFILE_EXTENSION);

      if (extension != null && !regex.IsMatch(extension))
      {
        TailSettings.AlertSettings.SoundFileNameFullPath = LogFile.ALERT_SOUND_FILENAME;
        return;
      }

      TailSettings.AlertSettings.SoundFileNameFullPath = fullPath;
    }

    /// <summary>
    /// If property does not exists, add it
    /// </summary>
    /// <param name="key">Key string</param>
    /// <param name="value">Default value</param>
    private static void AddNewProperties_IntoConfigFile(string key, string value)
    {
      try
      {
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        config.AppSettings.Settings.Add(key, value);

        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("appSettings");
      }
      catch (ConfigurationErrorsException ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }
    
    private void WriteValueToSetting(Configuration config, string setting, string value)
    {
      Arg.NotNull(config, "Configuration");
      config.AppSettings.Settings[setting].Value = value;
    }

    private static string GetStringFromSetting(string setting)
    {
      if (string.IsNullOrWhiteSpace(setting))
        return string.Empty;

      return ConfigurationManager.AppSettings[setting];
    }

    private static int GetIntFromSetting(string setting, int defaultValue = -1)
    {
      if (!int.TryParse(ConfigurationManager.AppSettings[setting], out int intSetting))
        intSetting = defaultValue;

      return intSetting;
    }

    private static double GetDoubleFromSetting(string setting, double defaultValue = -1)
    {
      if (!double.TryParse(ConfigurationManager.AppSettings[setting], out double doubleSetting))
        doubleSetting = defaultValue;

      return doubleSetting;
    }

    private static bool GetBoolFromSetting(string setting, bool defaultValue = false)
    {
      if (!bool.TryParse(ConfigurationManager.AppSettings[setting], out bool boolSetting))
        boolSetting = defaultValue;

      return boolSetting;
    }

    #endregion
  }
}
