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
  /// Class to read and hold information about application entities.
  /// </summary>
  public class CxEntitiesMetadata : CxMetadataCollection
	{
    //----------------------------------------------------------------------------
    protected Hashtable m_Entities = new Hashtable(); // Entities dictionary
    protected List<CxEntityMetadata> m_Items = new List<CxEntityMetadata>(); // Entities list
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="doc">XML doc to read metadata from</param>
    public CxEntitiesMetadata(CxMetadataHolder holder, XmlDocument doc) : 
      base(holder, doc)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="docs">name of file to read assemblies metadata</param>
    public CxEntitiesMetadata(CxMetadataHolder holder, IEnumerable<XmlDocument> docs)
      : base(holder, docs)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata collection from the XML document.
    /// </summary>
    /// <param name="doc">document to load data from</param>
    override protected void Load(XmlDocument doc)
    {
      base.Load(doc);
      foreach (XmlElement element in doc.DocumentElement.SelectNodes("entity"))
      {
        CxEntityMetadata entity = new CxEntityMetadata(Holder, element);
        m_Entities.Add(entity.Id, entity);
        m_Items.Add(entity);
      }
      LoadOverrides(doc, "entity_override", m_Entities);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds entity with the given ID.
    /// </summary>
    /// <param name="id">ID to find</param>
    /// <returns>found entity metadata or null</returns>
    public CxEntityMetadata Find(string id)
    {
      return (CxEntityMetadata) m_Entities[id.ToUpper()];
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Entity with the given ID.
    /// </summary>
    public CxEntityMetadata this[string id]
    {
      get
      { 
        CxEntityMetadata entity = Find(id);
        if (entity != null)
          return entity;
        else
          throw new ExMetadataException(string.Format("Entity with ID=\"{0}\" not defined", id));
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Entities dictionary.
    /// </summary>
    public Hashtable Entities
    {
      get { return m_Entities; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Entities list.
    /// </summary>
    public List<CxEntityMetadata> Items
    { 
      get { return m_Items; } 
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns default name for the metadata XML file.
    /// </summary>
    override protected string XmlFileName
    { get { return "Entities.xml"; } }
    //-------------------------------------------------------------------------
  }
}