using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Org.Vs.TailForWin.Controller;
using Org.Vs.TailForWin.Data;
using Org.Vs.TailForWin.Template.TabOptions.Interfaces;
using Org.Vs.TailForWin.Utils;


namespace Org.Vs.TailForWin.Template.TabOptions
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


    /// <summary>
    /// Standard constructor
    /// </summary>
    public AboutItem()
    {
      InitializeComponent();

      PreviewKeyDown += HandleEsc;

#if BUILD64
      string build = "64-Bit";
#elif BUILD32
      string build = "32-Bit";
#endif

#if DEBUG
      string channel = "debug";
#elif RELEASE
      string channel = "release";
#endif

      Assembly assembly = Assembly.GetExecutingAssembly();
      labelBuildDate.Content = BuildDate.GetBuildDateTime(assembly).ToString("dd.MM.yyyy HH:mm:ss");
      labelAppName.Content = CentralManager.APPLICATION_CAPTION;
      labelVersion.Content = $"{assembly.GetName().Version} - {build} ({channel})";
      updater.ApplicationName = CentralManager.APPLICATION_CAPTION;
      updater.DataContext = SettingsHelper.TailSettings;
      checkBoxAutoUpdate.DataContext = SettingsHelper.TailSettings;
      lbCopyright.Content = $"{lbCopyright.Content} {DateTime.Now.Year}";

      uptimeThread = new BackgroundWorker
      {
        WorkerSupportsCancellation = true
      };
      uptimeThread.DoWork += DoWork_Thread;

      Unloaded += (o, e) => uptimeThread.CancelAsync();
    }

    /// <summary>
    /// Handles the save click event
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    public void btnSave_Click(object sender, RoutedEventArgs e)
    {
      CloseDialog?.Invoke(this, EventArgs.Empty);
      uptimeThread.CancelAsync();
    }

    /// <summary>
    /// Handles the cancel click event
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    public void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      SaveSettings?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Handles the ESC button
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    public void HandleEsc(object sender, KeyEventArgs e)
    {
      if ( e.Key == Key.Escape )
        btnSave_Click(sender, e);
    }

    private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
      Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
      e.Handled = true;
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      if ( !string.IsNullOrEmpty(SettingsHelper.TailSettings.ProxySettings.UserName) && !string.IsNullOrEmpty(SettingsHelper.TailSettings.ProxySettings.Password) )
        updater.ProxyAuthentification = new System.Net.NetworkCredential(SettingsHelper.TailSettings.ProxySettings.UserName, StringEncryption.Decrypt(SettingsHelper.TailSettings.ProxySettings.Password, CentralManager.ENCRYPT_PASSPHRASE));

      if ( !uptimeThread.IsBusy )
        uptimeThread.RunWorkerAsync();
    }

    private void btnSysInfo_Click(object sender, RoutedEventArgs e)
    {
      Window wnd = Window.GetWindow(this);
      SystemInformation sysInfo = new SystemInformation
      {
        Owner = wnd
      };
      sysInfo.ShowDialog();
    }

    #region Thread

    private void DoWork_Thread(object sender, DoWorkEventArgs e)
    {
      while ( uptimeThread != null && !uptimeThread.CancellationPending )
      {
        TimeSpan updTime = DateTime.Now.Subtract(CentralManager.APP_MAIN_WINDOW.TfWUpTimeStart);

        labelUptime.Dispatcher.Invoke(new Action(() =>
        {
          labelUptime.Content = $"{updTime.Days} Day(s), {updTime.Hours:00}:{updTime.Minutes:00}:{updTime.Seconds:00} Hour(s)";
        }));

        System.Threading.Thread.Sleep(1000);
      }
    }

    #endregion
  }
}
