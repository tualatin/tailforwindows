using System.Threading.Tasks;
using NUnit.Framework;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class TestWebController : MockAppTest
  {
    private IWebController _webController;


    [SetUp]
    protected void Setup()
    {
      _webController = new WebController();

    }

    [Test]
    public async Task TestGetStringByUrl()
    {
      var webRequest = await _webController.GetStringByUrlAsync(EnvironmentContainer.ApplicationUpdateWebUrl);
    }
  }
}
