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
using System.Xml;
using System.Text;
using System.Data;
using Framework.Db;
using Framework.Utils;

namespace Framework.Metadata
{
  /// <summary>
  /// Class to hold information about application entity.
  /// </summary>
  public class CxEntityMetadata : CxMetadataObject
	{
    //-------------------------------------------------------------------------
    public const string HAS_CHILDREN_ATTR_ID = "F_HAS_CHILDREN";
    public const string ROW_CLASS_ATTR_ID    = "F_ROW_CLASS";
    //-------------------------------------------------------------------------
    // Entity operations
    public const string OP_VIEW   = "V";
    public const string OP_INSERT = "I";
    public const string OP_UPDATE = "U";
    public const string OP_DELETE = "D";
    //-------------------------------------------------------------------------
    // Prefix to add to old value parameter
    public const string PARAM_OLD_PREFIX = "OLD$";
    //-------------------------------------------------------------------------
    protected Type m_EntityClass = null; // Entity class
    protected Type m_EditClass = null; // Edit class
    protected Dictionary<string, CxAttributeMetadata> m_AttributeMap = new Dictionary<string, CxAttributeMetadata>(); // Dictionary for attributes
    protected List<CxAttributeMetadata> m_AttributeList = new List<CxAttributeMetadata>(); // Ordered list of attributes
    protected Hashtable m_CommandMap = new Hashtable(); // Dictionary for commands
    protected List<CxCommandMetadata> m_CommandList = new List<CxCommandMetadata>(); // Ordered list of commands
    protected CxAttributeMetadata m_UserIdAttribute = null; // Entity user ID attribute.
    protected CxAttributeMetadata[] m_PrimaryKeyAttributes = null; // List of primary key attributes
    protected Dictionary<int, CxAttributeMetadata[]> 
      m_AlternativeKeyAttributes = new Dictionary<int, CxAttributeMetadata[]>(); // List of alternative key attributes
    protected string m_Operations = null; // Opeartions available on entity
    protected bool m_Accessible = true; // true if this entity is accessible or false otherwise
    protected CxEntityUsageMetadata m_DefaultEntityUsage = null;
    // List of parent entity metadata that can be obtained from this entity.
    protected List<CxParentEntityMetadata> m_ParentEntities = new List<CxParentEntityMetadata>();
    protected CxAttributeMetadata m_NameAttribute = null;
    protected IList<CxCommandGroupMetadata> m_CommandGroups = new List<CxCommandGroupMetadata>();
    protected int m_DbObjectCount = 0;
    // Cached sort attributes
    protected CxAttributeMetadata[] m_SortAttributes = null;
    protected bool? m_IsInMemoryNotificationRequired = null;
    protected bool m_IsCustomizable = false;
    protected UniqueList<string> m_NewAttributeNames = null;
    protected Dictionary<NxAttributeContext, CxAttributeOrder>
      m_AttributeOrders = new Dictionary<NxAttributeContext, CxAttributeOrder>();
    protected string m_CustomizeCaption = null;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public CxEntityMetadata(CxMetadataHolder holder) : base(holder)
    {
      CreateAttributeOrders();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">metadata holder</param>
    /// <param name="element">XML element that holds metadata</param>
    public CxEntityMetadata(CxMetadataHolder holder, XmlElement element) : base(holder, element)
    {
      CreateAttributeOrders();
      AddNodeToProperties(element, "sql_select_run_before");
      AddNodeToProperties(element, "sql_select");
      AddNodeToProperties(element, "sql_select_single_row");
      AddNodeToProperties(element, "sql_insert");
      AddNodeToProperties(element, "sql_update");
      AddNodeToProperties(element, "sql_delete");
      AddNodeToProperties(element, "primary_key_clause");
      LoadParentEntityList((XmlElement) element.SelectSingleNode("parent_entities"));
      CreateCommandsFromOperations();
      // Set group name
      if (CxUtils.IsEmpty(GroupName) && element.ParentNode is XmlElement)
      {
        string groupName = CxXml.GetAttr((XmlElement) (element.ParentNode), "group_name");
        if (CxUtils.NotEmpty(groupName))
        {
          this["group_name"] = groupName;
        }
      }
      AddNodeToProperties(element, "hint_find");
      AddNodeToProperties(element, "hint_grid");
      AddNodeToProperties(element, "hint_new");
      AddNodeToProperties(element, "hint_edit");
      AddNodeToProperties(element, "hint_view");
      m_IsCustomizable = CxXml.GetAttr(element, "customizable").ToLower() == "true";
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Defines an attribute with the given id on the level of 
    /// the current entity metadata.
    /// </summary>
    /// <param name="attributeId">id of the attribute to be added</param>
    /// <returns>defined attribute metadata</returns>
    public virtual CxAttributeMetadata DefineEntityAttribute(string attributeId)
    {
      return DefineEntityAttribute(attributeId, null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Defines an attribute with the given id and initial values on the level of
    /// the current entity metadata.
    /// </summary>
    /// <param name="attributeId">id of the attribute to be added</param>
    /// <param name="initialValues">initial values of the attribute to be added</param>
    /// <returns>defined attribute metadata</returns>
    public virtual CxAttributeMetadata DefineEntityAttribute(
      string attributeId, IDictionary<string, string> initialValues)
    {
      return DefineEntityAttribute(attributeId, initialValues, "attribute");
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Defines an attribute with the given id and initial values on the level of 
    /// the current entity metadata using the given attribute tag.
    /// </summary>
    /// <param name="attributeId">id of the attribute to be added</param>
    /// <param name="initialValues">initial values of the attribute to be defined</param>
    /// <param name="attributeTag">name of the tag used</param>
    /// <returns>defined attribute metadata</returns>
    public virtual CxAttributeMetadata DefineEntityAttribute(
      string attributeId, IDictionary<string, string> initialValues, string attributeTag)
    {
      XmlElement element = CxAttributeUsagesMetadata.CreateXmlElementBase(attributeTag, attributeId);
      if (initialValues != null)
      {
        foreach (KeyValuePair<string, string> pair in initialValues)
        {
          XmlAttribute attribute = element.OwnerDocument.CreateAttribute(pair.Key);
          attribute.Value = pair.Value;
          element.Attributes.Append(attribute);
        }
      }
      
      CxAttributeUsageMetadata newAttributeUsage = new CxAttributeUsageMetadata(element, this);

      // Here is a workaround. We don't have AttributeUsages when they're actually being constructed
      // so we have somehow to add the attribute to the entity anyway.
      if (Holder.AttributeUsages != null)
        Holder.AttributeUsages.Add(this, newAttributeUsage);
      else
        AddAttribute(newAttributeUsage);

      return newAttributeUsage;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates attribute order objects.
    /// </summary>
    protected void CreateAttributeOrders()
    {
      m_AttributeOrders[NxAttributeContext.GridVisible] =
        new CxAttributeOrder(this, NxAttributeContext.GridVisible);
      m_AttributeOrders[NxAttributeContext.Queryable] =
        new CxAttributeOrder(this, NxAttributeContext.Queryable);
      m_AttributeOrders[NxAttributeContext.Edit] =
        new CxAttributeOrder(this, NxAttributeContext.Edit);
      m_AttributeOrders[NxAttributeContext.Filter] =
        new CxAttributeOrder(this, NxAttributeContext.Filter);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads parent entity list metadata.
    /// </summary>
    /// <param name="element">XML element to load data from</param>
    protected void LoadParentEntityList(XmlElement element)
    {
      if (element != null)
      {
        XmlNodeList nodes = element.SelectNodes("parent_entity");
        if (nodes == null)
          throw new ExNullReferenceException("nodes");
        foreach (XmlElement e in nodes)
        {
          CxParentEntityMetadata metadata = new CxParentEntityMetadata(Holder, e);
          m_ParentEntities.Add(metadata);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata object override.
    /// </summary>
    /// <param name="element">XML element to load overridden properties from</param>
    override public void LoadOverride(XmlElement element)
    {
      base.LoadOverride(element);
      LoadParentEntityList((XmlElement) element.SelectSingleNode("parent_entities"));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if at least one attribute with grid cell merging enabled.
    /// </summary>
    public bool GetContainsGridCellMergingEnabledAttributes()
    {
      CxAttributeOrder order = GetAttributeOrder(NxAttributeContext.GridVisible);
      if (order != null)
      {
        foreach (CxAttributeMetadata attributeMetadata in order.OrderAttributes)
        {
          if (attributeMetadata.GridCellMerging)
            return true;
          else
          {
            CxAttributeMetadata valueAttributeMetadata = GetValueAttribute(attributeMetadata);
            if (valueAttributeMetadata != null && valueAttributeMetadata.GridCellMerging)
              return true;
          }
        }
      }
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Caption of entity in plural case.
    /// </summary>
    public string PluralCaption 
    {
      get { return this["plural_caption"]; }
      set { this["plural_caption"] = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Caption of entity in single case.
    /// </summary>
    public string SingleCaption 
    {
      get { return this["single_caption"]; }
      set { this["single_caption"] = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Operations applicable to this entity.
    /// </summary>
    public string Operations
    {
      get 
      { 
        if (m_Operations == null)
        {
          m_Operations = this["operations"].ToUpper();
        }
        return m_Operations;
      }
      set { m_Operations = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// true if entity is editable in the grid.
    /// </summary>
    public bool EditableInGrid
    {
      get { return (this["editable_in_grid"].ToLower() != "false"); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// true if entity grid has AutoWidth option.
    /// </summary>
    public bool AutoWidthInGrid
    {
      get { return (this["auto_width_in_grid"].ToLower() == "true"); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// true if entity grid is "colorable".
    /// </summary>
    public bool ColorsInGrid
    {
      get { return (this["colors_in_grid"].ToLower() == "true"); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// true if entity may disapper after update.
    /// </summary>
    public bool MayDisappearAfterUpdate
    {
      get { return (this["may_disappear_after_update"].ToLower() == "true"); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// ID of entity class.
    /// </summary>
    public string EntityClassId
    {
      get { return this["entity_class_id"]; }
      set { this["entity_class_id"] = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Entity class.
    /// </summary>
    public Type EntityClass
    {
      get
      {
        if (m_EntityClass == null)
        {
          m_EntityClass = Holder.Classes[EntityClassId].Class;
        }
        return m_EntityClass;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// ID of frame class.
    /// </summary>
    public string FrameClassId
    {
      get { return this["frame_class_id"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Frame class.
    /// </summary>
    public CxClassMetadata FrameClass
    {
      get
      {
        return CxUtils.NotEmpty(FrameClassId) ? Holder.Classes[FrameClassId] : null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// ID of Windows edit controller class.
    /// </summary>
    public string WinEditControllerClassId
    {
      get { return this["win_edit_controller_class_id"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Windows edit controller class.
    /// </summary>
    public CxClassMetadata WinEditControllerClass
    {
      get
      {
        return CxUtils.NotEmpty(WinEditControllerClassId) ? Holder.Classes[WinEditControllerClassId] : null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Silverlight specific edit-frame id.
    /// </summary>
    public string SlEditFrameId
    {
      get { return CxUtils.Nvl(this["sl_edit_frame_id"]).ToUpper(); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// ID of edit class.
    /// </summary>
    public string EditClassId
    {
      get { return this["edit_class_id"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Edit class.
    /// </summary>
    public Type EditClass
    {
      get
      {
        if (m_EditClass == null)
        {
          m_EditClass = Holder.Classes[EditClassId].Class;
        }
        return m_EditClass;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Database object an entity works with.
    /// </summary>
    public string DbObject
    {
      get { return this["db_object"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns count of available DB objects.
    /// </summary>
    public int DbObjectCount
    {
      get
      {
        if (m_DbObjectCount == 0)
        {
          for (int i = 0; i < 10; i++)
          {
            if (CxUtils.NotEmpty(GetDbObject(i)))
            {
              m_DbObjectCount++;
            }
            else
            {
              break;
            }
          }
        }
        return m_DbObjectCount;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns DB object for the given index (for multiple DB objects).
    /// </summary>
    /// <param name="index">db object index</param>
    /// <returns>db object or empty string if not exists</returns>
    public string GetDbObject(int index)
    {
      if (index == 0)
      {
        return DbObject;
      }
      else if (index > 0)
      {
        return this["db_object_" + index];
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns array of attributes to store in the DB object with the specified index.
    /// </summary>
    /// <param name="index">db object index</param>
    /// <returns>array of attribute metadata</returns>
    public CxAttributeMetadata[] GetDbObjectAttributes(int index)
    {
      List<CxAttributeMetadata> attributes = new List<CxAttributeMetadata>();
      foreach (CxAttributeMetadata attribute in Attributes)
      {
        if (attribute.DbObjectIndex == index)
        {
          attributes.Add(attribute);
        }
      }
      return attributes.ToArray();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// SQL SELECT statement to read entities from the database.
    /// </summary>
    public string SqlSelect
    {
      get 
      { 
        string sql = this["sql_select"];
        if (CxUtils.IsEmpty(sql) && !string.IsNullOrEmpty(DbObject)) sql = "SELECT t.* FROM " + DbObject + " t";
        return sql;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// SQL statement to be performed before the SELECT statement.
    /// The statement can be parametrized with the application parameters,
    /// but its result set is not considered.
    /// </summary>
    public string SqlSelectRunBefore
    {
      get { return this["sql_select_run_before"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// SQL select clause to select single row from the database (w/o WHERE condition).
    /// </summary>
    public string SqlSelectSingleRow
    {
      get
      {
        if (CxUtils.NotEmpty(this["sql_select_single_row"]))
        {
          return this["sql_select_single_row"];
        }
        return SqlSelect;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// SQL SELECT statement to read entities from the database.
    /// </summary>
    public string SqlInsert
    {
      get { return this["sql_insert"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// SQL UPDATE statement to read entities from the database.
    /// </summary>
    public string SqlUpdate
    {
      get { return this["sql_update"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// SQL DELETE statement to read entities from the database.
    /// </summary>
    public string SqlDelete
    {
      get { return this["sql_delete"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// ID of the entity image.
    /// </summary>
    public string ImageId
    {
      get { return this["image_id"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Entity image.
    /// </summary>
    public CxImageMetadata Image
    {
      get { return CxUtils.NotEmpty(ImageId) ? Holder.Images[ImageId] : null; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns attribute with the given name.
    /// </summary>
    /// <param name="name">attribute name</param>
    /// <returns>attribute with the given name</returns>
    virtual public CxAttributeMetadata GetAttribute(string name)
    {
      string nameInUpperCase = name.ToUpper();
      if (CxUtils.NotEmpty(name) && m_AttributeMap.ContainsKey(nameInUpperCase))
      {
        return m_AttributeMap[nameInUpperCase];
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Ordered list of attributes.
    /// </summary>
    virtual public IList<CxAttributeMetadata> Attributes 
    {
      get { return m_AttributeList; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns command with the given name.
    /// </summary>
    /// <param name="commandId">command name</param>
    /// <returns>command with the given name</returns>
    virtual public CxCommandMetadata GetCommand(string commandId)
    {
      return (CxCommandMetadata) m_CommandMap[commandId.ToUpper()];
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Ordered list of commands.
    /// </summary>
    virtual public IList<CxCommandMetadata> Commands 
    {
      get { return m_CommandList; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds attribute to the list.
    /// </summary>
    /// <param name="attribute">attribute to add</param>
    virtual public void AddAttribute(CxAttributeMetadata attribute)
    {
      m_AttributeMap.Add(attribute.Id, attribute);
      m_AttributeList.Add(attribute);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds command to the list.
    /// </summary>
    /// <param name="command">command to add</param>
    virtual public void AddCommand(CxCommandMetadata command)
    {
      CxCommandMetadata parentCommand = (CxCommandMetadata) m_CommandMap[command.Id];
      if (parentCommand != null)
      {
        command.CopyPropertiesFrom(parentCommand);
        m_CommandMap[command.Id] = command;
        int index = m_CommandList.IndexOf(parentCommand);
        m_CommandList[index] = command;
      }
      else
      {
        m_CommandMap.Add(command.Id, command);
        m_CommandList.Add(command);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Removes command from the list of commands.
    /// </summary>
    /// <param name="command">command to remove</param>
    virtual internal protected void RemoveCommand(CxCommandMetadata command)
    {
      m_CommandMap.Remove(command.Id);
      m_CommandList.Remove(command);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Composes WHERE clause for primary key columns.
    /// </summary>
    public string ComposePKCondition()
    {
      return ComposePKCondition(0);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Composes WHERE clause for primary key columns.
    /// </summary>
    /// <param name="dbObjectIndex">Db object index to generate the condition for</param>
    public virtual string ComposePKCondition(int dbObjectIndex)
    {
      if (CxUtils.NotEmpty(PrimaryKeyClause))
      {
        return PrimaryKeyClause;
      }
      StringBuilder sb = new StringBuilder();
      foreach (CxAttributeMetadata attribute in GetPrimaryKeyAttributes(dbObjectIndex))
      {
        if (sb.Length > 0) sb.Append("   \r\nAND ");
        sb.Append(attribute.Id + " = :" + /*PARAM_OLD_PREFIX +*/ attribute.Id);
      }
      return sb.ToString();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the list of primary key attributes.
    /// </summary>
    /// <returns>array of PK attributes</returns>
    public CxAttributeMetadata[] GetPrimaryKeyAttributes()
    {
      if (m_PrimaryKeyAttributes == null)
      {
        List<CxAttributeMetadata> list = new List<CxAttributeMetadata>();
        foreach (CxAttributeMetadata attribute in Attributes)
        {
          if (attribute.PrimaryKey)
          {
            list.Add(attribute);
          }
        }
        m_PrimaryKeyAttributes = list.ToArray();
      }

      return m_PrimaryKeyAttributes;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the list of primary key attributes by the given db object index.
    /// </summary>
    /// <param name="dbObjectIndex">the index of the db object</param>
    /// <returns>array of PK attributes</returns>
    public virtual CxAttributeMetadata[] GetPrimaryKeyAttributes(int dbObjectIndex)
    {
      return GetPrimaryKeyAttributes();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Composes WHERE clause for alternative key columns.
    /// </summary>
    public string ComposeAKCondition()
    {
      StringBuilder sb = new StringBuilder();
      foreach (CxAttributeMetadata attribute in AlternativeKeyAttributes)
      {
        if (sb.Length > 0) sb.Append("   \r\nAND ");
        sb.Append(attribute.Id + " = :" + attribute.Id);
      }
      return sb.ToString();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Entity user ID attribute.
    /// </summary>
    public CxAttributeMetadata UserIdAttribute
    {
      get 
      {
        if (m_UserIdAttribute == null)
        {
          foreach (CxAttributeMetadata attribute in Attributes)
          {
            if (attribute.UserId)
            {
              m_UserIdAttribute = attribute;
              break;
            }
          }
        }
        return m_UserIdAttribute;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// List of primary key attributes.
    /// </summary>
    public CxAttributeMetadata[] PrimaryKeyAttributes
    {
      get 
      {
        return GetPrimaryKeyAttributes(0);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// List of primary key attribute ids.
    /// </summary>
    public IList<string> PrimaryKeyIds
    {
      get 
      {
        List<string> ids = new List<string>();
        foreach (CxAttributeMetadata attributeMetadata in PrimaryKeyAttributes)
        {
          ids.Add(attributeMetadata.Id);
        }
        return ids.AsReadOnly();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns alternative key attributes by the given alternative key index.
    /// </summary>
    /// <param name="index">alternative key index</param>
    /// <returns>array of attributes</returns>
    public CxAttributeMetadata[] GetAlternativeKeyAttributes(int index)
    {
      CxAttributeMetadata[] attributes;
      if (!m_AlternativeKeyAttributes.TryGetValue(index, out attributes))
      {
        List<CxAttributeMetadata> list = new List<CxAttributeMetadata>();
        foreach (CxAttributeMetadata attribute in Attributes)
        {
          if (attribute.AlternativeKey && attribute.AlternativeKeyIndex == index)
          {
            list.Add(attribute);
          }
        }
        attributes = list.ToArray();
        m_AlternativeKeyAttributes[index] = attributes;
      }
      return attributes;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// List of main alternative key attributes.
    /// </summary>
    public CxAttributeMetadata[] AlternativeKeyAttributes
    {
      get
      {
        return GetAlternativeKeyAttributes(0);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if primary key is defined for the entity.
    /// </summary>
    public bool IsPrimaryKeyDefined
    {
      get
      {
        CxAttributeMetadata[] pkArray = PrimaryKeyAttributes;
        return pkArray != null && pkArray.Length > 0;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if alternative key is defined for the entity.
    /// </summary>
    public bool IsAlternativeKeyDefined
    {
      get
      {
        CxAttributeMetadata[] akArray = AlternativeKeyAttributes;
        return akArray != null && akArray.Length > 0;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Raises an exception if primary key is not defined.
    /// </summary>
    public void CheckForPrimaryKey()
    {
      if (!IsPrimaryKeyDefined)
      {
        throw new ExMetadataException(
          "Primary key attribute is not defined for entity '" + Id + "'.");
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the first attribute marked as primary key.
    /// </summary>
    public CxAttributeMetadata PrimaryKeyAttribute
    {
      get
      {
        CxAttributeMetadata[] pkArray = PrimaryKeyAttributes;
        return pkArray != null && pkArray.Length > 0 ? pkArray[0] : null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Entity display name attribute.
    /// </summary>
    public CxAttributeMetadata NameAttribute
    {
      get 
      {
        if (m_NameAttribute == null)
        {
          CxAttributeMetadata defNameAttr = null;
          foreach (CxAttributeMetadata attribute in Attributes)
          {
            if (attribute.Id == "NAME")
            {
              defNameAttr = attribute;
            }
            if (attribute.IsDisplayName)
            {
              m_NameAttribute = attribute;
              break;
            }
          }
          if (m_NameAttribute == null)
          {
            m_NameAttribute = defNameAttr;
          }
        }
        return m_NameAttribute;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Analyses Operations field to get available entity operations.
    /// Maps operation codes to commands and creates corresponding commands.
    /// </summary>
    virtual protected void CreateCommandsFromOperations()
    {
      string operations = Operations;
      if (Holder != null &&
          Holder.Commands != null &&
          CxUtils.NotEmpty(operations))
      {
        for (int i = 0; i < operations.Length; i++)
        {
          string code = operations.Substring(i, 1);
          CxCommandMetadata operationCommand = Holder.Commands.FindByOperationCode(code);
          if (operationCommand != null)
          {
            CxCommandMetadata newCommand = new CxCommandMetadata(operationCommand, this); 
            AddCommand(newCommand);
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns default entity usage metadata object for the given entity.
    /// </summary>
    virtual public CxEntityUsageMetadata DefaultEntityUsage
    { 
      get 
      {
        if (m_DefaultEntityUsage == null)
        {
          CxEntityUsageMetadata firstItem = null;
          foreach (CxEntityUsageMetadata entityUsage in Holder.EntityUsages.Items)
          {
            if (entityUsage.EntityId == Id)
            {
              if (firstItem == null)
              {
                firstItem = entityUsage;
              }
              if (entityUsage.IsDefault)
              {
                m_DefaultEntityUsage = entityUsage;
                break;
              }
            }
          }
          if (m_DefaultEntityUsage == null)
          {
            m_DefaultEntityUsage = firstItem;
          }
        }
        return m_DefaultEntityUsage;
      } 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// List of parent entities metadata that can be obtained from this entity.
    /// Returns list of CxParentEntityMetadata objects.
    /// </summary>
    public IList<CxParentEntityMetadata> ParentEntities
    { get {return m_ParentEntities;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Internal method to get path to load parent entity.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="parentEntityUsage"></param>
    /// <param name="checkedEntityMetadataMap"></param>
    /// <returns></returns>
    protected bool GetParentEntityPath(
      IList<CxParentEntityMetadata> path, 
      CxEntityUsageMetadata parentEntityUsage,
      Hashtable checkedEntityMetadataMap)
    {
      foreach (CxParentEntityMetadata parentMetadata in ParentEntities)
      {
        if (CxUtils.NotEmpty(parentMetadata.EntityUsageId) &&
            parentMetadata.EntityUsageId == parentEntityUsage.Id)
        {
          path.Add(parentMetadata);
          return true;
        }
      }
      foreach (CxParentEntityMetadata parentMetadata in ParentEntities)
      {
        if (CxUtils.IsEmpty(parentMetadata.EntityUsageId) &&
            parentMetadata.Id == parentEntityUsage.EntityId)
        {
          path.Add(parentMetadata);
          return true;
        }
      }
      foreach (CxParentEntityMetadata parentMetadata in ParentEntities)
      {
        CxEntityMetadata entity = parentMetadata.Entity;
        if (!checkedEntityMetadataMap.ContainsKey(entity))
        {
          checkedEntityMetadataMap[entity] = true;
          bool result = 
            entity.GetParentEntityPath(path, parentEntityUsage, checkedEntityMetadataMap);
          if (result)
          {
            path.Insert(0, parentMetadata);
            return true;
          }
        }
      }
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns path of CxParentEntityMetadata objects to read parent entity
    /// instance with the specified entity usage from the database.
    /// </summary>
    /// <param name="parentEntityUsage">entity usage to get from DB</param>
    /// <returns>list of CxParentEntityMetadata objects</returns>
    public IList<CxParentEntityMetadata> GetParentEntityPath(CxEntityUsageMetadata parentEntityUsage)
    {
      List<CxParentEntityMetadata> path = new List<CxParentEntityMetadata>();
      Hashtable checkedEntityMetadataMap = new Hashtable();
      GetParentEntityPath(path, parentEntityUsage, checkedEntityMetadataMap);
      return path;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the name of WEB ASCX control that should be used as a web part
    /// content when web part content type is Find.
    /// </summary>
    public string WebFindControl
    { get {return this["web_find_control"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the name of WEB ASCX control that should be used as a web part
    /// content when web part content type is New, Edit or View.
    /// </summary>
    public string WebEditControl
    { get {return this["web_edit_control"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// WHERE condition to get root level for the self-referencing hierachical entity.
    /// </summary>
    public string RootCondition
    { 
      get 
      {
        if (IsSelfReferencing)
        {
          return SelfReferenceAttrId + " IS NULL";
        }
        return "";
      } 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// WHERE condition to get next level for the self-referencing hierachical entity.
    /// </summary>
    public string LevelCondition
    { 
      get 
      {
        if (IsSelfReferencing)
        {
          return SelfReferenceAttrId + " = :" + PrimaryKeyAttribute.Id;
        }
        return "";
      } 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// ID of attribute containing reference to this table primary key field.
    /// For self-referencing hierarchical entity.
    /// </summary>
    public string SelfReferenceAttrId
    { get {return this["self_reference_attr_id"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Attribute containing reference to this table primary key field.
    /// For self-referencing hierarchical entity.
    /// </summary>
    public CxAttributeMetadata SelfReferenceAttribute
    { get {return GetAttribute(SelfReferenceAttrId);} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if entity is a self-referencing hierarchical entity.
    /// </summary>
    public bool IsSelfReferencing
    { get {return SelfReferenceAttribute != null && PrimaryKeyAttribute != null;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates is filtering enabled (if false, grid is displayed without filter).
    /// </summary>
    public bool IsFilterEnabled
    { get {return this["filter_enabled"].ToLower() != "false";} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns page size for web grid.
    /// If page size is set to 0 or -1, paging is disabled.
    /// </summary>
    public int WebPageSize
    { get { return CxInt.Parse(this["web_page_size"], 20); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity display hint.
    /// </summary>
    public string HintFind
    { get {return this["hint_find"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity display hint.
    /// </summary>
    public string HintGrid
    { get {return this["hint_grid"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity display hint.
    /// </summary>
    public string HintNew
    { get {return this["hint_new"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity display hint.
    /// </summary>
    public string HintEdit
    { get {return this["hint_edit"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity display hint.
    /// </summary>
    public string HintView
    { get {return this["hint_view"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ORDER BY clause composed from attributes with 'sorting' option.
    /// </summary>
    public string OrderByClause
    {
      get
      {
        string orderBy = "";
        foreach (CxAttributeMetadata attr in SortAttributes)
        {
          orderBy += 
            (CxUtils.NotEmpty(orderBy) ? ", " : "") +
            attr.Id + 
            (attr.Sorting == NxSortingDirection.Desc ? " DESC" : " ASC");
        }
        return orderBy;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns attribute containing text description for the lookup attribute.
    /// </summary>
    /// <param name="valueAttribute">lookup value attribute</param>
    public CxAttributeMetadata GetTextAttribute(CxAttributeMetadata valueAttribute)
    {
      if (valueAttribute != null &&
          valueAttribute.RowSource != null &&
          !valueAttribute.RowSource.HardCoded &&
          CxUtils.NotEmpty(valueAttribute.TextAttributeId))
      {
        CxAttributeMetadata cachedTextAttr = valueAttribute.GetCachedTextAttribute(this);
        if (cachedTextAttr == null)
        {
          cachedTextAttr = GetAttribute(valueAttribute.TextAttributeId) ?? valueAttribute;
          valueAttribute.SetCachedTextAttribute(this, cachedTextAttr);
        }
        return cachedTextAttr != valueAttribute ? cachedTextAttr : null;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns attribute containing text description for the value attribute.
    /// </summary>
    /// <param name="valueAttribute">value attribute</param>
    public CxAttributeMetadata GetTextDefinedAttribute(CxAttributeMetadata valueAttribute)
    {
      if (valueAttribute != null)
      {
        CxAttributeMetadata cachedTextAttr = valueAttribute.GetCachedTextDefinedAttribute(this);
        if (cachedTextAttr == null && CxUtils.NotEmpty(valueAttribute.TextAttributeId))
        {
          cachedTextAttr = GetAttribute(valueAttribute.TextAttributeId);
        }
        if (cachedTextAttr == null)
        {
          foreach (CxAttributeMetadata attribute in Attributes)
          {
            if (CxText.Equals(valueAttribute.Id, attribute.ValueAttributeId) && attribute != valueAttribute)
            {
              cachedTextAttr = attribute;
              break;
            }
          }
        }
        if (cachedTextAttr == null)
          cachedTextAttr = valueAttribute;
        valueAttribute.SetCachedTextDefinedAttribute(this, cachedTextAttr);
        return cachedTextAttr != valueAttribute ? cachedTextAttr : null;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns attribute containing lookup value for the text description attribute.
    /// </summary>
    /// <param name="textAttribute">lookup text attribute</param>
    public CxAttributeMetadata GetValueAttribute(CxAttributeMetadata textAttribute)
    {
      if (textAttribute != null)
      {
        CxAttributeMetadata cachedValueAttr = textAttribute.GetCachedValueAttribute(this);
        if (cachedValueAttr == null)
        {
          foreach (CxAttributeMetadata attribute in Attributes)
          {
            if (attribute.RowSource != null &&
                !attribute.RowSource.HardCoded &&
                CxText.Equals(textAttribute.Id, attribute.TextAttributeId) &&
                attribute != textAttribute)
            {
              cachedValueAttr = attribute;
              break;
            }
          }
          if (cachedValueAttr == null)
          {
            cachedValueAttr = textAttribute;
          }
          textAttribute.SetCachedValueAttribute(this, cachedValueAttr);
        }
        return cachedValueAttr != textAttribute ? cachedValueAttr : null;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    public IList<CxAttributeMetadata> GetAttributesWithoutText()
    {
      List<CxAttributeMetadata> result = new List<CxAttributeMetadata>();
      List<string> textAttributes = new List<string>();
      foreach (CxAttributeMetadata attributeMetadata in Attributes)
      {
        string textAttributeId = attributeMetadata.TextAttributeId.ToUpper();
        string valueAttributeId = attributeMetadata.ValueAttributeId.ToUpper();
        if (!string.IsNullOrEmpty(valueAttributeId))
          textAttributes.Add(attributeMetadata.Id);

        if (!string.IsNullOrEmpty(textAttributeId))
          textAttributes.Add(textAttributeId);
      }
      
      foreach (CxAttributeMetadata attributeMetadata in Attributes)
      {
        if (!textAttributes.Contains(attributeMetadata.Id))
          result.Add(attributeMetadata);
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns attribute containing value for the text description attribute.
    /// </summary>
    /// <param name="textAttribute">text attribute</param>
    public CxAttributeMetadata GetValueDefinedAttribute(CxAttributeMetadata textAttribute)
    {
      if (textAttribute != null)
      {
        CxAttributeMetadata cachedValueAttr = textAttribute.GetCachedValueDefinedAttribute(this);
        if (cachedValueAttr == null)
        {
          if (!string.IsNullOrEmpty(textAttribute.ValueAttributeId))
            cachedValueAttr = GetAttribute(textAttribute.ValueAttributeId);
          if (cachedValueAttr == null)
          {
            foreach (CxAttributeMetadata attribute in Attributes)
            {
              if (CxText.Equals(textAttribute.Id, attribute.TextAttributeId) && attribute != textAttribute)
              {
                cachedValueAttr = attribute;
                break;
              }
            }
          }
          if (cachedValueAttr == null)
          {
            cachedValueAttr = textAttribute;
          }
          textAttribute.SetCachedValueDefinedAttribute(this, cachedValueAttr);
        }
        return cachedValueAttr != textAttribute ? cachedValueAttr : null;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of attributes that are dependent from the given attribute.
    /// </summary>
    /// <param name="attribute">attribute to get dependencies for</param>
    /// <returns>list of CxAttributeMetadata objects</returns>
    public IList<CxAttributeMetadata> GetDependentAttributes(CxAttributeMetadata attribute)
    {
      List<CxAttributeMetadata> list = new List<CxAttributeMetadata>();
      if (attribute != null)
      {
        foreach (CxAttributeMetadata attr in Attributes)
        {
          if (attr.Id != attribute.Id)
          {
            IList<string> dependentNames = attr.DependencyParamNames;
            foreach (string name in dependentNames)
            {
              CxAttributeMetadata dependentAttr = GetAttribute(name);
              if (dependentAttr == attribute)
              {
                list.Add(attr);
              }
            }
          }
        }
        CxAttributeMetadata textAttr = GetTextAttribute(attribute);
        if (textAttr != null && !list.Contains(textAttr))
        {
          list.Add(textAttr);
        }
      }
      return list;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of attributes which state (visibility or readonly) 
    /// is dependent from the given attribute.
    /// </summary>
    /// <param name="attribute">attribute to get dependencies for</param>
    /// <returns>list of CxAttributeMetadata objects</returns>
    public IList<CxAttributeMetadata> GetDependentStateAttributes(CxAttributeMetadata attribute)
    {
      List<CxAttributeMetadata> list = new List<CxAttributeMetadata>();
      if (attribute != null)
      {
        foreach (CxAttributeMetadata attr in Attributes)
        {
          if (attr.Id != attribute.Id)
          {
            IList<string> dependentNames = attr.DependencyStateParamNames;
            foreach (string name in dependentNames)
            {
              CxAttributeMetadata dependentAttr = GetAttribute(name);
              if (dependentAttr == attribute)
              {
                list.Add(attr);
              }
            }
          }
        }
      }
      return list;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of attributes that are dependent from the given attribute.
    /// </summary>
    /// <param name="attribute">attribute to get dependencies for</param>
    /// <returns>list of CxAttributeMetadata objects</returns>
    public IList<CxAttributeMetadata> GetDependentMandatoryAttributes(CxAttributeMetadata attribute)
    {
      List<CxAttributeMetadata> list = new List<CxAttributeMetadata>();
      if (attribute != null)
      {
        foreach (CxAttributeMetadata attr in Attributes)
        {
          if (attr.Id != attribute.Id)
          {
            IList<string> dependentNames = attr.DependencyMandatoryParamNames;
            foreach (string name in dependentNames)
            {
              CxAttributeMetadata dependentAttr = GetAttribute(name);
              if (dependentAttr == attribute)
              {
                list.Add(attr);
              }
            }
          }
        }
      }
      return list;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ID of command that should be executed immediately 
    /// after new entity instance created.
    /// </summary>
    public string PostCreateCommandId
    { get {return this["post_create_command_id"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ID of command that should be executed immediately 
    /// after entity instance is updated.
    /// </summary>
    public string PostUpdateCommandId
    { get {return this["post_update_command_id"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Checks WHERE condition for the entity instance.
    /// </summary>
    /// <param name="connection">Db connection</param>
    /// <param name="condition">condition to check</param>
    /// <param name="valueProvider">value provider to get param values from</param>
    /// <returns>true if condition is performed</returns>
    virtual public bool CheckEntityInstanceCondition(
      CxDbConnection connection,
      string condition,
      IxValueProvider valueProvider)
    {
      string sql = SqlSelectSingleRow;
      if (CxUtils.IsEmpty(sql))
      {
        throw new ExMetadataException("SQL SELECT statement not defined for " + Id + " entity usage");
      }
      sql = CxDbUtils.AddToWhere(sql, ComposePKCondition());
      sql = CxDbUtils.AddToWhere(sql, condition);
      DataTable dt = new DataTable();
      connection.GetQueryResult(dt, sql, valueProvider);
      return dt.Rows.Count > 0;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// User-friendly metadata object caption.
    /// </summary>
    override public string Text
    { get {return SingleCaption;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if entity metadata should be visible in security permissions list.
    /// </summary>
    public bool IsInSecurity
    { 
      get 
      {
        return this["in_security"].ToLower() != "false" || SecurityEntityUsage != null;
      } 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Encodes values of entity instance alternative key fields as string.
    /// </summary>
    /// <param name="entityValueProvider">provider to get entity field values</param>
    /// <returns>encoded alternative key values string</returns>
    public string EncodeAlternativeKeyValuesAsString(IxValueProvider entityValueProvider)
    {
      if (entityValueProvider != null)
      {
        ArrayList valueList = new ArrayList();
        foreach (CxAttributeMetadata attribute in AlternativeKeyAttributes)
        {
          string pkValue = CxUtils.ToString(entityValueProvider[attribute.Id]);
          valueList.Add(pkValue);
        }
        
        // Just to make it different from any Primary key record-set.
        valueList.Add("alt_key");

        string[] valueArray = new string[valueList.Count];
        valueList.CopyTo(valueArray);
        return String.Join("\t", valueArray);
      }
      return "";
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Encodes values of entity instance primary key fields as string.
    /// </summary>
    /// <param name="entityValueProvider">provider to get entity field values</param>
    /// <returns>encoded primary key values string</returns>
    public string EncodePrimaryKeyValuesAsString(IxValueProvider entityValueProvider)
    {
      if (entityValueProvider != null)
      {
        ArrayList valueList = new ArrayList();
        foreach (CxAttributeMetadata attribute in PrimaryKeyAttributes)
        {
          string pkValue = CxUtils.ToString(entityValueProvider[attribute.Id]);
          valueList.Add(pkValue);
        }
        string[] valueArray = new string[valueList.Count];
        valueList.CopyTo(valueArray);
        return String.Join("\t", valueArray);
      }
      return "";
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Encodes values of entity instance primary key fields as string.
    /// </summary>
    public string EncodePrimaryKeyValuesAsString(object[] pkValues)
    {
      ArrayList valueList = new ArrayList();
      foreach (object o in pkValues)
      {
        string pkValue = CxUtils.ToString(o);
        valueList.Add(pkValue);
      }
      string[] valueArray = new string[valueList.Count];
      valueList.CopyTo(valueArray);
      return String.Join("\t", valueArray);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Decodes values of entity instance primary key fields from string.
    /// </summary>
    /// <param name="encodedPkValues">encoded primary key values</param>
    /// <returns>primary key values array</returns>
    public object[] DecodePrimaryKeyValuesFromString(string encodedPkValues)
    {
      if (CxUtils.NotEmpty(encodedPkValues))
      {
        string[] stringArray = encodedPkValues.Split('\t');
        object[] objectArray = new object[stringArray.Length];
        Array.Copy(stringArray, objectArray, stringArray.Length);
        return objectArray;
      }
      return new object[0];
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// For file/image library entity only.
    /// Returns ID of entity usage that is used as a category table.
    /// </summary>
    public string FileLibraryCategoryEntityUsageId
    { get {return this["file_lib_category_entity_usage_id"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// For file/image library entity only.
    /// Returns entity usage that is used as a category table.
    /// </summary>
    public CxEntityUsageMetadata FileLibraryCategoryEntityUsage
    {
      get
      {
        return CxUtils.NotEmpty(FileLibraryCategoryEntityUsageId) ?
          Holder.EntityUsages[FileLibraryCategoryEntityUsageId] : null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// For file/image library entity only.
    /// Returns ID of attribute that is a reference to image library category entity.
    /// </summary>
    public string FileLibraryCategoryReferenceAttributeId
    { get {return this["file_lib_category_reference_attribute_id"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// For file/image library category entity only.
    /// Returns ID of attribute that contains category code.
    /// </summary>
    public string FileLibraryCategoryCodeAttributeId
    { get {return this["file_lib_category_code_attribute_id"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Additional CSS class for grid rows.
    /// </summary>
    public string GridRowCssClass
    { get {return this["grid_row_css_class"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// If true, grid row height is variable, not fixed.
    /// </summary>
    public bool GridRowVariableHeight
    { get {return this["grid_row_variable_height"].ToLower() == "true";} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// If true, grid row variable height property is specified.
    /// </summary>
    public bool IsGridRowVariableHeightSpecified
    { get { return CxUtils.NotEmpty(this["grid_row_variable_height"]); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// If true, word-wrap is switched on for all grid cells.
    /// </summary>
    public bool GridWordWrap
    { get {return this["grid_word_wrap"].ToLower() == "true";} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of attributes with Sorting option specified.
    /// </summary>
    public CxAttributeMetadata[] SortAttributes
    {
      get
      {
        if (m_SortAttributes == null)
        {
          ArrayList attrList = new ArrayList();
          ArrayList orderList = new ArrayList();
          for (int i = 0; i < Attributes.Count; i++)
          {
            CxAttributeMetadata attr = Attributes[i];
            if (attr.Sorting != NxSortingDirection.None)
            {
              attrList.Add(attr);
              orderList.Add(attr.SortOrder != 0 ? attr.SortOrder * 200 : i);
            }
          }
          CxAttributeMetadata[] attrArray = new CxAttributeMetadata[attrList.Count];
          attrList.CopyTo(attrArray);
          int[] orderArray = new int[orderList.Count];
          orderList.CopyTo(orderArray);
          Array.Sort(orderArray, attrArray);
          m_SortAttributes = attrArray;
        }
        return m_SortAttributes;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// SQL WHERE condition to find record by primary key.
    /// </summary>
    public string PrimaryKeyClause
    {
      get { return this["primary_key_clause"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// If true, filter condition will be auto generated by the filter elements.
    /// </summary>
    public bool IsFilterConditionAutoGenerated
    {
      get {return this["auto_generate_filter_condition"].ToLower() != "false";} 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if record count should be limited during fetching entity records.
    /// </summary>
    public bool IsRecordCountLimited
    {
      get
      {
        return (this["record_count_limit"].ToLower() == "true" && Holder.DefaultRecordCountLimit > 0) ||
                CxInt.Parse(this["record_count_limit"], -1) > 0;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Record count limit during fetching entity records.
    /// </summary>
    public int RecordCountLimit
    {
      get
      {
        if (this["record_count_limit"].ToLower() == "true")
        {
          return Holder.DefaultRecordCountLimit;
        }
        else 
        {
          return CxInt.Parse(this["record_count_limit"], -1);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if entity data should be cached.
    /// </summary>
    public bool IsCached
    { get {return this["cached"].ToLower() == "true";} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if current entity usage is workspace dependent.
    /// </summary>
    protected bool IsWorkspaceDependent
    {
      get
      {
        CxAttributeMetadata attr = GetAttribute("WorkspaceId");
        return attr != null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if entity should be filtered by the current workspace.
    /// </summary>
    protected bool IsFilteredByCurrentWorkspace
    {
      get
      {
        return this["current_workspace_filter"].ToLower() != "false";
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if entity should be filtered by the range of workspaces
    /// available for the current user.
    /// </summary>
    protected bool IsFilteredByAvailableWorkspaces
    {
      get
      {
        return this["available_workspace_filter"].ToLower() != "false";
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// If true, INSERT SQL statement will be executed instead of 
    /// UPDATE statement when entity is updated.
    /// </summary>
    public bool IsInsertOnUpdate
    {
      get
      {
        return this["insert_on_update"].ToLower() == "true";
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// If true, primary key of the entity is also updated on entity update.
    /// </summary>
    public bool IsPrimaryKeyUpdated
    {
      get
      {
        return this["primary_key_update"].ToLower() != "false";
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ID of the windows form metadata used to create, edit or view entity.
    /// </summary>
    public string WinEditFormId
    { get { return this["win_edit_form_id"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns windows form metadata used to create, edit or view entity.
    /// </summary>
    public CxWinFormMetadata WinForm
    { get { return CxUtils.NotEmpty(WinEditFormId) ? Holder.WinForms[WinEditFormId] : null; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// If true, grid layouts corresponding to this entity can be displayed in the navigation tree.
    /// </summary>
    public bool IsGridLayoutDisplayedInTree
    { get { return this["display_grid_layout_in_tree"].ToLower() == "true"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// If true, entity or entity usage should be read-only.
    /// </summary>
    public bool ReadOnly
    { get { return this["read_only"].ToLower() == "true"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns expression that determines read-only state condition of all the entity attributes.
    /// </summary>
    public string ReadOnlyAttributesCondition
    {
      get { return this["read_only_attributes_condition"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns expression that determines read-only state condition of all the entity attributes.
    /// Is checked just if all the previous checks returned "read-only false".
    /// </summary>
    public string ReadOnlyAttributesConditionPostCheck
    {
      get { return this["read_only_attributes_condition_post_check"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// If true, all attributes of the entity or entity usage should be read-only.
    /// </summary>
    public bool ReadOnlyAttributes
    { get { return this["read_only_attributes"].ToLower() == "true"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns caption to display in the menu for commands specific for the 
    /// current entity (Windows).
    /// </summary>
    public string CommandCategoryCaption
    { 
      get 
      { 
        return CxUtils.Nvl(this["command_category_caption"], SingleCaption); 
      } 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns object type code for localization.
    /// </summary>
    override public string LocalizationObjectTypeCode
    {
      get
      {
        return "Metadata.Entity";
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of command groups.
    /// </summary>
    public IList<CxCommandGroupMetadata> CommandGroups
    { get { return m_CommandGroups;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of entity usages inherited from this entity.
    /// </summary>
    virtual public IList<CxEntityUsageMetadata> GetInheritedEntityUsages()
    {
      List<CxEntityUsageMetadata> list = new List<CxEntityUsageMetadata>();
      foreach (CxEntityUsageMetadata entityUsage in Holder.EntityUsages.Items)
      {
        if (entityUsage.Entity == this)
        {
          list.Add(entityUsage);
        }
      }
      return list;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of entity usages which are referenced by attribute properties.
    /// </summary>
    /// <returns>list of CxEntityMetadata objects or null</returns>
    override public IList<CxEntityMetadata> GetReferencedEntities()
    {
      UniqueList<CxEntityMetadata> result = new UniqueList<CxEntityMetadata>();
      foreach (CxEntityUsageMetadata entityUsage in GetInheritedEntityUsages())
      {
        result.Add(entityUsage);
        result.AddRange(entityUsage.GetReferencedEntities());
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if attribute with the given ID is defined in the metadata.
    /// </summary>
    public bool IsAttributeDefined(string attributeId)
    {
      if (CxUtils.NotEmpty(attributeId))
      {
        return m_AttributeMap.ContainsKey(attributeId.ToUpper());
      }
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if entity should be notified about in-memory changes of the
    /// current entity, parent or child entity.
    /// </summary>
    public bool IsInMemoryNotificationRequired
    {
      get
      {
        if (!m_IsInMemoryNotificationRequired.HasValue)
        {
          m_IsInMemoryNotificationRequired = false;
          foreach (CxAttributeMetadata attribute in Attributes)
          {
            if (attribute.OnlyOneSelected || attribute.IncrementOnCreate)
            {
              m_IsInMemoryNotificationRequired = true;
              break;
            }
          }
        }
        return m_IsInMemoryNotificationRequired.Value;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ID of the database connection where entity is stored.
    /// </summary>
    public string ConnectionId
    { get { return this["connection_id"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if entity is customizable by user.
    /// </summary>
    public bool Customizable
    { get { return m_IsCustomizable; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns all attributes for a grid.
    /// </summary>
    public IList<CxAttributeMetadata> GridOrderedAttributes
    {
      get
      {
        return GetAttributeOrder(NxAttributeContext.GridVisible).OrderPlusAllAttributes;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns all queryable attributes.
    /// </summary>
    public IList<CxAttributeMetadata> QueryableAttributes
    {
      get
      {
        return GetAttributeOrder(NxAttributeContext.Queryable).OrderPlusNewAttributes;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns all editable attributes (that should be present on edit form).
    /// </summary>
    public IList<CxAttributeMetadata> EditableAttributes
    {
      get
      {
        return GetAttributeOrder(NxAttributeContext.Edit).OrderPlusNewAttributes;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns all editable attributes (that should be present on edit form).
    /// </summary>
    public IList<CxAttributeMetadata> FilterableAttributes
    {
      get
      {
        return GetAttributeOrder(NxAttributeContext.Filter).OrderPlusNewAttributes;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if attribute is visible in grid.
    /// </summary>
    public bool GetIsAttributeVisibleInGrid(CxAttributeMetadata attr)
    {
      if (attr != null)
      {
        CxAttributeOrder order = GetAttributeOrder(NxAttributeContext.GridVisible);
        IList<string> orderIds = order.OrderIds;
        if (!CxList.IsEmpty2(orderIds))
        {
          if (order.IsCustom && NewAttributeNames.Contains(attr.Id) && order.XmlOrderIds.Contains(attr.Id))
          {
            return true;
          }
          return orderIds.Contains(attr.Id);
        }
        return attr.Visible;
      }
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if attribute is queryable.
    /// </summary>
    public bool GetIsAttributeQueryable(CxAttributeMetadata attr)
    {
      if (attr != null)
      {
        CxAttributeOrder order = GetAttributeOrder(NxAttributeContext.Queryable);
        IList<string> orderIds = order.OrderIds;
        if (!CxList.IsEmpty2(orderIds))
        {
          if (order.IsCustom && NewAttributeNames.Contains(attr.Id))
          {
            return true;
          }
          return orderIds.Contains(attr.Id);
        }
        return attr.Visible;
      }
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if attribute is editable (visible on the edit form).
    /// </summary>
    public bool GetIsAttributeEditable(CxAttributeMetadata attr)
    {
      if (attr != null)
      {
        CxAttributeOrder order = GetAttributeOrder(NxAttributeContext.Edit);
        IList<string> orderIds = order.OrderIds;
        if (!CxList.IsEmpty2(orderIds))
        {
          if (order.IsCustom && NewAttributeNames.Contains(attr.Id))
          {
            return true;
          }
          return orderIds.Contains(attr.Id);
        }
        return attr.Editable;
      }
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if attribute is filterable (visible on the filter form).
    /// </summary>
    public bool GetIsAttributeFilterable(CxAttributeMetadata attr)
    {
      if (attr != null)
      {
        CxAttributeOrder order = GetAttributeOrder(NxAttributeContext.Filter);
        IList<string> orderIds = order.OrderIds;
        if (!CxList.IsEmpty2(orderIds))
        {
          if (order.IsCustom && NewAttributeNames.Contains(attr.Id))
          {
            return true;
          }
          return orderIds.Contains(attr.Id);
        }
        return attr.Filterable;
      }
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns attribute names for new added attributes that are not
    /// included into the grid visible order, edit order and filter order.
    /// </summary>
    protected internal UniqueList<string> NewAttributeNames
    {
      get
      {
        if (m_NewAttributeNames == null)
        {
          m_NewAttributeNames = new UniqueList<string>(StringComparer.OrdinalIgnoreCase);
          if (CxUtils.NotEmpty(KnownAttributesString))
          {
            IList<string> list = CxText.DecomposeWithWhiteSpaceAndComma(KnownAttributesString);
            UniqueList<string> oldAttrList = new UniqueList<string>(StringComparer.OrdinalIgnoreCase);
            oldAttrList.AddRange(CxList.ToList<string>(list));
            foreach (CxAttributeMetadata attr in Attributes)
            {
              if (!oldAttrList.Contains(attr.Id))
              {
                m_NewAttributeNames.Add(attr.Id);
              }
            }
          }
        }
        return m_NewAttributeNames;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The string containing the overall list of known (by DB, metadata customization) attributes.
    /// For internal use only.
    /// </summary>
    public string KnownAttributesString
    {
      get { return this["known_attributes"]; }
      set { this["known_attributes"] = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns non-customized visible in grid attributes.
    /// </summary>
    public IList<CxAttributeMetadata> NonCustomGridOrderedAttributes
    {
      get
      {
        return GetAttributesFromIds(GetAttributeOrder(NxAttributeContext.GridVisible).XmlOrderIds);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns non-customized visible in grid attributes.
    /// </summary>
    public IList<CxAttributeMetadata> NonCustomQueryableAttributes
    {
      get
      {
        return GetAttributesFromIds(GetAttributeOrder(NxAttributeContext.Queryable).XmlOrderIds);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns non-customized editable attributes.
    /// </summary>
    public IList<CxAttributeMetadata> NonCustomEditableAttributes
    {
      get
      {
        return GetAttributesFromIds(GetAttributeOrder(NxAttributeContext.Edit).XmlOrderIds);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns non-customized filterable attributes.
    /// </summary>
    public IList<CxAttributeMetadata> NonCustomFilterableAttributes
    {
      get
      {
        return GetAttributesFromIds(GetAttributeOrder(NxAttributeContext.Filter).XmlOrderIds);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns attribute order object depending on the given attribute order type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public CxAttributeOrder GetAttributeOrder(NxAttributeContext type)
    {
      return m_AttributeOrders[type];
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Caption to display on entity customize form.
    /// </summary>
    public string CustomizeCaption
    {
      get { return CxUtils.Nvl(this["customize_caption"], PluralCaption); }
      set { this["customize_caption"] = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns comma-separated entity usage IDs refresh of this entity depends on.
    /// </summary>
    public string RefreshDependsOnEntityUsages
    { get { return this["refresh_depends_on_entity_usages"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if grid view can be customizable with entity customization form.
    /// </summary>
    public bool IsGridViewCustomizable
    { get { return this["grid_view_customizable"].ToLower() != "false"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if query can be customizable with entity customization form.
    /// </summary>
    public bool IsQueryCustomizable
    { get { return this["query_customizable"].ToLower() != "false"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if edit form can be customizable with entity customization form.
    /// </summary>
    public bool IsEditFormCustomizable
    { get { return this["edit_form_customizable"].ToLower() != "false"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if filter form can be customizable with entity customization form.
    /// </summary>
    public bool IsFilterFormCustomizable
    { get { return this["filter_form_customizable"].ToLower() != "false"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the Custom Lookup customization interface 
    /// should be visible in Enterprise Customization.
    /// </summary>
    public bool IsCustomLookupsCustomizable
    { get { return CxBool.Parse(this["custom_lookups_customizable"], true); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Specifies if the visibility of the entity usage can be customized
    /// using the entity customization form.
    /// </summary>
    public bool IsVisibilityCustomizable
    { get { return this["visibility_customizable"].ToLower() != "false"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Specifies if the paging feature should be used where it's possible
    /// while displaying entity's data.
    /// </summary>
    public bool IsPagingEnabled 
    {
            get {
                if (DisplayAllRecordsWithoutFooter == true)
                    return false;
                return this["paging_enabled"].ToLower() != "false";
            }
        }
    //-------------------------------------------------------------------------
    /// <summary>
    /// If true, when the entity is shown in list (grid), the actual select
    /// will be performed just when the filter criteria are defined.
    /// </summary>
    public bool SelectOnlyWhenFilterEnabled
    {
      get { return CxBool.Parse(this["select_only_when_filter_enabled"], false); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the updated entity should be reloaded after update.
    /// </summary>
    public bool ReloadEntityAfterUpdate
    {
      get { return CxBool.Parse(this["reload_entity_after_update"], true); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// If true, filter form is displayed before displaying grid with
    /// entity list. If false, grid with entity list is displayed first.
    /// </summary>
    public bool WebFilterOnStart
    { get { return this["web_filter_on_start"].ToLower() != "false"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Silverlight option.
    /// If true, filter form is expanded and requires to perform filtering, by default.
    /// Otherwise, filter panel is collapsed, and the list displays records rightaway.
    /// </summary>
    public bool SlFilterOnStart
    { get { return CxBool.Parse(this["sl_filter_on_start"], false); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Applies attribute values of the current entity to all the entities inherited from this.
    /// </summary>
    public void ApplyAttributesToInheritedEntityUsages()
    {
      foreach (CxEntityUsageMetadata entityUsage in Holder.EntityUsages.Items)
      {
        bool shouldBeApplied = entityUsage.Entity == this;
        if (!shouldBeApplied)
        {
          CxEntityUsageMetadata currentEntityUsage = this as CxEntityUsageMetadata;
          shouldBeApplied = currentEntityUsage != null && entityUsage != this && entityUsage.IsInheritedFrom(currentEntityUsage);
        }

        if (shouldBeApplied)
        {
          ApplyAttributesToEntityUsage(entityUsage);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Applies attribute values of the current entity to the given entity usage.
    /// </summary>
    /// <param name="entityUsage">entity usage the attribute values should be applied to</param>
    public void ApplyAttributesToEntityUsage(CxEntityUsageMetadata entityUsage)
    {
      foreach (CxAttributeMetadata attribute in m_AttributeList)
      {
        attribute.ApplyToEntityUsage(entityUsage);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Applies current entity's visibility to all the inherited entity usages.
    /// </summary>
    virtual public void ApplyVisibilityToInheritedEntityUsages()
    {
      IList<CxEntityUsageMetadata> list = GetDirectlyInheritedEntityUsages();
      foreach (CxEntityUsageMetadata entityUsage in list)
      {
        if (!entityUsage.Customizable)
        {
          entityUsage.Visible = Visible;
          entityUsage.ApplyVisibilityToInheritedEntityUsages();
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a list of direct descendants of the current metadata object.
    /// </summary>
    /// <returns>a list of direct descendants</returns>
    virtual public IList<CxEntityUsageMetadata> GetDirectlyInheritedEntityUsages()
    {
      List<CxEntityUsageMetadata> list = new List<CxEntityUsageMetadata>();
      foreach (CxEntityUsageMetadata entityUsage in Holder.EntityUsages.Items)
      {
        if (entityUsage.InheritedEntityUsage == null && entityUsage.Entity == this)
          list.Add(entityUsage);
      }
      return list;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if SQL select and DB object are empty for this entity.
    /// </summary>
    public bool IsSqlEmpty
    {
      get { return CxUtils.IsEmpty(this["sql_select"]) && CxUtils.IsEmpty(this["db_object"]); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if entity should not be added to Bookmarks and Recent Items lists.
    /// </summary>
    public bool BookmarksAndRecentItemsDisabled
    { 
      get { return this["disable_bookmarks_and_recent_items"].ToLower() == "true"; } 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if editable grid columns should be marked with bold captions.
    /// Works for Windows only.
    /// </summary>
    public bool MarkEditableGridColumns
    {
      get { return this["mark_editable_grid_columns"].ToLower() == "true"; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a list of attributes built on the ground of the given attribute ids.
    /// </summary>
    /// <param name="attributeIds">attribute ids to be used to build the result</param>
    /// <returns>list of attributes</returns>
    public IList<CxAttributeMetadata> GetAttributesFromIds(IEnumerable<string> attributeIds)
    {
      // Validate
      if (attributeIds == null)
        throw new ExNullArgumentException("attributeIds");

      UniqueList<CxAttributeMetadata> result = new UniqueList<CxAttributeMetadata>();
      foreach (string attributeId in attributeIds)
      {
        CxAttributeMetadata attribute = GetAttribute(attributeId);
#if DEBUG
        //if (attribute == null)
        //  throw new ExException(
        //    string.Format("Cannot find attribute by its id: <{0}.{1}>", Id, attributeId), new ExNullReferenceException("attribute"));
#endif
        result.Add(attribute);
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a list of attribute ids built on the ground of the given attributes.
    /// </summary>
    /// <param name="attributes">attributes to be used to build the result</param>
    /// <returns>list of attribute ids</returns>
    public IList<string> GetIdsFromAttributes(IEnumerable<CxAttributeMetadata> attributes)
    {
      UniqueList<string> result = new UniqueList<string>();
      foreach (CxAttributeMetadata attribute in attributes)
      {
        result.Add(attribute.Id);
      }
      return result;
    }
        //-------------------------------------------------------------------------

        
        public bool MultilselectionAllowded
        {
            get { return CxBool.Parse (this["multilselection_allowded"], true); }
        }

        //-------------------------------------------------------------------------


        public bool DisplayAllRecordsWithoutFooter
        {
            get { return CxBool.Parse(this["display_all_records_without_footer"], false); }
        }

        public bool WordwrapRowdata
        {
            get { return CxBool.Parse(this["wordwrap_rowdata"], false); }
        }

        public string GridHint
        {
            get { return this["grid_hint"]; }
        }
        
    }
}