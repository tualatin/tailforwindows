using NUnit.Framework;
using Org.Vs.TailForWin.Core.Utils;

namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class TestQueueSet
  {
    private QueueSet<string> queue;

    [SetUp]
    protected void SetUp()
    {
      queue = new QueueSet<string>(5)
      {
        "test 1",
        "test 2",
        "test 3",
        "test 4",
        "test 5",
        "test 6",
        "test 7"
      };
    }

    [Test]
    public void TestEnqueueSet()
    {
      Assert.AreEqual(5, queue.Count);
      Assert.IsFalse(queue.Contains("test 1"));
      Assert.IsFalse(queue.Contains("test 2"));

      Assert.IsTrue(queue.Contains("test 7"));
      Assert.IsTrue(queue.Contains("test 5"));
    }

    [Test]
    public void TestDequeueSet()
    {
      Assert.AreEqual(5, queue.Count);

      string element = queue.Dequeue();

      Assert.AreEqual(4, queue.Count);
      Assert.AreEqual("test 3", element);
    }
  }
}
