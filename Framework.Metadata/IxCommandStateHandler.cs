/********************************************************************
 *  FulcrumWeb RAD Framework - Fulcrum of your business             *
 *  Copyright (c) 2002-2010 FulcrumWeb, ALL RIGHTS RESERVED         *
 *                                                                  *
 *  THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      *
 *  FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        *
 *  COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       *
 *  AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  *
 *  AND PERMISSION FROM FULCRUMWEB. CONSULT THE END USER LICENSE    *
 *  AGREEMENT FOR INFORMATION ON ADDITIONAL RESTRICTIONS.           *
 ********************************************************************/

using System.Collections.Generic;
using Framework.Db;
namespace Framework.Metadata
{
  //-------------------------------------------------------------------------
  public interface IxCommandStateHandler
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns command state depending on the selected entity usage
    /// and selected entities.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="commandMetadata">command metadata to return state for</param>
    /// <param name="entityUsage">selected entity usage</param>
    /// <param name="parentEntity">parent entity, or null if no parent entity available</param>
    /// <param name="entities">selected entities</param>
    /// <param name="proposedStateInfo">currently proposed state info</param>
    /// <param name="request">request type</param>
    /// <returns>actual command state info</returns>
    CxCommandStateInfo GetCommandState(
      CxDbConnection connection,
      CxCommandMetadata commandMetadata,
      CxEntityUsageMetadata entityUsage,
      IxEntity parentEntity,
      List<IxEntity> entities,
      CxCommandStateInfo proposedStateInfo,
      NxCommandStateRequest request);
    //-------------------------------------------------------------------------
  }
}
