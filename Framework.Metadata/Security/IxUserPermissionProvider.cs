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
using System.Data;
using Framework.Db;
using Framework.Utils;

namespace Framework.Metadata
{
	/// <summary>
	/// Provider of user permission.
	/// </summary>
	public interface IxUserPermissionProvider
	{
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads user permissions list from database.
    /// </summary>
    /// <param name="connection">DB connection to load permissions from</param>
    /// <param name="userName">current user name</param>
    void LoadUser(CxDbConnection connection, string userName);
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads role permissions list from database.
    /// </summary>
    /// <param name="connection">DB connection to load permissions from</param>
    /// <param name="roleId">role ID</param>
    void LoadRole(CxDbConnection connection, object roleId);
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns permission object for the given metadata object and permission group.
    /// </summary>
    /// <param name="objectType">type of metadata object (entity, attribute, etc.)</param>
    /// <param name="objectId">ID of metadata object</param>
    /// <param name="permissionGroupId">ID of permission group</param>
    CxPermission GetPermission(string objectType, string objectId, string permissionGroupId);
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns permission rule object (if specified).
    /// </summary>
    /// <param name="objectType">type of metadata object (entity, attribute, etc.)</param>
    /// <param name="objectId">ID of metadata object</param>
    /// <param name="permissionGroupId">ID of permission group</param>
    CxPermissionRule GetRule(string objectType, string objectId, string permissionGroupId);
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
    void SetPermission(
      CxDbConnection connection,
      object roleId,
      CxMetadataObject metadataObject,
      string objectType,
      string objectId,
      string permissionGroupId,
      string permissionId);
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns user ID for the given user name (login).
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="userName">user name (login)</param>
    object GetUserId(CxDbConnection connection, string userName);
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns framework user ID for the given user name (login).
    /// Framework user ID means user ID stored in the Framework_Users table.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="userName">user name (login)</param>
    object GetFrameworkUserId(CxDbConnection connection, string userName);
    //-------------------------------------------------------------------------
    /// <summary>
    /// Validates user login and password.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="userName">user name (login)</param>
    /// <param name="password">password</param>
    /// <param name="validatePassword">if false, user password will be not validated</param>
    /// <returns>user info</returns>
    CxUserInfo ValidateLogin(
      CxDbConnection connection, 
      string userName, 
      string password,
      bool validatePassword);
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns user name whose permissions were loaded.
    /// </summary>
    string UserName
    { get; }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns role ID which permissions were loaded.
    /// </summary>
    object RoleId
    { get; }

    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets user's assigned role IDs.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="userId">an ID of a user</param>
    /// <returns>an array of role IDs</returns>
    object[] GetUserRoles(
      CxDbConnection connection,
      object userId);
    //-------------------------------------------------------------------------
    /// <summary>
    /// Assigns roles to user.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="userId">ID of user</param>
    /// <param name="roleIdArray">array of Role IDs</param>
    void AssignUserRoles(
      CxDbConnection connection,
      object userId,
      object[] roleIdArray);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Removes roles from user.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="userId">ID of user</param>
    /// <param name="roleIdArray">array of Role IDs</param>
    void RemoveUserRoles(
      CxDbConnection connection,
      object userId,
      object[] roleIdArray);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Marks user as active.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="userId">ID of user</param>
    void ActivateUser(CxDbConnection connection, object userId);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Marks user as inactive.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="userId">ID of user</param>
    void DeactivateUser(CxDbConnection connection, object userId);
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if application is in development mode (security is not applied).
    /// </summary>
    bool IsDevelopmentMode
    { get; }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Validates DB user name before insert or update.
    /// </summary>
    /// <param name="userName">user name to validate</param>
    void ValidateDbUserName(string userName);
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if logged user is an administrator user.
    /// </summary>
    bool IsAdministrator
    { get; }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns user info parameter value.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="userId">ID of user</param>
    /// <param name="paramName">name of parameter to get from user info</param>
    string GetUserInfo(CxDbConnection connection, object userId, string paramName);
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns table of available workspaces for the given user.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="userId">ID of user</param>
    DataTable GetAvailableWorkspaces(CxDbConnection connection, object userId);
    //-------------------------------------------------------------------------
  }
}