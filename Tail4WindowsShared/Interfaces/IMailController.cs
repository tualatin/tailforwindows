namespace Org.Vs.Tail4Win.Shared.Interfaces
{
  /// <summary>
  /// E-Mail controller interface
  /// </summary>
  public interface IMailController
  {
    /// <summary>
    /// Send E-Mail async
    /// </summary>
    /// <param name="message">Message</param>
    /// <returns></returns>
    Task SendLogMailAsync(string message);
  }
}
