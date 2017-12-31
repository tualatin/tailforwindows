﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using log4net;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Core.Controllers
{
  /// <summary>
  /// T4W web controller
  /// </summary>
  public class WebController : IWebController
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(WebController));

    /// <summary>
    /// Get a string by URL
    /// </summary>
    /// <param name="url">URL to get</param>
    /// <returns>String given by Webrequest</returns>
    /// <exception cref="ArgumentException">If <c>url</c> is null or empty</exception>
    public async Task<string> GetStringByUrlAsync(string url)
    {
      Arg.NotNull(url, nameof(url));

      try
      {
        using ( var client = new HttpClient() )
        {
          var downloadTask = client.GetStringAsync(url);
          var timeoutTask = Task.Delay(TimeSpan.FromMinutes(3));
          var completedTask = await Task.WhenAny(downloadTask, timeoutTask);

          if ( completedTask == timeoutTask )
            return null;

          return await downloadTask;
        }
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        EnvironmentContainer.ShowErrorMessageBox(ex.Message);
      }
      return null;
    }
  }
}
