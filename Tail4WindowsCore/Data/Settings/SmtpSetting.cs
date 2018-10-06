using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Interfaces;


namespace Org.Vs.TailForWin.Core.Data.Settings
{
  /// <summary>
  /// SMTP settings
  /// </summary>
  public class SmtpSetting : NotifyMaster, ICloneable, IPropertyNotify
  {
    private string _smtpServerName;

    /// <summary>
    /// SMTP server name
    /// </summary>
    [DefaultValue("")]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public string SmtpServerName
    {
      get => _smtpServerName;
      set
      {
        if ( Equals(_smtpServerName, value) )
          return;

        _smtpServerName = value;
        OnPropertyChanged(nameof(SmtpServerName));
      }
    }

    private int _smtpPort;

    /// <summary>
    /// SMTP server port
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.SmtpPort)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public int SmtpPort
    {
      get => _smtpPort;
      set
      {
        if ( _smtpPort == value )
          return;

        _smtpPort = value;
        OnPropertyChanged(nameof(SmtpPort));
      }
    }

    private string _loginName;

    /// <summary>
    /// E-Mail-Server login name
    /// </summary>
    [DefaultValue("")]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public string LoginName
    {
      get => _loginName;
      set
      {
        if ( Equals(_loginName, value) )
          return;

        _loginName = value;
        OnPropertyChanged(nameof(LoginName));
      }

    }

    private string _password;

    /// <summary>
    /// E-Mail-Server password
    /// </summary>
    [JsonIgnore]
    public string Password
    {
      get => _password;
      set
      {
        if ( Equals(_password, value) )
          return;

        _password = value;
        OnPropertyChanged(nameof(Password));
      }
    }

    private string _fromAddress;

    /// <summary>
    /// E-Mail address From
    /// </summary>
    [DefaultValue("")]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public string FromAddress
    {
      get => _fromAddress;
      set
      {
        if ( Equals(_fromAddress, value) )
          return;

        _fromAddress = value;
        OnPropertyChanged(nameof(FromAddress));
      }
    }

    private string _subject;

    /// <summary>
    /// Subject
    /// </summary>
    [DefaultValue("")]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public string Subject
    {
      get => _subject;
      set
      {
        if ( Equals(_subject, value) )
          return;

        _subject = value;
        OnPropertyChanged(nameof(Subject));
      }
    }

    private bool _ssl;

    /// <summary>
    /// Use SSL
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.SmtpSsl)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool Ssl
    {
      get => _ssl;
      set
      {
        if ( _ssl == value )
          return;

        _ssl = value;
        OnPropertyChanged(nameof(Ssl));
      }
    }

    private bool _tls;

    /// <summary>
    /// Use TLS
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.SmtpTls)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool Tls
    {
      get => _tls;
      set
      {
        if ( _tls == value )
          return;

        _tls = value;
        OnPropertyChanged(nameof(Tls));
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
