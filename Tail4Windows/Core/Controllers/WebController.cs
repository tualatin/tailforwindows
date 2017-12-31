using System.Threading.Tasks;
using Org.Vs.TailForWin.Core.Interfaces;


namespace Org.Vs.TailForWin.Core.Controllers
{
  /// <summary>
  /// T4W web controller
  /// </summary>
  public class WebController : IWebController
  {
    /// <summary>
    /// Get a string by URL
    /// </summary>
    /// <param name="url">URL to get</param>
    /// <returns>String given by Webrequest</returns>
    public Task<string> GetStringByUrlAsync(string url)
    {
      throw new System.NotImplementedException();
    }
  }
}
