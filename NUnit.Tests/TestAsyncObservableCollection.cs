using System.Linq;
using NUnit.Framework;
using Org.Vs.TailForWin.Core.Collections;

namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class TestAsyncObservableCollection
  {
    private AsyncObservableCollection<int> _asyncObservableCollection;

    [Test]
    public void TestAddToCollection()
    {
      _asyncObservableCollection = new AsyncObservableCollection<int>();
      Assert.IsNotNull(_asyncObservableCollection);

      _asyncObservableCollection.Add(0);
      Assert.AreEqual(1, _asyncObservableCollection.Count);
      Assert.AreEqual(0, _asyncObservableCollection.First());
    }

    [Test]
    public void TestClearCollection()
    {
      GenerateList();
      Assert.DoesNotThrow(() => _asyncObservableCollection.Clear());
      Assert.AreEqual(0, _asyncObservableCollection.Count);
    }

    [Test]
    public void TestContainsCollection()
    {
      GenerateList();
      Assert.IsTrue(_asyncObservableCollection.Contains(4));
    }

    [Test]
    public void TestIndexOfCollection()
    {
      GenerateList();
      Assert.AreEqual(2, _asyncObservableCollection.IndexOf(2));
    }

    private void GenerateList()
    {
      _asyncObservableCollection = new AsyncObservableCollection<int>
      {
        0,
        1,
        2,
        3,
        4,
        5,
        6
      };
      Assert.AreEqual(7, _asyncObservableCollection.Count);
    }
  }
}
