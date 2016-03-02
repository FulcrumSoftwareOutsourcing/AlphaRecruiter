/********************************************************************
 *  FulcrumWeb RAD Framework - Fulcrum of your business             *
 *  Copyright (c) 2002-2009 FulcrumWeb, ALL RIGHTS RESERVED         *
 *                                                                  *
 *  THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      *
 *  FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        *
 *  COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       *
 *  AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  *
 *  AND PERMISSION FROM FULCRUMWEB. CONSULT THE END USER LICENSE    *
 *  AGREEMENT FOR INFORMATION ON ADDITIONAL RESTRICTIONS.           *
 ********************************************************************/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using Framework.Db;
using Framework.Entity;
using Framework.Metadata;
using Framework.Utils;
using Framework.Web.Utils;

namespace Framework.Remote
{
  //---------------------------------------------------------------------------
	/// <summary>
	/// Database connection manager.
	/// </summary>
  public class CxDbConnections
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Database content type.
    /// </summary>
    public enum NxDbContent { Entity, Portal, Report }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    protected const string CACHED_INSTANCE_KEY = "CxDbConnections.Instance";
    protected const string CONFIG_SECTION_NAME = "databaseConnections";
    //-------------------------------------------------------------------------
    protected Dictionary<NxDbContent, CxDbConnectionMetadata>
      m_ContentConnections = new Dictionary<NxDbContent, CxDbConnectionMetadata>();
    protected Dictionary<string, CxDbConnectionMetadata>
      m_AllConnections = new Dictionary<string, CxDbConnectionMetadata>(StringComparer.OrdinalIgnoreCase);
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates DB connections metadata object from the given XML node.
    /// </summary>
    /// <param name="node">XML node to get data from</param>
    public CxDbConnections(XmlNode node)
    {
      
      if (node == null)
        throw new ExNullArgumentException("node");

      XmlNodeList nodeList = node.SelectNodes("connection");
      if (nodeList == null)
        throw new ExNullReferenceException("nodeList");

      foreach (XmlElement element in nodeList)
      {
        CxDbConnectionMetadata connection = new CxDbConnectionMetadata(element);
        m_AllConnections[connection.Id] = connection;
      }

      foreach (NxDbContent content in Enum.GetValues(typeof(NxDbContent)))
      {
        string name = content.ToString().ToLower();
        XmlElement contentElement = (XmlElement) node.SelectSingleNode(name);
        if (contentElement == null)
        {
          throw new ExDbConnectionReadException("Connection ID for " + name + " database is not specified.");
        }
        string connectionId = CxXml.GetAttr(contentElement, "connectionId");
        if (CxUtils.IsEmpty(connectionId))
        {
          throw new ExDbConnectionReadException("Connection ID for " + name + " database is not specified.");
        }

        CxDbConnectionMetadata connection;
        try
        {
          connection = GetConnection(connectionId);
        }
        catch (Exception e)
        {
          throw new ExDbConnectionReadException("Connection ID for " + name + " database is invalid.", e);
        }
        m_ContentConnections[content] = connection;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns connection by its ID.
    /// </summary>
    /// <param name="id">ID of connection to get</param>
    /// <returns>connection metadata object</returns>
    public CxDbConnectionMetadata GetConnection(string id)
    {
      CxDbConnectionMetadata connectionMetadata;
      if (id != null && m_AllConnections.TryGetValue(id, out connectionMetadata))
      {
        return connectionMetadata;
      }
      throw new ExException(String.Format(
        "Database connection with ID = '{0}' is not defined.", id));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns connection by its ID.
    /// </summary>
    /// <param name="content">database content to get connection to</param>
    /// <returns>connection metadata object</returns>
    public CxDbConnectionMetadata GetConnection(NxDbContent content)
    {
      CxDbConnectionMetadata connectionMetadata;
      if (m_ContentConnections.TryGetValue(content, out connectionMetadata))
      {
        return connectionMetadata;
      }
      throw new ExException(String.Format(
        "Database connection with content type '{0}' is not defined.", content));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates DB connection from the given connection metadata.
    /// </summary>
    /// <param name="metadata">connection metadata</param>
    /// <returns>created connection</returns>
    static protected CxDbConnection CreateConnection(CxDbConnectionMetadata metadata)
    {
      CxDbConnection connection =
        CxDbConnection.Create(metadata.ProviderType, metadata.ConnectionString);

      if (metadata.LoggedUserInto_CONTEXT_INFO)
      {
        connection.OnBeginTransaction += new DxBeginTransaction(SetLoggedUserId);
      }
      if (metadata.SetContextInfo)
      {
        connection.OnBeginTransaction += new DxBeginTransaction(SetContextInfo);
      }

      return connection;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Set Logged User Id into CONTEXT_INFO
    /// </summary>
    static protected void SetLoggedUserId(CxDbConnection connection)
    {
            /*
      CxAppServerContext context = new CxAppServerContext();
      int userId = CxInt.Parse(context.FrameworkUserId, -1);
      if (userId >= 0)
      {
        connection.ExecuteCommand("exec p_Set_LoggedUserId @UserId = :UserId", userId);
      }
      */
    }

    //-------------------------------------------------------------------------
    /// <summary>
    /// Store current user ID, current application code and current language code 
    /// in the database CONTEXT_INFO variable.
    /// </summary>
    static protected void SetContextInfo(CxDbConnection connection)
    {
            /*
      CxAppServerContext context = new CxAppServerContext();
      object userId = context.FrameworkUserId;
      string applicationCode = context.Metadata.ApplicationCode;
      string languageCode = context.Metadata.LanguageCode;
      connection.ExecuteCommand(
        @"exec p_Set_ContextInfo 
              @UserId        = :UserId,
              @ApplicationCd = :ApplicationCd,
              @LanguageCd    = :LanguageCd",
        userId,
        applicationCode,
        languageCode);
        */
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates DB connection by the given ID.
    /// </summary>
    /// <param name="connectionId">ID of connection to create</param>
    /// <returns>created connection</returns>
    static protected CxDbConnection CreateConnection(string connectionId)
    {
      return CreateConnection(Instance.GetConnection(connectionId));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates DB connection by the Db content.
    /// </summary>
    /// <param name="content">Db content of connection to create</param>
    /// <returns>created connection</returns>
    static protected CxDbConnection CreateConnection(NxDbContent content)
    {
      return CreateConnection(Instance.GetConnection(content));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns current instance of the CxDbConnections object.
    /// The instance is stored in Http Session context.
    /// </summary>
    static protected CxDbConnections Instance
    {
      get
      {
        lock (typeof(CxDbConnections))
        {
          CxDbConnections instance =
            (CxDbConnections) (CxWebUtils.GetApplicationCachedObject(CACHED_INSTANCE_KEY));
          if (instance == null)
          {
            instance = 
              (CxDbConnections) (ConfigurationManager.GetSection(CONFIG_SECTION_NAME));
            CxWebUtils.SetApplicationCachedObject(CACHED_INSTANCE_KEY, instance);
          }
          return instance;
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns application entity DB connection.
    /// </summary>
    static public CxDbConnection CreateEntityConnection()
    {
      return CreateConnection(Instance.GetConnection(NxDbContent.Entity));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns application entity DB connection.
    /// </summary>
    /// <param name="connectionId">ID of connection to create</param>
    static public CxDbConnection CreateEntityConnection(string connectionId)
    {
      if (CxUtils.IsEmpty(connectionId))
      {
        return CreateEntityConnection();
      }
      else
      {
        return CreateConnection(Instance.GetConnection(connectionId));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns application entity DB connection.
    /// </summary>
    /// <param name="entityUsage">entity usage to get connection ID from</param>
    static public CxDbConnection CreateEntityConnection(CxEntityUsageMetadata entityUsage)
    {
      if (entityUsage == null)
      {
        return CreateEntityConnection();
      }
      else
      {
        return CreateEntityConnection(entityUsage.ConnectionId);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns application entity DB connection.
    /// </summary>
    /// <param name="entity">entity to get connection ID from</param>
    static public CxDbConnection CreateEntityConnection(CxBaseEntity entity)
    {
      if (entity == null)
      {
        return CreateEntityConnection();
      }
      else
      {
        return CreateEntityConnection(entity.Metadata);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns portal DB connection (portal custom data, etc.).
    /// </summary>
    static public CxDbConnection CreatePortalConnection()
    {
      return CreateConnection(Instance.GetConnection(NxDbContent.Portal));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns report service connection Metadata.
    /// </summary>
    static public CxDbConnectionMetadata GetReportConnection()
    {
      return Instance.GetConnection(NxDbContent.Report);
    }
    //-------------------------------------------------------------------------
  }
}
