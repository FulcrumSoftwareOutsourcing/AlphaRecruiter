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
        public ActionResult ExecuteCommand(
                string commandId,
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
                string queryType ,
                string openMode,
                bool isNewEntity,
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
            List<Dictionary<string, object>> selectedEntities ;
            Dictionary<string, object> currentEntity;
            Dictionary<string, object> primaryKeysValues ;
            Dictionary<string, object> whereValues  ;
            Dictionary<string, object> joinValues ;
            List<CxFilterItem> filterItems ;
            List<CxSortDescription> sortDescriptions ;
            Dictionary<string, object> parentPks ;

            List<string> validationErrors = new List<string>();

            CxEntityUsageMetadata entityUsage = mHolder.EntityUsages[entityUsageId];
          

            CxCommandMetadata commandMetadata = entityUsage.GetCommand(commandId);
           

          



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




            CxCommandData commandData = new CxCommandData();
            IxValueProvider entityValueProvider = CxQueryParams.CreateValueProvider(currentEntity);

            if (!GetIsCommandEnabled(commandId, entityUsage, commandMetadata, entityValueProvider))
            {
                throw new ExException("The command you're trying to execute is not applicable in the current context");
            }



            commandData.CommandId = commandId;
            commandData.EntityUsage = entityUsage;
            commandData.Command = commandMetadata;
            commandData.QueryParams = new CxQueryParams() {
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
            } ;
            commandData.IsNewEntity  = isNewEntity;

            commandData.CurrentEntity = CxBaseEntity.CreateByValueProvider(
                entityUsage,
                entityValueProvider);
            

            if (commandData.Command != null)
            {
                if (selectedEntities != null)
                {
                    foreach (Dictionary<string, object> entityValues in selectedEntities)
                    {

                        CxBaseEntity selectedEntity = CxBaseEntity.CreateByValueProvider
                            (entityUsage,
                             CxQueryParams.CreateValueProvider(entityValues));
                        commandData.SelectedEntities.Add(selectedEntity);
                    }
                }

                CxCommandController commandController = new CxCommandController();
                try
                {
                    commandController.HandleCommand(commandData);
                }
                catch (ExValidationException ex)
                {
                    validationErrors.Add(ex.Message);
                }
                catch
                {
                    throw;
                }
            }
            else
            {

                foreach (var entity in selectedEntities)
                {
                    IxValueProvider valueProvider = CxQueryParams.CreateValueProvider(entity);
                    commandData.SelectedEntities.Add(CxBaseEntity.CreateByValueProvider(
                      entityUsage,
                      valueProvider));
                }

                CxCommandController commandController = new CxCommandController();
                
                try
                {
                    commandController.HandleInternalCommand(commandData);
                }
                catch (ExValidationException ex)
                {
                    validationErrors.Add(ex.Message);
                }
                //catch 
                //{
                //    throw;
                //}
            }

            

            CxJsClientData data = null;

            if (commandData.QueryParams.QueryType == CxQueryTypes.ENTITY_LIST)
            {
                data = GetEntityList( commandData.QueryParams, false);
            }
            if (commandData.QueryParams.QueryType == CxQueryTypes.CHILD_ENTITY_LIST)
            {
                //return GetChildEntityList(marker, commandData.QueryParams);
            }
            if (commandData.QueryParams.QueryType == CxQueryTypes.ENTITY_FROM_PK)
            {
                data = GetEntityFromPkInternal(
                    commandData.EntityUsage.Id,
                    false, 
                    null, 
                   primaryKeysValues,
                   parentEntityUsageId, 
                   parentPks, 
                   currentEntity, 
                   openMode);
            }
            if (commandData.QueryParams.QueryType == CxQueryTypes.DIRECT_BACK_ENTITY)
            {
                using (CxDbConnection conn = CxDbConnections.CreateEntityConnection())
                {
                    CxJsClientData model = new CxJsClientData();
                    model.EntityUsageId = commandData.EntityUsage.Id;
                    List<CxBaseEntity> ents = new List<CxBaseEntity> { commandData.CurrentEntity };
                    data = model;
                    
                }
            }

            CxClientCommandMetadata returnBackCommand = null;
            if(commandMetadata != null && entityUsage != null)
            {
                returnBackCommand = new CxClientCommandMetadata(commandMetadata, entityUsage);
            }

                var result = new
                {
                    Templates = GetTemplates(requiredTemplates),
                    EntityList = data,
                    EntityUsageId = entityUsageId,
                    ValidationErrors = validationErrors,
                    ExecutedCommandId = commandId,
                    Command = returnBackCommand


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


        //----------------------------------------------------------------------------
        /// <summary>
        /// Returns true if the command has the enabled state, otherwise returns false.
        /// </summary>
        /// <param name="commandId">the id of the command</param>
        /// <param name="entityUsage">entity usage to apply the command on</param>
        /// <param name="commandMetadata">the command metadata</param>
        /// <param name="entityValueProvider">the value provider based on the entity</param>
        /// <returns>true if enabled, otherwise false</returns>
        private bool GetIsCommandEnabled(
          string commandId,
          CxEntityUsageMetadata entityUsage,
          CxCommandMetadata commandMetadata,
          IxValueProvider entityValueProvider)
        {
            bool isEnabled = true;
            if (commandMetadata != null)
            {
                isEnabled = commandMetadata.GetIsEnabled(entityUsage);
                if (isEnabled && commandMetadata.IsEntityInstanceRequired)
                {
                    using (CxDbConnection connection = CxDbConnections.CreateEntityConnection())
                    {
                        isEnabled = commandMetadata.GetIsEnabled(entityUsage, connection, entityValueProvider);
                    }
                }
            }
            if (isEnabled && commandId == CxCommandIDs.SAVE)
            {
                using (CxDbConnection connection = CxDbConnections.CreateEntityConnection())
                {
                    CxMetadataHolder holder = entityUsage.Holder;
                    if (holder.Security != null)
                    {
                        IxValueProvider provider = entityUsage.PrepareValueProvider(entityValueProvider);
                        isEnabled = holder.Security.GetRight(
                          entityUsage, entityUsage,
                          "EDIT",
                          connection, provider);
                    }
                }
            }
            return isEnabled;
        }
    }



}





