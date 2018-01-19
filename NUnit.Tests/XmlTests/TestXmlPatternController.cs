using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Org.Vs.TailForWin.Core.Controllers.XmlCore;
using Org.Vs.TailForWin.Core.Interfaces.XmlCore;


namespace Org.Vs.NUnit.Tests.XmlTests
{
  [TestFixture]
  public class TestXmlPatternController
  {
    private IXmlPattern _xmlPattern;
    private TestContext _currenTestContext;
    private string _path;

    [SetUp]
    protected void SetUp()
    {
      _currenTestContext = TestContext.CurrentContext;
      _path = _currenTestContext.TestDirectory + @"\Files\DefaultPatterns.xml";
      _xmlPattern = new XmlPatternController(_path);
    }

    [Test]
    public async Task TestReadDefaultPatterns()
    {
      var defaultPattern = new XmlPatternController(@"C:\blablabla.xml");
      Assert.IsNull(await defaultPattern.ReadDefaultPatternsAsync());

      var patterns = await _xmlPattern.ReadDefaultPatternsAsync();
      Assert.IsNotNull(patterns);
      Assert.AreEqual(5, patterns.Count);
      Assert.IsTrue(patterns.Any(p => p.PatternString.Equals("????-??-??")));
      Assert.IsTrue(patterns.Any(p => p.PatternString.Equals("??-??-????")));
      Assert.IsTrue(patterns.Any(p => p.PatternString.Equals(@"^\w*_\d+")));
      Assert.IsTrue(patterns.Any(p => p.PatternString.Equals(@"\d{2}-\d{2}-\d{4}")));
      Assert.IsTrue(patterns.Any(p =>p.PatternString.Equals(@"\d{4}-\d{2}-\d{2}")));
    }
  }
}
