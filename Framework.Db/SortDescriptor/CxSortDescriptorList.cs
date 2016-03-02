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
using System.ComponentModel;

namespace Framework.Db
{
  /// <summary>
  /// A list of sort descriptors.
  /// </summary>
  public class CxSortDescriptorList : List<CxSortDescriptor>
  {
    //----------------------------------------------------------------------------
    /// <summary>
    /// Default ctor.
    /// </summary>
    public CxSortDescriptorList()
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="list">a list to base on</param>
    public CxSortDescriptorList(IEnumerable<CxSortDescriptor> list)
      :this()
    {
      AddRange(list);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns a sort descriptor by the given field name.
    /// </summary>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    public CxSortDescriptor GetByFieldName(string fieldName)
    {
      for (int i = 0; i < Count; i++)
      {
        if (string.Equals(this[i].FieldName, fieldName, StringComparison.OrdinalIgnoreCase))
          return this[i];
      }
      return null;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns a list of sort descriptors with the given names.
    /// </summary>
    /// <param name="fieldNames"></param>
    /// <returns></returns>
    public CxSortDescriptorList GetByFieldName(string[] fieldNames)
    {
      CxSortDescriptorList list = new CxSortDescriptorList();
      for (int i = 0; i < Count; i++)
      {
        for (int j = 0; j < fieldNames.Length; j++)
          if (string.Equals(this[i].FieldName, fieldNames[j], StringComparison.OrdinalIgnoreCase))
            list.Add(this[i]);
      }
      return list;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Checks if this list represents the same sort descriptors as 
    /// the given one does.
    /// </summary>
    /// <param name="descriptions"></param>
    /// <returns></returns>
    public bool Equals(ListSortDescriptionCollection descriptions)
    {
      if (descriptions == null || descriptions.Count != Count)
        return false;
      for (int i = 0; i < descriptions.Count; i++)
      {
        if (this[i].SortDirection != descriptions[i].SortDirection || this[i].FieldName != descriptions[i].PropertyDescriptor.Name)
          return false;
      }
      return true;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Checks if this list represents the same sort descriptors as 
    /// the given one does.
    /// </summary>
    /// <param name="descriptions"></param>
    /// <returns></returns>
    public bool Equals(CxSortDescriptorList descriptions)
    {
      if (descriptions == null || descriptions.Count != Count)
        return false;
      for (int i = 0; i < descriptions.Count; i++)
      {
        if (this[i].SortDirection != descriptions[i].SortDirection || !string.Equals(this[i].FieldName, descriptions[i].FieldName, StringComparison.OrdinalIgnoreCase))
          return false;
      }
      return true;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates a sort descriptor list based on the given list sort description collection.
    /// </summary>
    /// <param name="source">a list sort description collection to be used as a data source</param>
    /// <returns>the created sort descriptor list</returns>
    static public CxSortDescriptorList Create(ListSortDescriptionCollection source)
    {
      int count = source == null ? 0 : source.Count;
      CxSortDescriptor[] descriptors = new CxSortDescriptor[count];
      for (int i = 0; i < count; i++)
      {
        ListSortDescription listSortDescription = source[i];
        descriptors[i] = new CxSortDescriptor(listSortDescription.PropertyDescriptor.Name, listSortDescription.SortDirection);
      }
      return new CxSortDescriptorList(descriptors);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets an array of names of the fields provided by the list.
    /// </summary>
    /// <param name="startIndex">a start index to read field names from</param>
    /// <param name="count">an amount of field names to read</param>
    /// <returns>an array of field names</returns>
    public string[] GetFieldNames(int startIndex, int count)
    {
      string[] result = new string[count];
      for (int i = startIndex; i < Math.Min(startIndex + count, Count); i++)
      {
        result[i - startIndex] = this[i].FieldName;
      }
      return result;
    }
    //----------------------------------------------------------------------------
  }
}
