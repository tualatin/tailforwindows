using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using NUnit.Framework.Legacy;
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
      ClassicAssert.IsFalse(_testList.HasDuplicates());

      _testList.Add("test1");
      ClassicAssert.IsTrue(_testList.HasDuplicates());
    }

    [Test]
    public void TestEnumerableHasDublicatesNullComparer() => Assert.Throws<ArgumentException>(() => _testList.HasDuplicates(null));

    [Test]
    public void TestEnumerableFindDublicatesNullNewValueNullComparer() => Assert.Throws<ArgumentException>(() => _stringCollection.IsAlreadyExists(null, null));

    [Test]
    public void TestEnumerableIsAlreadyExists()
    {
      TestDataObject newValue = new TestDataObject
      {
        Id = Guid.NewGuid(),
        TestDouble = 1.3,
        TestInt = 3,
        TestString = "Test3"
      };

      ClassicAssert.IsTrue(_stringCollection.IsAlreadyExists(newValue, new Comparer()));
    }

    [Test]
    public void TestEnumerableIsAlreadyExistsNullComparer()
    {
      ClassicAssert.IsFalse(_testList.IsAlreadyExists("blabla", null));
      ClassicAssert.IsTrue(_testList.IsAlreadyExists("test1", null));
    }

    [Test]
    public void TestEnumerableCompare()
    {
      var testList = new List<string> { "test1", "test2", "test3", "test4" };
      ClassicAssert.IsTrue(_testList.CompareGenericObservableCollections(testList));

      testList.Add("test5");
      ClassicAssert.IsFalse(_testList.CompareGenericObservableCollections(testList));
    }

    [Test]
    public void TestListCompare()
    {
      List<int> a = new List<int> { 1, 2, 3, 1, 4, 10 };
      List<int> b = new List<int> { 1, 2, 3, 1, 4, 10 };
      ClassicAssert.IsTrue(a.CompareGenericObservableCollections(b));

      List<int> c = new List<int> { 4, 6, 10, 20, 23, 10 };
      ClassicAssert.IsFalse(b.CompareGenericObservableCollections(c));
    }

    private class Comparer : IEqualityComparer<TestDataObject>
    {
      public bool Equals(TestDataObject x, TestDataObject y) => y != null && x != null && string.CompareOrdinal(x.TestString, y.TestString) == 0;

      public int GetHashCode(TestDataObject obj) => 0;
    }
  }
}
