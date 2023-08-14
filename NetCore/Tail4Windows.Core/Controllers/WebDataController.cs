using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
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

      HttpClientHandler handler = SettingsHelperController.CurrentSettings.ProxySettings.UseSystemSettings != null ?
        await CreateHttpClientHandlerAsync().ConfigureAwait(false) : new HttpClientHandler();

      using ( var client = new HttpClient(handler) )
      {
        using ( var cts = new CancellationTokenSource(TimeSpan.FromMinutes(3)) )
        {
          var downloadTask = client.GetStringAsync(url, cts.Token);
          Task timeoutTask = Task.Delay(TimeSpan.FromMinutes(2), cts.Token);
          Task completedTask = await Task.WhenAny(downloadTask, timeoutTask).ConfigureAwait(false);

          return completedTask == timeoutTask ? null : await downloadTask.ConfigureAwait(false);
        }
      }
    }

    private async Task<HttpClientHandler> CreateHttpClientHandlerAsync()
    {
      string proxyUrl = $"{SettingsHelperController.CurrentSettings.ProxySettings.ProxyUrl}:{SettingsHelperController.CurrentSettings.ProxySettings.ProxyPort}";
      string pw = await StringEncryption.DecryptAsync(SettingsHelperController.CurrentSettings.ProxySettings.Password).ConfigureAwait(false);
      bool useDefaultCredentials = true;
      NetworkCredential proxyCredential = null;

      if ( !string.IsNullOrWhiteSpace(SettingsHelperController.CurrentSettings.ProxySettings.UserName) && !string.IsNullOrWhiteSpace(pw) )
      {
        useDefaultCredentials = false;
        proxyCredential = new NetworkCredential(SettingsHelperController.CurrentSettings.ProxySettings.UserName, pw);
      }

      IWebProxy proxy;

      if ( SettingsHelperController.CurrentSettings.ProxySettings.UseSystemSettings != null && SettingsHelperController.CurrentSettings.ProxySettings.UseSystemSettings.Value )
      {
        proxy = WebRequest.GetSystemWebProxy();
        proxy.Credentials = useDefaultCredentials ? null : proxyCredential;
      }
      else
      {
        proxy = new WebProxy(proxyUrl, false)
        {
          UseDefaultCredentials = useDefaultCredentials,
          Credentials = useDefaultCredentials ? null : proxyCredential
        };
      }

      var httpClientHandler = new HttpClientHandler
      {
        Proxy = proxy,
        PreAuthenticate = true,
        UseDefaultCredentials = useDefaultCredentials
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
