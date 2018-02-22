using System;
using Org.Vs.TailForWin.Core.Data.Base;


namespace Org.Vs.TailForWin.Core.Data.Settings
{
  /// <summary>
  /// Proxy settings
  /// </summary>
  public class ProxySetting : NotifyMaster, ICloneable
  {
    private bool? _useSystemSettings;

    /// <summary>
    /// Use system settings
    /// Three state <see cref="bool"/> 
    /// 1. <c>Null</c> == No Proxy
    /// 2. <c>False</c> == Manual proxy setting
    /// 3. <c>True</c> == use system proxy settings
    /// </summary>
    public bool? UseSystemSettings
    {
      get => _useSystemSettings;
      set
      {
        if ( value == _useSystemSettings )
          return;

        _useSystemSettings = value;
        OnPropertyChanged(nameof(UseSystemSettings));
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
        if ( value == _proxyPort )
          return;

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
        if ( Equals(value, _proxyUrl) )
          return;

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
        if ( Equals(value, _userName) )
          return;

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
        if ( Equals(value, _password) )
          return;

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
