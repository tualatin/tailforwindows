using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using log4net;
using Org.Vs.TailForWin.Interfaces;
using Org.Vs.TailForWin.Utils;


namespace Org.Vs.TailForWin.Controller.WebService
{
  /// <summary>
  /// Tail4Windows web service
  /// </summary>
  public class DataController : IDataController
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(DataController));

    private static DataController instance;


    private DataController()
    {
    }

    /// <summary>
    /// Get instance of WebService
    /// </summary>
    /// <returns>Current instance of WebService</returns>
    public static DataController Instance()
    {
      return instance ?? (instance = new DataController());
    }

    /// <summary>
    /// HTTP GET method
    /// </summary>
    /// <param name="url">Request URL</param>
    /// <returns>A request stream otherwise null</returns>
    public string HttpGet(string url)
    {
      CheckUrl(url);

      try
      {
        var request = Tail4WndWebRequest(url, "GET");

        if ( request == null )
          return string.Empty;

        string result;

        using ( var response = request.GetResponse() as HttpWebResponse )
        {
          if ( response == null )
            return string.Empty;

          using ( var sr = new StreamReader(response.GetResponseStream()) )
          {
            result = sr.ReadToEnd();
          }
        }
        return result;
      }
      catch ( ArgumentNullException ex )
      {
        UserErrorException.HandleUserException(ex);
      }
      catch ( NotSupportedException ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
      catch ( UriFormatException ex )
      {
        UserErrorException.HandleUserException(ex);
      }
      catch ( WebException ex )
      {
        UserErrorException.HandleUserException(ex);
      }
      catch ( InvalidOperationException ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
      return string.Empty;
    }

    /// <summary>
    /// HTTP POST method
    /// </summary>
    /// <param name="url">Request URL</param>
    /// <param name="json">JSON to send</param>
    /// <returns>A request stream otherwise null</returns>
    public string HttpPost(string url, string json)
    {
      CheckUrl(url);

      return string.Empty;
    }

    private static void CheckUrl(string url)
    {
      if ( string.IsNullOrWhiteSpace(url) )
        throw new ArgumentException();

      Regex regex = new Regex(@"^https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)");

      if ( !regex.IsMatch(url) )
        throw new NotSupportedException();
    }

    private HttpWebRequest Tail4WndWebRequest(string url, string method)
    {
      HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

      if ( request == null )
        return null;

      request.UserAgent = CentralManager.APPLICATION_CAPTION;
      request.Method = method;
      request.Date = DateTime.Now;
      request.Timeout = 180000;
      request.Headers["Accept-Charset"] = "utf-8";
      request.Headers["Cache-Control"] = "no-cache, no-store";
      request.Headers["Pragma"] = "no-cache";

      if ( SettingsHelper.TailSettings.ProxySettings.UseProxy )
      {
        if ( !SettingsHelper.TailSettings.ProxySettings.UseSystemSettings )
          request.Proxy = new WebProxy($"{SettingsHelper.TailSettings.ProxySettings.ProxyUrl}:{SettingsHelper.TailSettings.ProxySettings.ProxyPort}", true);

        if ( !string.IsNullOrEmpty(SettingsHelper.TailSettings.ProxySettings.UserName) && !string.IsNullOrEmpty(SettingsHelper.TailSettings.ProxySettings.Password) )
          request.Proxy.Credentials = new NetworkCredential(SettingsHelper.TailSettings.ProxySettings.UserName, SettingsHelper.TailSettings.ProxySettings.Password);
      }

      if ( !SettingsHelper.TailSettings.ProxySettings.UseSystemSettings )
        return request;

      WebRequest.DefaultWebProxy = WebRequest.GetSystemWebProxy();

      if ( !string.IsNullOrEmpty(SettingsHelper.TailSettings.ProxySettings.UserName) && !string.IsNullOrEmpty(SettingsHelper.TailSettings.ProxySettings.Password) )
        WebRequest.DefaultWebProxy.Credentials = new NetworkCredential(SettingsHelper.TailSettings.ProxySettings.UserName, SettingsHelper.TailSettings.ProxySettings.Password);

      return request;
    }
  }
}
