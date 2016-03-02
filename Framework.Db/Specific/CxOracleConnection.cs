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
using System.Data.OracleClient;
using System.Collections;
using System.Text;

using Framework.Utils;
using System.Collections.Generic;

namespace Framework.Db
{
  /// <summary>
  /// Class that encapsulates Oracle connection.
  /// </summary>
  public class CxOracleConnection : CxDbConnection
  {
    //--------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    protected internal CxOracleConnection()
    {
      m_Connection = new OracleConnection();
      m_DatabaseType = NxDatabaseType.Oracle;
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Composes connection string.
    /// </summary>
    /// <param name="tnsAlias">TNS Alias</param>
    /// <param name="userName">user name</param>
    /// <param name="password">password</param>
    /// <returns>connection string for the given parameters</returns>
    static public string ComposeConnectionString(string tnsAlias, string userName, string password)
    {
      string s = string.Format("Server={0};User ID={1};Pwd={2};", tnsAlias, userName, password);
      return s;
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Creates database command.
    /// </summary>
    /// <returns>created command</returns>
    override protected IDbCommand CreateCommand()
    {
      return new OracleCommand();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates data adapter.
    /// </summary>
    /// <param name="command">database command to create adapter for</param>
    /// <returns>created data adapter</returns>
    override protected DbDataAdapter CreateDataAdapter(IDbCommand command)
    {
      return new OracleDataAdapter((OracleCommand) command);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates database parameter.
    /// </summary>
    /// <returns>created parameter</returns>
    override protected IDataParameter CreateParameter()
    {
      return new OracleParameter();
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
      OracleParameter parameter = (OracleParameter) CreateParameter();
      parameter.OracleType = (lobType == NxLobType.Clob  ? OracleType.Clob :
                              lobType == NxLobType.NClob ? OracleType.NClob :   
                                                           OracleType.Blob);
      parameter.Size = size;
      return parameter;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Prepares some kinds of parameters depending on database type.
    /// </summary>
    /// <param name="command">command to prepare parameters for</param>
    /// <param name="mayReturnResultSet">true if command may return result set</param>
    override protected void PrepareParameters(CxDbCommand command, bool mayReturnResultSet)
    {
      if (mayReturnResultSet)
      {
        AddRefCursorParameters(command);
      }
      SetLobValues(command);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Adds ref cursor parameters to the command.
    /// </summary>
    /// <param name="command">command to prepare parameters for</param>
    protected void AddRefCursorParameters(CxDbCommand command)
    {
      string s = command.CommandText.ToUpper();
      IList<string> parts = CxText.DecomposeWithSeparator(s, ".");
      int count = parts.Count;
      if (count == 1) return; // Not packaged procedure can't return ref cursor
      string packageName = parts[count - 2];
      string owner = (count == 3 ? "'" + parts[0] + "'" : "user");
      string objectName = parts[count - 1];
      string sql = "select argument_name\r\n" +  
        "  from all_arguments\r\n" +  
        " where owner = " + owner + "\r\n" +  
        "   and package_name = :av_package_name\r\n" +  
        "   and object_name = :av_object_name\r\n" + 
        "   and data_type = 'REF CURSOR'\r\n" +  
        "   and in_out = 'OUT'";
      IDataReader reader = ExecuteReader(sql, packageName, objectName);
      while (reader.Read())
      {
        string paramName = (string) reader["argument_name"];
        if (command.FindParameter(paramName) == null)
        {
          OracleParameter parameter = (OracleParameter) CreateParameter();
          parameter.OracleType = OracleType.Cursor;
          command.AddParameter(CreateParameter(parameter, paramName, null, ParameterDirection.Output, DbType.Object));
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Sets values for input LOB parameters.
    /// </summary>
    /// <param name="command">command to prepare parameters for</param>
    protected void SetLobValues(CxDbCommand command)
    {
      ArrayList lobParameters = new ArrayList();
      foreach (CxDbParameter parameter in command.Parameters)
      {
        OracleParameter oraParameter = (OracleParameter) parameter.Parameter;
        if ((oraParameter.Direction == ParameterDirection.Input) &&
            (oraParameter.OracleType == OracleType.Blob ||
             oraParameter.OracleType == OracleType.Clob ||
             oraParameter.OracleType == OracleType.NClob) &&
            ! CxUtils.IsNull(parameter.Value))
        {
          lobParameters.Add(parameter);
        }
      }
      int count = lobParameters.Count;
      if (count == 0) return;

      if ( ! command.Connection.InTransaction )
      {
        throw new ExDbException("Transaction should be open to work with input LOB parameters", command.CommandText);
      }
      foreach (CxDbParameter parameter in lobParameters)
      {
        OracleParameter oraParameter = (OracleParameter) parameter.Parameter;
        OracleType oracleType = oraParameter.OracleType;
        string type = oracleType.ToString();
        OracleCommand cmd = (OracleCommand) Connection.CreateCommand();
        cmd.Transaction = (OracleTransaction) Transaction;
        cmd.CommandText = "declare xx " + type +"; begin dbms_lob.createtemporary(xx, false, 0); :templob := xx; end;";
        cmd.Parameters.Add(new OracleParameter("templob", oracleType)).Direction = ParameterDirection.Output;
        cmd.ExecuteNonQuery();
        OracleLob tempLob = (OracleLob) cmd.Parameters[0].Value;
        tempLob.BeginBatch(OracleLobOpenMode.ReadWrite);
        byte[] buffer = (parameter.Value is byte[] ? (byte[]) parameter.Value :
                         Encoding.Unicode.GetBytes((string) parameter.Value));
        tempLob.Write(buffer, 0, buffer.Length);
        tempLob.EndBatch();

        parameter.Value = tempLob;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns FROM clause for SELECT statement returning an expression
    /// (returns FROM DUAL for Oracle, or empty string for MSSQL)
    /// </summary>
    override public string GetOneRowFromClause()
    {
      return "FROM DUAL";
    }
    //----------------------------------------------------------------------------
    protected override CxDbScriptGenerator CreateScriptGenerator()
    {
      return new CxOracleScriptGenerator(this);
    }
    //----------------------------------------------------------------------------
  }
}
