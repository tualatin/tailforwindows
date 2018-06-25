using System.Threading.Tasks;
using NUnit.Framework;
using Org.Vs.TailForWin.Business.Services;
using Org.Vs.TailForWin.Business.Services.Interfaces;
using Org.Vs.TailForWin.Core.Controllers;

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
      var result = await _windowEventReader.GetCategoriesAsync().ConfigureAwait(false);

      Assert.IsNotNull(result);
      Assert.AreNotEqual(0, result.Count);
    }

    [Test]
    public void TestReadWindowsEvents()
    {
      SettingsHelperController.CurrentSettings.LinesRead = 5;
      _windowEventReader.StartTail("System");
      _windowEventReader.StopTail();
    }
  }
}
