using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using Org.Vs.TailForWin.Controller;
using Org.Vs.TailForWin.Data.Enums;
using Org.Vs.TailForWin.Interfaces;


namespace Org.Vs.TailForWin.Data
{
  /// <summary>
  /// LogFile class
  /// </summary>
  public static class LogFile
  {
    /// <summary>
    /// Application caption
    /// </summary>
    public static readonly string APPLICATION_CAPTION = "Tail4Windows";

    /// <summary>
    /// Status bar state run
    /// </summary>
    public static readonly string STATUS_BAR_STATE_RUN = Application.Current.FindResource("Record").ToString();

    /// <summary>
    /// Status bar state pause
    /// </summary>
    public static readonly string STATUS_BAR_STATE_PAUSE = Application.Current.FindResource("Pause").ToString();

    /// <summary>
    /// Tabbar child empty
    /// </summary>
    public static readonly string TABBAR_CHILD_EMPTY_STRING = Application.Current.FindResource("NoFile").ToString();

    /// <summary>
    /// MessageBox error
    /// </summary>
    /// 
    public static readonly string MSGBOX_ERROR = Application.Current.FindResource("Error").ToString();

    /// <summary>
    /// Default forground color
    /// </summary>
    public static readonly string DEFAULT_FOREGROUND_COLOR = "#000000";

    /// <summary>
    /// Default background color
    /// </summary>
    public static readonly string DEFAULT_BACKGROUND_COLOR = "#FFFFFF";

    /// <summary>
    /// Default inactive foreground color
    /// </summary>
    public static readonly string DEFAULT_INACTIVE_FOREGROUND_COLOR = "#000000";

    /// <summary>
    /// default inactive background color
    /// </summary>
    public static readonly string DEFAULT_INACTIVE_BACKGROUND_COLOR = "#FFFCFAF5";

    /// <summary>
    /// Default find highlight background color
    /// </summary>
    public static readonly string DEFAULT_FIND_HIGHLIGHT_BACKGROUND_COLOR = "#FFCC00";

    /// <summary>
    /// Default find highlight foreground color
    /// </summary>
    public static readonly string DEFAULT_FIND_HIGHLIGHT_FOREGROUND_COLOR = "#000000";

    /// <summary>
    /// Default line number color
    /// </summary>
    public static readonly string DEFAULT_LINE_NUMBERS_COLOR = "#808080";

    /// <summary>
    /// Default highlight color
    /// </summary>
    public static readonly string DEFAULT_HIGHLIGHT_COLOR = "#FF0000FF";

    /// <summary>
    /// Alert sound filename
    /// </summary>
    public static readonly string ALERT_SOUND_FILENAME = "NoFile";

    /// <summary>
    /// Alert email address
    /// </summary>
    public static readonly string ALERT_EMAIL_ADDRESS = "NoMail";

    /// <summary>
    /// Encrypt passphrase
    /// </summary>
    public static readonly string ENCRYPT_PASSPHRASE = "fhsdtgf45FSDvj_dhjf#+sdkjfh567gAQW";

    /// <summary>
    /// Unlimited log line value
    /// </summary>
    public static readonly int UNLIMITED_LOG_LINE_VALUE = 400000;

    /// <summary>
    /// Delete log files older than xxx days
    /// </summary>
    public static readonly int DELETE_LOG_FILES_OLDER_THAN = 5;

    #region RegexPattern

    /// <summary>
    /// Regex for sound file extension
    /// </summary>
    public static readonly string REGEX_SOUNDFILE_EXTENSION = @"^\.(mp3|wav)";

    /// <summary>
    /// Regex for E-Mail address
    /// </summary>
    public static readonly string REGEX_EMAIL_ADDRESS = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";

    #endregion

    /// <summary>
    /// MainWindow reference from TfW
    /// </summary>
    public static UI.TabWindow APP_MAIN_WINDOW = Application.Current.MainWindow as UI.TabWindow;

    /// <summary>
    /// Max open tabs in TfW
    /// </summary>
    public const int MAX_TAB_CHILDS = 10;

    /// <summary>
    /// Current settings for Tail4Windows
    /// </summary>
    public static readonly ISettingsHelper Settings = new SettingsHelper();


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
    public static ObservableCollection<FileManagerHelper> FmHelper { get; } = new ObservableCollection<FileManagerHelper>();

    /// <summary>
    /// List of thread priority (static)
    /// </summary>
    public static ObservableCollection<ThreadPriorityMapping> ThreadPriority { get; } = new ObservableCollection<ThreadPriorityMapping>();

    /// <summary>
    /// List of supported refresh rates
    /// </summary>
    public static ObservableCollection<ETailRefreshRate> RefreshRate { get; } = new ObservableCollection<ETailRefreshRate>();

    /// <summary>
    /// List of supported file encodings
    /// </summary>
    public static ObservableCollection<Encoding> FileEncoding { get; } = new ObservableCollection<Encoding>();

    /// <summary>
    /// Initialize the observable collections from refresh rate (RR), thread priority (TP) and file encodings (FE)
    /// </summary>
    public static void InitObservableCollectionsRrtpfe()
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

        var xEncodingInfo = x as EncodingInfo;
        var yEncodingInfo = y as EncodingInfo;

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
      return listOfFilters.Any(item => String.Compare(item.Filter, newItem.Filter, StringComparison.Ordinal) == 0);
    }
  }
}
