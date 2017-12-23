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
    public async void TestReadXmlConfigFile()
    {
    }

    [Test]
    public async void TestWriteXmlConfigFile()
    {
    }

    [Test]
    public async void TestGetNodeById()
    {
    }

    [Test]
    public async void TestUpdateXmlConfigFile()
    {
    }

    [Test]
    public async void TestRemoveXmlElement()
    {

    }

  }
}
