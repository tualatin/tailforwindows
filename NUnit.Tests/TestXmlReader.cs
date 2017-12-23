using System.Threading.Tasks;
using NUnit.Framework;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Interfaces;


namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class TestXmlReader
  {
    private IXmlReader _xmlReader;

    [SetUp]
    protected void SetUp()
    {
      _xmlReader = new XmlConfigReadController();
    }

    [Test]
    public async Task TestReadXmlConfigFile()
    {
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
