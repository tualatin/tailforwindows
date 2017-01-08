using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;


namespace Org.Vs.TailForWin.Template.UpdateController
{
  /// <summary>
  /// Interaction logic for ResultDialog.xaml
  /// </summary>
  public partial class ResultDialog
  {
    private readonly bool doUpdate;
    private string updateUrl;


    public ResultDialog(string applicationName, bool update, string updUrl)
    {
      InitializeComponent();
      Title = string.Format("{0} Update", applicationName);

      doUpdate = update;
      updateUrl = updUrl;
      PreviewKeyDown += HandleEsc;
    }

    #region PublicProperties

    /// <summary>
    /// Latest webversion
    /// </summary>
    public Version WebVersion
    {
      get;
      set;
    }

    /// <summary>
    /// Application version
    /// </summary>
    public Version ApplicationVersion
    {
      get;
      set;
    }

    #endregion

    #region Events

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      if (doUpdate)
        btnOk.IsEnabled = true;
      else
        btnOk.IsEnabled = false;

      labelLocalVersion.Content = ApplicationVersion;
      labelWebVersion.Content = WebVersion;

      if (doUpdate)
        labelUpdate.Content = Application.Current.FindResource("DoUpdate");
      else
        labelUpdate.Content = Application.Current.FindResource("NoUpdate");
    }

    private void HandleEsc(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        OnExit();
    }

    #endregion

    #region HelperFunctions

    private void OnExit()
    {
      Close();
    }

    //private static IntPtr FindWindow (string title)
    //{
    //  Process[] tempProcesses = Process.GetProcesses ( );

    //  foreach (Process proc in tempProcesses)
    //  {
    //    if (proc.MainWindowTitle == title)
    //    {
    //      return (proc.MainWindowHandle);
    //    }
    //  }
    //  return (IntPtr.Zero);
    //}

    #endregion

    private void btnOK_Click(object sender, RoutedEventArgs e)
    {
      updateUrl = string.Format("{0}/tag/{1}", updateUrl, WebVersion);
      Process.Start(updateUrl);
      OnExit();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      OnExit();
    }
  }
}
