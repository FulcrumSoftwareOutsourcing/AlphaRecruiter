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

using System.Collections.Generic;
using System.Data;

namespace Framework.Db
{
  public class CxAggregateQueryDescriptor: CxQueryDescriptor
  {
    //----------------------------------------------------------------------------
    private Dictionary<object, string> m_AggregateAliases;
    //----------------------------------------------------------------------------
    /// <summary>
    /// Contains aggregate descriptor keys and corresponding aliases.
    /// </summary>
    public Dictionary<object, string> AggregateAliases
    {
      get { return m_AggregateAliases; }
      set { m_AggregateAliases = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Default ctor.
    /// </summary>
    public CxAggregateQueryDescriptor()
    {
      AggregateAliases = new Dictionary<object, string>();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns a dictionary of values by aggregate descriptor keys.
    /// </summary>
    /// <param name="row">a row to get values from</param>
    /// <returns>a dictionary of values by aggregate descriptor keys</returns>
    public Dictionary<object, object> GetDataRowAggregateValues(DataRow row)
    {
      return GetDataRowAggregateValues(row, new object[] { });
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns a dictionary of values by aggregate descriptor keys.
    /// </summary>
    /// <param name="row">a row to get values from</param>
    /// <param name="keysToIgnore">aggregate descriptor keys to be ignored</param>
    /// <returns>a dictionary of values by aggregate descriptor keys</returns>
    public Dictionary<object, object> GetDataRowAggregateValues(DataRow row, object[] keysToIgnore)
    {
      Dictionary<object, object> result = new Dictionary<object, object>();
      foreach (KeyValuePair<object, string> pair in AggregateAliases)
      {
        bool ignore = false;
        for (int i = 0; i < keysToIgnore.Length; i++)
          if (keysToIgnore[i] == pair.Key)
          {
            ignore = true;
            break;
          }
        if (ignore)
          continue;
        result.Add(pair.Key, row[pair.Value]);
      }
      return result;
    }
    //----------------------------------------------------------------------------
  }
}
