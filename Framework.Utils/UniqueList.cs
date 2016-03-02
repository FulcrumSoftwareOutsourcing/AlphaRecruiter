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
using System.Text;

namespace Framework.Utils
{
  /// <summary>
  /// List of unique items.
  /// </summary>
  /// <typeparam name="T">type of item</typeparam>
  public class UniqueList<T> : 
    IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable
  {
    //-------------------------------------------------------------------------
    protected List<T> m_List = null;
    protected Dictionary<T, int> m_Dictionary = null;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public UniqueList()
    {
      m_List = new List<T>();
      m_Dictionary = new Dictionary<T, int>();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public UniqueList(IEqualityComparer<T> comparer)
    {
      m_List = new List<T>();
      m_Dictionary = new Dictionary<T, int>(comparer);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public UniqueList(IEnumerable<T> collection) : this()
    {
      AddRange(collection);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public UniqueList(IEnumerable<T> collection, IEqualityComparer<T> comparer) : this(comparer)
    {
      AddRange(collection);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds item to the unique list.
    /// </summary>
    protected int InternalAdd(T value)
    {
      int index = -1;
      if (value != null && !m_Dictionary.ContainsKey(value))
      {
        lock (this)
        {
          m_List.Add(value);
          index = m_List.Count - 1;
          m_Dictionary.Add(value, index);
        }
      }
      return index;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Inserts item to the unique list.
    /// </summary>
    protected void InternalInsert(int index, T value)
    {
      if (value != null && !m_Dictionary.ContainsKey(value))
      {
        lock (this)
        {
          m_List.Insert(index, value);
          m_Dictionary.Add(value, index);
          for (int i = index + 1; i < m_List.Count; i++)
          {
            m_Dictionary[m_List[i]] = i;
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets value at the specified index.
    /// </summary>
    protected void InternalSetValue(int index, T value)
    {
      if (value == null)
      {
        throw new ArgumentNullException("value");
      }
      lock (this)
      {
        m_Dictionary.Remove(m_List[index]);
        m_List[index] = value;
        m_Dictionary.Add(value, index);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Removes value at the specified index.
    /// </summary>
    protected void InternalRemoveAt(int index)
    {
      lock (this)
      {
        m_Dictionary.Remove(m_List[index]);
        m_List.RemoveAt(index);
        for (int i = index; i < m_List.Count; i++)
        {
          m_Dictionary[m_List[i]] = i;
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Clears list.
    /// </summary>
    protected void InternalClear()
    {
      lock (this)
      {
        m_List.Clear();
        m_Dictionary.Clear();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns index of the specified value.
    /// </summary>
    protected int InternalIndexOf(T value)
    {
      int index;
      if (value != null && m_Dictionary.TryGetValue(value, out index))
      {
        return index;
      }
      return -1;
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
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return m_List.GetEnumerator();
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
    public IEnumerator GetEnumerator()
    {
      return m_List.GetEnumerator();
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
    ///Does not accept nulls.
    ///</summary>
    ///
    ///<param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
    ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
    public void Add(T item)
    {
      InternalAdd(item);
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Adds an item to the <see cref="T:System.Collections.IList"></see>.
    ///</summary>
    ///
    ///<returns>
    ///The position into which the new element was inserted.
    ///</returns>
    ///
    ///<param name="value">The <see cref="T:System.Object"></see> to add to the <see cref="T:System.Collections.IList"></see>. </param>
    ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only.-or- The <see cref="T:System.Collections.IList"></see> has a fixed size. </exception><filterpriority>2</filterpriority>
    int IList.Add(object value)
    {
      return InternalAdd((T) value);
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
    public bool Contains(T item)
    {
      return InternalIndexOf(item) >= 0;
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Determines whether the <see cref="T:System.Collections.IList"></see> contains a specific value.
    ///</summary>
    ///
    ///<returns>
    ///true if the <see cref="T:System.Object"></see> is found in the <see cref="T:System.Collections.IList"></see>; otherwise, false.
    ///</returns>
    ///
    ///<param name="value">The <see cref="T:System.Object"></see> to locate in the <see cref="T:System.Collections.IList"></see>. </param><filterpriority>2</filterpriority>
    bool IList.Contains(object value)
    {
      return InternalIndexOf((T) value) >= 0;
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Removes all items from the <see cref="T:System.Collections.IList"></see>.
    ///</summary>
    ///
    ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only. </exception><filterpriority>2</filterpriority>
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
    public void CopyTo(T[] array, int arrayIndex)
    {
      m_List.CopyTo(array, arrayIndex);
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
    void ICollection.CopyTo(Array array, int index)
    {
      m_List.CopyTo((T[]) array, index);
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Removes the <see cref="T:System.Collections.IList"></see> item at the specified index.
    ///</summary>
    ///
    ///<param name="index">The zero-based index of the item to remove. </param>
    ///<exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.IList"></see>. </exception>
    ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only.-or- The <see cref="T:System.Collections.IList"></see> has a fixed size. </exception><filterpriority>2</filterpriority>
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
    public bool Remove(T item)
    {
      int index = InternalIndexOf(item);
      if (index >= 0)
      {
        InternalRemoveAt(index);
        return true;
      }
      return false;
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList"></see>.
    ///</summary>
    ///
    ///<param name="value">The <see cref="T:System.Object"></see> to remove from the <see cref="T:System.Collections.IList"></see>. </param>
    ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only.-or- The <see cref="T:System.Collections.IList"></see> has a fixed size. </exception><filterpriority>2</filterpriority>
    void IList.Remove(object value)
    {
      Remove((T) value);
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
    public int IndexOf(T item)
    {
      return InternalIndexOf(item);
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Determines the index of a specific item in the <see cref="T:System.Collections.IList"></see>.
    ///</summary>
    ///
    ///<returns>
    ///The index of value if found in the list; otherwise, -1.
    ///</returns>
    ///
    ///<param name="value">The <see cref="T:System.Object"></see> to locate in the <see cref="T:System.Collections.IList"></see>. </param><filterpriority>2</filterpriority>
    int IList.IndexOf(object value)
    {
      return InternalIndexOf((T) value);
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
    public void Insert(int index, T item)
    {
      InternalInsert(index, item);
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Inserts an item to the <see cref="T:System.Collections.IList"></see> at the specified index.
    ///</summary>
    ///
    ///<param name="value">The <see cref="T:System.Object"></see> to insert into the <see cref="T:System.Collections.IList"></see>. </param>
    ///<param name="index">The zero-based index at which value should be inserted. </param>
    ///<exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.IList"></see>. </exception>
    ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only.-or- The <see cref="T:System.Collections.IList"></see> has a fixed size. </exception>
    ///<exception cref="T:System.NullReferenceException">value is null reference in the <see cref="T:System.Collections.IList"></see>.</exception><filterpriority>2</filterpriority>
    void IList.Insert(int index, object value)
    {
      InternalInsert(index, (T) value);
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
    public T this[int index]
    { 
      get { return m_List[index]; } 
      set { InternalSetValue(index, value); } 
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
    ///<param name="index">The zero-based index of the element to get or set. </param>
    ///<exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.IList"></see>. </exception>
    ///<exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.IList"></see> is read-only. </exception><filterpriority>2</filterpriority>
    object IList.this[int index]
    { 
      get { return m_List[index]; } 
      set { InternalSetValue(index, (T) value); } 
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Gets a value indicating whether the <see cref="T:System.Collections.IList"></see> is read-only.
    ///</summary>
    ///
    ///<returns>
    ///true if the <see cref="T:System.Collections.IList"></see> is read-only; otherwise, false.
    ///</returns>
    ///<filterpriority>2</filterpriority>
    public bool IsReadOnly
    {
      get { return false; }
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Gets a value indicating whether the <see cref="T:System.Collections.IList"></see> has a fixed size.
    ///</summary>
    ///
    ///<returns>
    ///true if the <see cref="T:System.Collections.IList"></see> has a fixed size; otherwise, false.
    ///</returns>
    ///<filterpriority>2</filterpriority>
    public bool IsFixedSize
    {
      get { return false; }
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Gets the number of elements contained in the <see cref="T:System.Collections.ICollection"></see>.
    ///</summary>
    ///
    ///<returns>
    ///The number of elements contained in the <see cref="T:System.Collections.ICollection"></see>.
    ///</returns>
    ///<filterpriority>2</filterpriority>
    public int Count
    {
      get { return m_List.Count; }
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
    /// Returns copy of the list.
    /// </summary>
    public List<T> ToList()
    {
      return new List<T>(m_List);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns array copy of the list.
    /// </summary>
    public T[] ToArray()
    {
      return m_List.ToArray();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds range of items to the list.
    /// </summary>
    public void AddRange(IEnumerable<T> collection)
    {
      if (collection != null)
      {
        foreach (T item in collection)
        {
          Add(item);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts given list of objects to the typed list.
    /// </summary>
    /// <param name="list">list to convert</param>
    /// <returns>converted list or empty list</returns>
    static public UniqueList<T> FromList(IList list)
    {
      UniqueList<T> result = new UniqueList<T>();
      if (list != null)
      {
        foreach (object obj in list)
        {
          if (obj is T)
          {
            result.Add((T) obj);
          }
        }
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns first list value.
    /// </summary>
    public T FirstValue
    { get { return m_List.Count > 0 ? m_List[0] : default(T); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns last list value.
    /// </summary>
    public T LastValue
    { get { return m_List.Count > 0 ? m_List[m_List.Count - 1] : default(T); } }
    //-------------------------------------------------------------------------
  }
}