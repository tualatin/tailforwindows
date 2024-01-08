using NUnit.Framework;
using NUnit.Framework.Legacy;
using Org.Vs.TailForWin.Core.Collections;

namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class TestQueueSet
  {
    private QueueSet<string> _queue;

    [SetUp]
    protected void SetUp() =>
      _queue = new QueueSet<string>(5)
      {
        "test 1",
        "test 2",
        "test 3",
        "test 4",
        "test 5",
        "test 6",
        "test 7"
      };

    [Test]
    public void TestEnqueueSet()
    {
      ClassicAssert.AreEqual(5, _queue.Count);
      ClassicAssert.IsFalse(_queue.Contains("test 1"));
      ClassicAssert.IsFalse(_queue.Contains("test 2"));

      ClassicAssert.IsTrue(_queue.Contains("test 7"));
      ClassicAssert.IsTrue(_queue.Contains("test 5"));
    }

    [Test]
    public void TestDequeueSet()
    {
      ClassicAssert.AreEqual(5, _queue.Count);

      string element = _queue.Dequeue();

      ClassicAssert.AreEqual(4, _queue.Count);
      ClassicAssert.AreEqual("test 3", element);
    }
  }
}
