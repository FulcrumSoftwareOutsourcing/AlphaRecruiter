using System;
using System.Collections.Generic;
using System.Text;
using Framework.Utils;

namespace Framework.Metadata
{
  public class CxLookupUsage
  {
    //-------------------------------------------------------------------------
    private CxAttributeMetadata m_Attribute;
    private CxEntityUsageMetadata m_Entity;
    //-------------------------------------------------------------------------
    /// <summary>
    /// The id of the entity.
    /// </summary>
    public string EntityId
    {
      get { return Entity != null ? Entity.Id : string.Empty; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Caption of the attribute.
    /// </summary>
    public string AttributeCaption
    {
      get { return Attribute != null ? Attribute.GetCaption(Entity) : string.Empty; }
    }
    //-------------------------------------------------------------------------
    public CxAttributeMetadata Attribute
    {
      get { return m_Attribute; }
      protected set { m_Attribute = value; }
    }
    //-------------------------------------------------------------------------
    public CxEntityUsageMetadata Entity
    {
      get { return m_Entity; }
      protected set { m_Entity = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="attribute"></param>
    public CxLookupUsage(CxEntityUsageMetadata entity, CxAttributeMetadata attribute)
    {
      Entity = entity;
      Attribute = attribute;
    }
    //-------------------------------------------------------------------------
  }
}
