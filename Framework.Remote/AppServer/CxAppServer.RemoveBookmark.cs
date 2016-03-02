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
using System.Linq;
using System.Text;
using Framework.Db;
using Framework.Entity;
using Framework.Remote.Mobile;

namespace Framework.Remote
{
  public partial class CxAppServer
  {
    /// <summary>
    /// Removes bookmark.
    /// </summary>
    ///<param name="uniqueId">Id of removed bookmark.</param>
    /// <returns>Initialized CxModel</returns>
    public CxModel RemoveBookmark(string uniqueId)
    {
      try
      {
        CxAppServerContext context = new CxAppServerContext();
        CxEntityMark toRemove =
          context.EntityMarks.BookmarkItems.FirstOrDefault(mark => mark.UniqueId == uniqueId);
        if (toRemove != null)
        {
          context.EntityMarks.DeleteMark(toRemove);
          using (CxDbConnection conn = CxDbConnections.CreateEntityConnection())
          {
            context.EntityMarks.SaveAndReload(conn, m_Holder);
          }
        }
        CxModel model = new CxModel { EntityMarks = new CxClientEntityMarks() };
        model.EntityMarks.RemovedBookmarkItems.Add(new CxClientEntityMark(toRemove));
        return model;
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
