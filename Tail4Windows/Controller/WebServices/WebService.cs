using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using log4net;
using Org.Vs.TailForWin.Interfaces;
using Org.Vs.TailForWin.Utils;


namespace Org.Vs.TailForWin.Controller.WebServices
{
  /// <summary>
  /// Tail4Windows web service
  /// </summary>
  public class WebService : IWebService
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(WebService));

    #region Constants

    private const string DefaultRequestAccept = "application/json";
    private const string DefaultApplicationJson = "application/json;charset=utf-8";

    #endregion

    private static WebService instance;


    private WebService()
    {
    }

    /// <summary>
    /// Get instance of WebService
    /// </summary>
    /// <returns>Current instance of WebService</returns>
    public static WebService Instance()
    {
      return instance ?? (instance = new WebService());
    }

    /// <summary>
    /// HTTP GET method
    /// </summary>
    /// <param name="url">Request URL</param>
    /// <returns>A request stream otherwise null</returns>
    public string HttpGet(string url)
    {
      LOG.Trace("{0} URL '{1}'", System.Reflection.MethodBase.GetCurrentMethod().Name, url);

      CheckUrl(url);

      try
      {
        var request = Tail4WndWebRequest(url, "GET");

        if ( request == null )
          return string.Empty;

        using ( var response = request.GetResponse() as HttpWebResponse )
        {
          if ( response != null )
          {
            HttpStatusCode statusCode = response.StatusCode;

            switch ( statusCode )
            {
            case HttpStatusCode.OK:

              return ReadContent(response);

            default:

              return string.Empty;
            }
          }
        }
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
      LOG.Trace("{0} URL '{1}'", System.Reflection.MethodBase.GetCurrentMethod().Name, url);

      CheckUrl(url);

      try
      {
        var request = Tail4WndWebRequest(url, "POST");

        if ( request == null )
          return string.Empty;

        request.ContentType = DefaultApplicationJson;
        request.Accept = DefaultRequestAccept;

        if ( !string.IsNullOrWhiteSpace(json) )
        {
          byte[] data = Encoding.UTF8.GetBytes(json);
          request.ContentLength = data.Length;

          using ( var post = request.GetRequestStream() )
          {
            post.Write(data, 0, data.Length);
          }
        }

        using ( var response = request.GetResponse() as HttpWebResponse )
        {
          if ( response != null )
          {
            HttpStatusCode statusCode = response.StatusCode;

            switch ( statusCode )
            {
            case HttpStatusCode.OK:

              return ReadContent(response);

            default:

              return string.Empty;
            }
          }
        }
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

    private static void CheckUrl(string url)
    {
      if ( url == null )
        throw new ArgumentNullException(nameof(url));

      if ( string.IsNullOrWhiteSpace(url) )
        throw new ArgumentException(nameof(url));

      Regex regex = new Regex(@"^https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)");

      if ( !regex.IsMatch(url) )
        throw new NotSupportedException(nameof(url));
    }

    private static string ReadContent(HttpWebResponse response)
    {
      Encoding encode = string.IsNullOrEmpty(response.ContentEncoding) ? Encoding.UTF8 : Encoding.GetEncoding(response.ContentEncoding);

      using ( var sr = response.GetResponseStream() )
      {
        if ( sr == null )
          return string.Empty;

        using ( var reader = new StreamReader(sr, encode) )
        {
          var result = reader.ReadToEnd();

          if ( !reader.EndOfStream )
            LOG.Warn("ReadToEnd completed, but EndOfStream was not reached");

          return result;
        }
      }
    }

    private static HttpWebRequest Tail4WndWebRequest(string url, string requestType)
    {
      HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

      if ( request == null )
        return null;

      request.UserAgent = CentralManager.APPLICATION_CAPTION;
      request.Method = requestType;
      request.Date = DateTime.Now;
      request.Timeout = 180000;
      request.Headers["Accept-Charset"] = "utf-8";
      request.Headers["Cache-Control"] = "no-cache, no-store";
      request.Headers["Pragma"] = "no-cache";
      request.AllowWriteStreamBuffering = true;
      request.AllowAutoRedirect = true;
      request.ContentLength = 0;

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
