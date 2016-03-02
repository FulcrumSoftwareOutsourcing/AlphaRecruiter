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

using System.Collections;
using System.Collections.Generic;
using System.Xml;

using Framework.Utils;

namespace Framework.Metadata
{
  /// <summary>
  /// Class to read and hold information about application entity usages.
  /// </summary>
  public class CxEntityUsagesMetadata : CxMetadataCollection
  {
    //----------------------------------------------------------------------------
    protected Hashtable m_EntityUsages = new Hashtable(); // Entity usages dictionary
    protected List<CxEntityUsageMetadata> m_Items = new List<CxEntityUsageMetadata>(); // Entity usages list
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">metadata holder</param>
    /// <param name="doc">XML doc to read metadata from</param>
    public CxEntityUsagesMetadata(CxMetadataHolder holder, XmlDocument doc)
      : base(holder, doc)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">metadata holder</param>
    /// <param name="docs">name of file to read assemblies metadata</param>
    public CxEntityUsagesMetadata(CxMetadataHolder holder, IEnumerable<XmlDocument> docs)
      : base(holder, docs)
    {
      // We perform inheritance just after all the metadata is loaded from XML, 
      // including overrides.
      foreach (CxEntityUsageMetadata entityUsageMetadata in Items)
      {
        entityUsageMetadata.InheritPropertiesFrom(entityUsageMetadata.InheritanceList);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="entityUsages">the list of entity usages to add</param>
    public CxEntityUsagesMetadata(
      CxMetadataHolder holder, IEnumerable<CxEntityUsageMetadata> entityUsages)
      : base(holder)
    {
      if (entityUsages == null)
        throw new ExNullArgumentException("entityUsages");
      foreach (CxEntityUsageMetadata entityUsageMetadata in entityUsages)
      {
        if (entityUsageMetadata != null)
          Add(entityUsageMetadata);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata collection from the XML document.
    /// </summary>
    /// <param name="doc">document to load data from</param>
    override protected void Load(XmlDocument doc)
    {
      base.Load(doc);
      foreach (XmlElement element in doc.DocumentElement.SelectNodes("entity_usage"))
      {
        CxEntityUsageMetadata entityUsage = new CxEntityUsageMetadata(Holder, element, this);
        Add(entityUsage);
      }
      LoadOverrides(doc, "entity_usage_override", m_EntityUsages);
      //LoadCustomizations(doc, "entity_usage_custom", m_EntityUsages);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds entity usage to the collection.
    /// </summary>
    protected void Add(CxEntityUsageMetadata entityUsage)
    {
      m_EntityUsages.Add(entityUsage.Id, entityUsage);
      m_Items.Add(entityUsage);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds entity usage with the given ID.
    /// </summary>
    /// <param name="id">ID to find</param>
    /// <returns>found entity usage metadata or null</returns>
    public CxEntityUsageMetadata Find(string id)
    {
      return (CxEntityUsageMetadata) m_EntityUsages[id.ToUpper()];
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Entity with the given ID.
    /// </summary>
    public CxEntityUsageMetadata this[string id]
    {
      get
      {
        CxEntityUsageMetadata entityUsage = Find(id);
        if (entityUsage != null)
          return entityUsage;
        else
          throw new ExMetadataException(string.Format("Entity usage with ID=\"{0}\" not defined", id));
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Entity usages dictionary.
    /// </summary>
    public Hashtable EntityUsages
    {
      get { return m_EntityUsages; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Entity usages list.
    /// </summary>
    public List<CxEntityUsageMetadata> Items
    {
      get { return m_Items; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Loads custom metadata objects from the given XML documents sorted by object IDs.
    /// </summary>
    public void LoadCustomMetadata(IDictionary<string, XmlDocument> documents)
    {
      foreach (KeyValuePair<string, XmlDocument> pair in documents)
      {
        CxEntityUsageMetadata entityUsageMetadata = Find(pair.Key);
        if (entityUsageMetadata != null)
        {
          foreach (XmlElement element in pair.Value.DocumentElement.SelectNodes("entity_usage_custom"))
          {
            entityUsageMetadata.LoadCustomMetadata(element);
          }
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns default name for the metadata XML file.
    /// </summary>
    override protected string XmlFileName
    { get { return "EntityUsages.xml"; } }
    //-------------------------------------------------------------------------
  }
}