/********************************************************************
 *  FulcrumWeb RAD Framework - Fulcrum of your business             *
 *  Copyright (c) 2002-2010 FulcrumWeb, ALL RIGHTS RESERVED         *
 *                                                                  *
 *  THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      *
 *  FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        *
 *  COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       *
 *  AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  *
 *  AND PERMISSION FROM FULCRUMWEB. CONSULT THE END USER LICENSE    *
 *  AGREEMENT FOR INFORMATION ON ADDITIONAL RESTRICTIONS.           *
 ********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Framework.Utils
{
  public class IndexedDictionary<TKey, TValue> : 
    IDictionary<TKey, TValue>, 
    IList<TKey>,
    ICollection<KeyValuePair<TKey, TValue>>,
    IEnumerable<KeyValuePair<TKey, TValue>>, 
    IDictionary, 
    ICollection, 
    IEnumerable
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Dictionary enumerator.
    /// </summary>
    protected class Enumerator : 
      IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
    {
      //-----------------------------------------------------------------------
      protected int m_CurrentIndex = -1;
      protected IndexedDictionary<TKey, TValue> m_Dictionary = null;
      //-----------------------------------------------------------------------
      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="dictionary"></param>
      public Enumerator(IndexedDictionary<TKey, TValue> dictionary)
      {
        m_Dictionary = dictionary;
      }
      //-----------------------------------------------------------------------
      ///<summary>
      ///Gets the element in the collection at the current position of the enumerator.
      ///</summary>
      ///
      ///<returns>
      ///The element in the collection at the current position of the enumerator.
      ///</returns>
      ///
      public KeyValuePair<TKey, TValue> Current
      {
        get 
        {
          return new KeyValuePair<TKey, TValue>(m_Dictionary.m_Keys[m_CurrentIndex], m_Dictionary.m_Values[m_CurrentIndex]);
        }
      }
      //-----------------------------------------------------------------------
      ///<summary>
      ///Gets the current element in the collection.
      ///</summary>
      ///
      ///<returns>
      ///The current element in the collection.
      ///</returns>
      ///
      ///<exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element. </exception><filterpriority>2</filterpriority>
      object IEnumerator.Current
      {
        get
        {
          return new KeyValuePair<TKey, TValue>(m_Dictionary.m_Keys[m_CurrentIndex], m_Dictionary.m_Values[m_CurrentIndex]);
        }
      }
      //-----------------------------------------------------------------------
      ///<summary>
      ///Gets the key of the current dictionary entry.
      ///</summary>
      ///
      ///<returns>
      ///The key of the current element of the enumeration.
      ///</returns>
      ///
      ///<exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.IDictionaryEnumerator"></see> is positioned before the first entry of the dictionary or after the last entry. </exception><filterpriority>2</filterpriority>
      public object Key
      {
        get { return Current.Key; }
      }
      //-----------------------------------------------------------------------
      ///<summary>
      ///Gets the value of the current dictionary entry.
      ///</summary>
      ///
      ///<returns>
      ///The value of the current element of the enumeration.
      ///</returns>
      ///
      ///<exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.IDictionaryEnumerator"></see> is positioned before the first entry of the dictionary or after the last entry. </exception><filterpriority>2</filterpriority>
      public object Value
      {
        get { return Current.Value; }
      }
      //-----------------------------------------------------------------------
      ///<summary>
      ///Gets both the key and the value of the current dictionary entry.
      ///</summary>
      ///
      ///<returns>
      ///A <see cref="T:System.Collections.DictionaryEntry"></see> containing both the key and the value of the current dictionary entry.
      ///</returns>
      ///
      ///<exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.IDictionaryEnumerator"></see> is positioned before the first entry of the dictionary or after the last entry. </exception><filterpriority>2</filterpriority>
      public DictionaryEntry Entry
      {
        get { return new DictionaryEntry(Key, Value); }
      }
      //-----------------------------------------------------------------------
      ///<summary>
      ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
      ///</summary>
      ///<filterpriority>2</filterpriority>
      public void Dispose()
      {
        m_Dictionary = null;
      }
      //-----------------------------------------------------------------------
      ///<summary>
      ///Advances the enumerator to the next element of the collection.
      ///</summary>
      ///
      ///<returns>
      ///true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
      ///</returns>
      ///
      ///<exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception><filterpriority>2</filterpriority>
      public bool MoveNext()
      {
        m_CurrentIndex++;
        if (m_CurrentIndex >= m_Dictionary.m_Keys.Count)
        {
          m_CurrentIndex = -1;
        }
        return m_CurrentIndex >= 0;
      }
      //-----------------------------------------------------------------------
      ///<summary>
      ///Sets the enumerator to its initial position, which is before the first element in the collection.
      ///</summary>
      ///
      ///<exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception><filterpriority>2</filterpriority>
      public void Reset()
      {
        m_CurrentIndex = -1;
      }
      //-----------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    protected List<TKey> m_Keys = new List<TKey>();
    protected List<TValue> m_Values = new List<TValue>();
    protected Dictionary<TKey, int> m_KeyIndexes = new Dictionary<TKey, int>();
    protected ReadOnlyCollection<TKey> m_KeysCollection = null;
    protected ReadOnlyCollection<TValue> m_ValuesCollection = null;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public IndexedDictionary()
    {
      m_KeysCollection = new ReadOnlyCollection<TKey>(m_Keys);
      m_ValuesCollection = new ReadOnlyCollection<TValue>(m_Values);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public IndexedDictionary(IndexedDictionary<TKey, TValue> dictionary) : this()
    {
      m_Keys.AddRange(dictionary.m_Keys);
      m_Values.AddRange(dictionary.m_Values);
      for (int i = 0; i < m_Keys.Count; i++)
      {
        m_KeyIndexes[m_Keys[i]] = i;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds item to the dictionary.
    /// </summary>
    /// <param name="item">item to add</param>
    protected void InternalAdd(KeyValuePair<TKey, TValue> item)
    {
      lock (this)
      {
        m_Keys.Add(item.Key);
        m_Values.Add(item.Value);
        m_KeyIndexes.Add(item.Key, m_Keys.Count - 1);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Removes key and value from the dictionary.
    /// </summary>
    protected void InternalRemoveAt(int index)
    {
      lock (this)
      {
        m_KeyIndexes.Remove(m_Keys[index]);
        m_Keys.RemoveAt(index);
        m_Values.RemoveAt(index);
        for (int i = index; i < m_Keys.Count; i++)
        {
          m_KeyIndexes[m_Keys[i]] = i;
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Removes key and value from the dictionary.
    /// </summary>
    protected bool InternalRemove(TKey key)
    {
      lock (this)
      {
        int index;
        if (m_KeyIndexes.TryGetValue(key, out index))
        {
          InternalRemoveAt(index);
          return true;
        }
        return false;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Clears dictionary.
    /// </summary>
    protected void InternalClear()
    {
      lock (this)
      {
        m_Keys.Clear();
        m_Values.Clear();
        m_KeyIndexes.Clear();
      }
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Returns an enumerator that iterates through the collection.
    ///</summary>
    ///
    ///<returns>
    ///A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
    ///</returns>
    ///<filterpriority>1</filterpriority>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
      return new Enumerator(this);
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Returns an <see cref="T:System.Collections.IDictionaryEnumerator"></see> object for the <see cref="T:System.Collections.IDictionary"></see> object.
    ///</summary>
    ///
    ///<returns>
    ///An <see cref="T:System.Collections.IDictionaryEnumerator"></see> object for the <see cref="T:System.Collections.IDictionary"></see> object.
    ///</returns>
    ///<filterpriority>2</filterpriority>
    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
      return new Enumerator(this);
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Returns an enumerator that iterates through a collection.
    ///</summary>
    ///
    ///<returns>
    ///An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
    ///</returns>
    ///<filterpriority>2</filterpriority>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return new Enumerator(this);
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Returns an enumerator that iterates through the collection.
    ///</summary>
    ///
    ///<returns>
    ///A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
    ///</returns>
    ///<filterpriority>1</filterpriority>
    IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
    {
      return m_Keys.GetEnumerator();
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"></see>.
    ///</summary>
    ///
    ///<returns>
    ///The index of item if found in the list; otherwise, -1.
    ///</returns>
    ///
    ///<param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"></see>.</param>
    public int IndexOf(TKey item)
    {
      int index;
      if (m_KeyIndexes.TryGetValue(item, out index))
      {
        return index;
      }
      return -1;
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"></see> at the specified index.
    ///</summary>
    ///
    ///<param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"></see>.</param>
    ///<param name="index">The zero-based index at which item should be inserted.</param>
    ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"></see> is read-only.</exception>
    ///<exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"></see>.</exception>
    public void Insert(int index, TKey item)
    {
      throw new NotSupportedException();
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Removes the <see cref="T:System.Collections.Generic.IList`1"></see> item at the specified index.
    ///</summary>
    ///
    ///<param name="index">The zero-based index of the item to remove.</param>
    ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"></see> is read-only.</exception>
    ///<exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"></see>.</exception>
    public void RemoveAt(int index)
    {
      InternalRemoveAt(index);
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
    ///</summary>
    ///
    ///<returns>
    ///true if item was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false. This method also returns false if item is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"></see>.
    ///</returns>
    ///
    ///<param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
    ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
      if (Contains(item))
      {
        return InternalRemove(item.Key);
      }
      return false;
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"></see>.
    ///</summary>
    ///
    ///<returns>
    ///true if the element is successfully removed; otherwise, false.  This method also returns false if key was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"></see>.
    ///</returns>
    ///
    ///<param name="key">The key of the element to remove.</param>
    ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"></see> is read-only.</exception>
    ///<exception cref="T:System.ArgumentNullException">key is null.</exception>
    bool IDictionary<TKey, TValue>.Remove(TKey key)
    {
      return InternalRemove(key);
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
    ///</summary>
    ///
    ///<returns>
    ///true if item was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false. This method also returns false if item is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"></see>.
    ///</returns>
    ///
    ///<param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
    ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
    public bool Remove(TKey item)
    {
      return InternalRemove(item);
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Removes the element with the specified key from the <see cref="T:System.Collections.IDictionary"></see> object.
    ///</summary>
    ///
    ///<param name="key">The key of the element to remove. </param>
    ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IDictionary"></see> object is read-only.-or- The <see cref="T:System.Collections.IDictionary"></see> has a fixed size. </exception>
    ///<exception cref="T:System.ArgumentNullException">key is null. </exception><filterpriority>2</filterpriority>
    public void Remove(object key)
    {
      InternalRemove((TKey) key);
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Determines whether the <see cref="T:System.Collections.IDictionary"></see> object contains an element with the specified key.
    ///</summary>
    ///
    ///<returns>
    ///true if the <see cref="T:System.Collections.IDictionary"></see> contains an element with the key; otherwise, false.
    ///</returns>
    ///
    ///<param name="key">The key to locate in the <see cref="T:System.Collections.IDictionary"></see> object.</param>
    ///<exception cref="T:System.ArgumentNullException">key is null. </exception><filterpriority>2</filterpriority>
    bool IDictionary.Contains(object key)
    {
      return m_KeyIndexes.ContainsKey((TKey) key);
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> contains a specific value.
    ///</summary>
    ///
    ///<returns>
    ///true if item is found in the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false.
    ///</returns>
    ///
    ///<param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
    public bool Contains(TKey item)
    {
      return m_KeyIndexes.ContainsKey(item);
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"></see> contains an element with the specified key.
    ///</summary>
    ///
    ///<returns>
    ///true if the <see cref="T:System.Collections.Generic.IDictionary`2"></see> contains an element with the key; otherwise, false.
    ///</returns>
    ///
    ///<param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"></see>.</param>
    ///<exception cref="T:System.ArgumentNullException">key is null.</exception>
    public bool ContainsKey(TKey key)
    {
      return m_KeyIndexes.ContainsKey(key);
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> contains a specific value.
    ///</summary>
    ///
    ///<returns>
    ///true if item is found in the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false.
    ///</returns>
    ///
    ///<param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
      int index;
      if (m_KeyIndexes.TryGetValue(item.Key, out index))
      {
        return m_Values[index].Equals(item.Value);
      }
      return false;
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
    ///</summary>
    ///
    ///<param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
    ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
      InternalAdd(item);
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Adds an element with the provided key and value to the <see cref="T:System.Collections.IDictionary"></see> object.
    ///</summary>
    ///
    ///<param name="value">The <see cref="T:System.Object"></see> to use as the value of the element to add. </param>
    ///<param name="key">The <see cref="T:System.Object"></see> to use as the key of the element to add. </param>
    ///<exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.IDictionary"></see> object. </exception>
    ///<exception cref="T:System.ArgumentNullException">key is null. </exception>
    ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IDictionary"></see> is read-only.-or- The <see cref="T:System.Collections.IDictionary"></see> has a fixed size. </exception><filterpriority>2</filterpriority>
    public void Add(object key, object value)
    {
      InternalAdd(new KeyValuePair<TKey, TValue>((TKey)key, (TValue)value));
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"></see>.
    ///</summary>
    ///
    ///<param name="value">The object to use as the value of the element to add.</param>
    ///<param name="key">The object to use as the key of the element to add.</param>
    ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"></see> is read-only.</exception>
    ///<exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2"></see>.</exception>
    ///<exception cref="T:System.ArgumentNullException">key is null.</exception>
    public void Add(TKey key, TValue value)
    {
      InternalAdd(new KeyValuePair<TKey, TValue>(key, value));
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
    ///</summary>
    ///
    ///<param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
    ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
    public void Add(TKey item)
    {
      throw new NotSupportedException();
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
    ///</summary>
    ///
    ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only. </exception>
    void ICollection<TKey>.Clear()
    {
      InternalClear();
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
    ///</summary>
    ///
    ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only. </exception>
    void ICollection<KeyValuePair<TKey, TValue>>.Clear()
    {
      InternalClear();
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Removes all elements from the <see cref="T:System.Collections.IDictionary"></see> object.
    ///</summary>
    ///
    ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IDictionary"></see> object is read-only. </exception><filterpriority>2</filterpriority>
    public void Clear()
    {
      InternalClear();
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"></see> to an <see cref="T:System.Array"></see>, starting at a particular <see cref="T:System.Array"></see> index.
    ///</summary>
    ///
    ///<param name="array">The one-dimensional <see cref="T:System.Array"></see> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"></see>. The <see cref="T:System.Array"></see> must have zero-based indexing.</param>
    ///<param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    ///<exception cref="T:System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
    ///<exception cref="T:System.ArgumentNullException">array is null.</exception>
    ///<exception cref="T:System.ArgumentException">array is multidimensional.-or-arrayIndex is equal to or greater than the length of array.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"></see> is greater than the available space from arrayIndex to the end of the destination array.-or-Type T cannot be cast automatically to the type of the destination array.</exception>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      for (int i = arrayIndex; i < array.Length && i < m_Keys.Count; i++)
      {
        array[i] = new KeyValuePair<TKey, TValue>(m_Keys[i], m_Values[i]);
      }
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"></see> to an <see cref="T:System.Array"></see>, starting at a particular <see cref="T:System.Array"></see> index.
    ///</summary>
    ///
    ///<param name="array">The one-dimensional <see cref="T:System.Array"></see> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"></see>. The <see cref="T:System.Array"></see> must have zero-based indexing.</param>
    ///<param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    ///<exception cref="T:System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
    ///<exception cref="T:System.ArgumentNullException">array is null.</exception>
    ///<exception cref="T:System.ArgumentException">array is multidimensional.-or-arrayIndex is equal to or greater than the length of array.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"></see> is greater than the available space from arrayIndex to the end of the destination array.-or-Type T cannot be cast automatically to the type of the destination array.</exception>
    public void CopyTo(TKey[] array, int arrayIndex)
    {
      for (int i = arrayIndex; i < array.Length && i < m_Keys.Count; i++)
      {
        array[i] = m_Keys[i];
      }
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Copies the elements of the <see cref="T:System.Collections.ICollection"></see> to an <see cref="T:System.Array"></see>, starting at a particular <see cref="T:System.Array"></see> index.
    ///</summary>
    ///
    ///<param name="array">The one-dimensional <see cref="T:System.Array"></see> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"></see>. The <see cref="T:System.Array"></see> must have zero-based indexing. </param>
    ///<param name="index">The zero-based index in array at which copying begins. </param>
    ///<exception cref="T:System.ArgumentNullException">array is null. </exception>
    ///<exception cref="T:System.ArgumentOutOfRangeException">index is less than zero. </exception>
    ///<exception cref="T:System.ArgumentException">array is multidimensional.-or- index is equal to or greater than the length of array.-or- The number of elements in the source <see cref="T:System.Collections.ICollection"></see> is greater than the available space from index to the end of the destination array. </exception>
    ///<exception cref="T:System.InvalidCastException">The type of the source <see cref="T:System.Collections.ICollection"></see> cannot be cast automatically to the type of the destination array. </exception><filterpriority>2</filterpriority>
    public void CopyTo(Array array, int index)
    {
      for (int i = index; i < array.Length && i < m_Keys.Count; i++)
      {
        array.SetValue(m_Keys[i], i);
      }
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
    ///</summary>
    ///
    ///<returns>
    ///The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
    ///</returns>
    ///
    public int Count
    {
      get { return m_Keys.Count; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Tries to return dictionary value.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue(TKey key, out TValue value)
    {
      int index;
      if (m_KeyIndexes.TryGetValue(key, out index))
      {
        value = m_Values[index];
        return true;
      }
      value = default(TValue);
      return false;
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Gets or sets the element at the specified index.
    ///</summary>
    ///
    ///<returns>
    ///The element at the specified index.
    ///</returns>
    ///
    ///<param name="index">The zero-based index of the element to get or set.</param>
    ///<exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"></see>.</exception>
    ///<exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1"></see> is read-only.</exception>
    public TKey this[int index]
    {
      get { return m_Keys[index]; }
      set { throw new NotSupportedException(); }
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Gets or sets the element with the specified key.
    ///</summary>
    ///
    ///<returns>
    ///The element with the specified key.
    ///</returns>
    ///
    ///<param name="key">The key of the element to get or set.</param>
    ///<exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IDictionary`2"></see> is read-only.</exception>
    ///<exception cref="T:System.ArgumentNullException">key is null.</exception>
    ///<exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and key is not found.</exception>
    public TValue this[TKey key]
    {
      get 
      { 
        return m_Values[m_KeyIndexes[key]]; 
      }
      set 
      {
        int index;
        if (m_KeyIndexes.TryGetValue(key, out index))
        {
          m_Values[index] = value;
        }
        else
        {
          Add(key, value);
        }
      }
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Gets or sets the element with the specified key.
    ///</summary>
    ///
    ///<returns>
    ///The element with the specified key.
    ///</returns>
    ///
    ///<param name="key">The key of the element to get or set. </param>
    ///<exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.IDictionary"></see> object is read-only.-or- The property is set, key does not exist in the collection, and the <see cref="T:System.Collections.IDictionary"></see> has a fixed size. </exception>
    ///<exception cref="T:System.ArgumentNullException">key is null. </exception><filterpriority>2</filterpriority>
    public object this[object key]
    {
      get { return this[(TKey) key]; }
      set { this[(TKey) key] = (TValue) value; }
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Gets an <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"></see>.
    ///</summary>
    ///
    ///<returns>
    ///An <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"></see>.
    ///</returns>
    ///
    public ICollection<TKey> Keys
    {
      get { return m_KeysCollection; }
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Gets an <see cref="T:System.Collections.ICollection"></see> object containing the keys of the <see cref="T:System.Collections.IDictionary"></see> object.
    ///</summary>
    ///
    ///<returns>
    ///An <see cref="T:System.Collections.ICollection"></see> object containing the keys of the <see cref="T:System.Collections.IDictionary"></see> object.
    ///</returns>
    ///<filterpriority>2</filterpriority>
    ICollection IDictionary.Keys
    {
      get { return m_KeysCollection; }
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Gets an <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"></see>.
    ///</summary>
    ///
    ///<returns>
    ///An <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"></see>.
    ///</returns>
    ///
    public ICollection<TValue> Values
    {
      get { return m_ValuesCollection; }
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Gets an <see cref="T:System.Collections.ICollection"></see> object containing the values in the <see cref="T:System.Collections.IDictionary"></see> object.
    ///</summary>
    ///
    ///<returns>
    ///An <see cref="T:System.Collections.ICollection"></see> object containing the values in the <see cref="T:System.Collections.IDictionary"></see> object.
    ///</returns>
    ///<filterpriority>2</filterpriority>
    ICollection IDictionary.Values
    {
      get { return m_ValuesCollection; }
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Gets a value indicating whether the <see cref="T:System.Collections.IDictionary"></see> object is read-only.
    ///</summary>
    ///
    ///<returns>
    ///true if the <see cref="T:System.Collections.IDictionary"></see> object is read-only; otherwise, false.
    ///</returns>
    ///<filterpriority>2</filterpriority>
    public bool IsReadOnly
    {
      get { return false; }
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Gets a value indicating whether the <see cref="T:System.Collections.IDictionary"></see> object has a fixed size.
    ///</summary>
    ///
    ///<returns>
    ///true if the <see cref="T:System.Collections.IDictionary"></see> object has a fixed size; otherwise, false.
    ///</returns>
    ///<filterpriority>2</filterpriority>
    public bool IsFixedSize
    {
      get { return false; }
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"></see>.
    ///</summary>
    ///
    ///<returns>
    ///An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"></see>.
    ///</returns>
    ///<filterpriority>2</filterpriority>
    public object SyncRoot
    {
      get { return this; }
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"></see> is synchronized (thread safe).
    ///</summary>
    ///
    ///<returns>
    ///true if access to the <see cref="T:System.Collections.ICollection"></see> is synchronized (thread safe); otherwise, false.
    ///</returns>
    ///<filterpriority>2</filterpriority>
    public bool IsSynchronized
    {
      get { return false; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns value by the given index.
    /// </summary>
    public TValue ValueByIndex(int index)
    {
      return m_Values[index];
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns index of the given value.
    /// </summary>
    public int IndexOfValue(TValue value)
    {
      return m_Values.IndexOf(value);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns array of keys.
    /// </summary>
    public TKey[] KeysArray()
    { 
      return m_Keys.ToArray();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns array of values.
    /// </summary>
    public TValue[] ValuesArray()
    { 
      return m_Values.ToArray();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns first key value or null if dictionary is empty.
    /// </summary>
    public TKey FirstKey
    { get { return m_Keys.Count > 0 ? m_Keys[0] : default(TKey); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns last key value or null if dictionary is empty.
    /// </summary>
    public TKey LastKey
    { get { return m_Keys.Count > 0 ? m_Keys[m_Keys.Count - 1] : default(TKey); } }
    //-------------------------------------------------------------------------
  }
}