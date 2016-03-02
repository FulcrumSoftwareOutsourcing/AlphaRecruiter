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
using System.Linq;
using System.Text;
using Framework.Db;
using Framework.Entity;
using Framework.Utils;

namespace Framework.Remote
{
  /// <summary>
  /// Provides base logic for DB commands handling.
  /// </summary>
  public class CxDbCommandHandler : IxCommandHandler
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Executes command for the particular entity.
    /// </summary>
    /// <param name="commandController">command controller invoked the method</param>
    /// <param name="commandData">prepared command data</param>
    public void ExecuteCommand(CxCommandController commandController, CxCommandData commandData)
    {
      if (CxUtils.NotEmpty(commandData.Command.SqlCommandText))
      {

        using (CxDbConnection connection = CxDbConnections.CreateEntityConnection())
        {
          CxBaseEntity.ExecuteSqlCommand(connection, commandData.Command, commandData.CurrentEntity);
        }
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

      foreach (CxBaseEntity entity in commandData.SelectedEntities)
      {
        if (CxUtils.NotEmpty(commandData.Command.SqlCommandText))
        {

          using (CxDbConnection connection = CxDbConnections.CreateEntityConnection())
          {
            CxBaseEntity.ExecuteSqlCommand(connection, commandData.Command, entity);
          }
        }
      }


    }


  }
}
