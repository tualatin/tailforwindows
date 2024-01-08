using NUnit.Framework;
using NUnit.Framework.Legacy;
using Org.Vs.TailForWin.Core.Utils;

namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class TestThrottledExecution
  {
    [Test]
    public void TestThrottleExecution()
    {
      string testString = null;
      const string message = "here comes the test";

      new ThrottledExecution().InMs(250).Do(
        () =>
        {
          testString = message;
        });

      ClassicAssert.IsNull(testString);

      System.Threading.Thread.Sleep(500);

      ClassicAssert.AreEqual(message, testString);
    }
  }
}
