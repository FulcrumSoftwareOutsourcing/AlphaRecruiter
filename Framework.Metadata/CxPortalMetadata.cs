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
using System.Reflection;
using System.IO;
using System.Xml;

using Framework.Utils;

namespace Framework.Metadata
{
  /// <summary>
  /// Class to hold information about application class.
  /// </summary>
  public class CxPortalMetadata : CxMetadataObject
  {
    //----------------------------------------------------------------------------
    protected CxTreeItemsMetadata m_Items = null;
    protected Hashtable m_ItemMap = new Hashtable();
    //----------------------------------------------------------------------------

    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="element">XML element that holds metadata</param>
    public CxPortalMetadata(
      CxMetadataHolder holder, 
      XmlElement element) : base(holder, element)
    {
      m_Items = new CxTreeItemsMetadata(Holder, this, element);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Registers tree item in the tree items map.
    /// </summary>
    /// <param name="item">item to register</param>
    public void RegisterTreeItem(CxTreeItemMetadata item)
    {
      m_ItemMap.Add(item.Id.ToUpper(), item);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Finds tree item metadata by ID.
    /// </summary>
    /// <param name="id">ID of item to find</param>
    /// <returns>found item or null</returns>
    public CxTreeItemMetadata Find(string id)
    {
      CxTreeItemMetadata item = (CxTreeItemMetadata) m_ItemMap[id.ToUpper()];
      if (item != null && item.GetIsAllowed())
      {
        return item;
      }
      return null;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata object override.
    /// </summary>
    /// <param name="element">XML element to load overridden properties from</param>
    override public void LoadOverride(XmlElement element)
    {
      base.LoadOverride(element);

      if (m_Items != null)
      {
        foreach (XmlElement itemElement in element.SelectNodes("tree_item"))
        {
          CxTreeItemMetadata treeItem = Find(CxXml.GetAttr(itemElement, "id"));
          if (treeItem != null)
          {
            treeItem.LoadOverride(itemElement);
          }
          else
          {
            treeItem = new CxTreeItemMetadata(Holder, this, itemElement);
            m_Items.Add(treeItem);
          }
        }
      }
    }
    //----------------------------------------------------------------------------

    //----------------------------------------------------------------------------
    /// <summary>
    /// Root portal tree items collection.
    /// </summary>
    public CxTreeItemsMetadata Items
    { get {return m_Items;} }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns portal skin ID.
    /// </summary>
    public string SkinId
    { get {return this["skin_id"];} }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns portal skin metadata object.
    /// </summary>
    public CxPortalSkinMetadata Skin
    {
      get
      {
        if (CxUtils.NotEmpty(SkinId))
        {
          return Holder.PortalSkins[SkinId];
        }
        return Holder.PortalSkins.DefaultSkin;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// True if the portal is a default portal.
    /// </summary>
    public bool IsDefault
    { get {return this["is_default"] == "true";} }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns application title for the portal.
    /// </summary>
    public string Title
    {
      get
      {
        string title = this["title"];
        if (CxUtils.IsEmpty(title) && 
            Holder.Portals.Default != null &&
            Holder.Portals.Default != this)
        {
          title = Holder.Portals.Default.Title;
        }
        return title;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Checks portal access permission depending on security settings.
    /// </summary>
    public bool GetIsAllowed()
    {
      if (Holder != null && Holder.Security != null)
      {
        return Holder.Security.GetRight(this);
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
        return "Metadata.WebPortal";
      }
    }
    //----------------------------------------------------------------------------
  }
}