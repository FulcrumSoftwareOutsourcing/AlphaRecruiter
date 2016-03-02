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
using System.Collections.Specialized;

namespace Framework.Utils
{
  /// <summary>
  /// Utility methods to work with lists, arrays, hashtables, etc.
  /// </summary>
  public class CxList
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds list for objects with the string representation equal to the given name.
    /// </summary>
    /// <param name="list">list to search in</param>
    /// <param name="text">string representation to look for</param>
    /// <param name="comparisonType">string comparison method</param>
    /// <returns>the string representation equal to the given name or null if not found</returns>
    static public T FindByText<T>(
      IEnumerable<T> list, 
      string text, 
      StringComparison comparisonType)
    {
      if (list != null && text != null)
      {
        foreach (object obj in list)
        {
          if (obj != null && obj.ToString().Equals(text, comparisonType))
          {
            return (T) obj;
          }
        }
      }
      return default(T);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds list for objects with the string representation equal to the given name.
    /// </summary>
    /// <param name="list">list to search in</param>
    /// <param name="text">string representation to look for</param>
    /// <returns>the string representation equal to the given name or null if not found</returns>
    static public T FindByText<T>(
      IEnumerable<T> list,
      string text)
    {
      return FindByText(list, text, StringComparison.Ordinal);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds list for objects with the string representation equal to the given name.
    /// Case-insensitive.
    /// </summary>
    /// <param name="list">list to search in</param>
    /// <param name="text">string representation to look for</param>
    /// <returns>the string representation equal to the given name or null if not found</returns>
    static public T FindByTextCI<T>(
      IEnumerable<T> list,
      string text)
    {
      return FindByText(list, text, StringComparison.OrdinalIgnoreCase);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds list for objects with the string representation equal to the given name.
    /// </summary>
    /// <param name="list">list to search in</param>
    /// <param name="text">string representation to look for</param>
    /// <param name="comparisonType">string comparison method</param>
    /// <returns>the string representation equal to the given name or null if not found</returns>
    static public object FindByText(
      IEnumerable list,
      string text,
      StringComparison comparisonType)
    {
      if (list != null && text != null)
      {
        foreach (object obj in list)
        {
          if (obj != null && obj.ToString().Equals(text, comparisonType))
          {
            return obj;
          }
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds list for objects with the string representation equal to the given name.
    /// </summary>
    /// <param name="list">list to search in</param>
    /// <param name="text">string representation to look for</param>
    /// <returns>the string representation equal to the given name or null if not found</returns>
    static public object FindByText(
      IEnumerable list,
      string text)
    {
      return FindByText(list, text, StringComparison.Ordinal);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds list for objects with the string representation equal to the given name.
    /// Case-insensitive.
    /// </summary>
    /// <param name="list">list to search in</param>
    /// <param name="text">string representation to look for</param>
    /// <returns>the string representation equal to the given name or null if not found</returns>
    static public object FindByTextCI(
      IEnumerable list,
      string text)
    {
      return FindByText(list, text, StringComparison.OrdinalIgnoreCase);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Reverts list.
    /// </summary>
    /// <param name="list">list to revert</param>
    /// <returns>reverted list</returns>
    static public IList<T> Reverse<T>(IList<T> list)
    {
      if (list != null)
      {
        List<T> result = new List<T>();
        for (int i = list.Count - 1; i >= 0; i--)
        {
          result.Add(list[i]);
        }
        return result;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Reverts list.
    /// </summary>
    /// <param name="list">list to revert</param>
    /// <returns>reverted list</returns>
    static public IList Reverse(IList list)
    {
      if (list != null)
      {
        ArrayList result = new ArrayList(list.Count);
        for (int i = list.Count - 1; i >= 0; i--)
        {
          result.Add(list[i]);
        }
        return result;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list that contains all not null objects from the array.
    /// </summary>
    /// <param name="arr">source array</param>
    /// <returns>list that contains all not null objects from the array</returns>
    static public IList<T> ArrayToList<T>(T[] arr) where T : class
    {
      List<T> list = new List<T>();
      foreach (T obj in arr)
      {
        if (obj != null) list.Add(obj);
      }
      return list;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Parses strings in the following format: Name=Value.
    /// </summary>
    /// <param name="args">array of strings in format Name=Value</param>
    /// <param name="isQuoted">true if value can be enclosed in double quotas</param>
    /// <returns>NameValueCollection of parsed name-value pairs</returns>
    static public NameValueCollection ParseNamesAndValues(
      string[] args,
      bool isQuoted)
    {
      NameValueCollection result = new NameValueCollection();
      if (args != null)
      {
        for (int i = 0; i < args.Length; i++)
        {
          string name = "";
          string value = "";
          if (CxUtils.NotEmpty(args[i]))
          {
            int index = args[i].IndexOf("=");
            if (index > 0)
            {
              name = args[i].Substring(0, index);
              value = args[i].Substring(index + 1);
              if (isQuoted)
              {
                value = CxText.ExtractQuotedString(value);
              }
            }
            else if (index < 0)
            {
              name = args[i];
            }
          }
          if (CxUtils.NotEmpty(name))
          {
            result[name] = value;
          }
        }
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Parses strings in the following format: Name=Value.
    /// </summary>
    /// <param name="args">array of strings in format Name=Value</param>
    /// <returns>NameValueCollection of parsed name-value pairs</returns>
    static public NameValueCollection ParseNamesAndValues(string[] args)
    {
      return ParseNamesAndValues(args, false);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Parses strings in the following format: Name="Value".
    /// </summary>
    /// <param name="args">array of strings in format Name=Value</param>
    /// <returns>NameValueCollection of parsed name-value pairs</returns>
    static public NameValueCollection ParseNamesAndQuotedValues(string[] args)
    {
      return ParseNamesAndValues(args, true);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds given string in the array or list.
    /// </summary>
    /// <param name="list">string array to find strings</param>
    /// <param name="s">string to find</param>
    /// <returns>index of the string in array of -1 if not found</returns>
    static public int IndexOf(IList list, string s)
    {
      for (int i = 0; i < list.Count; i++)
      {
        if ((string) list[i] == s) return i;
      }
      return -1;
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Finds given string in the array or list.
    /// Case-insensitive.
    /// </summary>
    /// <param name="list">string array to find strings</param>
    /// <param name="s">string to find</param>
    /// <returns>index of the string in array of -1 if not found</returns>
    static public int IndexOfCI(IList list, string s)
    {
      for (int i = 0; i < list.Count; i++)
      {
        if (CxText.Equals((string) list[i], s)) return i;
      }
      return -1;
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Returns dictionary (hashtable) containing all list elements.
    /// </summary>
    static public IDictionary GetDictionaryFromList(IList list)
    {
      Hashtable result = new Hashtable();
      AppendDictionaryFromList(result, list);
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Appends dictionary (hashtable) with the list elements.
    /// </summary>
    static public void AppendDictionaryFromList(IDictionary dictionary, IList list)
    {
      if (dictionary != null && list != null)
      {
        foreach (object element in list)
        {
          if (element != null)
          {
            dictionary[element] = element;
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given collection is null or does not have elements.
    /// </summary>
    static public bool IsEmpty(ICollection list)
    {
      return list == null || list.Count == 0;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given collection is null or does not have elements.
    /// </summary>
    static public bool IsEmpty2<T>(ICollection<T> list)
    {
      return list == null || list.Count == 0;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds item to the list if item is not null.
    /// </summary>
    /// <param name="list">list to add item to</param>
    /// <param name="item">item to add to list</param>
    /// <returns>index of the added item or -1 if item was not added</returns>
    static public int AddIfNotNull(IList list, object item)
    {
      return list != null && item != null ? list.Add(item) : -1;
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Adds item to the dictionary if item is not null.
    /// </summary>
    /// <param name="dictionary">dictionary to add item to</param>
    /// <param name="key">key to add to dictionary</param>
    /// <param name="value">value to add to dictionary</param>
    static public void AddIfNotNull(IDictionary dictionary, object key, object value)
    {
      if (dictionary != null && key != null && value != null)
      {
        dictionary[key] = value;
      }
    }
    //--------------------------------------------------------------------------
    static public void AddRangeIfNotInList<T>(IList<T> list, IList<T> appendix, Func<IList<T>, T, bool> containsFunc = null)
    {
      if (containsFunc == null)
        containsFunc = (l, i) => l.Contains(i);
      foreach (var item in appendix)
      {
        if (!containsFunc(list, item))
          list.Add(item);
      }
    }
    //--------------------------------------------------------------------------
    static public void InsertRangeIfNotInList<T>(IList<T> list, IList<T> appendix, int position)
    {
      foreach (var item in appendix)
      {
        if (!list.Contains(item))
        {
          list.Insert(position, item);
          position++;
        }
      }
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Adds item to the list if item is not null and not exists in the list.
    /// </summary>
    /// <param name="list">list to add item to</param>
    /// <param name="item">item to add to list</param>
    /// <returns>index of the added item or -1 if item was not added</returns>
    static public int AddIfNotInList(IList list, object item)
    {
      if (list != null && item != null && list.IndexOf(item) < 0)
      {
        return list.Add(item);
      }
      return -1;
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Adds source list items to the list if item is not null and not exists in the list.
    /// </summary>
    /// <param name="list">list to add item to</param>
    /// <param name="sourceList">source list to add items from</param>
    /// <returns>index of the added item or -1 if item was not added</returns>
    static public void AddIfNotInList(IList list, IEnumerable sourceList)
    {
      if (list != null && sourceList != null)
      {
        Hashtable itemMap = new Hashtable();
        foreach (object item in list)
        {
          if (item != null)
          {
            itemMap[item] = true;
          }
        }
        foreach (object item in sourceList)
        {
          if (item != null && !itemMap.ContainsKey(item))
          {
            list.Add(item);
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds a range of items into the given list.
    /// </summary>
    /// <typeparam name="T">type of elements</typeparam>
    /// <param name="list">destination list</param>
    /// <param name="sourceList">source list</param>
    static public void AddRange<T>(IList<T> list, IEnumerable<T> sourceList)
    {
      foreach (T element in sourceList)
      {
        list.Add(element);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds a range of items into the given list.
    /// </summary>
    /// <typeparam name="T">type of elements</typeparam>
    /// <param name="list">destination list</param>
    /// <param name="sourceList">source list</param>
    static public void AddRange(IList list, IEnumerable sourceList)
    {
      foreach (object element in sourceList)
      {
        list.Add(element);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts source list to string list.
    /// </summary>
    /// <param name="source">source list</param>
    /// <returns>source list converted to string list</returns>
    static public List<string> ToStringList(IList source)
    {
      List<string> target = new List<string>();
      if (source != null)
      {
        foreach (object o in source)
        {
          target.Add(CxUtils.ToString(o));
        }
      }
      return target;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Moves the given objects inside the given collection by the offset.
    /// </summary>
    /// <param name="objectsToMove">the objects being moved</param>
    /// <param name="objects">the object-holder collection</param>
    /// <param name="offset">the offset of the move</param>
    /// <returns>true if succeeded</returns>
    static public bool Move<T>(IList<T> objectsToMove, IList<T> objects, int offset)
    {
      int[] oldIndexes = new int[objectsToMove.Count];
      for (int i = 0; i < objectsToMove.Count; i++)
      {
        oldIndexes[i] = objects.IndexOf(objectsToMove[i]);
      }

      // Here we get the index of the leading item, 
      // i.e. the future index of the first item in the direction of the move.
      int leadingIndex;
      if (offset > 0)
        leadingIndex = oldIndexes[oldIndexes.Length - 1] + offset;
      else if (offset < 0)
        leadingIndex = oldIndexes[0] + offset;
      else
        return false;

      // If the leading row is not in the range of the grid visible items - then do nothing.
      if (leadingIndex >= objects.Count || leadingIndex < 0)
        return false;

      if (offset > 0)
      {
        for (int i = objectsToMove.Count - 1; i >= 0; i--)
        {
          Move(objectsToMove[i], objects, offset);
        }
      }
      else
      {
        for (int i = 0; i < objectsToMove.Count; i++)
        {
          Move(objectsToMove[i], objects, offset);
        }
      }
      return true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Moves the given object inside the given collection by the offset.
    /// </summary>
    /// <param name="objectToMove">the object being moved</param>
    /// <param name="objects">the object-holder collection</param>
    /// <param name="offset">the offset to move by</param>
    /// <returns>true if succeeded</returns>
    static public bool Move<T>(T objectToMove, IList<T> objects, int offset)
    {
      int oldIndex = objects.IndexOf(objectToMove);
      if (oldIndex < 0)
        return false;
      int newIndex = oldIndex + offset;
      if (newIndex < 0 || newIndex >= objects.Count)
        return false;
      objects.RemoveAt(oldIndex);
      objects.Insert(newIndex, objectToMove);
      return true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts given enumrable to the typed list.
    /// </summary>
    /// <typeparam name="T">type of list elements</typeparam>
    /// <param name="source">source enumerable</param>
    /// <returns>converted list</returns>
    static public List<T> ToList<T>(IEnumerable source)
    {
      List<T> list = new List<T>();
      foreach (T item in source)
      {
        list.Add(item);
      }
      return list;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Removes from the given collection some subset of items (given).
    /// </summary>
    static public void RemoveSubList<T>(IList<T> source, IList<T> objectsToRemove)
    {
      foreach (T obj in objectsToRemove)
      {
        source.Remove(obj);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Compares two lists. 
    /// Considers list equal if two lists contains the same elements,
    /// element order is ignored.
    /// </summary>
    /// <typeparam name="T">type of list elements</typeparam>
    /// <param name="l1">first list</param>
    /// <param name="l2">second list</param>
    /// <returns>true if lists are equal, false otherwise</returns>
    static public bool CompareUnOrdered<T>(IList<T> l1, IList<T> l2)
    {
      if (l1 == null && l2 == null)
      {
        return true;
      }
      if (l1 == null || l2 == null)
      {
        return false;
      }
      if (l1.Count != l2.Count)
      {
        return false;
      }
      foreach (T item in l1)
      {
        if (!l2.Contains(item))
        {
          return false;
        }
      }
      foreach (T item in l2)
      {
        if (!l1.Contains(item))
        {
          return false;
        }
      }
      return true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Compares two lists. Element order is important.
    /// </summary>
    /// <typeparam name="T">type of list elements</typeparam>
    /// <param name="l1">first list</param>
    /// <param name="l2">second list</param>
    /// <returns>true if lists are equal, false otherwise</returns>
    static public bool CompareOrdered<T>(IList<T> l1, IList<T> l2)
    {
      if (l1 == null && l2 == null)
      {
        return true;
      }
      if (l1 == null || l2 == null)
      {
        return false;
      }
      if (l1.Count != l2.Count)
      {
        return false;
      }
      for (int i = 0; i < l1.Count; i++)
      {
        if (!Equals(l1[i], l2[i]))
        {
          return false;
        }
      }
      return true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Compares two lists. Element order is important.
    /// </summary>
    /// <typeparam name="T">type of list elements</typeparam>
    /// <param name="l1">first list</param>
    /// <param name="l2">second list</param>
    /// <returns>true if lists are equal, otherwise false</returns>
    static public bool CompareOrdered<T>(IList l1, IList l2) where T: class
    {
      if (l1 == null && l2 == null)
      {
        return true;
      }
      if (l1 == null || l2 == null)
      {
        return false;
      }
      if (l1.Count != l2.Count)
      {
        return false;
      }
      for (int i = 0; i < l1.Count; i++)
      {
        if (!Equals(l1[i] as T, l2[i] as T))
        {
          return false;
        }
      }
      return true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Joins non-empty and non-whitespace strings with the given separator.
    /// </summary>
    /// <returns>the junction result</returns>
    public static string JoinNotEmpty(string separator, params string[] stringArray)
    {
      var list = new List<string>();
      foreach (var str in stringArray)
      {
        if (!string.IsNullOrWhiteSpace(str))
          list.Add(str);
      }
      return string.Join(separator, list);
    }
    //-------------------------------------------------------------------------
  }
}