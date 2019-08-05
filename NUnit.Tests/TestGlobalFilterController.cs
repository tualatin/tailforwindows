﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Interfaces;

namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  class TestGlobalFilterController
  {
    private IGlobalFilterController _globalFilterController;
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

    }

    [Test]
    public async Task TestSaveGlobalFiltersAsync()
    {


    }

    private void InitMyTest()
    {
      if ( File.Exists(_pathAsJson) )
        File.Delete(_pathAsJson);
    }

    private void CopyTempFile()
    {
      if ( File.Exists(_pathAsJson) )
        File.Delete(_pathAsJson);

      File.Copy(_tempPathAsJson, _pathAsJson);
    }
  }
}