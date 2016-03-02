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
using System.Configuration;
using System.Xml;
using Framework.Utils;
using System.Collections.Generic;
using Framework.Common;

namespace Framework.Metadata
{
	/// <summary>
	/// Page metadata collection
	/// </summary>
	public class CxPagesMetadata : CxMetadataCollection
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
    /// <param name="doc">XML document to create list of pages</param>
    public CxPagesMetadata(CxMetadataHolder holder, XmlDocument doc) : 
      base(holder, doc)
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="docs">XML documents to load metadata from</param>
    public CxPagesMetadata(CxMetadataHolder holder, IEnumerable<XmlDocument> docs)
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
      foreach (XmlElement pageElement in doc.DocumentElement.SelectNodes("page"))
      {
        CxPageMetadata page = new CxPageMetadata(Holder, pageElement);
        AddItem(page);
      }
      LoadOverrides(doc, "page_override", m_ItemsMap);
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
      return typeof(CxPageMetadata);
    }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of all items.
    /// </summary>
    public ArrayList ItemsList
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
    public Hashtable ItemsMap
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
    /// Finds page by the page ID.
    /// </summary>
    /// <param name="id">ID of page to find</param>
    /// <returns>found page or null</returns>
    public CxPageMetadata Find(string id)
    {
      return CxUtils.NotEmpty(id) ? (CxPageMetadata) ItemsMap[id.ToUpper()] : null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Page with a given ID.
    /// </summary>
    public CxPageMetadata this[string id]
    {
      get
      {
        CxPageMetadata page = Find(id);
        if (page != null)
          return page;
        else
          throw new ExMetadataException(string.Format("Page with ID=\"{0}\" not defined", id));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns default application startup page (if specified).
    /// </summary>
    public CxPageMetadata DefaultPage
    {
      get
      {
        string DefaultPageId = CxConfigurationHelper.DefaultPageId;
        if (DefaultPageId != null && DefaultPageId != string.Empty)
        {
          object _defaultPage = m_ItemsMap[DefaultPageId.ToUpper()];
          if (_defaultPage != null && _defaultPage is CxPageMetadata)
          {
            return (CxPageMetadata) _defaultPage;
          }
        }
        
        foreach (CxPageMetadata page in ItemsList)
        {
          if (page.IsDefault)
          {
            return page;
          }
        }
        return null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns default name for the metadata XML file.
    /// </summary>
    override protected string XmlFileName
    { get { return "WebPages.xml"; } }
    //-------------------------------------------------------------------------
  }
}