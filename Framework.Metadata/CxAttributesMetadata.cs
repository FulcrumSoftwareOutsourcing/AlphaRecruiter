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
using System.Collections.Generic;

using Framework.Utils;

namespace Framework.Metadata
{
	/// <summary>
  /// Class to read information about application entity attributes.
  /// </summary>
	public class CxAttributesMetadata : CxMetadataCollection
	{
    //----------------------------------------------------------------------------
    protected List<CxAttributeMetadata> m_Items = new List<CxAttributeMetadata>();
    //----------------------------------------------------------------------------

    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">metadata holder</param>
    /// <param name="doc">XML doc to read metadata from</param>
    public CxAttributesMetadata(CxMetadataHolder holder, XmlDocument doc) : 
      base(holder, doc)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">metadata holder</param>
    /// <param name="docs">name of file to read assemblies metadata</param>
    public CxAttributesMetadata(CxMetadataHolder holder, IEnumerable<XmlDocument> docs)
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
      
      // Load entities
      foreach (XmlElement entityElement in doc.DocumentElement.SelectNodes(GetEntityTagName()))
      {
        string entityId = CxXml.GetAttr(entityElement, "id");
        CxEntityMetadata entity = GetEntityMetadata(entityId);
        ReadEntityInfo(entityElement, entity);
        
        // Load entity attributes
        XmlElement attributesElement = (XmlElement) entityElement.SelectSingleNode(GetAttributesTagName());
        if (attributesElement != null)
        {
          foreach (XmlElement attributeElement in attributesElement.SelectNodes(GetAttributeTagName()))
          {
            try
            {
              CxAttributeMetadata attribute = CreateAttribute(attributeElement, entity);
              Add(entity, attribute);
            }
            catch (Exception ex)
            {
              string attributeId = CxXml.GetAttr(attributeElement, "id");
              throw new ExException(string.Format("Did not manage to create or add an attribute with such Id=<{0}>, Entity Id=<{1}>", attributeId, entityId), ex);
            }
          }
        }
      }

      // Load entity overrides
      foreach (XmlElement entityElement in doc.DocumentElement.SelectNodes(GetEntityTagName() + "_override"))
      {
        string entityId = CxXml.GetAttr(entityElement, "id");
        CxEntityMetadata entity = GetEntityMetadata(entityId);
        ReadEntityInfo(entityElement, entity);

        // Load overridden entity attributes
        XmlElement attributesElement = (XmlElement) entityElement.SelectSingleNode(GetAttributesTagName());
        if (attributesElement != null)
        {
          foreach (XmlElement attributeElement in attributesElement.SelectNodes(GetAttributeTagName()))
          {
            CxAttributeMetadata attribute = entity.GetAttribute(CxXml.GetAttr(attributeElement, "id"));
            if (attribute != null)
            {
              CxEntityUsageMetadata entityUsage = entity as CxEntityUsageMetadata;
              if (entityUsage != null)
                attribute = attribute.ApplyToEntityUsage(entityUsage);
              attribute.LoadOverride(attributeElement);
            }
            else
            {
              attribute = CreateAttribute(attributeElement, entity);
              Add(entity, attribute);
            }
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds attribute to the collection.
    /// </summary>
    public void Add(CxEntityMetadata entity, CxAttributeMetadata attribute)
    {
      entity.AddAttribute(attribute);
      m_Items.Add(attribute);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns name of XML tag for entity-level node.
    /// </summary>
    /// <returns>name of XML tag for entity-level node</returns>
    virtual protected string GetEntityTagName()
    {
      return "entity";
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns name of XML tag for attributes-level node.
    /// </summary>
    /// <returns>name of XML tag for attributes-level node</returns>
    virtual protected string GetAttributesTagName()
    {
      return "attributes";
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns name of XML tag for attribute-level node.
    /// </summary>
    /// <returns>name of XML tag for attribute-level node</returns>
    virtual protected string GetAttributeTagName()
    {
      return "attribute";
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns entity (or entity usage) by the given ID.
    /// </summary>
    /// <param name="id">ID of entity or entity usage</param>
    /// <returns>entity (or entity usage) by the given ID</returns>
    virtual protected CxEntityMetadata GetEntityMetadata(string id)
    {
      return Holder.Entities[id]; 
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads additional information about entity (if any).
    /// </summary>
    /// <param name="entityElement">XML element with entitu or entity usage information</param>
    /// <param name="entity">entity or entity usage</param>
    virtual protected void ReadEntityInfo(XmlElement entityElement, 
                                          CxEntityMetadata entity)
    {
      string gridVisibleOrder = CxXml.ReadTextElement(entityElement, "grid_visible_order");
      string queryOrder = CxXml.ReadTextElement(entityElement, "query_order");
      string editOrder = CxXml.ReadTextElement(entityElement, "edit_order");
      string filterOrder = CxXml.ReadTextElement(entityElement, "filter_order");

      entity.GetAttributeOrder(NxAttributeContext.GridVisible).SetXmlDefOrder(gridVisibleOrder);
      entity.GetAttributeOrder(NxAttributeContext.Queryable).SetXmlDefOrder(queryOrder);
      entity.GetAttributeOrder(NxAttributeContext.Edit).SetXmlDefOrder(editOrder);
      entity.GetAttributeOrder(NxAttributeContext.Filter).SetXmlDefOrder(filterOrder);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates attribute or attribute usage that belongs to the given entity or
    /// entity usage using information from XML element
    /// </summary>
    /// <param name="attributeElement">XML element attribute information stored under</param>
    /// <param name="entity">entity or entity usage this attribute belongs to</param>
    /// <returns>created attribute or attribute usage</returns>
    virtual protected CxAttributeMetadata CreateAttribute(XmlElement attributeElement, 
                                                          CxEntityMetadata entity)
    {
      return new CxAttributeMetadata(attributeElement, entity);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns list of all attribute metadata.
    /// </summary>
    public IList<CxAttributeMetadata> Items
    { get { return m_Items; } }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns default name for the metadata XML file.
    /// </summary>
    override protected string XmlFileName
    { get { return "Attributes.xml"; } }
    //-------------------------------------------------------------------------
  }
}