namespace Org.Vs.TailForWin.Core.Interfaces
{
  /// <summary>
  /// Web controller interface
  /// </summary>
  public interface IWebController
  {
    /// <summary>
    /// Get a string by URL
    /// </summary>
    /// <param name="url">URL to get</param>
    /// <returns>String given by Webrequest</returns>
    Task<string> GetStringByUrlAsync(string url);
  }
}
