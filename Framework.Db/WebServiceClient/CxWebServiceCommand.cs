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
	/// Class describing web service client command.
	/// </summary>
	public class CxWebServiceCommand : IDbCommand, IxDbCommandEx
	{
    //-------------------------------------------------------------------------
    protected CxWebServiceConnection m_Connection;
    protected CxWebServiceTransaction m_Transaction;
    protected string m_CommandText;
    protected CommandType m_CommandType = CommandType.Text;
    protected int m_CommandTimeout;
    protected CxWebServiceParameterCollection m_Parameters = new CxWebServiceParameterCollection();
    protected UpdateRowSource m_UpdateRowSource = UpdateRowSource.None;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    public CxWebServiceCommand()
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    public CxWebServiceCommand(string commandText)
    {
      m_CommandText = commandText;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    public CxWebServiceCommand(CxWebServiceConnection connection)
    {
      m_Connection = connection;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    public CxWebServiceCommand(
      string commandText, 
      CxWebServiceConnection connection)
		{
      m_CommandText = commandText;
      m_Connection = connection;
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    public CxWebServiceCommand(
      string commandText, 
      CxWebServiceTransaction transaction)
    {
      m_CommandText = commandText;
      m_Transaction = transaction;
      m_Connection = transaction.Connection;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Prepares command. Does nothing.
    /// </summary>
    public void Prepare()
	  {
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Cancels command. Raises an exception.
    /// </summary>
    public void Cancel()
	  {
	    throw new NotImplementedException();
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates parameter.
    /// </summary>
    public CxWebServiceParameter CreateParameter()
    {
      return new CxWebServiceParameter();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates parameter.
    /// </summary>
    IDbDataParameter IDbCommand.CreateParameter()
	  {
      return new CxWebServiceParameter();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns chunk of parameter data.
    /// </summary>
    protected object GetParamValueChunk(object paramValue, int chunkIndex)
    {
      if (GetIsPartialUploadRequired(paramValue))
      {
        int chunkLength = m_Connection.MaxParameterLength;
        int startIndex = chunkIndex * chunkLength;

        if (paramValue is string)
        {
          if (chunkIndex < 0)
          {
            return "";
          }
          else
          {
            string strValue = (string) paramValue;
            if (chunkLength > strValue.Length - startIndex)
            {
              chunkLength = strValue.Length - startIndex;
            }
            if (chunkLength > 0)
            {
              return strValue.Substring(startIndex, chunkLength);
            }
          }
        }
        else if (paramValue is byte[])
        {
          if (chunkIndex < 0)
          {
            return new byte[0];
          }
          else
          {
            byte[] byteValue = (byte[]) paramValue;
            if (chunkLength > byteValue.Length - startIndex)
            {
              chunkLength = byteValue.Length - startIndex;
            }
            if (chunkLength > 0)
            {
              byte[] chunkValue = new byte[chunkLength];
              Array.Copy(byteValue, startIndex, chunkValue, 0, chunkLength);
              return chunkValue;
            }
          }
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Validates command connection. Raises an exception if connection is invalid.
    /// </summary>
    protected void ValidateConnection()
    {
      if (m_Connection == null)
      {
        throw new ExException("Connection is not specified for the web service command.");
      }
      m_Connection.ValidateOpenState();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Executes command by call to the web service.
    /// </summary>
    protected CxDbCommandResult WebServiceExecuteCommand(NxSqlResult sqlResult)
    {
      ValidateConnection();

      CxDbService service = m_Connection.CreateWebService();
      if (m_CommandTimeout > 0)
      {
        service.Timeout = m_CommandTimeout * 1000; // Convert to milliseconds
      }

      CxDbCommandDescription command = new CxDbCommandDescription(this);
      command.SqlResult = sqlResult;

      CxDbCommandResult commandResult;

      if (!IsPartialUploadRequired)
      {
        // Single request command execution
        commandResult = service.ExecuteSQL(
          m_Connection.RegistrationRecord.ClientID,
          m_Connection.Database,
          m_Connection.UserID,
          m_Connection.EncryptedPassword,
          m_Transaction != null ? m_Transaction.ID : Guid.Empty,
          command);
      }
      else
      {
        // Partial command upload
        // Save original parameter values
        CxDbParameterDescription[] oldParams = command.Parameters;

        // Truncate partial parameter values for the command upload
        CxDbParameterDescription[] newParams = new CxDbParameterDescription[oldParams.Length];
        for (int i = 0; i < oldParams.Length; i++)
        {
          newParams[i] = new CxDbParameterDescription(oldParams[i]);
          if (GetIsPartialUploadRequired(newParams[i].Value))
          {
            newParams[i].Value = GetParamValueChunk(newParams[i].Value, -1);
          }
        }
        command.Parameters = newParams;

        // Upload command with truncated partial parameter values
        Guid commandId = Guid.NewGuid();
        service.UploadSQL(
          m_Connection.RegistrationRecord.ClientID,
          m_Connection.Database,
          m_Connection.UserID,
          m_Connection.EncryptedPassword,
          commandId,
          command);

        // Append partial parameter values one by one
        for (int i = 0; i < oldParams.Length; i++)
        {
          if (GetIsPartialUploadRequired(oldParams[i].Value))
          {
            int chunkIndex = 0;
            object chunkValue;
            while ((chunkValue = GetParamValueChunk(oldParams[i].Value, chunkIndex)) != null)
            {
              if (chunkValue is string)
              {
                service.AppendUploadedSQLParamStringValue(
                  m_Connection.RegistrationRecord.ClientID,
                  m_Connection.Database,
                  m_Connection.UserID,
                  m_Connection.EncryptedPassword,
                  commandId,
                  i,
                  (string)chunkValue);
              }
              else if (chunkValue is byte[])
              {
                service.AppendUploadedSQLParamBinaryValue(
                  m_Connection.RegistrationRecord.ClientID,
                  m_Connection.Database,
                  m_Connection.UserID,
                  m_Connection.EncryptedPassword,
                  commandId,
                  i,
                  (byte[])chunkValue);
              }
              chunkIndex++;
            }
          }
        }

        // Execute uploaded command
        commandResult = service.ExecuteLoadedSQL(
          m_Connection.RegistrationRecord.ClientID,
          m_Connection.Database,
          m_Connection.UserID,
          m_Connection.EncryptedPassword,
          m_Transaction != null ? m_Transaction.ID : Guid.Empty,
          commandId);
      }

      if (commandResult != null)
      {
        commandResult.CopyOutputParamValuesToCommand(this);
      }
      return commandResult;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Executes command by call to the web service.
    /// </summary>
    protected object WebServiceExecuteNonQuery()
    {
      return WebServiceExecuteCommand(NxSqlResult.None);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Executes command by call to the web service.
    /// </summary>
    protected object WebServiceExecuteNonQueryWithResult()
    {
      return WebServiceExecuteCommand(NxSqlResult.RowsAffected);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Executes command by call to the web service.
    /// </summary>
    protected object WebServiceExecuteScalar()
    {
      return WebServiceExecuteCommand(NxSqlResult.Scalar);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Executes reader by call to the web service.
    /// </summary>
    protected object WebServiceExecuteReader()
    {
      return WebServiceExecuteCommand(NxSqlResult.DataSet);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Executes non-query command.
    /// </summary>
    /// <returns></returns>
    public int ExecuteNonQuery()
	  {
      m_Connection.CallWebServiceMethod(WebServiceExecuteNonQuery);
      return 0;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Executes non-query command.
    /// </summary>
    /// <returns></returns>
    public int ExecuteNonQueryWithResult()
    {
      CxDbCommandResult result = (CxDbCommandResult)
        m_Connection.CallWebServiceMethod(WebServiceExecuteNonQueryWithResult);
      return result != null ? result.RowsAffected : 0;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Executes command and returns single result.
    /// </summary>
    public object ExecuteScalar()
    {
      CxDbCommandResult result = (CxDbCommandResult)
        m_Connection.CallWebServiceMethod(WebServiceExecuteScalar);
      return result != null ? result.ScalarValue : null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Executes command and returns reader.
    /// </summary>
    public IDataReader ExecuteReader(CommandBehavior behavior)
	  {
      CxDbCommandResult result = (CxDbCommandResult)
        m_Connection.CallWebServiceMethod(WebServiceExecuteReader);
      return result != null ? new CxWebServiceDataReader(result.DataSet, this) : null;
      /* Standard DataTableReader can be used instead, like this:
      DataTable[] tables = new DataTable[result.DataSet.Tables.Count];
      for (int i = 0; i < result.DataSet.Tables.Count; i++)
      {
        tables[i] = result.DataSet.Tables[i];
      }
      return result != null ? new DataTableReader(tables) : null;
       */ 
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Executes command and returns reader.
    /// </summary>
    public IDataReader ExecuteReader()
    {
      return ExecuteReader(CommandBehavior.Default);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets connection
    /// </summary>
    public CxWebServiceConnection Connection
    {
      get 
      { 
        return m_Connection; 
      }
      set 
      { 
        if (m_Connection != value)
        {
          m_Transaction = null;
        }
        m_Connection = value; 
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets connection
    /// </summary>
    IDbConnection IDbCommand.Connection
	  {
      get 
      { 
        return m_Connection; 
      }
      set 
      { 
        if (m_Connection != value)
        {
          m_Transaction = null;
        }
        m_Connection = (CxWebServiceConnection) value; 
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets transaction.
    /// </summary>
    public CxWebServiceTransaction Transaction
    {
      get 
      { 
        return m_Transaction; 
      }
      set 
      { 
        if (m_Transaction != value && m_Transaction != null)
        {
          m_Connection = m_Transaction.Connection;
        }
        m_Transaction = value; 
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets transaction.
    /// </summary>
    IDbTransaction IDbCommand.Transaction
	  {
      get 
      { 
        return m_Transaction; 
      }
      set 
      { 
        if (m_Transaction != value && m_Transaction != null)
        {
          m_Connection = m_Transaction.Connection;
        }
        m_Transaction = (CxWebServiceTransaction) value; 
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets command text.
    /// </summary>
    public string CommandText
	  {
	    get { return m_CommandText; }
	    set { m_CommandText = value; }
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets command timeout (in seconds).
    /// </summary>
    public int CommandTimeout
	  {
	    get { return m_CommandTimeout; }
	    set { m_CommandTimeout = value; }
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets command type.
    /// </summary>
    public CommandType CommandType
	  {
	    get { return m_CommandType; }
	    set { m_CommandType = value; }
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of command parameters.
    /// </summary>
    public CxWebServiceParameterCollection Parameters
    {
      get { return m_Parameters; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of command parameters.
    /// </summary>
    IDataParameterCollection IDbCommand.Parameters
	  {
      get { return m_Parameters; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets update row source value.
    /// </summary>
    public UpdateRowSource UpdatedRowSource
	  {
	    get { return m_UpdateRowSource; }
	    set { m_UpdateRowSource = value; }
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Disposes command.
    /// </summary>
    public void Dispose()
	  {
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns array of parameters descriptions.
    /// </summary>
    public CxDbParameterDescription[] ParameterDescriptions
    {
      get
      {
        return CxDbParameterDescription.GetParameterDescriptions(this);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if partial upload is required for the given parameter value.
    /// </summary>
    protected bool GetIsPartialUploadRequired(object parameterValue)
    {
      if (m_Connection != null && m_Connection.MaxParameterLength > 0)
      {
        int currentLength = 0;
        if (parameterValue is string)
        {
          currentLength += ((string)(parameterValue)).Length;
        }
        else if (parameterValue is byte[])
        {
          currentLength += ((byte[])(parameterValue)).Length;
        }
        return currentLength > m_Connection.MaxParameterLength;
      }
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if command data should be uploaded to the web service partially.
    /// </summary>
    public bool IsPartialUploadRequired
    {
      get
      {
        if (m_Connection != null && m_Connection.MaxParameterLength > 0)
        {
          foreach (CxWebServiceParameter parameter in Parameters)
          {
            if (GetIsPartialUploadRequired(parameter.Value))
            {
              return true;
            }
          }
        }
        return false;
      }
    }
    //-------------------------------------------------------------------------
  }
}