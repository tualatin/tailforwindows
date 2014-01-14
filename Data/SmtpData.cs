namespace TailForWin.Data
{
  public class SmtpData : INotifyMaster
  {
    private string smtpServerName;

    /// <summary>
    /// SMTP server name
    /// </summary>
    public string SmtpServerName
    {
      get
      {
        return (smtpServerName);
      }
      set
      {
        smtpServerName = value;
        OnPropertyChanged ("SmtpServerName");
      }
    }

    private int smtpPort;

    /// <summary>
    /// SMTP server port
    /// </summary>
    public int SmtpPort
    {
      get
      {
        return (smtpPort);
      }
      set
      {
        smtpPort = value;
        OnPropertyChanged ("SmtpPort");
      }
    }

    private string loginName;

    /// <summary>
    /// E-Mail-Server login name
    /// </summary>
    public string LoginName
    {
      get
      {
        return (loginName);
      }
      set
      {
        loginName = value;
        OnPropertyChanged ("LoginName");
      }

    }

    private string password;

    /// <summary>
    /// E-Mail-Server password
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
