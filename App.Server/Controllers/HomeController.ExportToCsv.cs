using Framework.Db;
using Framework.Entity;
using Framework.Entity.Filter;
using Framework.Metadata;
using Framework.Remote;
using Framework.Remote.Mobile;
using Framework.Utils;
using Framework.Web.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web.Mvc;
using System.Linq;
using System.Web.Caching;
using System.Text;

namespace App.Server.Controllers
{
    public partial class HomeController
    {
        [Authorize]
        public ActionResult ExportToCsv(
                string entityUsageId,
                string currentEnt,
                string selectedEnts,
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
                string openMode,
               
                IEnumerable<string> requiredTemplates,
                string settingsToSave
            )
        {
            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
//#if (!DEBUG)
            try
            {
 //#endif
                SaveSetting(settingsToSave);

                List<Dictionary<string, object>> selectedEntities;
            Dictionary<string, object> currentEntity;
            Dictionary<string, object> primaryKeysValues;
            Dictionary<string, object> whereValues;
            Dictionary<string, object> joinValues;
            List<CxFilterItem> filterItems;
            List<CxSortDescription> sortDescriptions;
            Dictionary<string, object> parentPks;

            List<string> validationErrors = new List<string>();

            CxEntityUsageMetadata entityUsage = mHolder.EntityUsages[entityUsageId];
                CxEntityUsageMetadata parentUsage = null;
                if (string.IsNullOrWhiteSpace(parentEntityUsageId) == false)
                    parentUsage = mHolder.EntityUsages[parentEntityUsageId];



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
                currentEnt,
                selectedEnts,
                entityUsage,
                parentUsage);



            CxQueryParams prms = new CxQueryParams()
            {
                ChangedAttributeId = changedAttributeId,
                EntityUsageId = entityUsageId,
                EntityValues = currentEntity,
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




            string csv = GetCsv(prms);

            //Guid csvId = Guid.NewGuid();
            //TimeSpan timeoutSpan = new TimeSpan(0, 0, 0, 0, 60000);
            //Cache cache = this.HttpContext.Cache;
            //cache.Insert(
            //  csvId.ToString(),
            //  csv,
            //  null,
            //  Cache.NoAbsoluteExpiration,
            //  timeoutSpan,
            //  CacheItemPriority.NotRemovable,
            //  null);


            byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(csv);
            string fileName = "exported_content.csv";

                

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);


           





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





        //----------------------------------------------------------------------------
        /// <summary>
        /// Creates and returns csv string. 
        /// </summary>
        /// <param name="prms">Parameters to create csv.</param>
        /// <returns>Created csv string.</returns>
        private string GetCsv(CxQueryParams prms)
        {
            CxEntityUsageMetadata entityUsage = mHolder.EntityUsages[prms.EntityUsageId];
            using (CxDbConnection connection = CxDbConnections.CreateEntityConnection())
            {
                IList<string> whereParamsMames =
                  CxDbParamParser.GetList(entityUsage.WhereClause, true);

                // Obtaining the parent entity.
                CxBaseEntity parent = null;
                if (prms.ParentEntityUsageId != null)
                {
                    CxEntityUsageMetadata parentEntityUsage = mHolder.EntityUsages[prms.ParentEntityUsageId];
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
                    mHolder.ApplicationValueProvider);

                foreach (CxFilterItem filterItem in prms.FilterItems)
                {
                    filterItem.Operation =
                      (NxFilterOperation)Enum.Parse(typeof(NxFilterOperation), filterItem.OperationAsString);
                    CxFilterOperator filterOperator
                      = CxFilterOperator.Create(entityUsage, filterItem);
                    if (filterOperator != null)
                    {
                        filterOperator.InitializeValueProvider(paramsProvider);
                    }

                }
                string filterCondition = GetFilterCondition(entityUsage, prms.FilterItems.ToList<IxFilterElement>());


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

                List<string> orderByClauses = new List<string>();
                orderByClauses.Add(connection.ScriptGenerator.GetOrderByClause(sortings));
                string completeOrderByClause = string.Join(",", orderByClauses.ToArray());

                DataTable dt = new DataTable();
                if (prms.JoinValues != null && prms.JoinValues.Count() > 0)
                {
                    IList<string> joinParamsNames = CxDbParamParser.GetList(entityUsage.JoinCondition, true);

                    paramsProvider =
                      CxValueProviderCollection.Create(
                        CxQueryParams.CreateValueProvider(prms.JoinValues),
                        mHolder.ApplicationValueProvider);

                    entityUsage.ReadChildData(
                    connection, dt,
                    paramsProvider, filterCondition,
                    NxEntityDataCache.NoCache,
                    prms.StartRecordIndex, prms.RecordsAmount);
                }
                else
                {
                    entityUsage.ReadData(connection, dt, filterCondition, paramsProvider, completeOrderByClause);
                }

                Dictionary<string, string> columnsCaptions = new Dictionary<string, string>();
                List<string> columnsToExport = new List<string>();
                foreach (CxAttributeMetadata attribute in entityUsage.GetAttributeOrder(NxAttributeContext.GridVisible).OrderAttributes)
                {
                    columnsToExport.Add(attribute.Id);
                    columnsCaptions.Add(attribute.Id, string.Concat("\"", attribute.Caption, "\""));
                }

                IEnumerable<CxAttributeMetadata> rsAttributes =
                  (from attribute in entityUsage.Attributes
                   where attribute.RowSource != null &&
                     string.IsNullOrEmpty(attribute.RowSource.EntityUsageId) == false
                   select attribute).ToList();

                Dictionary<string, IList<CxComboItem>> rsValues = new Dictionary<string, IList<CxComboItem>>();
                foreach (CxAttributeMetadata attrMetadata in rsAttributes)
                {
                    IList<CxComboItem> comboItems =
                      attrMetadata.RowSource.GetList(null, connection, null, null, true);
                    rsValues.Add(attrMetadata.Id, comboItems);

                    int rsClmOrdinal = 0;
                    List<DataColumn> columnsToAdd = new List<DataColumn>();
                    foreach (DataColumn column in dt.Columns)
                    {
                        if (column.ColumnName.ToUpper() == attrMetadata.Id)
                        {
                            DataColumn newRsTextClm = new DataColumn(attrMetadata.Id)
                            {
                                DataType = typeof(string)
                            };
                            columnsToAdd.Add(newRsTextClm);
                            column.ColumnName = string.Concat("_", column.ColumnName);
                            rsClmOrdinal = column.Ordinal;
                            break;
                        }
                    }
                    dt.Columns.AddRange(columnsToAdd.ToArray());

                    if (dt.Columns.Cast<DataColumn>().Contains(x => CxText.Equals(x.ColumnName, attrMetadata.Id)))
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            object rsID = row[rsClmOrdinal];
                            List<string> rsTexts = (from item in comboItems
                                                    where (rsID != null) &&
                                                          (item.Value != null) &&
                                                          item.Value.Equals(rsID)
                                                    select item.Description).ToList();

                            row[attrMetadata.Id] = rsTexts.Count > 0 ? rsTexts[0] : string.Empty;
                        }
                    }
                }

                return CxCSV.DataTableToCsv(
                  dt,
                  columnsToExport,
                  true,
                  columnsCaptions,
                  null,
                  CxWebUtils.CurrentCulture.TextInfo.ListSeparator,
                  CxWebUtils.CurrentCulture.TextInfo.ListSeparator);
            }
        }











    }





}