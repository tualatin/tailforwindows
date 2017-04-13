using System;
using System.Timers;
using System.Windows.Threading;
using log4net;
using Org.Vs.TailForWin.Controller;
using Org.Vs.TailForWin.Data;
using Org.Vs.TailForWin.Template.UpdateController;


namespace Org.Vs.TailForWin.Utils
{
  /// <summary>
  /// AutoUpdate
  /// </summary>
  public static class AutoUpdate
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(AutoUpdate));

    private static Timer timer;
    private static Updateservice updater;


    /// <summary>
    /// Initialize timer
    /// </summary>
    public static void Init()
    {
      timer = new Timer(5000)
      {
        Enabled = true
      };
      timer.Elapsed += TimerEvent;
    }

    private static void TimerEvent(object sender, ElapsedEventArgs e)
    {
      try
      {
        timer.Enabled = false;

        updater = new Updateservice
        {
          UseSystemSettings = SettingsHelper.TailSettings.ProxySettings.UseSystemSettings,
          UseProxy = SettingsHelper.TailSettings.ProxySettings.UseProxy,
          Proxy = SettingsHelper.TailSettings.ProxySettings.ProxyUrl,
          ProxyPort = SettingsHelper.TailSettings.ProxySettings.ProxyPort,
          UpdateURL = SettingsData.ApplicationWebUrl
        };

        if(!string.IsNullOrEmpty(SettingsHelper.TailSettings.ProxySettings.UserName) && !string.IsNullOrEmpty(SettingsHelper.TailSettings.ProxySettings.Password))
          updater.ProxyAuthentification = new System.Net.NetworkCredential(SettingsHelper.TailSettings.ProxySettings.UserName, StringEncryption.Decrypt(SettingsHelper.TailSettings.ProxySettings.Password, LogFile.ENCRYPT_PASSPHRASE));

        updater.InitWebService();
        updater.ThreadCompletedEvent += UpdateCompletedEvent;

        updater.StartUpdate();
      }
      catch(Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    private static void UpdateCompletedEvent(object sender, EventArgs e)
    {
      try
      {
        if(updater.HaveToUpdate)
        {
          System.Threading.Thread staThread = new System.Threading.Thread(() =>
          {
            Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
            {
              ResultDialog rd = new ResultDialog(LogFile.APPLICATION_CAPTION, updater.HaveToUpdate, updater.UpdateURL)
              {
                WebVersion = updater.WebVersion,
                ApplicationVersion = updater.AppVersion,
                Topmost = true,
                ShowInTaskbar = true
              };

              System.Windows.Window temp = new System.Windows.Window
              {
                Visibility = System.Windows.Visibility.Hidden,
                WindowState = System.Windows.WindowState.Minimized,
                ShowInTaskbar = false
              };

              temp.Show();
              rd.Owner = temp;

              rd.ShowDialog();
            }), DispatcherPriority.Normal);
          })
          {
            Name = $"{LogFile.APPLICATION_CAPTION}_AutoUpdateThread",
            IsBackground = true
          };

          staThread.SetApartmentState(System.Threading.ApartmentState.STA);
          staThread.Start();
          staThread.Join();
        }

        LOG.Info("AutoUpdate local version '{0}' - web version '{1}", updater.AppVersion, updater.WebVersion);
      }
      catch(Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }
  }
}
