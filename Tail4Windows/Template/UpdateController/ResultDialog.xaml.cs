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


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="applicationName">Application name</param>
    /// <param name="update">Update <code>true</code> or <code>false</code></param>
    /// <param name="updUrl">Update URL</param>
    public ResultDialog(string applicationName, bool update, string updUrl)
    {
      InitializeComponent();
      Title = $"{applicationName} Update";

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
      BtnOk.IsEnabled = doUpdate;
      LabelLocalVersion.Content = ApplicationVersion;
      LabelWebVersion.Content = WebVersion;

      LabelUpdate.Content = Application.Current.FindResource(doUpdate ? "DoUpdate" : "NoUpdate");
    }

    private void HandleEsc(object sender, KeyEventArgs e)
    {
      if(e.Key == Key.Escape)
        OnExit();
    }

    #endregion

    #region HelperFunctions

    private void OnExit()
    {
      Close();
    }

    #endregion

    private void btnOK_Click(object sender, RoutedEventArgs e)
    {
      updateUrl = $"{updateUrl}/tag/{WebVersion}";
      Process.Start(updateUrl);
      OnExit();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      OnExit();
    }
  }
}
