using System.Windows;
using System.Reflection;
using TailForWin.Utils;
using TailForWin.Data;
using System;
using System.Windows.Input;
using System.Diagnostics;
using TailForWin.Controller;
using System.ComponentModel;


namespace TailForWin.Template.TabOptions
{
  /// <summary>
  /// Interaction logic for AboutItem.xaml
  /// </summary>
  public partial class AboutItem : ITabOptionItems
  {
    /// <summary>
    /// Close dialog event handler
    /// </summary>
    public event EventHandler CloseDialog;
    
    /// <summary>
    /// Save settubgs event handler
    /// </summary>
    public event EventHandler SaveSettings;

    private readonly BackgroundWorker uptimeThread;


    public AboutItem ()
    {
      InitializeComponent ( );

      PreviewKeyDown += HandleEsc;

      Assembly assembly = Assembly.GetExecutingAssembly ( );
      labelBuildDate.Content = (BuildDate.GetBuildDateTime (assembly)).ToString ("dd.MM.yyyy HH:mm:ss");
      labelAppName.Content = LogFile.APPLICATION_CAPTION;
      labelVersion.Content = assembly.GetName ( ).Version;
      updater.ApplicationName = LogFile.APPLICATION_CAPTION;
      updater.DataContext = SettingsHelper.TailSettings;
      checkBoxAutoUpdate.DataContext = SettingsHelper.TailSettings;
      lbCopyright.Content = string.Format ("{0} {1}", lbCopyright.Content, DateTime.Now.Year);

      uptimeThread = new BackgroundWorker { WorkerSupportsCancellation = true };
      uptimeThread.DoWork += DoWork_Thread;

      Unloaded += (o, e) => uptimeThread.CancelAsync ( );
    }

    public void btnSave_Click (object sender, RoutedEventArgs e)
    {
      if (CloseDialog != null)
        CloseDialog (this, EventArgs.Empty);

      uptimeThread.CancelAsync ( );
    }

    public void btnCancel_Click (object sender, RoutedEventArgs e)
    {
      if (SaveSettings != null)
        SaveSettings (this, EventArgs.Empty);

      throw new NotImplementedException ( );
    }

    public void HandleEsc (object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        btnSave_Click (sender, e);
    }

    private void Hyperlink_RequestNavigate (object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
      Process.Start (new ProcessStartInfo (e.Uri.AbsoluteUri));
      e.Handled = true;
    }

    private void UserControl_Loaded (object sender, RoutedEventArgs e)
    {
      if (!string.IsNullOrEmpty (SettingsHelper.TailSettings.ProxySettings.UserName) && !string.IsNullOrEmpty (SettingsHelper.TailSettings.ProxySettings.Password))
        updater.ProxyAuthentification = new System.Net.NetworkCredential (SettingsHelper.TailSettings.ProxySettings.UserName, StringEncryption.Decrypt (SettingsHelper.TailSettings.ProxySettings.Password, LogFile.ENCRYPT_PASSPHRASE));

      if (!uptimeThread.IsBusy)
        uptimeThread.RunWorkerAsync ( );
    }

    private void btnSysInfo_Click (object sender, RoutedEventArgs e)
    {
      Window wnd = Window.GetWindow (this);
      SystemInformation sysInfo = new SystemInformation { Owner = wnd };
      sysInfo.ShowDialog ( );
    }

    #region Thread

    private void DoWork_Thread (object sender, DoWorkEventArgs e)
    {
      while (uptimeThread != null && !uptimeThread.CancellationPending)
      {
        TimeSpan updTime = DateTime.Now.Subtract (LogFile.APP_MAIN_WINDOW.TfWUpTimeStart);

        labelUptime.Dispatcher.Invoke (new Action (( ) => 
        {
          labelUptime.Content = string.Format ("{0} Day(s), {1:00}:{2:00}:{3:00} Hour(s)", updTime.Days, updTime.Hours, updTime.Minutes, updTime.Seconds);
        }));

        System.Threading.Thread.Sleep (1000);
      }
    }

    #endregion
  }
}
