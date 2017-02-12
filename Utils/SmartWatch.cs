using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using log4net;


namespace Org.Vs.TailForWin.Utils
{
  /// <summary>
  /// Smart watch logic
  /// </summary>
  public class SmartWatch : IDisposable
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(SmartWatch));

    private BackgroundWorker smartWorker;
    private ManualResetEvent resetEvent;
    private string currentLogFolder;
    private int currentFiles;


    /// <summary>
    /// Standard constructor
    /// </summary>
    public SmartWatch()
    {
      resetEvent = new ManualResetEvent(false);
      smartWorker = new BackgroundWorker
      {
        WorkerSupportsCancellation = true
      };
      smartWorker.DoWork += SmartWorker_DoWork;
      smartWorker.RunWorkerCompleted += SmartWorker_RunWorkerCompleted;
    }

    /// <summary>
    /// Start smart watch
    /// </summary>
    /// <param name="logFolder">Full path of current log file</param>
    /// <exception cref="ArgumentException">If logFolder is null or empty</exception>
    public void StartSmartWatch(string logFolder)
    {
      Arg.NotNull(logFolder, "LogFolder");

      currentLogFolder = logFolder;
      currentFiles = GetCountOfFiles();

      if(smartWorker.IsBusy)
      {
        ResumeSmartWatch();
        return;
      }

      smartWorker.RunWorkerAsync();
      resetEvent.Set();
    }

    /// <summary>
    /// Respend smart watch
    /// </summary>
    public void SuspendSmartWatch()
    {
      if(smartWorker.IsBusy)
        resetEvent.Reset();
    }

    /// <summary>
    /// Resume smart watch
    /// </summary>
    public void ResumeSmartWatch()
    {
      if(smartWorker.IsBusy)
        resetEvent.Set();
    }

    /// <summary>
    /// Releases all resources used by the SmartWorker.
    /// </summary>
    public void Dispose()
    {
      if(smartWorker == null)
        return;

      if(smartWorker.IsBusy)
        smartWorker.CancelAsync();

      resetEvent.Reset();
      smartWorker.Dispose();
    }

    #region Thread

    private void SmartWorker_DoWork(object sender, DoWorkEventArgs e)
    {
      while(smartWorker != null && !smartWorker.CancellationPending)
      {
        resetEvent.WaitOne();
        Thread.Sleep(2000);

        if(currentFiles < GetCountOfFiles())
        {
          LOG.Trace("SmartWatch Logfiles changed! Current '{0}' new '{1}'", currentFiles, GetCountOfFiles());
          currentFiles = GetCountOfFiles();
        }
        else if(currentFiles > GetCountOfFiles())
        {
          LOG.Trace("SmartWatch some logfile are deleted! Current '{0}' new '{1}'", currentFiles, GetCountOfFiles());
          currentFiles = GetCountOfFiles();
        }        
      }

      e.Cancel = true;
    }

    private void SmartWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      resetEvent.Reset();
    }

    #endregion

    #region HelperFunctions

    private int GetCountOfFiles()
    {
      return (Directory.GetFiles(currentLogFolder).Length);
    }

    #endregion
  }
}
