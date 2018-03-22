using System;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Interfaces;


namespace Org.Vs.TailForWin.Core.Data.Settings
{
  /// <summary>
  /// Alert settings
  /// </summary>
  public class AlertSetting : NotifyMaster, ICloneable, IPropertyNotify
  {
    private bool _bringToFront;

    /// <summary>
    /// Bring mainwindow to front when alert occurs
    /// </summary>
    public bool BringToFront
    {
      get => _bringToFront;
      set
      {
        if ( _bringToFront == value )
          return;

        _bringToFront = value;
        OnPropertyChanged(nameof(BringToFront));
      }
    }

    private bool _popupWnd;

    /// <summary>
    /// Show Popup window when alert occurs
    /// </summary>
    public bool PopupWnd
    {
      get => _popupWnd;
      set
      {
        if ( _popupWnd == value )
          return;

        _popupWnd = value;
        OnPropertyChanged(nameof(PopupWnd));
      }
    }

    private bool _playSoundFile;

    /// <summary>
    /// Play sound file when alert occurs
    /// </summary>
    public bool PlaySoundFile
    {
      get => _playSoundFile;
      set
      {
        if ( _playSoundFile == value )
          return;

        _playSoundFile = value;
        OnPropertyChanged(nameof(PlaySoundFile));
      }
    }

    private bool _sendMail;

    /// <summary>
    /// Send E-Mail when alert occurs
    /// </summary>
    public bool SendMail
    {
      get => _sendMail;
      set
      {
        if ( _sendMail == value )
          return;

        _sendMail = value;
        OnPropertyChanged(nameof(SendMail));
      }
    }

    private string _soundFileNameFullPath;

    /// <summary>
    /// Name of sound file to play when alert occurs
    /// </summary>
    public string SoundFileNameFullPath
    {
      get => _soundFileNameFullPath;
      set
      {
        if ( Equals(_soundFileNameFullPath, value) )
          return;

        _soundFileNameFullPath = value;
        SoundFileName = System.IO.Path.GetFileName(_soundFileNameFullPath);

        OnPropertyChanged(nameof(SoundFileNameFullPath));
      }
    }

    private string _soundFileName;

    /// <summary>
    /// Name of sound file without path
    /// </summary>
    public string SoundFileName
    {
      get => _soundFileName;
      set
      {
        if ( Equals(_soundFileName, value) )
          return;

        _soundFileName = value;
        OnPropertyChanged(nameof(SoundFileName));
      }
    }

    private string _emailAddress;

    /// <summary>
    /// E-Mail address to send E-Mail when alert occurs
    /// </summary>
    public string MailAddress
    {
      get => _emailAddress;
      set
      {
        if ( Equals(_emailAddress, value) )
          return;

        _emailAddress = value;
        OnPropertyChanged(nameof(MailAddress));
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
