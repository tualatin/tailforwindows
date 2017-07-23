using System;
using NUnit.Framework;
using Org.Vs.TailForWin.Controller.WebService;
using Org.Vs.TailForWin.Interfaces;


namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class WebServiceTest
  {
    private IDataController webService;

    [SetUp]
    protected void SetUp()
    {
      webService = DataController.Instance();
    }

    [Test]
    [Timeout(10000)]
    public void Test_WebService_Http_Get()
    {
      const string url = "https://www.github.com";

      Assert.Throws<ArgumentException>(() => webService.HttpGet(null));
      Assert.Throws<NotSupportedException>(() => webService.HttpGet("test url"));
      Assert.DoesNotThrow(() => webService.HttpGet(url));

      var result = webService.HttpGet(url);

      Assert.IsNotEmpty(result);
      Assert.That(result.Contains("https://github.com/"));
    }

    [Test]
    [Timeout(10000)]
    public void Test_WebService_Http_Post()
    {
      Assert.Throws<ArgumentException>(() => webService.HttpPost(null, null));
      Assert.Throws<NotSupportedException>(() => webService.HttpPost("test url", null));
    }
  }
}
