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
using System.Collections.Generic;
using System.Text;

namespace Framework.Utils
{
  //-------------------------------------------------------------------------
  /// <summary>
  /// Utility methods to work with dictionaries.
  /// </summary>
  static public class CxDictionary
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a list of dictionary's values.
    /// </summary>
    /// <typeparam name="TKey">a type of keys in the dictionary</typeparam>
    /// <typeparam name="TValue">a type of values in the dictionary</typeparam>
    /// <param name="dictionary">a dictionary to extract values from</param>
    /// <returns>a list of extracted values</returns>
    static public List<TValue> ExtractDictionaryValueList<TKey, TValue>(
      IDictionary<TKey, TValue> dictionary)
    {
      List<TValue> list = new List<TValue>();
      foreach (KeyValuePair<TKey, TValue> pair in dictionary)
      {
        list.Add(pair.Value);
      }
      return list;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a list of dictionary's keys.
    /// </summary>
    /// <typeparam name="TKey">a type of keys in the dictionary</typeparam>
    /// <typeparam name="TValue">a type of values in the dictionary</typeparam>
    /// <param name="dictionary">a dictionary to extract keys from</param>
    /// <returns>a list of extracted keys</returns>
    static public List<TKey> ExtractDictionaryKeyList<TKey, TValue>(
      IDictionary<TKey, TValue> dictionary)
    {
      List<TKey> list = new List<TKey>();
      foreach (KeyValuePair<TKey, TValue> pair in dictionary)
      {
        list.Add(pair.Key);
      }
      return list;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns arrays of keys and values in the respective order.
    /// </summary>
    /// <typeparam name="TKey">a type of keys</typeparam>
    /// <typeparam name="TValue">a type of values</typeparam>
    /// <param name="dictionary">input dictionary</param>
    /// <param name="keys">an array of keys</param>
    /// <param name="values">an array of values</param>
    static public void ExtractDictionaryKeysAndValues<TKey, TValue>
      (IDictionary<TKey, TValue> dictionary, out TKey[] keys, out TValue[] values)
    {
      keys = new TKey[dictionary.Count];
      values = new TValue[dictionary.Count];
      int i = 0;
      foreach (KeyValuePair<TKey, TValue> pair in dictionary)
      {
        keys[i] = pair.Key;
        values[i] = pair.Value;
        i++;
      }
    }
    //-------------------------------------------------------------------------
    static public Dictionary<TKey, TValue> CreateDictionary<TKey, TValue>(IList<TKey> keys, IList<TValue> values)
    {
      Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
      for (int i = 0; i < keys.Count; i++)
      {
        dictionary[keys[i]] = values[i];
      }
      return dictionary;
    }
    //-------------------------------------------------------------------------
  }
}
