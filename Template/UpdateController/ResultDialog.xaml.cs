using System.Windows;
using System.Windows.Input;


namespace TailForWin.Template.UpdateController
{
  /// <summary>
  /// Interaction logic for ResultDialog.xaml
  /// </summary>
  public partial class ResultDialog: Window
  {
    private bool doUpdate;
    private string updateUrl;


    public ResultDialog (string applicationName, bool update, string updateURL)
    {
      InitializeComponent ( );
      Title = string.Format ("{0} Update", applicationName);

      doUpdate = update;
      updateUrl = updateURL;
      PreviewKeyDown += HandleEsc;
    }

    #region PublicFunctions

    /// <summary>
    /// Latest webversion
    /// </summary>
    public System.Version WebVersion
    {
      get;
      set;
    }

    /// <summary>
    /// Application version
    /// </summary>
    public System.Version ApplicationVersion
    {
      get;
      set;
    }

    #endregion

    #region Events

    private void Window_Loaded (object sender, RoutedEventArgs e)
    {
      if (doUpdate)
        btnOk.IsEnabled = true;
      else
        btnOk.IsEnabled = false;

      labelLocalVersion.Content = ApplicationVersion;
      labelWebVersion.Content = WebVersion;

      if (doUpdate)
        labelUpdate.Content = Application.Current.FindResource ("DoUpdate");
      else
        labelUpdate.Content = Application.Current.FindResource ("NoUpdate");
     
    }

    private void HandleEsc (object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        OnExit ( );
    }

    #endregion
    
    #region HelperFunctions

    private void OnExit ()
    {
      Close ( );
    }
    
    #endregion

    private void btnOK_Click (object sender, RoutedEventArgs e)
    {
      updateUrl = string.Format ("{0}/tag/{1}", updateUrl, WebVersion);
      System.Diagnostics.Process.Start (updateUrl);
      OnExit ( );
    }

    private void btnCancel_Click (object sender, RoutedEventArgs e)
    {
      OnExit ( );
    }
  }
}
