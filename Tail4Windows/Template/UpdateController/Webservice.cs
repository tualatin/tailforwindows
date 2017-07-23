using System;
using System.IO;
using System.Net;
using Org.Vs.TailForWin.Controller.WebService;
using Org.Vs.TailForWin.Template.UpdateController.Data;


namespace Org.Vs.TailForWin.Template.UpdateController
{
  /// <summary>
  /// WebService
  /// </summary>
  public class Webservice
  {
    private readonly WebServiceData proxySettings;
    private HttpWebRequest request;


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="data">WebServiceData</param>
    public Webservice(WebServiceData data)
    {
      proxySettings = data;
    }

    /// <summary>
    /// Start update service
    /// </summary>
    /// <param name="html">Output html string</param>
    /// <returns>If success then true, otherwise false</returns>
    public bool UpdateWebRequest(out string html)
    {
      html = string.Empty;

      try
      {
        request = WebRequest.Create(proxySettings.Url) as HttpWebRequest;

        InitWebRequest(proxySettings.UseProxySystemSettings);

        using ( HttpWebResponse response = request.GetResponse() as HttpWebResponse )
        {
          using ( StreamReader sr = new StreamReader(response.GetResponseStream()) )
          {
            html = sr.ReadToEnd();
          }
        }
        return !string.IsNullOrWhiteSpace(html);
      }
      catch ( Exception ex )
      {
        UserErrorException.HandleUserException(ex);
      }
      return false;
    }

    #region HelperFunctions

    private void InitWebRequest(bool useSystemProxySettings)
    {
      request.UserAgent = "UpdateService";
      request.Headers["Accept-Charset"] = "utf-8";
      request.Headers["Cache-Control"] = "no-cache, no-store";
      request.Headers["Pragma"] = "no-cache";
      // 3 minutes
      request.Timeout = 180000;

      if ( proxySettings.UseProxy )
      {
        if ( !useSystemProxySettings )
          request.Proxy = new WebProxy($"{proxySettings.ProxyAddress}:{proxySettings.ProxyPort}", true);

        if ( !string.IsNullOrEmpty(proxySettings.ProxyCredential?.UserName) && !string.IsNullOrEmpty(proxySettings.ProxyCredential.Password) )
          request.Proxy.Credentials = proxySettings.ProxyCredential;
      }

      if ( !useSystemProxySettings )
        return;

      WebRequest.DefaultWebProxy = WebRequest.GetSystemWebProxy();

      if ( !string.IsNullOrEmpty(proxySettings.ProxyCredential?.UserName) && !string.IsNullOrEmpty(proxySettings.ProxyCredential.Password) )
        WebRequest.DefaultWebProxy.Credentials = proxySettings.ProxyCredential;
    }

    #endregion
  }
}
