using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using NUnit.Framework;
using Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule;
using Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule.Interfaces;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Enums;

namespace Org.Vs.NUnit.Tests.JsonTests
{
  [TestFixture]
  public class TestFileManagerController
  {
    private IFileManagerController _fileManagerController;
    private TestContext _currentTestContext;
    private CancellationTokenSource _cts;
    private string _pathAsJson;
    private string _path;
    private string _tempPathAsJson;
    private string _tempPath;

    [SetUp]
    protected void SetUp()
    {
      SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());

      if ( Application.Current == null )
        Application.LoadComponent(new Uri("/T4W;component/app.xaml", UriKind.RelativeOrAbsolute));

      _cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
      _currentTestContext = TestContext.CurrentContext;
      _tempPath = _currentTestContext.TestDirectory + @"\Files\FileManager.xml";
      _path = _currentTestContext.TestDirectory + @"\FileManager.xml";

      _pathAsJson = _currentTestContext.TestDirectory + @"\FileManager.json";
      _tempPathAsJson = _currentTestContext.TestDirectory + @"\Files\FileManager.json";

      _fileManagerController = new FileManagerController(_pathAsJson, _path);
    }

    [Test]
    public async Task TestConvertXmlToJsonConfigAsync()
    {
      InitMyTest();

      var result = await _fileManagerController.ConvertXmlToJsonConfigAsync(_cts.Token).ConfigureAwait(false);
      Assert.IsTrue(result);
      Assert.IsFalse(File.Exists(_path));
      Assert.IsTrue(File.Exists(_pathAsJson));
    }

    [Test]
    public async Task TestReadJsonConfigAsync()
    {
      InitMyTest();
      CopyTempFile();

      var result = await _fileManagerController.ReadJsonFileAsync(_cts.Token).ConfigureAwait(false);
      Assert.NotNull(result);
      Assert.IsTrue(result.Count > 0);

      var guid = Guid.Parse("ce14d954-9336-4b8f-8160-67362f2f11a2");
      TailData first = null;

      foreach ( TailData p in result )
      {
        if ( p.Id != guid )
          continue;

        first = p;
        break;
      }

      Assert.NotNull(first);
      Assert.AreEqual("ce14d954-9336-4b8f-8160-67362f2f11a2", first.Id.ToString());
    }

    [Test]
    public async Task TestGetCategoriesAsync()
    {
      CopyTempFile();

      var categories = await _fileManagerController.GetCategoriesAsync(_cts.Token).ConfigureAwait(false);
      Assert.NotNull(categories);
      Assert.IsTrue(categories.Count > 0);
      Assert.AreEqual(2, categories.Count);
    }

    [Test]
    public async Task TestGetCategoriesWithListAsync()
    {
      CopyTempFile();

      var result = await _fileManagerController.ReadJsonFileAsync(_cts.Token).ConfigureAwait(false);
      var categories = await _fileManagerController.GetCategoriesAsync(_cts.Token, result).ConfigureAwait(false);
      Assert.NotNull(categories);
      Assert.IsTrue(categories.Count > 0);
      Assert.AreEqual(2, categories.Count);
    }

    [Test]
    public async Task TestGetTailDataByIdAsync()
    {
      CopyTempFile();

      var result = await _fileManagerController.ReadJsonFileAsync(_cts.Token).ConfigureAwait(false);
      Assert.NotNull(result);
      Assert.IsTrue(result.Count > 0);

      var guid = Guid.Parse("ce14d954-9336-4b8f-8160-67362f2f11a2");
      var tailData = await _fileManagerController.GetTailDataByIdAsync(result, guid, _cts.Token).ConfigureAwait(false);
      Assert.NotNull(tailData);
      Assert.AreEqual("ce14d954-9336-4b8f-8160-67362f2f11a2", tailData.Id.ToString());
    }

    [Test]
    public async Task TestUpdateJsonFileAsync()
    {
      CopyTempFile();

      var tailData = new TailData
      {
        FileName = @"D:\Tools\TailForWindows\logs\tailforwindows_2017-03-06.log",
        Description = "Tail4Windows",
        Category = "T4F",
        ThreadPriority = ThreadPriority.Lowest,
        NewWindow = false,
        RefreshRate = ETailRefreshRate.Highest,
        Timestamp = false,
        RemoveSpace = false,
        Wrap = false,
        FileEncoding = Encoding.ASCII,
        FilterState = true,
        UsePattern = true,
        FontType = new FontType(),
        SmartWatch = false,
        TabItemBackgroundColorStringHex = "#FFE5C365",
        PatternString = "tailforwindows_????-??-??.log",
        ListOfFilter = new ObservableCollection<FilterData>
        {
          new FilterData
          {
            Description = "Error filter",
            Filter = "error",
            FontType = new FontType()
          },
          new FilterData
          {
            Description = "Debug filter",
            Filter = "debug",
            FontType = new FontType()
          }
        }
      };

      var result = await _fileManagerController.ReadJsonFileAsync(_cts.Token).ConfigureAwait(false);
      Assert.NotNull(result);
      Assert.IsTrue(result.Count > 0);

      result.Add(tailData);
      var success = await _fileManagerController.CreateUpdateJsonFileAsync(result, _cts.Token).ConfigureAwait(false);
      Assert.IsTrue(success);
    }

    [Test]
    public async Task TestAddTailDataToJsonFileAsync()
    {
      CopyTempFile();

      var tailData = new TailData
      {
        FileName = @"D:\Tools\TailForWindows\logs\addTailDataToJsonFile.log",
        Description = "Add TailData to JSON file",
        Category = "T4F",
        ThreadPriority = ThreadPriority.Normal,
        NewWindow = false,
        RefreshRate = ETailRefreshRate.Highest,
        Timestamp = true,
        RemoveSpace = true,
        Wrap = true,
        FileEncoding = Encoding.UTF8,
        FilterState = true,
        FontType = new FontType(),
        SmartWatch = false,
        TabItemBackgroundColorStringHex = "#FFE5C365",
        ListOfFilter = new ObservableCollection<FilterData>
        {
          new FilterData
          {
            Description = "Trace filter",
            Filter = "trace",
            FontType = new FontType()
          },
          new FilterData
          {
            Description = "Debug filter",
            Filter = "debug",
            FontType = new FontType()
          }
        }
      };

      var success = await _fileManagerController.AddTailDataAsync(tailData, _cts.Token).ConfigureAwait(false);
      Assert.IsTrue(success);

      var result = await _fileManagerController.ReadJsonFileAsync(_cts.Token).ConfigureAwait(false);
      Assert.NotNull(result);
      Assert.IsTrue(result.Count > 0);
      Assert.IsTrue(result.Count == 3);
    }

    private void InitMyTest()
    {
      if ( File.Exists(_path) )
        File.Delete(_path);

      if ( File.Exists(_pathAsJson) )
        File.Delete(_pathAsJson);

      File.Copy(_tempPath, _path);
    }

    private void CopyTempFile()
    {
      if ( File.Exists(_pathAsJson) )
        File.Delete(_pathAsJson);

      File.Copy(_tempPathAsJson, _pathAsJson);
    }
  }
}
