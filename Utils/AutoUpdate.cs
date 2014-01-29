using System;
using System.Timers;
using TailForWin.Controller;
using TailForWin.Data;
using TailForWin.Template.UpdateController;


namespace TailForWin.Utils
{
  public static class AutoUpdate
  {
    private static Timer timer;
    private static Updateservice updater;


    /// <summary>
    /// Initialize timer
    /// </summary>
    public static void Init ( )
    {
      timer = new Timer (5000)
      {
        Enabled = true
      };

      timer.Elapsed += TimerEvent;
    }

    private static void TimerEvent (object sender, ElapsedEventArgs e)
    {
      timer.Enabled = false;

      updater = new Updateservice ( )
      {
        UseSystemSettings = SettingsHelper.TailSettings.ProxySettings.UseSystemSettings,
        UseProxy = SettingsHelper.TailSettings.ProxySettings.UseProxy,
        Proxy = SettingsHelper.TailSettings.ProxySettings.ProxyUrl,
        ProxyPort = SettingsHelper.TailSettings.ProxySettings.ProxyPort,
        UpdateURL = SettingsHelper.TailSettings.ApplicationWebUrl
      };

      if (!string.IsNullOrEmpty (SettingsHelper.TailSettings.ProxySettings.UserName) && !string.IsNullOrEmpty (SettingsHelper.TailSettings.ProxySettings.Password))
        updater.ProxyAuthentification = new System.Net.NetworkCredential (SettingsHelper.TailSettings.ProxySettings.UserName, StringEncryption.Decrypt (SettingsHelper.TailSettings.ProxySettings.Password, LogFile.ENCRYPT_PASSPHRASE));

      updater.InitWebService ( );
      updater.ThreadCompletedEvent += UpdateCompletedEvent;

      updater.StartUpdate ( );
    }

    private static void UpdateCompletedEvent (object sender, EventArgs e)
    {
      if (updater.HaveToUpdate)
      {
        System.Threading.Thread STAThread = new System.Threading.Thread (() =>
        {
          ResultDialog rd = new ResultDialog (LogFile.APPLICATION_CAPTION, updater.HaveToUpdate, updater.UpdateURL);      

          rd.Dispatcher.Invoke (new Action (() =>
          {
            rd.WebVersion = updater.WebVersion;
            rd.ApplicationVersion = updater.AppVersion;
            rd.Topmost = true;
            rd.ShowDialog ( );
          }), System.Windows.Threading.DispatcherPriority.Normal);
        }) 
        { 
          Name = string.Format ("{0}_AutoUpdateThread", LogFile.APPLICATION_CAPTION),
          IsBackground = true
        };

        STAThread.SetApartmentState (System.Threading.ApartmentState.STA);
        STAThread.Start ( );
        STAThread.Join ( );
      }

      ErrorLog.WriteLog (ErrorFlags.Info, "AutoUpdate", string.Format ("local version: {0}, web version: {1}", updater.AppVersion, updater.WebVersion));
    }
  }
}
