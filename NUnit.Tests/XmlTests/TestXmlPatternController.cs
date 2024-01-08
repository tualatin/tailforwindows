using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Org.Vs.TailForWin.Controllers.PlugIns.PatternModule;
using Org.Vs.TailForWin.Controllers.PlugIns.PatternModule.Interfaces;


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
      SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());

      _currenTestContext = TestContext.CurrentContext;
      _path = _currenTestContext.TestDirectory + @"\Files\DefaultPatterns.xml";
      _xmlPattern = new XmlPatternController(_path);
    }

    [Test]
    public async Task TestReadDefaultPatternsAsync()
    {
      var defaultPattern = new XmlPatternController(@"C:\blablabla.xml");
      ClassicAssert.IsNull(await defaultPattern.ReadDefaultPatternsAsync().ConfigureAwait(false));

      var patterns = await _xmlPattern.ReadDefaultPatternsAsync().ConfigureAwait(false);
      ClassicAssert.IsNotNull(patterns);
      ClassicAssert.AreEqual(5, patterns.Count);
      ClassicAssert.IsTrue(patterns.Any(p => p.PatternString.Equals("????-??-??")));
      ClassicAssert.IsTrue(patterns.Any(p => p.PatternString.Equals("??-??-????")));
      ClassicAssert.IsTrue(patterns.Any(p => p.PatternString.Equals(@"^\w*_\d+")));
      ClassicAssert.IsTrue(patterns.Any(p => p.PatternString.Equals(@"\d{2}-\d{2}-\d{4}")));
      ClassicAssert.IsTrue(patterns.Any(p =>p.PatternString.Equals(@"\d{4}-\d{2}-\d{2}")));
    }
  }
}
