using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using TailForWin.Data;
using TailForWin.Data.Enums;
using TailForWin.Utils;


namespace TailForWin.Controller
{
  public class SettingsHelper
  {
    private static readonly SettingsData tailSettings = new SettingsData();


    /// <summary>
    /// Read app settings
    /// </summary>
    public static void ReadSettings()
    {
      try
      {
        int iHelper;
        double dHelper;
        bool bHelper;

        if (!int.TryParse(ConfigurationManager.AppSettings["LinesRead"], out iHelper))
          iHelper = 10;
        TailSettings.LinesRead = iHelper;

        if (!bool.TryParse(ConfigurationManager.AppSettings["AlwaysOnTop"], out bHelper))
          bHelper = false;
        TailSettings.AlwaysOnTop = bHelper;

        if (!bool.TryParse(ConfigurationManager.AppSettings["ShowNLineAtStart"], out bHelper))
          bHelper = true;
        TailSettings.ShowNLineAtStart = bHelper;

        if (!bool.TryParse(ConfigurationManager.AppSettings["ShowLineNumbers"], out bHelper))
          bHelper = false;
        TailSettings.ShowLineNumbers = bHelper;

        if (!bool.TryParse(ConfigurationManager.AppSettings["AlwaysScrollToEnd"], out bHelper))
          bHelper = true;
        TailSettings.AlwaysScrollToEnd = bHelper;

        if (!bool.TryParse(ConfigurationManager.AppSettings["RestoreWindowSize"], out bHelper))
          bHelper = false;
        TailSettings.RestoreWindowSize = bHelper;

        if (!double.TryParse(ConfigurationManager.AppSettings["wndWidth"], out dHelper))
          dHelper = -1;
        TailSettings.WndWidth = dHelper;

        if (!double.TryParse(ConfigurationManager.AppSettings["wndHeight"], out dHelper))
          dHelper = -1;
        TailSettings.WndHeight = dHelper;

        if (!bool.TryParse(ConfigurationManager.AppSettings["SaveWindowPosition"], out bHelper))
          bHelper = false;
        TailSettings.SaveWindowPosition = bHelper;

        if (!double.TryParse(ConfigurationManager.AppSettings["wndXPos"], out dHelper))
          dHelper = -1;
        TailSettings.WndXPos = dHelper;

        if (!double.TryParse(ConfigurationManager.AppSettings["wndYPos"], out dHelper))
          dHelper = -1;
        TailSettings.WndYPos = dHelper;

        string sHelper = ConfigurationManager.AppSettings["defaultThreadPriority"];
        ReadThreadPriorityEnum(sHelper);

        sHelper = ConfigurationManager.AppSettings["defaultRefreshRate"];
        ReadThreadRefreshRateEnum(sHelper);

        if (!bool.TryParse(ConfigurationManager.AppSettings["ExitWithEsc"], out bHelper))
          bHelper = false;
        TailSettings.ExitWithEscape = bHelper;

        if (!bool.TryParse(ConfigurationManager.AppSettings["AutoUpdate"], out bHelper))
          bHelper = false;
        TailSettings.AutoUpdate = bHelper;

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
      }
      catch (ConfigurationErrorsException ex)
      {
        ErrorLog.WriteLog(ErrorFlags.Error, "SettingsHelper", string.Format("ReadSettings exception {0}", ex));
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

        if (config.AppSettings.Settings.Count <= 0)
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
        config.AppSettings.Settings["defaultThreadPriority"].Value = TailSettings.DefaultThreadPriority.ToString();
        config.AppSettings.Settings["defaultRefreshRate"].Value = TailSettings.DefaultRefreshRate.ToString();
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

        SaveAlertSettings(config);
        SaveProxySettings(config);
        SaveSmptSettings(config);

        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("appSettings");
      }
      catch (ConfigurationErrorsException ex)
      {
        ErrorLog.WriteLog(ErrorFlags.Error, "SettingsHelper", string.Format("SaveSettings exception {0}", ex));
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

        config.AppSettings.Settings["SearchwndXPos"].Value = TailSettings.SearchWndXPos.ToString(CultureInfo.InvariantCulture);
        config.AppSettings.Settings["SearchwndYPos"].Value = TailSettings.SearchWndYPos.ToString(CultureInfo.InvariantCulture);

        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("appSettings");
      }
      catch (ConfigurationErrorsException ex)
      {
        ErrorLog.WriteLog(ErrorFlags.Error, "SettingsHelper", string.Format("{1}, exception {0}", ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
      }
    }

    /// <summary>
    /// Set app settings to default parameters
    /// </summary>
    public static void SetToDefault()
    {
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

      ResetAlertSettings();
      ResetProxySettings();
      ResetSmtpSettings();

      SaveSettings();
    }

    public static void ReloadSettings()
    {
      ConfigurationManager.RefreshSection("appSettings");
    }

    /// <summary>
    /// Get tail settings
    /// </summary>
    public static SettingsData TailSettings
    {
      get
      {
        return (tailSettings);
      }
    }

    #region HelperFunctions

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

    /// <summary>
    /// Read proxy config settings
    /// </summary>
    private static void ReadProxySettings()
    {
      bool bHelper;

      if (!bool.TryParse(ConfigurationManager.AppSettings["Proxy.Use"], out bHelper))
        bHelper = false;
      TailSettings.ProxySettings.UseProxy = bHelper;

      if (!bool.TryParse(ConfigurationManager.AppSettings["Proxy.UseSystem"], out bHelper))
        bHelper = true;
      TailSettings.ProxySettings.UseSystemSettings = bHelper;

      int port;

      if (!int.TryParse(ConfigurationManager.AppSettings["Proxy.Port"], out port))
        port = -1;
      TailSettings.ProxySettings.ProxyPort = port;

      string sHelper = ConfigurationManager.AppSettings["Proxy.Url"];
      TailSettings.ProxySettings.ProxyUrl = sHelper;

      sHelper = ConfigurationManager.AppSettings["Proxy.UserName"];
      TailSettings.ProxySettings.UserName = sHelper;

      sHelper = ConfigurationManager.AppSettings["Proxy.Password"];
      TailSettings.ProxySettings.Password = sHelper;
    }

    /// <summary>
    /// Read SMTP config settings
    /// </summary>
    private static void ReadSmtpSettings()
    {
      string sHelper = ConfigurationManager.AppSettings["Smtp.Server"];
      TailSettings.AlertSettings.SmtpSettings.SmtpServerName = sHelper;

      int port;

      if (!int.TryParse(ConfigurationManager.AppSettings["Smtp.Port"], out port))
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

      bool bHelper;

      if (!bool.TryParse(ConfigurationManager.AppSettings["Smtp.Ssl"], out bHelper))
        bHelper = true;
      TailSettings.AlertSettings.SmtpSettings.SSL = bHelper;

      if (!bool.TryParse(ConfigurationManager.AppSettings["Smtp.Tls"], out bHelper))
        bHelper = false;
      if (TailSettings.AlertSettings.SmtpSettings.SSL)
        bHelper = false;
      TailSettings.AlertSettings.SmtpSettings.TLS = bHelper;
    }

    /// <summary>
    /// Read alert config settings
    /// </summary>
    private static void ReadAlertSettings()
    {
      bool bHelper;

      if (!bool.TryParse(ConfigurationManager.AppSettings["Alert.BringToFront"], out bHelper))
        bHelper = true;
      TailSettings.AlertSettings.BringToFront = bHelper;

      if (!bool.TryParse(ConfigurationManager.AppSettings["Alert.PlaySoundFile"], out bHelper))
        bHelper = false;
      TailSettings.AlertSettings.PlaySoundFile = bHelper;

      if (!bool.TryParse(ConfigurationManager.AppSettings["Alert.SendEMail"], out bHelper))
        bHelper = false;
      TailSettings.AlertSettings.SendEMail = bHelper;

      string sHelper = ConfigurationManager.AppSettings["Alert.SoundFile"];
      ParseSoundFileName(sHelper);

      sHelper = ConfigurationManager.AppSettings["Alert.EMailAddress"];

      TailSettings.AlertSettings.EMailAddress = ParseEMailAddress(sHelper) ? sHelper : LogFile.ALERT_EMAIL_ADDRESS;

      if (ConfigurationManager.AppSettings["Alert.PopupWindow"] == null)
        AddNewProperties_IntoConfigFile("Alert.PopupWindow", "false");

      if (!bool.TryParse(ConfigurationManager.AppSettings["Alert.PopupWindow"], out bHelper))
        bHelper = false;
      TailSettings.AlertSettings.PopupWnd = bHelper;
    }

    /// <summary>
    /// Get all Enum ThreadPriorities
    /// </summary>
    /// <param name="s">Reference of thread priority string</param>
    /// <returns>Enum from thread priority</returns>
    public static ThreadPriority GetThreadPriority(string s)
    {
      if (s == null)
        return (ThreadPriority.Normal);

      if (Enum.GetNames(typeof(ThreadPriority)).All(priorityName => String.Compare(s.ToLower(), priorityName.ToLower(), StringComparison.Ordinal) != 0))
        return (ThreadPriority.Normal);

      ThreadPriority tp = (ThreadPriority)Enum.Parse(typeof(ThreadPriority), s);

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
    /// <returns>Enum from refresh rate</returns>
    public static ETailRefreshRate GetRefreshRate(string s)
    {
      if (s == null)
        return (ETailRefreshRate.Normal);

      if (Enum.GetNames(typeof(ETailRefreshRate)).All(refreshName => String.Compare(s.ToLower(), refreshName.ToLower(), StringComparison.Ordinal) != 0))
        return (ETailRefreshRate.Normal);

      ETailRefreshRate trr = (ETailRefreshRate)Enum.Parse(typeof(ETailRefreshRate), s);

      return (trr);
    }

    private static void ReadThreadRefreshRateEnum(string s)
    {
      TailSettings.DefaultRefreshRate = GetRefreshRate(s);
    }

    private static void ReadTimeFormatEnum(string s)
    {
      if (s != null)
        foreach (string timeFormat in Enum.GetNames(typeof(ETimeFormat)))
        {
          if (String.Compare(s, timeFormat, StringComparison.Ordinal) == 0)
          {
            ETimeFormat tf = (ETimeFormat)Enum.Parse(typeof(ETimeFormat), s);
            TailSettings.DefaultTimeFormat = tf;
            break;
          }

          TailSettings.DefaultTimeFormat = ETimeFormat.HHMMD;
        }
      else
        TailSettings.DefaultTimeFormat = ETimeFormat.HHMMD;
    }

    private static void ReadDateFormatEnum(string s)
    {
      if (s != null)
        foreach (string dateFormat in Enum.GetNames(typeof(EDateFormat)))
        {
          if (String.Compare(s, dateFormat, StringComparison.Ordinal) == 0)
          {
            EDateFormat df = (EDateFormat)Enum.Parse(typeof(EDateFormat), s);
            TailSettings.DefaultDateFormat = df;
            break;
          }

          TailSettings.DefaultDateFormat = EDateFormat.DDMMYYYY;
        }
      else
        TailSettings.DefaultDateFormat = EDateFormat.DDMMYYYY;
    }

    private static void ReadFileSortEnum(string s)
    {
      if (s != null)
        foreach (string fileSort in Enum.GetNames(typeof(EFileSort)))
        {
          if (String.Compare(s, fileSort, StringComparison.Ordinal) == 0)
          {
            EFileSort fs = (EFileSort)Enum.Parse(typeof(EFileSort), s);
            TailSettings.DefaultFileSort = fs;
            break;
          }

          TailSettings.DefaultFileSort = EFileSort.Nothing;
        }
      else
        TailSettings.DefaultFileSort = EFileSort.Nothing;
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

      return (regex.IsMatch(emailAddress));
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
      Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

      config.AppSettings.Settings.Add(key, value);

      config.Save(ConfigurationSaveMode.Modified);
      ConfigurationManager.RefreshSection("appSettings");
    }

    #endregion
  }
}
