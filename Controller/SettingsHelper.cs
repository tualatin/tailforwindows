using TailForWin.Data;
using System.Configuration;
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;


namespace TailForWin.Controller
{
  public class SettingsHelper
  {
    private static SettingsData tailSettings = new SettingsData ( );


    /// <summary>
    /// Read app settings
    /// </summary>
    public static void ReadSettings ()
    {
      try
      {
        int iHelper = -1;
        double dHelper = -1;
        bool bHelper;
        string sHelper = string.Empty;

        if (!int.TryParse (ConfigurationManager.AppSettings["LinesRead"], out iHelper))
          iHelper = 10;
        tailSettings.LinesRead = iHelper;

        if (!bool.TryParse (ConfigurationManager.AppSettings["AlwaysOnTop"], out bHelper))
          bHelper = false;
        tailSettings.AlwaysOnTop = bHelper;

        if (!bool.TryParse (ConfigurationManager.AppSettings["ShowNLineAtStart"], out bHelper))
          bHelper = true;
        tailSettings.ShowNLineAtStart = bHelper;

        if (!bool.TryParse (ConfigurationManager.AppSettings["AlwaysScrollToEnd"], out bHelper))
          bHelper = true;
        TailSettings.AlwaysScrollToEnd = bHelper;

        if (!bool.TryParse (ConfigurationManager.AppSettings["RestoreWindowSize"], out bHelper))
          bHelper = false;
        TailSettings.RestoreWindowSize = bHelper;

        if (!double.TryParse (ConfigurationManager.AppSettings["wndWidth"], out dHelper))
          dHelper = -1;
        tailSettings.WndWidth = dHelper;

        if (!double.TryParse (ConfigurationManager.AppSettings["wndHeight"], out dHelper))
          dHelper = -1;
        tailSettings.WndHeight = dHelper;

        if (!bool.TryParse (ConfigurationManager.AppSettings["SaveWindowPosition"], out bHelper))
          bHelper = false;
        TailSettings.SaveWindowPosition = bHelper;

        if (!double.TryParse (ConfigurationManager.AppSettings["wndXPos"], out dHelper))
          dHelper = -1;
        tailSettings.WndXPos = dHelper;

        if (!double.TryParse (ConfigurationManager.AppSettings["wndYPos"], out dHelper))
          dHelper = -1;
        tailSettings.WndYPos = dHelper;

        sHelper = ConfigurationManager.AppSettings["defaultThreadPriority"];
        ReadThreadPriorityEnum (sHelper);

        sHelper = ConfigurationManager.AppSettings["defaultRefreshRate"];
        ReadThreadRefreshRateEnum (sHelper);

        if (!bool.TryParse (ConfigurationManager.AppSettings["ExitWithEsc"], out bHelper))
          bHelper = false;
        TailSettings.ExitWithEscape = bHelper;

        sHelper = ConfigurationManager.AppSettings["TimeFormat"];
        ReadTimeFormatEnum (sHelper);

        sHelper = ConfigurationManager.AppSettings["DateFormat"];
        ReadDateFormatEnum (sHelper);

        sHelper = ConfigurationManager.AppSettings["ForegroundColor"];
        CheckHexColorValue (sHelper, SettingsData.ETailLogColorTypes.ForegroundColor);

        sHelper = ConfigurationManager.AppSettings["BackgroundColor"];
        CheckHexColorValue (sHelper, SettingsData.ETailLogColorTypes.BackgroundColor);

        sHelper = ConfigurationManager.AppSettings["InactiveForegroundColor"];
        CheckHexColorValue (sHelper, SettingsData.ETailLogColorTypes.InactiveForegroundColor);

        sHelper = ConfigurationManager.AppSettings["InactiveBackgroundColor"];
        CheckHexColorValue (sHelper, SettingsData.ETailLogColorTypes.InactiveBackgroundColor);

        sHelper = ConfigurationManager.AppSettings["FindHighlightForegroundColor"];
        CheckHexColorValue (sHelper, SettingsData.ETailLogColorTypes.FindHighlightForegroundColor);

        sHelper = ConfigurationManager.AppSettings["FindHighlightBackgroundColor"];
        CheckHexColorValue (sHelper, SettingsData.ETailLogColorTypes.FindHighlightBackgroundColor);

        sHelper = ConfigurationManager.AppSettings["LineNumbersColor"];
        CheckHexColorValue (sHelper, SettingsData.ETailLogColorTypes.LineNumbersColor);

        sHelper = ConfigurationManager.AppSettings ["HighlightColor"];
        CheckHexColorValue (sHelper, SettingsData.ETailLogColorTypes.HighlightColor);

        sHelper = ConfigurationManager.AppSettings["SearchwndXPos"];
        SetSearchWindowPosition (sHelper);

        sHelper = ConfigurationManager.AppSettings["SearchwndYPos"];
        SetSearchWindowPosition (sHelper, false);

        sHelper = ConfigurationManager.AppSettings["FileManagerSort"];
        ReadFileSortEnum (sHelper);

        if (!bool.TryParse (ConfigurationManager.AppSettings["ShowLineNumbers"], out bHelper))
          bHelper = false;
        TailSettings.ShowLineNumbers = bHelper;

        if (!bool.TryParse (ConfigurationManager.AppSettings["Alert.BringToFront"], out bHelper))
          bHelper = true;
        TailSettings.AlertSettings.BringToFront = bHelper;

        if (!bool.TryParse (ConfigurationManager.AppSettings["Alert.PlaySoundFile"], out bHelper))
          bHelper = false;
        TailSettings.AlertSettings.PlaySoundFile = bHelper;

        if (!bool.TryParse (ConfigurationManager.AppSettings["Alert.SendEMail"], out bHelper))
          bHelper = false;
        TailSettings.AlertSettings.SendEMail = bHelper;

        sHelper = ConfigurationManager.AppSettings["Alert.SoundFile"];
        ParseSoundFileName (sHelper);

        sHelper = ConfigurationManager.AppSettings["Alert.EMailAddress"];
        ParseEMailAddress (sHelper);
      }
      catch (ConfigurationErrorsException ex)
      {
        Debug.WriteLine (ex);
      }
    }

    /// <summary>
    /// Save app settings
    /// </summary>
    public static void SaveSettings ()
    {
      try
      {
        Configuration config = ConfigurationManager.OpenExeConfiguration (ConfigurationUserLevel.None);

        if (config.AppSettings.Settings.Count > 0)
        {
          config.AppSettings.Settings["LinesRead"].Value = TailSettings.LinesRead.ToString ( );
          config.AppSettings.Settings["AlwaysOnTop"].Value = TailSettings.AlwaysOnTop.ToString ( );
          config.AppSettings.Settings["ShowNLineAtStart"].Value = TailSettings.ShowNLineAtStart.ToString ( );
          config.AppSettings.Settings["AlwaysScrollToEnd"].Value = TailSettings.AlwaysScrollToEnd.ToString ( );
          config.AppSettings.Settings["RestoreWindowSize"].Value = TailSettings.RestoreWindowSize.ToString ( );
          config.AppSettings.Settings["wndWidth"].Value = TailSettings.WndWidth.ToString ( );
          config.AppSettings.Settings["wndHeight"].Value = TailSettings.WndHeight.ToString ( );
          config.AppSettings.Settings["SaveWindowPosition"].Value = TailSettings.RestoreWindowSize.ToString ( );
          config.AppSettings.Settings["wndXPos"].Value = TailSettings.WndXPos.ToString ( );
          config.AppSettings.Settings["wndYPos"].Value = TailSettings.WndYPos.ToString ( );
          config.AppSettings.Settings["defaultThreadPriority"].Value = TailSettings.DefaultThreadPriority.ToString ( );
          config.AppSettings.Settings["defaultRefreshRate"].Value = TailSettings.DefaultRefreshRate.ToString ( );
          config.AppSettings.Settings["ExitWithEsc"].Value = TailSettings.ExitWithEscape.ToString ( );
          config.AppSettings.Settings["TimeFormat"].Value = TailSettings.DefaultTimeFormat.ToString ( );
          config.AppSettings.Settings["DateFormat"].Value = TailSettings.DefaultDateFormat.ToString ( );
          config.AppSettings.Settings["ForegroundColor"].Value = TailSettings.DefaultForegroundColor;
          config.AppSettings.Settings["BackgroundColor"].Value = TailSettings.DefaultBackgroundColor;
          config.AppSettings.Settings["InactiveForegroundColor"].Value = TailSettings.DefaultInactiveForegroundColor;
          config.AppSettings.Settings["InactiveBackgroundColor"].Value = TailSettings.DefaultInactiveBackgroundColor;
          config.AppSettings.Settings["FindHighlightForegroundColor"].Value = TailSettings.DefaultHighlightForegroundColor;
          config.AppSettings.Settings["FindHighlightBackgroundColor"].Value = TailSettings.DefaultHighlightBackgroundColor;
          config.AppSettings.Settings["SearchwndXPos"].Value = TailSettings.SearchWndXPos.ToString ( );
          config.AppSettings.Settings["SearchwndYPos"].Value = TailSettings.SearchWndYPos.ToString ( );
          config.AppSettings.Settings["FileManagerSort"].Value = TailSettings.DefaultFileSort.ToString ( );
          config.AppSettings.Settings["ShowLineNumbers"].Value = TailSettings.ShowLineNumbers.ToString ( );
          config.AppSettings.Settings["LineNumbersColor"].Value = TailSettings.DefaultLineNumbersColor;
          config.AppSettings.Settings["HighlightColor"].Value = TailSettings.DefaultHighlightColor;
          config.AppSettings.Settings["Alert.BringToFront"].Value = TailSettings.AlertSettings.BringToFront.ToString ( );
          config.AppSettings.Settings["Alert.PlaySoundFile"].Value = TailSettings.AlertSettings.PlaySoundFile.ToString ( );
          config.AppSettings.Settings["Alert.SendEMail"].Value = TailSettings.AlertSettings.SendEMail.ToString ( );
          config.AppSettings.Settings["Alert.EMailAddress"].Value = TailSettings.AlertSettings.EMailAddress;
          config.AppSettings.Settings["Alert.SoundFile"].Value = TailSettings.AlertSettings.SoundFileNameFullPath;

          config.Save (ConfigurationSaveMode.Modified);
          ConfigurationManager.RefreshSection ("appSettings");
        }
      }
      catch (ConfigurationErrorsException ex)
      {
        Debug.WriteLine (ex);
      }
    }

    /// <summary>
    /// Save search dialog window position
    /// </summary>
    public static void SaveSearchWindowPosition ()
    {
      try
      {
        Configuration config = ConfigurationManager.OpenExeConfiguration (ConfigurationUserLevel.None);

        if (config.AppSettings.Settings.Count > 0)
        {
          config.AppSettings.Settings["SearchwndXPos"].Value = TailSettings.SearchWndXPos.ToString ( );
          config.AppSettings.Settings["SearchwndYPos"].Value = TailSettings.SearchWndYPos.ToString ( );

          config.Save (ConfigurationSaveMode.Modified);
          ConfigurationManager.RefreshSection ("appSettings");
        }
      }
      catch (ConfigurationErrorsException ex)
      {
        Debug.WriteLine (ex);
      }
    }

    /// <summary>
    /// Set app settings to default parameters
    /// </summary>
    public static void SetToDefault ()
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
      TailSettings.DefaultThreadPriority = System.Threading.ThreadPriority.Normal;
      TailSettings.DefaultRefreshRate = SettingsData.ETailRefreshRate.Normal;
      TailSettings.ExitWithEscape = false;
      TailSettings.DefaultTimeFormat = SettingsData.ETimeFormat.HHMMD;
      TailSettings.DefaultDateFormat = SettingsData.EDateFormat.DDMMYYYY;
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
      TailSettings.DefaultFileSort = SettingsData.EFileSort.Nothing;
      TailSettings.ShowLineNumbers = false;
      TailSettings.AlertSettings.BringToFront = true;
      TailSettings.AlertSettings.SendEMail = false;
      TailSettings.AlertSettings.PlaySoundFile = false;
      TailSettings.AlertSettings.SoundFileNameFullPath = LogFile.ALERT_SOUND_FILENAME;
      TailSettings.AlertSettings.EMailAddress = LogFile.ALERT_EMAIL_ADDRESS;

      SaveSettings ( );
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
    /// Get all Enum ThreadPriorities
    /// </summary>
    /// <param name="s">Reference of thread priority string</param>
    /// <returns>Enum from thread priority</returns>
    public static System.Threading.ThreadPriority GetThreadPriority (string s)
    {
      if (s != null)
        foreach (string priorityName in Enum.GetNames (typeof (System.Threading.ThreadPriority)))
        {
          if (s.ToLower ( ).CompareTo (priorityName.ToLower ( )) == 0)
          {
            System.Threading.ThreadPriority tp = (System.Threading.ThreadPriority) Enum.Parse (typeof (System.Threading.ThreadPriority), s);
          
            return (tp);
          }
        }
      return (System.Threading.ThreadPriority.Normal);
    }

    private static void ReadThreadPriorityEnum (string s)
    {
      TailSettings.DefaultThreadPriority = GetThreadPriority (s);
    }

    /// <summary>
    /// Get all Enum RefreshRates
    /// </summary>
    /// <param name="s">Reference of refresh rate string</param>
    /// <returns>Enum from refresh rate</returns>
    public static SettingsData.ETailRefreshRate GetRefreshRate (string s)
    {
      if (s != null)
        foreach (string refreshName in Enum.GetNames (typeof (SettingsData.ETailRefreshRate)))
        {
          if (s.ToLower ( ).CompareTo (refreshName.ToLower ( )) == 0)
          {
            SettingsData.ETailRefreshRate trr = (SettingsData.ETailRefreshRate) Enum.Parse (typeof (SettingsData.ETailRefreshRate), s);
            return (trr);
          }
        }
      return (SettingsData.ETailRefreshRate.Normal);
    }

    private static void ReadThreadRefreshRateEnum (string s)
    {
      TailSettings.DefaultRefreshRate = GetRefreshRate (s);
    }

    private static void ReadTimeFormatEnum (string s)
    {
      if (s != null)
        foreach (string timeFormat in Enum.GetNames (typeof (SettingsData.ETimeFormat)))
        {
          if (s.CompareTo (timeFormat) == 0)
          {
            SettingsData.ETimeFormat tf = (SettingsData.ETimeFormat) Enum.Parse (typeof (SettingsData.ETimeFormat), s);
            TailSettings.DefaultTimeFormat = tf;
            break;
          }
          else
            TailSettings.DefaultTimeFormat = SettingsData.ETimeFormat.HHMMD;
        }
      else
        TailSettings.DefaultTimeFormat = SettingsData.ETimeFormat.HHMMD;
    }

    private static void ReadDateFormatEnum (string s)
    {
      if (s != null)
        foreach (string dateFormat in Enum.GetNames (typeof (SettingsData.EDateFormat)))
        {
          if (s.CompareTo (dateFormat) == 0)
          {
            SettingsData.EDateFormat df = (SettingsData.EDateFormat) Enum.Parse (typeof (SettingsData.EDateFormat), s);
            TailSettings.DefaultDateFormat = df;
            break;
          }
          else
            TailSettings.DefaultDateFormat = SettingsData.EDateFormat.DDMMYYYY;
        }
      else
        TailSettings.DefaultDateFormat = SettingsData.EDateFormat.DDMMYYYY;
    }

    private static void ReadFileSortEnum (string s)
    {
      if (s != null)
        foreach (string fileSort in Enum.GetNames (typeof (SettingsData.EFileSort)))
        {
          if (s.CompareTo (fileSort) == 0)
          {
            SettingsData.EFileSort fs = (SettingsData.EFileSort) Enum.Parse (typeof (SettingsData.EFileSort), s);
            TailSettings.DefaultFileSort = fs;
            break;
          }
          else
            TailSettings.DefaultFileSort = SettingsData.EFileSort.Nothing;
        }
      else
        TailSettings.DefaultFileSort = SettingsData.EFileSort.Nothing;
    }

    private static void CheckHexColorValue (string s, SettingsData.ETailLogColorTypes cType)
    {
      Match match_6 = Regex.Match (s, @"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$");
      Match match_8 = Regex.Match (s, @"^(#)?([0-9a-fA-F]{4})([0-9a-fA-F]{4})?$");
      bool matched = match_6.Success | match_8.Success;

      switch (cType)
      {
      case SettingsData.ETailLogColorTypes.BackgroundColor:

        if (!matched)
          TailSettings.DefaultBackgroundColor = LogFile.DEFAULT_BACKGROUND_COLOR;
        else
          TailSettings.DefaultBackgroundColor = s;
        break;

      case SettingsData.ETailLogColorTypes.ForegroundColor:

        if (!matched)
          TailSettings.DefaultForegroundColor = LogFile.DEFAULT_FOREGROUND_COLOR;
        else
          TailSettings.DefaultForegroundColor = s;
        break;

      case SettingsData.ETailLogColorTypes.InactiveForegroundColor:

        if (!matched)
          TailSettings.DefaultInactiveForegroundColor = LogFile.DEFAULT_INACTIVE_FOREGROUND_COLOR;
        else
          TailSettings.DefaultInactiveForegroundColor = s;
        break;

      case SettingsData.ETailLogColorTypes.InactiveBackgroundColor:

        if (!matched)
          TailSettings.DefaultInactiveBackgroundColor = LogFile.DEFAULT_INACTIVE_BACKGROUND_COLOR;
        else
          TailSettings.DefaultInactiveBackgroundColor = s;
        break;

      case SettingsData.ETailLogColorTypes.FindHighlightForegroundColor:

        if (!matched)
          TailSettings.DefaultHighlightForegroundColor = LogFile.DEFAULT_FIND_HIGHLIGHT_FOREGROUND_COLOR;
        else
          TailSettings.DefaultHighlightForegroundColor = s;
        break;

      case SettingsData.ETailLogColorTypes.FindHighlightBackgroundColor:

        if (!matched)
          TailSettings.DefaultHighlightBackgroundColor = LogFile.DEFAULT_FIND_HIGHLIGHT_BACKGROUND_COLOR;
        else
          TailSettings.DefaultHighlightBackgroundColor = s;
        break;

      case SettingsData.ETailLogColorTypes.LineNumbersColor:

        if (!matched)
          TailSettings.DefaultLineNumbersColor = LogFile.DEFAULT_LINE_NUMBERS_COLOR;
        else
          TailSettings.DefaultLineNumbersColor = s;
        break;

      case SettingsData.ETailLogColorTypes.HighlightColor:

        if (!matched)
          TailSettings.DefaultHighlightColor = LogFile.DEFAULT_HIGHLIGHT_COLOR;
        else
          TailSettings.DefaultHighlightColor = s;
        break;
        
      default:

        throw new NotImplementedException ("not supported color type");
      }
    }

    private static void SetSearchWindowPosition (string s, bool xPos = true)
    {
      int pos;

      if (xPos == true)
      {
        if (!int.TryParse (s, out pos))
          pos = -1;

        TailSettings.SearchWndXPos = pos;
      }
      else
      {
        if (!int.TryParse (s, out pos))
          pos = -1;

        TailSettings.SearchWndYPos = pos;
      }       
    }

    private static void ParseEMailAddress (string emailAddress)
    {
      string checkedEMail = string.Empty;
      Regex regex = new Regex (LogFile.REGEX_EMAIL_ADDRESS);
      
      if (regex.IsMatch (emailAddress))
        checkedEMail = emailAddress;
      else
        checkedEMail = LogFile.ALERT_EMAIL_ADDRESS;

      TailSettings.AlertSettings.EMailAddress = checkedEMail;
    }

    private static void ParseSoundFileName (string fullPath)
    {
      string extension = System.IO.Path.GetExtension (fullPath);
      string fileName = System.IO.Path.GetFileName (fullPath);
      char[] reserved = System.IO.Path.GetInvalidFileNameChars ( );

      if (fileName.Length < 3)
      {
        TailSettings.AlertSettings.SoundFileNameFullPath = LogFile.ALERT_SOUND_FILENAME;
        return;
      }

      if (fileName.Length > 128)
      {
        TailSettings.AlertSettings.SoundFileNameFullPath = LogFile.ALERT_SOUND_FILENAME;
        return;
      }

      foreach (char c in reserved)
      {
        if (fileName.Contains (c.ToString ( )))
        {
          TailSettings.AlertSettings.SoundFileNameFullPath = LogFile.ALERT_SOUND_FILENAME;
          return;
        }
      }

      Regex regex = new Regex (LogFile.REGEX_SOUNDFILE_EXTENSION);

      if (!regex.IsMatch (extension))
      {
        TailSettings.AlertSettings.SoundFileNameFullPath = LogFile.ALERT_SOUND_FILENAME;
        return;
      }

      TailSettings.AlertSettings.SoundFileNameFullPath = fullPath;
    }

    #endregion
  }
}
