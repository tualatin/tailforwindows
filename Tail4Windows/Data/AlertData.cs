using Org.Vs.TailForWin.Data.Base;


namespace Org.Vs.TailForWin.Data
{
  /// <summary>
  /// AlertData object
  /// </summary>
  public class AlertData : NotifyMaster
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public AlertData()
    {
      SmtpSettings = new SmtpData();
    }

    private bool bringToFront;

    /// <summary>
    /// Bring mainwindow to front when alert occurs
    /// </summary>
    public bool BringToFront
    {
      get
      {
        return (bringToFront);
      }
      set
      {
        bringToFront = value;
        OnPropertyChanged("BringToFront");
      }
    }

    private bool popupWnd;

    /// <summary>
    /// Show Popup window when alert occurs
    /// </summary>
    public bool PopupWnd
    {
      get
      {
        return (popupWnd);
      }
      set
      {
        popupWnd = value;
        OnPropertyChanged("PopupWnd");
      }
    }

    private bool playSoundFile;

    /// <summary>
    /// Play sound file when alert occurs
    /// </summary>
    public bool PlaySoundFile
    {
      get
      {
        return (playSoundFile);
      }
      set
      {
        playSoundFile = value;
        OnPropertyChanged("PlaySoundFile");
      }
    }

    private bool sendEMail;

    /// <summary>
    /// Send E-Mail when alert occurs
    /// </summary>
    public bool SendEMail
    {
      get
      {
        return (sendEMail);
      }
      set
      {
        sendEMail = value;
        OnPropertyChanged("SendEMail");
      }
    }

    private string soundFileNameFullPath;

    /// <summary>
    /// Name of sound file to play when alert occurs
    /// </summary>
    public string SoundFileNameFullPath
    {
      get
      {
        return (soundFileNameFullPath);
      }
      set
      {
        soundFileNameFullPath = value;
        SoundFileName = System.IO.Path.GetFileName(soundFileNameFullPath);
        OnPropertyChanged("SoundFileNameFullPath");
      }
    }

    private string soundFileName;

    /// <summary>
    /// Name of sound file without path
    /// </summary>
    public string SoundFileName
    {
      get
      {
        return (soundFileName);
      }
      set
      {
        soundFileName = value;
        OnPropertyChanged("SoundFileName");
      }
    }

    private string emailAddress;

    /// <summary>
    /// E-Mail address to send E-Mail when alert occurs
    /// </summary>
    public string EMailAddress
    {
      get
      {
        return (emailAddress);
      }
      set
      {
        emailAddress = value;
        OnPropertyChanged("EMailAddress");
      }
    }

    /// <summary>
    /// SMTP server settings
    /// </summary>
    public SmtpData SmtpSettings
    {
      get;
      set;
    }
  }
}
