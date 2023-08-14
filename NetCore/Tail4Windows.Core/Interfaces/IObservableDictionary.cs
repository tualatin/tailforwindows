namespace Org.Vs.TailForWin.Core.Interfaces
{
  /// <summary>
  /// ObservableDictionary interface
  /// </summary>
  public interface IObservableDictionary<TKey, TValue>
  {
    /// <summary>
    /// Collection of keys
    /// </summary>
    ICollection<TKey> Keys
    {
      get;
    }

    /// <summary>
    /// Collection of values
    /// </summary>
    ICollection<TValue> Values
    {
      get;
    }

    /// <summary>
    /// Returns the number of Dictionary size
    /// </summary>
    int Count
    {
      get;
    }

    /// <summary>
    /// Returns the read only flag of the Dictionary
    /// </summary>
    bool IsReadOnly
    {
      get;
    }

    /// <summary>
    /// Get value by key index
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns>The corresponding value of given key</returns>
    TValue this[TKey key]
    {
      get;
      set;
    }

    /// <summary>
    /// Adds the specified key and value to the dictionary.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add. The value can be <c>null</c> for reference types.</param>
    void Add(TKey key, TValue value);

    /// <summary>
    /// Adds the specified key and value to the dictionary.
    /// </summary>
    /// <param name="item">The item of the element to add.</param>
    void Add(KeyValuePair<TKey, TValue> item);

    /// <summary>
    /// Add items to Dictionary
    /// </summary>
    /// <param name="items">Items to add</param>
    void AddRange(IDictionary<TKey, TValue> items);

    /// <summary>Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2" />.</summary>
    /// <param name="key">The key of the element to remove.</param>
    /// <returns>
    /// <see langword="true" /> if the element is successfully removed; otherwise, <see langword="false" />.  This method also returns <see langword="false" /> if <paramref name="key" /> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="key" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.</exception>
    bool Remove(TKey key);

    /// <summary>Removes the element with the specified KeyValuePair from the <see cref="T:System.Collections.Generic.IDictionary`2" />.</summary>
    /// <param name="item">The KeyValuePair to remove.</param>
    /// <returns>
    /// <see langword="true" /> if the element is successfully removed; otherwise, <see langword="false" />.  This method also returns <see langword="false" /> if <paramref name="item" /> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="item" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.</exception>
    bool Remove(KeyValuePair<TKey, TValue> item);

    /// <summary>
    /// Clears the Dictionary
    /// </summary>
    void Clear();

    /// <summary>
    /// Determines whether the Dictionary&lt;TKey, TValue&gt; contains the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the Dictionary&lt;TKey, TValue&gt;.</param>
    /// <returns><c>true</c> if the Dictionary&lt;TKey, TValue&gt; contains an element with the specified key; otherwise, <c>false</c>.</returns>
    bool ContainsKey(TKey key);

    /// <summary>
    /// Determines wheter the Dictionary&lt;TKey, TValue&gt; contains a specified KeyValue pair
    /// </summary>
    /// <param name="item">KeyValue pair</param>
    /// <returns></returns>
    bool Contains(KeyValuePair<TKey, TValue> item);

    /// <summary>
    /// Copies the elements of the ICollection to an Array, starting at a particular Array index
    /// </summary>
    /// <param name="array">The one-dimensional Array that is the destination of the elements copied from ICollection. The Array must have zero-based indexing.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex);

    /// <summary>
    /// Gets value associated with the specified key
    /// </summary>
    /// <param name="key">The key whose value to get</param>
    /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter.
    /// This parameter is passed uninitialized.</param>
    /// <returns><see langword="true" /> if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key; otherwise, <see langword="false" />.</returns>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is <see langword="null" />.</exception>
    bool TryGetValue(TKey key, out TValue value);
  }
}
