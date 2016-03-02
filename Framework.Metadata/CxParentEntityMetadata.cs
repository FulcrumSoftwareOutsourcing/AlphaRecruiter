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
using System.Xml;
using Framework.Utils;

namespace Framework.Metadata
{
	/// <summary>
	/// Metadata for description of parent entity that can be obtained from
	/// this entity.
	/// </summary>
	public class CxParentEntityMetadata : CxMetadataObject
	{
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="element">XML element to load data from</param>
		public CxParentEntityMetadata(
      CxMetadataHolder holder, 
      XmlElement element) : base(holder, element)
		{
      AddNodeToProperties(element, "where_clause");
    }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns SQL WHERE condition to get parent entity instance.
    /// </summary>
    public string WhereClause
    { get {return this["where_clause"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity usage ID that can be found by the WHERE clause.
    /// </summary>
    public string EntityUsageId
    { get {return CxText.ToUpper(this["entity_usage_id"]);} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Entity metadata object.
    /// </summary>
    public CxEntityMetadata Entity
    { get {return Holder.Entities[Id];} }
    //-------------------------------------------------------------------------
  }
}