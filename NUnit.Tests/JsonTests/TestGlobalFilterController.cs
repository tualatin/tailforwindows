using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Org.Vs.TailForWin.Controllers.PlugIns.GlobalHighlightModule;
using Org.Vs.TailForWin.Controllers.PlugIns.GlobalHighlightModule.Interfaces;
using Org.Vs.TailForWin.Core.Data;

namespace Org.Vs.NUnit.Tests.JsonTests
{
  [TestFixture]
  public class TestGlobalFilterController
  {
    private IGlobalFilterController _globalFilterController;
    private CancellationTokenSource _cts;
    private ObservableCollection<FilterData> _globalFilters;
    private TestContext _currentTestContext;
    private string _pathAsJson;
    private string _tempPathAsJson;

    [SetUp]
    protected void SetUp()
    {
      _globalFilters = new ObservableCollection<FilterData>();
      _currentTestContext = TestContext.CurrentContext;

      _pathAsJson = _currentTestContext.TestDirectory + @"\GlobalFilters.json";
      _tempPathAsJson = _currentTestContext.TestDirectory + @"\Files\GlobalFilters.json";

      _globalFilterController = new GlobalFilterController(_pathAsJson);
    }

    [Test]
    public void TestCreateGlobalFilter()
    {
      InitMyTest();

      var filter = new FilterData
      {
        Description = "Test global filter 1",
        Filter = "debug",
        IsEnabled = true,
        FindSettingsData = new FindData
        {
          CaseSensitive = false,
          UseRegex = false,
          UseWildcard = false
        },
        FilterColorHex = "#2F4F4F"
      };

      _globalFilters.Add(filter);
      Assert.IsTrue(_globalFilters.Count == 1);
      Assert.ThrowsAsync<ArgumentException>(() => _globalFilterController.UpdateGlobalFilterAsync(null));
      Assert.DoesNotThrowAsync(() => _globalFilterController.UpdateGlobalFilterAsync(_globalFilters));
      Assert.IsTrue(File.Exists(_pathAsJson));
      Assert.IsTrue(_globalFilters.All(p => p.IsGlobal));
    }

    [Test]
    public async Task TestDeleteGlobalFilterAsync()
    {

    }

    [Test]
    public async Task LoadGlobalFiltersAsync()
    {
      InitMyTest();
      CopyTempFile();

      Assert.DoesNotThrowAsync(() => _globalFilterController.ReadGlobalFiltersAsync(_cts.Token));

      var filters = await _globalFilterController.ReadGlobalFiltersAsync(_cts.Token).ConfigureAwait(false);

      Assert.IsInstanceOf<ObservableCollection<FilterData>>(filters);
      Assert.IsTrue(filters.Count > 0);
      Assert.IsTrue(filters.Count == 3);
      Assert.IsTrue(filters.All(p => p.IsGlobal));
    }

    [Test]
    public async Task TestSaveGlobalFiltersAsync()
    {


    }

    private void InitMyTest()
    {
      if ( File.Exists(_pathAsJson) )
        File.Delete(_pathAsJson);

      _cts?.Dispose();
      _cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
    }

    private void CopyTempFile()
    {
      if ( File.Exists(_pathAsJson) )
        File.Delete(_pathAsJson);

      File.Copy(_tempPathAsJson, _pathAsJson);
    }
  }
}
