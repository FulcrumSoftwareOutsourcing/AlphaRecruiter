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
	/// Metadata for navigation tree item.
	/// </summary>
	public class CxTreeItemMetadata : CxMetadataObject
	{
    //-------------------------------------------------------------------------
    protected CxPortalMetadata m_Portal = null;
    protected CxPageMetadata m_Page = null;
    protected CxTreeItemsMetadata m_Items = null;
    protected CxEntityUsageMetadata m_EntityUsage = null;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="element">XML element to load data from</param>
		public CxTreeItemMetadata(
      CxMetadataHolder holder, 
      CxPortalMetadata portal,
      XmlElement element) : base(holder, element)
		{
      m_Portal = portal;
      m_Portal.RegisterTreeItem(this);
      m_Items = new CxTreeItemsMetadata(Holder, Portal, element);
      if (CxUtils.NotEmpty(PageId))
      {
        m_Page = Holder.Pages[PageId];
      }
      if (CxUtils.NotEmpty(EntityUsageId))
      {
        m_EntityUsage = Holder.EntityUsages[EntityUsageId];
      }
    }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ID of the web page linked to the tree node.
    /// </summary>
    public string PageId
    {
      get
      {
        return this["page_id"];
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ID of the entity usage linked to the tree node.
    /// </summary>
    public string EntityUsageId
    {
      get
      {
        return this["entity_usage_id"];
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Child tree items.
    /// </summary>
    public CxTreeItemsMetadata Items
    { get {return m_Items;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Corresponding portal page link.
    /// </summary>
    public CxPageMetadata Page
    { get {return m_Page;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Corresponding entity usage metadata object.
    /// </summary>
    public CxEntityUsageMetadata EntityUsage
    { get {return m_EntityUsage;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns parent portal.
    /// </summary>
    public CxPortalMetadata Portal
    { get {return m_Portal;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Checks tree item access permission depending on security settings.
    /// </summary>
    public bool GetIsAllowed()
    {
      if (Holder != null && Holder.Security != null)
      {
        return Holder.Security.GetRight(this, EntityUsage);
      }
      return true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns object type code for localization.
    /// </summary>
    override public string LocalizationObjectTypeCode
    {
      get
      {
        return "Metadata.WebTreeItem";
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns unique object name for localization.
    /// </summary>
    override public string LocalizationObjectName
    {
      get
      {
        return Portal != null ? Portal.LocalizationObjectName + "." + Id : base.LocalizationObjectName;
      }
    }
    //----------------------------------------------------------------------------
  }
}