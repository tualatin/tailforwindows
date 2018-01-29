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
    public async Task TestGetEncodingUtf8Bom()
    {
      string file = _currentTestDirectory + @"\Files\Encoding_UTF8wB.txt";
      var encoding = await EncodingDetector.GetEncodingAsync(file).ConfigureAwait(false);
      Assert.AreEqual(Encoding.UTF8, encoding);
    }

    [Test]
    public async Task TestGetEncodingAnsi()
    {
      string file = _currentTestDirectory + @"\Files\Encoding_ANSI.txt";
      var encoding = await EncodingDetector.GetEncodingAsync(file).ConfigureAwait(false);
      Assert.AreEqual(Encoding.Default, encoding);
    }
  }
}
