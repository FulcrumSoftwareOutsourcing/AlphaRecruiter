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

namespace Framework.Remote
{
    //---------------------------------------------------------------------------
    public enum NxCommandResult { OK, Fail }
    //---------------------------------------------------------------------------

    //---------------------------------------------------------------------------
    /// <summary>
    /// Interface for windows command handler.
    /// </summary>
    public interface IxCommandHandler
    {
        //-------------------------------------------------------------------------
        /// <summary>
        /// Executes command for the particular entity.
        /// </summary>
        /// <param name="commandController">command controller invoked the method</param>
        /// <param name="commandData">prepared command data</param>
        /// <returns>command execution result</returns>
        void ExecuteCommand(
          CxCommandController commandController,
          CxCommandData commandData);
        //-------------------------------------------------------------------------
        /// <summary>
        /// Executes command for the entity batch.
        /// </summary>
        /// <param name="commandController">command controller invoked the method</param>
        /// <param name="commandData">original command data</param>
        /// <returns>command execution result</returns>
        void ExecuteBatch(
          CxCommandController commandController,
          CxCommandData commandData);
        //-------------------------------------------------------------------------
    }
    //---------------------------------------------------------------------------
}
