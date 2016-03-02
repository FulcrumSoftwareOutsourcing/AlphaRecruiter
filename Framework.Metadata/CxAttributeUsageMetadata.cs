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

using System.Xml;
using System.Collections.Generic;

namespace Framework.Metadata
{
  /// <summary>
  /// Class to hold information about attribute usage.
  /// </summary>
  public class CxAttributeUsageMetadata : CxAttributeMetadata
	{
    internal const string OBJECT_TYPE_ATTRIBUTE_USAGE = "Metadata.AttributeUsage";
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="element">XML element that holds metadata</param>
    /// <param name="entity">entity this attribute belongs to</param>
    public CxAttributeUsageMetadata(XmlElement element, CxEntityMetadata entity) : 
      base(element, entity)
    {
      InheritPropertiesFrom(InheritanceList);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns attribute this attribute usage "overrides" or this attribute usage object.
    /// </summary>
    public CxAttributeMetadata Attribute 
    {
      get { return EntityMetadata.GetAttribute(Id); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Attribute usage this attribute usage "overrides".
    /// </summary>
    public CxAttributeMetadata AttributeUsage 
    {
      get 
      { 
        if (EntityMetadata is CxEntityUsageMetadata)
        {
          CxEntityUsageMetadata entityUsage = (CxEntityUsageMetadata) EntityMetadata;
          if (entityUsage.InheritedEntityUsage != null)
          {
            return entityUsage.InheritedEntityUsage.GetAttribute(Id);
          }
        }
        return null; 
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns object type code for localization.
    /// </summary>
    override public string LocalizationObjectTypeCode
    {
      get
      {
        return OBJECT_TYPE_ATTRIBUTE_USAGE;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns list of metadata objects properties was inherited from.
    /// </summary>
    override public IList<CxMetadataObject> InheritanceList
    {
      get
      {
        List<CxMetadataObject> list = new List<CxMetadataObject>();
        if (AttributeUsage != null)
        {
          list.Add(AttributeUsage);
        }
        if (Attribute != null && Attribute != this)
        {
          list.Add(Attribute);
        }
        return list;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns the XML tag name applicable for the current metadata object.
    /// </summary>
    public override string GetTagName()
    {
      return "attribute_usage";
    }
    //----------------------------------------------------------------------------
  }
}
