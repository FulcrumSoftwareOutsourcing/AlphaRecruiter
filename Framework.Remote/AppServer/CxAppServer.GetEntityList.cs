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
using System.ComponentModel;
using System.Linq;
using System.Text;
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
    /// Returns list of entities.
    /// </summary>
    /// <param name="marker">Server operation identifier.</param>
    /// <param name="prms">Query parameters.</param>
    /// <returns>Initialized CxModel</returns>
    public CxModel GetEntityList(Guid marker, CxQueryParams prms)
    {
      try
      {

        

        CxEntityUsageMetadata entityUsage = m_Holder.EntityUsages[prms.EntityUsageId];
        CxEntityUsageMetadata parentEntityUsage = null;
        if (prms.ParentEntityUsageId != null)
        {
          parentEntityUsage = m_Holder.EntityUsages[prms.ParentEntityUsageId];
        }

        //---------------------------------------------------------------------------------------
        //  If there is no custom sorting then predefined sorting from attribute-metadata is used
        if (prms.SortDescriptions != null && prms.SortDescriptions.Count() == 0)
        {
          if (entityUsage.SortAttributes.Count() > 0)
          {
            CxSortDescription[] strArray = new CxSortDescription[entityUsage.SortAttributes.Count()];

            for (int i = 0; i < entityUsage.SortAttributes.Count(); i++)
            {
              CxAttributeMetadata attr = entityUsage.SortAttributes[i];

              strArray[i] = new CxSortDescription
                              {
                                AttributeId = attr.Id,
                                Direction =
                                  attr.Sorting == NxSortingDirection.Desc
                                    ? NxListSortDirection.Descending
                                    : attr.Sorting == NxSortingDirection.Asc ? NxListSortDirection.Ascending : NxListSortDirection.None
                              };
            }
            prms.SortDescriptions = (new List<CxSortDescription>(strArray));
          }
        }

        CxModel model = new CxModel(marker);
          using (CxDbConnection connection = CxDbConnections.CreateEntityConnection())
          {

            IList<string> whereParamsMames =
              CxDbParamParser.GetList(entityUsage.WhereClause, true);

            string parentWhere = string.Empty;

            if (parentEntityUsage != null)
            {
              CxParentEntityMetadata parentEntityMetadata =
                parentEntityUsage.Entity.ParentEntities.FirstOrDefault(pe => pe.Entity.Id == entityUsage.EntityId);
              //if(parentEntityMetadata == null)
              //{
              //  parentEntityMetadata =
              //    entityUsage.ParentEntities.FirstOrDefault(pe => pe.Entity.Id == parentEntityUsage.Id);
              //}
              if (parentEntityMetadata != null)
                parentWhere = parentEntityMetadata.WhereClause;
            }




            // Obtaining the parent entity.
            CxBaseEntity parent = null;
            if (parentEntityUsage != null)
            {
              IList<string> parentPkNames = parentEntityUsage.PrimaryKeyIds;
              IxValueProvider parentVlProvider =
                CxQueryParams.CreateValueProvider(prms.ParentPks);
              parent = CxBaseEntity.CreateAndReadFromDb
                (parentEntityUsage,
                 connection,
                 parentVlProvider);
            }

            IxValueProvider paramsProvider =
              CxValueProviderCollection.Create(
                parent,
                CxQueryParams.CreateValueProvider(prms.WhereValues),
                m_Holder.ApplicationValueProvider);

            foreach (CxFilterItem filterItem in prms.FilterItems)
            {
              filterItem.Operation =
                (NxFilterOperation) Enum.Parse(typeof (NxFilterOperation), filterItem.OperationAsString);
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
            bool doPerformQueryForCount = entityUsage.IsPagingEnabled &&
                                          (prms.StartRecordIndex >= 1 || prms.RecordsAmount != -1);

            int totalEntityAmount = -1;
            if (doPerformQueryForCount)
            {
              totalEntityAmount = CxBaseEntity.ReadEntityAmount(
                connection, entityUsage, GetWhereCondition(filterCondition, entityUsage.WhereClause, parentWhere),
                paramsProvider);
            }

            // Composing the list of sort descriptors from the input params.
            CxSortDescriptorList sortings = new CxSortDescriptorList();
            if (prms.SortDescriptions != null)
            {
              foreach (CxSortDescription sorting in prms.SortDescriptions)
              {
                sortings.Add(new CxSortDescriptor(sorting.AttributeId,
                                                  sorting.Direction == NxListSortDirection.Ascending
                                                    ? ListSortDirection.Ascending
                                                    : ListSortDirection.Descending));
              }
            }

            if (!entityUsage.IsPagingEnabled)
            {
              prms.StartRecordIndex = -1;
              prms.RecordsAmount = -1;
            }



            if (!entityUsage.IsFilterConditionAutoGenerated)
            {
              filterCondition = string.Empty;
            }

            // Query to the backend.
            CxBaseEntity[] entities = CxBaseEntity.ReadEntities(
              connection, entityUsage, GetWhereCondition(filterCondition, entityUsage.WhereClause, parentWhere),
              paramsProvider,
              prms.StartRecordIndex, prms.RecordsAmount, sortings);



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
                        TotalDataRecordAmount = totalEntityAmount,
                      };
            if (prms.SortDescriptions != null && prms.SortDescriptions.Count() > 0)
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
