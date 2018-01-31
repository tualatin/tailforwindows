using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using NUnit.Framework;
using Org.Vs.TailForWin.PlugIns.PatternModule.Controller;
using Org.Vs.TailForWin.PlugIns.PatternModule.Interfaces;


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
      Assert.IsNull(await defaultPattern.ReadDefaultPatternsAsync().ConfigureAwait(false));

      var patterns = await _xmlPattern.ReadDefaultPatternsAsync().ConfigureAwait(false);
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
