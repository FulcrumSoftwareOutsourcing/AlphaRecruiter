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

using Framework.Metadata;
using Framework.Utils;
using Framework.Db;

namespace Framework.Entity
{
	/// <summary>
	/// Entity representing application user.
	/// </summary>
	public class CxSecurityUserEntity : CxBaseEntity
	{
    //-------------------------------------------------------------------------
    public const int MinPasswordLength = 8;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="metadata">metadata that describes this entity</param>
    public CxSecurityUserEntity(CxEntityUsageMetadata metadata) : base(metadata)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Assigns roles to user.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="roleIdArray">array of Role IDs</param>
    public void AssignRoles(
      CxDbConnection connection,
      object[] roleIdArray)
    {
      CreateUserPermissionProvider().AssignUserRoles(connection, UserId, roleIdArray);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Removes roles from user.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="roleIdArray">array of Role IDs</param>
    public void RemoveRoles(
      CxDbConnection connection,
      object[] roleIdArray)
    {
      CreateUserPermissionProvider().RemoveUserRoles(connection, UserId, roleIdArray);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Marks user as active.
    /// </summary>
    /// <param name="connection">database connection</param>
    public void Activate(CxDbConnection connection)
    {
      CreateUserPermissionProvider().ActivateUser(connection, UserId);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Marks user as inactive.
    /// </summary>
    /// <param name="connection">database connection</param>
    public void Deactivate(CxDbConnection connection)
    {
      CreateUserPermissionProvider().DeactivateUser(connection, UserId);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Validates entity.
    /// </summary>
    override public void Validate()
    {
      base.Validate();

      Metadata.Holder.Security.ValidateDbUserName(Login);

      string displayPassword = CxUtils.ToString(this["DisplayPassword"]);
      string confirmPassword = CxUtils.ToString(this["ConfirmPassword"]);
      if ((CxUtils.NotEmpty(displayPassword) || CxUtils.NotEmpty(confirmPassword)) &&
          displayPassword != confirmPassword)
      {
        throw new ExValidationException(
          Metadata.Holder.GetErr("Password confirmation is invalid."), "ConfirmPassword");
      }
      
      if (CxUtils.NotEmpty(displayPassword))
      {
        if (CxOptions.Instance != null && CxOptions.Instance.StrongPassword && displayPassword.Length < MinPasswordLength)
          throw new ExValidationException(Metadata.Holder.GetErr("Password is too short."), "DisplayPassword");

        this["Password"] = CxCommon.ComputeMD5Hash(displayPassword);
      }
      string password = CxUtils.ToString(this["Password"]);
      if (CxUtils.IsEmpty(password))
      {
        throw new ExValidationException(
          Metadata.Holder.GetErr("Password could not be empty."), "DisplayPassword");
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Inserts entity (and all "owned" child) entities to the database.
    /// </summary>
    /// <param name="connection">connection an INSERT should work in context of</param>
    override public void Insert(CxDbConnection connection)
    {
      string userName = Login;
      base.Insert(connection);
      if (IsPkValueEmpty(this["UserId"]) && CxUtils.NotEmpty(userName))
      {
        object userId = 
          CreateUserPermissionProvider().GetUserId(connection, userName);
        if (CxUtils.NotEmpty(userId))
        {
          this["UserId"] = userId;
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates new user permission provider object instance.
    /// </summary>
    public IxUserPermissionProvider CreateUserPermissionProvider()
    {
      IxUserPermissionProvider provider = Metadata.Holder.UserPermissionProvider;
      if (provider == null)
      {
        throw new ExException(
          "Could not create user permission provider instance to process user entity operation.");
      }
      return provider;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns user ID.
    /// </summary>
    public object UserId
    { get {return this["UserId"];} }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns user name (login).
    /// </summary>
    public string Login
    { get {return CxUtils.ToString(this["Login"]);} }
    //----------------------------------------------------------------------------
  }
}