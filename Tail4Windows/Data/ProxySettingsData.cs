using Org.Vs.TailForWin.Data.Base;


namespace Org.Vs.TailForWin.Data
{
  /// <summary>
  /// ProxySettingsData object
  /// </summary>
  public class ProxySettingsData : NotifyMaster
  {
    private bool useSystemSettings;

    /// <summary>
    /// Use system settings
    /// </summary>
    public bool UseSystemSettings
    {
      get => useSystemSettings;
      set
      {
        useSystemSettings = value;
        OnPropertyChanged("UseSystemSettings");
      }
    }

    private bool useProxy;

    /// <summary>
    /// Use proxy server
    /// </summary>
    public bool UseProxy
    {
      get => useProxy;
      set
      {
        useProxy = value;
        OnPropertyChanged("UseProxy");
      }
    }

    private int proxyPort;

    /// <summary>
    /// Proxy port
    /// </summary>
    public int ProxyPort
    {
      get => proxyPort;
      set
      {
        proxyPort = value;
        OnPropertyChanged("ProxyPort");
      }
    }

    private string proxyUrl;

    /// <summary>
    /// Proxy server url
    /// </summary>
    public string ProxyUrl
    {
      get => proxyUrl;
      set
      {
        proxyUrl = value;
        OnPropertyChanged("ProxyUrl");
      }
    }

    private string userName;

    /// <summary>
    /// Username
    /// </summary>
    public string UserName
    {
      get => userName;
      set
      {
        userName = value;
        OnPropertyChanged("UserName");
      }
    }

    private string password;

    /// <summary>
    /// Password
    /// </summary>
    public string Password
    {
      get => password;
      set
      {
        password = value;
        OnPropertyChanged("Password");
      }
    }
  }
}
