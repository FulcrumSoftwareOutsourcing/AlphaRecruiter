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

namespace Framework.Metadata
{
  //-------------------------------------------------------------------------
  /// <summary>
  /// A base class for all the items that can participate in defining
  /// an entity's edit order.
  /// </summary>
  public class CxOrderItem
  {
    //-------------------------------------------------------------------------
    public class CxComparer : IEqualityComparer<CxOrderItem>
    {
      //-------------------------------------------------------------------------
      public bool Equals(CxOrderItem item1, CxOrderItem item2)
      {
        if (item1 == null && item2 == null)
          return true;
        if (item1 != null && item2 != null)
          return item1.Equals(item2);
        return false;
      }
      //-------------------------------------------------------------------------
      public int GetHashCode(CxOrderItem item)
      {
        return item.GetHashCode();
      }
      //-------------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
  }
}
