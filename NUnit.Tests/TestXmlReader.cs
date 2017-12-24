using System.IO;
using System.Threading.Tasks;
using System.Xml;
using NUnit.Framework;
using Org.Vs.TailForWin.Core.Controllers;
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
      Assert.That(() => xmlReader.ReadXmlFile(), Throws.InstanceOf<FileNotFoundException>());

      var path = _currenTestContext.TestDirectory + @"\FileManager_Root.xml";
      xmlReader = new XmlConfigReadController(path);
      Assert.That(() => xmlReader.ReadXmlFile(), Throws.InstanceOf<XmlException>());

      var files = await _xmlReader.ReadXmlFile();
      Assert.NotNull(files);
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
