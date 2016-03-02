/********************************************************************
 *  FulcrumWeb RAD Framework - Fulcrum of your business             *
 *  Copyright (c) 2002-2009 FulcrumWeb, ALL RIGHTS RESERVED         *
 *                                                                  *
 *  THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      *
 *  FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        *
 *  COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       *
 *  AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  *
 *  AND PERMISSION FROM FULCRUMWEB. CONSULT THE END USER LICENSE    *
 *  AGREEMENT FOR INFORMATION ON ADDITIONAL RESTRICTIONS.           *
 ********************************************************************/

using System;
using Framework.Entity;
using Framework.Metadata;
using Framework.Remote.Mobile;

namespace Framework.Remote
{
  public partial class CxAppServer
  {
    /// <summary>
    /// Returns entity by primary keys.
    /// </summary>
    /// <param name="attributeId">Id of attribute that need rowsource.</param>
    /// <param name="prms">Query parameters.</param>
    /// <returns>Initialized CxModel</returns>    
    public CxClientRowSource GetRowSource(string attributeId, CxQueryParams prms)
    {
      try
      {
        CxEntityUsageMetadata meta = m_Holder.EntityUsages[prms.EntityUsageId];
        CxBaseEntity entity = CxBaseEntity.CreateByValueProvider
                    (meta, CxQueryParams.CreateValueProvider(prms.EntityValues));
        CxEditController editController = new CxEditController(meta);
        return editController.GetDynamicRowSource(meta.GetAttribute(attributeId), entity);
      }
      catch (Exception ex)
      {
        CxClientRowSource rs = new CxClientRowSource();
        CxExceptionDetails exceptionDetails = new CxExceptionDetails(ex);
        rs.Error = exceptionDetails;
        return rs;
      }
    }
  }
}
