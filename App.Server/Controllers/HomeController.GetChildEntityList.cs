using App.Server.Models.Markup;
using App.Server.Models.Settings;
using Framework.Db;
using Framework.Entity;
using Framework.Entity.Filter;
using Framework.Metadata;
using Framework.Remote;
using Framework.Remote.Mobile;
using Framework.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace App.Server.Controllers
{
    public partial class HomeController
    {

        [Authorize]
        public ActionResult GetChildEntityList(string entityUsageId,
            bool clientEntityUsagesRequired,
            IEnumerable<string> requiredTemplates,
             string joins,
                string whereVals,
                string pkVals,
                string filters,
                string sorts,
                string parentPrimaryKeys,
                string parentEntityUsageId,
                string changedAttributeId,
                int startRecordIndex,
                int recordsAmount,
                string queryType,
                bool getWithoutData,
                string openMode,
                string settingsToSave,
                string requiredSettings
                )
        {
           
            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
//#if (!DEBUG)
            try
            {
//#endif

                SaveSetting(settingsToSave);

                CxEntityUsageMetadata usage = mHolder.EntityUsages[entityUsageId];
                CxEntityUsageMetadata parentUsage = null;
                if (string.IsNullOrWhiteSpace(parentEntityUsageId) == false)
                    parentUsage = mHolder.EntityUsages[parentEntityUsageId];

                List<CxFilterItem> filterItems;
            List<CxSortDescription> sortDescriptions;
            Dictionary<string, object> parentPks;
            Dictionary<string, object> joinValues;
            Dictionary<string, object> whereValues;
            Dictionary<string, object> primaryKeysValues;

            List<Dictionary<string, object>> selectedEntities;
            Dictionary<string, object> currentEntity;


            ParseQueryParams(
                out primaryKeysValues,
                out whereValues,
                out joinValues,
                out filterItems,
                out sortDescriptions,
                out parentPks,
                out currentEntity, 
                out selectedEntities,
                pkVals,
                whereVals,
                joins,
                filters,
                sorts,
                parentPrimaryKeys,
                null,
                null,
                usage, 
                parentUsage);

            CxQueryParams prms = new CxQueryParams()
            {
                ChangedAttributeId = changedAttributeId,
                EntityUsageId = entityUsageId,
                //EntityValues = currentEntity,
                FilterItems = filterItems,
                JoinValues = joinValues,
                OpenMode = openMode,
                ParentEntityUsageId = parentEntityUsageId,
                ParentPks = parentPks,
                PrimaryKeysValues = primaryKeysValues,
                QueryType = queryType,
                RecordsAmount = recordsAmount,
                SortDescriptions = sortDescriptions,
                StartRecordIndex = startRecordIndex,
                WhereValues = whereValues
            };








            CxEntityUsageMetadata entityUsage = mHolder.EntityUsages[prms.EntityUsageId];
                CxEntityUsageMetadata parentEntityUsage = null;
                if (prms.ParentEntityUsageId != null)
                {
                    parentEntityUsage = mHolder.EntityUsages[prms.ParentEntityUsageId];
                }


                CxJsClientData model = new CxJsClientData();
                using (CxDbConnection connection = CxDbConnections.CreateEntityConnection())
                {

                    IList<string> joinParamsNames = CxDbParamParser.GetList(entityUsage.JoinCondition, true);

                    IxValueProvider paramsProvider =
                      CxValueProviderCollection.Create(
                        CxQueryParams.CreateValueProvider(prms.JoinValues),
                        mHolder.ApplicationValueProvider);



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
                    if (doPerformQueryForCount && getWithoutData == false)
                    {
                        totalEntityAmount = CxBaseEntity.ReadChildEntityAmount(
                          connection, entityUsage,
                          GetWhereCondition(filterCondition, entityUsage.WhereClause, parentWhere), paramsProvider);
                    }

                    DataTable tblChild = new DataTable();

                CxBaseEntity[] entities = new CxBaseEntity[] { };
                if (getWithoutData == false)
                {
                    entityUsage.ReadChildData(
                      connection, tblChild,
                      paramsProvider, GetWhereCondition(filterCondition, entityUsage.WhereClause, parentWhere),
                      NxEntityDataCache.NoCache,
                      prms.StartRecordIndex, prms.RecordsAmount);

                    System.Collections.ArrayList childList = (System.Collections.ArrayList)CxBaseEntity.GetEntityListFromTable(entityUsage, tblChild);
                    entities = (CxBaseEntity[])childList.ToArray(typeof(CxBaseEntity));
                }

                    if (!doPerformQueryForCount)
                    {
                        totalEntityAmount = entities.Length;
                    }

                    Dictionary<string, CxClientRowSource> unfilteredRowSources = new Dictionary<string, CxClientRowSource>();
                Dictionary<string, CxClientRowSource> filteredRowSources = new Dictionary<string, CxClientRowSource>();

                    model = new CxJsClientData(entityUsage, entities, connection)
                    {
                        EntityUsageId = entityUsage.Id,
                        UnfilteredRowSources = unfilteredRowSources,
                        FilteredRowSources = filteredRowSources,
                        TotalDataRecordAmount = totalEntityAmount == -1 ? 0 : totalEntityAmount
                    };
                    if (prms.SortDescriptions != null && prms.SortDescriptions.Count() > 0)
                    {
                        model.SortDescriptions = prms.SortDescriptions.Select(s =>
                        new CxSortDescriptionJs()
                        {
                            AttributeId = s.AttributeId,
                            Direction = Enum.GetName(typeof(NxListSortDirection), s.Direction)
                        }).ToList();
                }
                    

                    UpdateRecentItems(connection, model);
                }

                InitApplicationValues(model.ApplicationValues);



            Dictionary<string, CxClientRowSource> rowSources = GetFilterFormRowSources(usage);










            var result = new
            {
                Templates = GetTemplates(requiredTemplates),
                Metadata = clientEntityUsagesRequired ? GetEntityMetadata(entityUsageId) : null,
                EntityList = model,
                RowSources = rowSources,
                EntityUsageId = entityUsageId,
                RecordsAmount = recordsAmount,
                Settings = GetSettings(requiredSettings, null)
            };


            json.Data = result;
            return json;
//#if (!DEBUG)
            }
            catch (Exception ex)
            {
                var result = new { Error = ex.Message };

                json.Data = result;
                return json;
            }
//#endif
        }
    }
}