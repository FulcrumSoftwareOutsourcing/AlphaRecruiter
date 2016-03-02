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
using System.Collections.Generic;
using Framework.Db;
using Framework.Entity;
using Framework.Remote.Mobile;

namespace Framework.Remote
{
  public partial class CxAppServer
  {
    /// <summary>
    /// Clears all history.
    /// </summary>
    /// <returns>Initialized CxModel</returns>
    public CxModel ClearHistory()
    {
      try
      {
        CxAppServerContext context = new CxAppServerContext();
        List<CxEntityMark> toRemove = new List<CxEntityMark>();
        toRemove.AddRange(context.EntityMarks.RecentItems);
        foreach (CxEntityMark recentItem in toRemove)
        {
          context.EntityMarks.DeleteMark(recentItem);
        }
        using (CxDbConnection conn = CxDbConnections.CreateEntityConnection())
        {
          context.EntityMarks.SaveAndReload(conn, m_Holder);
        }
        return new CxModel();
      }
      catch (Exception ex)
      {
        CxExceptionDetails exceptionDetails = new CxExceptionDetails(ex);
        CxModel model = new CxModel { Error = exceptionDetails };
        return model;
      }
    }
  }
}
