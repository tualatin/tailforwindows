using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using TailForWin.Template.UpdateController.Data;
using System.Text.RegularExpressions;


namespace TailForWin.Template.UpdateController
{
  /// <summary>
  /// Interaction logic for Updater.xaml
  /// </summary>
  public partial class Updater: UserControl
  {
    private Webservice webservice;
    private BackgroundWorker updateThread;
    private string webData = string.Empty;


    public Updater ()
    {
      InitializeComponent ( );

      updateThread = new BackgroundWorker ( ) { WorkerSupportsCancellation = true };
      updateThread.DoWork += updateThread_DoWork;
      updateThread.RunWorkerCompleted += updateThread_Completed;
    }

    #region Public Properties

    public static readonly DependencyProperty ApplicationNameProperty = DependencyProperty.Register ("ApplicationName", typeof (string), typeof (Updater), new PropertyMetadata (string.Empty));

    [Category ("Updater"), Description ("Application name for message dialogue")]
    public string ApplicationName
    {
      get
      {
        return ((string) GetValue (ApplicationNameProperty));
      }
      set
      {
        SetValue (ApplicationNameProperty, value);
      }
    }

    public static readonly DependencyProperty UpdateURLProperty = DependencyProperty.Register ("UpdateURL", typeof (string), typeof (Updater), new PropertyMetadata (string.Empty));

    [Category ("Updater"), Description ("Update Url")]
    public string UpdateURL
    {
      get
      {
        return ((string) GetValue (UpdateURLProperty));
      }
      set
      {
        SetValue (UpdateURLProperty, value);
      }
    }

    public static readonly DependencyProperty ProxyProperty = DependencyProperty.Register ("Proxy", typeof (string), typeof (Updater), new PropertyMetadata (string.Empty));

    [Category ("Updater"), Description ("Proxy Server")]
    public string Proxy
    {
      get
      {
        return ((string) GetValue (ProxyProperty));
      }
      set
      {
        SetValue (ProxyProperty, value);
      }
    }

    public static readonly DependencyProperty ProxyPortProperty = DependencyProperty.Register ("ProxyPort", typeof (int), typeof (Updater), new PropertyMetadata (-1));

    [Category ("Updater"), Description ("Proxy Port")]
    public int ProxyPort
    {
      get
      {
        return ((int) GetValue (ProxyPortProperty));
      }
      set
      {
        SetValue (ProxyPortProperty, value);
      }
    }

    public static readonly DependencyProperty ProxyAuthentificationProperty = DependencyProperty.Register ("ProxyAuthentification", typeof (System.Net.NetworkCredential), typeof (Updater), new PropertyMetadata (new System.Net.NetworkCredential ( )));

    [Category ("Updater"), Description ("Proxy Username/Password")]
    public System.Net.NetworkCredential ProxyAuthentification
    {
      get
      {
        return ((System.Net.NetworkCredential) GetValue (ProxyAuthentificationProperty));
      }
      set
      {
        SetValue (ProxyAuthentificationProperty, value);
      }
    }

    public static readonly DependencyProperty UseProxyProperty = DependencyProperty.Register ("UseProxy", typeof (bool), typeof (Updater), new PropertyMetadata (false));

    [Category ("Updater"), Description ("Use a proxy server")]
    public bool UseProxy
    {
      get
      {
        return ((bool) GetValue (UseProxyProperty));
      }
      set
      {
        SetValue (UseProxyProperty, value);
      }
    }

    public static readonly DependencyProperty UseSystemSettingsProperty = DependencyProperty.Register ("UseSystemSettings", typeof (bool), typeof (Updater), new PropertyMetadata (false));

    [Category ("Updater"), Description ("Use system settings")]
    public bool UseSystemSettings
    {
      get
      {
        return ((bool) GetValue (UseSystemSettingsProperty));
      }
      set
      {
        SetValue (UseSystemSettingsProperty, value);
      }
    }

    #endregion

    #region Thread

    private void updateThread_DoWork (object sender, DoWorkEventArgs e)
    {
      string html = string.Empty;

      if (webservice.UpdateWebRequest (out html))
        webData = html;
    }

    private void updateThread_Completed (object sender, RunWorkerCompletedEventArgs e)
    {
      if (!string.IsNullOrEmpty (webData)) 
      {
        UpdateController uc = new UpdateController ( );
        Match match = Regex.Match (UpdateURL, @"https://github.com", RegexOptions.IgnoreCase);

        if (match.Success)
        {
          string tag = UpdateURL.Substring (match.Value.Length, UpdateURL.Length - match.Value.Length);

          Window parentWindow = Window.GetWindow (this);
          ResultDialog rd = new ResultDialog (ApplicationName, uc.UpdateNecessary (webData, tag), UpdateURL)
          {
            Owner = parentWindow,
            WebVersion = uc.WebVersion,
            ApplicationVersion = uc.AppVersion
          };
          rd.ShowDialog ( );
        }
      }

      btnUpdater.IsEnabled = true;
    }

    #endregion

    private void btnUpdater_Click (object sender, RoutedEventArgs e)
    {
      if (!updateThread.IsBusy)
        updateThread.RunWorkerAsync ( );

      btnUpdater.IsEnabled = false;
    }

    private void UserControl_Loaded (object sender, RoutedEventArgs e)
    {
      WebServiceData data = new WebServiceData ( )
      {
        ProxyAddress = Proxy,
        ProxyPort = ProxyPort,
        ProxyCredential = ProxyAuthentification,
        Url = UpdateURL,
        UseProxy = UseProxy,
        UseProxySystemSettings = UseSystemSettings
      };

      webservice = new Webservice (data);
    }
  }
}
