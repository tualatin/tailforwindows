using System.Windows;
using System.ComponentModel;
using System;


namespace Org.Vs.TailForWin.Template.UpdateController
{
  /// <summary>
  /// Interaction logic for Updater.xaml
  /// </summary>
  public partial class Updater : IDisposable
  {
    private Updateservice updater;


    public void Dispose()
    {
      if (updater == null)
        return;

      updater.Dispose();
      updater = null;
    }

    public Updater()
    {
      InitializeComponent();
    }

    #region Public Properties

    public static readonly DependencyProperty ApplicationNameProperty = DependencyProperty.Register("ApplicationName", typeof(string), typeof(Updater), new PropertyMetadata(string.Empty));

    [Category("Updater"), Description("Application name for message dialogue")]
    public string ApplicationName
    {
      get
      {
        return ((string)GetValue(ApplicationNameProperty));
      }
      set
      {
        SetValue(ApplicationNameProperty, value);
      }
    }

    public static readonly DependencyProperty UpdateURLProperty = DependencyProperty.Register("UpdateURL", typeof(string), typeof(Updater), new PropertyMetadata(string.Empty));

    [Category("Updater"), Description("Update Url")]
    public string UpdateURL
    {
      get
      {
        return ((string)GetValue(UpdateURLProperty));
      }
      set
      {
        SetValue(UpdateURLProperty, value);
      }
    }

    public static readonly DependencyProperty ProxyProperty = DependencyProperty.Register("Proxy", typeof(string), typeof(Updater), new PropertyMetadata(string.Empty));

    [Category("Updater"), Description("Proxy Server")]
    public string Proxy
    {
      get
      {
        return ((string)GetValue(ProxyProperty));
      }
      set
      {
        SetValue(ProxyProperty, value);
      }
    }

    public static readonly DependencyProperty ProxyPortProperty = DependencyProperty.Register("ProxyPort", typeof(int), typeof(Updater), new PropertyMetadata(-1));

    [Category("Updater"), Description("Proxy Port")]
    public int ProxyPort
    {
      get
      {
        return ((int)GetValue(ProxyPortProperty));
      }
      set
      {
        SetValue(ProxyPortProperty, value);
      }
    }

    public static readonly DependencyProperty ProxyAuthentificationProperty = DependencyProperty.Register("ProxyAuthentification", typeof(System.Net.NetworkCredential), typeof(Updater), new PropertyMetadata(new System.Net.NetworkCredential()));

    [Category("Updater"), Description("Proxy Username/Password")]
    public System.Net.NetworkCredential ProxyAuthentification
    {
      get
      {
        return ((System.Net.NetworkCredential)GetValue(ProxyAuthentificationProperty));
      }
      set
      {
        SetValue(ProxyAuthentificationProperty, value);
      }
    }

    public static readonly DependencyProperty UseProxyProperty = DependencyProperty.Register("UseProxy", typeof(bool), typeof(Updater), new PropertyMetadata(false));

    [Category("Updater"), Description("Use a proxy server")]
    public bool UseProxy
    {
      get
      {
        return ((bool)GetValue(UseProxyProperty));
      }
      set
      {
        SetValue(UseProxyProperty, value);
      }
    }

    public static readonly DependencyProperty UseSystemSettingsProperty = DependencyProperty.Register("UseSystemSettings", typeof(bool), typeof(Updater), new PropertyMetadata(false));

    [Category("Updater"), Description("Use system settings")]
    public bool UseSystemSettings
    {
      get
      {
        return ((bool)GetValue(UseSystemSettingsProperty));
      }
      set
      {
        SetValue(UseSystemSettingsProperty, value);
      }
    }

    #endregion

    private void btnUpdater_Click(object sender, RoutedEventArgs e)
    {
      updater.StartUpdate();

      btnUpdater.IsEnabled = updater.IsThreadCompleted;
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      if (updater != null)
        updater = null;

      updater = new Updateservice
      {
        UseProxy = UseProxy,
        UseSystemSettings = UseSystemSettings,
        Proxy = Proxy,
        ProxyPort = ProxyPort,
        UpdateURL = UpdateURL,
        ProxyAuthentification = ProxyAuthentification
      };

      updater.ThreadCompletedEvent += UpdateCompleted;
      updater.InitWebService();
    }

    private void UpdateCompleted(object sender, EventArgs e)
    {
      btnUpdater.IsEnabled = updater.IsThreadCompleted;

      if (!updater.Success)
        return;

      Window wnd = Window.GetWindow(this);
      ResultDialog rd = new ResultDialog(ApplicationName, updater.HaveToUpdate, UpdateURL)
      {
        Owner = wnd,
        WebVersion = updater.WebVersion,
        ApplicationVersion = updater.AppVersion
      };
      rd.ShowDialog();
    }
  }
}
