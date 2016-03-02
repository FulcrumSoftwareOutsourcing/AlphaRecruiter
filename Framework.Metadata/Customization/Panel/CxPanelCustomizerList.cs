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
  public class CxPanelCustomizerList: List<CxPanelCustomizer>
  {
    //-------------------------------------------------------------------------
    public CxPanelCustomizer FindById(string id)
    {
      foreach (CxPanelCustomizer panelCustomizer in this)
      {
        if (string.Equals(panelCustomizer.Metadata.Id, id, StringComparison.OrdinalIgnoreCase))
          return panelCustomizer;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    public CxPanelCustomizerList GetSublistBy(
      IList<CxWinPanelMetadata> panelMetadatas)
    {
      IList<string> ids = CxMetadataObject.ExtractIds(panelMetadatas);
      return GetSublistBy(ids);
    }
    //-------------------------------------------------------------------------
    public CxPanelCustomizerList GetSublistBy(
      IList<string> panelIds)
    {
      CxPanelCustomizerList result = new CxPanelCustomizerList();
      foreach (string panelId in panelIds)
      {
        CxPanelCustomizer customizer = FindById(panelId);
        if (customizer != null)
          result.Add(customizer);
      }
      return result;
    }
    //-------------------------------------------------------------------------
  }
}
