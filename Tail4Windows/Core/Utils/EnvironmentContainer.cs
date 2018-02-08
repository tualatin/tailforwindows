using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Interfaces;


namespace Org.Vs.TailForWin.Core.Utils
{
  /// <summary>
  /// Environment container for T4W
  /// </summary>
  public class EnvironmentContainer
  {
    private static EnvironmentContainer _instance;

    /// <summary>
    /// Current instance
    /// </summary>
    public static EnvironmentContainer Instance => _instance ?? (_instance = new EnvironmentContainer());

    private readonly ISettingsHelper _settings;


    private EnvironmentContainer()
    {
      _settings = new SettingsHelperController();
      UpTime = DateTime.Now;
      CurrentEventManager = new EventAggregator();
      
      IntializeObservableCollections();
    }

    /// <summary>
    /// Application title
    /// </summary>
    public static string ApplicationTitle => Application.Current.TryFindResource("ApplicationTitle").ToString();

    /// <summary>
    /// Application Update URL
    /// </summary>
    public static string ApplicationUpdateWebUrl => Application.Current.TryFindResource("WebUrl").ToString();

    /// <summary>
    /// Delete log files older than a certain time span
    /// </summary>
    public const int DeleteLogFilesOlderThan = 5;

    /// <summary>
    /// Current event manager
    /// </summary>
    public readonly IEventAggregator CurrentEventManager;

    /// <summary>
    /// List of supported file encodings
    /// </summary>
    public ObservableCollection<Encoding> FileEncoding { get; } = new ObservableCollection<Encoding>();

    /// <summary>
    /// List of supported refresh rates
    /// </summary>
    public ObservableCollection<ETailRefreshRate> RefreshRate { get; } = new ObservableCollection<ETailRefreshRate>();

    /// <summary>
    /// List of thread priority (static)
    /// </summary>
    public ObservableCollection<ThreadPriorityMapping> ThreadPriority { get; } = new ObservableCollection<ThreadPriorityMapping>();

    /// <summary>
    /// Current application path
    /// </summary>
    public static string ApplicationPath => Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

    /// <summary>
    /// Read current settings
    /// </summary>
    public async Task ReadSettingsAsync() => await _settings.ReadSettingsAsync().ConfigureAwait(false);

    /// <summary>
    /// Save current settings
    /// </summary>
    /// <returns></returns>
    public async Task SaveSettingsAsync() => await _settings.SaveSettingsAsync().ConfigureAwait(false);

    /// <summary>
    /// Reload settings from config file
    /// </summary>
    /// <returns></returns>
    public async Task ReloadSettingsAsync() => await _settings.ReloadCurrentSettingsAsync().ConfigureAwait(false);

    /// <summary>
    /// Create default T4W font
    /// </summary>
    /// <returns>Default font configuration</returns>
    public static Font CreateDefaultFont() => new Font("Segoe UI", 11f, System.Drawing.FontStyle.Regular);

    /// <summary>
    /// Current installed .NET version
    /// </summary>
    public static int NetFrameworkKey
    {
      get
      {
        using ( var netFrameworkKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\") )
        {
          try
          {
            return Convert.ToInt32(netFrameworkKey?.GetValue("Release"));
          }
          catch
          {
            return -1;
          }
        }
      }
    }

    /// <summary>
    /// Current up time
    /// </summary>
    public DateTime UpTime
    {
      get;
      private set;
    }

    /// <summary>
    /// Converts a hex string to brush color
    /// </summary>
    /// <param name="hex">Color as hex string</param>
    /// <returns>A valid brush color</returns>
    public static System.Windows.Media.Brush ConvertHexStringToBrush(string hex)
    {
      if ( string.IsNullOrWhiteSpace(hex) )
        return System.Windows.Media.Brushes.Black;

      var convertFromString = System.Windows.Media.ColorConverter.ConvertFromString(hex);

      if ( convertFromString == null )
        return System.Windows.Media.Brushes.Black;

      System.Windows.Media.Color color = (System.Windows.Media.Color) convertFromString;

      return new SolidColorBrush(color);
    }

    /// <summary>
    /// Shows a information MessageBox
    /// </summary>
    /// <param name="message">Message to show</param>
    public static void ShowInformationMessageBox(string message)
    {
      if ( string.IsNullOrWhiteSpace(message) )
        return;

      MessageBox.Show(message, ApplicationTitle, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    /// <summary>
    /// Shows a error MessageBox
    /// </summary>
    /// <param name="errorMessage">Error to show</param>
    public static void ShowErrorMessageBox(string errorMessage)
    {
      if ( string.IsNullOrWhiteSpace(errorMessage) )
        return;

      string caption = $"{ApplicationTitle} - {Application.Current.TryFindResource("Error")}";
      MessageBox.Show(errorMessage, caption, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    /// <summary>
    /// Show a question MessageBox
    /// </summary>
    /// <param name="question">Question to show</param>
    /// <param name="defaultMessageBoxResult">default MessageBoxResult is <c>MessageBoxResult.Yes</c></param>
    /// <returns>MessageBoxResult</returns>
    public static MessageBoxResult ShowQuestionMessageBox(string question, MessageBoxResult defaultMessageBoxResult = MessageBoxResult.Yes)
    {
      string caption = $"{ApplicationTitle} - {Application.Current.TryFindResource("Question")}";
      return string.IsNullOrWhiteSpace(question) ? MessageBoxResult.None :
        MessageBox.Show(question, caption, MessageBoxButton.YesNo, MessageBoxImage.Question, defaultMessageBoxResult);
    }

    /// <summary>
    /// Get all Enum RefreshRates
    /// </summary>
    /// <param name="s">Reference of refresh rate string</param>
    /// <returns>Enum of ETailRefreshRate</returns>
    public static ETailRefreshRate GetRefreshRate(string s)
    {
      if ( string.IsNullOrWhiteSpace(s) )
        return ETailRefreshRate.Normal;

      if ( Enum.GetNames(typeof(ETailRefreshRate)).All(refreshName => string.Compare(s.ToLower(), refreshName.ToLower(), StringComparison.Ordinal) != 0) )
        return ETailRefreshRate.Normal;

      Enum.TryParse(s, out ETailRefreshRate trr);

      return trr;
    }

    /// <summary>
    /// Get all Enum ThreadPriorities
    /// </summary>
    /// <param name="s">Reference of thread priority string</param>
    /// <returns>Enum from thread priority</returns>
    public static ThreadPriority GetThreadPriority(string s)
    {
      if ( string.IsNullOrWhiteSpace(s) )
        return System.Threading.ThreadPriority.Normal;

      if ( Enum.GetNames(typeof(ThreadPriority)).All(priorityName => string.Compare(s.ToLower(), priorityName.ToLower(), StringComparison.Ordinal) != 0) )
        return System.Threading.ThreadPriority.Normal;

      Enum.TryParse(s, out ThreadPriority tp);

      return tp;
    }

    private void IntializeObservableCollections()
    {
      // ThreadRefresh rate
      foreach ( ETailRefreshRate refreshName in Enum.GetValues(typeof(ETailRefreshRate)) )
      {
        RefreshRate.Add(refreshName);
      }

      // ThreadPriority
      foreach ( ThreadPriority priority in Enum.GetValues(typeof(ThreadPriority)) )
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

    #region CaseInsentiveEncodingInfoNameComparer

    private class CaseInsensitiveEncodingInfoNameComparer : IComparer
    {
      int IComparer.Compare(object x, object y)
      {
        const int result = 0;

        if ( !(x is EncodingInfo) || !(y is EncodingInfo) )
          return result;

        var xEncodingInfo = (EncodingInfo) x;
        var yEncodingInfo = (EncodingInfo) y;

        return new CaseInsensitiveComparer().Compare(xEncodingInfo.Name, yEncodingInfo.Name);
      }
    }

    #endregion
  }
}
