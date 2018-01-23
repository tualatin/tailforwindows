using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;
using Org.Vs.TailForWin.PlugIns.FindModule.Controller;
using Org.Vs.TailForWin.PlugIns.FindModule.Interfaces;


namespace Org.Vs.NUnit.Tests.XmlTests
{
  [TestFixture]
  public class TestXmlHistoryController
  {
    private IXmlSearchHistory _xmlHistory;
    private TestContext _currenTestContext;
    private string _path;
    private string _tempPath;

    [SetUp]
    protected void SetUp()
    {
      _currenTestContext = TestContext.CurrentContext;
      _path = _currenTestContext.TestDirectory + @"\History.xml";
      _tempPath = _currenTestContext.TestDirectory + @"\Files\History.xml";
    }

    [Test]
    public async Task TestReadXmlHistoryFile()
    {
      IXmlSearchHistory xmlReader = new XmlSearchHistoryController(@"C:\blabla\HistoryTest.xml");
      var reader = xmlReader;
      Assert.That(() => reader.ReadXmlFileAsync(), Throws.InstanceOf<FileNotFoundException>());

      InitXmlReader();

      var history = await _xmlHistory.ReadXmlFileAsync();
      Assert.NotNull(history);
      Assert.AreEqual(3, history.Count);
      Assert.IsFalse(_xmlHistory.Wrap);
      Assert.IsTrue(history.Values.Any(p => p.Equals("error")));
      Assert.IsTrue(history.Values.Any(p => p.Equals("debug")));
      Assert.IsTrue(history.Values.Any(p => p.Equals("testsearch")));
      Assert.IsFalse(history.Values.Any(p => p.Equals("blabla")));
    }

    [Test]
    public async Task TestSaveWrapAttribute()
    {
      InitXmlReader();

      await _xmlHistory.ReadXmlFileAsync();
      _xmlHistory.Wrap = true;
      var wrap = await _xmlHistory.SaveSearchHistoryWrapAttributeAsync();
      Assert.IsNotNull(wrap);
      Assert.IsInstanceOf<XElement>(wrap);

      _xmlHistory.Wrap = false;
      var history = await _xmlHistory.ReadXmlFileAsync();
      Assert.NotNull(history);
      Assert.IsTrue(_xmlHistory.Wrap);
    }

    [Test]
    public async Task TestSaveXmlHistoryFile()
    {
      InitXmlReader();

      await _xmlHistory.ReadXmlFileAsync();

      _xmlHistory.Wrap = true;

      await _xmlHistory.SaveSearchHistoryWrapAttributeAsync();
      await _xmlHistory.SaveSearchHistoryAsync("test1234");

      _xmlHistory.Wrap = false;
      var history = await _xmlHistory.ReadXmlFileAsync();

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
