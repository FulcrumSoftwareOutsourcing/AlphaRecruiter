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
using System.Xml;
using Framework.Db;
using Framework.Utils;
using System.Collections.Generic;

namespace Framework.Metadata
{
  /// <summary>
	/// Security configuration metadata
	/// </summary>
	public class CxSecurityMetadata
	{
    //-------------------------------------------------------------------------
    // Types for security metadata objects.
    public const string TYPE_SUBSYSTEM     = "subsystem";
    public const string TYPE_ENTITY        = "entity";
    public const string TYPE_COMMAND       = "command";
    public const string TYPE_ATTRIBUTE     = "attribute";
    public const string TYPE_PORTAL        = "portal";
    public const string TYPE_TREE_ITEM     = "tree_item";
    public const string TYPE_TAB           = "tab";
    public const string TYPE_WEB_PART      = "web_part";
    public const string TYPE_WIN_SECTION   = "win_section";
    public const string TYPE_WIN_TREE_ITEM = "win_tree_item";
    public const string TYPE_SL_SECTION    = "sl_section";
    public const string TYPE_SL_TREE_ITEM  = "sl_tree_item";
    public const string TYPE_SL_SKIN       = "sl_skin";
    public const string TYPE_SL_DASHBOARD_ITEM  = "sl_dasboard_item";
    //-------------------------------------------------------------------------
    // IDs of permission groups
    public const string PERM_GROUP_VIEW      = "View";
    public const string PERM_GROUP_EDIT      = "Edit";
    public const string PERM_GROUP_EXECUTE   = "Execute";
    public const string PERM_GROUP_ACCESS    = "Access";
    public const string PERM_GROUP_ATTRIBUTE = "Attribute";
    //-------------------------------------------------------------------------
    public const string KEY_SECURITY_PERMISSION_GROUP_ID = "Security$PermissionGroupId";
    //-------------------------------------------------------------------------
    protected CxMetadataHolder m_Holder = null;
    protected Hashtable m_ObjectsMap = new Hashtable();
    protected ArrayList m_RulesList = new ArrayList();
    protected Hashtable m_RulesMap = new Hashtable();
    protected CxPermissionRule m_DefaultRule = null;
    protected CxPermissionRule m_CustomRule = null;
    protected CxPermissionRule m_AllowRule = null;
    protected CxImageMetadata m_DefaultRuleImage = null;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="doc">document to load data from</param>
    public CxSecurityMetadata(CxMetadataHolder holder, XmlDocument doc)
    {
      m_Holder = holder;
      Load(doc);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata collection from the XML document.
    /// </summary>
    /// <param name="doc">document to load data from</param>
    virtual protected void Load(XmlDocument doc)
    {
      if (doc.DocumentElement != null)
      {
        XmlElement objectsElement =
          (XmlElement) doc.DocumentElement.SelectSingleNode("objects");
        if (objectsElement != null)
        {
          foreach (XmlNode node in objectsElement.ChildNodes)
          {
            if (node is XmlElement)
            {
              CxSecurityObject securityObject = new CxSecurityObject(Holder, (XmlElement) node);
              securityObject.Id = node.Name;
              m_ObjectsMap.Add(securityObject.Id, securityObject);
            }
          }
        }

        XmlElement rulesElement =
          (XmlElement) doc.DocumentElement.SelectSingleNode("rules");
        if (rulesElement != null)
        {
          string defaultRuleImageId = CxXml.GetAttr(rulesElement, "default_image_id");
          if (CxUtils.NotEmpty(defaultRuleImageId))
          {
            m_DefaultRuleImage = Holder.Images[defaultRuleImageId];
          }
          XmlNodeList rules = rulesElement.SelectNodes("rule");
          if (rules == null)
            throw new ExNullReferenceException("rules");

          foreach (XmlElement element in rules)
          {
            CxPermissionRule rule = new CxPermissionRule(Holder, element);
            m_RulesList.Add(rule);
            m_RulesMap.Add(rule.Id, rule);
            if (rule.IsDefault && m_DefaultRule == null)
            {
              m_DefaultRule = rule;
            }
            if (rule.IsAllowRule && m_AllowRule == null)
            {
              m_AllowRule = rule;
            }
            if (rule.IsCustomRule && m_CustomRule == null)
            {
              m_CustomRule = rule;
            }
          }
        }
      }
      if (m_DefaultRule == null && m_RulesList.Count > 0)
      {
        m_DefaultRule = (CxPermissionRule) m_RulesList[0];
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns index of the permission rule in the list of rules.
    /// </summary>
    /// <param name="ruleId">ID of rule to get index of</param>
    public int GetIndexOfRule(string ruleId)
    {
      if (CxUtils.NotEmpty(ruleId))
      {
        CxPermissionRule rule = GetRule(ruleId);
        if (rule != null)
        {
          return m_RulesList.IndexOf(rule);
        }
      }
      return -1;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns permission rule object by rule ID.
    /// </summary>
    /// <param name="ruleId">ID of rule</param>
    public CxPermissionRule GetRule(string ruleId)
    {
      if (CxUtils.NotEmpty(ruleId))
      {
        return (CxPermissionRule) m_RulesMap[ruleId.ToUpper()];
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns security object type ID for the given metadata object.
    /// </summary>
    protected string GetObjectType(CxMetadataObject metadataObject)
    {
      if (metadataObject is CxEntityMetadata)
      {
        return TYPE_ENTITY;
      }
      else if (metadataObject is CxCommandMetadata)
      {
        return TYPE_COMMAND;
      }
      else if (metadataObject is CxAttributeMetadata)
      {
        return TYPE_ATTRIBUTE;
      }
      else if (metadataObject is CxPortalMetadata)
      {
        return TYPE_PORTAL;
      }
      else if (metadataObject is CxTreeItemMetadata)
      {
        return TYPE_TREE_ITEM;
      }
      else if (metadataObject is CxTabMetadata)
      {
        return TYPE_TAB;
      }
      else if (metadataObject is CxWebPartMetadata)
      {
        return TYPE_WEB_PART;
      }
      else if (metadataObject is CxWinSectionMetadata)
      {
        return TYPE_WIN_SECTION;
      }
      else if (metadataObject is CxWinTreeItemMetadata)
      {
        return TYPE_WIN_TREE_ITEM;
      }
      else if (metadataObject is CxSlSectionMetadata)
      {
        return TYPE_SL_SECTION;
      }
      else if (metadataObject is CxSlTreeItemMetadata)
      {
        return TYPE_SL_TREE_ITEM;
      }
      else if (metadataObject is CxSlSkinMetadata)
      {
        return TYPE_SL_SKIN;
      }
      else if (metadataObject is CxSlDashboardItemMetadata)
      {
        return TYPE_SL_DASHBOARD_ITEM;
      }
      
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns security object ID for the given metadata object.
    /// </summary>
    /// <param name="metadataObject">metadata object</param>
    /// <param name="entityUsage">parent entity usage (if applicable)</param>
    protected string GetObjectId(
      CxMetadataObject metadataObject,
      CxEntityUsageMetadata entityUsage)
    {
      if (metadataObject is CxCommandMetadata ||
          metadataObject is CxAttributeMetadata)
      {
        return GetObjectId(entityUsage, entityUsage) + "." + metadataObject.Id;
      }
      else if (metadataObject is CxEntityUsageMetadata)
      {
        return metadataObject.SecurityEntityUsage == null ?
          ((CxEntityUsageMetadata)metadataObject).EntityId : metadataObject.SecurityEntityUsage.Id;
      }
      return metadataObject.Id;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Splits compound security object ID into object ID and parent entity ID.
    /// </summary>
    /// <param name="objectType"></param>
    /// <param name="securityId"></param>
    /// <param name="objectId"></param>
    /// <param name="entityId"></param>
    public void SplitObjectId(
      string objectType, 
      string securityId, 
      out string objectId, 
      out string entityId)
    {
      objectId = securityId;
      entityId = null;
      if (!TYPE_COMMAND.Equals(objectType, StringComparison.OrdinalIgnoreCase) &&
          !TYPE_ATTRIBUTE.Equals(objectType, StringComparison.OrdinalIgnoreCase))
      {
        return;
      }
      int index = securityId.IndexOf(".");
      if (index > 0 && index < securityId.Length - 1)
      {
        entityId = securityId.Substring(0, index);
        objectId = securityId.Substring(index + 1);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns security object for the given metadata object type.
    /// </summary>
    /// <param name="objectType">metadata object type</param>
    public CxSecurityObject GetSecurityObject(string objectType)
    {
      if (CxUtils.NotEmpty(objectType))
      {
        return (CxSecurityObject) m_ObjectsMap[objectType.ToUpper()];
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity which security permissions are taken for the given entity. 
    /// </summary>
    public CxEntityMetadata GetSecurityEntity(CxEntityMetadata entity)
    {
      CxEntityMetadata securityEntity = entity;
      while (securityEntity.SecurityEntityUsage != null && 
             securityEntity.SecurityEntityUsage != securityEntity)
      {
        securityEntity = securityEntity.SecurityEntityUsage;
      }
      if (securityEntity is CxEntityUsageMetadata)
      {
        CxEntityUsageMetadata entityUsage = (CxEntityUsageMetadata) securityEntity;
        return entityUsage.SecurityEntityUsage == null ? entityUsage.Entity : entityUsage;
      }
      return securityEntity;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns permission rule.
    /// </summary>
    /// <param name="metadataObject">object to get rule for</param>
    /// <param name="entityUsage">parent entity usage</param>
    /// <param name="permissionGroupId">permission group</param>
    public CxPermissionRule GetRule(
      CxMetadataObject metadataObject,
      CxEntityUsageMetadata entityUsage,
      string permissionGroupId)
    {
      // If here is some specific security entity usage defined - we should use it in the first hand.
      if (metadataObject.SecurityEntityUsage != null &&
          metadataObject.SecurityEntityUsage != metadataObject)
      {
        return GetRule(
          metadataObject.SecurityEntityUsage, 
          metadataObject.SecurityEntityUsage, 
          permissionGroupId);
      }

      // If it is entity metadata and entity is not in security configuration,
      // always return Allow rule, otherwise object will be always denied.
      if (metadataObject is CxEntityMetadata &&
          !((CxEntityMetadata)metadataObject).IsInSecurity)
      {
        return AllowRule;
      }

      // First of all we should determine the type of the security object associated with the
      // given metadata object.
      string objectType = GetObjectType(metadataObject);
      CxSecurityObject securityObject = GetSecurityObject(objectType);
      if (securityObject == null)
      {
        return DefaultRule;
      }

      // Determining the entity group the given metadata object is related to.
      CxEntityGroup entityGroup = securityObject.GetEntityGroup(metadataObject);
      if (entityGroup != null && entityUsage != null)
      {
        CxPermissionRule rule = GetRule(entityUsage, entityUsage, entityGroup.Id);
        if (rule != null && rule.Allow != NxBoolEx.Undefined)
        {
          return rule;
        }
      }

      string objectId = GetObjectId(metadataObject, entityUsage);
      if (CxUtils.IsEmpty(objectId))
      {
        return DefaultRule;
      }

      CxPermissionGroup permissionGroup = 
        securityObject.GetPermissionGroup(permissionGroupId) ?? securityObject.DefaultPermissionGroup;

      if (permissionGroup == null)
      {
        return DefaultRule;
      }

      if (UserPermissionProvider != null)
      {
        CxPermissionRule rule = UserPermissionProvider.GetRule(objectType, objectId, permissionGroup.Id);
        if (rule != null)
        {
          return rule;
        }
        // Get subsystem permission for the entity
        if (metadataObject is CxEntityMetadata)
        {
          CxEntityMetadata entity = (CxEntityMetadata) metadataObject;
          if (CxUtils.NotEmpty(entity.GroupName))
          {
            rule = UserPermissionProvider.GetRule(TYPE_SUBSYSTEM, entity.GroupName, permissionGroup.Id);
            if (rule != null)
            {
              return rule;
            }
          }
        }
      }

      CxPermissionRule result = null;
      if (permissionGroup.DefaultPermission != null)
      {
        result = permissionGroup.DefaultPermission.Rule;
      }
      return result ?? DefaultRule;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns access right for the object - allowed or denied.
    /// </summary>
    /// <param name="metadataObject">object to get access right for</param>
    /// <param name="entityUsage">parent entity usage (for attributes and commands)</param>
    /// <param name="permissionGroupId">permission group</param>
    /// <param name="connection">database connection</param>
    /// <param name="entityValueProvider">value provider to get entity values</param>
    public bool GetRight(
      CxMetadataObject metadataObject,
      CxEntityUsageMetadata entityUsage,
      string permissionGroupId,
      CxDbConnection connection,
      IxValueProvider entityValueProvider)
    {
      CxPermissionRule rule = GetRule(metadataObject, entityUsage, permissionGroupId);
      if (rule != null)
      {
        if (connection != null &&
            entityValueProvider != null &&
            !String.IsNullOrEmpty(rule.GetWhereClause(entityUsage)))
        {
          CxEntityRuleCache cache = EntityRuleCache;
          if (cache != null)
          {
            NxBoolEx cacheValue = 
              cache.GetCachedValue(rule, entityUsage, entityValueProvider);
            if (cacheValue != NxBoolEx.Undefined)
            {
              return CxBoolEx.GetBool(cacheValue);
            }
          }
          if (permissionGroupId != null)
          {
          entityValueProvider[KEY_SECURITY_PERMISSION_GROUP_ID] = permissionGroupId.ToUpper();
          }
          bool isAllowed = entityUsage.CheckEntityInstanceCondition(
            connection, 
            rule.GetWhereClause(entityUsage), 
            entityValueProvider);
          if (cache != null)
          {
            cache.AddValueToCache(rule, entityUsage, entityValueProvider, CxBoolEx.GetBoolEx(isAllowed));
          }
          return isAllowed;
        }
        return rule.Allow == NxBoolEx.False ? false : true;
      }
      return true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns access right for the object - allowed or denied.
    /// </summary>
    /// <param name="metadataObject">object to get access right for</param>
    /// <param name="entityUsage">parent entity usage metadata</param>
    /// <param name="permissionGroupId">permission group</param>
    public bool GetRight(
      CxMetadataObject metadataObject,
      CxEntityUsageMetadata entityUsage,
      string permissionGroupId)
    {
      return GetRight(metadataObject, entityUsage, permissionGroupId, null, null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns access right for the object - allowed or denied.
    /// </summary>
    /// <param name="metadataObject">object to get access right for</param>
    /// <param name="entityUsage">parent entity usage metadata</param>
    public bool GetRight(
      CxMetadataObject metadataObject,
      CxEntityUsageMetadata entityUsage)
    {
      return GetRight(metadataObject, entityUsage, null, null, null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns access right for the object - allowed or denied.
    /// </summary>
    /// <param name="metadataObject">object to get access right for</param>
    /// <param name="permissionGroupId">permission group</param>
    public bool GetRight(
      CxMetadataObject metadataObject,
      string permissionGroupId)
    {
      return GetRight(metadataObject, null, permissionGroupId, null, null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns access right for the object - allowed or denied.
    /// </summary>
    /// <param name="metadataObject">object to get access right for</param>
    public bool GetRight(CxMetadataObject metadataObject)
    {
      return GetRight(metadataObject, null, null, null, null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns WHERE clause to add to entity list SQL SELECT.
    /// </summary>
    /// <param name="entityUsage">entity usage metadata</param>
    public string GetWhereClause(CxEntityUsageMetadata entityUsage)
    {
      CxPermissionRule rule = GetRule(entityUsage, entityUsage, null);
      return rule != null ? rule.GetWhereClause(entityUsage) : String.Empty;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity metadata object that contains security settings for
    /// the given entity metadata object.
    /// </summary>
    /// <param name="entity">entity to get security entity for</param>
    protected CxEntityMetadata GetSecurityEntityObject(CxEntityMetadata entity)
    {
      if (entity.SecurityEntityUsage != null &&
          entity.SecurityEntityUsage != entity)
      {
        return GetSecurityEntityObject(entity.SecurityEntityUsage);
      }
      return entity;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity usage metadata object that contains security settings 
    /// for the given metadata object.
    /// </summary>
    /// <param name="metadataObject">metadata object to get security entity for</param>
    public CxEntityUsageMetadata GetSecurityEntityUsage(CxMetadataObject metadataObject)
    {
      if (metadataObject.SecurityEntityUsage != null &&
          metadataObject.SecurityEntityUsage != metadataObject)
      {
        return GetSecurityEntityUsage(metadataObject.SecurityEntityUsage);
      }
      return metadataObject is CxEntityUsageMetadata ? 
        (CxEntityUsageMetadata) metadataObject : null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Initializes permission table data row corresponding to the given metadata object.
    /// </summary>
    /// <param name="objectId">id of the object</param>
    /// <param name="objectType">a type of the object</param>
    /// <param name="row">row to initialize</param>
    /// <param name="metadataObject">metadata object</param>
    /// <param name="entityUsage">entity usage of the object if any</param>
    /// <param name="provider">user permissions provider</param>
    protected void InitPermissionTableRow(
      string objectId,
      string objectType,
      DataRow row,
      CxMetadataObject metadataObject,
      CxEntityUsageMetadata entityUsage,
      IxUserPermissionProvider provider)
    {
      row["ObjectId"] = objectId;
      row["ObjectGroup"] = metadataObject.GroupName;
      row["ObjectName"] = metadataObject.Text;
      row["ObjectInstance"] = metadataObject;
      row["MetadataObjectType"] = metadataObject.GetType().Name;
      row["SecurityObjectType"] = objectType;

      CxSecurityObject securityObject = GetSecurityObject(objectType);

      IList<CxEntityMetadata> entityList = null;
      if (objectType == TYPE_SUBSYSTEM)
      {
        // Get entity count for subsystem
        entityList = GetSecurityEntityList();
        if (entityList == null)
          throw new ExNullReferenceException("entityList");

        int objectCount = 0;
        foreach (CxEntityMetadata entity in entityList)
        {
          if (CxText.Equals(entity.GroupName, objectId))
          {
            objectCount++;
          }
        }
        row["ObjectCount"] = objectCount;
      }

      foreach (CxPermissionGroup group in securityObject.PermissionGroups)
      {
        CxPermission permission = provider.GetPermission(objectType, objectId, group.Id);
        if (permission == null && (metadataObject is CxEntityMetadata))
        {
          // Get subsystem permission if entity permission is not specified
          CxEntityMetadata entity = (CxEntityMetadata) metadataObject;
          if (CxUtils.NotEmpty(entity.GroupName))
          {
            permission = provider.GetPermission(TYPE_SUBSYSTEM, entity.GroupName, group.Id);
          }
        }
        if (permission == null)
        {
          // Get default permission if object permission is not specified
          permission = group.DefaultPermission;
        }

        row["GroupId_" + group.Id] = permission.Id;
        row["GroupName_" + group.Id] = permission.Text;
        row["GroupImageUrl_" + group.Id] = permission.Rule != null ? permission.Rule.ImageUrl : "";

        if (objectType == TYPE_SUBSYSTEM)
        {
          // Get overridden entity count for subsystem
          int overrideCount = 0;
          if (entityList == null)
            throw new ExNullReferenceException("entityList");

          foreach (CxEntityMetadata entity in entityList)
          {
            if (CxText.Equals(entity.GroupName, objectId))
            {
              CxPermission entityPermission = provider.GetPermission(
                TYPE_ENTITY,
                GetObjectId(entity, null),
                group.Id);
              if (entityPermission != null && permission.Rule != entityPermission.Rule)
              {
                overrideCount++;
              }
            }
          }
          if (overrideCount > 0)
          {
            row["GroupOverrideCount_" + group.Id] = overrideCount;
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns data table with entity permissions list for the given role.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="dt">data table to fill with data</param>
    /// <param name="objectType">type of security object</param>
    /// <param name="objectList">list of CxMetadataObject</param>
    /// <param name="roleId">ID of role to get permissions for</param>
    /// <param name="entityUsage">an entity usage to get permissions for</param>
    /// <param name="entity">an entity to get permissions for</param>
    public void GetObjectPermissionsTable(
      CxDbConnection connection,
      DataTable dt,
      string objectType,
      IList<CxMetadataObject> objectList,
      object roleId,
      CxEntityUsageMetadata entityUsage,
      CxEntityMetadata entity)
    {
      GetObjectPermissionsTable(
        connection, dt, objectType, objectList, roleId, entityUsage, entity, null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns data table with entity permissions list for the given role.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="dt">data table to fill with data</param>
    /// <param name="objectType">type of security object</param>
    /// <param name="objectList">list of CxMetadataObject</param>
    /// <param name="roleId">ID of role to get permissions for</param>
    /// <param name="entityUsage">an entity usage to get permissions for</param>
    /// <param name="entity">an entity to get permissions for</param>
    /// <param name="objectIdFilter">id of the object to filter by 
    /// (if null, no filter applied)</param>
    public void GetObjectPermissionsTable(
      CxDbConnection connection,
      DataTable dt,
      string objectType,
      IList<CxMetadataObject> objectList,
      object roleId,
      CxEntityUsageMetadata entityUsage,
      CxEntityMetadata entity,
      string objectIdFilter)
    {
      IxUserPermissionProvider permissionProvider = CreateUserPermissionProvider();
      permissionProvider.LoadRole(connection, roleId);

      dt.Clear();
      dt.Rows.Clear();
      dt.Columns.Clear();

      DataColumn dc;

      dc = dt.Columns.Add("ObjectGroup", typeof(string));
      dc.Caption = "Subsystem";
      dc.ExtendedProperties["width"] = 120;

      dc = dt.Columns.Add("ObjectName", typeof(string));
      dc.Caption = "Name";
      dc.ExtendedProperties["width"] = 250;

      dc = dt.Columns.Add("ObjectInstance", typeof(object));
      dc.ExtendedProperties["hidden"] = true;

      dc = dt.Columns.Add("ObjectCount", typeof(int));
      dc.Caption = "Object Count";
      dc.ExtendedProperties["hidden"] = true;
      dc.ExtendedProperties["width"] = 70;

      CxSecurityObject securityObject = GetSecurityObject(objectType);
      foreach (CxPermissionGroup group in securityObject.PermissionGroups)
      {
        dc = dt.Columns.Add("GroupId_" + group.Id, typeof(string));
        dc.Caption = group.Text;
        dc.ExtendedProperties["hidden"] = true;

        dc = dt.Columns.Add("GroupName_" + group.Id, typeof(string));
        dc.Caption = group.Text;

        dc = dt.Columns.Add("GroupImageUrl_" + group.Id, typeof(string));
        dc.ExtendedProperties["hidden"] = true;

        dc = dt.Columns.Add("GroupOverrideCount_" + group.Id, typeof(int));
        dc.Caption = "Override Count";
        dc.ExtendedProperties["hidden"] = true;
        dc.ExtendedProperties["width"] = 80;

        dc = dt.Columns.Add("GroupIsOverridden_" + group.Id, typeof(bool));
        dc.Caption = "Overridden";
        dc.ExtendedProperties["hidden"] = true;
        dc.ExtendedProperties["width"] = 80;
      }

      dc = dt.Columns.Add("ObjectId", typeof(string));
      dc.Caption = "ID";
      dc.ExtendedProperties["width"] = 300;
      /* Commented by Gleb: make column always visible
      if (!IsDevelopmentMode)
      {
        dc.ExtendedProperties["hidden"] = true;
      }
      */

      dc = dt.Columns.Add("MetadataObjectType", typeof(string));
      dc.Caption = "Metadata Object Type";
      dc.ExtendedProperties["hidden"] = true;

      dc = dt.Columns.Add("SecurityObjectType", typeof(string));
      dc.Caption = "Metadata Object Type";
      dc.ExtendedProperties["hidden"] = true;

      dc = dt.Columns.Add("RoleId", typeof(int));
      dc.Caption = "Role ID";
      dc.ExtendedProperties["hidden"] = true;

      dc = dt.Columns.Add("EntityMetadataInstance", typeof(object));
      dc.Caption = "Entity Metadata";
      dc.ExtendedProperties["hidden"] = true;

      bool isGroupColumnVisible = false;
      foreach (CxMetadataObject metadataObject in objectList)
      {
        string objectId = GetObjectId(metadataObject, entityUsage);
        
        if (objectIdFilter != null && 
            !string.Equals(objectId, objectIdFilter, StringComparison.OrdinalIgnoreCase))
          continue;

        DataRow row = dt.NewRow();
        InitPermissionTableRow(objectId, objectType, row, metadataObject, entityUsage, permissionProvider);
        row["RoleId"] = CxInt.Parse(roleId, 0);
        if (entity != null)
        {
          row["EntityMetadataInstance"] = entity;
        }
        dt.Rows.Add(row);
        if (CxUtils.NotEmpty(metadataObject.GroupName))
        {
          isGroupColumnVisible = true;
        }
      }
      if (!isGroupColumnVisible)
      {
        dt.Columns["ObjectGroup"].ExtendedProperties["hidden"] = true;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns data table with subsystem permissions list for the given role.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="dt">data table to fill with data</param>
    /// <param name="roleId">ID of role to get permissions for</param>
    public void GetSubsystemPermissionsTable(
      CxDbConnection connection,
      DataTable dt,
      object roleId)
    {
      GetSubsystemPermissionsTable(connection, dt, roleId, null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns data table with subsystem permissions list for the given role.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="dt">data table to fill with data</param>
    /// <param name="roleId">ID of role to get permissions for</param>
    /// <param name="subsystemIdFilter">subsystem id to filter by (if null, no filter is applied)</param>
    public void GetSubsystemPermissionsTable(
      CxDbConnection connection,
      DataTable dt,
      object roleId,
      string subsystemIdFilter)
    {
      // Get list of subsystems (entity group names)
      IList<string> subsystemNameList = Holder.GetEntityGroupNameList();
      IList<CxMetadataObject> subsystemList = new List<CxMetadataObject>();
      foreach (string name in subsystemNameList)
      {
        CxMetadataObject subsystem = new CxMetadataObject(Holder);
        subsystem.Id = name;
        subsystem.Text = name;
        subsystemList.Add(subsystem);
      }

      // Get permissions table.
      GetObjectPermissionsTable(connection, dt, TYPE_SUBSYSTEM, subsystemList, roleId, null, null, subsystemIdFilter);

      // Customize columns
      dt.Columns["ObjectId"].ExtendedProperties["hidden"] = true; 
      dt.Columns["ObjectName"].ExtendedProperties["width"] = 200; 
      dt.Columns["ObjectCount"].Caption = "Entity Count"; 
      dt.Columns["ObjectCount"].ExtendedProperties["hidden"] = null; 
      // Show override count columns
      CxSecurityObject securityObject = GetSecurityObject(TYPE_SUBSYSTEM);
      foreach (CxPermissionGroup group in securityObject.PermissionGroups)
      {
        dt.Columns["GroupOverrideCount_" + group.Id].ExtendedProperties["hidden"] = null; 
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of all security entities.
    /// </summary>
    public IList<CxEntityMetadata> GetSecurityEntityList()
    {
      List<CxEntityMetadata> result = new List<CxEntityMetadata>();
      Hashtable securityEntities = new Hashtable();
      foreach (CxEntityMetadata entity in Holder.Entities.Items)
      {
        CxEntityMetadata securityEntity = GetSecurityEntityObject(entity);
        if (securityEntity.IsInSecurity &&
            !securityEntities.ContainsKey(securityEntity))
        {
          securityEntities[securityEntity] = true;
          result.Add(securityEntity);
        }
      }
      foreach (CxEntityUsageMetadata entityUsage in Holder.EntityUsages.Items)
      {
        if (entityUsage.SecurityEntityUsage != null)
        {
          CxEntityMetadata securityEntity = GetSecurityEntityObject(entityUsage);
          if (securityEntity.IsInSecurity &&
              !securityEntities.ContainsKey(securityEntity))
          {
            securityEntities[securityEntity] = true;
            result.Add(securityEntity);
          }
        }
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns data table with entity permissions list for the given role.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="dt">data table to fill with data</param>
    /// <param name="roleId">ID of role to get permissions for</param>
    public void GetEntityPermissionsTable(
      CxDbConnection connection,
      DataTable dt,
      object roleId)
    {
      GetEntityPermissionsTable(
        connection, dt, roleId, null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns data table with entity permissions list for the given role.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="dt">data table to fill with data</param>
    /// <param name="roleId">ID of role to get permissions for</param>
    /// <param name="entityIdFilter">
    /// if defined, causes to return just one row, corresponding to the entity id given
    /// </param>
    public void GetEntityPermissionsTable(
      CxDbConnection connection,
      DataTable dt,
      object roleId,
      string entityIdFilter)
    {
      List<CxMetadataObject> metadataObjects = new List<CxMetadataObject>();
      
      foreach (CxEntityMetadata entity in GetSecurityEntityList())
        metadataObjects.Add(entity);

      GetObjectPermissionsTable(connection, dt, TYPE_ENTITY, metadataObjects, roleId, null, null, entityIdFilter);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns data table with portal permissions list for the given role.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="dt">data table to fill with data</param>
    /// <param name="roleId">ID of role to get permissions for</param>
    public void GetPortalPermissionsTable(
      CxDbConnection connection,
      DataTable dt,
      object roleId)
    {
      List<CxMetadataObject> objectList = new List<CxMetadataObject>();
      foreach (CxPortalMetadata portal in Holder.Portals.AllItems)
      {
        objectList.Add(portal);
      }
      GetObjectPermissionsTable(connection, dt, TYPE_PORTAL, objectList, roleId, null, null);
      dt.Columns["ObjectId"].ExtendedProperties["hidden"] = true; 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns data table with windows section permissions list for the given role.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="dt">data table to fill with data</param>
    /// <param name="roleId">ID of role to get permissions for</param>
    public void GetWinSectionPermissionsTable(
      CxDbConnection connection,
      DataTable dt,
      object roleId)
    {
      List<CxMetadataObject> objectList = new List<CxMetadataObject>();
      foreach (CxWinSectionMetadata section in Holder.WinSections.AllItemsList)
      {
        objectList.Add(section);
      }
      GetObjectPermissionsTable(connection, dt, TYPE_WIN_SECTION, objectList, roleId, null, null);
      dt.Columns["ObjectId"].ExtendedProperties["hidden"] = true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns data table with command permissions list for the given role.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="dt">data table to fill with data</param>
    /// <param name="entity">entity to get commands for</param>
    /// <param name="roleId">ID of role to get permissions for</param>
    public void GetCommandPermissionsTable(
      CxDbConnection connection,
      DataTable dt,
      CxEntityMetadata entity,
      object roleId)
    {
      CxEntityUsageMetadata entityUsage = 
        entity is CxEntityUsageMetadata ? (CxEntityUsageMetadata)entity : entity.DefaultEntityUsage;
      List<CxMetadataObject> objectList = new List<CxMetadataObject>();
      if (entityUsage != null)
      {
        foreach (CxCommandMetadata command in entityUsage.Commands)
        {
          objectList.Add(command);
        }
      }
      GetObjectPermissionsTable(connection, dt, TYPE_COMMAND, objectList, roleId, entityUsage, entity);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns data table with attribute permissions list for the given role.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="dt">data table to fill with data</param>
    /// <param name="entity">entity to get attributes for</param>
    /// <param name="roleId">ID of role to get permissions for</param>
    public void GetAttributePermissionsTable(
      CxDbConnection connection,
      DataTable dt,
      CxEntityMetadata entity,
      object roleId)
    {
      CxEntityUsageMetadata entityUsage = 
        entity is CxEntityUsageMetadata ? (CxEntityUsageMetadata)entity : entity.DefaultEntityUsage;
      List<CxMetadataObject> objectList = new List<CxMetadataObject>();
      if (entityUsage != null)
      {
        foreach (CxAttributeMetadata attribute in entityUsage.Attributes)
        {
          if (attribute.Visible || attribute.Editable)
          {
            objectList.Add(attribute);
          }
        }
      }
      GetObjectPermissionsTable(connection, dt, TYPE_ATTRIBUTE, objectList, roleId, entityUsage, entity);
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
    /// <param name="permissionId">ID of permission option</param>
    public void SetPermission(
      CxDbConnection connection,
      object roleId,
      CxMetadataObject metadataObject,
      string objectType,
      string objectId,
      string permissionGroupId,
      string permissionId)
    {
      IxUserPermissionProvider provider = CreateUserPermissionProvider();
      provider.SetPermission(
        connection, roleId, metadataObject, objectType, objectId, permissionGroupId, permissionId);

      // Auto set permissions
      CxSecurityObject securityObject = GetSecurityObject(objectType);
      if (securityObject != null)
      {
        CxPermissionGroup group = securityObject.GetPermissionGroup(permissionGroupId);
        if (group != null)
        {
          CxPermission permission = group.GetPermission(permissionId);
          if (permission != null && permission.IsAutoSetPermissionSpecified)
          {
            SetPermission(
              connection, 
              roleId, 
              metadataObject, 
              objectType, 
              objectId,
              permission.AutoSetPermissionGroupId,
              permission.AutoSetPermissionId);
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets custom permission option for the given entity and permission group.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="roleId">ID of role to set permission for</param>
    /// <param name="entity">entity to set custom permission for</param>
    /// <param name="permissionGroupId">ID of permission group</param>
    protected void SetEntityCustomPermission(
      CxDbConnection connection,
      object roleId,
      CxEntityMetadata entity,
      string permissionGroupId)
    {
      CxSecurityObject securityObject = GetSecurityObject(TYPE_ENTITY);
      if (securityObject != null)
      {
        CxPermissionGroup group = securityObject.GetPermissionGroup(permissionGroupId);
        if (group != null)
        {
          CxPermission permission = group.CustomRulePermission;
          if (permission != null)
          {
            string objectId = GetObjectId(entity, null);
            SetPermission(
              connection, roleId, entity, TYPE_ENTITY, objectId, permissionGroupId, permission.Id);
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets custom permission option for the commands of given entity.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="roleId">ID of role to set permission for</param>
    /// <param name="entity">entity to set custom permission for</param>
    public void SetEntityCommandCustomPermission(
      CxDbConnection connection,
      object roleId,
      CxEntityMetadata entity)
    {
      CxSecurityObject securityObject = GetSecurityObject(TYPE_COMMAND);
      if (securityObject != null)
      {
        ArrayList permissionGroupCodes = new ArrayList();
        foreach (CxEntityGroup group in securityObject.EntityGroups)
        {
          if (permissionGroupCodes.IndexOf(group.Id) < 0)
          {
            permissionGroupCodes.Add(group.Id);
          }
        }
        foreach (string permissionGroupId in permissionGroupCodes)
        {
          SetEntityCustomPermission(connection, roleId, entity, permissionGroupId);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets custom permission option for the attributes of given entity.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="roleId">ID of role to set permission for</param>
    /// <param name="entity">entity to set custom permission for</param>
    public void SetEntityAttributeCustomPermission(
      CxDbConnection connection,
      object roleId,
      CxEntityMetadata entity)
    {
      CxSecurityObject securityObject = GetSecurityObject(TYPE_ATTRIBUTE);
      if (securityObject != null)
      {
        ArrayList permissionGroupCodes = new ArrayList();
        foreach (CxEntityGroup group in securityObject.EntityGroups)
        {
          if (permissionGroupCodes.IndexOf(group.Id) < 0)
          {
            permissionGroupCodes.Add(group.Id);
          }
        }
        foreach (string permissionGroupId in permissionGroupCodes)
        {
          SetEntityCustomPermission(connection, roleId, entity, permissionGroupId);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns permission metadata object for the given object type.
    /// </summary>
    /// <param name="objectType">type of metadata object</param>
    /// <param name="permissionGroupId">permission group ID</param>
    /// <param name="permissionId">permission option ID</param>
    /// <returns>permission metadata</returns>
    public CxPermission GetPermissionMetadata(
      string objectType,
      string permissionGroupId,
      string permissionId)
    {
      CxSecurityObject securityObject = GetSecurityObject(objectType);
      if (securityObject != null)
      {
        CxPermissionGroup group = securityObject.GetPermissionGroup(permissionGroupId);
        if (group != null)
        {
          return group.GetPermission(permissionId);
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns permission metadata object for the given metadata object.
    /// </summary>
    /// <param name="metadataObject">metadata object</param>
    /// <param name="permissionGroupId">permission group ID</param>
    /// <param name="permissionId">permission option ID</param>
    /// <returns>permission metadata</returns>
    public CxPermission GetPermissionMetadata(
      CxMetadataObject metadataObject,
      string permissionGroupId,
      string permissionId)
    {
      return GetPermissionMetadata(
        GetObjectType(metadataObject), permissionGroupId, permissionId);
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
    public CxUserInfo ValidateLogin(
      CxDbConnection connection, 
      string userName, 
      string password,
      bool validatePassword)
    {
      IxUserPermissionProvider provider = CreateUserPermissionProvider();
      return provider.ValidateLogin(connection, userName, password, validatePassword);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates new instance of user permission provider.
    /// </summary>
    public IxUserPermissionProvider CreateUserPermissionProvider()
    { 
      if (Holder == null)
      {
        throw new ExException(
          "Could not create user permission provider. " +
          "Metadata holder object is empty.");
      }
      IxUserPermissionProvider provider = Holder.CreateUserPermissionProvider();
      if (provider == null)
      {
        throw new ExException(
          "Could not create user permission provider. " +
          "Probably OnCreateUserPermissionProvider event handler " +
          "is not specified for metadata holder object.");
      }
      return provider;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns allow permission object for the given metadata object and permission group.
    /// </summary>
    /// <param name="objectType">type of metadata object (entity, attribute, etc.)</param>
    /// <param name="objectId">ID of metadata object</param>
    /// <param name="permissionGroupId">ID of permission group</param>
    public CxPermission GetAllowPermission(
      string objectType,
      string objectId,
      string permissionGroupId)
    {
      if (CxUtils.NotEmpty(objectType) &&
        CxUtils.NotEmpty(permissionGroupId))
      {
        CxSecurityObject securityObject = Holder.Security.GetSecurityObject(objectType);
        if (securityObject != null)
        {
          CxPermissionGroup group = securityObject.GetPermissionGroup(permissionGroupId);
          if (group != null)
          {
            return group.AllowPermission;
          }
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Validates DB user name before insert or update.
    /// </summary>
    /// <param name="userName">user name to validate</param>
    public void ValidateDbUserName(string userName)
    {
      if (UserPermissionProvider != null)
      {
        UserPermissionProvider.ValidateDbUserName(userName);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of security entity objects which are referenced by
    /// the given security entity object.
    /// </summary>
    protected IList<CxEntityMetadata> GetReferencedSecurityEntities(CxEntityMetadata entity)
    {
      UniqueList<CxEntityMetadata> referencedEntities = new UniqueList<CxEntityMetadata>();

      if (entity is CxEntityUsageMetadata)
      {
        foreach (CxEntityUsageMetadata entityUsage in Holder.EntityUsages.Items)
        {
          if (entityUsage.SecurityEntityUsage == entity)
          {
            referencedEntities.AddRange(entityUsage.GetReferencedEntities());
          }
        }
      }
      else
      {
        foreach (CxEntityUsageMetadata entityUsage in entity.GetInheritedEntityUsages())
        {
          referencedEntities.AddRange(entityUsage.GetReferencedEntities());
        }
      }

      UniqueList<CxEntityMetadata> result = new UniqueList<CxEntityMetadata>();
      foreach (CxEntityMetadata referencedEntity in referencedEntities)
      {
        result.Add(GetSecurityEntity(referencedEntity));
      }
      return result;
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
    /// Returns collection of user permissions loaded from the database.
    /// </summary>
    public IxUserPermissionProvider UserPermissionProvider
    { 
      get 
      {
        return Holder != null ? Holder.UserPermissionProvider : null;
      } 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns default permission rule.
    /// </summary>
    public CxPermissionRule DefaultRule
    { get {return m_DefaultRule;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns custom permission rule (with allow=undefined).
    /// </summary>
    public CxPermissionRule CustomRule
    { get {return m_CustomRule;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns 'allow' permission rule.
    /// </summary>
    public CxPermissionRule AllowRule
    { get {return m_AllowRule;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity rule cache.
    /// </summary>
    public CxEntityRuleCache EntityRuleCache
    {
      get
      {
        return Holder != null ? Holder.EntityRuleCache : null;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if application is in development mode (security is not applied).
    /// </summary>
    public bool IsDevelopmentMode
    { 
      get 
      {
        return UserPermissionProvider != null && 
               UserPermissionProvider.IsDevelopmentMode;
      } 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns metadata of the default image to display with rule in a grid view.
    /// </summary>
    public CxImageMetadata DefaultRuleImage
    { get { return m_DefaultRuleImage; } }
    //-------------------------------------------------------------------------
  }
}