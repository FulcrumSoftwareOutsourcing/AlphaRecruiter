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
using System.Collections.Specialized;
using System.Linq;
using System.Text;
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
    /// Returns entity by primary keys.
    /// </summary>
    /// <param name="marker">Server operation identifier.</param>
    /// <param name="prms">Query parameters.</param>
    /// <returns>Initialized CxModel</returns>   
    public CxModel GetEntityFromPk(Guid marker, CxQueryParams prms)
    {
      try
      {
      
        CxEntityUsageMetadata entityUsage = m_Holder.EntityUsages[prms.EntityUsageId];

        CxModel model = new CxModel(marker);
        using (CxDbConnection conn = CxDbConnections.CreateEntityConnection())
        {
          // Recognizing if the request requires to create a new entity.
          bool createNew = false;
          foreach (KeyValuePair<string, object> pkPair in prms.PrimaryKeysValues)
          {
            if (pkPair.Value is int && (int)pkPair.Value == int.MinValue)
            {
              createNew = true;
              break;
            }
          }

          // Obtaining the parent entity.
          CxBaseEntity parent = null;
          if (prms.ParentEntityUsageId != null)
          {
            CxEntityUsageMetadata parentEntityUsage = m_Holder.EntityUsages[prms.ParentEntityUsageId];
            IList<string> parentPkNames = parentEntityUsage.PrimaryKeyIds;
            IxValueProvider parentVlProvider =
              CxQueryParams.CreateValueProvider(prms.ParentPks);
            parent = CxBaseEntity.CreateAndReadFromDb
              (parentEntityUsage,
               conn,
               parentVlProvider);
          }

          // Obtaining the value provider.
          IList<string> pkNames = entityUsage.PrimaryKeyIds;

                  
          // Obtaining the entity.
          CxBaseEntity entityFromPk;
          if (!createNew)
          {
            IxValueProvider paramsProvider;
            if (prms.EntityValues != null && prms.EntityValues.Count > 0)
            {
              paramsProvider =
                CxValueProviderCollection.Create(
                  CxQueryParams.CreateValueProvider(prms.EntityValues),
                  m_Holder.ApplicationValueProvider);
            }
            else
            {
              paramsProvider =
                CxValueProviderCollection.Create(
                  CxQueryParams.CreateValueProvider(prms.PrimaryKeysValues),
                  m_Holder.ApplicationValueProvider);
            }

            entityFromPk = CxBaseEntity.CreateAndReadFromDb(entityUsage, conn, paramsProvider);
          }
          else
          {
            entityFromPk = CxBaseEntity.CreateWithDefaults(entityUsage, parent, conn);
          }
          CxBaseEntity[] entities = entityFromPk == null ? new CxBaseEntity[0] :
              new[] { entityFromPk };

          Dictionary<string, CxClientRowSource> unfilteredRowSources;
          List<CxClientRowSource> filteredRowSources;
          GetDynamicRowSources(out unfilteredRowSources, out filteredRowSources, entities, entityUsage);

          model = new CxModel
          {
            Marker = marker,
            EntityUsageId = entityUsage.Id,
            UnfilteredRowSources = unfilteredRowSources,
            FilteredRowSources = filteredRowSources,
            TotalDataRecordAmount = entities.Length,
            IsNewEntity = createNew
          };
          model.SetData(entityUsage, entities, conn);

          model.EntityMarks = new CxClientEntityMarks();
          if (!createNew)
          {
            UpdateRecentItems(conn, model);
            CxAppServerContext context = new CxAppServerContext();
            CxEntityMark alreadyPresents = context.EntityMarks.Find(entityFromPk, NxEntityMarkType.Recent,
              context["APPLICATION$APPLICATIONCODE"].ToString());
            if (alreadyPresents != null)
            {
              context.EntityMarks.RecentItems.Remove(alreadyPresents);
              model.EntityMarks.RemovedRecentItems.Add(new CxClientEntityMark(alreadyPresents));
            }
            context.EntityMarks.AddMark(entityFromPk, NxEntityMarkType.Open, true, prms.OpenMode,
              context["APPLICATION$APPLICATIONCODE"].ToString());
            model.EntityMarks.AllRecentItems.Clear();
            foreach (CxEntityMark recentItem in context.EntityMarks.RecentItems)
            {
              model.EntityMarks.AllRecentItems.Add(new CxClientEntityMark(recentItem));
            }
          }
          else
          {
            CxAppServerContext context = new CxAppServerContext();
            model.EntityMarks.AllRecentItems.Clear();
            foreach (CxEntityMark recentItem in context.EntityMarks.RecentItems)
            {
              model.EntityMarks.AllRecentItems.Add(new CxClientEntityMark(recentItem));
            }
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

    //----------------------------------------------------------------------------
    public void GetDynamicRowSources
      (out Dictionary<string, CxClientRowSource> unfilteredRowSources,
      out List<CxClientRowSource> filteredRowSources,
      CxBaseEntity[] entities,
      CxEntityUsageMetadata meta

      )
    {
      unfilteredRowSources = new Dictionary<string, CxClientRowSource>();
      filteredRowSources = new List<CxClientRowSource>();

      using (CxDbConnection connection = CxDbConnections.CreateEntityConnection())
      {
        foreach (CxBaseEntity entity in entities)
        {
          foreach (CxAttributeMetadata attributeMetadata in meta.Attributes)
          {
            if ((meta.GetAttributeOrder(NxAttributeContext.Edit).OrderAttributes.Contains(attributeMetadata) ||
                 meta.GetAttributeOrder(NxAttributeContext.GridVisible).OrderAttributes.Contains(attributeMetadata)) &&
                 attributeMetadata.RowSource != null &&
                 !string.IsNullOrEmpty(attributeMetadata.RowSource.EntityUsageId))
            {

              if (unfilteredRowSources.ContainsKey(attributeMetadata.RowSourceId.ToUpper()))
              {
                continue;
              }

              IList<CxComboItem> items = attributeMetadata.RowSource.GetList(
                null,
                connection,
                attributeMetadata.RowSourceFilter,
                entity,
                !CxEditController.GetIsMandatory(attributeMetadata, entity));

              CxClientRowSource rs = new CxClientRowSource(items, entity, attributeMetadata);
              if (rs.IsFilteredRowSource)
              {
                filteredRowSources.Add(rs);
              }
              else
              {
                unfilteredRowSources.Add(rs.RowSourceId.ToUpper(), rs);
              }

            }
          }

        }
      }
    }
  }
}
