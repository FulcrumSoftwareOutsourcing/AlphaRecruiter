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

namespace Framework.Db
{
  //-------------------------------------------------------------------------
  /// <summary>
  /// An interface for a factory that's able to produce valid connections.
  /// </summary>
  public interface IxConnectionFactory
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates a valid connection to be used.
    /// </summary>
    /// <returns>a valid connection</returns>
    CxDbConnection CreateConnection();
    //-------------------------------------------------------------------------
    /// <summary>
    /// Disposes the given connection
    /// </summary>
    /// <param name="connection">a connection to be disposed</param>
    void DisposeConnection(CxDbConnection connection);
    //-------------------------------------------------------------------------
  }
}
