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

using System.ComponentModel;

namespace Framework.Db
{
  //----------------------------------------------------------------------------
  /// <summary>
  /// Descripts field sorting element.
  /// </summary>
  public class CxSortDescriptor
  {
    //----------------------------------------------------------------------------
    private string m_FieldName;
    private ListSortDirection m_SortDirection;
    //----------------------------------------------------------------------------
    /// <summary>
    /// A name of a field.
    /// </summary>
    public string FieldName
    {
      get { return m_FieldName; }
      set { m_FieldName = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// A direction of sorting.
    /// </summary>
    public ListSortDirection SortDirection
    {
      get { return m_SortDirection; }
      set { m_SortDirection = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Default ctor.
    /// </summary>
    public CxSortDescriptor()
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="fieldName"></param>
    /// <param name="sortDirection"></param>
    public CxSortDescriptor(string fieldName, ListSortDirection sortDirection)
      : this()
    {
      FieldName = fieldName;
      SortDirection = sortDirection;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    ///                     Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
    /// </summary>
    /// <returns>
    ///                     A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public override string ToString()
    {
      if (!string.IsNullOrEmpty(FieldName))
        return FieldName;
      return base.ToString();
    }
    //----------------------------------------------------------------------------
  }
}
