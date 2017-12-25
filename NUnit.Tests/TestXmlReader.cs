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
    public async Task TestGetNodeById()
    {
      var id = Guid.Parse("8a0c7206-7d0e-4d81-a25c-1d4accca09b7");

      var files = await _xmlReader.ReadXmlFile().ConfigureAwait(false);
      var tailData = await _xmlReader.GetNodeById(files, id).ConfigureAwait(false);
      Assert.NotNull(tailData);
      Assert.AreEqual("Tail4Windows", tailData.Description);
      Assert.AreEqual(2, tailData.ListOfFilter.Count);
      Assert.That(() => _xmlReader.GetNodeById(null, id), Throws.InstanceOf<ArgumentException>());
      Assert.That(() => _xmlReader.GetNodeById(files, Guid.Empty), Throws.InstanceOf<ArgumentException>());
    }

    [Test]
    public async Task TestUpdateXmlConfigFile()
    {
    }

    [Test]
    public async Task TestRemoveXmlElement()
    {
      var id = "8a0c7206-7d0e-4d81-a25c-1d4accca09b7";
      await _xmlReader.ReadXmlFile().ConfigureAwait(false);
      await _xmlReader.DeleteXmlElement(id).ConfigureAwait(false);
      var files = await _xmlReader.ReadXmlFile().ConfigureAwait(false);

      Assert.AreEqual(1, files.Count);
      Assert.That(() => _xmlReader.DeleteXmlElement(null), Throws.InstanceOf<ArgumentException>());
    }

    [Test]
    public async Task TestRemoveFilterElement()
    {
      var id = "8a0c7206-7d0e-4d81-a25c-1d4accca09b7";
      var idFilter = "e8378c20-c0cc-457e-872f-c4139539dec9";
      await _xmlReader.DeleteFilterElement(id, idFilter);
    }
  }
}
