using System;
using Org.Vs.TailForWin.Core.Data.Base;


namespace Org.Vs.TailForWin.Core.Data.Settings
{
  /// <summary>
  /// Proxy settings
  /// </summary>
  public class ProxySetting : NotifyMaster, ICloneable
  {
    private bool _useSystemSettings;

    /// <summary>
    /// Use system settings
    /// </summary>
    public bool UseSystemSettings
    {
      get => _useSystemSettings;
      set
      {
        _useSystemSettings = value;
        OnPropertyChanged(nameof(UseSystemSettings));
      }
    }

    private bool _useProxy;

    /// <summary>
    /// Use proxy server
    /// </summary>
    public bool UseProxy
    {
      get => _useProxy;
      set
      {
        _useProxy = value;
        OnPropertyChanged(nameof(UseProxy));
      }
    }

    private int _proxyPort;

    /// <summary>
    /// Proxy port
    /// </summary>
    public int ProxyPort
    {
      get => _proxyPort;
      set
      {
        _proxyPort = value;
        OnPropertyChanged(nameof(ProxyPort));
      }
    }

    private string _proxyUrl;

    /// <summary>
    /// Proxy server url
    /// </summary>
    public string ProxyUrl
    {
      get => _proxyUrl;
      set
      {
        _proxyUrl = value;
        OnPropertyChanged(nameof(ProxyUrl));
      }
    }

    private string _userName;

    /// <summary>
    /// Username
    /// </summary>
    public string UserName
    {
      get => _userName;
      set
      {
        _userName = value;
        OnPropertyChanged(nameof(UserName));
      }
    }

    private string _password;

    /// <summary>
    /// Password
    /// </summary>
    public string Password
    {
      get => _password;
      set
      {
        _password = value;
        OnPropertyChanged(nameof(Password));
      }
    }

    /// <summary>
    /// Clone the object
    /// </summary>
    /// <returns>Cloned object</returns>
    public object Clone() => MemberwiseClone();
  }
}
