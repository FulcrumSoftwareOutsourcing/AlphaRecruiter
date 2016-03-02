using Framework.Db;
using Framework.Entity;
using Framework.Metadata;
using Framework.Remote;
using Framework.Remote.Mobile;
using Framework.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace App.Server.Controllers
{
    public partial class HomeController
    {
         [Authorize]
        [ValidateInput(false)]
        public ActionResult GetEntityFromPk( string entityUsageId, bool clientEntityUsagesRequired, IEnumerable<string> requiredTemplates,
             
             string pkVals,
             string parentEntityUsageId,
            string parentPrimaryKeys,
             string entityValues,
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


                SaveSetting(settingsToSave);



                CxEntityUsageMetadata entityUsage = mHolder.EntityUsages[entityUsageId];
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
                null,
                null,
                null,
                null,
                parentPrimaryKeys,
                entityValues, 
                null,
                entityUsage, 
                parentUsage);



            var model = GetEntityFromPkInternal(entityUsageId, clientEntityUsagesRequired, requiredTemplates, primaryKeysValues, parentEntityUsageId, parentPks, currentEntity, openMode);

                var result = new
                {
                    Templates = GetTemplates(requiredTemplates),
                    Metadata = clientEntityUsagesRequired ? GetEntityMetadata(entityUsageId) : null,
                    EntityList = model,
                    EntityUsageId = entityUsageId,
                    RowSources = GetFilterFormRowSources(entityUsage),
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


        private CxJsClientData GetEntityFromPkInternal(
                string entityUsageId, 
                bool clientEntityUsagesRequired, 
                IEnumerable<string> requiredTemplates,
                Dictionary<string, object> primaryKeysValues,
                string parentEntityUsageId,
                Dictionary<string, object> parentPks,
                Dictionary<string, object> entityValues,
                string openMode)
        {
            CxEntityUsageMetadata entityUsage = mHolder.EntityUsages[entityUsageId];

            CxJsClientData model = null;
            using (CxDbConnection conn = CxDbConnections.CreateEntityConnection())
            {
                
                bool createNew = string.IsNullOrEmpty(openMode) == false && (openMode.ToUpper() == "NEW" || openMode.ToUpper() == "CHILDNEW");
              

                // Obtaining the parent entity.
                CxBaseEntity parent = null;
                if (parentEntityUsageId != null)
                {
                    CxEntityUsageMetadata parentEntityUsage = mHolder.EntityUsages[parentEntityUsageId];
                    IList<string> parentPkNames = parentEntityUsage.PrimaryKeyIds;
                    IxValueProvider parentVlProvider =
                      CxQueryParams.CreateValueProvider(parentPks);
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
                    if (entityValues != null && entityValues.Count > 0)
                    {
                        paramsProvider =
                          CxValueProviderCollection.Create(
                            CxQueryParams.CreateValueProvider(entityValues),
                            mHolder.ApplicationValueProvider);
                    }
                    else
                    {
                        paramsProvider =
                          CxValueProviderCollection.Create(
                            CxQueryParams.CreateValueProvider(primaryKeysValues),
                            mHolder.ApplicationValueProvider);
                    }

                    entityFromPk = CxBaseEntity.CreateAndReadFromDb(entityUsage, conn, paramsProvider);
                }
                else
                {
                    entityFromPk = CxBaseEntity.CreateWithDefaults(entityUsage, parent, conn);
                }
                CxBaseEntity[] entities = entityFromPk == null ? new CxBaseEntity[0] :
                    new[] { entityFromPk };

                //Dictionary<string, CxClientRowSource> unfilteredRowSources;
                //Dictionary<string, CxClientRowSource> filteredRowSources;
                //GetDynamicRowSources(out unfilteredRowSources, out filteredRowSources, entities, entityUsage);


                Dictionary<string, CxClientRowSource> filteredRowSources = new Dictionary<string, CxClientRowSource>();
                if (entityFromPk != null)
                {
                    foreach (var attr in entityUsage.Attributes)
                    {
                        if (string.IsNullOrEmpty(attr.RowSourceId) == false)
                        {
                            CxEditController editController = new CxEditController(entityUsage);
                            filteredRowSources.Add( attr.Id, editController.GetDynamicRowSource(attr, entityFromPk));
                        }
                    }
                }
                



             



                model = new CxJsClientData(entityUsage, entities, conn)
                {

                    EntityUsageId = entityUsage.Id,


                    //UnfilteredRowSources = unfilteredRowSources,
                    FilteredRowSources = filteredRowSources,

                    TotalDataRecordAmount = entities.Length,
                    IsNewEntity = createNew
                };


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
                    context.EntityMarks.AddMark(entityFromPk, NxEntityMarkType.Open, true, openMode,
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



         public void GetDynamicRowSources
      (out Dictionary<string, CxClientRowSource> unfilteredRowSources,
      out Dictionary<string, CxClientRowSource> filteredRowSources,
      CxBaseEntity[] entities,
      CxEntityUsageMetadata meta

      )
         {
             unfilteredRowSources = new Dictionary<string, CxClientRowSource>();
            filteredRowSources = new Dictionary<string, CxClientRowSource>();

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
                                if(filteredRowSources.ContainsKey(rs.OwnerAttributeId.ToUpper())) //TODO: need to fix it for grid-type data 
                                    filteredRowSources.Add(rs.OwnerAttributeId.ToUpper(), rs);
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


    public class DictionaryModel
    {
        public Dictionary<string, string> dict { get; set; }

    }


}