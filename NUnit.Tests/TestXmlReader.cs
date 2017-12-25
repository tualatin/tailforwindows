using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using NUnit.Framework;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Interfaces;


namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class TestXmlReader
  {
    private IXmlReader _xmlReader;
    private TestContext _currenTestContext;

    [SetUp]
    protected void SetUp()
    {
      _currenTestContext = TestContext.CurrentContext;
      var path = _currenTestContext.TestDirectory + @"\FileManager.xml";
      _xmlReader = new XmlConfigReadController(path);
    }

    [Test]
    public async Task TestReadXmlConfigFile()
    {
      IXmlReader xmlReader = new XmlConfigReadController(@"C:\blabla\Test.xml");
      var reader = xmlReader;
      Assert.That(() => reader.ReadXmlFile(), Throws.InstanceOf<FileNotFoundException>());

      var path = _currenTestContext.TestDirectory + @"\FileManager_Root.xml";
      xmlReader = new XmlConfigReadController(path);
      Assert.That(() => xmlReader.ReadXmlFile(), Throws.InstanceOf<XmlException>());

      var files = await _xmlReader.ReadXmlFile().ConfigureAwait(false);
      Assert.NotNull(files);
      Assert.AreEqual(2, files.Count);
      Assert.IsInstanceOf<TailData>(files.First());
    }

    [Test]
    public async Task TestGetCategories()
    {
      var files = await _xmlReader.ReadXmlFile().ConfigureAwait(false);
      var categories = await _xmlReader.GetCategories(files).ConfigureAwait(false);
      Assert.NotNull(categories);
      Assert.AreEqual(2, categories.Count);
      Assert.AreEqual("T4F", categories.First());
      Assert.AreEqual("MS Setup", categories.Last());
      Assert.That(() => _xmlReader.GetCategories(null), Throws.InstanceOf<ArgumentException>());
    }

    [Test]
    public async Task TestWriteXmlConfigFile()
    {
    }

    [Test]
    public async Task TestGetNodeById()
    {
    }

    [Test]
    public async Task TestUpdateXmlConfigFile()
    {
    }

    [Test]
    public async Task TestRemoveXmlElement()
    {

    }
  }
}
