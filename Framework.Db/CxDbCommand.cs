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
using System.Collections.Specialized;

using Framework.Utils;
using System.Collections.Generic;

namespace Framework.Db
{
  /// <summary>
  /// Class that encapsulates database command. 
  /// Implements IDisposable to use in using() construct.
  /// 
  /// Do not use this class for direct execution - use CxDbConnection methods instead.
  /// </summary>
  public class CxDbCommand : IDisposable
  {
    //----------------------------------------------------------------------------
    protected IDbCommand m_Command = null; // ADO.NET database command
    protected CxDbConnection m_Connection = null; // Connection this command belongs to
    protected string[] m_ParameterNames = new string[0]; // List of parameter names
    protected CxDbParameter[] m_Parameters = new CxDbParameter[0]; // List of parameters
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="connection">conenction this command belongs to</param>
    /// <param name="command">ADO.NET database command</param>
    /// <param name="sql">SQL statement to create command for</param>
    protected CxDbCommand(CxDbConnection connection, IDbCommand command, string sql)
    {
      m_Connection = connection;
      m_Command = command;
      CommandText = sql;
      if (connection.DefaultCommandTimeout > 0)
      {
        command.CommandTimeout = connection.DefaultCommandTimeout;
      }
      command.Connection = connection.Connection;
      command.Transaction = connection.Transaction;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="connection">conenction this command belongs to</param>
    /// <param name="command">ADO.NET database command</param>
    /// <param name="sql">SQL statement to create command for</param>
    /// <param name="paramValues">values of statement parameters</param>
    protected internal CxDbCommand(CxDbConnection connection, IDbCommand command, string sql, params object[] paramValues)
      : this(connection, command, sql)
    {
      AssignParameters(paramValues);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="connection">conenction this command belongs to</param>
    /// <param name="command">ADO.NET database command</param>
    /// <param name="sql">SQL statement to create command for</param>
    /// <param name="parameters">statement parameters</param>
    protected internal CxDbCommand(CxDbConnection connection, IDbCommand command, string sql, params CxDbParameter[] parameters)
      : this(connection, command, sql)
    {
      AssignParameters(parameters);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="connection">conenction this command belongs to</param>
    /// <param name="command">ADO.NET database command</param>
    /// <param name="sql">SQL statement to create command for</param>
    /// <param name="provider">value provider to get parameter values</param>
    protected internal CxDbCommand(CxDbConnection connection, IDbCommand command, string sql, IxValueProvider provider)
      : this(connection, command, sql)
    {
      AssignParameters(provider);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Frees allocated resources.
    /// </summary>
    public void Dispose()
    {
      m_Command.Dispose();
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Encapsulated ADO.NET command.
    /// </summary>
    public IDbCommand Command
    {
      get { return m_Command; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// SQL statement or stored procedure name.
    /// </summary>
    public string CommandText
    {
      get { return m_Command.CommandText; }
      set { m_Command.CommandText = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Connection this command belongs to.
    /// </summary>
    public CxDbConnection Connection 
    {
      get { return m_Connection; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// List of command parameter names.
    /// </summary>
    public string[] ParameterNames
    {
      get { return m_ParameterNames; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// List of command parameters.
    /// </summary>
    public CxDbParameter[] Parameters
    {
      get { return m_Parameters; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Property to get or set parameter values by name.
    /// If parameter with such name does not exists throws exception.
    /// </summary>
    public CxDbParameter this[string paramName]
    {
      get 
      { 
        CxDbParameter parameter = FindParameter(paramName);
        if (parameter != null) 
          return parameter;
        else
          throw new ExDbException(string.Format("Parameter <{0}> does not exists in <{1}>", paramName, CommandText), 
                                  CommandText); 
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Finds parameter by name.
    /// </summary>
    /// <param name="paramName">name of the parameter to find</param>
    /// <returns>parameter with the given name or null if not found</returns>
    public CxDbParameter FindParameter(string paramName)
    {
      string paramNameUp = Connection.GetPureParamName(paramName);
      foreach (CxDbParameter parameter in m_Parameters)
      {
        if (Connection.GetPureParamName(parameter.Name) == paramNameUp) 
        {
          return parameter;
        }
      }
      return null;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Property to get or set parameter values by index.
    /// If parameter with such numver does not exists throws exception.
    /// </summary>
    public CxDbParameter this[int paramNo]
    {
      get 
      { 
        if (paramNo >= 0 && paramNo < m_Parameters.Length)
          return m_Parameters[paramNo]; 
        else
          throw new ExDbException(string.Format("Parameter with index <{0}> does not exists in <{1}>", paramNo, CommandText), 
                                  CommandText); 
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Adds new parameter to the parameter list.
    /// </summary>
    /// <param name="parameter">parameetr to add</param>
    public void AddParameter(CxDbParameter parameter)
    {
      int newLength = m_ParameterNames.Length + 1;
      string[] newParameterNames = new string[newLength]; 
      CxDbParameter[] newParameters = new CxDbParameter[newLength]; 
      m_ParameterNames.CopyTo(newParameterNames, 0);
      m_Parameters.CopyTo(newParameters, 0);
      newParameterNames[newLength - 1] = parameter.Name;
      newParameters[newLength - 1] = parameter;
      m_ParameterNames = newParameterNames;
      m_Parameters = newParameters;
      parameter.Command = this;
      Command.Parameters.Add(parameter.Parameter);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Assigns command parameters.
    /// </summary>
    /// <param name="parameters">list of command parameters</param>
    protected void AssignParameters(CxDbParameter[] parameters)
    {
      m_Parameters = (CxDbParameter[]) parameters.Clone();
      m_ParameterNames = new string[m_Parameters.Length];
      int count = 0;
      foreach (CxDbParameter parameter in m_Parameters)
      {
        parameter.Command = this;
        Command.Parameters.Add(parameter.Parameter);
        m_ParameterNames[count] = parameter.Name;
        count++;
      }

      string[] paramNames = CxDbParamParser.GetList(CommandText, true);
      NameValueCollection substitues = new NameValueCollection();
      for (int i = 0; i < paramNames.Length; i++)
      {
        string name = paramNames[i];
        substitues.Add(name, Connection.ScriptGenerator.PrepareParameterSubstitute(name, i));
      }
      CommandText = CxDbParamParser.ReplaceParameters(CommandText, substitues);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Assigns command parameters.
    /// </summary>
    /// <param name="paramValues">list of command parameters</param>
    protected void AssignParameters(object[] paramValues)
    {
      string[] paramNames = CxDbParamParser.GetList(CommandText, true);
      int count = paramValues.Length;
      CxDbParameter[] parameters = new CxDbParameter[count];
      for (int i = 0; i < count; i++)
      {
        string name = paramNames[i];
        string paramName = Connection.ScriptGenerator.PrepareParameterSubstitute(name, i);
        parameters[i] = Connection.CreateParameter(paramName, paramValues[i]);
      }
      AssignParameters(parameters);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Assigns command parameters. 
    /// Parses command text, gets list of the parameters, 
    /// creates parameter objects, sets parameter values from the value provider.
    /// </summary>
    /// <param name="provider">value provider to get parameter values</param>
    protected void AssignParameters(IxValueProvider provider)
    {
      string[] paramNames = CxDbParamParser.GetList(CommandText, true);
      CxDbParameter[] parameters = new CxDbParameter[paramNames.Length];
      for (int i = 0; i < paramNames.Length; i++)
      {
        string name = paramNames[i];
        string paramName = Connection.ScriptGenerator.PrepareParameterSubstitute(name, i);
                if (provider.ValueTypes.Count > 0 && ((Dictionary<string, string>)provider.ValueTypes).ContainsKey(name))
                {
                    parameters[i] = Connection.CreateParameter(paramName, provider[name], provider.ValueTypes[name]);
                }
                else
                {
                    parameters[i] = Connection.CreateParameter(paramName, provider[name], null);
                }
                    
      }
      AssignParameters(parameters);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Sets command parameter values.
    /// </summary>
    /// <param name="paramValues">array with parameter values</param>
    public void SetParamValues(object[] paramValues)
    {
      for (int i = 0; i < paramValues.Length; i++)
      {
        this[i].Value = paramValues[i];
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets command parameter values.
    /// </summary>
    /// <returns>array with parameter values</returns>
    public object[] GetParamValues()
    {
      int count = m_Parameters.Length;
      object[] paramValues = new object[count];
      for (int i = 0; i < count; i++)
      {
        paramValues[i] = m_Parameters[i].Value;
      }
      return paramValues;
    }
    //----------------------------------------------------------------------------
  }
}
