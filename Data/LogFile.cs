using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using Org.Vs.TailForWin.Data.Enums;


namespace Org.Vs.TailForWin.Data
{
  /// <summary>
  /// LogFile class
  /// </summary>
  public static partial class LogFile
  {
    /// <summary>
    /// Application caption
    /// </summary>
    public static string APPLICATION_CAPTION = "Tail4Windows";

    /// <summary>
    /// Status bar state run
    /// </summary>
    public static string STATUS_BAR_STATE_RUN = Application.Current.FindResource("Record") as string;

    /// <summary>
    /// Status bar state pause
    /// </summary>
    public static string STATUS_BAR_STATE_PAUSE = Application.Current.FindResource("Pause") as string;

    /// <summary>
    /// Tabbar child empty
    /// </summary>
    public static string TABBAR_CHILD_EMPTY_STRING = Application.Current.FindResource("NoFile") as string;

    /// <summary>
    /// MessageBox error
    /// </summary>
    /// 
    public static string MSGBOX_ERROR = Application.Current.FindResource("Error") as string;

    /// <summary>
    /// Default forground color
    /// </summary>
    public static string DEFAULT_FOREGROUND_COLOR = "#000000";

    /// <summary>
    /// Default background color
    /// </summary>
    public static string DEFAULT_BACKGROUND_COLOR = "#FFFFFF";

    /// <summary>
    /// Default inactive foreground color
    /// </summary>
    public static string DEFAULT_INACTIVE_FOREGROUND_COLOR = "#000000";

    /// <summary>
    /// default inactive background color
    /// </summary>
    public static string DEFAULT_INACTIVE_BACKGROUND_COLOR = "#FFFCFAF5";

    /// <summary>
    /// Default find highlight background color
    /// </summary>
    public static string DEFAULT_FIND_HIGHLIGHT_BACKGROUND_COLOR = "#FFCC00";

    /// <summary>
    /// Default find highlight foreground color
    /// </summary>
    public static string DEFAULT_FIND_HIGHLIGHT_FOREGROUND_COLOR = "#000000";

    /// <summary>
    /// Default line number color
    /// </summary>
    public static string DEFAULT_LINE_NUMBERS_COLOR = "#808080";

    /// <summary>
    /// Default highlight color
    /// </summary>
    public static string DEFAULT_HIGHLIGHT_COLOR = "#FF0000FF";

    /// <summary>
    /// Alert sound filename
    /// </summary>
    public static string ALERT_SOUND_FILENAME = "NoFile";

    /// <summary>
    /// Alert email address
    /// </summary>
    public static string ALERT_EMAIL_ADDRESS = "NoMail";

    /// <summary>
    /// Encrypt passphrase
    /// </summary>
    public static string ENCRYPT_PASSPHRASE = "fhsdtgf45FSDvj_dhjf#+sdkjfh567gAQW";

    #region ObservableCollection

    private static ObservableCollection<System.Threading.ThreadPriority> threadPriority = new ObservableCollection<System.Threading.ThreadPriority>();
    private static ObservableCollection<ETailRefreshRate> refreshRate = new ObservableCollection<ETailRefreshRate>();
    private static ObservableCollection<Encoding> fileEncoding = new ObservableCollection<Encoding>();
    private static ObservableCollection<FileManagerHelper> fmHelper = new ObservableCollection<FileManagerHelper>();

    #endregion

    #region RegexPattern

    /// <summary>
    /// Regex for sound file extension
    /// </summary>
    public static string REGEX_SOUNDFILE_EXTENSION = @"^\.(mp3|wav)";

    /// <summary>
    /// Regex for E-Mail address
    /// </summary>
    public static string REGEX_EMAIL_ADDRESS = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";

    #endregion

    /// <summary>
    /// MainWindow reference from TfW
    /// </summary>
    //public static MainWindow APP_MAIN_WINDOW = (Application.Current.MainWindow as MainWindow);
    public static UI.TabWindow APP_MAIN_WINDOW = (Application.Current.MainWindow as UI.TabWindow);


    /// <summary>
    /// Max open tabs in TfW
    /// </summary>
    public const int MAX_TAB_CHILDS = 10;

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

      if(result != true)
        return (false);

      fileName = openDialog.FileName;

      return (true);
    }

    /// <summary>
    /// Bring main window to front and set it active
    /// </summary>
    public static void BringMainWindowToFront()
    {
      if(APP_MAIN_WINDOW.WindowState == WindowState.Minimized)
        APP_MAIN_WINDOW.WindowState = WindowState.Normal;

      APP_MAIN_WINDOW.Activate();
      APP_MAIN_WINDOW.Focus();
    }

    /// <summary>
    /// Minimize main window
    /// </summary>
    public static void MinimizeMainWindow()
    {
      if(APP_MAIN_WINDOW.WindowState == WindowState.Normal || APP_MAIN_WINDOW.WindowState == WindowState.Maximized)
        APP_MAIN_WINDOW.WindowState = WindowState.Minimized;
    }

    /// <summary>
    /// List of open items from FileManager
    /// </summary>
    public static ObservableCollection<FileManagerHelper> FmHelper
    {
      get
      {
        return (fmHelper);
      }
      set
      {
        fmHelper = value;
      }
    }

    /// <summary>
    /// List of thread priority (static)
    /// </summary>
    public static ObservableCollection<System.Threading.ThreadPriority> ThreadPriority
    {
      get
      {
        return (threadPriority);
      }
      set
      {
        threadPriority = value;
      }
    }

    /// <summary>
    /// List of supported refresh rates
    /// </summary>
    public static ObservableCollection<ETailRefreshRate> RefreshRate
    {
      get
      {
        return (refreshRate);
      }
      set
      {
        refreshRate = value;
      }
    }

    /// <summary>
    /// List of supported file encodings
    /// </summary>
    public static ObservableCollection<Encoding> FileEncoding
    {
      get
      {
        return (fileEncoding);
      }
      set
      {
        fileEncoding = value;
      }
    }

    /// <summary>
    /// Initialize the observable collections from refresh rate (RR), thread priority (TP) and file encodings (FE)
    /// </summary>
    public static void InitObservableCollectionsRrtpfe()
    {
      // Threadrefresh rate
      foreach(ETailRefreshRate refreshName in Enum.GetValues(typeof(ETailRefreshRate)))
      {
        RefreshRate.Add(refreshName);
      }

      // Threadpriority
      foreach(System.Threading.ThreadPriority priority in Enum.GetValues(typeof(System.Threading.ThreadPriority)))
      {
        ThreadPriority.Add(priority);
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

        if(!(x is EncodingInfo) || !(y is EncodingInfo))
          return (result);

        var xEncodingInfo = x as EncodingInfo;
        var yEncodingInfo = y as EncodingInfo;

        return ((new CaseInsensitiveComparer()).Compare(xEncodingInfo.Name, yEncodingInfo.Name));
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
      return (listOfFilters.Any(item => String.Compare(item.Filter, newItem.Filter, StringComparison.Ordinal) == 0));
    }
  }
}
