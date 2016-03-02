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

using System.Collections;
using System.Data;
using System.Security.Principal;
using System.Web;
using System.Web.SessionState;
using System.Xml.Linq;

using Framework.Db;
using Framework.Metadata;
using Framework.Utils;

using Hashtable = System.Collections.Hashtable;
using Framework.Entity;
using System;
using System.Collections.Generic;

namespace Framework.Remote
{
  /// <summary>
  /// Container for most common server data.
  /// </summary>
  public class CxAppServerContext : IxValueProvider
  {
    private readonly HttpContext m_context = HttpContext.Current;
    private readonly HttpSessionState m_session = HttpContext.Current.Session;
    private readonly CxMetadataHolder m_metadata = (CxMetadataHolder)HttpContext.Current.Application[CxAppServerConsts.METADATA_APP_KEY];
    //-------------------------------------------------------------------------
    protected const string SESSION_PAGE_STATE_STACK_KEY = "PageStateStack";
    protected const string SESSION_TRANSFER_PAGE_STATE_KEY = "TransferPageState";
    //-------------------------------------------------------------------------
    protected const string SESSION_KEY_USER_CONTEXT_TABLE = "UserSessionContextTable";
    protected const string SESSION_KEY_USER_CONTEXT_NAME = "UserSessionContextName";
    //-------------------------------------------------------------------------
    protected const string APP_KEY_METADATA = "GlobalMetadata";
    protected const string APP_KEY_IMAGE_CACHE = "GlobalImageCache";
    //-------------------------------------------------------------------------
    protected const string USER_CONTEXT_KEY_USER_PERMISSION_PROVIDER = "UserPermissionProvider";
    protected const string USER_CONTEXT_KEY_USER_ID = "UserId";
    protected const string USER_CONTEXT_KEY_FRAMEWORK_USER_ID = "FrameworkUserId";
    protected const string USER_CONTEXT_KEY_ENTITY_RULE_CACHE = "EntityRuleCache";
    protected const string USER_CONTEXT_KEY_IS_USER_VALIDATED = "IsUserValidated";
    protected const string USER_CONTEXT_KEY_WORKSPACE_TABLE = "AvailableWorkspacesTable";
    protected const string USER_CONTEXT_KEY_CURRENT_WORKSPACE_ID = "CurrentWorkspaceId";
    protected const string USER_CONTEXT_KEY_ENTITY_MARKS = "EntityMarks";
    //-------------------------------------------------------------------------
    protected const string CACHE_KEY_APP_PARAMS = "ApplicationParameters";

    /// <summary>
    /// Returns dictionary with user context variables that depend on current logged user.
    /// Is used for caching of values depending on current user.
    /// </summary>
    public IDictionary UserSessionContext
    {
      get
      {
        IDictionary table = (IDictionary)m_session[SESSION_KEY_USER_CONTEXT_TABLE];
        string name = CxUtils.ToString(m_session[SESSION_KEY_USER_CONTEXT_NAME]);
        if (table == null || name.ToUpper() != UserName.ToUpper())
        {
          table = new Hashtable();
          m_session[SESSION_KEY_USER_CONTEXT_TABLE] = table;
          m_session[SESSION_KEY_USER_CONTEXT_NAME] = UserName;
        }
        return table;
      }
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns name of the current logged user.
    /// </summary>
    virtual public string UserName
    {
      get
      {
        if (m_context != null && m_context.User != null && m_context.User.Identity != null)
        {
          return CxUtils.Nvl(m_context.User.Identity.Name);
        }
        return "";
      }
      set
      {
        if (CxText.ToUpper(m_context.User.Identity.Name) != CxText.ToUpper(value))
        {
          GenericIdentity identity = new GenericIdentity(value);
          m_context.User = new GenericPrincipal(identity, new string[] { });
        }
      }
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns current user ID to pass to security WHERE statements.
    /// </summary>
    public object UserId
    {
      get
      {
        object userId = UserSessionContext[USER_CONTEXT_KEY_USER_ID];
        if (userId == null)
        {
          IxUserPermissionProvider provider = GetUserPermissionProvider();
          if (provider != null)
          {
            using (CxDbConnection connection = CreateDbConnection())
            {
              userId = provider.GetUserId(connection, UserName);
              UserSessionContext[USER_CONTEXT_KEY_USER_ID] = userId;
            }
          }
        }
        return userId;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns list of CxEntityMarks that contains 'History' and 'Favorites' items.
    /// </summary>
    public CxEntityMarks EntityMarks
    {
      get
      {
        CxEntityMarks marks = UserSessionContext[USER_CONTEXT_KEY_ENTITY_MARKS] as CxEntityMarks;
        if (marks == null && UserId != null)
        {
          using (CxDbConnection connection = CreateDbConnection())
          {
            marks = CxEntityMarks.Create(
              connection,
              (int)UserId,
              m_metadata);
            UserSessionContext[USER_CONTEXT_KEY_ENTITY_MARKS] = marks;
          }
        }
        return marks;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns value.
    /// </summary>
    public virtual object this[string name]
    {
      get
      {
        if (name != null)
        {
          if (name.ToUpper() == "APPLICATION$USERID")
          {
            return UserId;
          }
          else if (name.ToUpper() == "APPLICATION$USERNAME")
          {
            return UserName;
          }
          else if (name.ToUpper() == "APPLICATION$USEREMAIL")
          {
            if (CxUtils.NotEmpty(UserId) &&
                Metadata != null &&
                Metadata.Security != null &&
                Metadata.Security.UserPermissionProvider != null)
            {
              using (CxDbConnection connection = CreateDbConnection())
              {
                return Metadata.Security.UserPermissionProvider.GetUserInfo(
                    connection, UserId, "Email");
              }
            }
          }
          else if (name.ToUpper() == "APPLICATION$APPLICATIONCODE")
          {
            return Metadata.ApplicationCode;
          }
          else if (name.ToUpper() == "APPLICATION$FRAMEWORKUSERID")
          {
            return FrameworkUserId;
          }
          else if (name.ToUpper() == "APPLICATION$CURRENTWORKSPACEID")
          {
            return CurrentWorkspaceId;
          }
          else if (name.ToUpper() == "APPLICATION$WORKSPACEAVAILABLEFORUSERTABLE")
          {
            return AvailableWorkspacesTable;
          }
          else if (name.ToUpper() == "APPLICATION$WORKSPACEAVAILABLEFORUSERTABLE_XML")
          {
            return AvailableWorkspacesXML;
          }
         
          else if (name.ToUpper() == "APPLICATION$LANGUAGECODE")
          {
            return Metadata.LanguageCode;
          }
          else if (name.ToUpper() == "APPLICATION$LOCALIZATIONAPPLICATIONCODE")
          {
            return Metadata.LocalizationApplicationCode;
          }
        }
        return null;
      }
      set
      {
        throw new ExException("Value of global application value provider could not be changed.");
      }

    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns user permission provider.
    /// </summary>
    public IxUserPermissionProvider GetUserPermissionProvider()
    {
      IxUserPermissionProvider result =
        (IxUserPermissionProvider)UserSessionContext[USER_CONTEXT_KEY_USER_PERMISSION_PROVIDER];
      if (result == null)
      {
        result = new CxUserPermissionProvider(Metadata);
        using (CxDbConnection connection = CreateDbConnection())
        {
          result.LoadUser(connection, UserName);
        }
        UserSessionContext[USER_CONTEXT_KEY_USER_PERMISSION_PROVIDER] = result;
      }

      return result;
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates and returns CxDbConnection.
    /// </summary>
    /// <returns>Created CxDbConnection.</returns>
    private CxDbConnection CreateDbConnection()
    {
      return CxDbConnections.CreateEntityConnection();
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns current framework user ID to pass to security WHERE statements.
    /// </summary>
    public object FrameworkUserId
    {
      get
      {
        object userId = UserSessionContext[USER_CONTEXT_KEY_FRAMEWORK_USER_ID];
        if (userId == null)
        {
          IxUserPermissionProvider provider = GetUserPermissionProvider();
          if (provider != null)
          {
            using (CxDbConnection connection = CreateDbConnection())
            {
              userId = provider.GetFrameworkUserId(connection, UserName);
              UserSessionContext[USER_CONTEXT_KEY_FRAMEWORK_USER_ID] = userId;
            }
          }
        }
        return userId;
      }
    }

    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets current workspace ID.
    /// </summary>
    public int CurrentWorkspaceId
    {
      get
      {
        // Get workspace ID from the user session context
        int workspaceId = CxInt.Parse(UserSessionContext[USER_CONTEXT_KEY_CURRENT_WORKSPACE_ID], 0);
        if (workspaceId == 0)
        {
          // Get workspace ID from the database cache
          string dbCacheCd = m_session.SessionID + ":" + UserName + ":WorkspaceId";
          using (CxDbConnection connection = CreateDbConnection())
          {
            workspaceId = CxInt.Parse(CxDbUtils.GetCachedValue(connection, dbCacheCd), 0);
          }
        }
        if (workspaceId == 0)
        {
          // Return default workspace ID
          DataTable dt = AvailableWorkspacesTable;
          if (dt != null && dt.Rows.Count > 0)
          {
            workspaceId = CxInt.Parse(dt.Rows[0]["WorkspaceId"], 0);
            UserSessionContext[USER_CONTEXT_KEY_CURRENT_WORKSPACE_ID] = workspaceId;
          }
        }
        return workspaceId;
      }

      set
      {
        bool isValid = false;
        DataTable dt = AvailableWorkspacesTable;
        if (dt != null)
        {
          foreach (DataRow dr in dt.Rows)
          {
            if (CxInt.Parse(dr["WorkspaceId"], 0) == value)
            {
              isValid = true;
            }
          }
        }
        if (isValid)
        {
          UserSessionContext[USER_CONTEXT_KEY_CURRENT_WORKSPACE_ID] = value;
        }
      }
    }

    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns table of workspaces available for the current user.
    /// </summary>
    public DataTable AvailableWorkspacesTable
    {
      get
      {
        DataTable dt = (DataTable)UserSessionContext[USER_CONTEXT_KEY_WORKSPACE_TABLE];
        if (dt != null)
        {
          return dt;
        }
        dt = new DataTable();
        IxUserPermissionProvider provider = GetUserPermissionProvider();
        if (provider != null)
        {
          using (CxDbConnection connection = CreateDbConnection())
          {
            dt = provider.GetAvailableWorkspaces(connection, UserId);
          }
        }
        UserSessionContext[USER_CONTEXT_KEY_WORKSPACE_TABLE] = dt;
        return dt;
      }
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns XML of workspaces available for the current user.
    /// </summary>
    public string AvailableWorkspacesXML
    {
      get
      {
        return ConvertDataTableToXml(AvailableWorkspacesTable);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Metadata holder
    /// </summary>
    public CxMetadataHolder Metadata
    {
      get { return m_metadata; }
    }

        private Dictionary<string, string> valueTypes = new Dictionary<string, string>();
        public IDictionary<string, string> ValueTypes
        {
            get
            {
                return valueTypes;
            }
        }

        //----------------------------------------------------------------------------
        /// <summary>
        /// Convert Sysstem.Data.DataTable to XML.
        /// </summary>
        /// <param name="table">DataTable to conversion.</param>
        /// <returns>Created XML with values from given DataTable.</returns>
        private string ConvertDataTableToXml(DataTable table)
    {
      XElement root = new XElement("AvailableWorkspaces");

      foreach (DataRow row in table.Rows)
      {
        XElement workspace = new XElement("workspace");

        foreach (DataColumn column in table.Columns)
        {
          XAttribute attribute = new XAttribute(column.ColumnName, row[column.ColumnName]);
          workspace.Add(attribute);
        }
        root.Add(workspace);
      }
      return root.ToString(SaveOptions.DisableFormatting);
    }
  }
}
