using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using Org.Vs.Tail4Win.Shared.Interfaces;
using Org.Vs.Tail4Win.Shared.Utils;

namespace Org.Vs.Tail4Win.Shared.Collections
{
  /// <summary>
  /// ObservableDictionary
  /// </summary>
  public sealed class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged, IObservableDictionary<TKey, TValue>
  {
    private const string CountString = "Count";
    private const string IndexerName = "Item[]";
    private const string KeysName = "Keys";
    private const string ValuesName = "Values";

    private IDictionary<TKey, TValue> _dictionary;

    /// <summary>
    /// Get value by key index
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns>The corresponding value of given key</returns>
    /// <exception cref="ArgumentException">When add a item with a key, that is already exists</exception>
    public TValue this[TKey key]
    {
      get => _dictionary[key];
      set => Insert(key, value, false);
    }

    /// <summary>
    /// Collection of keys
    /// </summary>
    public ICollection<TKey> Keys => _dictionary.Keys;

    /// <summary>
    /// Collection of values
    /// </summary>
    public ICollection<TValue> Values => _dictionary.Values;

    /// <summary>
    /// Returns the number of Dictionary size
    /// </summary>
    public int Count => _dictionary.Count;

    /// <summary>
    /// Returns the read only flag of the Dictionary
    /// </summary>
    public bool IsReadOnly => _dictionary.IsReadOnly;

    /// <summary>
    /// Standard constructor
    /// </summary>
    public ObservableDictionary() => _dictionary = new Dictionary<TKey, TValue>();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dictionary">An existing dictionary</param>
    public ObservableDictionary(IDictionary<TKey, TValue> dictionary) => _dictionary = new Dictionary<TKey, TValue>(dictionary);

    /// <summary>
    /// Occurs when a property value changes
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Occurs when the collection changes
    /// </summary>
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    /// <summary>
    /// Adds the specified key and value to the dictionary.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add. The value can be <c>null</c> for reference types.</param>
    public void Add(TKey key, TValue value) => Insert(key, value, true);

    /// <summary>
    /// Adds the specified key and value to the dictionary.
    /// </summary>
    /// <param name="item">The item of the element to add.</param>
    public void Add(KeyValuePair<TKey, TValue> item) => Insert(item.Key, item.Value, true);

    /// <summary>
    /// Add items to Dictionary
    /// </summary>
    /// <param name="items">Items to add</param>
    /// <exception cref="ArgumentException">If items are null or an item with the same key has already been added</exception>
    public void AddRange(IDictionary<TKey, TValue> items)
    {
      Arg.NotNull(items, nameof(items));

      if ( items.Count == 0 )
        return;

      if ( _dictionary.Count > 0 )
      {
        if ( items.Any(p => _dictionary.ContainsKey(p.Key)) )
          throw new ArgumentException("An item with the same key has already been added.");

        foreach ( var item in items )
        {
          _dictionary.Add(item.Key, item.Value);
        }
      }
      else
      {
        _dictionary = new Dictionary<TKey, TValue>(items);
      }

      OnCollectionChanged(NotifyCollectionChangedAction.Add, items.ToList());
    }

    /// <summary>
    /// Clears the Dictionary
    /// </summary>
    public void Clear()
    {
      if ( _dictionary.Count == 0 )
        return;

      _dictionary.Clear();
      OnCollectionChanged();
    }

    /// <summary>
    /// Determines whether the Dictionary&lt;TKey, TValue&gt; contains a specified KeyValue pair
    /// </summary>
    /// <param name="item">KeyValue pair</param>
    /// <returns></returns>
    public bool Contains(KeyValuePair<TKey, TValue> item) => _dictionary.Contains(item);

    /// <summary>
    /// Determines whether the Dictionary&lt;TKey, TValue&gt; contains the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the Dictionary&lt;TKey, TValue&gt;.</param>
    /// <returns><c>true</c> if the Dictionary&lt;TKey, TValue&gt; contains an element with the specified key; otherwise, <c>false</c>.</returns>
    public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

    /// <summary>
    /// Copies the elements of the ICollection to an Array, starting at a particular Array index
    /// </summary>
    /// <param name="array">The one-dimensional Array that is the destination of the elements copied from ICollection. The Array must have zero-based indexing.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    /// <exception cref="ArgumentException">If array is null</exception>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      Arg.NotNull(array, nameof(array));
      _dictionary.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection
    /// </summary>
    /// <returns>An <c>IEnumerator</c> object that can be used to iterate through the collection</returns>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();

    /// <summary>Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2" />.</summary>
    /// <param name="key">The key of the element to remove.</param>
    /// <returns>
    /// <see langword="true" /> if the element is successfully removed; otherwise, <see langword="false" />.  This method also returns <see langword="false" /> if <paramref name="key" /> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="key" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.</exception>
    public bool Remove(TKey key)
    {
      _dictionary.TryGetValue(key, out var value);
      bool removed = _dictionary.Remove(key);

      if ( removed )
        OnCollectionChanged(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, value));

      return removed;
    }

    /// <summary>Removes the element with the specified KeyValuePair from the <see cref="T:System.Collections.Generic.IDictionary`2" />.</summary>
    /// <param name="item">The KeyValuePair to remove.</param>
    /// <returns>
    /// <see langword="true" /> if the element is successfully removed; otherwise, <see langword="false" />.  This method also returns <see langword="false" /> if <paramref name="item" /> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="item" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.</exception>
    public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

    /// <summary>
    /// Gets value associated with the specified key
    /// </summary>
    /// <param name="key">The key whose value to get</param>
    /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter.
    /// This parameter is passed uninitialized.</param>
    /// <returns><see langword="true" /> if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key; otherwise, <see langword="false" />.</returns>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is <see langword="null" />.</exception>
    public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _dictionary).GetEnumerator();

    private void Insert(TKey key, TValue value, bool add)
    {
      if ( _dictionary.TryGetValue(key, out var item) )
      {
        if ( add )
          throw new ArgumentException("An item with same key has already been added");

        // Value already exists, nothing to do
        if ( Equals(item, value) )
          return;

        _dictionary[key] = value;
        OnCollectionChanged(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, value));
      }
      else
      {
        _dictionary[key] = value;
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

    private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

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

    //private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem)
    //{
    //  OnPropertyChanged();
    //  CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, newItem, oldItem));
    //}

    private void OnCollectionChanged(NotifyCollectionChangedAction action, IList newItems)
    {
      OnPropertyChanged();
      CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, newItems));
    }
  }
}
