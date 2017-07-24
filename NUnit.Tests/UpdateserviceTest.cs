using System;
using System.Threading;
using Moq;
using NUnit.Framework;
using Org.Vs.TailForWin.Data;
using Org.Vs.TailForWin.Interfaces;
using Org.Vs.TailForWin.Template.UpdateController;


namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class UpdateserviceTest
  {
    [Test]
    public void Test_UpdateService()
    {
      Mock<IWebService> mockedWebService = new Mock<IWebService>();
      Updateservice updateService = new Updateservice(mockedWebService.Object)
      {
        UpdateUrl = SettingsData.ApplicationWebUrl
      };
      updateService.ThreadCompletedEvent += (sender, e) =>
      {
        Assert.IsTrue(updateService.Success);
        Assert.IsTrue(updateService.IsThreadCompleted);
        Assert.IsFalse(updateService.HaveToUpdate);
        Assert.IsNotNull(updateService.AppVersion);
        Assert.IsNotNull(updateService.WebVersion);
        Assert.AreEqual(new Version(1, 5, 6312), updateService.WebVersion);
      };

      mockedWebService.Setup(p => p.HttpGet(It.IsAny<string>())).Returns("<a href=\"/tualatin/tailforwindows/releases/tag/1.5.6312\">v1.5.6312</a>");
      updateService.StartUpdate();

      while ( !updateService.IsThreadCompleted )
      {
        Thread.Sleep(500);
      }
    }
  }
}
