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
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Data.Mappings;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Interfaces;


namespace Org.Vs.TailForWin.Core.Utils
{
  /// <summary>
  /// Environment container for T4W as singelton
  /// </summary>
  public class EnvironmentContainer
  {
    private static EnvironmentContainer instance;

    /// <summary>
    /// Current instance
    /// </summary>
    public static EnvironmentContainer Instance => instance ?? (instance = new EnvironmentContainer());

    private readonly ISettingsHelper _settings;


    private EnvironmentContainer()
    {
      _settings = new SettingsHelperController();
      UpTime = DateTime.Now;
      CurrentEventManager = new EventAggregator();

      NotifyTaskCompletion.Create(IntializeObservableCollectionsAsync);
    }

    /// <summary>
    /// Application title
    /// </summary>
    public static string ApplicationTitle => Application.Current.TryFindResource("ApplicationTitle").ToString();

    /// <summary>
    /// Application Regex compare URL
    /// </summary>
    public static string ApplicationRegexWebUrl => "https://www.virtual-studios.de";

    /// <summary>
    /// Application Update URL
    /// </summary>
    public static string ApplicationUpdateWebUrl => "https://www.virtual-studios.de/tail4wnd/releases.txt";

    /// <summary>
    /// Application release URL
    /// </summary>
    public static string ApplicationReleaseWebUrl => "https://github.com/tualatin/tailforwindows/releases";

    /// <summary>
    /// Application donate web URL
    /// </summary>
    public static string ApplicationDonateWebUrl => "https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=M436BDAMQL7WE";

    /// <summary>
    /// Unlimited log line value
    /// </summary>
    public const int UnlimitedLogLineValue = 15000;

    /// <summary>
    /// Minimum log line value
    /// </summary>
    public const int MinLogLineValue = 3000;

    /// <summary>
    /// Minimum SmartWatch interval
    /// </summary>
    public const int MinSmartWatchInterval = 2000;

    /// <summary>
    /// Maximum SmartWatch interval
    /// </summary>
    public const int MaxSmartWatchInterval = 120000;

    /// <summary>
    /// Current event manager
    /// </summary>
    public readonly IEventAggregator CurrentEventManager;

    /// <summary>
    /// List of supported file encodings
    /// </summary>
    public ObservableCollection<Encoding> FileEncoding
    {
      get;
    } = new ObservableCollection<Encoding>();

    /// <summary>
    /// List of supported refresh rates
    /// </summary>
    public ObservableCollection<RefreshRateMapping> RefreshRate
    {
      get;
    } = new ObservableCollection<RefreshRateMapping>();

    /// <summary>
    /// List of thread priority of type <see cref="ThreadPriorityMapping"/>
    /// </summary>
    public ObservableCollection<ThreadPriorityMapping> ThreadPriority
    {
      get;
    } = new ObservableCollection<ThreadPriorityMapping>();

    /// <summary>
    /// List of DateFormats of type <see cref="DateFormatMapping"/>
    /// </summary>
    public ObservableCollection<DateFormatMapping> DateFormat
    {
      get;
    } = new ObservableCollection<DateFormatMapping>();

    /// <summary>
    /// List of TimeFormats of type <see cref="TimeFormatMapping"/>
    /// </summary>
    public ObservableCollection<TimeFormatMapping> TimeFormat
    {
      get;
    } = new ObservableCollection<TimeFormatMapping>();

    /// <summary>
    /// List of languages of type <see cref="LanguageMapping"/>
    /// </summary>
    public ObservableCollection<LanguageMapping> Languages
    {
      get;
      private set;
    }

    /// <summary>
    /// List of window styles if type <see cref="WindowStyleMapping"/>
    /// </summary>
    public ObservableCollection<WindowStyleMapping> WindowStyles
    {
      get;
    } = new ObservableCollection<WindowStyleMapping>();

    /// <summary>
    /// FileSort of type <see cref="FileSortMapping"/>
    /// </summary>
    public ObservableCollection<FileSortMapping> FileSort
    {
      get;
    } = new ObservableCollection<FileSortMapping>();

    /// <summary>
    /// List of <see cref="ESmartWatchMode"/>
    /// </summary>
    public ObservableCollection<SmartWatchMapping> SmartWatchModes
    {
      get;
    } = new ObservableCollection<SmartWatchMapping>();

    /// <summary>
    /// Current application path
    /// </summary>
    public static string ApplicationPath => Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

    /// <summary>
    /// TailStore path
    /// </summary>
    public static string TailStorePath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\{ApplicationTitle}";

    /// <summary>
    /// Read current settings
    /// </summary>
    /// <returns>Task</returns>
    public async Task ReadSettingsAsync() => await _settings.ReadSettingsAsync().ConfigureAwait(false);

    /// <summary>
    /// Read current settings
    /// </summary>
    /// <param name="cts"><see cref="CancellationTokenSource"/></param>
    /// <returns>Task</returns>
    public async Task ReadSettingsAsync(CancellationTokenSource cts) => await _settings.ReadSettingsAsync(cts).ConfigureAwait(false);

    /// <summary>
    /// Save current settings
    /// </summary>
    /// <param name="cts">CancellationTokenSource</param>
    /// <returns>Task</returns>
    public async Task SaveSettingsAsync(CancellationTokenSource cts) => await _settings.SaveSettingsAsync(cts).ConfigureAwait(false);

    /// <summary>
    /// Reload settings from config file
    /// </summary>
    /// <param name="cts"><see cref="CancellationTokenSource"/></param>
    /// <returns>Task</returns>
    public async Task ReloadSettingsAsync(CancellationTokenSource cts) => await _settings.ReloadCurrentSettingsAsync(cts).ConfigureAwait(false);

    /// <summary>
    /// Reset current setting to default values
    /// </summary>
    /// <param name="cts">CancellationTokenSource</param>
    /// <returns>Task</returns>
    public async Task ResetCurrentSettingsAsync(CancellationTokenSource cts) => await _settings.SetDefaultSettingsAsync(cts).ConfigureAwait(false);

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
    }

    /// <summary>
    /// Converts a hex string to brush color
    /// </summary>
    /// <param name="hex">Color as hex string</param>
    /// <returns>A valid brush color</returns>
    public static System.Windows.Media.Brush ConvertHexStringToBrush(string hex) => ConvertHexStringToBrush(hex, System.Windows.Media.Brushes.Black);

    /// <summary>
    /// Converts a hex string to brush color
    /// </summary>
    /// <param name="hex">Color as hex string</param>
    /// <param name="defaultValue">Default brush</param>
    /// <returns>A valid brush color</returns>
    public static System.Windows.Media.Brush ConvertHexStringToBrush(string hex, System.Windows.Media.Brush defaultValue)
    {
      if ( string.IsNullOrWhiteSpace(hex) )
        return defaultValue;

      var convertFromString = System.Windows.Media.ColorConverter.ConvertFromString(hex);
      return convertFromString == null ? defaultValue : new SolidColorBrush((System.Windows.Media.Color) convertFromString);
    }

    /// <summary>
    /// Converts a <see cref="System.Windows.Media.Brush"/> to <see cref="System.Drawing.Color"/>
    /// </summary>
    /// <param name="brush">Brush to convert</param>
    /// <returns>Color of type <see cref="System.Drawing.Color"/></returns>
    public static System.Drawing.Color ConvertMediaBrushToDrawingColor(System.Windows.Media.Brush brush)
    {
      var mediaColor = ((SolidColorBrush) brush).Color;
      return System.Drawing.Color.FromArgb(mediaColor.A, mediaColor.R, mediaColor.G, mediaColor.B);
    }

    /// <summary>
    /// Converts a string to <see cref="System.Drawing.Color"/>
    /// </summary>
    /// <param name="hex">Color to convert</param>
    /// <param name="defaultValue">Default color</param>
    /// <returns>Color of type <see cref="System.Drawing.Color"/></returns>
    public static System.Drawing.Color ConvertStringToDrawingColor(string hex, System.Drawing.Color defaultValue)
    {
      if ( string.IsNullOrWhiteSpace(hex) )
        return defaultValue;

      var convertFromString = ColorTranslator.FromHtml(hex);
      return convertFromString;
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

    /// <summary>
    /// Init all collections
    /// </summary>
    /// <returns>Task</returns>
    public async Task IntializeObservableCollectionsAsync()
    {
      await Task.Run(
        () =>
        {
          // ThreadRefresh rate
          foreach ( ETailRefreshRate refreshName in Enum.GetValues(typeof(ETailRefreshRate)) )
          {
            RefreshRate.Add(
              new RefreshRateMapping
              {
                RefreshRate = refreshName
              });
          }

          // ThreadPriority
          foreach ( ThreadPriority priority in Enum.GetValues(typeof(ThreadPriority)) )
          {
            ThreadPriority.Add(
              new ThreadPriorityMapping
              {
                ThreadPriority = priority
              });
          }

          // DateFormat
          foreach ( EDateFormat format in Enum.GetValues(typeof(EDateFormat)) )
          {
            DateFormat.Add(
              new DateFormatMapping
              {
                DateFormat = format
              });
          }

          // TimeFormat
          foreach ( ETimeFormat format in Enum.GetValues(typeof(ETimeFormat)) )
          {
            TimeFormat.Add(
              new TimeFormatMapping
              {
                TimeFormat = format
              });
          }

          // Languages
          var languages = new ObservableCollection<LanguageMapping>();

          foreach ( EUiLanguage language in Enum.GetValues(typeof(EUiLanguage)) )
          {
            languages.Add(
              new LanguageMapping
              {
                Language = language
              });
          }

          Languages = new ObservableCollection<LanguageMapping>(languages.OrderBy(p => p.Description));

          // WindowStyle
          foreach ( EWindowStyle style in Enum.GetValues(typeof(EWindowStyle)) )
          {
            WindowStyles.Add(new WindowStyleMapping
            {
              WindowStyle = style
            });
          }

          // FileSort
          foreach ( EFileSort fileSort in Enum.GetValues(typeof(EFileSort)) )
          {
            FileSort.Add(new FileSortMapping
            {
              FileSort = fileSort
            });
          }

          foreach ( ESmartWatchMode smMode in Enum.GetValues(typeof(ESmartWatchMode)) )
          {
            SmartWatchModes.Add(new SmartWatchMapping
            {
              SmartWatchMode = smMode
            });
          }

          // Fileencoding
          EncodingInfo[] encodings = Encoding.GetEncodings();
          Array.Sort(encodings, new CaseInsensitiveEncodingInfoNameComparer());
          Array.ForEach(encodings, fileEncode => FileEncoding.Add(fileEncode.GetEncoding()));
        }).ConfigureAwait(false);
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
