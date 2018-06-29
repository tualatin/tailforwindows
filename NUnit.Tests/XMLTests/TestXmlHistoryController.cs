﻿using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml.Linq;
using NUnit.Framework;
using Org.Vs.TailForWin.Controllers.PlugIns.FindModule;
using Org.Vs.TailForWin.Core.Interfaces;


namespace Org.Vs.NUnit.Tests.XmlTests
{
  [TestFixture]
  public class TestXmlHistoryController
  {
    private IXmlSearchHistory<IObservableDictionary<string, string>> _xmlHistory;
    private TestContext _currenTestContext;
    private string _path;
    private string _tempPath;

    [SetUp]
    protected void SetUp()
    {
      SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());

      _currenTestContext = TestContext.CurrentContext;
      _path = _currenTestContext.TestDirectory + @"\History.xml";
      _tempPath = _currenTestContext.TestDirectory + @"\Files\History.xml";
    }

    [Test]
    public async Task TestReadXmlHistoryFileAsync()
    {
      IXmlSearchHistory<IObservableDictionary<string, string>> xmlReader = new XmlSearchHistoryController(@"C:\blabla\HistoryTest.xml");
      var reader = xmlReader;
      Assert.That(() => reader.ReadXmlFileAsync(), Throws.InstanceOf<FileNotFoundException>());

      InitXmlReader();

      var history = await _xmlHistory.ReadXmlFileAsync().ConfigureAwait(false);
      Assert.NotNull(history);
      Assert.AreEqual(3, history.Count);
      Assert.IsFalse(_xmlHistory.Wrap);
      Assert.IsTrue(history.Values.Any(p => p.Equals("error")));
      Assert.IsTrue(history.Values.Any(p => p.Equals("debug")));
      Assert.IsTrue(history.Values.Any(p => p.Equals("testsearch")));
      Assert.IsFalse(history.Values.Any(p => p.Equals("blabla")));
    }

    [Test]
    public async Task TestSaveWrapAttributeAsync()
    {
      InitXmlReader();

      await _xmlHistory.ReadXmlFileAsync().ConfigureAwait(false);
      _xmlHistory.Wrap = true;
      var wrap = await _xmlHistory.SaveSearchHistoryWrapAttributeAsync().ConfigureAwait(false);
      Assert.IsNotNull(wrap);
      Assert.IsInstanceOf<XElement>(wrap);

      _xmlHistory.Wrap = false;
      var history = await _xmlHistory.ReadXmlFileAsync().ConfigureAwait(false);
      Assert.NotNull(history);
      Assert.IsTrue(_xmlHistory.Wrap);
    }

    [Test]
    public async Task TestSaveXmlHistoryFileAsync()
    {
      InitXmlReader();

      await _xmlHistory.ReadXmlFileAsync().ConfigureAwait(false);

      _xmlHistory.Wrap = true;

      await _xmlHistory.SaveSearchHistoryWrapAttributeAsync().ConfigureAwait(false);
      await _xmlHistory.SaveSearchHistoryAsync("test1234").ConfigureAwait(false);

      _xmlHistory.Wrap = false;
      var history = await _xmlHistory.ReadXmlFileAsync().ConfigureAwait(false);

      Assert.NotNull(history);
      Assert.IsTrue(history.Values.Any(p => p.Equals("test1234")));
      Assert.IsTrue(_xmlHistory.Wrap);
    }

    private void InitXmlReader()
    {
      if ( File.Exists(_path) )
        File.Delete(_path);

      File.Copy(_tempPath, _path);
      _xmlHistory = new XmlSearchHistoryController(_path);
    }
  }
}
