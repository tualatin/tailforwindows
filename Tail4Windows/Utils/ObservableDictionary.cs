using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;


namespace Org.Vs.TailForWin.Utils
{
  /// <summary>
  /// ObservableDictionary
  /// </summary>
  /// <typeparam name="TKey">INotifyCollectionChanged</typeparam>
  /// <typeparam name="TValue">INotifyPropertyChanged</typeparam>
  public sealed class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
  {
    private const string CountString = "Count";
    private const string IndexerName = "Item[]";
    private const string KeysName = "Keys";
    private const string ValuesName = "Values";

    /// <summary>
    /// Dictionary
    /// </summary>
    private IDictionary<TKey, TValue> Dictionary
    {
      get;
      set;
    }


    #region Constructors

    public ObservableDictionary()
    {
      Dictionary = new Dictionary<TKey, TValue>();
    }

    public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
    {
      Dictionary = new Dictionary<TKey, TValue>(dictionary);
    }

    public ObservableDictionary(IEqualityComparer<TKey> comparer)
    {
      Dictionary = new Dictionary<TKey, TValue>(comparer);
    }

    public ObservableDictionary(int capacity)
    {
      Dictionary = new Dictionary<TKey, TValue>(capacity);
    }

    public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
    {
      Dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
    }

    public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
    {
      Dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
    }

    #endregion

    #region IDictionary<TKey,TValue> Members

    /// <summary>
    /// Adds the specified key and value to the dictionary.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add. The value can be <c>null</c> for reference types.</param>
    public void Add(TKey key, TValue value)
    {
      Insert(key, value, true);
    }

    /// <summary>
    /// Determines whether the Dictionary&lt;TKey, TValue&gt; contains the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the Dictionary&lt;TKey, TValue&gt;.</param>
    /// <returns><c>true</c> if the Dictionary&lt;TKey, TValue&gt; contains an element with the specified key; otherwise, <c>false</c>.</returns>
    public bool ContainsKey(TKey key)
    {
      return (Dictionary.ContainsKey(key));
    }

    public ICollection<TKey> Keys => (Dictionary.Keys);

    /// <summary>
    /// Remove key from Dictionary
    /// </summary>
    /// <param name="key">Key to remove</param>
    /// <returns>If success true otherwise false</returns>
    /// <exception cref="ArgumentException">If key is null</exception>
    public bool Remove(TKey key)
    {
      Arg.NotNull(key, "Key");

      TValue value;
      Dictionary.TryGetValue(key, out value);
      var removed = Dictionary.Remove(key);

      if(removed)
        //OnCollectionChanged(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, value));
        OnCollectionChanged();

      return (removed);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
      return (Dictionary.TryGetValue(key, out value));
    }

    public ICollection<TValue> Values => (Dictionary.Values);

    public TValue this[TKey key]
    {
      get => (Dictionary[key]);
      set => Insert(key, value, false);
    }

    #endregion

    #region ICollection<KeyValuePair<TKey,TValue>> Members

    /// <summary>
    /// Adds the specified key and value to the dictionary.
    /// </summary>
    /// <param name="item">The item of the element to add.</param>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
      Insert(item.Key, item.Value, true);
    }

    public void Clear()
    {
      if(Dictionary.Count <= 0)
        return;

      Dictionary.Clear();
      OnCollectionChanged();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
      return (Dictionary.Contains(item));
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      Dictionary.CopyTo(array, arrayIndex);
    }

    public int Count => (Dictionary.Count);

    public bool IsReadOnly => (Dictionary.IsReadOnly);

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
      return (Remove(item.Key));
    }

    #endregion

    #region IEnumerable<KeyValuePair<TKey,TValue>> Members

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
      return (Dictionary.GetEnumerator());
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (((IEnumerable) Dictionary).GetEnumerator());
    }

    #endregion

    #region INotifyCollectionChanged Members

    public event NotifyCollectionChangedEventHandler CollectionChanged;

    #endregion

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    /// <summary>
    /// Add items to Dictionary
    /// </summary>
    /// <param name="items">Items to add</param>
    /// <exception cref="ArgumentException">If items is null</exception>
    public void AddRange(IDictionary<TKey, TValue> items)
    {
      Arg.NotNull(items, "Items");

      if(items.Count <= 0)
        return;

      if(Dictionary.Count > 0)
      {
        if(items.Keys.Any(k => Dictionary.ContainsKey(k)))
        {
          throw new ArgumentException("An item with the same key has already been added.");
        }

        foreach(var item in items)
        {
          Dictionary.Add(item);
        }
      }
      else
      {
        Dictionary = new Dictionary<TKey, TValue>(items);
      }

      OnCollectionChanged(NotifyCollectionChangedAction.Add, items.ToArray());
    }

    private void Insert(TKey key, TValue value, bool add)
    {
      Arg.NotNull(key, "Key");

      if(Dictionary.TryGetValue(key, out TValue item))
      {
        if(add)
          throw new ArgumentException("An item with the same key has already been added.");
        if(Equals(item, value))
          return;

        Dictionary[key] = value;

        OnCollectionChanged(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, value), new KeyValuePair<TKey, TValue>(key, item));
      }
      else
      {
        Dictionary[key] = value;

        OnCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value));
      }
    }

    private void OnPropertyChanged()
    {
      OnPropertyChanged(CountString);
      OnPropertyChanged(IndexerName);
      OnPropertyChanged(KeysName);
      OnPropertyChanged(ValuesName);
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    /// <param name="propertyName">Name of property</param>
    private void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void OnCollectionChanged()
    {
      OnPropertyChanged();
      CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> changedItem)
    {
      OnPropertyChanged();
      CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, changedItem));
    }

    private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem)
    {
      OnPropertyChanged();
      CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, newItem, oldItem));
    }

    private void OnCollectionChanged(NotifyCollectionChangedAction action, IList newItems)
    {
      OnPropertyChanged();
      CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, newItems));
    }
  }
}
