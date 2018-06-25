using System;
using System.Threading.Tasks;
using System.Windows;
using NUnit.Framework;
using Org.Vs.TailForWin.Business.Services;
using Org.Vs.TailForWin.Business.Services.Interfaces;
using Org.Vs.TailForWin.Core.Controllers;

namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class TestWindowsEventReader
  {
    private IWindowEventReader _windowEventReader;

    [SetUp]
    protected void SetUp()
    {
      _windowEventReader = new WindowsEventReader();

      if ( Application.Current == null )
        Application.LoadComponent(new Uri("/T4W;component/app.xaml", UriKind.Relative));
    }

    [Test]
    public async Task TestGetWindowsCategoriesAsync()
    {
      var result = await _windowEventReader.GetCategoriesAsync().ConfigureAwait(false);

      Assert.IsNotNull(result);
      Assert.AreNotEqual(0, result.Count);
    }

    [Test]
    public void TestReadWindowsEvents()
    {
      SettingsHelperController.CurrentSettings.LinesRead = 20;
      var lastResult = _windowEventReader.StartReadWindowsEvents("System");

      Assert.NotNull(lastResult);
      Assert.AreNotEqual(0, lastResult.Count);
      Assert.AreEqual(SettingsHelperController.CurrentSettings.LinesRead, lastResult.Count);

      _windowEventReader.StopReadWindowsEvents();
    }
  }
}
