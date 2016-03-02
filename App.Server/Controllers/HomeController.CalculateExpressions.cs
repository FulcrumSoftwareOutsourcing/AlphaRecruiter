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
        public ActionResult CalculateExpressions(
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
                
                string[] dependentAttributesIds,
                string[] dependentMandatoryAttributesIds,
                string[] dependentStateIds )
        {
            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //#if (!DEBUG)
            try
            {
                //#endif

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



                CxEntityUsageMetadata meta = mHolder.EntityUsages[entityUsageId];
                Dictionary<CxClientAttributeMetadata, object> entityValues = new Dictionary<CxClientAttributeMetadata, object>();
                foreach (CxAttributeMetadata attribute in meta.Attributes)
                {
                    CxClientAttributeMetadata clientAttributeMetadata =
                      new CxClientAttributeMetadata(attribute, meta);
                    entityValues.Add(clientAttributeMetadata, currentEntity[attribute.Id]);
                }

                Dictionary<string, CxClientRowSource> unfilteredRowSources = new Dictionary<string, CxClientRowSource>();
                List<CxClientRowSource> filteredRowSources = new List<CxClientRowSource>();

                CalculateExpressions(
                  ref entityValues,
                  meta,
                  changedAttributeId,
                  ref unfilteredRowSources,
                  ref filteredRowSources);

                CxExpressionResult exprResult = new CxExpressionResult();
                exprResult.Attributes = new List<CxClientAttributeMetadata>();
                exprResult.Values = new Dictionary<string, object>();
                CxEditController editController = new CxEditController(meta);
                Dictionary<string, object> calculatedValues = new Dictionary<string, object>();
                foreach (KeyValuePair<CxClientAttributeMetadata, object> pair in entityValues)
                {
                    calculatedValues.Add(pair.Key.Id, pair.Value);
                }

               

                CxBaseEntity entity = CxBaseEntity.CreateByValueProvider
                            (meta, CxQueryParams.CreateValueProvider(calculatedValues));


                foreach (CxAttributeMetadata attribute in meta.Attributes)
                {
                    bool isVisible = editController.GetIsVisible(attribute, entity);
                    bool isReadOnly = editController.GetIsReadOnly(attribute, entity);
                    CxClientAttributeMetadata clientAttribute = new CxClientAttributeMetadata(attribute, meta)
                    {
                        Visible = isVisible,
                        ReadOnly = isReadOnly
                    };
                    exprResult.Attributes.Add(clientAttribute);

                    object val = entity[attribute.Id];
                    if (val is DBNull)
                        val = null;
                    if (attribute.PrimaryKey ||
                        (currentEntity.ContainsKey(attribute.Id) && dependentAttributesIds.Contains(attribute.Id) ))
                    {
                        exprResult.Values[attribute.Id] = val;
                    }
                }
                exprResult.FilteredRowSources = filteredRowSources;
                exprResult.UnfilteredRowSources = unfilteredRowSources;



                var result = new
                {
                    Templates = GetTemplates(requiredTemplates),
                    ExprResult = exprResult,
                    EntityUsageId = entityUsageId,
                    ValidationErrors = validationErrors
                };



                json.Data = result;
                return json;

            

            }
            catch (Exception ex)
            {
                var result = new { Error = ex.Message };

                json.Data = result;
                return json;
            }

}



        private void CalculateExpressions(
            ref Dictionary<CxClientAttributeMetadata, object> entityValues,
             CxEntityUsageMetadata meta,
             string changedAttributeId,
            ref Dictionary<string, CxClientRowSource> unfilteredRowSources,
            ref List<CxClientRowSource> filteredRowSources)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            foreach (KeyValuePair<CxClientAttributeMetadata, object> pair in entityValues)
            {
                values.Add(pair.Key.Id, pair.Value);
            }
            CxBaseEntity entity = CxBaseEntity.CreateByValueProvider
                          (meta, CxQueryParams.CreateValueProvider(values));
            CxExpressionResult exprResult = new CxExpressionResult();
            exprResult.ActualEntity = entity;


            CxEditController editController = new CxEditController(meta);
            editController.ProcessDependentFields(
              exprResult,
              meta.GetAttribute(changedAttributeId),
              values[changedAttributeId]);

            Dictionary<CxClientAttributeMetadata, object> changedAndDepend =
              new Dictionary<CxClientAttributeMetadata, object>();
            Dictionary<CxClientAttributeMetadata, object> newValues =
              new Dictionary<CxClientAttributeMetadata, object>();

            foreach (CxAttributeMetadata attrMetadata in meta.Attributes)
            {
                bool isVisible = editController.GetIsVisible(attrMetadata, exprResult.ActualEntity);
                bool isReadOnly = editController.GetIsReadOnly(attrMetadata, exprResult.ActualEntity);
                CxClientAttributeMetadata attrMeta = new CxClientAttributeMetadata(attrMetadata, meta)
                {
                    Visible = isVisible,
                    ReadOnly = isReadOnly
                };
                if (!Equals(exprResult.ActualEntity[attrMetadata.Id], values[attrMetadata.Id]) &&
                  HasDependencies(attrMeta))
                {
                    changedAndDepend.Add(attrMeta, exprResult.ActualEntity[attrMetadata.Id]);
                }

                newValues.Add(attrMeta, exprResult.ActualEntity[attrMetadata.Id]);
            }
            entityValues = newValues;

            foreach (KeyValuePair<string, CxClientRowSource> pair in exprResult.UnfilteredRowSources)
            {
                if (unfilteredRowSources.ContainsKey(pair.Key))
                    unfilteredRowSources[pair.Key] = pair.Value;
                else
                    unfilteredRowSources.Add(pair.Key, pair.Value);
            }

            foreach (KeyValuePair<string, CxClientRowSource> pair in exprResult.UnfilteredRowSources)
            {
                if (unfilteredRowSources.ContainsKey(pair.Key))
                    unfilteredRowSources[pair.Key] = pair.Value;
                else
                    unfilteredRowSources.Add(pair.Key, pair.Value);
            }

            foreach (CxClientRowSource rs in exprResult.FilteredRowSources)
            {
                CxClientRowSource cfr =
                  filteredRowSources.FirstOrDefault(r => r.RowSourceId == rs.RowSourceId);

                filteredRowSources.Add(rs);
            }
            if (changedAndDepend.Count > 0)
            {
                foreach (KeyValuePair<CxClientAttributeMetadata, object> depend in changedAndDepend)
                {
                    CalculateExpressions(
                      ref entityValues,
                      meta,
                      depend.Key.Id,
                      ref unfilteredRowSources,
                      ref filteredRowSources);
                }
            }
        }


        private bool HasDependencies(CxClientAttributeMetadata attrMetadata)
        {
            if (attrMetadata.DependentAttributesIds.Count > 0 ||
              attrMetadata.DependentStateIds.Count > 0 ||
              attrMetadata.DependentMandatoryAttributesIds.Count > 0)
            {
                return true;
            }
            return false;
        }




    }







}
