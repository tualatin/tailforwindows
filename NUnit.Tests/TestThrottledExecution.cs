using NUnit.Framework;
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

      Assert.IsNull(testString);

      System.Threading.Thread.Sleep(500);

      Assert.AreEqual(message, testString);
    }
  }
}
