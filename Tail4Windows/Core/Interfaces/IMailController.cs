using System.Threading.Tasks;


namespace Org.Vs.TailForWin.Core.Interfaces
{
  /// <summary>
  /// E-Mail controller interface
  /// </summary>
  public interface IMailController
  {
    /// <summary>
    /// Send E-Mail async
    /// </summary>
    /// <param name="userToken">UserToken</param>
    /// <param name="message">Message</param>
    /// <returns></returns>
    Task SendMailAsync(object userToken, string message);
  }
}
