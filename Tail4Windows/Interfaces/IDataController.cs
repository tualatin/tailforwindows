namespace Org.Vs.TailForWin.Interfaces
{
  /// <summary>
  /// Data controller interface
  /// </summary>
  public interface IDataController
  {
    /// <summary>
    /// HTTP GET method
    /// </summary>
    /// <param name="url">Request URL</param>
    /// <returns>A request stream otherwise null</returns>
    string HttpGet(string url);

    /// <summary>
    /// HTTP POST method
    /// </summary>
    /// <param name="url">Request URL</param>
    /// <param name="json">JSON to send</param>
    /// <returns>A request stream otherwise null</returns>
    string HttpPost(string url, string json);
  }
}
