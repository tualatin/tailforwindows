using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using NUnit.Framework;
using Org.Vs.TailForWin.Controllers.PlugIns.FindModule;
using Org.Vs.TailForWin.Core.Interfaces;

namespace Org.Vs.NUnit.Tests.JsonTests
{
  [TestFixture]
  public class TestHistoryController
  {
    private IHistory _historyController;
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
      _tempPath = _currentTestContext.TestDirectory + @"\Files\History.xml";
      _path = _currentTestContext.TestDirectory + @"\History.xml";

      _pathAsJson = _currentTestContext.TestDirectory + @"\History.json";
      _tempPathAsJson = _currentTestContext.TestDirectory + @"\Files\History.json";
      _historyController = new HistoryController(_pathAsJson, _path);
    }

    [Test]
    public async Task TestConvertXmlToJsonAsync()
    {
      InitMyTest();

      var result = await _historyController.ConvertXmlToJsonFileAsync(_cts.Token).ConfigureAwait(false);
      Assert.IsTrue(result);
      Assert.IsFalse(File.Exists(_path));
      Assert.IsTrue(File.Exists(_pathAsJson));
    }

    [Test]
    public async Task TestReadHistoryFileAsync()
    {
      InitMyTest();
      CopyTempFile();

      var result = await _historyController.ReadHistoryAsync(_cts.Token).ConfigureAwait(false);
      Assert.NotNull(result);
      Assert.IsTrue(result.Wrap);
      Assert.IsTrue(result.FindCollection.Count > 1);
    }

    [Test]
    public async Task TestSaveHistoryAsync()
    {
      InitMyTest();
      CopyTempFile();

      var searchText = "Hello world";
      var oldHistory = await _historyController.ReadHistoryAsync(_cts.Token);
      var result = await _historyController.SaveHistoryAsync(searchText, _cts.Token);
      Assert.IsTrue(result);

      var newHistory = await _historyController.ReadHistoryAsync(_cts.Token);
      Assert.IsTrue(newHistory.FindCollection.Contains(searchText));
      Assert.IsTrue(oldHistory.FindCollection.Count < newHistory.FindCollection.Count);
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
