using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Org.Vs.TailForWin.Interfaces;


namespace Org.Vs.TailForWin.Template.UpdateController
{
  /// <summary>
  /// Update service
  /// </summary>
  public class Updateservice : IDisposable
  {
    private readonly IWebService webService;
    private BackgroundWorker updateThread;
    private string webData = string.Empty;


    /// <summary>
    /// Event, when update process finished
    /// </summary>
    public event EventHandler ThreadCompletedEvent;


    #region Public properties

    /// <summary>
    /// URL with updater webaddress
    /// </summary>
    public string UpdateUrl
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
      private set;
    }

    /// <summary>
    /// Is update necessary
    /// </summary>
    public bool HaveToUpdate
    {
      get;
      private set;
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
      private set;
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
    /// Constructor
    /// </summary>
    /// <param name="webController">Webservice interface</param>
    public Updateservice(IWebService webController)
    {
      webService = webController;

      InitUpdateService();
    }

    private void InitUpdateService()
    {
      updateThread = new BackgroundWorker
      {
        WorkerSupportsCancellation = true
      };
      updateThread.DoWork += UpdateThread_DoWork;
      updateThread.RunWorkerCompleted += UpdateThread_Completed;
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
      if ( webService == null )
        return;

      webData = webService.HttpGet(UpdateUrl);
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
