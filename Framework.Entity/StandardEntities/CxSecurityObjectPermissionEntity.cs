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
using System.Collections.Generic;
using System.Data;
using System.Text;
using Framework.Db;
using Framework.Metadata;
using Framework.Utils;

namespace Framework.Entity
{
  /// <summary>
  /// Security object permission entity.
  /// </summary>
  public class CxSecurityObjectPermissionEntity : CxBaseEntity
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="metadata">entity metadata</param>
    public CxSecurityObjectPermissionEntity(CxEntityUsageMetadata metadata) : base(metadata)
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Updates entity (and all "owned" child entities) in the database.
    /// </summary>
    /// <param name="connection">connection an UPDATE should work in context of</param>
    public override void Update(CxDbConnection connection)
    {
      UpdatePermission(connection);
      UpdateChildren(connection);
      ApplyUnchangedChildrenChildUpdates(connection);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Updates permission.
    /// </summary>
    /// <param name="connection">database connection</param>
    protected void UpdatePermission(CxDbConnection connection)
    {
      CxSecurityMetadata security = Metadata.Holder.Security;
      string objectType = CxUtils.ToString(this["SecurityObjectType"]);
      CxSecurityObject securityObject = security.GetSecurityObject(objectType);
      CxMetadataObject metadataObject = (CxMetadataObject) this["ObjectInstance"];
      object roleId = this["RoleId"];
      CxEntityMetadata entityMetadata = this["EntityMetadataInstance"] as CxEntityMetadata;
      foreach (CxPermissionGroup group in securityObject.PermissionGroups)
      {
        string permissionId = CxUtils.ToString(this["GroupId_" + group.Id]);
        if (!String.IsNullOrEmpty(permissionId))
        {
          security.SetPermission(
            connection, roleId, metadataObject, objectType, 
            CxUtils.ToString(this["ObjectId"]), group.Id, permissionId);

          if (metadataObject is CxAttributeMetadata)
          {
            security.SetEntityAttributeCustomPermission(connection, roleId, entityMetadata);
          }
          if (metadataObject is CxCommandMetadata)
          {
            security.SetEntityCommandCustomPermission(connection, roleId, entityMetadata);
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Validates entity.
    /// </summary>
    public override void Validate(CxDbConnection connection)
    {
      base.Validate(connection);

      CxSecurityMetadata security = Metadata.Holder.Security;
      string objectType = CxUtils.ToString(this["SecurityObjectType"]);
      CxSecurityObject securityObject = security.GetSecurityObject(objectType);
      CxMetadataObject metadataObject = (CxMetadataObject)this["ObjectInstance"];
      CxEntityMetadata entityMetadata = metadataObject as CxEntityMetadata;
      if (entityMetadata != null)
      {
        foreach (CxPermissionGroup group in securityObject.PermissionGroups)
        {
          string permissionId = CxUtils.ToString(this["GroupId_" + group.Id]);
          if (!String.IsNullOrEmpty(permissionId))
          {
            CxPermission permission = 
              security.GetPermissionMetadata(metadataObject, group.Id, permissionId);
            if (permission != null && !permission.IsRuleApplicableTo(entityMetadata))
            {
              string permissionName = permissionId;
              CxRowSourceMetadata rowSource = Metadata.GetAttribute("GroupId_" + group.Id).RowSource;
              if (rowSource != null)
              {
                permissionName = rowSource.GetDescriptionByValue(connection, permissionId, false);
              }
              throw new ExValidationException(
                String.Format("'{0}' permission is not applicable to this object.", permissionName));
            }
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Reads an array of entities of the given entity usage.
    /// </summary>
    /// <param name="connection">a database connection</param>
    /// <param name="where">where clause (filter condition)</param>
    /// <param name="valueProvider">a provider for parameter values</param>
    /// <param name="startRecordIndex">start index of record set to get</param>
    /// <param name="recordsAmount">amount of records to get</param>
    /// <param name="sortings">a list of sort descriptors</param>
    /// <returns>an array of entities</returns>
    public override IxEntity[] ReadEntities(
      CxDbConnection connection, 
      string where, 
      IxValueProvider valueProvider, 
      int startRecordIndex, 
      int recordsAmount, 
      CxSortDescriptorList sortings)
    {
      object roleId = valueProvider["RoleId"];
      string objectType = Metadata["SECURITY_OBJECT"];
      if (String.IsNullOrEmpty(objectType))
      {
        throw new ExException("SECURITY_OBJECT tag is not specified for the entity usage.");
      }
      DataTable table = new DataTable();
      CxSecurityMetadata security = Metadata.Holder.Security;
      switch (objectType)
      {
        case CxSecurityMetadata.TYPE_SUBSYSTEM:
          security.GetSubsystemPermissionsTable(connection, table, roleId);
          break;
        //case CxSecurityMetadata.TYPE_WIN_SECTION:
        //  security.GetWinSectionPermissionsTable(connection, table, roleId);
        //  break;
        case CxSecurityMetadata.TYPE_ENTITY:
          security.GetEntityPermissionsTable(connection, table, roleId);
          break;
        //case CxSecurityMetadata.TYPE_COMMAND:
        //  security.GetCommandPermissionsTable(connection, table, GetParentEntityMetadata(), roleId);
        //  break;
        //case CxSecurityMetadata.TYPE_ATTRIBUTE:
        //  security.GetAttributePermissionsTable(connection, table, GetParentEntityMetadata(), roleId);
        //  break;
        default:
          throw new ExException("SECURITY_OBJECT tag is invalid.");

      }
      
      List<CxBaseEntity> entityList = new List<CxBaseEntity>();
      foreach (DataRow row in table.Rows)
      {
        CxBaseEntity entity = CreateByDataRow(Metadata, row);
        entity["OBJECTINSTANCE"] = null;
        entityList.Add(entity);
      }
      return entityList.ToArray();

    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Reads an entity of the given entity usage and with the given entity PK.
    /// </summary>
    /// <param name="connection">a database connection</param>
    /// <param name="where">where clause (filter condition)</param>
    /// <param name="valueProvider">a provider for parameter values, including PK</param>
    /// <returns>the entity read</returns>
    public override IxEntity ReadEntity(
      CxDbConnection connection, string where, IxValueProvider valueProvider)
    {
      object roleId = valueProvider["RoleId"];
      string objectId = Convert.ToString(valueProvider["OBJECTID"]);
      string objectType = Metadata["SECURITY_OBJECT"];
      if (String.IsNullOrEmpty(objectType))
      {
        throw new ExException("SECURITY_OBJECT tag is not specified for the entity usage.");
      }
      DataTable table = new DataTable();
      CxSecurityMetadata security = Metadata.Holder.Security;
      switch (objectType)
      {
        case CxSecurityMetadata.TYPE_SUBSYSTEM:
          security.GetSubsystemPermissionsTable(connection, table, roleId, objectId);
          break;
        case CxSecurityMetadata.TYPE_WIN_SECTION:
          security.GetWinSectionPermissionsTable(connection, table, roleId);
          break;
        case CxSecurityMetadata.TYPE_ENTITY:
          security.GetEntityPermissionsTable(connection, table, roleId, objectId);
          break;
        //case CxSecurityMetadata.TYPE_COMMAND:
        //  security.GetCommandPermissionsTable(connection, table, GetParentEntityMetadata(), roleId);
        //  break;
        //case CxSecurityMetadata.TYPE_ATTRIBUTE:
        //  security.GetAttributePermissionsTable(connection, table, GetParentEntityMetadata(), roleId);
        //  break;
        default:
          throw new ExException("SECURITY_OBJECT tag is invalid.");

      }

      List<CxBaseEntity> entityList = new List<CxBaseEntity>();
      foreach (DataRow row in table.Rows)
      {
        CxBaseEntity entity = CreateByDataRow(Metadata, row);
        entity["OBJECTINSTANCE"] = null;
        entityList.Add(entity);
      }
      if (entityList.Count == 0)
        return null;
      else
        return entityList[0];
    }
    //-------------------------------------------------------------------------
  }
}