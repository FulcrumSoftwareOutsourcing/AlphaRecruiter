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

namespace Framework.Metadata
{
  public class CxStorableInIdOrderList: List<IxStorableInIdOrder>
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Seeks for the item with the given id.
    /// </summary>
    /// <param name="id">id to seek by</param>
    /// <returns>found item, otherwise null</returns>
    public IxStorableInIdOrder FindById(string id)
    {
      foreach (IxStorableInIdOrder item in this)
      {
        if (string.Equals(item.Id, id, StringComparison.OrdinalIgnoreCase))
          return item;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if an item with the given id is in the list.
    /// </summary>
    public bool Contains(string id)
    {
      return FindById(id) != null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts the list to the list of ids.
    /// </summary>
    public IList<string> ToStringList()
    {
      List<string> result = new List<string>();
      foreach (IxStorableInIdOrder item in this)
      {
        result.Add(item.Id);
      }
      return result.AsReadOnly();
    }
    //-------------------------------------------------------------------------
  }
}
