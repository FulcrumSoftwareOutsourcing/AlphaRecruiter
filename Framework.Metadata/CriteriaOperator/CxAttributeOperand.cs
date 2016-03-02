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
using System.Collections.Generic;
using System.Text;

using Framework.Db;
using Framework.Metadata;
using Framework.Utils;

namespace Framework.Metadata
{
  public class CxAttributeOperand: CxPropertyOperand
  {
    //-------------------------------------------------------------------------
    private CxAttributeMetadata m_AttributeMetadata;
    private CxEntityUsageMetadata m_EntityUsageContext;
    //-------------------------------------------------------------------------
    public CxAttributeMetadata AttributeMetadata
    {
      get { return m_AttributeMetadata; }
      set { m_AttributeMetadata = value; }
    }
    //-------------------------------------------------------------------------
    public override string PropertyName
    {
      get
      {
        return EntityUsageContext.Id + "." + AttributeMetadata.Id;
      }
      set
      {
        throw new ExException(
          "Cannot set the PropertyName property directly. Use the Attribute property setter");
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The Entity Usage context of the current attribute.
    /// </summary>
    public CxEntityUsageMetadata EntityUsageContext
    {
      get { return m_EntityUsageContext; }
      set { m_EntityUsageContext = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Default ctor.
    /// </summary>
    /// <param name="entityUsageContext">the context of the entity usage the attribute is used with</param>
    /// <param name="attributeMetadata">the attribute metadata to initialize the operand with</param>
    public CxAttributeOperand(CxEntityUsageMetadata entityUsageContext, CxAttributeMetadata attributeMetadata)
    {
      if (entityUsageContext == null)
        throw new ExNullArgumentException("entityUsageContext");
      if (attributeMetadata == null)
        throw new ExNullArgumentException("attributeMetadata");
      EntityUsageContext = entityUsageContext;
      AttributeMetadata = attributeMetadata;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public override string ToString()
    {
      if (EntityUsageContext != null && AttributeMetadata != null)
        return EntityUsageContext.Id + "." + AttributeMetadata.Id;
      return base.ToString();
    }

    //-------------------------------------------------------------------------
  }
}
