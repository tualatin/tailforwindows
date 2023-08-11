using System.ComponentModel;
using Newtonsoft.Json;
using Org.Vs.Tail4Win.Shared.Data.Base;
using Org.Vs.Tail4Win.Shared.Interfaces;

namespace Org.Vs.Tail4Win.Shared.Data.Settings
{
  /// <summary>
  /// Alert settings
  /// </summary>
  public class AlertSetting : NotifyMaster, ICloneable, IPropertyNotify
  {
    private bool _bringToFront;

    /// <summary>
    /// Bring MainWindow to front when alert occurs
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.AlertBringToFront)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool BringToFront
    {
      get => _bringToFront;
      set
      {
        if ( _bringToFront == value )
          return;

        _bringToFront = value;
        OnPropertyChanged();
      }
    }

    private bool _popupWnd;

    /// <summary>
    /// Show Popup window when alert occurs
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.AlertPopUpWindow)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool PopupWnd
    {
      get => _popupWnd;
      set
      {
        if ( _popupWnd == value )
          return;

        _popupWnd = value;
        OnPropertyChanged();
      }
    }

    private bool _playSoundFile;

    /// <summary>
    /// Play sound file when alert occurs
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.AlertPlaySoundFile)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool PlaySoundFile
    {
      get => _playSoundFile;
      set
      {
        if ( _playSoundFile == value )
          return;

        _playSoundFile = value;
        OnPropertyChanged();
      }
    }

    private bool _sendMail;

    /// <summary>
    /// Send E-Mail when alert occurs
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.AlertSendMail)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool SendMail
    {
      get => _sendMail;
      set
      {
        if ( _sendMail == value )
          return;

        _sendMail = value;
        OnPropertyChanged();
      }
    }

    private string _soundFileNameFullPath;

    /// <summary>
    /// Name of sound file to play when alert occurs
    /// </summary>
    [DefaultValue("")]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public string SoundFileNameFullPath
    {
      get => _soundFileNameFullPath;
      set
      {
        if ( Equals(_soundFileNameFullPath, value) )
          return;

        _soundFileNameFullPath = value;
        SoundFileName = System.IO.Path.GetFileName(_soundFileNameFullPath);

        OnPropertyChanged();
      }
    }

    private string _soundFileName;

    /// <summary>
    /// Name of sound file without path
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.AlertSoundFile)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public string SoundFileName
    {
      get => _soundFileName;
      private set
      {
        if ( Equals(_soundFileName, value) )
          return;

        _soundFileName = value;
        OnPropertyChanged();
      }
    }

    private string _emailAddress;

    /// <summary>
    /// E-Mail address to send E-Mail when alert occurs
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.AlertMailAddress)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public string MailAddress
    {
      get => _emailAddress;
      set
      {
        if ( Equals(_emailAddress, value) )
          return;

        _emailAddress = value;
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
