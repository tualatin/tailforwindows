using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Org.Vs.TailForWin.Business.Services;
using Org.Vs.TailForWin.Business.Services.Interfaces;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;

namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class TestWindowsEventReader
  {
    private ILogReadService _windowEventReader;

    [SetUp]
    protected void SetUp()
    {
      _windowEventReader = new WindowsEventReadService();
    }

    [Test]
    public async Task TestGetWindowsCategoriesAsync()
    {
      var result = await _windowEventReader.GetCategoriesAsync(new CancellationTokenSource(TimeSpan.FromMinutes(2)).Token).ConfigureAwait(false);

      Assert.IsNotNull(result);
      Assert.AreNotEqual(0, result.Count);
    }

    [Test]
    public void TestReadWindowsEvents()
    {
      SettingsHelperController.CurrentSettings.LinesRead = 5;
      var tailData = new TailData
      {
        IsWindowsEvent = true,
        WindowsEvent = new WindowsEventData
        {
          Category = "System"
        }
      };
      _windowEventReader.TailData = tailData;

      _windowEventReader.StartTail();
      _windowEventReader.StopTail();
    }
  }
}
