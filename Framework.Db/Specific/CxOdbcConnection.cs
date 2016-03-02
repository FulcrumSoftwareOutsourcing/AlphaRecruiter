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
using System.Data.Odbc;
using Framework.Utils;

namespace Framework.Db
{
  /// <summary>
  /// Class that encapsulates ODBC connection.
  /// </summary>
  public class CxOdbcConnection: CxDbConnection
  {
    //--------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    protected internal CxOdbcConnection()
    {
      m_Connection = new OdbcConnection();
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Creates an appropriate instance of Script Generator.
    /// </summary>
    /// <returns></returns>
    protected override CxDbScriptGenerator CreateScriptGenerator()
    {
      return new CxOdbcScriptGenerator(this);
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Composes connection string for the given provider and database.
    /// </summary>
    /// <param name="providerType">type of data provider to use</param>
    /// <param name="databaseType">type of the database to use</param>
    /// <param name="server">name of the server (TNS Alias in case of Oracle)</param>
    /// <param name="database">name of the default database (may be empty)</param>
    /// <param name="userName">user name (may be empty for Windows authentication)</param>
    /// <param name="password">password (may be empty)</param>
    /// <returns>connection string for the given parameters</returns>
    new static public string ComposeConnectionString(NxDataProviderType providerType,
                                                     NxDatabaseType databaseType,
                                                     string server,
                                                     string database,
                                                     string userName,
                                                     string password)
    {
      throw new ExException("CxOdbcConnection.ComposeConnectionString(): Not implemented yet");
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Creates database command.
    /// </summary>
    /// <returns>created command</returns>
    override protected IDbCommand CreateCommand()
    {
      return new OdbcCommand();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates data adapter.
    /// </summary>
    /// <param name="command">database command to create adapter for</param>
    /// <returns>created data adapter</returns>
    override protected DbDataAdapter CreateDataAdapter(IDbCommand command)
    {
      return new OdbcDataAdapter((OdbcCommand) command);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates database parameter.
    /// </summary>
    /// <returns>created parameter</returns>
    override protected IDataParameter CreateParameter()
    {
      return new OdbcParameter();
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
      OdbcParameter parameter = (OdbcParameter) CreateParameter();
      parameter.OdbcType = (lobType == NxLobType.NClob ? OdbcType.NVarChar :
                             lobType == NxLobType.Clob ? OdbcType.VarChar :
                                                          OdbcType.VarBinary);
      parameter.Size = size;
      return parameter;
    }
    //----------------------------------------------------------------------------
    public override string GetPureParamName(string name)
    {
      string s = name.ToUpper();
      return (s.StartsWith("@") ? s.Substring(1, s.Length - 1) : s);
    }
    //----------------------------------------------------------------------------
  }
}
