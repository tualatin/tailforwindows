using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Org.Vs.TailForWin.Core.Collections;
using Org.Vs.TailForWin.Core.Interfaces;


namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class TestObservableDictionary
  {
    private Dictionary<int, string> _dictionary;
    private IObservableDictionary<int, string> _collection;

    [SetUp]
    protected void Setup()
    {
      _dictionary = new Dictionary<int, string>
      {
        { 1, "test 1" },
        { 2, "test 2" },
        { 3, "test 3" }
      };
    }

    [Test]
    public void TestCreateObservableDictionary()
    {
      _collection = new ObservableDictionary<int, string>();
      Assert.IsNotNull(_collection);
      Assert.AreEqual(0, _collection.Count);

      _collection = new ObservableDictionary<int, string>(_dictionary);
      Assert.AreEqual(3, _collection.Count);
      Assert.IsFalse(_collection.IsReadOnly);

      Assert.AreEqual(3, _collection.Keys.Count);
      Assert.AreEqual(3, _collection.Values.Count);
      Assert.AreEqual(_dictionary.Keys, _collection.Keys);
      Assert.AreEqual(_dictionary.Values, _collection.Values);
    }

    [Test]
    public void TestKeyIndexAccessor()
    {
      Assert.IsNotNull(_collection);
      Assert.AreEqual("test 2", _collection[2]);

      _collection[1] = "change test";
      Assert.AreEqual("change test", _collection[1]);

      _collection[4] = "test 4";
      Assert.AreEqual(4, _collection.Count);
      Assert.AreEqual("test 4", _collection[4]);
    }

    [Test]
    public void TestGetIEnumerator()
    {
      _collection = new ObservableDictionary<int, string>(_dictionary);

      using ( var numerator = ((ObservableDictionary<int, string>) _collection)?.GetEnumerator() )
      {
        Assert.NotNull(numerator);

        numerator.MoveNext();
        Assert.AreEqual(_dictionary.First(), numerator.Current);
      }
    }

    [Test]
    public void TestTryGetValue()
    {
      _collection = new ObservableDictionary<int, string>(_dictionary);
      Assert.IsTrue(_collection.TryGetValue(1, out string item));
      Assert.AreEqual(_dictionary[1], item);
    }

    [Test]
    public void TestRemoveItem()
    {
      _collection = new ObservableDictionary<int, string>(_dictionary);
      Assert.IsTrue(_collection.Remove(1));
      Assert.AreEqual(_dictionary.Count - 1, _collection.Count);

      Assert.IsTrue(_collection.Remove(_dictionary.Last()));
      Assert.AreEqual(_dictionary.Count - 2, _collection.Count);
    }

    [Test]
    public void TestContainsItem()
    {
      _collection = new ObservableDictionary<int, string>(_dictionary);
      Assert.IsTrue(_collection.ContainsKey(1));
      Assert.IsFalse(_collection.ContainsKey(4));
      Assert.IsTrue(_collection.Contains(_dictionary.First()));
      Assert.IsFalse(_collection.Contains(new KeyValuePair<int, string>(5, "test 5")));
    }

    [Test]
    public void TestClearsCollection()
    {
      _collection = new ObservableDictionary<int, string>(_dictionary);
      Assert.Greater(_collection.Count, 0);

      _collection.Clear();
      Assert.AreEqual(0, _collection.Count);
    }

    [Test]
    public void TestAddItemToCollection()
    {
      _collection = new ObservableDictionary<int, string>(_dictionary);
      Assert.Greater(_collection.Count, 2);

      Assert.DoesNotThrow(() => _collection.Add(5, "test 5"));
      Assert.IsTrue(_collection.ContainsKey(5));
      Assert.AreEqual(4, _collection.Count);

      Assert.DoesNotThrow(() => _collection.Add(new KeyValuePair<int, string>(6, "test 6")));
      Assert.IsTrue(_collection.Contains(new KeyValuePair<int, string>(6, "test 6")));
      Assert.IsFalse(_collection.Contains(new KeyValuePair<int, string>(7, "test 7")));
      Assert.That(() => _collection.Add(new KeyValuePair<int, string>(6, "test 6")), Throws.InstanceOf<ArgumentException>());
    }

    [Test]
    public void TestAddRangeToCollection()
    {
      _collection = new ObservableDictionary<int, string>(_dictionary);
      Assert.Greater(_collection.Count, 2);
      Assert.That(() => _collection.AddRange(null), Throws.InstanceOf<ArgumentException>());
      Assert.Throws<ArgumentException>(() => _collection.AddRange(
          new Dictionary<int, string>
          {
            {
              3, "test 3"
            }
          }));

      var newDictionary = new Dictionary<int, string>
      {
        {
          7, "test 7"
        },
        {
          8, "test 8"
        },
        {
          9, "test 9"
        }
      };
      Assert.DoesNotThrow(() => _collection.AddRange(newDictionary));
      Assert.Greater(_collection.Count, 3);
      Assert.IsTrue(_collection.ContainsKey(9));
      Assert.IsTrue(_collection.ContainsKey(7));
    }

    [Test]
    public void TestCopyCollectionTo()
    {
      _collection = new ObservableDictionary<int, string>(_dictionary);
      Assert.Greater(_collection.Count, 2);

      KeyValuePair<int, string>[] array = new KeyValuePair<int, string>[_collection.Count];
      Assert.DoesNotThrow(() => _collection.CopyTo(array, 0));
      Assert.AreEqual(3, array.Length);
      Assert.AreEqual(_dictionary.First(),  array[0]);
      Assert.Throws<ArgumentException>(() => _collection.CopyTo(null, 0));
    }
  }
}
