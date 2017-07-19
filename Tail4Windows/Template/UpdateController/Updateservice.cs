using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Org.Vs.TailForWin.Template.UpdateController.Data;


namespace Org.Vs.TailForWin.Template.UpdateController
{
  /// <summary>
  /// Update service
  /// </summary>
  public class Updateservice : IDisposable
  {
    private Webservice webservice;
    private BackgroundWorker updateThread;
    private string webData = string.Empty;

    /// <summary>
    /// Event, when update process finished
    /// </summary>
    public event EventHandler ThreadCompletedEvent;


    #region Public properties

    /// <summary>
    /// Proxy server address
    /// </summary>
    public string Proxy
    {
      get;
      set;
    }

    /// <summary>
    /// Proxy server port
    /// </summary>
    public int ProxyPort
    {
      get;
      set;
    }

    /// <summary>
    /// URL with updater webaddress
    /// </summary>
    public string UpdateUrl
    {
      get;
      set;
    }

    /// <summary>
    /// Use proxy yes/no
    /// </summary>
    public bool UseProxy
    {
      get;
      set;
    }

    /// <summary>
    /// Use proxy settings from system
    /// </summary>
    public bool UseSystemSettings
    {
      get;
      set;
    }

    /// <summary>
    /// Proxy authentification
    /// </summary>
    public System.Net.NetworkCredential ProxyAuthentification
    {
      get;
      set;
    }

    /// <summary>
    /// Is thread completed yes/no
    /// </summary>
    public bool IsThreadCompleted
    {
      get;
      set;
    }

    /// <summary>
    /// Is update necessary
    /// </summary>
    public bool HaveToUpdate
    {
      get;
      set;
    }

    /// <summary>
    /// WebVersion of application
    /// </summary>
    public Version WebVersion
    {
      get;
      private set;
    }

    /// <summary>
    /// Application version
    /// </summary>
    public Version AppVersion
    {
      get;
      private set;
    }

    /// <summary>
    /// Update process was successful
    /// </summary>
    public bool Success
    {
      get;
      set;
    }

    #endregion

    /// <summary>
    /// Releases all resources used by the Updateservice.
    /// </summary>
    public void Dispose()
    {
      if ( updateThread == null )
        return;

      updateThread.Dispose();
      updateThread = null;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public Updateservice()
    {
      updateThread = new BackgroundWorker
      {
        WorkerSupportsCancellation = true
      };
      updateThread.DoWork += UpdateThread_DoWork;
      updateThread.RunWorkerCompleted += UpdateThread_Completed;
    }

    /// <summary>
    /// Initialize web service
    /// </summary>
    public void InitWebService()
    {
      WebServiceData data = new WebServiceData
      {
        ProxyAddress = Proxy,
        ProxyPort = ProxyPort,
        ProxyCredential = ProxyAuthentification,
        Url = UpdateUrl,
        UseProxy = UseProxy,
        UseProxySystemSettings = UseSystemSettings
      };

      webservice = new Webservice(data);
    }

    /// <summary>
    /// Start update thread
    /// </summary>
    public void StartUpdate()
    {
      if ( !updateThread.IsBusy )
        updateThread.RunWorkerAsync();

      IsThreadCompleted = false;
    }

    #region Thread

    private void UpdateThread_DoWork(object sender, DoWorkEventArgs e)
    {
      string html;

      if ( !webservice.UpdateWebRequest(out html) )
        return;

      webData = html;
      Success = true;
    }

    private void UpdateThread_Completed(object sender, RunWorkerCompletedEventArgs e)
    {
      if ( !string.IsNullOrEmpty(webData) )
      {
        UpdateController uc = new UpdateController();
        Match match = Regex.Match(UpdateUrl, @"https://github.com", RegexOptions.IgnoreCase);

        if ( match.Success )
        {
          string tag = UpdateUrl.Substring(match.Value.Length, UpdateUrl.Length - match.Value.Length);

          HaveToUpdate = uc.UpdateNecessary(webData, tag);
          WebVersion = uc.WebVersion;
          AppVersion = uc.AppVersion;
        }
      }

      IsThreadCompleted = true;
      ThreadCompletedEvent?.Invoke(this, EventArgs.Empty);
    }

    #endregion
  }
}
