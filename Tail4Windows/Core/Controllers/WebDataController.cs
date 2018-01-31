using System;
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

      using ( var client = new HttpClient() )
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
          EnvironmentContainer.ShowErrorMessageBox(ex.Message);

        }
      }
      return null;
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
