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
using Framework.Metadata;
using Framework.Remote.Mobile;
using Framework.Utils;

namespace Framework.Remote
{

  public partial class CxAppServer
  {
    /// <summary>
    /// Adds entity to bookmarks.
    /// </summary>
    /// <param name="entityUsageId">Entity usage Id.</param>
    /// <param name="pkValues">Entity primary keys values.</param>
    /// <param name="openMode">Entity open mode.</param>
    /// <returns>Initialized CxModel</returns>
    public CxModel AddToBookmarks(string entityUsageId, Dictionary<string, object> pkValues, string openMode)
    {
      try
      {
        CxEntityUsageMetadata entityUsage = m_Holder.EntityUsages[entityUsageId];

        CxModel model = new CxModel();
        using (CxDbConnection conn = CxDbConnections.CreateEntityConnection())
        {
          // Obtaining the value provider.
          IList<string> pkNames = entityUsage.PrimaryKeyIds;

          IxValueProvider paramsProvider =
            CxValueProviderCollection.Create(
              CxQueryParams.CreateValueProvider(pkValues),
              m_Holder.ApplicationValueProvider);

          // Obtaining the entity.
          CxBaseEntity entityFromPk;
          entityFromPk = CxBaseEntity.CreateAndReadFromDb(entityUsage, conn, paramsProvider);

          //add new bookmark
          CxAppServerContext context = new CxAppServerContext();
          CxClientEntityMarks marks = new CxClientEntityMarks();
          model.EntityMarks = marks;

          if (context.EntityMarks.AddMark(
                entityFromPk,
                NxEntityMarkType.Bookmark,
                true,
                openMode,
                context["APPLICATION$APPLICATIONCODE"].ToString()))
          {
            context.EntityMarks.Save(conn);

            CxEntityMark addedMark = context.EntityMarks.BookmarkItems[0];
            marks.AddedBookmarkItems.Add(new CxClientEntityMark(addedMark));
          }
        }

        InitApplicationValues(model.ApplicationValues);
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
