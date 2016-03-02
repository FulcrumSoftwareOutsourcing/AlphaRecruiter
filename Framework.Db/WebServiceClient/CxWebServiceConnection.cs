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

using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.Services.Protocols;

using Framework.Utils;

namespace Framework.Db.WebServiceClient
{
	/// <summary>
	/// DB connection class for the WebService .NET Data Provider.
	/// The connection string should be in the following format:
	/// Server={0};Database={1};DBType={2};User ID={3};Password={4};Timeout={5};MaxParamLen={6}
	///  - Server      : URL to the web service (http://host/folder/service.asmx).
	///  - Database    : The name of database connection configured at the web service 
	///                  (in the Web.config file of the web service).
	///  - DBType      : Type of the target database: SqlServer or Oracle.
	///  - User ID     : User name to login to the web service. 
	///                  Depends on the web service authentication mode configured.
	///  - Password    : User password to login to the web service.
	///  - Timeout     : Web service response wait timeout (in millisecinds).
	///  - MaxParamLen : Maximum parameter value length can be passed with a
	///                  single web service request. If parameter value length
	///                  is greater than MaxParamLen, parameter value is passed
	///                  in several web service calls. MaxParamLen is in bytes.
	/// </summary>
	public class CxWebServiceConnection : IDbConnection
	{
    //-------------------------------------------------------------------------
    public const int DEFAULT_MAX_PARAMETER_LENGTH = 1048576; // 1 MB (in bytes)
    //-------------------------------------------------------------------------
    protected string m_Server;
    protected string m_Database;
    protected NxDatabaseType m_DatabaseType = NxDatabaseType.SqlServer;
    protected string m_UserID;
    protected string m_Password;
    protected ConnectionState m_ConnectionState = ConnectionState.Closed;
    protected string m_ConnectionString;
    protected int m_ConnectionTimeout;
    protected int m_MaxParameterLength = DEFAULT_MAX_PARAMETER_LENGTH;
    static protected Hashtable m_RegistrationRecordsTable = new Hashtable();
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
		public CxWebServiceConnection()
		{
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Begins transaction
    /// </summary>
    public CxWebServiceTransaction BeginTransaction()
    {
      return BeginTransaction(IsolationLevel.ReadCommitted);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Begins transaction
    /// </summary>
    public CxWebServiceTransaction BeginTransaction(IsolationLevel il)
    {
      return new CxWebServiceTransaction(this, il);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Begins transaction
    /// </summary>
    IDbTransaction IDbConnection.BeginTransaction()
	  {
      return BeginTransaction(IsolationLevel.ReadCommitted);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Begins transaction
    /// </summary>
    IDbTransaction IDbConnection.BeginTransaction(IsolationLevel il)
	  {
      return new CxWebServiceTransaction(this, il);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Opens connection
    /// </summary>
    public void Open()
    {
      m_ConnectionState = ConnectionState.Open;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Closes connection
    /// </summary>
    public void Close()
	  {
	    m_ConnectionState = ConnectionState.Closed;
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Changes connection database.
    /// </summary>
	  public void ChangeDatabase(string databaseName)
	  {
      m_Database = databaseName;
      m_ConnectionString = ComposeConnectionString();
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates command.
    /// </summary>
    public CxWebServiceCommand CreateCommand()
    {
      return new CxWebServiceCommand(this);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates command.
    /// </summary>
    IDbCommand IDbConnection.CreateCommand()
	  {
      return new CxWebServiceCommand(this);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets connection string.
    /// </summary>
	  public string ConnectionString
	  {
	    get 
      { 
        return m_ConnectionString; 
      }
	    set 
      { 
        m_ConnectionString = value;
        DecomposeConnectionString(m_ConnectionString);
      }
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets connection timeout (in milliseconds).
    /// </summary>
	  public int ConnectionTimeout
	  {
	    get 
      { 
        return m_ConnectionTimeout; 
      }
      set
      {
        m_ConnectionTimeout = value;
        m_ConnectionString = ComposeConnectionString();
      }
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns connection database.
    /// </summary>
	  public string Database
	  {
	    get 
      { 
        return m_Database; 
      }
      set
      {
        m_Database = value;
        m_ConnectionString = ComposeConnectionString();
      }
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns connection state.
    /// </summary>
	  public ConnectionState State
	  {
	    get { return m_ConnectionState; }
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Disposes connection.
    /// </summary>
	  public void Dispose()
	  {
      Close();
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Decomposes connection string to the list of parameters.
    /// </summary>
    protected void DecomposeConnectionString(string connectionString)
    {
      m_Server = null;
      m_Database = null;
      m_DatabaseType = NxDatabaseType.SqlServer;
      m_UserID = null;
      m_Password = null;
      IList<string> list = CxText.DecomposeWithSeparator(connectionString, ";");
      foreach (string s in list)
      {
        int i = s.IndexOf("=");
        if (i > 0 && i < s.Length - 1)
        {
          string name = s.Substring(0, i);
          string value = s.Substring(i + 1);
          if (name.ToUpper() == "SERVER")
          {
            m_Server = value;
          }
          else if (name.ToUpper() == "DATABASE")
          {
            m_Database = value;
          }
          else if (name.ToUpper() == "DBTYPE")
          {
            m_DatabaseType = CxEnum.Parse(name, NxDatabaseType.SqlServer);
          }
          else if (name.ToUpper() == "USER ID")
          {
            m_UserID = value;
          }
          else if (name.ToUpper() == "PASSWORD")
          {
            m_Password = value;
          }
          else if (name.ToUpper() == "TIMEOUT")
          {
            m_ConnectionTimeout = CxInt.Parse(value, 0);
          }
          else if (name.ToUpper() == "MAXPARAMLEN")
          {
            m_MaxParameterLength = CxInt.Parse(value, DEFAULT_MAX_PARAMETER_LENGTH);
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Composes connection string from the current parameters.
    /// </summary>
    protected string ComposeConnectionString()
    {
      return CxText.Format(
        "Server={0};Database={1};DBType={2};User ID={3};Password={4};Timeout={5};MaxParamLen={6}",
        m_Server, m_Database, m_DatabaseType, m_UserID, m_Password, m_ConnectionTimeout, m_MaxParameterLength);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Raises an exception if connection state is not 'Open'
    /// </summary>
    public void ValidateOpenState()
    {
      if (m_ConnectionState != ConnectionState.Open)
      {
        throw new ExException("Web service connection state should be 'Open'.");
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets web service URL to connect
    /// </summary>
    public string Server
    { 
      get
      {
        return m_Server;
      }
      set
      {
        m_Server = value;
        m_ConnectionString = ComposeConnectionString();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets target database type.
    /// </summary>
    public NxDatabaseType DatabaseType
    { 
      get
      {
        return m_DatabaseType;
      }
      set
      {
        m_DatabaseType = value;
        m_ConnectionString = ComposeConnectionString();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets user ID
    /// </summary>
    public string UserID
    { 
      get
      {
        return m_UserID;
      }
      set
      {
        m_UserID = value;
        m_ConnectionString = ComposeConnectionString();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets user password
    /// </summary>
    public string Password
    { 
      get
      {
        return m_Password;
      }
      set
      {
        m_Password = value;
        m_ConnectionString = ComposeConnectionString();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates web service proxy and returns it.
    /// </summary>
    public CxDbService CreateWebService()
    {
      ValidateOpenState();
      CxDbService service = new CxDbService();
      service.EnableDecompression = true;
      service.Url = Server;

      if (ConnectionTimeout > 0)
      {
        service.Timeout = ConnectionTimeout;
      }
      return service;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Calls web service method with three connection attempts.
    /// Client registration record may expire, that's why connection attempts are used.
    /// </summary>
    /// <param name="method">method to call</param>
    /// <param name="repeatCalls">if true, method call is repeated 3 times if not successful</param>
    /// <returns>result returned from method call</returns>
    public object CallWebServiceMethod(DxWebServiceCallMethod method, bool repeatCalls)
    {
      object result = null;
      int tryCount = 0;
      bool success = false;
      do
      {
        try
        {
          result = method();
          success = true;
        }
        catch (SoapException e)
        {
          ExWebServiceException error = CxWebService.CreateClientException(e);
          if (e.Message.Contains("INCOMPATIBLE_CLIENT_AND_DATABASE_VERSIONS"))
            throw error;

          tryCount++;
          if (repeatCalls && 
              tryCount < 3 && 
              error.WebServiceException is ExWebServiceConnectException)
          {
            ClearRegistraionRecord();
          }
          else
          {
            throw error;
          }
        }
      } 
      while (!success);
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Calls web service method with three connection attempts.
    /// Client registration record may expire, that's why connection attempts are used.
    /// </summary>
    /// <param name="method">method to call</param>
    /// <returns>result returned from method call</returns>
    public object CallWebServiceMethod(DxWebServiceCallMethod method)
    {
      return CallWebServiceMethod(method, true);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Calls web service registration client method.
    /// </summary>
    /// <returns>client registration record or null</returns>
    protected object WebServiceRegisterClient()
    {
      return CreateWebService().RegisterClient();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Clears client registration record.
    /// </summary>
    public void ClearRegistraionRecord()
    {
      if (CxUtils.NotEmpty(Server))
      {
        m_RegistrationRecordsTable[CxText.ToUpper(Server)] = null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns web service client registration record.
    /// </summary>
    public CxWebServiceClientRegistrationRecord RegistrationRecord
    {
      get
      {
        if (CxUtils.IsEmpty(Server))
        {
          throw new ExException(
            "Could not get web service client registration record. Connection server name is not specified.");
        }
        CxWebServiceClientRegistrationRecord record =
          (CxWebServiceClientRegistrationRecord) m_RegistrationRecordsTable[CxText.ToUpper(Server)];
        if (record == null)
        {
          record = (CxWebServiceClientRegistrationRecord) 
            CallWebServiceMethod(WebServiceRegisterClient, false);
          if (record != null)
          {
            m_RegistrationRecordsTable[CxText.ToUpper(Server)] = record;
          }
        }
        return record;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns encoded password to pass to the web service.
    /// </summary>
    public string EncryptedPassword
    {
      get
      {
        return RegistrationRecord.EncryptPassword(Password);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets maximum parameter length allowed to pass to the 
    /// web service with a single request. (length in bytes)
    /// </summary>
    public int MaxParameterLength
    { 
      get 
      { 
        return m_MaxParameterLength; 
      } 
      set 
      { 
        m_MaxParameterLength = value; 
        m_ConnectionString = ComposeConnectionString();
      } 
    }
    //-------------------------------------------------------------------------
  }
  //---------------------------------------------------------------------------

  //---------------------------------------------------------------------------
  /// <summary>
  /// Delegate to call web service method.
  /// </summary>
  public delegate object DxWebServiceCallMethod();
  //---------------------------------------------------------------------------
}