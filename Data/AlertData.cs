namespace TailForWin.Data
{
  public class AlertData : INotifyMaster
  {
    private bool bringToFront;

    /// <summary>
    /// Bring mainwindow to front when alert occourse
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
        OnPropertyChanged ("BringToFront");
      }
    }

    private bool playSoundFile;

    /// <summary>
    /// Play sound file when alert occourse
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
        OnPropertyChanged ("PlaySoundFile");
      }
    }

    private bool sendEMail;

    /// <summary>
    /// Send E-Mail when alert occourse
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
        OnPropertyChanged ("SendEMail");
      }
    }

    private string soundFileNameFullPath;

    /// <summary>
    /// Name of sound file to play when alert occourse
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
        SoundFileName = System.IO.Path.GetFileName (soundFileNameFullPath);
        OnPropertyChanged ("SoundFileNameFullPath");
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
        OnPropertyChanged ("SoundFileName");
      }
    }

    private string emailAddress;

    /// <summary>
    /// E-Mail address to send E-Mail when alert occourse
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
        OnPropertyChanged ("EMailAddress");
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
