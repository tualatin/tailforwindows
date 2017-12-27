using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
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
    private static EnvironmentContainer instance;

    /// <summary>
    /// Current instance
    /// </summary>
    public static EnvironmentContainer Instance => instance ?? (instance = new EnvironmentContainer());

    private readonly ISettingsHelper _settings;
    private readonly IXmlReader _xmlReader;


    private EnvironmentContainer()
    {
      _settings = new SettingsHelper();

      // For test purposes
      if ( System.Reflection.Assembly.GetEntryAssembly() != null )
        _xmlReader = new XmlConfigReadController();

      IntializeObservableCollections();
    }

    /// <summary>
    /// Application title
    /// </summary>
    public string ApplicationTitle => Application.Current.TryFindResource("ApplicationTitle").ToString();

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
    /// Read current settings
    /// </summary>
    public async Task ReadSettings()
    {
      await _settings.ReadSettingsAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Save current settings
    /// </summary>
    /// <returns></returns>
    public async Task SaveSettings()
    {
      await _settings.SaveSettingsAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Reload settings from config file
    /// </summary>
    /// <returns></returns>
    public async Task ReloadSettings()
    {
      await _settings.ReloadCurrentSettingsAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Create default T4W font
    /// </summary>
    /// <returns>Default font configuration</returns>
    public static Font CreateDefaultFont()
    {
      return new Font("Segoe UI", 11f, System.Drawing.FontStyle.Regular);
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

    private void IntializeObservableCollections()
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
