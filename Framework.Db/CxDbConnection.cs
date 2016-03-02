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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using Framework.Db.LightWeight;
using Framework.Utils;
using Framework.Common;

namespace Framework.Db
{
  //----------------------------------------------------------------------------
  /// <summary>
  /// Enumeration for data provider types.
  /// </summary>
  public enum NxDataProviderType { OleDb, SqlClient, OracleClient, WebServiceClient, Odbc }
  //----------------------------------------------------------------------------
  /// <summary>
  /// Enumeration for suported database types.
  /// </summary>
  public enum NxDatabaseType { Unknown, SqlServer, Oracle }
  //----------------------------------------------------------------------------
  /// <summary>
  /// Enumeration for parameter LOB types.
  /// </summary>
  public enum NxLobType { Blob, NewBlob, Clob, NClob }
  //------------------------------------------------------------------------------
  /// <summary>
  /// Enumeration for different kinds of SQL execution result.
  /// </summary>
  public enum NxSqlResult { None, RowsAffected, Scalar, DataSet, SchemaTable }
  //------------------------------------------------------------------------------
  /// <summary>
  /// Delegate method on Begin Transaction.
  /// </summary>
  public delegate void DxBeginTransaction(CxDbConnection connection);
  //----------------------------------------------------------------------------
  /// <summary>
  /// Delegate method to execute some actions that require DB connection.
  /// </summary>
  public delegate void DxDbMethod(CxDbConnection connection, object parameter);
  //----------------------------------------------------------------------------
  /// <summary>
  /// Delegate to create database connection.
  /// </summary>
  /// <returns>created database connection</returns>
  public delegate CxDbConnection DxCreateDbConnection();
  //---------------------------------------------------------------------------
  /// <summary>
  /// Class that encapsulates database connection.
  /// 
  /// Implements IDisposable to use in using() construct.
  /// Capable to log all executed statements along with their parameters.
  /// Throws special exception instead of original database one. This exception
  /// encapsulates SQL statement, parameter values and an original exeption.
  /// </summary>
  abstract public class CxDbConnection : IDisposable
  {
    //--------------------------------------------------------------------------
    protected bool m_Logging; // If true logs all SQL operations
    protected bool m_ErrorLogging; // If true logs all SQL exceptions
    protected int m_SqlCallId; // ID of SQL call
    protected ArrayList m_Loggers = new ArrayList(); // List of active loggers
    protected int m_DefaultCommandTimeout; // Default timeout for commands
    private bool m_IsDisposed;
    static protected object m_LogLock = new object(); // Synchronization object for locking
    // Command timeout specified in the config file
    static protected int m_ConfigCommandTimeout = CxConfigurationHelper.DefaultCommandTimeout;
    //----------------------------------------------------------------------------
    protected IDbConnection m_Connection; // ADO.NET connection this class encapsulates
    protected NxDataProviderType m_ProviderType; // Data provider type connection uses
    protected NxDatabaseType m_DatabaseType = NxDatabaseType.Unknown; // Database type
    protected IDbTransaction m_Transaction; // Current transaction object
    private CxDbScriptGenerator m_ScriptGenerator;
    //----------------------------------------------------------------------------
    /// <summary>
    /// A container of methods that generate scripts 
    /// for a particular DB server type.
    /// </summary>
    public CxDbScriptGenerator ScriptGenerator
    {
      get { return m_ScriptGenerator; }
      set { m_ScriptGenerator = value; }
    }
    //----------------------------------------------------------------------------
    public bool IsDisposed
    {
      get { return m_IsDisposed; }
      protected set { m_IsDisposed = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Default ctor.
    /// </summary>
    protected CxDbConnection()
    {
      ScriptGenerator = CreateScriptGenerator();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates database connection.
    /// </summary>
    /// <param name="providerType">type of data provider to use</param>
    /// <param name="connectionString">connecton string</param>
    /// <returns>created connection</returns>
    static public CxDbConnection Create(NxDataProviderType providerType, string connectionString)
    {
      CxDbConnection connection;
      switch (providerType)
      {
        case NxDataProviderType.OleDb:
          connection = new CxOleDbConnection();
          break;
        case NxDataProviderType.OracleClient:
          connection = new CxOracleConnection();
          break;
        case NxDataProviderType.SqlClient:
          connection = new CxSqlConnection();
          break;
        case NxDataProviderType.WebServiceClient:
          connection = new CxWebConnection();
          break;
        case NxDataProviderType.Odbc:
          connection = new CxOdbcConnection();
          break;
        default:
          throw new ExDbException("Unsupported provider connection type: " + providerType, "");
      }
      connection.Connection.ConnectionString = connectionString;
      connection.m_ProviderType = providerType;
      connection.m_DefaultCommandTimeout = m_ConfigCommandTimeout;
      connection.Open();
      return connection;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates database connection.
    /// </summary>
    /// <param name="providerType">type of data provider to use</param>
    /// <param name="databaseType">type of the database to use</param>
    /// <param name="server">name of the server TNS Alias in case of Oracle)</param>
    /// <param name="database">name of the default database (may be empty)</param>
    /// <param name="userName">user name (may be empty for Windows authentication)</param>
    /// <param name="password">password (may be empty)</param>
    /// <returns>created connection</returns>
    static public CxDbConnection Create(NxDataProviderType providerType,
                                        NxDatabaseType databaseType,
                                        string server,
                                        string database,
                                        string userName,
                                        string password)
    {
      string connectionString = ComposeConnectionString(providerType, databaseType, server, database, userName, password);
      return Create(providerType, connectionString);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates an appropriate instance of Script Generator.
    /// </summary>
    /// <returns></returns>
    abstract protected CxDbScriptGenerator CreateScriptGenerator();
    //----------------------------------------------------------------------------
    /// <summary>
    /// Composes connection string for the given provider and database type.
    /// </summary>
    /// <param name="providerType">type of data provider to use</param>
    /// <param name="databaseType">type of the database to use</param>
    /// <param name="server">name of the server (TNS Alias in case of Oracle)</param>
    /// <param name="database">name of the default database (may be empty)</param>
    /// <param name="userName">user name (may be empty for Windows authentication)</param>
    /// <param name="password">password (may be empty)</param>
    /// <returns>connection string for the given parameters</returns>
    static public string ComposeConnectionString(NxDataProviderType providerType,
                                                 NxDatabaseType databaseType,
                                                 string server,
                                                 string database,
                                                 string userName,
                                                 string password)
    {
      string s;
      switch (providerType)
      {
        case NxDataProviderType.OleDb:
          s = CxOleDbConnection.ComposeConnectionString(providerType, databaseType, server, database, userName, password);
          break;
        case NxDataProviderType.OracleClient:
          s = CxOracleConnection.ComposeConnectionString(server, userName, password);
          break;
        case NxDataProviderType.SqlClient:
          s = CxSqlConnection.ComposeConnectionString(server, database, userName, password);
          break;
        case NxDataProviderType.Odbc:
          s = CxOleDbConnection.ComposeConnectionString(providerType, databaseType, server, database, userName, password);
          break;
        default:
          throw new ExDbException("Unsupported provider connection type: " + providerType, "");
      }
      return s;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Frees allocated resources.
    /// </summary>
    public void Dispose()
    {
      if (!IsDisposed)
      {
        Close();
        m_Connection.Dispose();
        IsDisposed = true;
      }
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Original database connection.
    /// </summary>
    public IDbConnection Connection
    {
      get { return m_Connection; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Data provider type connection uses.
    /// </summary>
    public NxDataProviderType ProviderType
    {
      get { return m_ProviderType; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Database type.
    /// </summary>
    virtual public NxDatabaseType DatabaseType
    {
      get { return m_DatabaseType; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Default timeout for commands.
    /// </summary>
    public int DefaultCommandTimeout
    {
      get { return m_DefaultCommandTimeout; }
      set { m_DefaultCommandTimeout = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// If true logs all SQL operations.
    /// </summary>
    public bool Logging
    {
      get { return m_Logging; }
      set { m_Logging = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// If true logs all SQL exceptions.
    /// </summary>
    public bool ErrorLogging
    {
      get { return m_ErrorLogging || Logging; }
      set { m_ErrorLogging = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Opens connection and returns ADO.NET connection object.
    /// </summary>
    /// <returns>ADO.NET connection object</returns>
    public IDbConnection Open()
    {
      m_Connection.Open();
      return m_Connection;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Closes connection. If exception was raised during closing returns it 
    /// instead of throwing.
    /// </summary>
    /// <returns>exception if occurred during closing or null if closing was successful</returns>
    public Exception Close()
    {
      try
      {
        m_Connection.Close();
        return null;
      }
      catch (Exception e)
      {
        return e;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Opens transaction and returns appropriate object.
    /// if connection already in transaction raises exception.
    /// </summary>
    /// <returns>ADO.NET transaction object</returns>
    public IDbTransaction BeginTransaction()
    {
      return BeginTransaction(IsolationLevel.ReadCommitted);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Opens transaction and returns appropriate object.
    /// if connection already in transaction raises exception.
    /// </summary>
    /// <param name="isolationLevel">transaction isolation level</param>
    /// <returns>ADO.NET transaction object</returns>
    public IDbTransaction BeginTransaction(IsolationLevel isolationLevel)
    {
      if (InTransaction)
      {
        throw new ExDbException("Transaction already open", "");
      }
      m_Transaction = m_Connection.BeginTransaction(isolationLevel);
      if (m_Transaction != null && OnBeginTransaction != null)
      {
        OnBeginTransaction(this);
      }
      return m_Transaction;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Commits open transaction.
    /// </summary>
    public void Commit()
    {
      if (!InTransaction)
      {
        throw new ExDbException("There is no open transaction", "");
      }
      m_Transaction.Commit();
      m_Transaction = null;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Rollbacks open transaction. If exception was raised during rollback returns it 
    /// instead of throwing.
    /// </summary>
    /// <returns>exception if occurred during rollback or null if rollback was successful</returns>
    public Exception Rollback()
    {
      if (!InTransaction)
      {
        throw new ExDbException("There is no open transaction", "");
      }

      Exception error = null;
      try
      {
        m_Transaction.Rollback();
      }
      catch (Exception e)
      {
        error = e;
      }
      m_Transaction = null;
      return error;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Current transaction object.
    /// </summary>
    public IDbTransaction Transaction
    {
      get { return m_Transaction; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if we are in transaction now.
    /// </summary>
    public bool InTransaction
    {
      get { return (m_Transaction != null); }
    }
    //----------------------------------------------------------------------------
    public CxLwRowList GetQueryResultLightWeight(string sql)
    {
      var result = new CxLwRowList();
      var table = GetQueryResult(sql);
      foreach (DataColumn dataColumn in table.Columns)
      {
        var column = new CxLwColumn()
        {
          Name = dataColumn.ColumnName,
          DataType = dataColumn.DataType
        };
        result.Columns.Add(column);
      }
      foreach (DataRow row in table.Rows)
      {
        var record = new CxLwRow()
        {
          RowList = result
        };
        foreach (DataColumn column in table.Columns)
        {
          record[column.ColumnName] = row[column.ColumnName];
        }
        result.Add(record);
      }
      return result;
    }
    //----------------------------------------------------------------------------
    public void ApplyUpdatesLightWeight(CxLwRowList rowList, string tableName, string keyFieldName)
    {
      List<CxLwRow> rowsToRemove = new List<CxLwRow>();
      foreach (var row in rowList)
      {
        if (row.Status == NxLwRowStatus.Updated)
        {
          var fieldSettings = new List<string>();
          foreach (var column in rowList.Columns)
          {
            if (!string.Equals(column.Name, keyFieldName, StringComparison.OrdinalIgnoreCase))
              fieldSettings.Add(string.Format("{0} = :{1}", ScriptGenerator.GetExplicitFieldName(column.Name), column.Name));
          }
          if (fieldSettings.Count == 0)
            throw new Exception("No fields to update");

          var script = string.Format(
            "update [{0}] set {1} where {2} = :{3}", 
            tableName, string.Join(", ", fieldSettings.ToArray()), 
            ScriptGenerator.GetExplicitFieldName(keyFieldName), 
            keyFieldName);

          var result = ExecuteCommandWithResult(script, row);
          if (result != 1)
            throw new Exception("No records were modified");
        }
        if (row.Status == NxLwRowStatus.New)
        {
          var fieldNames = ScriptGenerator.GetCommaSeparatedFields(rowList.Columns.Where(x => !string.Equals(x.Name, keyFieldName, StringComparison.OrdinalIgnoreCase)).Select(x => x.Name).ToArray());
          var parameterNames = string.Join(", ", rowList.Columns.Where(x => !string.Equals(x.Name, keyFieldName, StringComparison.OrdinalIgnoreCase)).Select(x => ":" + x.Name).ToArray());
          var script = string.Format(
              "insert into [{0}] ({1}) " +
              "values ({2})",
              tableName, fieldNames, parameterNames);
          var result = ExecuteCommandWithResult(script, row);
          if (result != 1)
            throw new Exception("No records were modified");
          row[keyFieldName] = Convert.ChangeType(ExecuteScalar("select @@identity", (IxValueProvider) null), rowList.Columns.FirstOrDefault(x=> string.Equals(x.Name, keyFieldName, StringComparison.OrdinalIgnoreCase)).DataType);
        }
        if (row.Status == NxLwRowStatus.Deleted)
        {
          var result = ExecuteCommandWithResult(
            string.Format(
              "delete from [{0}] where {1} = :{2}", tableName, ScriptGenerator.GetExplicitFieldName(keyFieldName), keyFieldName), row);
          if (result != 1)
            throw new Exception("No records were modified");
          rowsToRemove.Add(row);
        }
        row.Status = NxLwRowStatus.NonChanged;
      }
      rowList.RemoveAll(x => rowsToRemove.Contains(x));
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Populates data table with query result set.
    /// </summary>
    /// <param name="dt">data table to populate</param>
    /// <param name="sql">text of SQL query or stored procedure name</param>
    /// <param name="paramValues">parameter values to use</param>
    public void GetQueryResult(DataTable dt, string sql, params object[] paramValues)
    {
      using (CxDbCommand command = CreateCommand(sql, paramValues))
      {
        GetQueryResult(dt, command);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Populates data table with query result set.
    /// </summary>
    /// <param name="dt">data table to populate</param>
    /// <param name="sql">text of SQL query or stored procedure name</param>
    /// <param name="provider">value provider to get parameter values</param>
    public void GetQueryResult(DataTable dt, string sql, IxValueProvider provider)
    {
      using (CxDbCommand command = CreateCommand(sql, provider))
      {
        GetQueryResult(dt, command);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Populates data table with query result set.
    /// </summary>
    /// <param name="dt">data table to populate</param>
    /// <param name="sql">text of SQL query or stored procedure name</param>
    /// <param name="provider">value provider to get parameter values</param>
    /// <param name="startRecord">starting index of the records that 
    /// should be passed to the data-table</param>
    /// <param name="maxRecords">maximum amount of records to get</param>
    public void GetQueryResult(
      DataTable dt,
      string sql,
      IxValueProvider provider,
      int startRecord,
      int maxRecords)
    {
      using (CxDbCommand command = CreateCommand(sql, provider))
      {
        GetQueryResult(dt, command, startRecord, maxRecords);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Populates data table with query result set.
    /// </summary>
    /// <param name="dt">data table to populate</param>
    /// <param name="sql">text of SQL query or stored procedure name</param>
    /// <param name="provider">value provider to get parameter values</param>
    /// <param name="maxRecords">maximum amount of records to get</param>
    public void GetQueryResult(
      DataTable dt,
      string sql,
      IxValueProvider provider,
      int maxRecords)
    {
      if (maxRecords > 0)
      {
        GetQueryResult(dt, sql, provider, 0, maxRecords);
      }
      else
      {
        GetQueryResult(dt, sql, provider);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Populates data table with query result set.
    /// </summary>
    /// <param name="dt">data table to populate</param>
    /// <param name="command">database command to execute</param>
    /// <param name="paramValues">parameter values to use</param>
    public void GetQueryResult(DataTable dt, CxDbCommand command, params object[] paramValues)
    {
      GetQueryResult(dt, command, -1, -1, paramValues);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Populates data table with query result set.
    /// </summary>
    /// <param name="dt">data table to populate</param>
    /// <param name="command">database command to execute</param>
    /// <param name="startRecord">number of record to start fetching from</param>
    /// <param name="maxRecords">maximal number of records to fetch</param>
    /// <param name="paramValues">parameter values to use</param>
    public void GetQueryResult(DataTable dt,
                               CxDbCommand command,
                               int startRecord,
                               int maxRecords,
                               params object[] paramValues)
    {
      int logId = 0;
      DoOnSqlBegin();
      try
      {
        if (command.Command.CommandType == CommandType.StoredProcedure)
        {
          PrepareParameters(command, true);
        }
        command.SetParamValues(paramValues);
        logId = Log(command);
        using (DbDataAdapter da = CreateDataAdapter(command))
        {
          dt.Clear();
          if (dt.DataSet != null && startRecord >= 0)
          {
            da.Fill(dt.DataSet, startRecord, maxRecords, dt.TableName);
          }
          else
          {
            da.Fill(dt);
          }
        }
        Log(logId);
      }
      catch (Exception e)
      {
        Log(logId, command, e);
        throw new ExDbException(command.CommandText, paramValues, e);
      }
      finally
      {
        DoOnSqlEnd();
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Populates data table with query result set and returns it.
    /// </summary>
    /// <param name="sql">text of SQL query or stored procedure name</param>
    /// <param name="paramValues">parameter values to use</param>
    /// <returns>data table with query result set</returns>
    public CxGenericDataTable GetQueryResult(string sql, params object[] paramValues)
    {
      using (CxDbCommand command = CreateCommand(sql, paramValues))
      {
        return GetQueryResult(command);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Populates data table with query result set and returns it.
    /// </summary>
    /// <param name="command">database command to execute</param>
    /// <param name="paramValues">parameter values to use</param>
    /// <returns>data table with query result set</returns>
    public CxGenericDataTable GetQueryResult(CxDbCommand command, params object[] paramValues)
    {
      CxGenericDataTable dt = new CxGenericDataTable();
      GetQueryResult(dt, command, paramValues);
      return dt;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Populates data table with query result set and returns it.
    /// </summary>
    /// <param name="queryDescriptor">a query to execute</param>
    /// <returns>data table with query result set</returns>
    public CxGenericDataTable GetQueryResult(CxQueryDescriptor queryDescriptor)
    {
      CxDbCommand command = CreateCommand(queryDescriptor.CommandText, CreateParameters(queryDescriptor.Parameters.ToArray()));
      return GetQueryResult(command, new object[] { });
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Populates data table with stored procedure result set and returns it.
    /// </summary>
    /// <param name="procName">stored procedure name</param>
    /// <param name="parameters">parameters to use</param>
    /// <returns>data table with stored procedure result set</returns>
    public DataTable GetQueryResultSP(string procName, params CxDbParameter[] parameters)
    {
      using (CxDbCommand command = CreateCommandSP(procName, parameters))
      {
        return GetQueryResult(command);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Populates data table with stored procedure result set and returns it.
    /// </summary>
    /// <param name="dt">data table to populate</param>
    /// <param name="procName">stored procedure name</param>
    /// <param name="startRecord">number of record to start fetching from</param>
    /// <param name="maxRecords">maximal number of records to fetch</param>
    /// <param name="parameters">parameters to use</param>
    public void GetQueryResultSP(DataTable dt,
                                 string procName,
                                 int startRecord,
                                 int maxRecords,
                                 params CxDbParameter[] parameters)
    {
      using (CxDbCommand command = CreateCommandSP(procName, parameters))
      {
        GetQueryResult(dt, command, startRecord, maxRecords);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns data reader for the given SQL SELECT statement.
    /// </summary>
    /// <param name="sql">text of SQL query</param>
    /// <param name="paramValues">parameter values to use</param>
    /// <returns>data reader for the given SQL Select statement of stored procedure</returns>
    public IDataReader ExecuteReader(string sql, params object[] paramValues)
    {
      return ExecuteReader(CommandBehavior.Default, sql, paramValues);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns data reader for the given SQL SELECT statement.
    /// </summary>
    /// <param name="commandBehavior">data reader behaviour</param>
    /// <param name="sql">text of SQL query</param>
    /// <param name="paramValues">parameter values to use</param>
    /// <returns>data reader for the given SQL Select statement of stored procedure</returns>
    public IDataReader ExecuteReader(CommandBehavior commandBehavior,
                                     string sql,
                                     params object[] paramValues)
    {
      using (CxDbCommand command = CreateCommand(sql, paramValues))
      {
        return ExecuteReader(commandBehavior, command);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns data reader for the given SQL SELECT statement.
    /// </summary>
    /// <param name="command">database command to execute</param>
    /// <param name="paramValues">parameter values to use</param>
    /// <returns>data reader for the given SQL Select statement of stored procedure</returns>
    public IDataReader ExecuteReader(CxDbCommand command, params object[] paramValues)
    {
      return ExecuteReader(CommandBehavior.Default, command, paramValues);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns data reader for the given SQL SELECT statement.
    /// </summary>
    /// <param name="commandBehavior">data reader behaviour</param>
    /// <param name="command">database command to execute</param>
    /// <param name="paramValues">parameter values to use</param>
    /// <returns>data reader for the given SQL Select statement of stored procedure</returns>
    public IDataReader ExecuteReader(CommandBehavior commandBehavior,
                                     CxDbCommand command,
                                     params object[] paramValues)
    {
      int logId = 0;
      DoOnSqlBegin();
      try
      {
        if (command.Command.CommandType == CommandType.StoredProcedure)
        {
          PrepareParameters(command, true);
        }
        command.SetParamValues(paramValues);
        logId = Log(command);
        IDataReader dataReader = command.Command.ExecuteReader(commandBehavior);
        Log(logId);
        return dataReader;
      }
      catch (Exception e)
      {
        Log(logId, command, e);
        throw new ExDbException(command.CommandText, paramValues, e);
      }
      finally
      {
        DoOnSqlEnd();
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns data reader for the given stored procedure.
    /// </summary>
    /// <param name="procName">name of the stored proceduer to call</param>
    /// <param name="parameters">parameter to use</param>
    /// <returns>data reader for the given SQL Select statement of stored procedure</returns>
    public IDataReader ExecuteReaderSP(string procName, params CxDbParameter[] parameters)
    {
      return ExecuteReaderSP(CommandBehavior.Default, procName, parameters);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns data reader for the given stored procedure.
    /// </summary>
    /// <param name="commandBehavior">data reader behaviour</param>
    /// <param name="procName">name of the stored proceduer to call</param>
    /// <param name="parameters">parameter to use</param>
    /// <returns>data reader for the given SQL Select statement of stored procedure</returns>
    public IDataReader ExecuteReaderSP(CommandBehavior commandBehavior,
                                       string procName,
                                       params CxDbParameter[] parameters)
    {
      using (CxDbCommand command = CreateCommandSP(procName, parameters))
      {
        return ExecuteReader(commandBehavior, command);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns the first column of the first row in the resultset returned by the query
    /// or value of expression.
    /// </summary>
    /// <param name="sql">SQL SELECT statement or expression to calculate</param>
    /// <param name="paramValues">parameter values to use</param>
    /// <returns>the first column of the first row in the resultset returned by the query</returns>
    public object ExecuteScalar(string sql, params object[] paramValues)
    {
      using (CxDbCommand command = CreateCommand(sql, paramValues))
      {
        return ExecuteScalar(command);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns the first column of the first row in the resultset returned by the query
    /// or value of expression.
    /// </summary>
    /// <param name="command">SQL command to execute</param>
    /// <param name="paramValues">parameter values to use</param>
    /// <returns>the first column of the first row in the resultset returned by the query</returns>
    public object ExecuteScalar(CxDbCommand command, params object[] paramValues)
    {
      int logId = 0;
      DoOnSqlBegin();
      try
      {
        if (command.Command.CommandType == CommandType.StoredProcedure)
        {
          PrepareParameters(command, true);
        }
        command.SetParamValues(paramValues);
        logId = Log(command);
        object result = command.Command.ExecuteScalar();
        Log(logId);
        return result;
      }
      catch (Exception e)
      {
        Log(logId, command, e);
        throw new ExDbException(command.CommandText, paramValues, e);
      }
      finally
      {
        DoOnSqlEnd();
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns the first column of the first row in the resultset returned by the query
    /// or value of expression.
    /// </summary>
    /// <param name="sql">SQL statement to execute</param>
    /// <param name="valueProvider">parameter values to use</param>
    /// <returns>the first column of the first row in the resultset returned by the query</returns>
    public object ExecuteScalar(string sql, IxValueProvider valueProvider)
    {
      using (CxDbCommand command = CreateCommand(sql, valueProvider))
      {
        return ExecuteScalar(command);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns the first column of the first row in the resultset returned 
    /// by the stored procedure.
    /// </summary>
    /// <param name="procName">name of the stored procedure to execute</param>
    /// <param name="parameters">parameter values to use</param>
    /// <returns>the first column of the first row in the resultset returned 
    /// by the stored procedure</returns>
    public object ExecuteScalarSP(string procName, params CxDbParameter[] parameters)
    {
      using (CxDbCommand command = CreateCommandSP(procName, parameters))
      {
        return ExecuteScalar(command);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Executes SQL command and returns number of affected rows.
    /// </summary>
    /// <param name="sql">SQL statement to execute</param>
    /// <param name="paramValues">parameter values to use</param>
    public void ExecuteCommand(string sql, params object[] paramValues)
    {
      using (CxDbCommand command = CreateCommand(sql, paramValues))
      {
        ExecuteCommand(command);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Executes SQL command and returns number of affected rows.
    /// </summary>
    /// <param name="sql">SQL statement to execute</param>
    /// <param name="valueProvider">parameter values to use</param>
    public void ExecuteCommand(string sql, IxValueProvider valueProvider)
    {
      using (CxDbCommand command = CreateCommand(sql, valueProvider))
      {
        ExecuteCommand(command);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Executes SQL command and returns number of affected rows.
    /// </summary>
    /// <param name="command">SQL command to execute</param>
    /// <param name="paramValues">parameter values to use</param>
    public void ExecuteCommand(CxDbCommand command, params object[] paramValues)
    {
      InternalExecuteCommand(command, false, paramValues);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Executes SQL command and returns number of affected rows.
    /// </summary>
    /// <param name="sql">SQL statement to execute</param>
    /// <param name="paramValues">parameter values to use</param>
    /// <returns>number of affected rows</returns>
    public int ExecuteCommandWithResult(string sql, params object[] paramValues)
    {
      using (CxDbCommand command = CreateCommand(sql, paramValues))
      {
        return ExecuteCommandWithResult(command);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Executes SQL command and returns number of affected rows.
    /// </summary>
    /// <param name="sql">SQL statement to execute</param>
    /// <param name="valueProvider">parameter values to use</param>
    /// <returns>number of affected rows</returns>
    public int ExecuteCommandWithResult(string sql, IxValueProvider valueProvider)
    {
      using (CxDbCommand command = CreateCommand(sql, valueProvider))
      {
        return ExecuteCommandWithResult(command);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Executes SQL command and returns number of affected rows.
    /// </summary>
    /// <param name="command">SQL command to execute</param>
    /// <param name="paramValues">parameter values to use</param>
    /// <returns>number of affected rows</returns>
    public int ExecuteCommandWithResult(CxDbCommand command, params object[] paramValues)
    {
      return InternalExecuteCommand(command, true, paramValues);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Executes SQL command and returns number of affected rows.
    /// </summary>
    /// <param name="command">SQL command to execute</param>
    /// <param name="returnResult">true if result should be returned</param>
    /// <param name="paramValues">parameter values to use</param>
    /// <returns>number of affected rows</returns>
    protected int InternalExecuteCommand(
      CxDbCommand command,
      bool returnResult,
      params object[] paramValues)
    {
      int logId = 0;
      DoOnSqlBegin();
      try
      {
        if (command.Command.CommandType == CommandType.StoredProcedure)
        {
          PrepareParameters(command, false);
        }
        command.SetParamValues(paramValues);
        logId = Log(command);
        int result;
        if (returnResult && command.Command is IxDbCommandEx)
        {
          result = ((IxDbCommandEx) (command.Command)).ExecuteNonQueryWithResult();
        }
        else
        {
          result = command.Command.ExecuteNonQuery();
        }
        Log(logId);
        return result;
      }
      catch (Exception e)
      {
        Log(logId, command, e);
        throw new ExDbException(command.CommandText, paramValues, e);
      }
      finally
      {
        DoOnSqlEnd();
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Executes SQL stored procedure and returns number of affected rows.
    /// </summary>
    /// <param name="procName">name of procedure to execute</param>
    /// <param name="parameters">parameter to use</param>
    /// <returns>number of affected rows</returns>
    public void ExecuteCommandSP(string procName, params CxDbParameter[] parameters)
    {
      using (CxDbCommand command = CreateCommandSP(procName, parameters))
      {
        InternalExecuteCommand(command, false);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Executes SQL stored procedure and returns number of affected rows.
    /// </summary>
    /// <param name="procName">name of procedure to execute</param>
    /// <param name="parameters">parameter to use</param>
    /// <returns>number of affected rows</returns>
    public int ExecuteCommandWithResultSP(string procName, params CxDbParameter[] parameters)
    {
      using (CxDbCommand command = CreateCommandSP(procName, parameters))
      {
        return InternalExecuteCommand(command, true);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Composes SQL SELECT statement from parts.
    /// </summary>
    /// <param name="columnList">list of columns to select</param>
    /// <param name="tableList">list of tables to select from</param>
    /// <param name="whereClause">WHERE clause</param>
    /// <param name="orderByClause">ORDER BY clause</param>
    /// <param name="groupByClause">GROUP BY clause</param>
    /// <param name="havingClause">HAVING clause</param>
    /// <returns>SQL SELECT statement composed from parts</returns>
    public string ComposeQuery(
      string columnList,
      string tableList,
      string whereClause,
      string orderByClause,
      string groupByClause,
      string havingClause)
    {
      string sql =
        "select " + columnList + "\r\n" +
        "  from " + tableList + "\r\n" +
        (CxUtils.NotEmpty(CxText.TrimSpace(whereClause)) ? " where " + whereClause + "\r\n" : "") +
        (CxUtils.NotEmpty(CxText.TrimSpace(groupByClause)) ? " group by " + groupByClause + "\r\n" : "") +
        (CxUtils.NotEmpty(CxText.TrimSpace(havingClause)) ? " having " + havingClause + "\r\n" : "") +
        (CxUtils.NotEmpty(CxText.TrimSpace(orderByClause)) ? " order by " + orderByClause : "");
      return sql;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns list of query columns.
    /// </summary>
    /// <param name="sql">SQL SELECT statement or stored procedure to get columns from</param>
    /// <returns>list of query columns</returns>
    public IList<DataColumn> ListQueryColumns(string sql)
    {
      int logId = 0;
      using (CxDbCommand command = CreateCommand(sql, new object[0]))
      {
        DoOnSqlBegin();
        try
        {
          DataTable table = new DataTable();
          logId = Log(command);
          using (DbDataAdapter da = CreateDataAdapter(command))
          {
            da.FillSchema(table, SchemaType.Source);
          }
          Log(logId);
          IList<DataColumn> list = new List<DataColumn>();
          foreach (DataColumn column in table.Columns)
          {
            list.Add(column);
          }
          return list;
        }
        catch (Exception e)
        {
          Log(logId, command, e);
          throw new ExDbException(sql, null, e);
        }
        finally
        {
          DoOnSqlEnd();
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns list of query columns names.
    /// </summary>
    /// <param name="sql">SQL SELECT statement or stored procedure to get columns from</param>
    /// <returns>list of query column names</returns>
    public StringCollection ListQueryColumnNames(string sql)
    {
      IList<DataColumn> columnList = ListQueryColumns(sql);
      StringCollection list = new StringCollection();
      foreach (DataColumn column in columnList)
      {
        list.Add(column.ColumnName);
      }
      return list;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates database command for the given SQL statement.
    /// </summary>
    /// <param name="sql">SQL statement to create command for</param>
    /// <returns>database command for the given SQL statement</returns>
    public CxDbCommand CreateCommand(string sql)
    {
      return CreateCommand(sql, new CxDbParameter[0]);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates database command for the given SQL statement.
    /// </summary>
    /// <param name="sql">SQL statement to create command for</param>
    /// <param name="paramValues">values of statement parameters</param>
    /// <returns>database command for the given SQL statement</returns>
    public CxDbCommand CreateCommand(string sql, params object[] paramValues)
    {
      IDbCommand command = CreateCommand();
      SetCommandProps(command, CommandType.Text);
      return new CxDbCommand(this, command, sql, paramValues);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates database command for the given SQL statement.
    /// </summary>
    /// <param name="sql">SQL statement to create command for</param>
    /// <param name="parameters">statement parameters</param>
    /// <returns>database command for the given SQL statement</returns>
    public CxDbCommand CreateCommand(string sql, params CxDbParameter[] parameters)
    {
      IDbCommand command = CreateCommand();
      SetCommandProps(command, CommandType.Text);
      return new CxDbCommand(this, command, sql, parameters);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates database command for the given SQL statement.
    /// </summary>
    /// <param name="sql">SQL statement to create command for</param>
    /// <param name="commandType">type of the command</param>
    /// <param name="parameters">statement parameters</param>
    /// <returns>database command for the given SQL statement</returns>
    public CxDbCommand CreateCommand(
      string sql,
      CommandType commandType,
      CxDbParameter[] parameters)
    {
      IDbCommand command = CreateCommand();
      SetCommandProps(command, commandType);
      return new CxDbCommand(this, command, sql, parameters);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates database command for the given SQL statement.
    /// </summary>
    /// <param name="sql">SQL statement to create command for</param>
    /// <param name="provider">value provider to get parameter values</param>
    /// <returns>database command for the given SQL statement</returns>
    public CxDbCommand CreateCommand(string sql, IxValueProvider provider)
    {
      IDbCommand command = CreateCommand();
      SetCommandProps(command, CommandType.Text);
      return new CxDbCommand(this, command, sql, provider);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates database command for the given stored procedure.
    /// </summary>
    /// <param name="procedureName">name of the stored procedure to create command for</param>
    /// <returns>database command for the given stored procedure</returns>
    public CxDbCommand CreateCommandSP(string procedureName)
    {
      return CreateCommandSP(procedureName, new CxDbParameter[0]);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates database command for the given stored procedure.
    /// </summary>
    /// <param name="procedureName">name of the stored procedure to create command for</param>
    /// <param name="paramValues">values of statement parameters</param>
    /// <returns>database command for the given stored procedure</returns>
    public CxDbCommand CreateCommandSP(string procedureName, params object[] paramValues)
    {
      IDbCommand command = CreateCommand();
      SetCommandProps(command, CommandType.StoredProcedure);
      return new CxDbCommand(this, command, procedureName, paramValues);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates database command for the given stored procedure.
    /// </summary>
    /// <param name="procedureName">name of the stored procedure to create command for</param>
    /// <param name="parameters">statement parameters</param>
    /// <returns>database command for the given stored procedure</returns>
    public CxDbCommand CreateCommandSP(string procedureName, params CxDbParameter[] parameters)
    {
      IDbCommand command = CreateCommand();
      SetCommandProps(command, CommandType.StoredProcedure);
      return new CxDbCommand(this, command, procedureName, parameters);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Sets command properties.
    /// </summary>
    /// <param name="command">ADO.NET command to set properties of</param>
    /// <param name="commandType"></param>
    protected void SetCommandProps(IDbCommand command, CommandType commandType)
    {
      command.CommandType = commandType;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates database parameter.
    /// </summary>
    /// <param name="name">parameter name</param>
    public CxDbParameter CreateParameter(string name)
    {
      return CreateParameter(name, null, null);
    }

        //----------------------------------------------------------------------------
        /// <summary>
        /// Creates database parameter.
        /// </summary>
        /// <param name="name">parameter name</param>
        /// <param name="value">parameter value</param>
        public CxDbParameter CreateParameter(string name, object value)
        {
            return CreateParameter(name, value, ParameterDirection.Input,  null);
        }

        //----------------------------------------------------------------------------
        /// <summary>
        /// Creates database parameter.
        /// </summary>
        /// <param name="name">parameter name</param>
        /// <param name="value">parameter value</param>
        public CxDbParameter CreateParameter(string name, object value, string objType)
    {
      return CreateParameter(name, value, ParameterDirection.Input, objType);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates database parameter.
    /// </summary>
    /// <param name="name">parameter name</param>
    /// <param name="value">parameter value</param>
    /// <param name="direction">direction of the parameter</param>
    public CxDbParameter CreateParameter(string name, object value, ParameterDirection direction, string objType)
    {
      return CreateParameter(name, value, direction, DbType.Object, objType);
    }

        //----------------------------------------------------------------------------
        /// <summary>
        /// Creates database parameter.
        /// </summary>
        /// <param name="name">parameter name</param>
        /// <param name="value">parameter value</param>
        /// <param name="direction">direction of the parameter</param>
        /// <param name="type">database type of the parameter</param>
        public CxDbParameter CreateParameter(
          string name, object value, ParameterDirection direction, DbType type)
        {
            return CreateParameter(name, value, direction, DbType.Object, null);
        }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates database parameter.
    /// </summary>
    /// <param name="name">parameter name</param>
    /// <param name="value">parameter value</param>
    /// <param name="direction">direction of the parameter</param>
    /// <param name="type">database type of the parameter</param>
        public CxDbParameter CreateParameter(
      string name, object value, ParameterDirection direction, DbType type, string objType)
    {

            IDataParameter parameter;
            if (!string.IsNullOrEmpty(objType) && objType == "file" || objType == "photo")
            {
                
                if (value is byte[])
                {
                    parameter = CreateParameterLob(NxLobType.NewBlob, ((byte[])value).Length);
                }
                else if (value == null )
                {
                    parameter = CreateParameterLob(NxLobType.NewBlob, 0);
                }
                else
                {
                    parameter = CreateParameter();
                }
                return CreateParameter(parameter, name, value, direction, type);
            }



      
      if (value is byte[])  
      {
        parameter = CreateParameterLob(NxLobType.NewBlob, ((byte[]) value).Length);
      }
      else if (value == null && type == DbType.Binary)
      {
        parameter = CreateParameterLob(NxLobType.NewBlob, 0);   
      }
      else
      {
        parameter = CreateParameter();
      }
      return CreateParameter(parameter, name, value, direction, type);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates database parameter by IDataParameter.
    /// </summary>
    /// <param name="iDataParameter">IDataParameter to create parameter by</param>
    /// <param name="name">parameter name</param>
    /// <param name="value">parameter value</param>
    /// <param name="direction">direction of the parameter</param>
    /// <param name="type">database type of the parameter</param>
    protected CxDbParameter CreateParameter(IDataParameter iDataParameter, string name, object value, ParameterDirection direction, DbType type)
    {
      CxDbParameter parameter = new CxDbParameter(iDataParameter);
      parameter.Name = ScriptGenerator.PrepareParameterName(name, -1);

      parameter.Direction = direction;
      if (type != DbType.Object) parameter.DataType = type;

      // The value setting should go after all the other properties are set.
      parameter.Value = value;
      return parameter;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates database parameter by the given parameter description.
    /// </summary>
    /// <param name="parameterDescription">a parameter description to create a parameter by</param>
    /// <returns>created parameter</returns>
    public CxDbParameter CreateParameter(CxDbParameterDescription parameterDescription)
    {
      return CreateParameter(
        parameterDescription.Name,
        parameterDescription.Value,
        parameterDescription.Direction,
        parameterDescription.DataType);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates database parameter set by the given parameter descriptions.
    /// </summary>
    /// <param name="parameterDescriptions">an array of parameter descriptions</param>
    /// <returns>an array of parameters</returns>
    public CxDbParameter[] CreateParameters(CxDbParameterDescription[] parameterDescriptions)
    {
      CxDbParameter[] parameters = new CxDbParameter[parameterDescriptions.Length];
      for (int i = 0; i < parameterDescriptions.Length; i++)
      {
        parameters[i] = CreateParameter(parameterDescriptions[i]);
      }
      return parameters;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates database LOB parameter.
    /// </summary>
    /// <param name="name">parameter name</param>
    /// <param name="lobType">type of the LOB this parameter represents</param>
    /// <param name="size">paremeter size</param>
    public CxDbParameter CreateParameterLob(string name, NxLobType lobType, int size)
    {
      return CreateParameterLob(name, lobType, size, null);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates database LOB parameter.
    /// </summary>
    /// <param name="name">parameter name</param>
    /// <param name="lobType">type of the LOB this parameter represents</param>
    /// <param name="size">paremeter size</param>
    /// <param name="value">parameter value</param>
    public CxDbParameter CreateParameterLob(string name, NxLobType lobType, int size, object value)
    {
      return CreateParameterLob(name, lobType, size, value, ParameterDirection.Input);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates database LOB parameter.
    /// </summary>
    /// <param name="name">parameter name</param>
    /// <param name="lobType">type of the LOB this parameter represents</param>
    /// <param name="size">paremeter size</param>
    /// <param name="value">parameter value</param>
    /// <param name="direction">direction of the parameter</param>
    public CxDbParameter CreateParameterLob(string name, NxLobType lobType, int size, object value, ParameterDirection direction)
    {
      return CreateParameter(CreateParameterLob(lobType, size), name, value, direction, DbType.Object);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Registers logger.
    /// </summary>
    /// <param name="logger">logger to register</param>
    public void RegisterLogger(IxLogger logger)
    {
      if (!m_Loggers.Contains(logger))
      {
        m_Loggers.Add(logger);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Unregisters logger.
    /// </summary>
    /// <param name="logger">logger to unregister</param>
    public void UnregisterLogger(IxLogger logger)
    {
      m_Loggers.Remove(logger);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Logs SQL statement and parameters.
    /// </summary>
    /// <param name="command">command to log</param>
    /// <returns>ID of SQL call</returns>
    protected int Log(CxDbCommand command)
    {
      lock (m_LogLock)
      {
        if (Logging)
        {
          m_SqlCallId++;
          string s = CxUtils.ComposeSqlDescription(command.CommandText, command.GetParamValues());
          WriteToLog("Call <" + m_SqlCallId + ">: " + s);
        }
        return m_SqlCallId;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Logs SQL statement and parameters.
    /// </summary>
    /// <param name="sql">SQL statement</param>
    /// <param name="parameters">statement parameters</param>
    /// <returns>ID of SQL call</returns>
    protected int Log(string sql, CxDbParameter[] parameters)
    {
      lock (m_LogLock)
      {
        if (Logging)
        {
          m_SqlCallId++;
          string s = CxUtils.ComposeSqlDescription(sql, parameters);
          WriteToLog("Call <" + m_SqlCallId + ">: " + s);
        }
        return m_SqlCallId;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Logs completion of the SQL execution.
    /// </summary>
    /// <param name="sqlCallId">id of teh SQL call</param>
    protected void Log(int sqlCallId)
    {
      if (Logging)
      {
        WriteToLog("Call <" + m_SqlCallId + "> completed");
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Logs completion of the SQL execution.
    /// </summary>
    /// <param name="sqlCallId">id of teh SQL call</param>
    /// <param name="command">command to log</param>
    /// <param name="e">exception to log</param>
    protected void Log(int sqlCallId, CxDbCommand command, Exception e)
    {
      if (ErrorLogging)
      {
        string sqlDesc;
        if (Logging)
        {
          sqlDesc = "<" + m_SqlCallId + "> ";
        }
        else
        {
          sqlDesc = CxUtils.ComposeSqlDescription(command.CommandText, command.GetParamValues()) + "\r\n";
        }

        Exception actualException = CxUtils.GetOriginalException(e);
        string errorMessage = CxCommon.GetExceptionFullStackTrace(actualException);
        StackTrace stackTrace = new StackTrace(true);
        WriteToLog("SQL ERROR\r\n" +
          "Time: " + DateTime.Now + "\r\n" +
          errorMessage +
          "Stack: " + stackTrace + "\r\n\r\n" +
          sqlDesc + "\r\n");
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Writes message to log(s).
    /// </summary>
    /// <param name="message">message to write</param>
    public void WriteToLog(string message)
    {
      foreach (IxLogger logger in m_Loggers)
      {
        logger.Write(message);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates a clone of the given parameter in the context 
    /// of the current connection.
    /// </summary>
    /// <param name="parameter">a parameter to be cloned</param>
    /// <returns>a parameter-clone</returns>
    public CxDbParameter CloneParameter(CxDbParameter parameter)
    {
      return CreateParameter(parameter.Name, parameter.Value, parameter.Direction, parameter.DataType);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Clones a set of parameters in the context of the current connection.
    /// </summary>
    /// <param name="parameters">an array of parameters to be cloned</param>
    /// <returns>an array of cloned parameters</returns>
    public CxDbParameter[] CloneParameters(CxDbParameter[] parameters)
    {
      CxDbParameter[] result = new CxDbParameter[parameters.Length];
      for (int i = 0; i < parameters.Length; i++)
      {
        result[i] = CloneParameter(parameters[i]);
      }
      return result;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates data adapter for the given command.
    /// </summary>
    /// <param name="command">command to create dataadapter for</param>
    /// <returns>created data adapter</returns>
    protected DbDataAdapter CreateDataAdapter(CxDbCommand command)
    {
      return CreateDataAdapter(command.Command);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates database command.
    /// </summary>
    /// <returns>created command</returns>
    abstract protected IDbCommand CreateCommand();
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates data adapter.
    /// </summary>
    /// <param name="command">database command to create adapter for</param>
    /// <returns>created data adapter</returns>
    abstract protected DbDataAdapter CreateDataAdapter(IDbCommand command);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates database parameter.
    /// </summary>
    /// <returns>created parameter</returns>
    abstract protected IDataParameter CreateParameter();
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates database LOB parameter.
    /// </summary>
    /// <param name="lobType">type of the LOB this parameter represents</param>
    /// <param name="size">paremeter size</param>
    /// <returns>created parameter</returns>
    abstract protected IDataParameter CreateParameterLob(NxLobType lobType, int size);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns pure parameter name (without prefixes).
    /// </summary>
    /// <param name="name">provider-dependent parameter name</param>
    /// <returns>pure parameter name (without prefixes)</returns>
    virtual public string GetPureParamName(string name)
    {
      return name.ToUpper();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Prepares some kinds of parameters depending on database type.
    /// </summary>
    /// <param name="command">command to prepare parameters for</param>
    /// <param name="mayReturnResultSet">true if command may return result set</param>
    virtual protected void PrepareParameters(CxDbCommand command, bool mayReturnResultSet)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns next unique ID value.
    /// </summary>
    virtual public int GetNextId()
    {
      CxDbParameter p = CreateParameter("NextValue", DBNull.Value, ParameterDirection.Output, DbType.Int32);
      ExecuteCommandSP("p_GetFrameworkSequenceNextValue", p);
      return Convert.ToInt32(p.Value);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns next range of unique ID values.
    /// </summary>
    virtual public void GetNextIdRange(int requiredCount, out int minId, out int maxId)
    {
      CxDbParameter p1 = CreateParameter("RequestedCount", DBNull.Value, ParameterDirection.Input, DbType.Int32);
      p1.Value = requiredCount;
      CxDbParameter p2 = CreateParameter("MinNextValue", DBNull.Value, ParameterDirection.Output, DbType.Int32);
      CxDbParameter p3 = CreateParameter("MaxNextValue", DBNull.Value, ParameterDirection.Output, DbType.Int32);
      ExecuteCommandSP("p_GetFrameworkSequenceNextRange", p1, p2, p3);
      minId = Convert.ToInt32(p2.Value);
      maxId = Convert.ToInt32(p3.Value);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns FROM clause for SELECT statement returning an expression
    /// (returns FROM DUAL for Oracle, or empty string for MSSQL)
    /// </summary>
    virtual public string GetOneRowFromClause()
    {
      return "";
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns SQL statement for calculation of the boolean expression.
    /// </summary>
    /// <param name="expression">boolean expression</param>
    /// <returns>SQL statement returning 0 or 1 depending on the boolean expression result</returns>
    virtual public string GetBoolExpressionCalculateSql(string expression)
    {
      return "SELECT COUNT(*) " + GetOneRowFromClause() + " WHERE " + expression;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Calculates boolean expression on the server.
    /// </summary>
    /// <param name="expression">expression to calculate</param>
    /// <param name="valueProvider">value provider to get expression values</param>
    /// <returns>result of the expression</returns>
    public bool CalculateBoolExpression(
      string expression,
      IxValueProvider valueProvider)
    {
      string sql = GetBoolExpressionCalculateSql(expression);
      int result = CxInt.Parse(ExecuteScalar(sql, valueProvider), 0);
      return result > 0;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Fills dataset with query result(s)
    /// </summary>
    /// <param name="ds">dataset to fill</param>
    /// <param name="command">command to execute</param>
    /// <param name="paramValues">parameter values</param>
    public void FillDataSet(DataSet ds, CxDbCommand command, params object[] paramValues)
    {
      int logId = 0;
      DoOnSqlBegin();
      try
      {
        if (command.Command.CommandType == CommandType.StoredProcedure)
        {
          PrepareParameters(command, true);
        }
        command.SetParamValues(paramValues);
        logId = Log(command);
        using (DbDataAdapter da = CreateDataAdapter(command))
        {
          da.Fill(ds);
        }
        Log(logId);
      }
      catch (Exception e)
      {
        Log(logId, command, e);
        throw new ExDbException(command.CommandText, paramValues, e);
      }
      finally
      {
        DoOnSqlEnd();
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Fills dataset with query result(s)
    /// </summary>
    /// <param name="ds">dataset to fill</param>
    /// <param name="sql">query to execute</param>
    /// <param name="paramValues">parameter values</param>
    public void FillDataSet(DataSet ds, string sql, params object[] paramValues)
    {
      using (CxDbCommand command = CreateCommand(sql, paramValues))
      {
        FillDataSet(ds, command);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Fills dataset with query result(s)
    /// </summary>
    /// <param name="ds">dataset to fill</param>
    /// <param name="sql">query to execute</param>
    /// <param name="valueProvider">value provider to get parameter values</param>
    public void FillDataSet(DataSet ds, string sql, IxValueProvider valueProvider)
    {
      using (CxDbCommand command = CreateCommand(sql, valueProvider))
      {
        FillDataSet(ds, command);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Does actions before each SQL command begin.
    /// </summary>
    protected void DoOnSqlBegin()
    {
      if (OnSqlBegin != null)
      {
        OnSqlBegin(this, new EventArgs());
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Does actions after each SQL command end.
    /// </summary>
    protected void DoOnSqlEnd()
    {
      if (OnSqlEnd != null)
      {
        OnSqlEnd(this, new EventArgs());
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if given sql result type is result returning type.
    /// </summary>
    static public bool IsResultReturnExpected(NxSqlResult sqlResult)
    {
      return sqlResult == NxSqlResult.RowsAffected ||
             sqlResult == NxSqlResult.Scalar ||
             sqlResult == NxSqlResult.DataSet ||
             sqlResult == NxSqlResult.SchemaTable;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Executes given method in DB transaction.
    /// If connection is not in transaction, begins transaction,
    /// commits on OK and rollbacks on error.
    /// </summary>
    /// <param name="method">method to execute</param>
    /// <param name="parameter">developer defined optional parameter</param>
    public void ExecuteInTransaction(DxDbMethod method, object parameter)
    {
      bool ownsTransaction = !InTransaction;
      if (ownsTransaction)
      {
        BeginTransaction();
      }
      try
      {
        method(this, parameter);
        if (ownsTransaction)
        {
          Commit();
        }
      }
      catch (Exception e)
      {
        if (ownsTransaction)
        {
          Rollback();
        }
        throw new ExException(e.Message, e);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Executes given method in DB transaction.
    /// If connection is not in transaction, begins transaction,
    /// commits on OK and rollbacks on error.
    /// </summary>
    /// <param name="method">method to execute</param>
    public void ExecuteInTransaction(DxDbMethod method)
    {
      ExecuteInTransaction(method, null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Event handler on begin transaction
    /// </summary>
    public event DxBeginTransaction OnBeginTransaction;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Event handler called before each SQL command start.
    /// </summary>
    public event EventHandler OnSqlBegin;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Event handler called after each SQL command end.
    /// </summary>
    public event EventHandler OnSqlEnd;
    //-------------------------------------------------------------------------
  }
}