using System.Windows;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace TailForWin.Data
{
  public static class LogFile
  {
    public static string APPLICATION_CAPTION = "TailForWindows";
    public static string STATUS_BAR_STATE_RUN = Application.Current.FindResource ("Record") as string;
    public static string STATUS_BAR_STATE_PAUSE = Application.Current.FindResource ("Pause") as string;
    public static string TABBAR_CHILD_EMPTY_STRING = Application.Current.FindResource ("NoFile") as string;
    public static string MSGBOX_ERROR = Application.Current.FindResource ("Error") as string;
    public static string DEFAULT_FOREGROUND_COLOR = "#000000";
    public static string DEFAULT_BACKGROUND_COLOR = "#FFFFFF";
    public static string DEFAULT_INACTIVE_FOREGROUND_COLOR = "#000000";
    public static string DEFAULT_INACTIVE_BACKGROUND_COLOR = "#FFFCFAF5";
    public static string DEFAULT_FIND_HIGHLIGHT_BACKGROUND_COLOR = "#FFCC00";
    public static string DEFAULT_FIND_HIGHLIGHT_FOREGROUND_COLOR = "#000000";
    public static string DEFAULT_LINE_NUMBERS_COLOR = "#808080";
    public static string DEFAULT_HIGHLIGHT_COLOR = "#FF0000FF";
    public static string ALERT_SOUND_FILENAME = "NoFile";
    public static string ALERT_EMAIL_ADDRESS = "NoMail";

    #region ObservableCollection

    private static ObservableCollection<System.Threading.ThreadPriority> threadPriority = new ObservableCollection<System.Threading.ThreadPriority> ( );
    private static ObservableCollection<SettingsData.ETailRefreshRate> refreshRate = new ObservableCollection<SettingsData.ETailRefreshRate> ( );
    private static ObservableCollection<Encoding> fileEncoding = new ObservableCollection<Encoding> ( );

    #endregion

    #region RegexPattern

    public static string REGEX_SOUNDFILE_EXTENSION = @"^\.(mp3|wav)";
    public static string REGEX_EMAIL_ADDRESS = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";

    #endregion

    public static MainWindow APP_MAIN_WINDOW = (Application.Current.MainWindow as MainWindow);
    public const int MAX_TAB_CHILDS = 10;


    /// <summary>
    /// Shows open file dialog
    /// </summary>
    /// <param name="fileName">Output of filename</param>
    /// <returns>If success true otherwise false</returns>
    public static bool OpenFileLogDialog (out string fileName, string filter, string title)
    {
      OpenFileDialog openDialog = new OpenFileDialog ( )
      {
        Filter = filter,
        RestoreDirectory = true,
        Title = title
      };

      Nullable<bool> result = openDialog.ShowDialog ( );
      fileName = string.Empty;

      if (result == true)
      {
        fileName = openDialog.FileName;

        return (true);
      }
      return (false);
    }

    /// <summary>
    /// Bring main window to front and set it active
    /// </summary>
    public static void BringMainWindoToFront ()
    {
      if (APP_MAIN_WINDOW.WindowState == WindowState.Minimized)
        APP_MAIN_WINDOW.WindowState = WindowState.Normal;

      APP_MAIN_WINDOW.Activate ( );
      APP_MAIN_WINDOW.Focus ( );
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
    public static ObservableCollection<SettingsData.ETailRefreshRate> RefreshRate
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
    public static void InitObservableCollectionsRRTPFE ()
    {
      // Threadrefresh rate
      foreach (SettingsData.ETailRefreshRate refreshName in Enum.GetValues (typeof (SettingsData.ETailRefreshRate)))
      {
        RefreshRate.Add (refreshName);
      }

      // Threadpriority
      foreach (System.Threading.ThreadPriority threadPriority in Enum.GetValues (typeof (System.Threading.ThreadPriority)))
      {
        ThreadPriority.Add (threadPriority);
      }

      // Fileencoding
      EncodingInfo[] encodings = Encoding.GetEncodings ( );
      Array.Sort (encodings, new CaseInsensitiveEncodingInfoNameComparer ( ));

      foreach (EncodingInfo fileEncode in encodings)
      {
        FileEncoding.Add (fileEncode.GetEncoding ( ));
      }
    }

    private class CaseInsensitiveEncodingInfoNameComparer: IComparer
    {
      int IComparer.Compare (Object x, Object y)
      {
        int result = 0;

        if (x is EncodingInfo && y is EncodingInfo)
        {
          var xEncodingInfo = x as EncodingInfo;
          var yEncodingInfo = y as EncodingInfo;
          result = ((new CaseInsensitiveComparer ( )).Compare (xEncodingInfo.Name, yEncodingInfo.Name));
        }
        return (result);
      }
    }

    public static bool FindDuplicateInFilterList (ObservableCollection<FilterData> listOfFilters, FilterData newItem)
    {
      foreach (FilterData item in listOfFilters)
      {
        if (item.Filter.CompareTo (newItem.Filter) == 0)
          return (true);
      }
      return (false);
    }

    /// <summary>
    /// FileCreationTime comparer
    /// </summary>
    public class FileManagerDataFileCreationTimeComparer: IComparer<FileManagerData>
    {
      #region IComparer<DateTime?> Members

      public int Compare (FileManagerData x, FileManagerData y)
      {
        DateTime nx = x.FileCreationTime ?? DateTime.MaxValue;
        DateTime ny = y.FileCreationTime ?? DateTime.MaxValue;

        return (-(nx.CompareTo (ny)));
      }
      
      #endregion
    }
  }
}
