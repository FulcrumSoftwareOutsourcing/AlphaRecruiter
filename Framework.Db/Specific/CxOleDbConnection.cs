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
using System.Data.OleDb;

using Framework.Utils;

namespace Framework.Db
{
	/// <summary>
	/// Class that encapsulates OLE DB connection.
	/// </summary>
  public class CxOleDbConnection : CxDbConnection
  {
    //--------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    protected internal CxOleDbConnection()
    {
      m_Connection = new OleDbConnection();
    }
    //--------------------------------------------------------------------------
	  /// <summary>
	  /// Creates an appropriate instance of Script Generator.
	  /// </summary>
	  /// <returns></returns>
	  protected override CxDbScriptGenerator CreateScriptGenerator()
	  {
      return new CxOleDbScriptGenerator(this);
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
      if (server != database || server == database)
        throw new ExException("CxOleDbConnection.ComposeConnectionString(): Not implemented yet");
      return null;
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Creates database command.
    /// </summary>
    /// <returns>created command</returns>
    override protected IDbCommand CreateCommand()
    {
      return new OleDbCommand();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates data adapter.
    /// </summary>
    /// <param name="command">database command to create adapter for</param>
    /// <returns>created data adapter</returns>
    override protected DbDataAdapter CreateDataAdapter(IDbCommand command)
    {
      return new OleDbDataAdapter((OleDbCommand) command);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates database parameter.
    /// </summary>
    /// <returns>created parameter</returns>
    override protected IDataParameter CreateParameter()
    {
      return new OleDbParameter();
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
      OleDbParameter parameter = (OleDbParameter) CreateParameter();
      parameter.OleDbType = (lobType == NxLobType.NClob ? OleDbType.LongVarWChar : 
                             lobType == NxLobType.Clob  ? OleDbType.LongVarChar : 
                                                          OleDbType.LongVarBinary);
      parameter.Size = size;
      return parameter;
    }
    //----------------------------------------------------------------------------
    public override string GetPureParamName(string name)
    {
      return "?";
    }
    //----------------------------------------------------------------------------
  }
}
