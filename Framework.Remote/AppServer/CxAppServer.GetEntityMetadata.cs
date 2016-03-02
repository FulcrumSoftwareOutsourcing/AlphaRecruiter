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
using Framework.Metadata;
using Framework.Remote.Mobile;

namespace Framework.Remote
{
  public partial class CxAppServer
  {
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns CxClientEntityMetadata by entity usage Id.
    /// </summary>
    /// <param name="entityUsageId">Entity Usage Id.</param>
    /// <returns>CxClientEntityMetadata</returns>
    public CxClientEntityMetadata GetEntityMetadata(string entityUsageId)
    {
      try
      {
        CxEntityUsageMetadata meta = m_Holder.EntityUsages[entityUsageId];

        CxClientEntityMetadata entityMetadata = new CxClientEntityMetadata(m_Holder, meta, null, null);
        InitApplicationValues(entityMetadata.ApplicationValues);
        return entityMetadata;
      }
      catch (Exception ex)
      {
        CxClientEntityMetadata entityMetadata = new CxClientEntityMetadata();
        CxExceptionDetails exceptionDetails = new CxExceptionDetails(ex);
        entityMetadata.Error = exceptionDetails;
        return entityMetadata;
      }

    }
  }
}
