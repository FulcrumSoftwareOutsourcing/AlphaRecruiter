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
  public class CxMultilanguageItemList: List<CxMultilanguageItem>
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Default ctor.
    /// </summary>
    public CxMultilanguageItemList()
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    ///                     Initializes a new instance of the <see cref="T:System.Collections.Generic.List`1" /> class that contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.
    /// </summary>
    /// <param name="collection">
    ///                     The collection whose elements are copied to the new list.
    ///                 </param>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="collection" /> is null.
    ///                 </exception>
    public CxMultilanguageItemList(IEnumerable<CxMultilanguageItem> collection) : base(collection)
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Seeks for the multilanguage item with the given name.
    /// </summary>
    /// <param name="name">name to seek by</param>
    /// <returns>the item found, null otherwise</returns>
    public CxMultilanguageItem FindByName(string name)
    {
      foreach (CxMultilanguageItem item in this)
      {
        if (string.Equals(item.ObjectName, name, StringComparison.OrdinalIgnoreCase))
          return item;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Seeks for the multilanguage item with the given property code.
    /// </summary>
    /// <param name="propertyCd">property code to seek by</param>
    /// <returns>the item found, null otherwise</returns>
    public CxMultilanguageItem FindByPropertyCd(string propertyCd)
    {
      foreach (CxMultilanguageItem item in this)
      {
        if (string.Equals(item.PropertyCd, propertyCd, StringComparison.OrdinalIgnoreCase))
          return item;
      }
      return null;
    }
    //-------------------------------------------------------------------------
  }
}
