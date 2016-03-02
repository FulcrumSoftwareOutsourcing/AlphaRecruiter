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
using Framework.Utils;
using System.Web;

namespace Framework.Remote
{
    /// <summary>
    /// Provides base logic for commands managing.
    /// </summary>
    public class CxCommandController
    {
        //-------------------------------------------------------------------------
        /// <summary>
        /// Creates command handler instance for the given command.
        /// </summary>
        protected IxCommandHandler CreateCommandHandler(Metadata.CxCommandMetadata command)
        {
            if (CxUtils.NotEmpty(command.SqlCommandText))
            {
                return new CxDbCommandHandler();
            }

            if (string.IsNullOrEmpty(command.SqlCommandText) && command.WindowsHandlerClass == null)
            {
                throw new ExException(
                  String.Format("Handler class is not specified for command '{0}'.", command.Id));
            }
            Type handlerType = command.WindowsHandlerClass.Class;
            IxCommandHandler handler = (IxCommandHandler)CxType.CreateInstance(handlerType);
            if (handler == null)
            {
                throw new ExException(
                  String.Format("Could not create handler instance for command '{0}'.", command.Id));
            }
            return handler;
        }

        //----------------------------------------------------------------------------
        /// <summary>
        /// Handles internal(not from metadata, such as 'SAVE' etc...) command.
        /// </summary>
        /// <param name="commandData"></param>
        public void HandleInternalCommand(CxCommandData commandData)
        {
            if (commandData.CommandId == CxCommandIDs.SAVE)
            {
                
                if (commandData.SelectedEntities != null && commandData.SelectedEntities.Count > 1)
                {

                    foreach (CxBaseEntity entity in commandData.SelectedEntities)
                    {

                        HandleUploadedFiles(entity, commandData.EntityUsage);


                        using (CxDbConnection connection = CxDbConnections.CreateEntityConnection())
                        {
                            entity.IsNew = commandData.IsNewEntity;
                            entity.Validate(connection);
                            entity.WriteChangesToDb(connection);
                        }
                    }

                }
                else
                {
                    HandleUploadedFiles(commandData.CurrentEntity, commandData.EntityUsage);
                    using (CxDbConnection connection = CxDbConnections.CreateEntityConnection())
                    {
                        commandData.CurrentEntity.IsNew = commandData.IsNewEntity;
                        commandData.CurrentEntity.Validate(connection);
                        commandData.CurrentEntity.WriteChangesToDb(connection);

                        if (commandData.QueryParams.PrimaryKeysValues == null)
                            commandData.QueryParams.PrimaryKeysValues = new Dictionary<string, object>();

                        foreach (KeyValuePair<string, object> info in commandData.CurrentEntity.PrimaryKeyInfo)
                        {
                            if (!commandData.QueryParams.PrimaryKeysValues.ContainsKey(info.Key))
                                commandData.QueryParams.PrimaryKeysValues.Add(info.Key, info.Value);
                            else
                                commandData.QueryParams.PrimaryKeysValues[info.Key] = info.Value;


                        }



                    }
                }


            }
        }

        private void HandleUploadedFiles(CxBaseEntity entity, CxEntityUsageMetadata entityUsage)
        {
            var cache = HttpContext.Current.Cache;
            foreach (var attr in entityUsage.Attributes)
            {
                CxUploadHandler uploadHandler = cache[Convert.ToString(entity[attr.Id])] as CxUploadHandler;
                if ((attr.Type == "file" || attr.Type == "photo") && uploadHandler == null)
                {
                    string contentStateAttrName = attr.Id + "STATE";
                    string state = Convert.ToString(entity[contentStateAttrName]);

                    //if (string.IsNullOrEmpty(state) || state == "REMOVE_BLOB_FROM_DB")
                    //{
                    //    entity[attr.Id] = new byte[0];
                    //}


                }

                if ((attr.Type == "file" || attr.Type == "photo") && uploadHandler != null)
                {
                    entity[attr.Id] = uploadHandler.GetData();
                }
            }
        }

        //-------------------------------------------------------------------------
        /// <summary>
        /// Handles command.
        /// </summary>
        /// <param name="commandData">execution command data</param>
        public void HandleCommand(CxCommandData commandData)
        {

            CheckCommandDisableConditions(commandData.Command, commandData.EntityUsage, commandData.CurrentEntity);

            commandData.IsMultiple = commandData.Command.IsEntityInstanceRequired &&
                                     commandData.Command.IsMultiple &&
                                     commandData.SelectedEntities.Count > 1;

            //NxCommandResult result = NxCommandResult.Fail;


            if (commandData.Command.IsSlHandlerBatch &&
                commandData.Command.IsEntityInstanceRequired &&
                commandData.Command.WindowsHandlerClassId != null)
            {
                // Batch execution. Allowed for entity instance commands which have windows handler.
                if (commandData.Command.IsEntityInstanceRequired && commandData.SelectedEntities.Count == 0)
                {
                    throw new Exception(string.Format(
                        "Command with Id '{0}' marked as 'IsEntityInstanceRequired' but no entity is selected.",
                        commandData.Command.Id));
                }
                foreach (CxBaseEntity entity in commandData.SelectedEntities)
                {
                    Metadata.CxCommandMetadata cmd = entity.Metadata.GetCommand(commandData.Command.Id);
                    CheckCommandDisableConditions(cmd, entity.Metadata, entity);
                }
                IxCommandHandler handler = CreateCommandHandler(commandData.Command);
                handler.ExecuteBatch(this, commandData);
                DoAfterCommand(commandData);
            }
            else
            {
                if (commandData.IsMultiple)
                {
                    // Command by command execution
                    for (int i = 0; i < commandData.SelectedEntities.Count; i++)
                    {
                        //InitCommandData(commandData, i);
                        //Metadata.CxCommandMetadata cmd = commandData.SelectedEntities[i].Metadata.GetCommand(commandData.Command.Id);
                        IxCommandHandler handler = CreateCommandHandler(commandData.Command);
                        commandData.CurrentEntity = commandData.SelectedEntities[i];
                        handler.ExecuteCommand(this, commandData);
                    }
                    DoAfterCommand(commandData);
                }
                else
                {
                    // Execute single command
                    IxCommandHandler handler = CreateCommandHandler(commandData.Command);
                    handler.ExecuteCommand(this, commandData);
                    DoAfterCommand(commandData);
                }
            }

        }

        //-------------------------------------------------------------------------
        /// <summary>
        /// Checks command disable conditions, raises an exception if disable condition is true.
        /// </summary>
        protected void CheckCommandDisableConditions(
          Metadata.CxCommandMetadata command,
          CxEntityUsageMetadata entityUsage,
          CxBaseEntity entity)
        {
            string errorText = null;
            if (command != null && (entityUsage != null || entity != null))
            {

                using (CxDbConnection connection = CxDbConnections.CreateEntityConnection())
                {
                    CxGetParentEntityWrapper wrapper = new CxGetParentEntityWrapper(connection, entity);
                    errorText = CxBaseEntity.GetDisableConditionsErrorText(
                    connection,
                    entity != null ? entity.Metadata : entityUsage,
                    command.DisableConditions,
                    new DxGetEntityByEntityUsage(wrapper.GetParentEntity));
                }
            }
            if (CxUtils.NotEmpty(errorText))
            {
                throw new ExValidationException(errorText);
            }
        }

        //----------------------------------------------------------------------------
        /// <summary>
        /// Does actions after command is performed.
        /// </summary>
        protected void DoAfterCommand(CxCommandData commandData)
        {

        }





    }
}
