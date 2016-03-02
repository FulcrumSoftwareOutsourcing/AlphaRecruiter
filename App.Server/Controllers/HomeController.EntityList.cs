
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
        public ActionResult GetEntityList(
            string entityUsageId,
            bool clientEntityUsagesRequired,
            IEnumerable<string> requiredTemplates,
            string pkVals,
             string joins,
                string whereVals,

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
                string requiredSettings)
        {
            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
//#if (!DEBUG)
            try
            {
//#endif
            CxEntityUsageMetadata usage = mHolder.EntityUsages[entityUsageId];
            CxEntityUsageMetadata parentUsage = null;
            if(string.IsNullOrWhiteSpace(parentEntityUsageId) == false)
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



               

            Dictionary<string, CxClientRowSource> filterRowSources = GetFilterFormRowSources(usage);


              




            CxQueryParams queryData = new CxQueryParams()
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


            CxJsClientData data = GetEntityList(queryData, getWithoutData);
            if (data.TotalDataRecordAmount == -1)
                data.TotalDataRecordAmount = 0;


                var result = new
                {
                    Templates = GetTemplates(requiredTemplates),
                    Metadata = clientEntityUsagesRequired ? GetEntityMetadata(entityUsageId) : null,
                    EntityList = data,
                    RowSources = filterRowSources,
                    EntityUsageId = entityUsageId,
                    RecordsAmount = recordsAmount,
                    Settings = GetSettings(requiredSettings, null)
            };


            json.Data = result;

                SaveSetting(settingsToSave);

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







        private Dictionary<string, CxClientRowSource> GetFilterFormRowSources(CxEntityUsageMetadata entityUsage)
        {

            using (CxDbConnection conn = CxDbConnections.CreateEntityConnection())
            {
                CxBaseEntity[] entities = new CxBaseEntity[] { };


                IEnumerable<CxAttributeMetadata> rsAttributes =
                  (from attribute in entityUsage.Attributes
                   where attribute.RowSource != null &&
                     string.IsNullOrEmpty(attribute.RowSource.EntityUsageId) == false
                   select attribute).ToList();

                Dictionary<string, CxClientRowSource> unfilteredRowSources = new Dictionary<string, CxClientRowSource>();
                foreach (CxAttributeMetadata attrMetadata in rsAttributes)
                {
                    CxClientRowSource clientRS = new CxClientRowSource
                    {
                        RowSourceId = attrMetadata.RowSourceId,
                        RowSourceData = new List<CxClientRowSourceItem>()
                    };


                    IList<CxComboItem> comboItems =
                      attrMetadata.RowSource.GetList(null, conn, null, null, true);




                    foreach (CxComboItem comboItem in comboItems)
                    {
                        CxClientRowSourceItem clienRsItem =
                          new CxClientRowSourceItem
                          {
                              Value = comboItem.Value,
                              Text = comboItem.Description,
                              ImageId = comboItem.ImageReference
                          };

                        clientRS.RowSourceData.Add(clienRsItem);
                    }
                    if (!unfilteredRowSources.ContainsKey(clientRS.RowSourceId.ToUpper()))
                    {
                        unfilteredRowSources.Add(clientRS.RowSourceId.ToUpper(), clientRS);
                    }
                }



                // InitApplicationValues(model.ApplicationValues);
                return unfilteredRowSources;
            }

        }


        private CxJsClientData GetEntityList(CxQueryParams prms, bool getWithoutData)
        {




            CxEntityUsageMetadata entityUsage = mHolder.EntityUsages[prms.EntityUsageId];
            CxEntityUsageMetadata parentEntityUsage = null;
            if (prms.ParentEntityUsageId != null)
            {
                parentEntityUsage = mHolder.EntityUsages[prms.ParentEntityUsageId];
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


            CxJsClientData data = null;
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
                    mHolder.ApplicationValueProvider);

                foreach (CxFilterItem filterItem in prms.FilterItems)
                {
                    filterItem.Operation =
                      (Framework.Entity.NxFilterOperation)Enum.Parse(typeof(Framework.Entity.NxFilterOperation), filterItem.OperationAsString);
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
                if (doPerformQueryForCount && getWithoutData == false)
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
                CxBaseEntity[] entities = new CxBaseEntity[] { };
                if (getWithoutData == false)
                {
                    entities = CxBaseEntity.ReadEntities(
                      connection, entityUsage, GetWhereCondition(filterCondition, entityUsage.WhereClause, parentWhere),
                      paramsProvider,
                      prms.StartRecordIndex, prms.RecordsAmount, sortings);
                }



                if (!doPerformQueryForCount)
                {
                    totalEntityAmount = entities.Length;
                }

                Dictionary<string, CxClientRowSource> unfilteredRowSources;
                Dictionary<string, CxClientRowSource> filteredRowSources;
                GetDynamicRowSources(out unfilteredRowSources, out filteredRowSources, entities, entityUsage);

                data = new CxJsClientData(entityUsage, entities, connection);
                data.TotalDataRecordAmount = totalEntityAmount;
                data.UnfilteredRowSources = unfilteredRowSources;
                data.FilteredRowSources = filteredRowSources;


                if (prms.SortDescriptions != null && prms.SortDescriptions.Count() > 0)
                {


                    data.SortDescriptions = prms.SortDescriptions.Select(s =>
                        new CxSortDescriptionJs()
                        {
                            AttributeId = s.AttributeId,
                            Direction = Enum.GetName(typeof(NxListSortDirection), s.Direction)
                        }).ToList();

                }


                UpdateRecentItems(connection, data);

            }

            //InitApplicationValues(model.ApplicationValues);
            return data;

        }






        //----------------------------------------------------------------------------
        /// <summary>
        /// Returns WHERE condition composed from the given filter elements. 
        /// </summary>
        /// <param name="meta">entity usage</param>
        /// <param name="filterElements">list of IxFilterElement objects</param>
        /// <returns>composed WHERE clause</returns>
        private string GetFilterCondition(CxEntityUsageMetadata meta, IList<IxFilterElement> filterElements)
        {
            string where = "";

            foreach (CxFilterItem element in filterElements)
            {
                element.Operation = (NxFilterOperation)Enum.Parse(typeof(NxFilterOperation), element.OperationAsString);
                CxFilterOperator filterOperator = CreateFilterOperator(meta, element);
                if (filterOperator != null)
                {
                    string wherePart = filterOperator.GetCondition();
                    if (CxUtils.NotEmpty(wherePart))
                    {
                        where += (where != "" ? " AND " : "") + wherePart;
                    }
                }
            }

            return where;
        }

        //----------------------------------------------------------------------------
        private string GetWhereCondition(string filterCondition, string whereClause, string parentWhereClause)
        {
            string where = "";
            if (!string.IsNullOrEmpty(filterCondition))
            {
                where = where + " " + filterCondition + " ";
            }
            if (!string.IsNullOrEmpty(whereClause))
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where = where + " " + whereClause + " ";
            }
            if (!string.IsNullOrEmpty(parentWhereClause))
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where = where + " " + parentWhereClause + " ";
            }
            return where;
        }


        //----------------------------------------------------------------------------
        /// <summary>
        /// Updates recent items lists.
        /// </summary>
        private void UpdateRecentItems(CxDbConnection connection, CxJsClientData model)
        {
            /*
            CxAppServerContext context = new CxAppServerContext();
            if (context.EntityMarks.OpenItems.Count > 0)
            {
                CxEntityMark openItem = context.EntityMarks.OpenItems[0];
                CxBaseEntity openEntity = openItem.CreateAndReadFromDb(connection);
                context.EntityMarks.DeleteMark(openItem);

                context.EntityMarks.AddMark(openEntity, NxEntityMarkType.Recent, true, openItem.OpenMode,
                  context["APPLICATION$APPLICATIONCODE"].ToString());
                if (context.EntityMarks.RecentItems.Count > 30)
                {
                    int lastItem = context.EntityMarks.RecentItems.Count - 1;
                    context.EntityMarks.DeleteMark(context.EntityMarks.RecentItems[lastItem]);
                }
                context.EntityMarks.SaveAndReload(connection, mHolder);
                CxEntityMark addedMark =
                  context.EntityMarks.RecentItems[context.EntityMarks.RecentItems.Count - 1];
                model.EntityMarks = new CxClientEntityMarks();
                //model.EntityMarks.AddedRecentItems.Add(new CxClientEntityMark(addedMark));
                foreach (CxEntityMark recentItem in context.EntityMarks.RecentItems)
                {
                    model.EntityMarks.AllRecentItems.Add(new CxClientEntityMark(recentItem));
                }
            }
            */


        }

        /// <summary>
        /// Creates and returns filter operator for the given filter element.
        /// </summary>
        /// <param name="meta">entity usage</param>
        /// <param name="element">filter element</param>
        /// <returns>created filter operator or null</returns>
        virtual protected CxFilterOperator CreateFilterOperator(CxEntityUsageMetadata meta, IxFilterElement element)
        {
            return CxFilterOperator.Create(meta, element);
        }


    }

    public enum NxOpenMode
    {
        /// <summary>
        /// Defines Frame that open to data view.
        /// </summary>
        View,

        /// <summary>
        /// Defines Frame that open to edit data.
        /// </summary>
        Edit,

        /// <summary>
        /// Defines Frame that open to create new data.
        /// </summary>
        New,

        /// <summary>
        /// Defines Frame that must detect the OpenMode by itself.
        /// </summary>
        Autodetect,

        /// <summary>
        /// Defines Frame that opens as child frame, to data view.
        /// </summary>
        ChildView,

        /// <summary>
        /// Defines Frame that opens as child frame, to data edit.
        /// </summary>
        ChildEdit,

        /// <summary>
        /// Defines Frame that opens as child frame, to create new data.
        /// </summary>
        ChildNew
    }
}