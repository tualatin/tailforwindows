using System;
using NUnit.Framework;
using Org.Vs.TailForWin.Controller.WebServices;
using Org.Vs.TailForWin.Interfaces;
using Org.Vs.TailForWin.Utils;


namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class WebServiceTest
  {
    private IWebService webService;

    [SetUp]
    protected void SetUp()
    {
      webService = WebService.Instance();
    }

    [Test]
    public void Test_WebService_Http_Get()
    {
      const string url = "https://httpbin.org/user-agent";

      Assert.Throws<ArgumentNullException>(() => webService.HttpGet(null));
      Assert.Throws<ArgumentException>(() => webService.HttpGet(string.Empty));
      Assert.Throws<NotSupportedException>(() => webService.HttpGet("test url"));
      Assert.DoesNotThrow(() => webService.HttpGet(url));

      var result = webService.HttpGet(url);

      Assert.IsNotEmpty(result);
      Assert.That(result.Contains(CentralManager.APPLICATION_CAPTION));
    }

    [Test]
    public void Test_WebService_Http_Post()
    {
      const string url = "https://httpbin.org/post";

      Assert.Throws<ArgumentNullException>(() => webService.HttpPost(null, null));
      Assert.Throws<ArgumentException>(() => webService.HttpPost(string.Empty, null));
      Assert.Throws<NotSupportedException>(() => webService.HttpPost("test url", null));
      Assert.DoesNotThrow(() => webService.HttpPost(url, "test data"));

      var result = webService.HttpPost(url, "test data");

      Assert.IsNotEmpty(result);
      Assert.That(result.Contains("test data"));
    }
  }
}
