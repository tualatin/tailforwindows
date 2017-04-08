using Org.Vs.TailForWin.Data.Base;


namespace Org.Vs.TailForWin.Data
{
  /// <summary>
  /// SmtpData object
  /// </summary>
  public class SmtpData : NotifyMaster
  {
    private string smtpServerName;

    /// <summary>
    /// SMTP server name
    /// </summary>
    public string SmtpServerName
    {
      get => smtpServerName;
      set
      {
        smtpServerName = value;
        OnPropertyChanged("SmtpServerName");
      }
    }

    private int smtpPort;

    /// <summary>
    /// SMTP server port
    /// </summary>
    public int SmtpPort
    {
      get => smtpPort;
      set
      {
        smtpPort = value;
        OnPropertyChanged("SmtpPort");
      }
    }

    private string loginName;

    /// <summary>
    /// E-Mail-Server login name
    /// </summary>
    public string LoginName
    {
      get => loginName;
      set
      {
        loginName = value;
        OnPropertyChanged("LoginName");
      }

    }

    private string password;

    /// <summary>
    /// E-Mail-Server password
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

    private string fromAddress;

    /// <summary>
    /// E-Mail address From
    /// </summary>
    public string FromAddress
    {
      get => fromAddress;
      set
      {
        fromAddress = value;
        OnPropertyChanged("FromAddress");
      }
    }

    private string subject;

    /// <summary>
    /// Subject
    /// </summary>
    public string Subject
    {
      get => subject;
      set
      {
        subject = value;
        OnPropertyChanged("Subject");
      }
    }

    private bool ssl;

    /// <summary>
    /// Use SSL
    /// </summary>
    public bool SSL
    {
      get => ssl;
      set
      {
        ssl = value;
        OnPropertyChanged("SSL");
      }
    }

    private bool tls;

    /// <summary>
    /// Use TLS
    /// </summary>
    public bool TLS
    {
      get => tls;
      set
      {
        tls = value;
        OnPropertyChanged("TLS");
      }
    }
  }
}
