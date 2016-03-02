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

namespace Framework.Metadata
{
  public class CxTabCustomizerList: List<CxTabCustomizer>
  {
    //-------------------------------------------------------------------------
    public CxTabCustomizer FindById(string id)
    {
      foreach (CxTabCustomizer tabCustomizer in this)
      {
        if (string.Equals(tabCustomizer.Metadata.Id, id, StringComparison.OrdinalIgnoreCase))
          return tabCustomizer;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    public CxTabCustomizerList GetSublistBy(
      IList<CxWinTabMetadata> tabMetadatas)
    {
      IList<string> ids = CxMetadataObject.ExtractIds(tabMetadatas);
      return GetSublistBy(ids);
    }
    //-------------------------------------------------------------------------
    public CxTabCustomizerList GetSublistBy(
      IList<string> tabIds)
    {
      CxTabCustomizerList result = new CxTabCustomizerList();
      foreach (string tabId in tabIds)
      {
        CxTabCustomizer customizer = FindById(tabId);
        if (customizer != null)
          result.Add(customizer);
      }
      return result;
    }
    //-------------------------------------------------------------------------
  }
}
