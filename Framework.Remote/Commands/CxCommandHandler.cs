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

using Framework.Db;
using Framework.Entity;

namespace Framework.Remote
{
  /// <summary>
  /// Provides base logic for commands handling.
  /// </summary>
  public class CxCommandHandler : IxCommandHandler
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Executes command for the particular entity.
    /// </summary>
    /// <param name="commandController">command controller invoked the method</param>
    /// <param name="commandData">prepared command data</param>
    public void ExecuteCommand(CxCommandController commandController, CxCommandData commandData)
    {
      switch (commandData.Command.Id)
      {
        case CxCommandIDs.DELETE:
          ExecuteDeleteCommand(commandController, commandData);
          break;
        case CxCommandIDs.EDIT:
          ExecuteEditCommand(commandController, commandData);
          break;
        case CxCommandIDs.VIEW:
          ExecuteViewCommand(commandController, commandData);
          break;
       
        default:
          return;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Executes command for the entity batch.
    /// </summary>
    /// <param name="commandController">command controller invoked the method</param>
    /// <param name="commandData">original command data</param>
    public void ExecuteBatch(CxCommandController commandController, CxCommandData commandData)
    {
      switch (commandData.Command.Id)
      {
        case CxCommandIDs.DELETE:
          ExecuteDleteCommandBatch(commandController, commandData);
          break;
      }
    }

    //-------------------------------------------------------------------------
    /// <summary>
    /// Executes batch delete command.
    /// </summary>
    virtual protected void ExecuteDleteCommandBatch(
      CxCommandController commandController,
      CxCommandData commandData)
    {
      foreach (CxBaseEntity entity in commandData.SelectedEntities)
      {
        commandData.CurrentEntity = entity;
        commandData.CurrentEntity.IsDeleted = true;
        using (CxDbConnection connection = CxDbConnections.CreateEntityConnection())
        {
          commandData.CurrentEntity.WriteChangesToDb(connection);
        }
      }
    }

    //-------------------------------------------------------------------------
    /// <summary>
    /// Executes delete command.
    /// </summary>
    virtual protected void ExecuteDeleteCommand(
      CxCommandController commandController,
      CxCommandData commandData)
    {
      commandData.CurrentEntity.IsDeleted = true;
      using (CxDbConnection connection = CxDbConnections.CreateEntityConnection())
      {
        commandData.CurrentEntity.WriteChangesToDb(connection);
      }
    }

    //----------------------------------------------------------------------------

    /// <summary>
    /// Executes Edit command.
    /// </summary>
    virtual protected void ExecuteEditCommand(
      CxCommandController commandController,
      CxCommandData commandData)
    {

    }

    /// <summary>
    /// Executes View command.
    /// </summary>
    virtual protected void ExecuteViewCommand(
      CxCommandController commandController,
      CxCommandData commandData)
    {

    }
    //----------------------------------------------------------------------------



  }
}
