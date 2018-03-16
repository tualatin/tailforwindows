using System.Threading.Tasks;
using NUnit.Framework;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Interfaces;

namespace Org.Vs.NUnit.Tests
{
  public class TestMailController
  {
    [Test]
    public async Task TestSendMailAsync()
    {
      IMailController mailController = new MailController();
      await mailController.SendLogMailAsync("Hello world").ConfigureAwait(false);
    }
  }
}
