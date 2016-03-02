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
using System.Data.SqlClient;

using Framework.Utils;

namespace Framework.Db
{
  /// <summary>
  /// Class that encapsulates SQL Server connection.
  /// </summary>
  public class CxSqlConnection : CxDbConnection
  {
    //--------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    protected internal CxSqlConnection()
    {
      m_Connection = new SqlConnection();
      m_DatabaseType = NxDatabaseType.SqlServer;
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Composes connection string.
    /// </summary>
    /// <param name="server">name of the server</param>
    /// <param name="database"></param>
    /// <param name="userName">user name (may be empty for Windows authentication)</param>
    /// <param name="password">password (may be empty)</param>
    /// <returns>connection string for the given parameters</returns>
    static public string ComposeConnectionString(string server, 
                                                 string database, 
                                                 string userName, 
                                                 string password)
    {
      string s = string.Format("Server={0};Initial Catalog={1};", server, database);
      s += (CxUtils.IsEmpty(userName) ? "Trusted_Connection=true;" :
                                         string.Format("User ID={0};Pwd={1};", userName, password));
      return s;
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Creates database command.
    /// </summary>
    /// <returns>created command</returns>
    override protected IDbCommand CreateCommand()
    {
      return new SqlCommand();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates data adapter.
    /// </summary>
    /// <param name="command">database command to create adapter for</param>
    /// <returns>created data adapter</returns>
    override protected DbDataAdapter CreateDataAdapter(IDbCommand command)
    {
      return new SqlDataAdapter((SqlCommand) command);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates database parameter.
    /// </summary>
    /// <returns>created parameter</returns>
    override protected IDataParameter CreateParameter()
    {
      return new SqlParameter();
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
      SqlParameter parameter = (SqlParameter) CreateParameter();
      parameter.SqlDbType = (lobType == NxLobType.NClob ? SqlDbType.NText : 
                             lobType == NxLobType.Clob  ? SqlDbType.Text : 
                             lobType == NxLobType.NewBlob ? SqlDbType.VarBinary :
                                                          SqlDbType.Image);
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
      string s = name.ToUpper();
      return (s.StartsWith("@") ? s.Substring(1, s.Length - 1) : s);
    }
    //----------------------------------------------------------------------------
    protected override CxDbScriptGenerator CreateScriptGenerator()
    {
      return new CxSqlScriptGenerator(this);
    }
  }
}