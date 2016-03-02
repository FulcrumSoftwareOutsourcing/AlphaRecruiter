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
	/// Summary description for CxPortalSkinsMetadata.
	/// </summary>
	public class CxPortalSkinsMetadata : CxMetadataCollection
	{
    //-------------------------------------------------------------------------
    protected ArrayList m_ItemsList = new ArrayList();
    protected Hashtable m_ItemsMap = new Hashtable();
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="doc">XML document to load metadata from</param>
		public CxPortalSkinsMetadata(CxMetadataHolder holder, XmlDocument doc) : 
      base(holder, doc)
		{
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="docs">XML documents to load metadata from</param>
    public CxPortalSkinsMetadata(CxMetadataHolder holder, IEnumerable<XmlDocument> docs)
      : base(holder, docs)
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata collection from the XML document.
    /// </summary>
    /// <param name="doc">document to load data from</param>
    override protected void Load(XmlDocument doc)
    {
      base.Load(doc);
      foreach (XmlElement skinElement in doc.DocumentElement.SelectNodes("skin"))
      {
        CxPortalSkinMetadata skin = new CxPortalSkinMetadata(Holder, skinElement);
        AddItem(skin);
      }
      LoadOverrides(doc, "skin_override", m_ItemsMap);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds item to list and map.
    /// </summary>
    /// <param name="item">item to add</param>
    protected void AddItem(CxMetadataObject item)
    {
      m_ItemsList.Add(item);
      m_ItemsMap.Add(item.Id, item);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns type of metadata object holded by the collection.
    /// </summary>
    virtual protected Type GetMetadataObjectType()
    {
      return typeof(CxPortalSkinMetadata);
    }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of all items.
    /// </summary>
    protected ArrayList ItemsList
    {
      get
      {
        CxUserMetadataCacheElement cache = 
          Holder.GetUserMetadataCache(GetMetadataObjectType(), null, m_ItemsList);
        if (cache != null)
        {
          return cache.List;
        }
        return m_ItemsList;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns map of all items indexed by item ID.
    /// </summary>
    protected Hashtable ItemsMap
    {
      get
      {
        CxUserMetadataCacheElement cache = 
          Holder.GetUserMetadataCache(GetMetadataObjectType(), null, m_ItemsList);
        if (cache != null)
        {
          return cache.Map;
        }
        return m_ItemsMap;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds skin by the skin ID.
    /// </summary>
    /// <param name="id">ID of skin to find</param>
    /// <returns>found skin or null</returns>
    public CxPortalSkinMetadata Find(string id)
    {
      return CxUtils.NotEmpty(id) ? (CxPortalSkinMetadata) ItemsMap[id.ToUpper()] : null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds predefeined skin (defined in XML file) by the skin ID.
    /// </summary>
    /// <param name="id">ID of skin to find</param>
    /// <returns>found skin or null</returns>
    public CxPortalSkinMetadata FindPredefined(string id)
    {
      return CxUtils.NotEmpty(id) ? (CxPortalSkinMetadata) m_ItemsMap[id.ToUpper()] : null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns portal skin by ID.
    /// </summary>
    public CxPortalSkinMetadata this[string id]
    {
      get
      {
        CxPortalSkinMetadata skin = Find(id);
        if (skin != null)
          return skin;
        else
          throw new ExMetadataException(string.Format("Portal Skin with ID=\"{0}\" not defined", id));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of skins.
    /// </summary>
    public IList Items
    { get {return ItemsList;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Default portal application skin.
    /// </summary>
    public CxPortalSkinMetadata DefaultSkin
    { 
      get 
      {
        IList items = ItemsList;
        foreach (CxPortalSkinMetadata skin in items)
        {
          if (skin.IsDefault)
          {
            return skin;
          }
        }
        return items.Count > 0 ? (CxPortalSkinMetadata) items[0] : null;
      } 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns default name for the metadata XML file.
    /// </summary>
    override protected string XmlFileName
    { get { return "WebPortalSkins.xml"; } }
    //-------------------------------------------------------------------------
  }
}