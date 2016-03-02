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
using System.Data;
using System.Text;
using Framework.Db;
using Framework.Utils;

namespace Framework.Metadata
{
	/// <summary>
	/// Class containing permissions for the current user read from DB.
	/// </summary>
	public class CxUserPermissionProvider : IxUserPermissionProvider
	{
    //-------------------------------------------------------------------------
    // Database table names
    protected const string TABLE_USERS              = "Framework_Users";
    protected const string TABLE_ROLES              = "Framework_Roles";
    protected const string TABLE_USER_ROLES         = "Framework_UserRoles";
    protected const string TABLE_OBJECT_PERMISSIONS = "Framework_ObjectPermissions";
    //-------------------------------------------------------------------------
    protected const string COLUMN_OBJECT_PERMISSIONS_ROLE_ID = "RoleId";
    protected const string COLUMN_OBJECT_PERMISSIONS_USER_ID = "UserId";
    protected const string COLUMN_OBJECT_PERMISSIONS_OBJECT_ID = "ObjectId";
    protected const string COLUMN_OBJECT_PERMISSIONS_OBJECT_TYPE = "ObjectType";
    protected const string COLUMN_OBJECT_PERMISSIONS_PERMISSION_GROUP_ID = "PermissionGroupId";
    protected const string COLUMN_OBJECT_PERMISSIONS_PERMISSION_ID = "PermissionId";
    //-------------------------------------------------------------------------
    public const string SC_ADMINISTRATOR = "Administrator";
    //-------------------------------------------------------------------------
    protected CxMetadataHolder m_Holder = null;
    protected Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<string, string>>>> m_Data =
      new Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>();
    protected string m_UserName = "";
    protected object m_RoleId = null;
    protected bool m_IsAdministrator = false;
    protected Dictionary<CxPermissionKey, string> m_Records = new Dictionary<CxPermissionKey, string>();
    //-------------------------------------------------------------------------
    public const string SYS_LOGIN = "Developer";
    public const string SYS_LOGIN_HASH = "4lE0lVAbYwFvXW+EIg73Cw==";
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
		public CxUserPermissionProvider(CxMetadataHolder holder)
		{
      m_Holder = holder;
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds permission to the list of available permissions.
    /// </summary>
    /// <param name="roleId">role ID</param>
    /// <param name="objectType">type of metadata object (entity, attribute, etc.)</param>
    /// <param name="objectId">ID of metadata object</param>
    /// <param name="permissionGroupId">ID of permission group</param>
    /// <param name="permissionId">ID of assigned permission option</param>
    protected void AddPermission(
      int roleId,
      string objectType,
      string objectId,
      string permissionGroupId,
      string permissionId)
    {
      if (roleId < 0 ||
          String.IsNullOrEmpty(objectType) ||
          String.IsNullOrEmpty(objectId) ||
          String.IsNullOrEmpty(permissionGroupId) ||
          String.IsNullOrEmpty(permissionId))
      {
        return;
      }
      objectType = CxText.ToUpper(objectType);
      objectId = CxText.ToUpper(objectId);
      permissionGroupId = CxText.ToUpper(permissionGroupId);
      permissionId = CxText.ToUpper(permissionId);
      string currentId = GetPermissionRuleId(roleId, objectType, objectId, permissionGroupId);
      int curIndex = Holder.Security.GetIndexOfRule(currentId);
      int newIndex = Holder.Security.GetIndexOfRule(permissionId);
      bool addPermission = newIndex >= 0;
      if (addPermission && curIndex >= 0 && newIndex > curIndex)
      {
        addPermission = false;
      }
      if (!addPermission)
      {
        return;
      }
      Dictionary<string, Dictionary<string, Dictionary<string, string>>> objectTypes;
      if (!m_Data.TryGetValue(roleId, out objectTypes))
      {
        objectTypes = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
        m_Data.Add(roleId, objectTypes);
      }
      Dictionary<string, Dictionary<string, string>> objects;
      if (!objectTypes.TryGetValue(objectType, out objects))
      {
        objects = new Dictionary<string, Dictionary<string, string>>();
        objectTypes.Add(objectType, objects);
      }
      Dictionary<string, string> permissionGroups;
      if (!objects.TryGetValue(objectId, out permissionGroups))
      {
        permissionGroups = new Dictionary<string, string>();
        objects.Add(objectId, permissionGroups);
      }
      permissionGroups[permissionGroupId] = permissionId;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns permission ID for the given metadata object and permission group.
    /// </summary>
    /// <param name="roleId">role ID</param>
    /// <param name="objectType">type of metadata object (entity, attribute, etc.)</param>
    /// <param name="objectId">ID of metadata object</param>
    /// <param name="permissionGroupId">ID of permission group</param>
    protected string GetPermissionId(
      int roleId,
      string objectType,
      string objectId,
      string permissionGroupId)
    {
      if (String.IsNullOrEmpty(objectType) ||
          String.IsNullOrEmpty(objectId) ||
          String.IsNullOrEmpty(permissionGroupId))
      {
        return null;
      }
      objectType = CxText.ToUpper(objectType);
      objectId = CxText.ToUpper(objectId);
      permissionGroupId = CxText.ToUpper(permissionGroupId);
      Dictionary<string, Dictionary<string, Dictionary<string, string>>> objectTypes;
      if (!m_Data.TryGetValue(roleId, out objectTypes))
      {
        return null;
      }
      Dictionary<string, Dictionary<string, string>> objects;
      if (!objectTypes.TryGetValue(objectType, out objects))
      {
        return null;
      }
      Dictionary<string, string> permissionGroups;
      if (!objects.TryGetValue(objectId, out permissionGroups))
      {
        return null;
      }
      string permissionId;
      if (!permissionGroups.TryGetValue(permissionGroupId, out permissionId))
      {
        return null;
      }
      return permissionId;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns permission ID for the given metadata object and permission group.
    /// </summary>
    /// <param name="objectType">type of metadata object (entity, attribute, etc.)</param>
    /// <param name="objectId">ID of metadata object</param>
    /// <param name="permissionGroupId">ID of permission group</param>
    protected string GetPermissionId(
      string objectType,
      string objectId,
      string permissionGroupId)
    {
      string maxPermissionId = null;
      int minPermissionIndex = Int32.MaxValue;
      foreach (int roleId in m_Data.Keys)
      {
        CxPermission permission = GetPermission(roleId, objectType, objectId, permissionGroupId);
        if (permission == null && 
            GetIsCustomPermissionChild(roleId, objectType, objectId, permissionGroupId))
        {
          // Bug fix. For the command or attribute permissions which are children of an entity 
          // with custom permission we must take default permission if permission is not set.
          CxSecurityObject securityObject = Holder.Security.GetSecurityObject(objectType);
          if (securityObject != null)
          {
            CxPermissionGroup group = securityObject.GetPermissionGroup(permissionGroupId);
            if (group != null)
            {
              permission = group.DefaultPermission;
            }
          }
        }
        if (permission != null)
        {
          int index = Holder.Security.GetIndexOfRule(permission.RuleId);
          if (index >= 0 && index < minPermissionIndex)
          {
            maxPermissionId = permission.Id;
            minPermissionIndex = index;
          }
        }
      }
      return maxPermissionId;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if given permission is a child of entity which have Custom permission.
    /// Such children are command permissions and attribute permissions.
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="objectType"></param>
    /// <param name="objectId"></param>
    /// <param name="permissionGroupId"></param>
    /// <returns></returns>
    protected bool GetIsCustomPermissionChild(
      int roleId,
      string objectType,
      string objectId,
      string permissionGroupId)
    {
      if (!CxSecurityMetadata.TYPE_COMMAND.Equals(objectType, StringComparison.OrdinalIgnoreCase) &&
          !CxSecurityMetadata.TYPE_ATTRIBUTE.Equals(objectType, StringComparison.OrdinalIgnoreCase))
      {
        return false;
      }

      // Get entity ID and command/attribute ID from the compound security object ID.
      string entityId;
      string childObjectId;
      Holder.Security.SplitObjectId(objectType, objectId, out childObjectId, out entityId);
      if (String.IsNullOrEmpty(entityId))
      {
        return false;
      }

      // Get metadata object.
      CxMetadataObject metadataObject = null;
      if (CxSecurityMetadata.TYPE_COMMAND.Equals(objectType, StringComparison.OrdinalIgnoreCase))
      {
        metadataObject = Holder.Commands.Find(childObjectId);
      }
      else if (CxSecurityMetadata.TYPE_ATTRIBUTE.Equals(objectType, StringComparison.OrdinalIgnoreCase))
      {
        CxEntityMetadata entityMetadata = Holder.EntityUsages.Find(entityId);
        if (entityMetadata == null)
        {
          entityMetadata = Holder.Entities.Find(entityId);
        }
        if (entityMetadata != null)
        {
          metadataObject = entityMetadata.GetAttribute(childObjectId);
        }
      }

      // Get entity permission group by the metadata object.
      CxSecurityObject securityObject = Holder.Security.GetSecurityObject(objectType);
      CxEntityGroup entityGroup = null;
      if (metadataObject != null)
      {
        entityGroup = securityObject.GetEntityGroup(metadataObject);
      }
      else if (securityObject.EntityGroups.Count > 0)
      {
        entityGroup = securityObject.EntityGroups[0];
      }
      if (entityGroup == null)
      {
        return false;
      }

      // Get entity permission for the related permission group.
      CxPermission entityPermission = GetPermission(
        roleId, CxSecurityMetadata.TYPE_ENTITY, entityId, entityGroup.Id);

      return entityPermission != null && 
             entityPermission.Rule != null && 
             entityPermission.Rule.IsCustomRule;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns permission object for the given metadata object and permission group.
    /// </summary>
    /// <param name="objectType">type of metadata object (entity, attribute, etc.)</param>
    /// <param name="objectId">ID of metadata object</param>
    /// <param name="permissionGroupId">ID of permission group</param>
    /// <param name="permissionId">ID of permission</param>
    protected CxPermission GetPermission(
      string objectType,
      string objectId,
      string permissionGroupId,
      string permissionId)
    {
      if (CxUtils.NotEmpty(permissionId))
      {
        CxSecurityObject securityObject = Holder.Security.GetSecurityObject(objectType);
        if (securityObject != null)
        {
          CxPermissionGroup group = securityObject.GetPermissionGroup(permissionGroupId);
          if (group != null)
          {
            return group.GetPermission(permissionId);
          }
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns permission object for the given metadata object and permission group.
    /// </summary>
    /// <param name="roleId">role ID</param>
    /// <param name="objectType">type of metadata object (entity, attribute, etc.)</param>
    /// <param name="objectId">ID of metadata object</param>
    /// <param name="permissionGroupId">ID of permission group</param>
    protected CxPermission GetPermission(
      int roleId,
      string objectType,
      string objectId,
      string permissionGroupId)
    {
      string permissionId = GetPermissionId(roleId, objectType, objectId, permissionGroupId);
      return GetPermission(objectType, objectId, permissionGroupId, permissionId);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns permission object for the given metadata object and permission group.
    /// </summary>
    /// <param name="objectType">type of metadata object (entity, attribute, etc.)</param>
    /// <param name="objectId">ID of metadata object</param>
    /// <param name="permissionGroupId">ID of permission group</param>
    virtual public CxPermission GetPermission(
      string objectType,
      string objectId,
      string permissionGroupId)
    {
      if (IsDevelopmentMode || IsAdministrator)
      {
        return Holder.Security.GetAllowPermission(objectType, objectId, permissionGroupId);
      }
      string permissionId = GetPermissionId(objectType, objectId, permissionGroupId);
      return GetPermission(objectType, objectId, permissionGroupId, permissionId);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns rule ID for the given metadata object and permission group.
    /// </summary>
    /// <param name="roleId">role ID</param>
    /// <param name="objectType">type of metadata object (entity, attribute, etc.)</param>
    /// <param name="objectId">ID of metadata object</param>
    /// <param name="permissionGroupId">ID of permission group</param>
    protected string GetPermissionRuleId(
      int roleId,
      string objectType,
      string objectId,
      string permissionGroupId)
    {
      CxPermission permission = GetPermission(roleId, objectType, objectId, permissionGroupId);
      return permission != null ? permission.RuleId : null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns permission rule object (if specified).
    /// </summary>
    /// <param name="objectType">type of metadata object (entity, attribute, etc.)</param>
    /// <param name="objectId">ID of metadata object</param>
    /// <param name="permissionGroupId">ID of permission group</param>
    virtual public CxPermissionRule GetRule(
      string objectType,
      string objectId,
      string permissionGroupId)
    {
      if (IsDevelopmentMode || IsAdministrator)
      {
        return Holder.Security.AllowRule;
      }
      CxPermission permission = GetPermission(objectType, objectId, permissionGroupId);
      return permission != null ? permission.Rule : null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Clears all loaded permissions.
    /// </summary>
    protected void Clear()
    {
      m_Data.Clear();
      m_Records.Clear();
      m_UserName = "";
      m_RoleId = null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads user permissions list from database.
    /// </summary>
    /// <param name="connection">DB connection to load permissions from</param>
    /// <param name="userName">current user name</param>
    virtual public void LoadUser(CxDbConnection connection, string userName)
    {
      Clear();
      m_IsAdministrator = false;
      if (CxUtils.NotEmpty(userName))
      {
        if (userName != SYS_LOGIN)
        {
          string sql =
            "SELECT op.* " +
            "  FROM " + TABLE_OBJECT_PERMISSIONS + " op, " +
            "       " + TABLE_ROLES + " r, " +
            "       " + TABLE_USER_ROLES + " ur, " +
            "       " + TABLE_USERS + " u " +
            " WHERE op.RoleId = r.RoleId " +
            "   AND r.RoleId = ur.RoleId " +
            "   AND ur.UserId = u.UserId " +
            "   AND u.Login = :Login ";

          DataTable dt = connection.GetQueryResult(sql, userName);

          UniqueList<int> roles = new UniqueList<int>();
          foreach (DataRow row in dt.Rows)
          {
            AddPermission(
              CxInt.Parse(row[COLUMN_OBJECT_PERMISSIONS_ROLE_ID], -1),
              row[COLUMN_OBJECT_PERMISSIONS_OBJECT_TYPE].ToString(),
              row[COLUMN_OBJECT_PERMISSIONS_OBJECT_ID].ToString(),
              row[COLUMN_OBJECT_PERMISSIONS_PERMISSION_GROUP_ID].ToString(),
              row[COLUMN_OBJECT_PERMISSIONS_PERMISSION_ID].ToString());

            m_Records[new CxPermissionKey(row)] = row[COLUMN_OBJECT_PERMISSIONS_PERMISSION_ID].ToString();
            roles.Add(CxInt.Parse(row[COLUMN_OBJECT_PERMISSIONS_ROLE_ID], 0));
          }

          sql =
            @"SELECT COUNT(1) AS RecCount
                FROM " + TABLE_ROLES + @" r,
                     " + TABLE_USER_ROLES + @" ur,
                     " + TABLE_USERS + @" u
               WHERE r.RoleId = ur.RoleId 
                 AND ur.UserId = u.UserId 
                 AND u.Login = :Login 
                 AND r.SpecialCategory = :SpecialCategory";
          int count =
            CxInt.Parse(connection.ExecuteScalar(sql, userName, SC_ADMINISTRATOR), 0);
          if (count > 0)
          {
            m_IsAdministrator = true;
          }

          m_UserName = userName;

          // Add entity permissions provided by subsystems
          IList<string> subsystems = Holder.GetEntityGroupNameList();
          IList<CxEntityMetadata> entities = Holder.Security.GetSecurityEntityList();
          CxSecurityObject securityObject = Holder.Security.GetSecurityObject(CxSecurityMetadata.TYPE_ENTITY);
          foreach (string subsystem in subsystems)
          {
            foreach (CxEntityMetadata entity in entities)
            {
              if (entity.GroupName == subsystem)
              {
                foreach (CxPermissionGroup group in securityObject.PermissionGroups)
                {
                  foreach (int roleId in roles)
                  {
                    CxPermissionKey entityKey =
                      new CxPermissionKey(entity.Id, CxSecurityMetadata.TYPE_ENTITY, group.Id, roleId);
                    CxPermissionKey subsystemKey =
                      new CxPermissionKey(subsystem, CxSecurityMetadata.TYPE_SUBSYSTEM, group.Id, roleId);
                    if (!m_Records.ContainsKey(entityKey))
                    {
                      string permissionId;
                      if (m_Records.TryGetValue(subsystemKey, out permissionId))
                      {
                        AddPermission(roleId, CxSecurityMetadata.TYPE_ENTITY, entity.Id, group.Id, permissionId);
                      }
                    }
                  }
                }
              }
            }
          }
        }
        else
        {
          // For Developer user
          m_UserName = userName;
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads role permissions list from database.
    /// </summary>
    /// <param name="connection">DB connection to load permissions from</param>
    /// <param name="roleId">role ID</param>
    virtual public void LoadRole(CxDbConnection connection, object roleId)
    {
      Clear();
      m_IsAdministrator = false;
      if (CxUtils.NotEmpty(roleId))
      {
        string sql =
          "SELECT op.* " +
          "  FROM " + TABLE_OBJECT_PERMISSIONS + " op " +
          " WHERE op.RoleId = :RoleId ";

        DataTable dt = connection.GetQueryResult(sql, roleId);
        foreach (DataRow row in dt.Rows)
        {
          AddPermission(
            CxInt.Parse(roleId, -1),
            row[COLUMN_OBJECT_PERMISSIONS_OBJECT_TYPE].ToString(),
            row[COLUMN_OBJECT_PERMISSIONS_OBJECT_ID].ToString(),
            row[COLUMN_OBJECT_PERMISSIONS_PERMISSION_GROUP_ID].ToString(),
            row[COLUMN_OBJECT_PERMISSIONS_PERMISSION_ID].ToString());
        }

        sql = 
          @"SELECT COUNT(1) AS RecCount
                FROM " + TABLE_ROLES + @" r
               WHERE r.RoleId = :RoleId 
                 AND r.SpecialCategory = :SpecialCategory";
        int count = 
          CxInt.Parse(connection.ExecuteScalar(sql, roleId, SC_ADMINISTRATOR), 0);
        if (count > 0)
        {
          m_IsAdministrator = true;
        }

        m_RoleId = roleId;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if given permission is default for the given metadata object.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="roleId">ID of role to check permission for</param>
    /// <param name="metadataObject">metadata object</param>
    /// <param name="permissionGroupId">ID of permission group</param>
    /// <param name="permissionId">ID of permission</param>
    protected NxBoolEx GetIsPermissionDefault(
      CxDbConnection connection,
      object roleId,
      CxMetadataObject metadataObject,
      string permissionGroupId,
      string permissionId)
    {
      if (metadataObject is CxEntityMetadata)
      {
        CxEntityMetadata entity = (CxEntityMetadata) metadataObject;
        if (CxUtils.NotEmpty(entity.GroupName))
        {
          string sql = 
            @"select top 1 op.PermissionId
                from " + TABLE_OBJECT_PERMISSIONS + @" op
               where op.ObjectType = :ObjectType
                 and op.ObjectId = :ObjectId
                 and op.PermissionGroupId = :PermissionGroupId
                 and op.RoleId = :RoleId";

          string subsystemPermissionId = CxUtils.ToString(
            connection.ExecuteScalar(
              sql, 
              CxSecurityMetadata.TYPE_SUBSYSTEM, 
              entity.GroupName, 
              permissionGroupId, 
              roleId));

          if (CxUtils.NotEmpty(subsystemPermissionId))
          {
            return CxBoolEx.GetBoolEx(subsystemPermissionId == permissionId);
          }
        }
      }
      return NxBoolEx.Undefined;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets permission option for the given metadata object and permission group.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="roleId">ID of role to set permission for</param>
    /// <param name="metadataObject">metadata object</param>
    /// <param name="objectType">type of metadata object (entity, attribute, etc.)</param>
    /// <param name="objectId">ID of metadata object</param>
    /// <param name="permissionGroupId">ID of permission group</param>
    /// <param name="permissionGroupId">ID of permission option</param>
    /// <param name="permissionId">permission id</param>
    virtual public void SetPermission(
      CxDbConnection connection,
      object roleId,
      CxMetadataObject metadataObject,
      string objectType,
      string objectId,
      string permissionGroupId,
      string permissionId)
    {
      if (CxUtils.NotEmpty(permissionId))
      {
        CxSecurityObject securityObject = Holder.Security.GetSecurityObject(objectType);
        if (securityObject != null)
        {
          CxPermissionGroup group = securityObject.GetPermissionGroup(permissionGroupId);
          if (group != null)
          {
            CxPermission permission = group.GetPermission(permissionId);
            if (permission != null)
            {
              string checkSql = 
                @"SELECT COUNT(*) AS RoleCount
                    FROM " + TABLE_ROLES + @"
                   WHERE RoleId = :RoleId 
                     AND SpecialCategory = :SpecialCategory";
              int result = 
                CxInt.Parse(connection.ExecuteScalar(checkSql, roleId, SC_ADMINISTRATOR), 0);
              if (result > 0)
              {
                throw new ExException(Holder.GetErr("Administrator role permissions could not be changed."));
              }

              bool ownsTransaction = !connection.InTransaction;
              if (ownsTransaction)
              {
                connection.BeginTransaction();
              }
              try
              {
                string deleteSql = 
                  "DELETE FROM " + TABLE_OBJECT_PERMISSIONS +
                  " WHERE RoleId = :RoleId " +
                  "   AND ObjectType = :ObjectType " +
                  "   AND ObjectId = :ObjectId " +
                  "   AND PermissionGroupId = :PermissionGroupId";
                connection.ExecuteCommand(
                  deleteSql, roleId, objectType.ToUpper(), objectId, permissionGroupId);

                /*NxBoolEx isDefault = GetIsPermissionDefault(
                  connection, roleId, metadataObject, permissionGroupId, permissionId);
                if (isDefault == NxBoolEx.Undefined)
                {
                  isDefault = CxBoolEx.GetBoolEx(permission.IsDefault);
                }
                if (isDefault == NxBoolEx.False)
                {*/
                  string insertSql =
                    "INSERT INTO " + TABLE_OBJECT_PERMISSIONS +
                    "       (RoleId, ObjectType, ObjectId, PermissionGroupId, PermissionId) " +
                    "VALUES (:RoleId, :ObjectType, :ObjectId, :PermissionGroupId, :PermissionId)";
                  connection.ExecuteCommand(
                    insertSql, roleId, objectType.ToUpper(), objectId, permissionGroupId, permissionId);
                //}
                if (ownsTransaction)
                {
                  connection.Commit();
                }
              }
              catch
              {
                if (ownsTransaction)
                {
                  connection.Rollback();
                }
                throw;
              }
            }
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Validates user login and password.
    /// Raises exception if login or password is invalid.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="userName">user name (login)</param>
    /// <param name="password">password</param>
    /// <param name="validatePassword">if false, user password will be not validated</param>
    /// <returns>user id</returns>
    virtual public CxUserInfo ValidateLogin(
      CxDbConnection connection, 
      string userName, 
      string password,
      bool validatePassword)
    {
      if (CxUtils.IsEmpty(userName))
      {
        throw new ExValidationException(Holder.GetErr("User name could not be empty."));
      }
      if (validatePassword && CxUtils.IsEmpty(password))
      {
        throw new ExValidationException(Holder.GetErr("Password could not be empty."));
      }

      if (userName == SYS_LOGIN)
      {
        if (validatePassword && CxCommon.ComputeMD5Hash(password) != SYS_LOGIN_HASH)
        {
          throw new ExValidationException(Holder.GetErr("Invalid password."));
        }
        return new CxUserInfo(0, userName, false);
      }

      string sql =
        "SELECT u.UserId, " +
        "       u.Login, " +
        "       u.Password, " +
        "       u.IsDeactivated, " +
        "       u.DeactivatedDate, " +
        "       u.IsForceToChangePassword, " +
        "       (SELECT COUNT(*) " +
        "          FROM " + TABLE_USER_ROLES + " ur " +
        "         WHERE u.UserId = ur.UserId) AS RoleCount " +
        "  FROM " + TABLE_USERS + " u " +
        " WHERE u.Login = :Login";
      DataTable dt = connection.GetQueryResult(sql, userName);
      if (dt.Rows.Count == 0)
      {
        throw new ExValidationException(Holder.GetErr("User name is invalid."));
      }
      else if (dt.Rows.Count > 1)
      {
        throw new ExValidationException(Holder.GetErr("User name is not unique."));
      }

      if (validatePassword &&
          CxCommon.ComputeMD5Hash(password) != dt.Rows[0]["Password"].ToString())
      {
        throw new ExValidationException(Holder.GetErr("Invalid password."));
      }

      if (CxBool.Parse(dt.Rows[0]["IsDeactivated"]))
      {
        throw new ExValidationException(Holder.GetErr("User name is deactivated."));
      }
      if (CxUtils.IsEmpty(dt.Rows[0]["RoleCount"]) ||
        (int) dt.Rows[0]["RoleCount"] == 0)
      {
        throw new ExValidationException(Holder.GetErr("User does not have roles."));
      }
      int userId = CxInt.Parse(dt.Rows[0]["UserId"], -1);
      if (userId < 0)
        throw new ExValidationException(Holder.GetErr("User has invalid UserId"));
      bool isForceToChangePassword = CxBool.Parse(dt.Rows[0]["IsForceToChangePassword"], false);
      
      CxUserInfo result = new CxUserInfo();
      result.UserId = userId;
      result.UserName = userName;
      result.IsForceToChangePassword = isForceToChangePassword;
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns user ID for the given user name (login).
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="userName">user name (login)</param>
    virtual public object GetUserId(
      CxDbConnection connection,
      string userName)
    {
      if (userName == SYS_LOGIN)
      {
        return 0;
      }
      string sql =
        "SELECT UserId FROM " + TABLE_USERS + 
        " WHERE Login = :Login";
      object userId = connection.ExecuteScalar(sql, userName);
      return userId;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns framework user ID for the given user name (login).
    /// Framework user ID means user ID stored in the Framework_Users table.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="userName">user name (login)</param>
    virtual public object GetFrameworkUserId(CxDbConnection connection, string userName)
    {
      return GetUserId(connection, userName);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets user's assigned role IDs.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="userId">an ID of a user</param>
    /// <returns>an array of role IDs</returns>
    virtual public object[] GetUserRoles(
      CxDbConnection connection,
      object userId)
    {
      string selectSql =
        "select RoleId " +
        "  from " + TABLE_USER_ROLES +
        " where UserId = :UserId";
      DataTable dt = new DataTable();
      connection.GetQueryResult(dt, selectSql, userId);
      object[] roleIds = new object[dt.Rows.Count];
      for (int i = 0; i < dt.Rows.Count; i++)
      {
        DataRow row = dt.Rows[i];
        roleIds[i] = row[0];
      }
      return roleIds;
    }
	  //-------------------------------------------------------------------------
    /// <summary>
    /// Assigns roles to user.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="userId">ID of user</param>
    /// <param name="roleIdArray">array of Role IDs</param>
    virtual public void AssignUserRoles(
      CxDbConnection connection,
      object userId,
      object[] roleIdArray)
    {
      string checkSql =
        "SELECT COUNT(*) " +
        "  FROM " + TABLE_USER_ROLES + 
        " WHERE UserId = :UserId " +
        "   AND RoleId = :RoleId";
      string insertSql =
        "INSERT INTO " + TABLE_USER_ROLES + " (UserId, RoleId) " +
        "VALUES (:UserId, :RoleId)";
      connection.BeginTransaction();
      try
      {
        foreach (object roleId in roleIdArray)
        {
          object result = connection.ExecuteScalar(checkSql, userId, roleId);
          if (result is int && (int)result > 0)
          {
            continue;
          }
          connection.ExecuteCommand(insertSql, userId, roleId);
        }
        connection.Commit();
      }
      catch
      {
        connection.Rollback();
        throw;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Removes roles from user.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="userId">ID of user</param>
    /// <param name="roleIdArray">array of Role IDs</param>
    virtual public void RemoveUserRoles(
      CxDbConnection connection,
      object userId,
      object[] roleIdArray)
    {
      string deleteSql =
        "DELETE FROM " + TABLE_USER_ROLES + 
        " WHERE UserId = :UserId AND RoleId = :RoleId";
      connection.BeginTransaction();
      try
      {
        foreach (object roleId in roleIdArray)
        {
          connection.ExecuteCommand(deleteSql, userId, roleId);
        }
        connection.Commit();
      }
      catch
      {
        connection.Rollback();
        throw;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Marks user as active.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="userId">ID of user</param>
    virtual public void ActivateUser(CxDbConnection connection, object userId)
    {
      string sql =
        "UPDATE " + TABLE_USERS + 
        "   SET IsDeactivated = NULL, DeactivatedDate = NULL " +
        " WHERE UserId = :UserId";
      connection.ExecuteCommand(sql, userId);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Marks user as inactive.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="userId">ID of user</param>
    virtual public void DeactivateUser(CxDbConnection connection, object userId)
    {
      string sql =
        "UPDATE " + TABLE_USERS + 
        "   SET IsDeactivated = 1, DeactivatedDate = GETDATE() " +
        " WHERE UserId = :UserId " +
        "   AND (IsDeactivated = 0 OR IsDeactivated IS NULL)";
      connection.ExecuteCommand(sql, userId);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if application is in development mode (security is not applied).
    /// </summary>
    virtual protected bool GetIsDevelopmentMode()
    {
      return m_UserName == SYS_LOGIN;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Validates DB user name before insert or update.
    /// </summary>
    /// <param name="userName">user name to validate</param>
    virtual public void ValidateDbUserName(string userName)
    {
      if (CxText.Equals(userName, SYS_LOGIN))
      {
        throw new ExValidationException(
          Holder.GetErr("This user name is reserved for internal system purposes.\r\nPlease try another user name."));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns user info parameter value.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="userId">ID of user</param>
    /// <param name="paramName">name of parameter to get from user info</param>
    virtual public string GetUserInfo(
      CxDbConnection connection, 
      object userId, 
      string paramName)
    {
      string sql = 
        "select " + paramName + " from " + TABLE_USERS + " where UserId = :UserId";
      return CxUtils.ToString(connection.ExecuteScalar(sql, userId));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns table of available workspaces for the given user.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="userId">ID of user</param>
    virtual public DataTable GetAvailableWorkspaces(CxDbConnection connection, object userId)
    {
      DataTable dt;
      string sql;
      if (!IsDevelopmentMode)
      {
        sql = 
          @"select distinct 
                 w.WorkspaceId, 
                 w.Name,
                 w.Code,
                 w.Priority,
                 case 
                   when u.DefaultWorkspaceId = w.WorkspaceId
                   then 0
                   else 1
                 end as DefaultOrder
            from Framework_UserRoles ur
           inner join Framework_RoleWorkspaces rw
              on ur.RoleId = rw.RoleId
           inner join Framework_Workspaces w
              on rw.WorkspaceId = w.WorkspaceId
           inner join Framework_Users u
              on ur.UserId = u.UserId
           where ur.UserId = :UserId
             and w.WorkspaceId > 0
             and w.IsAvailable = 1

           union
           select 
                 w.WorkspaceId, 
                 w.Name,
                 w.Code,
                 w.Priority,
                 0 as DefaultOrder
           from Framework_Workspaces w
           inner join Framework_Users u
              on u.DefaultWorkspaceId = w.WorkspaceId
           where u.UserId = :UserId
             and w.WorkspaceId > 0
             and w.IsAvailable = 1

           order by 5, 4";
        dt = connection.GetQueryResult(sql, userId);
      }
      else
      {
        sql = 
          @"select w.WorkspaceId, 
                   w.Name,
                   w.Code,
                   w.Priority,
                   0 as DefaultOrder
              from Framework_Workspaces w
             where w.WorkspaceId > 0
               and w.IsAvailable = 1
             order by w.Priority";
        dt = connection.GetQueryResult(sql);
      }
      return dt;
    }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Parent metadata holder object.
    /// </summary>
    public CxMetadataHolder Holder
    { get {return m_Holder;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns user name whose permissions were loaded.
    /// </summary>
    public string UserName
    { get {return m_UserName;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns role ID which permissions were loaded.
    /// </summary>
    public object RoleId
    { get {return m_RoleId;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if application is in development mode (security is not applied).
    /// </summary>
    public bool IsDevelopmentMode
    { get {return GetIsDevelopmentMode();} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if logged user is an administrator user.
    /// </summary>
    virtual public bool IsAdministrator
    { get {return m_IsAdministrator;} }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    protected class CxPermissionKey : IComparable, IEquatable<CxPermissionKey>
    {
      private string m_ObjectId;
      private string m_ObjectType;
      private string m_PermissionGroupId;
      private int m_RoleId;

      //-----------------------------------------------------------------------
      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="objectId"></param>
      /// <param name="objectType"></param>
      /// <param name="permissionGroupId"></param>
      /// <param name="roleId"></param>
      public CxPermissionKey(
        string objectId,
        string objectType,
        string permissionGroupId,
        int roleId)
      {
        m_ObjectId = objectId;
        m_ObjectType = objectType;
        m_PermissionGroupId = permissionGroupId;
        m_RoleId = roleId;
      }
      //-----------------------------------------------------------------------
      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="dr">data row</param>
      public CxPermissionKey(DataRow dr)
      {
        m_ObjectId = dr["ObjectId"].ToString();
        m_ObjectType = dr["ObjectType"].ToString();
        m_PermissionGroupId = dr["PermissionGroupId"].ToString();
        m_RoleId = CxInt.Parse(dr["RoleId"], 0);
      }
      //-----------------------------------------------------------------------
      ///<summary>
      ///Compares the current instance with another object of the same type.
      ///</summary>
      ///
      ///<returns>
      ///A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than obj. Zero This instance is equal to obj. Greater than zero This instance is greater than obj. 
      ///</returns>
      ///
      ///<param name="obj">An object to compare with this instance. </param>
      ///<exception cref="T:System.ArgumentException">obj is not the same type as this instance. </exception><filterpriority>2</filterpriority>
      public int CompareTo(object obj)
      {
        CxPermissionKey key = obj as CxPermissionKey;
        if (key != null)
        {
          if (ObjectId.Equals(key.ObjectId, StringComparison.OrdinalIgnoreCase) &&
              ObjectType.Equals(key.ObjectType, StringComparison.OrdinalIgnoreCase) &&
              PermissionGroupId.Equals(key.PermissionGroupId, StringComparison.OrdinalIgnoreCase) &&
              RoleId == key.RoleId)
          {
            return 0;
          }
          else
          {
            return ObjectId.CompareTo(key.ObjectId);
          }
        }
        return 1;
      }
      //-----------------------------------------------------------------------
      ///<summary>
      ///Indicates whether the current object is equal to another object of the same type.
      ///</summary>
      ///
      ///<returns>
      ///true if the current object is equal to the other parameter; otherwise, false.
      ///</returns>
      ///
      ///<param name="other">An object to compare with this object.</param>
      public bool Equals(CxPermissionKey other)
      {
        return CompareTo(other) == 0;
      }
      //-----------------------------------------------------------------------
      /// <summary>
      /// Retruns true if objects are equal.
      /// </summary>
      /// <param name="obj">object to compare</param>
      /// <returns>true if objects are equal</returns>
      public override bool Equals(object obj)
      {
        return Equals(obj as CxPermissionKey);
      }
      //-----------------------------------------------------------------------
      /// <summary>
      /// Returns object hash code.
      /// </summary>
      /// <returns>hash code</returns>
      public override int GetHashCode()
      {
        string text = String.Format(
          "ObjectId:{0};ObjectType:{1};PermissionGroupId:{2};RoleId:{3}",
          ObjectId.ToUpper(), ObjectType.ToUpper(), PermissionGroupId.ToUpper(), RoleId);
        return text.GetHashCode();
      }
      //-----------------------------------------------------------------------
      /// <summary>
      /// Object ID
      /// </summary>
      public string ObjectId
      {
        get { return m_ObjectId; }
        set { m_ObjectId = value; }
      }
      //-----------------------------------------------------------------------
      /// <summary>
      /// Object type
      /// </summary>
      public string ObjectType
      {
        get { return m_ObjectType; }
        set { m_ObjectType = value; }
      }
      //-----------------------------------------------------------------------
      /// <summary>
      /// Permission Group ID
      /// </summary>
      public string PermissionGroupId
      {
        get { return m_PermissionGroupId; }
        set { m_PermissionGroupId = value; }
      }
      //-----------------------------------------------------------------------
      /// <summary>
      /// Role ID
      /// </summary>
      public int RoleId
      {
        get { return m_RoleId; }
        set { m_RoleId = value; }
      }
    }
    //-------------------------------------------------------------------------
  }
}