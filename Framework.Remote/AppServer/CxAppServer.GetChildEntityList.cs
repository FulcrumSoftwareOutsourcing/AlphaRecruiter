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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Framework.Db;
using Framework.Entity;
using Framework.Entity.Filter;
using Framework.Metadata;
using Framework.Remote.Mobile;
using Framework.Utils;

namespace Framework.Remote
{
  public partial class CxAppServer
  {
    /// <summary>
    /// Returns list of child entities.
    /// </summary>
    /// <param name="marker">Server operation identifier.</param>
    /// <param name="prms">Query parameters.</param>
    /// <returns>Initialized CxModel</returns>
    public CxModel GetChildEntityList(Guid marker, CxQueryParams prms)
    {
      try
      {
        CxEntityUsageMetadata entityUsage = m_Holder.EntityUsages[prms.EntityUsageId];
        CxEntityUsageMetadata parentEntityUsage = null;
        if (prms.ParentEntityUsageId != null)
        {
          parentEntityUsage = m_Holder.EntityUsages[prms.ParentEntityUsageId];
        }


        CxModel model = new CxModel(marker);
        using (CxDbConnection connection = CxDbConnections.CreateEntityConnection())
        {

          IList<string> joinParamsNames = CxDbParamParser.GetList(entityUsage.JoinCondition, true);

          IxValueProvider paramsProvider =
            CxValueProviderCollection.Create(
              CxQueryParams.CreateValueProvider(prms.JoinValues),
              m_Holder.ApplicationValueProvider);



          IList<string> whereParamsMames =
          CxDbParamParser.GetList(entityUsage.WhereClause, true);

          string parentWhere = string.Empty;

          if (parentEntityUsage != null)
          {
            CxParentEntityMetadata parentEntityMetadata =
             parentEntityUsage.Entity.ParentEntities.FirstOrDefault(pe => pe.Entity.Id == entityUsage.EntityId);
            //if (parentEntityMetadata == null)
            //{
            //  parentEntityMetadata =
            //    entityUsage.ParentEntities.FirstOrDefault(pe => pe.Entity.Id == parentEntityUsage.Id);
            //}
            if (parentEntityMetadata != null)
              parentWhere = parentEntityMetadata.WhereClause;
          }


          foreach (CxFilterItem filterItem in prms.FilterItems)
          {
            filterItem.Operation = (NxFilterOperation)Enum.Parse(typeof(NxFilterOperation), filterItem.OperationAsString);
            CxFilterOperator filterOperator
                = CxFilterOperator.Create(entityUsage, filterItem);
            if (filterOperator != null)
            {
              filterOperator.InitializeValueProvider(paramsProvider);
            }

          }
          string filterCondition = GetFilterCondition(entityUsage, prms.FilterItems.ToList<IxFilterElement>());

          // Indicates whether we should perform an additional query to the 
          // database to obtain the overall amount of records.
          bool doPerformQueryForCount = entityUsage.IsPagingEnabled && (prms.StartRecordIndex >= 1 || prms.RecordsAmount != -1);

          int totalEntityAmount = -1;
          if (doPerformQueryForCount)
          {
            totalEntityAmount = CxBaseEntity.ReadChildEntityAmount(
              connection, entityUsage, 
              GetWhereCondition(filterCondition, entityUsage.WhereClause, parentWhere), paramsProvider);
          }

          DataTable tblChild = new DataTable();
          entityUsage.ReadChildData(
            connection, tblChild,
            paramsProvider, GetWhereCondition(filterCondition, entityUsage.WhereClause, parentWhere),
            NxEntityDataCache.NoCache,
            prms.StartRecordIndex, prms.RecordsAmount);

          ArrayList childList = (ArrayList)CxBaseEntity.GetEntityListFromTable(entityUsage, tblChild);
          CxBaseEntity[] entities = (CxBaseEntity[])childList.ToArray(typeof(CxBaseEntity));

          if (!doPerformQueryForCount)
          {
            totalEntityAmount = entities.Length;
          }

          Dictionary<string, CxClientRowSource> unfilteredRowSources = new Dictionary<string, CxClientRowSource>();
          List<CxClientRowSource> filteredRowSources = new List<CxClientRowSource>();

          model = new CxModel
          {
            Marker = marker,
            EntityUsageId = entityUsage.Id,
            UnfilteredRowSources = unfilteredRowSources,
            FilteredRowSources = filteredRowSources,
            TotalDataRecordAmount = totalEntityAmount
          };
          if(prms.SortDescriptions != null)
          {
            model.SortDescriptions = (new List<CxSortDescription>(prms.SortDescriptions)).ToArray();
          }
          model.SetData(entityUsage, entities, connection);

          UpdateRecentItems(connection, model);
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
