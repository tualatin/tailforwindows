using TailForWin.Utils;


namespace TailForWin.Data
{
  public class ProxySettingsData : INotifyMaster
  {
    private bool useSystemSettings;

    /// <summary>
    /// Use system settings
    /// </summary>
    public bool UseSystemSettings
    {
      get
      {
        return (useSystemSettings);
      }
      set
      {
        useSystemSettings = value;
        OnPropertyChanged ("UseSystemSettings");
      }
    }

    private bool useProxy;

    /// <summary>
    /// Use proxy server
    /// </summary>
    public bool UseProxy
    {
      get
      {
        return (useProxy);
      }
      set
      {
        useProxy = value;
        OnPropertyChanged ("UseProxy");
      }
    }

    private int proxyPort;

    /// <summary>
    /// Proxy port
    /// </summary>
    public int ProxyPort
    {
      get
      {
        return (proxyPort);
      }
      set
      {
        proxyPort = value;
        OnPropertyChanged ("ProxyPort");
      }
    }

    private string proxyUrl;

    /// <summary>
    /// Proxy server url
    /// </summary>
    public string ProxyUrl
    {
      get
      {
        return (proxyUrl);
      }
      set
      {
        proxyUrl = value;
        OnPropertyChanged ("ProxyUrl");
      }
    }

    private string userName;

    /// <summary>
    /// Username
    /// </summary>
    public string UserName
    {
      get
      {
        return (userName);
      }
      set
      {
        userName = value;
      }
    }

    private string password;

    /// <summary>
    /// Password
    /// </summary>
    public string Password
    {
      get
      {
        return (password);
      }
      set
      {
        password = value;
        OnPropertyChanged ("Password");
      }
    }
  }
}
