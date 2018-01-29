using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Org.Vs.TailForWin.Core.Utils;

namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class TestGetFileEncoding
  {
    private string _currentTestDirectory;

    [SetUp]
    protected void SetUp()
    {
      _currentTestDirectory = TestContext.CurrentContext.TestDirectory;
    }

    [Test]
    public void TestGetEncodingUtf8Bom()
    {
      string file = _currentTestDirectory + @"\Files\Encoding_UTF8wB.txt";
      var encoding = EncodingDetector.GetEncoding(file);
      Assert.AreEqual(Encoding.UTF8, encoding);
    }

    [Test]
    public void TestGetEncodingAnsi()
    {
      string file = _currentTestDirectory + @"\Files\Encoding_ANSI.txt";
      var encoding = EncodingDetector.GetEncoding(file);
      Assert.AreEqual(Encoding.Default, encoding);
    }

    [Test]
    public async Task TestGetEncodingUtf8BomAsync()
    {
      string file = _currentTestDirectory + @"\Files\Encoding_UTF8wB.txt";
      var encoding = await EncodingDetector.GetEncodingAsync(file);
      Assert.AreEqual(Encoding.UTF8, encoding);
    }

    [Test]
    public async Task TestGetEncodingAnsiAsync()
    {
      string file = _currentTestDirectory + @"\Files\Encoding_ANSI.txt";
      var encoding = await EncodingDetector.GetEncodingAsync(file);
      Assert.AreEqual(Encoding.Default, encoding);
    }
  }
}
