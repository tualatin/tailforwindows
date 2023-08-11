using System.ComponentModel;
using Newtonsoft.Json;
using Org.Vs.Tail4Win.Shared.Data.Base;
using Org.Vs.Tail4Win.Shared.Interfaces;

namespace Org.Vs.Tail4Win.Shared.Data.Settings
{
  /// <summary>
  /// Proxy settings
  /// </summary>
  public class ProxySetting : NotifyMaster, ICloneable, IPropertyNotify
  {
    private bool? _useSystemSettings;

    /// <summary>
    /// Use system settings
    /// Three state <see cref="bool"/> 
    /// 1. <c>Null</c> == No Proxy
    /// 2. <c>False</c> == Manual proxy setting
    /// 3. <c>True</c> == use system proxy settings
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.ProxyUseSystemSettings)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool? UseSystemSettings
    {
      get => _useSystemSettings;
      set
      {
        if ( value == _useSystemSettings )
          return;

        _useSystemSettings = value;
        OnPropertyChanged();
      }
    }

    private int _proxyPort;

    /// <summary>
    /// Proxy port
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.ProxyPort)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public int ProxyPort
    {
      get => _proxyPort;
      set
      {
        if ( value == _proxyPort )
          return;

        _proxyPort = value;
        OnPropertyChanged();
      }
    }

    private string _proxyUrl;

    /// <summary>
    /// Proxy server url
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.ProxyUrl)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public string ProxyUrl
    {
      get => _proxyUrl;
      set
      {
        if ( Equals(value, _proxyUrl) )
          return;

        _proxyUrl = value;
        OnPropertyChanged();
      }
    }

    private string _userName;

    /// <summary>
    /// Username
    /// </summary>
    [DefaultValue("")]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public string UserName
    {
      get => _userName;
      set
      {
        if ( Equals(value, _userName) )
          return;

        _userName = value;
        OnPropertyChanged();
      }
    }

    private string _password;

    /// <summary>
    /// Password
    /// </summary>
    [JsonIgnore]
    public string Password
    {
      get => _password;
      set
      {
        if ( Equals(value, _password) )
          return;

        _password = value;
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Clone the object
    /// </summary>
    /// <returns>Cloned object</returns>
    public object Clone() => MemberwiseClone();

    /// <summary>
    /// Call OnPropertyChanged
    /// </summary>
    public void RaiseOnPropertyChanged() => OnPropertyChanged();
  }
}
