using System.Threading.Tasks;
using Org.Vs.TailForWin.Core.Interfaces;


namespace Org.Vs.TailForWin.Core.Controllers
{
  /// <summary>
  /// E-Mail controller
  /// </summary>
  public class MailController : IMailController
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public MailController() => InitializeComponents();

    private void InitializeComponents()
    {

    }

    /// <summary>
    /// Send E-Mail async
    /// </summary>
    /// <param name="userToken">UserToken</param>
    /// <param name="message">Message</param>
    /// <returns>Task</returns>
    public async Task SendMailAsync(object userToken, string message)
    {
      throw new System.NotImplementedException();
    }
  }
}
