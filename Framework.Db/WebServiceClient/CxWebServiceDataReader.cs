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

using System;
using System.Data;
using Framework.Utils;

namespace Framework.Db.WebServiceClient
{
	/// <summary>
	/// Web service client data reader.
	/// </summary>
	public class CxWebServiceDataReader : CxDataTableReader
	{
    //-------------------------------------------------------------------------
    protected CxWebServiceConnection m_Connection = null;
    protected CxDbCommandDescription m_CommandDescription = null;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    internal CxWebServiceDataReader(DataSet ds, CxWebServiceCommand command) : base(ds)
		{
      m_Connection = command.Connection;
      m_CommandDescription = new CxDbCommandDescription(command);
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Calls web service to get schema table.
    /// </summary>
    protected object WebServiceGetSchemaTable()
    {
      CxDbService service = m_Connection.CreateWebService();
      if (m_CommandDescription.CommandTimeout > 0)
      {
        service.Timeout = m_CommandDescription.CommandTimeout * 1000; // convert to milliseconds
      }
      m_CommandDescription.SqlResult = NxSqlResult.SchemaTable;
      CxDbCommandResult commandResult = service.ExecuteSQL(
        m_Connection.RegistrationRecord.ClientID,
        m_Connection.Database,
        m_Connection.UserID,
        m_Connection.EncryptedPassword,
        Guid.Empty,
        m_CommandDescription);
      return commandResult;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns schema table.
    /// </summary>
    override public DataTable GetSchemaTable()
    {
      CxDbCommandResult commandResult = (CxDbCommandResult) 
        m_Connection.CallWebServiceMethod(new DxWebServiceCallMethod(WebServiceGetSchemaTable));
      return commandResult != null ? commandResult.DataTable : null;
    }
    //-------------------------------------------------------------------------
  }
}