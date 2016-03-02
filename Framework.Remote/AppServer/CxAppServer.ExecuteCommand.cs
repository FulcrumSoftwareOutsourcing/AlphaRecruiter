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
using Framework.Remote.Mobile;
using Framework.Utils;

namespace Framework.Remote
{
  public partial class CxAppServer
  {
    //----------------------------------------------------------------------------
    /// <summary>
    /// Executes command.
    /// </summary>
    /// <param name="marker">Server operation identifier.</param>
    /// <param name="commandParams">Command parameters.</param>
    /// <returns>CxModel</returns>
    public CxModel ExecuteCommand(Guid marker,
        CxCommandParameters commandParams)
    {
      try
      {
        CxCommandData commandData = new CxCommandData();
        IxValueProvider entityValueProvider = CxQueryParams.CreateValueProvider(commandParams.CurrentEntity);

        CxEntityUsageMetadata entityUsage = m_Holder.EntityUsages[commandParams.EntityUsageId];
        CxCommandMetadata commandMetadata = entityUsage.GetCommand(commandParams.CommandId);
        if (commandMetadata != null && !string.IsNullOrEmpty(commandMetadata.EntityUsageId))
        {
          entityUsage = m_Holder.EntityUsages[commandMetadata.EntityUsageId];
        }

        if (!GetIsCommandEnabled(commandParams.CommandId, entityUsage, commandMetadata, entityValueProvider))
        {
          throw new ExException("The command you're trying to execute is not applicable in the current context");
        }

        commandData.CommandId = commandParams.CommandId;
        commandData.EntityUsage = entityUsage;
        commandData.Command = commandMetadata;
        commandData.QueryParams = commandParams.QueryParams;
        commandData.IsNewEntity = commandParams.IsNewEntity;

        commandData.CurrentEntity = CxBaseEntity.CreateByValueProvider(
            entityUsage,
            entityValueProvider);
        commandData.QueryParams = commandParams.QueryParams;

        if (commandData.Command != null)
        {
          if (commandParams.SelectedEntities != null)
          {
            foreach (Dictionary<string, object> entityValues in commandParams.SelectedEntities)
            {

              CxBaseEntity selectedEntity = CxBaseEntity.CreateByValueProvider
                  (entityUsage,
                   CxQueryParams.CreateValueProvider(entityValues));
              commandData.SelectedEntities.Add(selectedEntity);
            }
          }

          CxCommandController commandController = new CxCommandController();
          commandController.HandleCommand(commandData);
        }
        else
        {
         
          foreach (var entity in commandParams.SelectedEntities)
          {
            IxValueProvider valueProvider = CxQueryParams.CreateValueProvider(entity);
            commandData.SelectedEntities.Add(CxBaseEntity.CreateByValueProvider(
              entityUsage,
              valueProvider));
          }
          
          CxCommandController commandController = new CxCommandController();
          commandController.HandleInternalCommand(commandData);
        }

        if (commandData.QueryParams.QueryType == CxQueryTypes.ENTITY_LIST)
        {
          return GetEntityList(marker, commandData.QueryParams);
        }
        if (commandData.QueryParams.QueryType == CxQueryTypes.CHILD_ENTITY_LIST)
        {
          return GetChildEntityList(marker, commandData.QueryParams);
        }
        if (commandData.QueryParams.QueryType == CxQueryTypes.ENTITY_FROM_PK)
        {
          return GetEntityFromPk(marker, commandData.QueryParams);
        }
        if(commandData.QueryParams.QueryType == CxQueryTypes.DIRECT_BACK_ENTITY)
        {
          using (CxDbConnection conn = CxDbConnections.CreateEntityConnection())
          {
            CxModel model = new CxModel();
            model.EntityUsageId = commandData.EntityUsage.Id;
            List<CxBaseEntity> ents = new List<CxBaseEntity> {commandData.CurrentEntity};
            model.SetData(commandData.EntityUsage, ents, conn);
            return model;
          }
        }
      }
      catch (Exception ex)
      {
        CxExceptionDetails exceptionDetails = new CxExceptionDetails(ex);
        CxModel model = new CxModel { Error = exceptionDetails };
        return model;
      }

      return null;
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
