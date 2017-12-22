using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Org.Vs.NUnit.Tests.Data;
using Org.Vs.TailForWin.Core.Extensions;


namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class TestEnumerableExtension
  {
    private List<string> _testList;
    private ObservableCollection<TestDataObject> _stringCollection;

    [SetUp]
    protected void SetUp()
    {
      _testList = new List<string> { "test1", "test2", "test3", "test4" };
      _stringCollection = new ObservableCollection<TestDataObject>();

      var testData = new TestDataObject
      {
        Id = Guid.NewGuid(),
        TestDouble = 1.1,
        TestInt = 1,
        TestString = "Test1"
      };

      _stringCollection.Add(testData);

      testData = new TestDataObject
      {
        Id = Guid.NewGuid(),
        TestDouble = 1.2,
        TestInt = 2,
        TestString = "Test2"
      };

      _stringCollection.Add(testData);

      testData = new TestDataObject
      {
        Id = Guid.NewGuid(),
        TestDouble = 1.3,
        TestInt = 3,
        TestString = "Test3"
      };

      _stringCollection.Add(testData);
    }

    [Test]
    public void TestEnumerableHasDublicates()
    {
      Assert.IsFalse(_testList.HasDuplicates());

      _testList.Add("test1");
      Assert.IsTrue(_testList.HasDuplicates());
    }

    [Test]
    public void TestEnumerableHasDublicatesNullComparer()
    {
      Assert.Throws<ArgumentException>(() => _testList.HasDuplicates(null));
    }

    [Test]
    public void TestEnumerableFindDublicatesNullNewValueNullComparer()
    {
      Assert.Throws<ArgumentException>(() => _stringCollection.FindDublicates(null, null));
    }

    [Test]
    public void TestEnumerableFindDublicates()
    {
      TestDataObject newValue = new TestDataObject
      {
        Id = Guid.NewGuid(),
        TestDouble = 1.3,
        TestInt = 3,
        TestString = "Test3"
      };

      Assert.IsTrue(_stringCollection.FindDublicates(newValue, new Comparer()));
    }

    [Test]
    public void TestEnumerableFindDublicatesNullComparer()
    {
      Assert.IsFalse(_testList.FindDublicates("blabla", null));
      Assert.IsTrue(_testList.FindDublicates("test1", null));
    }

    private class Comparer : IEqualityComparer<TestDataObject>
    {
      public bool Equals(TestDataObject x, TestDataObject y)
      {
        return y != null && x != null && string.CompareOrdinal(x.TestString, y.TestString) == 0;
      }

      public int GetHashCode(TestDataObject obj)
      {
        return 0;
      }
    }
  }
}
