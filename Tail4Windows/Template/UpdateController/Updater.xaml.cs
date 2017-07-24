using System;
using System.ComponentModel;
using System.Windows;
using Org.Vs.TailForWin.Controller.WebServices;


namespace Org.Vs.TailForWin.Template.UpdateController
{
  /// <summary>
  /// Interaction logic for Updater.xaml
  /// </summary>
  public partial class Updater : IDisposable
  {
    private Updateservice updater;


    /// <summary>
    /// Releases all resources used by the Updater.
    /// </summary>
    public void Dispose()
    {
      if ( updater == null )
        return;

      updater.Dispose();
      updater = null;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public Updater()
    {
      InitializeComponent();
    }

    #region Public Properties

    /// <summary>
    /// Update application name
    /// </summary>
    public static readonly DependencyProperty ApplicationNameProperty = DependencyProperty.Register("ApplicationName", typeof(string), typeof(Updater), new PropertyMetadata(string.Empty));

    /// <summary>
    /// Update application name
    /// </summary>
    [Category("Updater"), Description("Application name for message dialogue")]
    public string ApplicationName
    {
      get => (string) GetValue(ApplicationNameProperty);
      set => SetValue(ApplicationNameProperty, value);
    }

    /// <summary>
    /// Update URL
    /// </summary>
    public static readonly DependencyProperty UpdateUrlProperty = DependencyProperty.Register("UpdateUrl", typeof(string), typeof(Updater), new PropertyMetadata(string.Empty));

    /// <summary>
    /// Update URL
    /// </summary>
    [Category("Updater"), Description("Update Url")]
    public string UpdateUrl
    {
      get => (string) GetValue(UpdateUrlProperty);
      set => SetValue(UpdateUrlProperty, value);
    }

    /// <summary>
    /// Proxy settings
    /// </summary>
    public static readonly DependencyProperty ProxyProperty = DependencyProperty.Register("Proxy", typeof(string), typeof(Updater), new PropertyMetadata(string.Empty));

    /// <summary>
    /// Proxy settings
    /// </summary>
    [Category("Updater"), Description("Proxy Server")]
    public string Proxy
    {
      get => (string) GetValue(ProxyProperty);
      set => SetValue(ProxyProperty, value);
    }

    /// <summary>
    /// Proxy port settings
    /// </summary>
    public static readonly DependencyProperty ProxyPortProperty = DependencyProperty.Register("ProxyPort", typeof(int), typeof(Updater), new PropertyMetadata(-1));

    /// <summary>
    /// Proxy port settings
    /// </summary>
    [Category("Updater"), Description("Proxy Port")]
    public int ProxyPort
    {
      get => (int) GetValue(ProxyPortProperty);
      set => SetValue(ProxyPortProperty, value);
    }

    /// <summary>
    /// Proxy authentification
    /// </summary>
    public static readonly DependencyProperty ProxyAuthentificationProperty = DependencyProperty.Register("ProxyAuthentification", typeof(System.Net.NetworkCredential), typeof(Updater), new PropertyMetadata(new System.Net.NetworkCredential()));

    /// <summary>
    /// Proxy authentification
    /// </summary>
    [Category("Updater"), Description("Proxy Username/Password")]
    public System.Net.NetworkCredential ProxyAuthentification
    {
      get => (System.Net.NetworkCredential) GetValue(ProxyAuthentificationProperty);
      set => SetValue(ProxyAuthentificationProperty, value);
    }

    /// <summary>
    /// Use a Proxy
    /// </summary>
    public static readonly DependencyProperty UseProxyProperty = DependencyProperty.Register("UseProxy", typeof(bool), typeof(Updater), new PropertyMetadata(false));

    /// <summary>
    /// Use Proxy
    /// </summary>
    [Category("Updater"), Description("Use a proxy server")]
    public bool UseProxy
    {
      get => (bool) GetValue(UseProxyProperty);
      set => SetValue(UseProxyProperty, value);
    }

    /// <summary>
    /// Use system settings for Proxy
    /// </summary>
    public static readonly DependencyProperty UseSystemSettingsProperty = DependencyProperty.Register("UseSystemSettings", typeof(bool), typeof(Updater), new PropertyMetadata(false));

    /// <summary>
    /// Use system settings for Proxy
    /// </summary>
    [Category("Updater"), Description("Use system settings")]
    public bool UseSystemSettings
    {
      get => (bool) GetValue(UseSystemSettingsProperty);
      set => SetValue(UseSystemSettingsProperty, value);
    }

    #endregion

    private void btnUpdater_Click(object sender, RoutedEventArgs e)
    {
      updater.StartUpdate();

      btnUpdater.IsEnabled = updater.IsThreadCompleted;
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      if ( updater == null )
        updater = null;

      updater = new Updateservice(WebService.Instance())
      {
        UpdateUrl = UpdateUrl,
      };

      updater.ThreadCompletedEvent += UpdateCompleted;
    }

    private void UpdateCompleted(object sender, EventArgs e)
    {
      btnUpdater.IsEnabled = updater.IsThreadCompleted;

      if ( !updater.Success )
        return;

      Window wnd = Window.GetWindow(this);
      ResultDialog rd = new ResultDialog(ApplicationName, updater.HaveToUpdate, UpdateUrl)
      {
        Owner = wnd,
        WebVersion = updater.WebVersion,
        ApplicationVersion = updater.AppVersion
      };
      rd.ShowDialog();
    }
  }
}
