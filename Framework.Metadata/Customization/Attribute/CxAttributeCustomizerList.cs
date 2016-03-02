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
using Framework.Utils;

namespace Framework.Metadata
{
  public class CxAttributeCustomizerList: List<CxAttributeCustomizer>
  {
    private Dictionary<string, CxAttributeCustomizer> m_AttributeByTextOrValueId_Cache;
    //-------------------------------------------------------------------------
    public CxAttributeCustomizerList()
    {
      m_AttributeByTextOrValueId_Cache = new Dictionary<string, CxAttributeCustomizer>();
    }
    //-------------------------------------------------------------------------
    public new void Add(CxAttributeCustomizer customizer)
    {
      base.Add(customizer);
      if (m_AttributeByTextOrValueId_Cache.ContainsKey(customizer.Id))
        throw new ExException("The collection already contains an attribute customizer with the same ID");
      m_AttributeByTextOrValueId_Cache.Add(customizer.Id, customizer);
    }
    //-------------------------------------------------------------------------
    public CxAttributeCustomizer FindById(string id)
    {
      foreach (CxAttributeCustomizer attributeCustomizer in this)
      {
        if (string.Equals(attributeCustomizer.Id, id, StringComparison.OrdinalIgnoreCase))
          return attributeCustomizer;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    public CxAttributeCustomizer FindByValueOrTextId(string id)
    {
      if (m_AttributeByTextOrValueId_Cache.ContainsKey(id))
        return m_AttributeByTextOrValueId_Cache[id];

      foreach (CxAttributeCustomizer attributeCustomizer in this)
      {
        if (CxText.Equals(attributeCustomizer.Id, id) ||
            CxText.Equals(attributeCustomizer.TextId, id))
          return m_AttributeByTextOrValueId_Cache[id] = attributeCustomizer;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    public CxAttributeCustomizerList GetSublistBy(
      IList<CxAttributeMetadata> attributeMetadatas)
    {
      IList<string> ids = CxMetadataObject.ExtractIds(attributeMetadatas);
      return GetSublistBy(ids);
    }
    //-------------------------------------------------------------------------
    public CxAttributeCustomizerList GetSublistBy(
      IList<string> attributeIds)
    {
      CxAttributeCustomizerList result = new CxAttributeCustomizerList();
      foreach (string attributeId in attributeIds)
      {
        CxAttributeCustomizer customizer = FindByValueOrTextId(attributeId);
        if (customizer != null && !result.Contains(customizer))
          result.Add(customizer);
      }
      return result;
    }
    //-------------------------------------------------------------------------
  }
}
