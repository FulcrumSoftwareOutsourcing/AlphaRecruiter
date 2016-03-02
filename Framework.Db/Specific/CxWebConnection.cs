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

using System.Data;
using System.Data.Common;
using Framework.Db.WebServiceClient;

namespace Framework.Db
{
	/// <summary>
	/// Wrapper for the web service client connection.
	/// </summary>
	public class CxWebConnection : CxDbConnection
	{
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
		protected internal CxWebConnection()
		{
      m_Connection = new CxWebServiceConnection();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates and returns connection of type corresponding to the target 
    /// database type (SqlServer or Oracle).
    /// </summary>
    internal CxDbConnection CreateTargetDbConnection()
    {
      switch (DatabaseType)
      {
        case NxDatabaseType.SqlServer :
          return new CxSqlConnection();
        case NxDatabaseType.Oracle :
          return new CxOracleConnection();
        default:
          return new CxOleDbConnection();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates database command.
    /// </summary>
    /// <returns>created command</returns>
    override protected IDbCommand CreateCommand()
    {
      return new CxWebServiceCommand();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates data adapter.
    /// </summary>
    /// <param name="command">database command to create adapter for</param>
    /// <returns>created data adapter</returns>
    override protected DbDataAdapter CreateDataAdapter(IDbCommand command)
    {
      return new CxWebServiceDataAdapter(command);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates database parameter.
    /// </summary>
    /// <returns>created parameter</returns>
    override protected IDataParameter CreateParameter()
    {
      return new CxWebServiceParameter();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates database LOB parameter.
    /// </summary>
    /// <param name="lobType">type of the LOB this parameter represents</param>
    /// <param name="size">paremeter size</param>
    /// <returns>created parameter</returns>
    override protected IDataParameter CreateParameterLob(NxLobType lobType, int size)
    {
      CxWebServiceParameter parameter = (CxWebServiceParameter) CreateParameter();
      parameter.Size = size;
      return parameter;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns pure parameter name (without prefixes).
    /// </summary>
    /// <param name="name">provider-dependent parameter name</param>
    /// <returns>pure parameter name (without prefixes)</returns>
    override public string GetPureParamName(string name)
    {
      using (CxDbConnection connection = CreateTargetDbConnection())
      {
        return connection.GetPureParamName(name);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns FROM clause for SELECT statement returning an expression
    /// (returns FROM DUAL for Oracle, or empty string for MSSQL)
    /// </summary>
    override public string GetOneRowFromClause()
    {
      using (CxDbConnection connection = CreateTargetDbConnection())
      {
        return connection.GetOneRowFromClause();
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns database type.
    /// </summary>
    override public NxDatabaseType DatabaseType
    {
      get
      {
        return ((CxWebServiceConnection) m_Connection).DatabaseType;
      }
    }
    //----------------------------------------------------------------------------
    protected override CxDbScriptGenerator CreateScriptGenerator()
    {
      return new CxWebScriptGenerator(this);
    }
    //----------------------------------------------------------------------------
  }
}