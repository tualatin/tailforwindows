using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using log4net;
using Org.Vs.TailForWin.Controller;
using Org.Vs.TailForWin.Utils.Events;


namespace Org.Vs.TailForWin.Utils
{
  /// <summary>
  /// Smart watch logic
  /// </summary>
  public class SmartWatch : IDisposable
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(SmartWatch));

    /// <summary>
    /// Fires, when files changed
    /// </summary>
    public event SmartWatchFilesChangedEventHandler SmartWatchFilesChanged;

    private BackgroundWorker smartWorker;
    private ManualResetEvent resetEvent;
    private string currentLogFolder;
    private string currentFileExtension;
    private string[] currentFiles;


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
    /// <param name="logFile">Full path of current log file</param>
    /// <exception cref="ArgumentException">If logFolder is null or empty</exception>
    public void StartSmartWatch(string logFile)
    {
      Arg.NotNull(logFile, "LogFolder");

      currentLogFolder = Path.GetDirectoryName(logFile);
      currentFileExtension = Path.GetExtension(logFile);
      currentFiles = GetFilesInCurrentLogDirectory();

      if(currentFiles == null)
        return;

      if(smartWorker.IsBusy)
      {
        ResumeSmartWatch();
        return;
      }

      LOG.Trace("Start SmartWatch");
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

      LOG.Trace("Suspend SmartWatch");
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

        var newValue = GetFilesInCurrentLogDirectory();

        if(newValue == null)
          continue;

        if(currentFiles.Length < newValue.Length)
        {
          LOG.Trace("SmartWatch Logfiles changed! Current '{0}' new '{1}'", currentFiles.Length, newValue.Length);
          GetLatestFile(newValue);

          currentFiles = newValue;
        }
        else if(currentFiles.Length > newValue.Length)
        {
          LOG.Trace("SmartWatch some logfile are deleted! Current '{0}' new '{1}'", currentFiles.Length, newValue.Length);
          currentFiles = newValue;
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

    private void GetLatestFile(string[] fileInput)
    {
      foreach(var item in fileInput)
      {
        try
        {
          if(!currentFiles.Contains(item))
            SmartWatchFilesChanged?.Invoke(this, item);
        }
        catch
        {
          continue;
        }
      }
    }

    private string[] GetFilesInCurrentLogDirectory()
    {
      try
      {
        string[] files;

        if(SettingsHelper.TailSettings.SmartWatchData.FilterByExtension)
          files = Directory.GetFiles(currentLogFolder, $"*{currentFileExtension}", SearchOption.TopDirectoryOnly);
        else
          files = Directory.GetFiles(currentLogFolder, "*.*", SearchOption.TopDirectoryOnly);

        return (files);
      }
      catch(Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
      return (null);
    }

    #endregion
  }
}
