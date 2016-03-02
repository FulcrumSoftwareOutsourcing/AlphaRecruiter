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
using System.Xml;

using Framework.Utils;
using System.Collections.Generic;

namespace Framework.Metadata
{
	/// <summary>
	/// Class that holds information about child entity usage.
	/// </summary>
	public class CxEntityUsageToEditMetadata : CxMetadataObject
	{
    //----------------------------------------------------------------------------
    protected List<string> m_PKColumns = null; // List of primary key columns to use to find entity
    protected List<string> m_Columns = null; // List of columns that should corresponds to the entity usage
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="element">XML element that holds metadata</param>
    public CxEntityUsageToEditMetadata(CxMetadataHolder holder, XmlElement element) : 
      base(holder, element, "child_id")
    {
      m_PKColumns = CxText.DecomposeWithSeparator(this["pk_columns"], ",");
      m_Columns = CxText.DecomposeWithSeparator(this["columns"].ToUpper(), ",");
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// List of primary key columns to use to find entity.
    /// </summary>
    public List<string> PKColumns 
    {
      get { return m_PKColumns; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// ID of the entity usage to edit.
    /// </summary>
    public string EntityUsageId
    {
      get { return this["entity_usage_id"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Entity usage to edit.
    /// </summary>
    public CxEntityUsageMetadata EntityUsage
    {
      get { return Holder.EntityUsages[EntityUsageId]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// ID of the entity usage chooser class.
    /// </summary>
    public string ChooserClassId
    {
      get { return this["chooser_class_id"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Entity usage chooser class.
    /// </summary>
    public CxClassMetadata ChooserClass
    {
      get { return Holder.Classes[ChooserClassId]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given column should be edited by this entity usage.
    /// </summary>
    /// <param name="columnName">name of the column to check</param>
    /// <param name="exact">if false an ansence of column names means compliance</param>
    /// <returns>true if the given column should be edited by this entity usage</returns>
    public bool Complies(string columnName, bool exact)
    {
      return ((m_Columns.Count == 0 && ! exact) || 
              m_Columns.IndexOf(columnName.ToUpper()) != -1);
    }
    //----------------------------------------------------------------------------
  }
}
