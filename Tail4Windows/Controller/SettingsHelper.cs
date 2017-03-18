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
    private static readonly SettingsData tailSettings = new SettingsData();


    /// <summary>
    /// Read app settings
    /// </summary>
    public static void ReadSettings()
    {
      LOG.Trace("Read TailForWindows settings");

      try
      {
        if(!int.TryParse(ConfigurationManager.AppSettings["LinesRead"], out int iHelper))
          iHelper = 10;
        TailSettings.LinesRead = iHelper;

        if(!bool.TryParse(ConfigurationManager.AppSettings["AlwaysOnTop"], out bool bHelper))
          bHelper = false;
        TailSettings.AlwaysOnTop = bHelper;

        if(!bool.TryParse(ConfigurationManager.AppSettings["ShowNLineAtStart"], out bHelper))
          bHelper = true;
        TailSettings.ShowNLineAtStart = bHelper;

        if(!bool.TryParse(ConfigurationManager.AppSettings["ShowLineNumbers"], out bHelper))
          bHelper = false;
        TailSettings.ShowLineNumbers = bHelper;

        if(!bool.TryParse(ConfigurationManager.AppSettings["AlwaysScrollToEnd"], out bHelper))
          bHelper = true;
        TailSettings.AlwaysScrollToEnd = bHelper;

        if(!bool.TryParse(ConfigurationManager.AppSettings["RestoreWindowSize"], out bHelper))
          bHelper = false;
        TailSettings.RestoreWindowSize = bHelper;

        if(!double.TryParse(ConfigurationManager.AppSettings["wndWidth"], out double dHelper))
          dHelper = -1;
        TailSettings.WndWidth = dHelper;

        if(!double.TryParse(ConfigurationManager.AppSettings["wndHeight"], out dHelper))
          dHelper = -1;
        TailSettings.WndHeight = dHelper;

        if(!bool.TryParse(ConfigurationManager.AppSettings["SaveWindowPosition"], out bHelper))
          bHelper = false;
        TailSettings.SaveWindowPosition = bHelper;

        if(!double.TryParse(ConfigurationManager.AppSettings["wndXPos"], out dHelper))
          dHelper = -1;
        TailSettings.WndXPos = dHelper;

        if(!double.TryParse(ConfigurationManager.AppSettings["wndYPos"], out dHelper))
          dHelper = -1;
        TailSettings.WndYPos = dHelper;

        if(ConfigurationManager.AppSettings["WindowState"] == null)
          AddNewProperties_IntoConfigFile("WindowState", System.Windows.WindowState.Normal.ToString());

        string sHelper = ConfigurationManager.AppSettings["WindowState"];
        TailSettings.CurrentWindowState = GetWindowState(sHelper);

        sHelper = ConfigurationManager.AppSettings["DefaultThreadPriority"];
        ReadThreadPriorityEnum(sHelper);

        sHelper = ConfigurationManager.AppSettings["DefaultRefreshRate"];
        ReadThreadRefreshRateEnum(sHelper);

        if(!bool.TryParse(ConfigurationManager.AppSettings["ExitWithEsc"], out bHelper))
          bHelper = false;
        TailSettings.ExitWithEscape = bHelper;

        if(!bool.TryParse(ConfigurationManager.AppSettings["AutoUpdate"], out bHelper))
          bHelper = false;
        TailSettings.AutoUpdate = bHelper;

        if(ConfigurationManager.AppSettings["SmartWatch"] == null)
          AddNewProperties_IntoConfigFile("SmartWatch", "false");

        if(!bool.TryParse(ConfigurationManager.AppSettings["SmartWatch"], out bHelper))
          bHelper = false;
        TailSettings.SmartWatch = bHelper;

        if(ConfigurationManager.AppSettings["GroupByCategory"] == null)
          AddNewProperties_IntoConfigFile("GroupByCategory", "true");

        if(!bool.TryParse(ConfigurationManager.AppSettings["GroupByCategory"], out bHelper))
          bHelper = true;
        TailSettings.GroupByCategory = bHelper;

        if(ConfigurationManager.AppSettings["CurrentWindowStyle"] == null)
          AddNewProperties_IntoConfigFile("CurrentWindowStyle", EWindowStyle.ModernBlueWindowStyle.ToString());

        sHelper = ConfigurationManager.AppSettings["CurrentWindowStyle"];
        TailSettings.CurrentWindowStyle = GetWindowStyle(sHelper);

        sHelper = ConfigurationManager.AppSettings["TimeFormat"];
        ReadTimeFormatEnum(sHelper);

        sHelper = ConfigurationManager.AppSettings["DateFormat"];
        ReadDateFormatEnum(sHelper);

        sHelper = ConfigurationManager.AppSettings["ForegroundColor"];
        CheckHexColorValue(sHelper, ETailLogColorTypes.ForegroundColor);

        sHelper = ConfigurationManager.AppSettings["BackgroundColor"];
        CheckHexColorValue(sHelper, ETailLogColorTypes.BackgroundColor);

        sHelper = ConfigurationManager.AppSettings["InactiveForegroundColor"];
        CheckHexColorValue(sHelper, ETailLogColorTypes.InactiveForegroundColor);

        sHelper = ConfigurationManager.AppSettings["InactiveBackgroundColor"];
        CheckHexColorValue(sHelper, ETailLogColorTypes.InactiveBackgroundColor);

        sHelper = ConfigurationManager.AppSettings["FindHighlightForegroundColor"];
        CheckHexColorValue(sHelper, ETailLogColorTypes.FindHighlightForegroundColor);

        sHelper = ConfigurationManager.AppSettings["FindHighlightBackgroundColor"];
        CheckHexColorValue(sHelper, ETailLogColorTypes.FindHighlightBackgroundColor);

        sHelper = ConfigurationManager.AppSettings["LineNumbersColor"];
        CheckHexColorValue(sHelper, ETailLogColorTypes.LineNumbersColor);

        sHelper = ConfigurationManager.AppSettings["HighlightColor"];
        CheckHexColorValue(sHelper, ETailLogColorTypes.HighlightColor);

        sHelper = ConfigurationManager.AppSettings["SearchwndXPos"];
        SetSearchWindowPosition(sHelper);

        sHelper = ConfigurationManager.AppSettings["SearchwndYPos"];
        SetSearchWindowPosition(sHelper, false);

        sHelper = ConfigurationManager.AppSettings["FileManagerSort"];
        ReadFileSortEnum(sHelper);

        ReadAlertSettings();
        ReadProxySettings();
        ReadSmtpSettings();
        ReadSmartWatchSettings();
      }
      catch(ConfigurationErrorsException ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    /// <summary>
    /// Save app settings
    /// </summary>
    public static void SaveSettings()
    {
      try
      {
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        if(config.AppSettings.Settings.Count <= 0)
          return;

        config.AppSettings.Settings["LinesRead"].Value = TailSettings.LinesRead.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings["AlwaysOnTop"].Value = TailSettings.AlwaysOnTop.ToString();
        config.AppSettings.Settings["ShowNLineAtStart"].Value = TailSettings.ShowNLineAtStart.ToString();
        config.AppSettings.Settings["AlwaysScrollToEnd"].Value = TailSettings.AlwaysScrollToEnd.ToString();
        config.AppSettings.Settings["RestoreWindowSize"].Value = TailSettings.RestoreWindowSize.ToString();
        config.AppSettings.Settings["wndWidth"].Value = TailSettings.WndWidth.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings["wndHeight"].Value = TailSettings.WndHeight.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings["SaveWindowPosition"].Value = TailSettings.RestoreWindowSize.ToString();
        config.AppSettings.Settings["wndXPos"].Value = TailSettings.WndXPos.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings["wndYPos"].Value = TailSettings.WndYPos.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings["DefaultThreadPriority"].Value = TailSettings.DefaultThreadPriority.ToString();
        config.AppSettings.Settings["DefaultRefreshRate"].Value = TailSettings.DefaultRefreshRate.ToString();
        config.AppSettings.Settings["ExitWithEsc"].Value = TailSettings.ExitWithEscape.ToString();
        config.AppSettings.Settings["TimeFormat"].Value = TailSettings.DefaultTimeFormat.ToString();
        config.AppSettings.Settings["DateFormat"].Value = TailSettings.DefaultDateFormat.ToString();
        config.AppSettings.Settings["ForegroundColor"].Value = TailSettings.DefaultForegroundColor;
        config.AppSettings.Settings["BackgroundColor"].Value = TailSettings.DefaultBackgroundColor;
        config.AppSettings.Settings["InactiveForegroundColor"].Value = TailSettings.DefaultInactiveForegroundColor;
        config.AppSettings.Settings["InactiveBackgroundColor"].Value = TailSettings.DefaultInactiveBackgroundColor;
        config.AppSettings.Settings["FindHighlightForegroundColor"].Value = TailSettings.DefaultHighlightForegroundColor;
        config.AppSettings.Settings["FindHighlightBackgroundColor"].Value = TailSettings.DefaultHighlightBackgroundColor;
        config.AppSettings.Settings["SearchwndXPos"].Value = TailSettings.SearchWndXPos.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings["SearchwndYPos"].Value = TailSettings.SearchWndYPos.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings["FileManagerSort"].Value = TailSettings.DefaultFileSort.ToString();
        config.AppSettings.Settings["ShowLineNumbers"].Value = TailSettings.ShowLineNumbers.ToString();
        config.AppSettings.Settings["LineNumbersColor"].Value = TailSettings.DefaultLineNumbersColor;
        config.AppSettings.Settings["HighlightColor"].Value = TailSettings.DefaultHighlightColor;
        config.AppSettings.Settings["AutoUpdate"].Value = TailSettings.AutoUpdate.ToString();
        config.AppSettings.Settings["SmartWatch"].Value = TailSettings.SmartWatch.ToString();
        config.AppSettings.Settings["GroupByCategory"].Value = TailSettings.GroupByCategory.ToString();
        config.AppSettings.Settings["CurrentWindowStyle"].Value = TailSettings.CurrentWindowStyle.ToString();
        config.AppSettings.Settings["WindowState"].Value = TailSettings.CurrentWindowState.ToString();

        SaveAlertSettings(config);
        SaveProxySettings(config);
        SaveSmptSettings(config);
        SaveSmartWatchSettings(config);

        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("appSettings");
      }
      catch(ConfigurationErrorsException ex)
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

        if(config.AppSettings.Settings.Count <= 0)
          return;

        config.AppSettings.Settings["SearchwndXPos"].Value = TailSettings.SearchWndXPos.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings["SearchwndYPos"].Value = TailSettings.SearchWndYPos.ToString(CultureInfo.InvariantCulture);

        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("appSettings");
      }
      catch(ConfigurationErrorsException ex)
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
    public static SettingsData TailSettings => (tailSettings);

    #region HelperFunctions

    private static void ResetSmartWatchSettings()
    {
      TailSettings.SmartWatchData.FilterByExtension = true;
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
      config.AppSettings.Settings["Alert.BringToFront"].Value = TailSettings.AlertSettings.BringToFront.ToString();
      config.AppSettings.Settings["Alert.PlaySoundFile"].Value = TailSettings.AlertSettings.PlaySoundFile.ToString();
      config.AppSettings.Settings["Alert.SendEMail"].Value = TailSettings.AlertSettings.SendEMail.ToString();
      config.AppSettings.Settings["Alert.EMailAddress"].Value = TailSettings.AlertSettings.EMailAddress;
      config.AppSettings.Settings["Alert.SoundFile"].Value = TailSettings.AlertSettings.SoundFileNameFullPath;
      config.AppSettings.Settings["Alert.PopupWindow"].Value = TailSettings.AlertSettings.PopupWnd.ToString();
    }

    /// <summary>
    /// Savve Proxy settings to config
    /// </summary>
    /// <param name="config">Reference to config file</param>
    private static void SaveProxySettings(Configuration config)
    {
      config.AppSettings.Settings["Proxy.UserName"].Value = TailSettings.ProxySettings.UserName;
      config.AppSettings.Settings["Proxy.Password"].Value = TailSettings.ProxySettings.Password;
      config.AppSettings.Settings["Proxy.Use"].Value = TailSettings.ProxySettings.UseProxy.ToString();
      config.AppSettings.Settings["Proxy.Port"].Value = TailSettings.ProxySettings.ProxyPort.ToString(CultureInfo.InvariantCulture);
      config.AppSettings.Settings["Proxy.Url"].Value = TailSettings.ProxySettings.ProxyUrl;
      config.AppSettings.Settings["Proxy.UseSystem"].Value = TailSettings.ProxySettings.UseSystemSettings.ToString();
    }

    /// <summary>
    /// Save SMTP settings to config
    /// </summary>
    /// <param name="config">Reference to config file</param>
    private static void SaveSmptSettings(Configuration config)
    {
      config.AppSettings.Settings["Smtp.Server"].Value = TailSettings.AlertSettings.SmtpSettings.SmtpServerName;
      config.AppSettings.Settings["Smtp.Port"].Value = TailSettings.AlertSettings.SmtpSettings.SmtpPort.ToString(CultureInfo.InvariantCulture);
      config.AppSettings.Settings["Smtp.Login"].Value = TailSettings.AlertSettings.SmtpSettings.LoginName;
      config.AppSettings.Settings["Smtp.Password"].Value = TailSettings.AlertSettings.SmtpSettings.Password;
      config.AppSettings.Settings["Smtp.FromEMail"].Value = TailSettings.AlertSettings.SmtpSettings.FromAddress;
      config.AppSettings.Settings["Smtp.Subject"].Value = TailSettings.AlertSettings.SmtpSettings.Subject;
      config.AppSettings.Settings["Smtp.Ssl"].Value = TailSettings.AlertSettings.SmtpSettings.SSL.ToString();
      config.AppSettings.Settings["Smtp.Tls"].Value = TailSettings.AlertSettings.SmtpSettings.TLS.ToString();
    }

    private static void SaveSmartWatchSettings(Configuration config)
    {
      config.AppSettings.Settings["SmartWatch.FilterByExtension"].Value = TailSettings.SmartWatchData.FilterByExtension.ToString();
    }

    private static void ReadSmartWatchSettings()
    {
      try
      {
        if(ConfigurationManager.AppSettings["SmartWatch.FilterByExtension"] == null)
          AddNewProperties_IntoConfigFile("SmartWatch.FilterByExtension", "True");

        if(!bool.TryParse(ConfigurationManager.AppSettings["SmartWatch.FilterByExtension"], out bool bHelper))
          bHelper = true;
        TailSettings.SmartWatchData.FilterByExtension = bHelper;
      }
      catch(ConfigurationErrorsException ex)
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
        if(!bool.TryParse(ConfigurationManager.AppSettings["Proxy.Use"], out bool bHelper))
          bHelper = false;
        TailSettings.ProxySettings.UseProxy = bHelper;

        if(!bool.TryParse(ConfigurationManager.AppSettings["Proxy.UseSystem"], out bHelper))
          bHelper = true;
        TailSettings.ProxySettings.UseSystemSettings = bHelper;

        if(!int.TryParse(ConfigurationManager.AppSettings["Proxy.Port"], out int port))
          port = -1;
        TailSettings.ProxySettings.ProxyPort = port;

        string sHelper = ConfigurationManager.AppSettings["Proxy.Url"];
        TailSettings.ProxySettings.ProxyUrl = sHelper;

        sHelper = ConfigurationManager.AppSettings["Proxy.UserName"];
        TailSettings.ProxySettings.UserName = sHelper;

        sHelper = ConfigurationManager.AppSettings["Proxy.Password"];
        TailSettings.ProxySettings.Password = sHelper;
      }
      catch(ConfigurationErrorsException ex)
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
        string sHelper = ConfigurationManager.AppSettings["Smtp.Server"];
        TailSettings.AlertSettings.SmtpSettings.SmtpServerName = sHelper;

        if(!int.TryParse(ConfigurationManager.AppSettings["Smtp.Port"], out int port))
          port = -1;
        TailSettings.AlertSettings.SmtpSettings.SmtpPort = port;

        sHelper = ConfigurationManager.AppSettings["Smtp.Login"];
        TailSettings.AlertSettings.SmtpSettings.LoginName = sHelper;

        sHelper = ConfigurationManager.AppSettings["Smtp.Password"];
        TailSettings.AlertSettings.SmtpSettings.Password = sHelper;

        sHelper = ConfigurationManager.AppSettings["Smtp.FromEMail"];
        TailSettings.AlertSettings.SmtpSettings.FromAddress = ParseEMailAddress(sHelper) ? sHelper : string.Empty;

        sHelper = ConfigurationManager.AppSettings["Smtp.Subject"];
        TailSettings.AlertSettings.SmtpSettings.Subject = sHelper;

        if(!bool.TryParse(ConfigurationManager.AppSettings["Smtp.Ssl"], out bool bHelper))
          bHelper = true;
        TailSettings.AlertSettings.SmtpSettings.SSL = bHelper;

        if(!bool.TryParse(ConfigurationManager.AppSettings["Smtp.Tls"], out bHelper))
          bHelper = false;
        if(TailSettings.AlertSettings.SmtpSettings.SSL)
          bHelper = false;
        TailSettings.AlertSettings.SmtpSettings.TLS = bHelper;
      }
      catch(ConfigurationErrorsException ex)
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
        if(!bool.TryParse(ConfigurationManager.AppSettings["Alert.BringToFront"], out bool bHelper))
          bHelper = true;
        TailSettings.AlertSettings.BringToFront = bHelper;

        if(!bool.TryParse(ConfigurationManager.AppSettings["Alert.PlaySoundFile"], out bHelper))
          bHelper = false;
        TailSettings.AlertSettings.PlaySoundFile = bHelper;

        if(!bool.TryParse(ConfigurationManager.AppSettings["Alert.SendEMail"], out bHelper))
          bHelper = false;
        TailSettings.AlertSettings.SendEMail = bHelper;

        string sHelper = ConfigurationManager.AppSettings["Alert.SoundFile"];
        ParseSoundFileName(sHelper);

        sHelper = ConfigurationManager.AppSettings["Alert.EMailAddress"];

        TailSettings.AlertSettings.EMailAddress = ParseEMailAddress(sHelper) ? sHelper : LogFile.ALERT_EMAIL_ADDRESS;

        if(ConfigurationManager.AppSettings["Alert.PopupWindow"] == null)
          AddNewProperties_IntoConfigFile("Alert.PopupWindow", "false");

        if(!bool.TryParse(ConfigurationManager.AppSettings["Alert.PopupWindow"], out bHelper))
          bHelper = false;
        TailSettings.AlertSettings.PopupWnd = bHelper;
      }
      catch(ConfigurationErrorsException ex)
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
      if(s == null)
        return (ThreadPriority.Normal);

      if(Enum.GetNames(typeof(ThreadPriority)).All(priorityName => String.Compare(s.ToLower(), priorityName.ToLower(), StringComparison.Ordinal) != 0))
        return (ThreadPriority.Normal);

      Enum.TryParse(s, out ThreadPriority tp);

      return (tp);
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
      if(string.IsNullOrEmpty(s))
        return (ETailRefreshRate.Normal);

      if(Enum.GetNames(typeof(ETailRefreshRate)).All(refreshName => string.Compare(s.ToLower(), refreshName.ToLower(), StringComparison.Ordinal) != 0))
        return (ETailRefreshRate.Normal);

      Enum.TryParse(s, out ETailRefreshRate trr);

      return (trr);
    }

    /// <summary>
    /// Get current window state from Enum
    /// </summary>
    /// <param name="s">Enum value as string</param>
    /// <returns>Enum of System.Windows.WindowState</returns>
    public static System.Windows.WindowState GetWindowState(string s)
    {
      if(string.IsNullOrEmpty(s))
        return (System.Windows.WindowState.Normal);

      if(Enum.GetNames(typeof(System.Windows.WindowState)).All(w => string.Compare(s.ToLower(), w.ToLower(), StringComparison.Ordinal) != 0))
        return (System.Windows.WindowState.Normal);

      Enum.TryParse(s, out System.Windows.WindowState wndState);

      return (wndState);
    }

    /// <summary>
    /// Get current window style from Enum
    /// </summary>
    /// <param name="s">Enum value as string</param>
    /// <returns>Enum of EWindowStyle</returns>
    public static EWindowStyle GetWindowStyle(string s)
    {
      if(string.IsNullOrEmpty(s))
        return (EWindowStyle.ModernBlueWindowStyle);

      if(Enum.GetNames(typeof(EWindowStyle)).All(w => string.Compare(s.ToLower(), w.ToLower(), StringComparison.Ordinal) != 0))
        return (EWindowStyle.ModernBlueWindowStyle);

      Enum.TryParse(s, out EWindowStyle wnd);

      return (wnd);
    }

    private static void ReadThreadRefreshRateEnum(string s)
    {
      TailSettings.DefaultRefreshRate = GetRefreshRate(s);
    }

    private static void ReadTimeFormatEnum(string s)
    {
      if(s != null)
      {
        foreach(string timeFormat in Enum.GetNames(typeof(ETimeFormat)))
        {
          if(String.Compare(s, timeFormat, StringComparison.Ordinal) == 0)
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
      if(s != null)
      {
        foreach(string dateFormat in Enum.GetNames(typeof(EDateFormat)))
        {
          if(String.Compare(s, dateFormat, StringComparison.Ordinal) == 0)
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
      if(s != null)
      {
        foreach(string fileSort in Enum.GetNames(typeof(EFileSort)))
        {
          if(String.Compare(s, fileSort, StringComparison.Ordinal) == 0)
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

      switch(cType)
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

      if(xPos)
      {
        if(!int.TryParse(s, out pos))
          pos = -1;

        TailSettings.SearchWndXPos = pos;
      }
      else
      {
        if(!int.TryParse(s, out pos))
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

      return (regex.IsMatch(emailAddress));
    }

    private static void ParseSoundFileName(string fullPath)
    {
      string extension = System.IO.Path.GetExtension(fullPath);
      string fileName = System.IO.Path.GetFileName(fullPath);
      char[] reserved = System.IO.Path.GetInvalidFileNameChars();

      if(fileName != null && fileName.Length < 3)
      {
        TailSettings.AlertSettings.SoundFileNameFullPath = LogFile.ALERT_SOUND_FILENAME;
        return;
      }

      if(fileName != null && fileName.Length > 128)
      {
        TailSettings.AlertSettings.SoundFileNameFullPath = LogFile.ALERT_SOUND_FILENAME;
        return;
      }

      if(reserved.Any(c => fileName != null && fileName.Contains(c.ToString(CultureInfo.InvariantCulture))))
      {
        TailSettings.AlertSettings.SoundFileNameFullPath = LogFile.ALERT_SOUND_FILENAME;
        return;
      }

      Regex regex = new Regex(LogFile.REGEX_SOUNDFILE_EXTENSION);

      if(extension != null && !regex.IsMatch(extension))
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
      catch(ConfigurationErrorsException ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    #endregion
  }
}
