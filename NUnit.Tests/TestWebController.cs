using System;
using System.Threading.Tasks;
using System.Windows;
using NUnit.Framework;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class TestWebController
  {
    private IWebController _webController;


    [SetUp]
    protected void Setup()
    {
      _webController = new WebController();

      if ( Application.Current == null )
        Application.LoadComponent(new Uri("/T4W;component/app.xaml", UriKind.Relative));
    }

    [Test]
    public async Task TestGetStringByUrl()
    {
      Assert.That(() => _webController.GetStringByUrlAsync(null), Throws.InstanceOf<ArgumentException>());

      var webRequest = await _webController.GetStringByUrlAsync(EnvironmentContainer.ApplicationUpdateWebUrl);
      Assert.IsNotNull(webRequest);
      Assert.IsTrue(webRequest.Contains("TfW_x64.zip"));
    }
  }
}
