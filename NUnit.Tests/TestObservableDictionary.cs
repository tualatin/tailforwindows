using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
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
    protected void Setup() =>
      _dictionary = new Dictionary<int, string>
      {
        { 1, "test 1" },
        { 2, "test 2" },
        { 3, "test 3" }
      };

    [Test]
    public void TestCreateObservableDictionary()
    {
      _collection = new ObservableDictionary<int, string>();
      ClassicAssert.IsNotNull(_collection);
      ClassicAssert.AreEqual(0, _collection.Count);

      _collection = new ObservableDictionary<int, string>(_dictionary);
      ClassicAssert.AreEqual(3, _collection.Count);
      ClassicAssert.IsFalse(_collection.IsReadOnly);

      ClassicAssert.AreEqual(3, _collection.Keys.Count);
      ClassicAssert.AreEqual(3, _collection.Values.Count);
      ClassicAssert.AreEqual(_dictionary.Keys, _collection.Keys);
      ClassicAssert.AreEqual(_dictionary.Values, _collection.Values);
    }

    [Test]
    public void TestKeyIndexAccessor()
    {
      ClassicAssert.IsNotNull(_collection);
      ClassicAssert.AreEqual("test 2", _collection[2]);

      _collection[1] = "change test";
      ClassicAssert.AreEqual("change test", _collection[1]);

      _collection[4] = "test 4";
      ClassicAssert.AreEqual(4, _collection.Count);
      ClassicAssert.AreEqual("test 4", _collection[4]);
    }

    [Test]
    public void TestGetIEnumerator()
    {
      _collection = new ObservableDictionary<int, string>(_dictionary);

      using ( var numerator = ((ObservableDictionary<int, string>) _collection)?.GetEnumerator() )
      {
        ClassicAssert.NotNull(numerator);

        numerator.MoveNext();
        ClassicAssert.AreEqual(_dictionary.First(), numerator.Current);
      }
    }

    [Test]
    public void TestTryGetValue()
    {
      _collection = new ObservableDictionary<int, string>(_dictionary);
      ClassicAssert.IsTrue(_collection.TryGetValue(1, out string item));
      ClassicAssert.AreEqual(_dictionary[1], item);
    }

    [Test]
    public void TestRemoveItem()
    {
      _collection = new ObservableDictionary<int, string>(_dictionary);
      ClassicAssert.IsTrue(_collection.Remove(1));
      ClassicAssert.AreEqual(_dictionary.Count - 1, _collection.Count);

      ClassicAssert.IsTrue(_collection.Remove(_dictionary.Last()));
      ClassicAssert.AreEqual(_dictionary.Count - 2, _collection.Count);
    }

    [Test]
    public void TestContainsItem()
    {
      _collection = new ObservableDictionary<int, string>(_dictionary);
      ClassicAssert.IsTrue(_collection.ContainsKey(1));
      ClassicAssert.IsFalse(_collection.ContainsKey(4));
      ClassicAssert.IsTrue(_collection.Contains(_dictionary.First()));
      ClassicAssert.IsFalse(_collection.Contains(new KeyValuePair<int, string>(5, "test 5")));
    }

    [Test]
    public void TestClearsCollection()
    {
      _collection = new ObservableDictionary<int, string>(_dictionary);
      ClassicAssert.Greater(_collection.Count, 0);

      _collection.Clear();
      ClassicAssert.AreEqual(0, _collection.Count);
    }

    [Test]
    public void TestAddItemToCollection()
    {
      _collection = new ObservableDictionary<int, string>(_dictionary);
      ClassicAssert.Greater(_collection.Count, 2);

      ClassicAssert.DoesNotThrow(() => _collection.Add(5, "test 5"));
      ClassicAssert.IsTrue(_collection.ContainsKey(5));
      ClassicAssert.AreEqual(4, _collection.Count);

      ClassicAssert.DoesNotThrow(() => _collection.Add(new KeyValuePair<int, string>(6, "test 6")));
      ClassicAssert.IsTrue(_collection.Contains(new KeyValuePair<int, string>(6, "test 6")));
      ClassicAssert.IsFalse(_collection.Contains(new KeyValuePair<int, string>(7, "test 7")));
      ClassicAssert.That(() => _collection.Add(new KeyValuePair<int, string>(6, "test 6")), Throws.InstanceOf<ArgumentException>());
    }

    [Test]
    public void TestAddRangeToCollection()
    {
      _collection = new ObservableDictionary<int, string>(_dictionary);
      ClassicAssert.Greater(_collection.Count, 2);
      ClassicAssert.That(() => _collection.AddRange(null), Throws.InstanceOf<ArgumentException>());
      ClassicAssert.Throws<ArgumentException>(() => _collection.AddRange(
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
      ClassicAssert.DoesNotThrow(() => _collection.AddRange(newDictionary));
      ClassicAssert.Greater(_collection.Count, 3);
      ClassicAssert.IsTrue(_collection.ContainsKey(9));
      ClassicAssert.IsTrue(_collection.ContainsKey(7));
    }

    [Test]
    public void TestCopyCollectionTo()
    {
      _collection = new ObservableDictionary<int, string>(_dictionary);
      ClassicAssert.Greater(_collection.Count, 2);

      KeyValuePair<int, string>[] array = new KeyValuePair<int, string>[_collection.Count];
      ClassicAssert.DoesNotThrow(() => _collection.CopyTo(array, 0));
      ClassicAssert.AreEqual(3, array.Length);
      ClassicAssert.AreEqual(_dictionary.First(),  array[0]);
      ClassicAssert.Throws<ArgumentException>(() => _collection.CopyTo(null, 0));
    }
  }
}
