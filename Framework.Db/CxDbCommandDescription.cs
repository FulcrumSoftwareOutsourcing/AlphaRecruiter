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
using System.ComponentModel;
using System.Data;
using System.Xml.Serialization;
using Framework.Utils;

namespace Framework.Db
{
	/// <summary>
	/// Class to cache SQL statement.
	/// </summary>
	[Serializable]
  public class CxDbCommandDescription
  {
    //-------------------------------------------------------------------------
    protected string m_CommandText = null;
    protected CommandType m_CommandType = CommandType.Text;
    protected CommandBehavior m_CommandBehavior = CommandBehavior.Default;
    protected int m_CommandTimeout = 0;
    protected ArrayList m_Parameters = new ArrayList();
    protected NxSqlResult m_SqlResult = NxSqlResult.None;
    protected DateTime m_CreationTime = DateTime.Now;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public CxDbCommandDescription()
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="command">command to copy properties from</param>
    public CxDbCommandDescription(IDbCommand command)
    {
      m_CommandText = command.CommandText;
      m_CommandType = command.CommandType;
      m_CommandTimeout = command.CommandTimeout;
      Parameters = CxDbParameterDescription.GetParameterDescriptions(command);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="commandText">SQL statement text</param>
    /// <param name="parameters">parameter values list</param>
    /// <param name="sqlResult">SQL statement expected result type</param>
    public CxDbCommandDescription(
      string commandText,
      CommandType commandType,
      CxDbParameterDescription[] parameters,
      NxSqlResult sqlResult)
    {
      m_CommandText = commandText;
      m_CommandType = commandType;
      if (parameters != null)
      {
        m_Parameters.AddRange(parameters);
      }
      m_SqlResult = sqlResult;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds parameter value to the parameters list.
    /// </summary>
    public int AddParameter(CxDbParameterDescription parameter)
    {
      lock (this)
      {
        if (parameter != null)
        {
          return m_Parameters.Add(parameter);
        }
        return -1;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Appends binary parameter value.
    /// </summary>
    /// <param name="index">index of parameter to append value to</param>
    /// <param name="valueChunk">byte array to append value from</param>
    public void AppendBinaryParamValue(int index, byte[] valueChunk)
    {
      if (valueChunk != null && valueChunk.Length > 0)
      {
        lock (this)
        {
          if (index < 0 || index >= m_Parameters.Count)
          {
            throw new ExException("SQL parameter index is invalid.");
          }
          CxDbParameterDescription param = (CxDbParameterDescription) m_Parameters[index];
          if (param.Value is byte[])
          {
            byte[] binaryValue = (byte[]) param.Value;
            byte[] newValue = new byte[binaryValue.Length + valueChunk.Length];
            Array.Copy(binaryValue, newValue, binaryValue.Length);
            Array.Copy(valueChunk, 0, newValue, binaryValue.Length, valueChunk.Length);
            param.Value = newValue;
          }
          else
          {
            throw new ExException("SQL parameter type is invalid for binary value append.");
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Appends string parameter value.
    /// </summary>
    /// <param name="index">index of parameter to append value to</param>
    /// <param name="valueChunk">string to append value from</param>
    public void AppendStringParamValue(int index, string valueChunk)
    {
      if (CxUtils.NotEmpty(valueChunk))
      {
        lock (this)
        {
          if (index < 0 || index >= m_Parameters.Count)
          {
            throw new ExException("SQL parameter index is invalid.");
          }
          CxDbParameterDescription param = (CxDbParameterDescription) m_Parameters[index];
          if (param.Value is string)
          {
            param.Value = (string) param.Value + valueChunk;
          }
          else
          {
            throw new ExException("SQL parameter type is invalid for string value append.");
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Executes command using the given database connection.
    /// </summary>
    public CxDbCommandResult ExecuteCommand(CxDbConnection connection)
    {
      CxDbCommandResult result = new CxDbCommandResult();

      CxDbParameterDescription[] descriptions = Parameters;
      CxDbParameter[] parameters = new CxDbParameter[descriptions.Length];
      for (int i = 0; i < descriptions.Length; i++)
      {
        parameters[i] = connection.CreateParameter(
          descriptions[i].Name,
          descriptions[i].Value,
          descriptions[i].Direction,
          descriptions[i].DataType);
      }

      CxDbCommand command = connection.CreateCommand(CommandText, CommandType, parameters);
      try
      {
        switch (SqlResult)
        {
          case NxSqlResult.None :
            connection.ExecuteCommand(command);
            break;
          case NxSqlResult.RowsAffected :
            result.RowsAffected = connection.ExecuteCommandWithResult(command);
            break;
          case NxSqlResult.Scalar :
            result.ScalarValue = connection.ExecuteScalar(command);
            break;
          case NxSqlResult.DataSet :
            DataSet ds = new DataSet();
            connection.FillDataSet(ds, command);
            result.DataSet = ds;
            break;
          case NxSqlResult.SchemaTable :
            using (IDataReader reader = connection.ExecuteReader(command))
            {
              result.DataTable = reader.GetSchemaTable();
            }
            break;
        }
        result.AssignOutputParamsFromCommand(command);
      }
      finally
      {
        command.Dispose();
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets SQL statement text.
    /// </summary>
    public string CommandText
    { get { return m_CommandText; } set { m_CommandText = value; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets command type.
    /// </summary>
    [DefaultValue(CommandType.Text)]
    public CommandType CommandType
    { get { return m_CommandType; } set { m_CommandType = value; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets command behavior for the next command execution.
    /// </summary>
    [DefaultValue(CommandBehavior.Default)]
    public CommandBehavior CommandBehavior
    { get { return m_CommandBehavior; } set { m_CommandBehavior = value; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets command timeout.
    /// </summary>
    [DefaultValue(0)]
    public int CommandTimeout
    { get { return m_CommandTimeout; } set { m_CommandTimeout = value; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// List of SQL parameter values
    /// </summary>
    public CxDbParameterDescription[] Parameters
    { 
      get 
      { 
        CxDbParameterDescription[] array = new CxDbParameterDescription[m_Parameters.Count];
        m_Parameters.CopyTo(array);
        return array; 
      } 
      set
      {
        m_Parameters.Clear();
        if (value != null && value.Length > 0)
        {
          m_Parameters.AddRange(value);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets expected SQL statement execution result type
    /// </summary>
    [DefaultValue(NxSqlResult.None)]
    public NxSqlResult SqlResult
    { get { return m_SqlResult; } set { m_SqlResult = value; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns number of seconds since SQL creation.
    /// </summary>
    [XmlIgnore]
    [SoapIgnore]
    public double SecondsSinceCreation
    { 
      get 
      { 
        return (DateTime.Now - m_CreationTime).TotalSeconds; 
      } 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if return of result is expected for the command.
    /// </summary>
    [XmlIgnore]
    [SoapIgnore]
    public bool IsResultReturnExpected
    {
      get
      {
        bool result = CxDbConnection.IsResultReturnExpected(SqlResult);
        if (!result)
        {
          foreach (CxDbParameterDescription param in m_Parameters)
          {
            if (param.Direction != ParameterDirection.Input)
            {
              result = true;
              break;
            }
          }
        }
        return result;
      }
    }
    //-------------------------------------------------------------------------
  }
}