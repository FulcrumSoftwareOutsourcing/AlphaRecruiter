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

namespace Framework.Metadata
{
  //---------------------------------------------------------------------------
  /// <summary>
  /// User metadata cache element.
  /// </summary>
  public class CxUserMetadataCacheElement
  {
    //-------------------------------------------------------------------------
    protected ArrayList m_List = new ArrayList();
    protected Hashtable m_Map = new Hashtable();
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public CxUserMetadataCacheElement()
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds metadata object to cache element list.
    /// </summary>
    /// <param name="item">metadata object to add</param>
    public void AddItem(CxMetadataObject item)
    {
      m_List.Add(item);
      m_Map.Add(item.Id, item);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds list of items to cache element list.
    /// </summary>
    /// <param name="items">list of metadata objects</param>
    public void AddItems(IList items)
    {
      if (items != null)
      {
        foreach (CxMetadataObject item in items)
        {
          AddItem(item);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// List of metadata objects.
    /// </summary>
    public ArrayList List
    { get {return m_List;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Map of metadata objects indexed by ID.
    /// </summary>
    public Hashtable Map
    { get {return m_Map;} }
    //-------------------------------------------------------------------------
  }
  //---------------------------------------------------------------------------

  //---------------------------------------------------------------------------
  /// <summary>
	/// Cache for user-defined metadata.
	/// </summary>
	public class CxUserMetadataCache
	{
    //-------------------------------------------------------------------------
    protected Hashtable m_TypeMap = new Hashtable();
    protected Hashtable m_ParentObjectMap = new Hashtable();
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
		public CxUserMetadataCache()
		{
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns cached element.
    /// </summary>
    public CxUserMetadataCacheElement GetCacheElement(
      Type metadataObjectType,
      CxMetadataObject parentObject)
    {
      CxUserMetadataCacheElement cacheElement = null;
      if (parentObject != null)
      {
        cacheElement = (CxUserMetadataCacheElement) m_ParentObjectMap[parentObject];
      }
      else if (metadataObjectType != null)
      {
        cacheElement = (CxUserMetadataCacheElement) m_TypeMap[metadataObjectType];
      }
      return cacheElement;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Saves cache element to cache.
    /// </summary>
    public void SetCacheElement(
      Type metadataObjectType,
      CxMetadataObject parentObject,
      CxUserMetadataCacheElement cacheElement)
    {
      if (cacheElement != null)
      {
        if (parentObject != null)
        {
          m_ParentObjectMap[parentObject] = cacheElement;
        }
        else if (metadataObjectType != null)
        {
          m_TypeMap[metadataObjectType] = cacheElement;
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Removes cache element to cache.
    /// </summary>
    public void ClearCacheElement(
      Type metadataObjectType,
      CxMetadataObject parentObject)
    {
      if (parentObject != null)
      {
        m_ParentObjectMap.Remove(parentObject);
      }
      else if (metadataObjectType != null)
      {
        m_TypeMap.Remove(metadataObjectType);
      }
    }
    //-------------------------------------------------------------------------
  }
}