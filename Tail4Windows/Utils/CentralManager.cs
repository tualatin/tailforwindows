using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using Org.Vs.TailForWin.Controller;
using Org.Vs.TailForWin.Data;
using Org.Vs.TailForWin.Data.Enums;
using Org.Vs.TailForWin.Interfaces;


namespace Org.Vs.TailForWin.Utils
{
  /// <summary>
  /// Central manager for Tail4Windows
  /// </summary>
  public sealed class CentralManager
  {
    /// <summary>
    /// Application caption
    /// </summary>
    public const string APPLICATION_CAPTION = "Tail4Windows";

    /// <summary>
    /// Status bar state run
    /// </summary>
    public static readonly string STATUS_BAR_STATE_RUN = Application.Current.FindResource("Record")?.ToString();

    /// <summary>
    /// Status bar state pause
    /// </summary>
    public static readonly string STATUS_BAR_STATE_PAUSE = Application.Current.FindResource("Pause")?.ToString();

    /// <summary>
    /// Tabbar child empty
    /// </summary>
    public static readonly string TABBAR_CHILD_EMPTY_STRING = Application.Current.FindResource("NoFile")?.ToString();

    /// <summary>
    /// MessageBox error
    /// </summary>
    /// 
    public static readonly string MSGBOX_ERROR = Application.Current.FindResource("Error")?.ToString();

    /// <summary>
    /// Default forground color
    /// </summary>
    public const string DEFAULT_FOREGROUND_COLOR = "#000000";

    /// <summary>
    /// Default background color
    /// </summary>
    public const string DEFAULT_BACKGROUND_COLOR = "#FFFFFF";

    /// <summary>
    /// Default inactive foreground color
    /// </summary>
    public const string DEFAULT_INACTIVE_FOREGROUND_COLOR = "#000000";

    /// <summary>
    /// default inactive background color
    /// </summary>
    public const string DEFAULT_INACTIVE_BACKGROUND_COLOR = "#FFFCFAF5";

    /// <summary>
    /// Default find highlight background color
    /// </summary>
    public const string DEFAULT_FIND_HIGHLIGHT_BACKGROUND_COLOR = "#FFCC00";

    /// <summary>
    /// Default find highlight foreground color
    /// </summary>
    public const string DEFAULT_FIND_HIGHLIGHT_FOREGROUND_COLOR = "#000000";

    /// <summary>
    /// Default line number color
    /// </summary>
    public const string DEFAULT_LINE_NUMBERS_COLOR = "#808080";

    /// <summary>
    /// Default highlight color
    /// </summary>
    public const string DEFAULT_HIGHLIGHT_COLOR = "#FF0000FF";

    /// <summary>
    /// Alert sound filename
    /// </summary>
    public const string ALERT_SOUND_FILENAME = "NoFile";

    /// <summary>
    /// Alert email address
    /// </summary>
    public const string ALERT_EMAIL_ADDRESS = "NoMail";

    /// <summary>
    /// Encrypt passphrase
    /// </summary>
    public const string ENCRYPT_PASSPHRASE = "fhsdtgf45FSDvj_dhjf#+sdkjfh567gAQW";

    /// <summary>
    /// Unlimited log line value
    /// </summary>
    public const int UNLIMITED_LOG_LINE_VALUE = 400000;

    /// <summary>
    /// Delete log files older than xxx days
    /// </summary>
    public const int DELETE_LOG_FILES_OLDER_THAN = 5;

    #region RegexPattern

    /// <summary>
    /// Regex for sound file extension
    /// </summary>
    public const string REGEX_SOUNDFILE_EXTENSION = @"^\.(mp3|wav)";

    /// <summary>
    /// Regex for E-Mail address
    /// </summary>
    public const string REGEX_EMAIL_ADDRESS =
        @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?"
      ;

    #endregion

    /// <summary>
    /// MainWindow reference from TfW
    /// </summary>
    public static UI.TabWindow APP_MAIN_WINDOW = Application.Current.MainWindow as UI.TabWindow;

    /// <summary>
    /// Max open tabs in TfW
    /// </summary>
    public const int MAX_TAB_CHILDS = 10;

    private readonly ISettingsHelper settings = new SettingsHelper();
    private static CentralManager instance;


    private CentralManager()
    {
    }

    /// <summary>
    /// Get a instance of the CentralManager
    /// </summary>
    /// <returns>Current instance of CentralManager</returns>
    public static CentralManager Instance()
    {
      return instance ?? (instance = new CentralManager());
    }

    /// <summary>
    /// Read current settings
    /// </summary>
    public void ReadSettings()
    {
      settings.ReadSettings();
    }

    /// <summary>
    /// Reloads current settings
    /// </summary>
    public void ReloadSettings()
    {
      settings.ReloadSettings();
    }

    /// <summary>
    /// Save current settings
    /// </summary>
    public void SaveSettings()
    {
      settings.SaveSettings();
    }

    /// <summary>
    /// Save search window settings
    /// </summary>
    public void SaveSearchWindowSettings()
    {
      settings.SaveSearchWindowPosition();
    }

    /// <summary>
    /// Reset current settings to default values
    /// </summary>
    public void ResetSettings()
    {
      settings.SetToDefault();
    }

    /// <summary>
    /// Shows open file dialog
    /// </summary>
    /// <param name="fileName">Output of filename</param>
    /// <param name="filter">Filter</param>
    /// <param name="title">Title</param>
    /// <returns>If success true otherwise false</returns>
    public static bool OpenFileLogDialog(out string fileName, string filter, string title)
    {
      OpenFileDialog openDialog = new OpenFileDialog
      {
        Filter = filter,
        RestoreDirectory = true,
        Title = title
      };

      bool? result = openDialog.ShowDialog();
      fileName = string.Empty;

      if ( result != true )
        return false;

      fileName = openDialog.FileName;

      return true;
    }

    /// <summary>
    /// Bring main window to front and set it active
    /// </summary>
    public static void BringMainWindowToFront()
    {
      if ( APP_MAIN_WINDOW.WindowState == WindowState.Minimized )
        APP_MAIN_WINDOW.WindowState = WindowState.Normal;

      APP_MAIN_WINDOW.Activate();
      APP_MAIN_WINDOW.Focus();
    }

    /// <summary>
    /// Minimize main window
    /// </summary>
    public static void MinimizeMainWindow()
    {
      if ( APP_MAIN_WINDOW.WindowState == WindowState.Normal || APP_MAIN_WINDOW.WindowState == WindowState.Maximized )
        APP_MAIN_WINDOW.WindowState = WindowState.Minimized;
    }

    /// <summary>
    /// List of open items from FileManager, to see, which file is already open
    /// </summary>
    public ObservableCollection<FileManagerHelper> FmHelper { get; } = new ObservableCollection<FileManagerHelper>();

    /// <summary>
    /// List of thread priority (static)
    /// </summary>
    public ObservableCollection<ThreadPriorityMapping> ThreadPriority
    {
      get;
    } =
      new ObservableCollection<ThreadPriorityMapping>();

    /// <summary>
    /// List of supported refresh rates
    /// </summary>
    public ObservableCollection<ETailRefreshRate> RefreshRate { get; } = new ObservableCollection<ETailRefreshRate>();

    /// <summary>
    /// List of supported file encodings
    /// </summary>
    public ObservableCollection<Encoding> FileEncoding { get; } = new ObservableCollection<Encoding>();

    /// <summary>
    /// Initialize the observable collections from refresh rate (RR), thread priority (TP) and file encodings (FE)
    /// </summary>
    public void InitObservableCollectionsRrtpfe()
    {
      // ThreadRefresh rate
      foreach ( ETailRefreshRate refreshName in Enum.GetValues(typeof(ETailRefreshRate)) )
      {
        RefreshRate.Add(refreshName);
      }

      // ThreadPriority
      foreach ( System.Threading.ThreadPriority priority in Enum.GetValues(typeof(System.Threading.ThreadPriority)) )
      {
        ThreadPriority.Add(new ThreadPriorityMapping
        {
          ThreadPriority = priority
        });
      }

      // Fileencoding
      EncodingInfo[] encodings = Encoding.GetEncodings();
      Array.Sort(encodings, new CaseInsensitiveEncodingInfoNameComparer());
      Array.ForEach(encodings, fileEncode => FileEncoding.Add(fileEncode.GetEncoding()));
    }

    private class CaseInsensitiveEncodingInfoNameComparer : IComparer
    {
      int IComparer.Compare(Object x, Object y)
      {
        const int result = 0;

        if ( !(x is EncodingInfo) || !(y is EncodingInfo) )
          return result;

        var xEncodingInfo = (EncodingInfo) x;
        var yEncodingInfo = (EncodingInfo) y;

        return new CaseInsensitiveComparer().Compare(xEncodingInfo.Name, yEncodingInfo.Name);
      }
    }

    /// <summary>
    /// Find duplicated items in filter list
    /// </summary>
    /// <param name="listOfFilters">List of filters</param>
    /// <param name="newItem">Item to add</param>
    /// <returns><c>True</c> if exists, otherwise <c>False</c></returns>
    public static bool FindDuplicateInFilterList(ObservableCollection<FilterData> listOfFilters, FilterData newItem)
    {
      return listOfFilters.Any(item => string.Compare(item.Filter, newItem.Filter, StringComparison.Ordinal) == 0);
    }
  }
}