using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using NUnit.Framework;
using Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule;
using Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule.Interfaces;
using Org.Vs.TailForWin.Core.Data;

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

    private void InitMyTest()
    {
      if ( File.Exists(_path) )
        File.Delete(_path);

      if ( File.Exists(_pathAsJson) )
        File.Delete(_pathAsJson);
    }

    private void CopyTempFile() => File.Copy(_tempPathAsJson, _pathAsJson);
  }
}
