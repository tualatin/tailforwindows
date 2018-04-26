using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Core.Controllers
{
  /// <summary>
  /// T4W web controller
  /// </summary>
  public sealed class WebDataController : IWebController
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(WebDataController));

    /// <summary>
    /// Get a string by URL
    /// </summary>
    /// <param name="url">URL to get</param>
    /// <returns>String given by Webrequest</returns>
    /// <exception cref="ArgumentException">If <c>url</c> is null or empty</exception>
    public async Task<string> GetStringByUrlAsync(string url)
    {
      Arg.NotNull(url, nameof(url));
      CheckUrl(url);

      HttpClientHandler handler = null;

      if ( SettingsHelperController.CurrentSettings.ProxySettings.UseSystemSettings != null )
        handler = await CreateHttpClientHandlerAsync().ConfigureAwait(false);
      else
        handler = new HttpClientHandler();

      using ( var client = new HttpClient(handler) )
      {
        try
        {
          using ( var cts = new CancellationTokenSource(TimeSpan.FromMinutes(3)) )
          {
            var downloadTask = client.GetStringAsync(url);
            var timeoutTask = Task.Delay(TimeSpan.FromMinutes(2), cts.Token);
            var completedTask = await Task.WhenAny(downloadTask, timeoutTask).ConfigureAwait(false);

            if ( completedTask == timeoutTask )
              return null;

            return await downloadTask.ConfigureAwait(false);
          }
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
          InteractionService.ShowErrorMessageBox(ex.Message);
        }
      }
      return null;
    }

    private async Task<HttpClientHandler> CreateHttpClientHandlerAsync()
    {
      string proxyUrl = $"{SettingsHelperController.CurrentSettings.ProxySettings.ProxyUrl}:{SettingsHelperController.CurrentSettings.ProxySettings.ProxyPort}";
      string pw = await StringEncryption.DecryptAsync(SettingsHelperController.CurrentSettings.ProxySettings.Password).ConfigureAwait(false);
      bool usedefaultCredentials = true;
      NetworkCredential proxyCredential = null;

      if ( !string.IsNullOrWhiteSpace(SettingsHelperController.CurrentSettings.ProxySettings.UserName) && !string.IsNullOrWhiteSpace(pw) )
      {
        usedefaultCredentials = false;
        proxyCredential = new NetworkCredential(SettingsHelperController.CurrentSettings.ProxySettings.UserName, pw);
      }

      IWebProxy proxy;

      if ( SettingsHelperController.CurrentSettings.ProxySettings.UseSystemSettings != null && SettingsHelperController.CurrentSettings.ProxySettings.UseSystemSettings.Value )
      {
        proxy = WebRequest.GetSystemWebProxy();
        proxy.Credentials = usedefaultCredentials ? null : proxyCredential;
      }
      else
      {
        proxy = new WebProxy(proxyUrl, false)
        {
          UseDefaultCredentials = usedefaultCredentials,
          Credentials = usedefaultCredentials ? null : proxyCredential
        };
      }

      var httpClientHandler = new HttpClientHandler
      {
        Proxy = proxy,
        PreAuthenticate = true,
        UseDefaultCredentials = usedefaultCredentials
      };
      return httpClientHandler;
    }

    private static void CheckUrl(string url)
    {
      Arg.NotNull(url, nameof(url));

      Regex regex = new Regex(@"^https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)");

      if ( !regex.IsMatch(url) )
        throw new NotSupportedException(nameof(url));
    }
  }
}
