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

using System.Collections.Generic;
using System.Xml;

using Framework.Utils;

namespace Framework.Metadata
{
  /// <summary>
  /// Class to read information about entity attribute usages.
  /// </summary>
  public class CxAttributeUsagesMetadata : CxAttributesMetadata
	{
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">metadata holder</param>
    /// <param name="doc">XML doc to read metadata from</param>
    public CxAttributeUsagesMetadata(CxMetadataHolder holder, XmlDocument doc) 
      : base(holder, doc)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">metadata holder</param>
    /// <param name="docs">name of file to read assemblies metadata</param>
    public CxAttributeUsagesMetadata(CxMetadataHolder holder, IEnumerable<XmlDocument> docs)
      : base(holder, docs)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns name of XML tag for entity-level node.
    /// </summary>
    /// <returns>name of XML tag for entity-level node</returns>
    override protected string GetEntityTagName()
    {
      return "entity_usage";
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns name of XML tag for attributes-level node.
    /// </summary>
    /// <returns>name of XML tag for attributes-level node</returns>
    override protected string GetAttributesTagName()
    {
      return "attribute_usages";
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns name of XML tag for attribute-level node.
    /// </summary>
    /// <returns>name of XML tag for attribute-level node</returns>
    override protected string GetAttributeTagName()
    {
      return "attribute_usage";
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns entity (or entity usage) by the given ID.
    /// </summary>
    /// <param name="id">ID of entity or entity usage</param>
    /// <returns>entity (or entity usage) by the given ID</returns>
    override protected CxEntityMetadata GetEntityMetadata(string id)
    {
      return Holder.EntityUsages[id]; 
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads additional information about entity (if any).
    /// </summary>
    /// <param name="entityElement">XML element with entity or entity usage information</param>
    /// <param name="entity">entity or entity usage</param>
    override protected void ReadEntityInfo(XmlElement entityElement, 
                                           CxEntityMetadata entity)
    {
      base.ReadEntityInfo(entityElement, entity);

      if (entity is CxEntityUsageMetadata)
      {
        CxEntityUsageMetadata entityUsage = (CxEntityUsageMetadata) entity;
        string gridOrder = CxXml.ReadTextElement(entityElement, "grid_order");
        bool hasGridOrder = entityElement.SelectSingleNode("grid_order") != null;
        // This entity usage properties are overwritten if corresponding node is present.
        entityUsage.SetAttributeOrder(gridOrder, hasGridOrder);
        // Set properties to all descendant entity usages (with inherited entity usage ID).
        // Descendant entity usage properties are never overwritten if already present.
        foreach (CxEntityUsageMetadata descendant in entityUsage.DescendantEntityUsages)
        {
          descendant.SetAttributeOrder(gridOrder, false);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads the custom metadata from the given dictionary of XML documents sorted by
    /// attribute usage IDs.
    /// </summary>
    /// <param name="documents">documents containing the custom metadata</param>
    public void LoadCustomMetadata(IDictionary<string, XmlDocument> documents)
    {
      foreach (KeyValuePair<string, XmlDocument> pair in documents)
      {
        // Load entity customization
        foreach (XmlElement entityElement in pair.Value.DocumentElement.SelectNodes(GetEntityTagName() + "_custom"))
        {
          CxEntityUsageMetadata entity = GetEntityMetadata(pair.Key) as CxEntityUsageMetadata;
          ReadEntityInfo(entityElement, entity);

          // Load overridden entity attributes
          XmlElement attributesElement = (XmlElement) entityElement.SelectSingleNode("attribute_usages");
          if (attributesElement != null)
          {
            foreach (XmlElement attributeElement in attributesElement.SelectNodes("attribute_usage"))
            {
              CxAttributeMetadata attribute = entity.GetAttribute(CxXml.GetAttr(attributeElement, "id"));
              if (attribute != null)
              {
                attribute = attribute.ApplyToEntityUsage(entity);
                attribute.LoadCustomMetadata(attributeElement);
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
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates attribute or attribute usage that belongs to the given entity or
    /// entity usage using information from XML element
    /// </summary>
    /// <param name="attributeElement">XML element attribute information stored under</param>
    /// <param name="entity">entity or entity usage this attribute belongs to</param>
    /// <returns>created attribute or attribute usage</returns>
    override protected CxAttributeMetadata CreateAttribute(XmlElement attributeElement, 
                                                           CxEntityMetadata entity)
    {
      return new CxAttributeUsageMetadata(attributeElement, entity);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns default name for the metadata XML file.
    /// </summary>
    override protected string XmlFileName
    { get { return "AttributeUsages.xml"; } }
    //-------------------------------------------------------------------------
  }
}