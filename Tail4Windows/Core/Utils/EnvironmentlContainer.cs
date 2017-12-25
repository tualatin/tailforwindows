using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Interfaces;


namespace Org.Vs.TailForWin.Core.Utils
{
  /// <summary>
  /// Environment container for T4W
  /// </summary>
  public class EnvironmentlContainer
  {
    private static EnvironmentlContainer instance;

    /// <summary>
    /// Current instance
    /// </summary>
    public static EnvironmentlContainer Instance => instance ?? (instance = new EnvironmentlContainer());

    private readonly ISettingsHelper _settings;
    private readonly IXmlReader _xmlReader;


    private EnvironmentlContainer()
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
    async public Task ReadSettings()
    {
      await _settings.ReadSettingsAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Save current settings
    /// </summary>
    /// <returns></returns>
    async public Task SaveSettings()
    {
      await _settings.SaveSettingsAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Reload settings from config file
    /// </summary>
    /// <returns></returns>
    async public Task ReloadSettings()
    {
      await _settings.ReloadCurrentSettingsAsync().ConfigureAwait(false);
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
