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

namespace Framework.Db
{
  //----------------------------------------------------------------------------
  /// <summary>
  /// Represents a list of aggregate descriptors.
  /// </summary>
  public class CxAggregateDescriptorList : List<CxAggregateDescriptor>
  {
    //----------------------------------------------------------------------------
    /// <summary>
    /// Default ctor.
    /// </summary>
    public CxAggregateDescriptorList()
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="list">a list to base on</param>
    public CxAggregateDescriptorList(IEnumerable<CxAggregateDescriptor> list)
      :this()
    {
      AddRange(list);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns aggregate descriptors by the given field name.
    /// </summary>
    /// <param name="fieldName">a name of the field to seek by</param>
    /// <returns>a list of aggregate descriptors</returns>
    public CxAggregateDescriptorList GetByFieldName(string fieldName)
    {
      CxAggregateDescriptorList list = new CxAggregateDescriptorList();
      for (int i = 0; i < Count; i++)
      {
        if (string.Equals(this[i].FieldName, fieldName, StringComparison.OrdinalIgnoreCase))
          list.Add(this[i]);
      }
      return list;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns aggregate descriptors by the given descriptor type.
    /// </summary>
    /// <param name="descriptorType">a type of descriptors to be returned</param>
    /// <returns>a list of aggregate descriptors</returns>
    public CxAggregateDescriptorList GetByDescriptorType(
      NxAggregateDescriptorType descriptorType)
    {
      return GetByDescriptorType(descriptorType, false);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns aggregate descriptors by the given descriptor type.
    /// </summary>
    /// <param name="descriptorType">a type of descriptors to be returned</param>
    /// <param name="doInvert">if true, the returned collection contains only
    /// aggregate descriptors of all types apart from the given one</param>
    /// <returns>a list of aggregate descriptors</returns>
    public CxAggregateDescriptorList GetByDescriptorType(
      NxAggregateDescriptorType descriptorType, bool doInvert)
    {
      CxAggregateDescriptorList list = new CxAggregateDescriptorList();
      for (int i = 0; i < Count; i++)
      {
        if ((this[i].DescriptorType == descriptorType && !doInvert)
         || (this[i].DescriptorType != descriptorType && doInvert))
          list.Add(this[i]);
      }
      return list;
    }
    //----------------------------------------------------------------------------
  }
}
