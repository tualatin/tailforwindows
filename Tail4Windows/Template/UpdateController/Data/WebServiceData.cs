namespace Org.Vs.TailForWin.Template.UpdateController.Data
{
  /// <summary>
  /// Webservice property class
  /// </summary>
  public class WebServiceData
  {
    /// <summary>
    /// Proxy server port
    /// </summary>
    public int ProxyPort
    {
      get;
      set;
    }

    /// <summary>
    /// Proxy server address
    /// </summary>
    public string ProxyAddress
    {
      get;
      set;
    }

    /// <summary>
    /// Proxy server credential
    /// </summary>
    public System.Net.NetworkCredential ProxyCredential
    {
      get;
      set;
    }

    /// <summary>
    /// Update web url
    /// </summary>
    public string Url
    {
      get;
      set;
    }

    /// <summary>
    /// Use proxy
    /// </summary>
    public bool UseProxy
    {
      get;
      set;
    }

    /// <summary>
    /// Use system settings for proxy settings
    /// </summary>
    public bool UseProxySystemSettings
    {
      get;
      set;
    }
  }
}
