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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Framework.Entity.Filter;
using Framework.Utils;
using Framework.Metadata;
using Framework.Db;

namespace Framework.Entity
{
  //----------------------------------------------------------------------------
  /// <summary>
  /// Base entity class.
  /// </summary>
  public class CxBaseEntity : CxAbstractDefinition, IxEntity
  {
    //----------------------------------------------------------------------------
    // Standard attribute names
    public const string NAME = "Name";
    public const string DESCRIPTION = "Description";
    public const string CALCULATION_PREFIX_PARENT = "PARENT.";
    public const string CALCULATION_PREFIX_SQL = "SQL.";
    //----------------------------------------------------------------------------
    static protected bool m_IsRefreshAfterInsertOrUpdatePerformed = true;
    //----------------------------------------------------------------------------
    protected CxEntityUsageMetadata m_Metadata; // Entity usage metadata the entity based on
    protected Hashtable m_Properties = new Hashtable(); // Dictionary for properties
    protected Hashtable m_OldProperties = new Hashtable(); // Dictionary for properties old values
    protected List<string> m_AllAttrs; // List of all attribute names
    protected List<string> m_EditableAttrs; // List of editable attribute names
    protected List<string> m_StorableAttrs; // List of storable attribute names
    protected CxBaseEntity m_Parent; // Parent entity for this entity.
    protected bool m_IsNew; // true if entity does not exists in the database
    protected bool m_IsUpdated; // true if entity exists in the database but was updated
    protected bool m_IsDeleted; // true if entity exists in the database but was deleted
    protected DataRow m_DataRow; // Data row this entity was created by
    protected string m_Guid = Guid.NewGuid().ToString(); // Global unique identifier of this entity
    protected bool m_WriteToDb = true; // true if data should be written to the database on ApplyUpdate()
    protected CxChildEntitiesDictionary m_InitialChildEntities =
      new CxChildEntitiesDictionary(); // Map with child entity usages vs. list of initial child entities
    protected CxChildEntitiesDictionary m_CurrentChildEntities =
      new CxChildEntitiesDictionary(); // Map with child entity usages vs. list of current child entities
    protected CxChildEntitiesDictionary m_InsertedChildEntities =
      new CxChildEntitiesDictionary(); // Map with child entity usages vs. list of inserted child entities
    protected CxChildEntitiesDictionary m_UpdatedChildEntities =
      new CxChildEntitiesDictionary(); // Map with child entity usages vs. list of updatd child entities
    protected CxChildEntitiesDictionary m_DeletedChildEntities =
      new CxChildEntitiesDictionary(); // Map with child entity usages vs. list of deleted child entities
    protected CxChildEntitiesDictionary m_UnchangedChildEntities =
      new CxChildEntitiesDictionary(); // Map with child entity usages vs. list of unchanged child entities
    protected bool m_DisappearedAfterUpdate; // true if entity disappeared after update
    protected Hashtable m_Extra = new Hashtable(); // Dictionary of extra objects attached to the entity
    protected CxBaseEntity m_OriginalEntity; // Entity with the original values to compare with
    protected CxChildEntityUsageMetadata m_ErrorChildEntityUsage; // On-save error source child entity usage
    protected CxBaseEntity m_ErrorChildEntity; // On-save error source entity
    protected bool m_IsMain; // Custom flag indicating main dialog entity
    // List of referenced entities to insert before child entities insert
    protected List<CxBaseEntity> m_EntitiesToInsertBeforeChildren = new List<CxBaseEntity>();
    // List of entities to delete after child entities delete
    protected List<CxBaseEntity> m_EntitiesToDeleteAfterChildren = new List<CxBaseEntity>();
    //-------------------------------------------------------------------------
    public List<CxBaseEntity> EntitiesToInsertBeforeChildren
    {
      get { return m_EntitiesToInsertBeforeChildren; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates entity class for the given entity usage metadata.
    /// </summary>
    /// <param name="metadata">metadata that describes this entity</param>
    /// <returns>created entity instance</returns>
    static public CxBaseEntity Create(CxEntityUsageMetadata metadata)
    {
      return (CxBaseEntity) metadata.CreateEntityInstance();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates entity class for the given entity usage metadata 
    /// and populates it with default values.
    /// </summary>
    /// <param name="metadata">metadata that describes this entity</param>
    /// <param name="parentEntity">parent entity of the new one</param>
    /// <param name="connection">database connection to use</param>
    /// <returns>created entity instance</returns>
    static public CxBaseEntity CreateWithDefaults(CxEntityUsageMetadata metadata,
                                                  CxBaseEntity parentEntity,
                                                  CxDbConnection connection)
    {
      CxBaseEntity entity = Create(metadata);
      entity.Parent = parentEntity;
      entity.SetDefaultValues(connection, null, null, false);
      return entity;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates entity class for the given entity usage metadata 
    /// and populates it with default values.
    /// </summary>
    /// <param name="metadata">metadata that describes this entity</param>
    /// <param name="getParentValue">method to get parent values</param>
    /// <param name="getSequenceValue">method to get a sequence code</param>
    /// <param name="connection">database connection to use</param>
    /// <returns>created entity instance</returns>
    static public CxBaseEntity CreateWithDefaults(
      CxEntityUsageMetadata metadata,
      DxGetParentValue getParentValue,
      DxGetSequenceValue getSequenceValue,
      CxDbConnection connection)
    {
      CxBaseEntity entity = Create(metadata);
      entity.SetDefaultValues(connection, getParentValue, getSequenceValue, false);
      return entity;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates entity class for the given entity usage metadata 
    /// and populates it from the given data row.
    /// </summary>
    /// <param name="metadata">metadata that describes this entity</param>
    /// <param name="dataRow">data row to populate entity from</param>
    /// <returns>created entity instance</returns>
    static public CxBaseEntity CreateByDataRow(CxEntityUsageMetadata metadata,
                                               DataRow dataRow)
    {
      CxBaseEntity entity = Create(metadata);
      entity.ReadDataRow(dataRow);
      entity.SetDataRow(dataRow);
      entity.OriginalEntity = entity.Copy();
      return entity;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity from the given data row.
    /// </summary>
    public static CxBaseEntity GetRowEntity(
      CxEntityUsageMetadata entityUsage, CxBaseEntity parentEntity, CxGenericDataRow dr)
    {
      if (dr != null)
      {
        CxBaseEntity entity;
        if (dr.Tag == null)
        {
          entity = CreateByDataRow(entityUsage, dr);
          dr.Tag = entity;
          entity.Parent = parentEntity;
        }
        else
        {
          entity = (CxBaseEntity) dr.Tag;
          // Here we synchronize the entity and the data-row, 
          // however it's a performance consuming operation.
          // TODO: Provide the 100% synchronization in another way.
          entity.ReadDataRow(dr);
          entity.SetDataRow(dr);
          if (entity.OriginalEntity == null)
            entity.OriginalEntity = entity.Copy();
        }
        return entity;
      }
      return null;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates entity class for the given entity usage metadata 
    /// and populates it from the given value provider.
    /// </summary>
    /// <param name="metadata">metadata that describes this entity</param>
    /// <param name="provider">value provider to populate entity from</param>
    /// <returns>created entity instance</returns>
    static public CxBaseEntity CreateByValueProvider(
      CxEntityUsageMetadata metadata,
      IxValueProvider provider)
    {
      CxBaseEntity entity = Create(metadata);
      entity.ReadValueProvider(provider);
      return entity;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates entity class for the given entity usage metadata 
    /// and populates it data found in DB by primary key.
    /// If entity with such PK not found returns null
    /// </summary>
    /// <param name="metadata">metadata that describes this entity</param>
    /// <param name="connection">database connection to use</param>
    /// <param name="pkValues">values of entity primary key columns</param>
    /// <returns>created entity instance populated from database
    /// or null if entity with such primary key values not found</returns>
    static public CxBaseEntity CreateAndReadFromDb(CxEntityUsageMetadata metadata,
                                                   CxDbConnection connection,
                                                   object[] pkValues)
    {
      return CreateAndReadFromDb(metadata, connection, metadata.CreatePrimaryKeyValueProvider(pkValues));
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates entity class for the given entity usage metadata 
    /// and populates it data found in DB by primary key.
    /// If entity with such PK not found returns null
    /// </summary>
    /// <param name="metadata">metadata that describes this entity</param>
    /// <param name="connection">database connection to use</param>
    /// <param name="provider">value provider to get condition param values</param>
    /// <returns>created entity instance populated from database
    /// or null if entity with such primary key values not found</returns>
    static public CxBaseEntity CreateAndReadFromDb(
      CxEntityUsageMetadata metadata,
      CxDbConnection connection,
      IxValueProvider provider)
    {
      CxBaseEntity entity = GetEntity(metadata, connection, provider);
      if (entity != null)
      {
        entity.SetPermanentCalculatedValues(connection);
        entity.OriginalEntity = entity.Copy();
      }
      return entity;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates entity class for the given entity usage metadata 
    /// and populates it data found in DB by primary key.
    /// If entity with such PK not found returns null
    /// </summary>
    /// <param name="metadata">metadata that describes this entity</param>
    /// <param name="connection">database connection to use</param>
    /// <param name="where">where statement to be used to get data from the database</param>
    /// <param name="provider">value provider to get condition param values</param>
    /// <returns>created entity instance populated from database
    /// or null if entity with such primary key values not found</returns>
    static public CxBaseEntity CreateAndReadFromDb(
      CxEntityUsageMetadata metadata,
      CxDbConnection connection,
      string where,
      IxValueProvider provider)
    {
      DataRow dataRow = GetEntityDataRow(metadata, connection, where, provider);
      if (dataRow != null)
      {
        CxBaseEntity entity = CreateByDataRow(metadata, dataRow);
        entity.SetPermanentCalculatedValues(connection);
        entity.OriginalEntity = entity.Copy();
        return entity;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates entity by the entity data.
    /// </summary>
    /// <param name="holder">metadata holder instance</param>
    /// <param name="data">entity data to create entity from</param>
    /// <returns>created entity or null</returns>
    static public CxBaseEntity CreateByEntityData(CxMetadataHolder holder, CxEntityData data)
    {
      if (holder != null && data != null && CxUtils.NotEmpty(data.EntityUsageId))
      {
        CxEntityUsageMetadata entityUsage = holder.EntityUsages.Find(data.EntityUsageId);
        if (entityUsage != null)
        {
          return CreateByValueProvider(entityUsage, data.ValueProvider);
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates entity by the entity data for the futher paste.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    /// <param name="holder">metadata holder instance</param>
    /// <param name="data">entity data to create entity from</param>
    /// <param name="parentEntity">an entity to be the parent entity for the pasted entity</param>
    /// <returns>created entity or null</returns>
    static public CxBaseEntity CreateForPaste(
      CxDbConnection connection,
      CxMetadataHolder holder,
      CxEntityData data,
      CxBaseEntity parentEntity)
    {
      CxBaseEntity entity = CreateByEntityData(holder, data);
      if (entity != null)
      {
        entity.Parent = parentEntity;
        entity.SetValuesForPaste(connection);
      }
      return entity;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns data row that that corresponds with the entity primary key.
    /// If there are no such row in the database returns null.
    /// If there are several rows raiss exceptopn
    /// </summary>
    /// <param name="metadata">metadata that describes this entity</param>
    /// <param name="connection">database connection to use</param>
    /// <param name="pkValues">values of entity primary key columns</param>
    /// <returns>data row that that corresponds with the entity primary key
    /// or null if there are now suc row or there are sevelar rows</returns>
    static public DataRow GetEntityDataRow(CxEntityUsageMetadata metadata,
                                           CxDbConnection connection,
                                           object[] pkValues)
    {
      return metadata.ReadDataRow(connection, metadata.ComposePKCondition(), pkValues);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns data row that that corresponds with the entity where condition.
    /// If there are no such row in the database returns null.
    /// If there are several rows raiss exceptopn
    /// </summary>
    /// <param name="metadata">metadata that describes this entity</param>
    /// <param name="connection">database connection to use</param>
    /// <param name="provider">value provider to get parameter values</param>
    /// <returns>data row that that corresponds with the entity where condition
    /// or null if there are no such row or there are sevelar rows</returns>
    static public DataRow GetEntityDataRow(
      CxEntityUsageMetadata metadata,
      CxDbConnection connection,
      IxValueProvider provider)
    {
      return metadata.ReadDataRow(connection, metadata.ComposePKCondition(), provider);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns entity retrieved by the given value provider (filtered).
    /// </summary>
    /// <param name="metadata">metadata the entity belongs to</param>
    /// <param name="connection">database connection</param>
    /// <param name="provider">value provider to get parameter values</param>
    /// <returns>the entity retrieved</returns>
    static public CxBaseEntity GetEntity(
      CxEntityUsageMetadata metadata,
      CxDbConnection connection,
      IxValueProvider provider)
    {
      return (CxBaseEntity) metadata.ReadEntity(connection, metadata.ComposePKCondition(), provider);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns data row that that corresponds with the entity where condition.
    /// If there are no such row in the database returns null.
    /// If there are several rows raiss exceptopn
    /// </summary>
    /// <param name="metadata">metadata that describes this entity</param>
    /// <param name="connection">database connection to use</param>
    /// <param name="where">where clause to be used</param>
    /// <param name="provider">value provider to get parameter values</param>
    /// <returns>data row that that corresponds with the entity where condition
    /// or null if there are no such row or there are sevelar rows</returns>
    static public DataRow GetEntityDataRow(
      CxEntityUsageMetadata metadata,
      CxDbConnection connection,
      string where,
      IxValueProvider provider)
    {
      return metadata.ReadDataRow(connection, where, provider);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="metadata">metadata that describes this entity</param>
    public CxBaseEntity(CxEntityUsageMetadata metadata)
    {
      m_Metadata = metadata;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Entity usage metadata the entity based on.
    /// </summary>
    public CxEntityUsageMetadata Metadata
    {
      get { return m_Metadata; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Parent entity for this entity.
    /// </summary>
    public CxBaseEntity Parent
    {
      get { return m_Parent; }
      set { m_Parent = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Data row this entity was created by.
    /// </summary>
    public DataRow DataRow
    {
      get { return m_DataRow; }
      protected set { m_DataRow = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if entity disappeared after update.
    /// </summary>
    public bool DisappearedAfterUpdate
    {
      get { return m_DisappearedAfterUpdate; }
      set { m_DisappearedAfterUpdate = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Dictionary of extra objects attached to the entity.
    /// </summary>
    public Hashtable Extra
    {
      get { return m_Extra; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns list of editable properties.
    /// </summary>
    /// <returns>list of editable properties</returns>
    override protected IList<string> GetEditableProperties()
    {
      if (m_EditableAttrs == null)
      {
        m_EditableAttrs = new List<string>();
        foreach (CxAttributeMetadata attribute in Metadata.Attributes)
        {
          if (attribute.Editable)
          {
            m_EditableAttrs.Add(attribute.Id);
          }
        }
      }
      return m_EditableAttrs;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if property with the given name is editable or false otherwise.
    /// </summary>
    /// <param name="name">name of property to check</param>
    /// <returns>true if property with the given name is editable or false otherwise.</returns>
    override protected bool IsEditable(string name)
    {
      return true;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns list of all properties.
    /// </summary>
    /// <returns>list of all properties</returns>
    override protected IList<string> GetAllProperties()
    {
      if (m_AllAttrs == null)
      {
        m_AllAttrs = new List<string>();
        foreach (CxAttributeMetadata attribute in Metadata.Attributes)
        {
          m_AllAttrs.Add(attribute.Id);
        }
      }
      return m_AllAttrs;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns list of storable properties.
    /// </summary>
    /// <returns>list of storable properties</returns>
    override protected IList<string> GetStorableProperties()
    {
      if (m_StorableAttrs == null)
      {
        m_StorableAttrs = new List<string>();
        foreach (CxAttributeMetadata attribute in Metadata.Attributes)
        {
          if (attribute.Storable)
          {
            m_StorableAttrs.Add(attribute.Id);
          }
        }
      }
      return m_StorableAttrs;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if property with the given name is storable or false otherwise.
    /// </summary>
    /// <param name="name">name of property to check</param>
    /// <returns>true if property with the given name is storable or false otherwise.</returns>
    override protected bool IsStorable(string name)
    {
      return true;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if property with the given name and value should be saved or false otherwise.
    /// </summary>
    /// <param name="name">name of property</param>
    /// <param name="value">property value</param>
    /// <returns>true if property with the given name and value should be saved or false otherwise</returns>
    override protected bool ShouldBeWritten(string name, ref object value)
    {
      return true;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns caption of the attribute (or just the name if such attribute not found).
    /// </summary>
    /// <param name="name">name of the attribute</param>
    /// <returns>caption of the attribute (or just the name if such attribute not found)</returns>
    virtual public string GetAttributeCaption(string name)
    {
      CxAttributeMetadata attribute = Metadata.GetAttribute(name);
      return (attribute != null ? attribute.GetCaption(Metadata) : name);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Completely validates entity. Calculates on-save expressions before validation.
    /// Uses database connection to calculate on-save expressions and mandatory conditions.
    /// </summary>
    virtual public void Validate(CxDbConnection connection)
    {
      CalculateOnSaveExpressions(connection);
      ValidateMandatoryConditions(connection);
      Validate();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Validates entity.
    /// </summary>
    virtual public void Validate()
    {
      ValidateMandatory();
      ValidateRanges();

      foreach (CxChildEntityUsageMetadata childMetadata in Metadata.ChildEntityUsagesList)
      {
        ValidateChildren(childMetadata);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Validates specified children of entity.
    /// </summary>
    /// <param name="childMetadata">child entity usage to validate entities of</param>
    virtual public void ValidateChildren(CxChildEntityUsageMetadata childMetadata)
    {
      ResetChildEntityErrorData();

      if (childMetadata.OwnedBy)
      {
        IList<CxBaseEntity> entityList = m_CurrentChildEntities[childMetadata];
        if (entityList != null)
        {
          foreach (CxBaseEntity childEntity in entityList)
          {
            try
            {
              ValidateChildEntity(childEntity);
            }
            catch
            {
              SetChildEntityErrorData(childMetadata, childEntity);
              throw;
            }
          }
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Validates child entity.
    /// </summary>
    /// <param name="childEntity">child entity to validate</param>
    virtual public void ValidateChildEntity(CxBaseEntity childEntity)
    {
      childEntity.Validate();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Validates if all mandatory properties filled.
    /// </summary>
    virtual protected void ValidateMandatory()
    {
      List<string> violatedMandatoryIds = new List<string>();
      foreach (CxAttributeMetadata attributeMetadata in NonReadOnlyAttributes)
      {
        if (IsMandatory(attributeMetadata.Id))
        {
          try
          {
            ValidateMandatoryAttribute(attributeMetadata.Id);
          }
          catch (ExMandatoryViolationException)
          {
            violatedMandatoryIds.Add(attributeMetadata.Id);
          }
        }
      }
      if (violatedMandatoryIds.Count > 0)
        throw new ExMandatoryViolationException(this, violatedMandatoryIds.ToArray());
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Validates mandatory attribute value and raises an exception.
    /// </summary>
    /// <param name="attrName">attribute name</param>
    virtual public void ValidateMandatoryAttribute(string attrName)
    {
      if (CxUtils.IsEmpty(this[attrName]))
      {
        throw new ExMandatoryViolationException(this, attrName);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Validates if numeric properties are in range.
    /// </summary>
    virtual protected void ValidateRanges()
    {
      foreach (string name in EditableProperties)
      {
        CxAttributeMetadata attribute = Metadata.GetAttribute(name);
        if ((attribute.MinValue > Decimal.MinValue || attribute.MaxValue < Decimal.MaxValue) &&
            !CxUtils.IsNull(this[name]))
        {
          string s = CxUtils.ValidateRange(
            CxFloat.ParseDecimal(this[name], 0),
            attribute.MinValue,
            attribute.MaxValue,
            GetAttributeCaption(name),
            Metadata.Holder.GetErr("{0} should be between {1} and {2}"),
            Metadata.Holder.GetErr("{0} should be not less than {1}"),
            Metadata.Holder.GetErr("{0} should be not greater than {2}"));
          if (CxUtils.NotEmpty(s))
          {
            throw new ExValidationException(s, name);
          }
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if an attribute with the given name is mandatory one.
    /// </summary>
    /// <param name="name">attribute name</param>
    /// <returns>true if an attribute with the given anme is mandatory one</returns>
    virtual public bool IsMandatory(string name)
    {
      CxAttributeMetadata attribute = Metadata.GetAttribute(name);
      return (attribute != null ? (!attribute.Nullable) : false);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns value of the property.
    /// </summary>
    /// <param name="name">name of the field</param>
    /// <returns>value of the edit field by the field name</returns>
    override protected object GetProperty(string name)
    {
      if (name.StartsWith(CxEntityMetadata.PARAM_OLD_PREFIX))
      {
        return GetOldValue(name.Substring(CxEntityMetadata.PARAM_OLD_PREFIX.Length));
      }
      object propValue = m_Properties[name.ToUpper()];
      if (Metadata != null)
      {
        CxAttributeMetadata attr = Metadata.GetAttribute(name);
        if (attr != null)
        {
          if (attr.IsDbFile && CxUtils.IsEmpty(propValue))
          {
            return new byte[0];
          }
        }
      }
      return propValue;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets property value.
    /// </summary>
    /// <param name="name">name of the property</param>
    /// <param name="value">value of the property</param>
    override protected void SetProperty(string name, object value)
    {
      if (name.StartsWith(CxEntityMetadata.PARAM_OLD_PREFIX))
      {
        SetOldValue(name.Substring(CxEntityMetadata.PARAM_OLD_PREFIX.Length), value);
      }
      else
      {
        m_Properties[name.ToUpper()] = value;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns data table with the rows of child entities of the given child ID.
    /// </summary>
    /// <param name="connection">database conenction to use</param>
    /// <param name="dt">data table with the rows of child entities of the given child ID</para,>
    /// <param name="childId">ID of the children entity</param>
    virtual public void LoadChildTable(
      CxDbConnection connection,
      DataTable dt,
      string childId,
      NxEntityDataCache cacheMode)
    {
      Metadata.GetChildMetadata(childId).ReadChildData(connection, dt, this, "", cacheMode);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns data table with the rows of child entities of the given child ID.
    /// </summary>
    /// <param name="connection">database conenction to use</param>
    /// <param name="dt">data table with the rows of child entities of the given child ID</para,>
    /// <param name="childId">ID of the children entity</param>
    virtual public void LoadChildTable(
      CxDbConnection connection,
      DataTable dt,
      string childId)
    {
      LoadChildTable(connection, dt, childId, NxEntityDataCache.Default);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns data table with the rows next level of data for the 
    /// self- referencing hierarchical entity.
    /// </summary>
    /// <param name="connection">database conenction to use</param>
    /// <param name="dt">data table with the rows of child entities of the given child ID</para,>
    virtual public void LoadNextLevelTable(
      CxDbConnection connection, DataTable dt)
    {
      Metadata.ReadNextLevelData(connection, dt, this);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Calculates and returns default value expression.
    /// </summary>
    /// <param name="expression">expression to calculate</param>
    /// <param name="attr">attribute to calculate expression for</param>
    /// <param name="connection">Db connection to use</param>
    /// <param name="entity">entity to get expression parameters from</param>
    /// <param name="getParentValue">handler that returns the parent entity value</param>
    /// <param name="getSequenceValue">handler that returns the appropriate sequence value</param>
    /// <returns>calculated value</returns>
    static public object CalculateDefaultValue(
      string expression,
      CxAttributeMetadata attr,
      CxDbConnection connection,
      CxBaseEntity entity,
      DxGetParentValue getParentValue,
      DxGetSequenceValue getSequenceValue)
    {
      object value = null;
      if (expression.StartsWith("="))
      {
        expression = expression.Substring(1);
        string sUp = expression.ToUpper();
        if (sUp.StartsWith(CALCULATION_PREFIX_PARENT))
        {
          expression = expression.Substring(CALCULATION_PREFIX_PARENT.Length);
          if (getParentValue != null)
          {
            value = getParentValue(expression);
          }
          else if (entity != null)
          {
            value = entity.GetParentEntityValue(connection, expression);
          }
        }
        else if (sUp.StartsWith(CALCULATION_PREFIX_SQL))
        {
          string sql = expression.Substring(CALCULATION_PREFIX_SQL.Length);
          if (entity != null)
          {
            IxValueProvider valueProvider = entity.Metadata.PrepareValueProvider(entity);
            using (CxDbCommand command = connection.CreateCommand(sql, valueProvider))
            {
              value = connection.ExecuteScalar(command);
            }
          }
          else
          {
            using (CxDbCommand command = connection.CreateCommand(sql))
            {
              value = connection.ExecuteScalar(command);
            }
          }
        }
        else if (sUp.StartsWith("APPLICATION$"))
        {
          if (entity != null)
          {
            IxValueProvider provider = entity.Metadata.Holder.ApplicationValueProvider;
            if (provider != null)
            {
              value = provider[sUp];
            }
          }
        }
        else if (sUp == "@TODAY")
        {
          value = DateTime.Today;
        }
        else if (sUp == "@NOW")
        {
          value = DateTime.Now;
        }
        else if (sUp == "@SEQUENCE")
        {
          value = getSequenceValue != null ? getSequenceValue(attr, connection) : entity.GetSequenceValue(attr, connection);
        }
        else
        {
          throw new ExMetadataException(string.Format(
            "Unrecognized expression <{0}> defined for the attribute with ID=<{1}> " +
            "in entity usage with ID=<{2}>.",
            expression, attr.Id, attr.EntityMetadata.Id));
        }
      }
      else if (CxUtils.NotEmpty(expression))
      {
        value = attr.ConvertConstantToObject(expression);
      }
      return value;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns next sequence value.
    /// </summary>
    virtual public int GetSequenceValue(
      CxAttributeMetadata attr,
      CxDbConnection connection)
    {
      return connection.GetNextId();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets default values of attributes for the entity.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    /// <param name="getParentValue">parent value provider delegate</param>
    /// <param name="getSequenceValue">sequence value provider delegate</param>
    /// <param name="overwrite">if true overwrites not empty attribute values</param>
    public void SetDefaultValues(CxDbConnection connection,
                                         DxGetParentValue getParentValue,
                                         DxGetSequenceValue getSequenceValue,
                                         bool overwrite)
    {
      SetDefaultValues(connection, getParentValue, getSequenceValue, overwrite, null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets default values of attributes for the entity.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    /// <param name="getParentValue">parent value provider delegate</param>
    /// <param name="getSequenceValue">sequence value provider delegate</param>
    /// <param name="overwrite">if true overwrites not empty attribute values</param>
    /// <param name="excludedAttrIDs">excluded attribute ID list</param>
    virtual public void SetDefaultValues(CxDbConnection connection,
                                         DxGetParentValue getParentValue,
                                         DxGetSequenceValue getSequenceValue,
                                         bool overwrite,
                                         string[] excludedAttrIDs)
    {
      // Sort by default value calculation order
      ArrayList orderedAttrList = new ArrayList();
      ArrayList orderedKeysList = new ArrayList();
      ArrayList unsortedAttrList = new ArrayList();
      foreach (CxAttributeMetadata attribute in Metadata.Attributes)
      {
        if (attribute.DefaultValueCalculationOrder >= 0)
        {
          orderedAttrList.Add(attribute);
          orderedKeysList.Add(attribute.DefaultValueCalculationOrder);
        }
        else
        {
          unsortedAttrList.Add(attribute);
        }
      }

      CxAttributeMetadata[] orderedAttrArray = new CxAttributeMetadata[orderedAttrList.Count];
      orderedAttrList.CopyTo(orderedAttrArray);
      int[] orderedKeysArray = new int[orderedKeysList.Count];
      orderedKeysList.CopyTo(orderedKeysArray);
      Array.Sort(orderedKeysArray, orderedAttrArray);

      ArrayList attrList = new ArrayList(orderedAttrArray);
      attrList.AddRange(unsortedAttrList);

      UniqueList<CxAttributeMetadata> excludedAttrs = new UniqueList<CxAttributeMetadata>();
      if (excludedAttrIDs != null)
      {
        foreach (string attrId in excludedAttrIDs)
        {
          excludedAttrs.Add(Metadata.GetAttribute(attrId));
        }
      }

      // Calculate default values
      foreach (CxAttributeMetadata attribute in attrList)
      {
        if (CxUtils.NotEmpty(this[attribute.Id]) && !overwrite) continue;
        if (excludedAttrs.Contains(attribute)) continue;

        object value = null;
        string defaultText = null;

        try
        {
          value = CalculateDefaultValue(
            attribute.Default,
            attribute,
            connection,
            this,
            getParentValue,
            getSequenceValue);

          // Calculate mandatory row source default value
          if (value == null &&
              attribute.RowSource != null &&
              attribute.InitWithRowSourceDefaultValue &&
              !attribute.RowSourceHasEmptyRow)
          {
            object defaultValue;

            attribute.RowSource.GetDefaultValue(
              connection, attribute.RowSourceFilter, this, true, attribute.IsMultiValueLookup, null,
              out defaultValue, out defaultText);

            value = defaultValue;
          }
        }
        catch (Exception ex)
        {
          throw new ExException(string.Format("Error trying to calculate the default value for attribute <{0}>", attribute.Id), ex);
        }
        if (value != null)
        {
          this[attribute.Id] = value;
          // Assign row source text attribute value
          if (CxUtils.NotEmpty(defaultText))
          {
            CxAttributeMetadata textAttr = Metadata.GetTextAttribute(attribute);
            if (textAttr != null && (CxUtils.IsEmpty(this[textAttr.Id]) || overwrite))
            {
              this[textAttr.Id] = defaultText;
            }
          }

        }
      }

      SetCalculatedValues(connection, overwrite, excludedAttrIDs);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the text by the current attribute's value while expecting the attribute to have 
    /// a proper row-source defined.
    /// If the method is supplied with somehow wrong data, an empty string is returned back.
    /// </summary>
    public string GetTextByRowSource(CxDbConnection connection, string attributeId)
    {
      var attribute = Metadata.GetAttribute(attributeId);
      if (attribute != null)
      {
        var rowSource = attribute.RowSource;
        if (rowSource != null)
        {
          var value = GetProperty(attributeId);
          return rowSource.GetDescriptionByValue(connection, value, attribute.IsMultiValueLookup);
        }
      }
      return string.Empty;
    }
    //-------------------------------------------------------------------------
    public DataRow GetCurrentRowSourceDataRow(CxDbConnection connection, string attributeId)
    {
      var attribute = Metadata.GetAttribute(attributeId);
      if (attribute != null)
      {
        var rowSource = attribute.RowSource;
        if (rowSource != null)
        {
          var value = GetProperty(attributeId);
          var table = rowSource.GetDataTable(null, connection, null, null, false);
          return table.Select(string.Format("{0}='{1}'", rowSource.EntityUsage.PrimaryKeyAttribute.Id, Convert.ToString(value))).FirstOrDefault();
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Calculates calculated values and sets entity attributes.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    /// <param name="overwrite">if true overwrites not empty attribute values</param>
    virtual public void SetCalculatedValues(CxDbConnection connection,
                                            bool overwrite)
    {
      SetCalculatedValues(connection, overwrite, null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Calculates calculated values and sets entity attributes.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    /// <param name="overwrite">if true overwrites not empty attribute values</param>
    /// <param name="excludedAttrIDs">an array of attribute ids to be excluded from the calculation
    /// attribute list</param>
    virtual public void SetCalculatedValues(CxDbConnection connection,
                                            bool overwrite,
                                            string[] excludedAttrIDs)
    {
      UniqueList<CxAttributeMetadata> excludedAttrs = new UniqueList<CxAttributeMetadata>();
      if (excludedAttrIDs != null)
      {
        foreach (string attrId in excludedAttrIDs)
        {
          excludedAttrs.Add(Metadata.GetAttribute(attrId));
        }
      }

      foreach (CxAttributeMetadata attribute in Metadata.Attributes)
      {
        if (!excludedAttrs.Contains(attribute))
        {
          SetCalculatedValue(connection, attribute, overwrite);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Calculates calculated values and sets entity attributes.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    virtual public void SetPermanentCalculatedValues(CxDbConnection connection)
    {
      foreach (CxAttributeMetadata attribute in Metadata.Attributes)
      {
        if (attribute.IsCalculated)
        {
          SetCalculatedValue(connection, attribute, true);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets calculated value for the specified attribute.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="attribute"></param>
    /// <param name="overwrite"></param>
    virtual public void SetCalculatedValue(
      CxDbConnection connection,
      CxAttributeMetadata attribute,
      bool overwrite)
    {
      if (attribute != null && CxUtils.NotEmpty(attribute.LocalExpression))
      {
        if (overwrite || CxUtils.IsEmpty(this[attribute.Id]))
        {
          object value = CalculateLocalExpression(connection, attribute.LocalExpression);
          if (value != null)
          {
            this[attribute.Id] = value;
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Calculates local expression.
    /// </summary>
    /// <param name="connection">a database connection to be used</param>
    /// <param name="expression">expression to calculate</param>
    /// <returns>calculated result</returns>
    public object CalculateLocalExpression(
      CxDbConnection connection,
      string expression)
    {
      return CalculateLocalExpression(connection, expression, false);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Calculates local expression.
    /// </summary>
    /// <param name="connection">a database connection to be used</param>
    /// <param name="expression">expression to calculate</param>
    /// <param name="checkJustLocalExpression">if true, no database expression will be checked</param>
    /// <returns>calculated result</returns>
    virtual public object CalculateLocalExpression(
      CxDbConnection connection,
      string expression,
      bool checkJustLocalExpression)
    {
      object result = null;
      if (CxUtils.NotEmpty(expression))
      {
        IxValueProvider valueProvider = Metadata.PrepareValueProvider(this);
        if (expression.ToUpper().StartsWith("SQL."))
        {
          if (!checkJustLocalExpression)
          {
            string sql = expression.Substring("SQL.".Length);
            using (CxDbCommand command = connection.CreateCommand(sql, valueProvider))
            {
              result = connection.ExecuteScalar(command);
            }
          }
        }
        else
        {
          result = CxDbUtils.CalculateLocalExpression(expression, valueProvider);
        }
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Calculates local expression.
    /// </summary>
    /// <param name="connection">a database connection to be used</param>
    /// <param name="expression">expression to calculate</param>
    /// <returns>calculated result</returns>
    virtual public bool CalculateBoolExpression(
      CxDbConnection connection,
      string expression)
    {
      return CalculateBoolExpression(connection, expression, false);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Calculates local expression.
    /// </summary>
    /// <param name="connection">a database connection to be used</param>
    /// <param name="expression">expression to calculate</param>
    /// <param name="checkJustLocalExpression">if true, no database-related expression is actually checked</param>
    /// <returns>calculated result</returns>
    virtual public bool CalculateBoolExpression(
      CxDbConnection connection,
      string expression,
      bool checkJustLocalExpression)
    {
      bool result = false;
      if (CxUtils.NotEmpty(expression))
      {
        IxValueProvider valueProvider = Metadata.PrepareValueProvider(this);
        if (expression.ToUpper().StartsWith("SQL."))
        {
          if (!checkJustLocalExpression)
          {
            string sql = expression.Substring("SQL.".Length);
            result = connection.CalculateBoolExpression(sql, valueProvider);
          }
        }
        else
        {
          result = CxDbUtils.CalculateLocalBoolExpression(expression, valueProvider);
        }
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Delegate for SQL statements compoition methods.
    /// </summary>
    protected delegate string DxStatementComposerDelegate();
    //-------------------------------------------------------------------------
    /// <summary>
    /// Composes (using the delegate) and executes SQL statement using 
    /// entity attribute values as parameter values.
    /// </summary>
    /// <param name="connection">database connection a SQL should work in context of</param>
    /// <param name="sqlComposer">method that generates SQL statement</param>
    virtual protected void ComposeAndExecuteSQL(CxDbConnection connection,
                                                DxStatementComposerDelegate sqlComposer)
    {
      string sql = sqlComposer();
      ExecuteSQL(connection, sql);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Executes SQL statement using entity attribute values as parameter values.
    /// </summary>
    /// <param name="connection">database connection a SQL should work in context of</param>
    /// <param name="sql">SQL statement</param>
    virtual protected void ExecuteSQL(CxDbConnection connection, string sql)
    {
      IxValueProvider valueProvider = Metadata.PrepareValueProvider(this);
      using (CxDbCommand command = connection.CreateCommand(sql, valueProvider))
      {
        connection.ExecuteCommand(command);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Composes SQL INSERT statement to for entity.
    /// </summary>
    public string ComposeInsert()
    {
      if (CxUtils.NotEmpty(Metadata.SqlInsert))
      {
        return Metadata.SqlInsert;
      }
      else
      {
        return ComposeInsert(Metadata.DbObject, Metadata.GetDbObjectAttributes(0));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Composes SQL INSERT statement to for entity.
    /// </summary>
    /// <param name="dbObjectIndex">index of the DB object</param>
    /// <returns>insert SQL statement</returns>
    public string ComposeInsert(int dbObjectIndex)
    {
      UniqueList<CxAttributeMetadata> attributes = new UniqueList<CxAttributeMetadata>();
      attributes.AddRange(GetPrimaryKeyAttributes(dbObjectIndex));
      attributes.AddRange(Metadata.GetDbObjectAttributes(dbObjectIndex));
      return ComposeInsert(Metadata.GetDbObject(dbObjectIndex), attributes);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets the list of primary key attributes to be applied during update to the given
    /// database object (index).
    /// </summary>
    /// <returns>an array of attributes which are considered as primary for the given DB-object</returns>
    public virtual CxAttributeMetadata[] GetPrimaryKeyAttributes(int dbObjectIndex)
    {
      return Metadata.GetPrimaryKeyAttributes();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Composes SQL INSERT statement to for entity.
    /// </summary>
    /// <param name="dbObjectName">database table or view name</param>
    /// <param name="attributes">list of available attributes</param>
    /// <returns>insert SQL statement</returns>
    protected virtual string ComposeInsert(string dbObjectName, IList<CxAttributeMetadata> attributes)
    {
      ArrayList paramNames = new ArrayList();
      StringBuilder sb = new StringBuilder();
      sb.Append("INSERT INTO " + dbObjectName + "\r\n");
      sb.Append("            (");
      foreach (CxAttributeMetadata attribute in attributes)
      {
        if (attribute.Storable)
        {
          if (paramNames.Count > 0) sb.Append(", ");
          sb.Append(attribute.Id);
          paramNames.Add(attribute.Id);
        }
      }
      sb.Append(")\r\n");
      sb.Append("     VALUES (");
      for (int i = 0; i < paramNames.Count; i++)
      {
        if (i > 0) sb.Append(", ");
        sb.Append(":" + (string) (paramNames[i]));
      }
      sb.Append(")");
      return sb.ToString();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Composes SQL UPDATE statement to for entity.
    /// </summary>
    public string ComposeUpdate()
    {
      if (CxUtils.NotEmpty(Metadata.SqlUpdate))
      {
        return Metadata.SqlUpdate;
      }
      else
      {
        return ComposeUpdate(Metadata.DbObject, Metadata.GetDbObjectAttributes(0));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Composes SQL UPDATE statement to for entity.
    /// </summary>
    /// <param name="dbObjectIndex">index of the DB object</param>
    /// <returns>update SQL statement</returns>
    public string ComposeUpdate(int dbObjectIndex)
    {
      UniqueList<CxAttributeMetadata> attributes = new UniqueList<CxAttributeMetadata>();
      attributes.AddRange(GetPrimaryKeyAttributes(dbObjectIndex));
      attributes.AddRange(Metadata.GetDbObjectAttributes(dbObjectIndex));
      return ComposeUpdate(
        Metadata.GetDbObject(dbObjectIndex), attributes, Metadata.ComposePKCondition(dbObjectIndex));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Composes SQL UPDATE statement to for entity.
    /// </summary>
    /// <param name="attributes">list of available attributes</param>
    /// <returns>update SQL statement</returns>
    public string ComposeUpdate(IEnumerable<CxAttributeMetadata> attributes)
    {
      return ComposeUpdate(Metadata.DbObject, attributes);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Composes SQL UPDATE statement to for entity.
    /// </summary>
    /// <param name="dbObjectName">database table or view name</param>
    /// <param name="attributes">list of available attributes</param>
    /// <returns>update SQL statement</returns>
    protected string ComposeUpdate(
      string dbObjectName, IEnumerable<CxAttributeMetadata> attributes)
    {
      return ComposeUpdate(dbObjectName, attributes, Metadata.ComposePKCondition());
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Composes SQL UPDATE statement to for entity.
    /// </summary>
    /// <param name="dbObjectName">database table or view name</param>
    /// <param name="attributes">list of available attributes</param>
    /// <param name="whereClause">the where clause to use for the update</param>
    /// <returns>update SQL statement</returns>
    protected virtual string ComposeUpdate(
      string dbObjectName, IEnumerable<CxAttributeMetadata> attributes, string whereClause)
    {
      ArrayList paramNames = new ArrayList();
      StringBuilder sb = new StringBuilder();
      sb.Append("UPDATE " + dbObjectName + "\r\n");
      sb.Append("   SET ");
      foreach (CxAttributeMetadata attribute in attributes)
      {
        if (attribute.Storable && (Metadata.IsPrimaryKeyUpdated || !attribute.PrimaryKey))
        {
          if (paramNames.Count > 0)
          {
            sb.Append(",\r\n       ");
          }
          sb.Append(attribute.Id + " = :" + attribute.Id);
          paramNames.Add(attribute.Id);
        }
      }
      sb.Append("\r\n");
      sb.Append(" WHERE ");
      sb.Append(whereClause);
      return sb.ToString();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Composes SQL DELETE statement to for entity.
    /// </summary>
    public string ComposeDelete()
    {
      if (CxUtils.NotEmpty(Metadata.SqlDelete))
      {
        return Metadata.SqlDelete;
      }
      else
      {
        return ComposeDelete(Metadata.DbObject);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Composes SQL DELETE statement to for entity.
    /// </summary>
    /// <param name="dbObjectIndex">database object index</param>
    /// <returns>delete SQL statement</returns>
    public string ComposeDelete(int dbObjectIndex)
    {
      return ComposeDelete(Metadata.GetDbObject(dbObjectIndex));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Composes SQL DELETE statement to for entity.
    /// </summary>
    /// <param name="dbObjectName">database table or view name</param>
    /// <returns>delete SQL statement</returns>
    protected virtual string ComposeDelete(string dbObjectName)
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("DELETE");
      sb.Append("  FROM " + dbObjectName + "\r\n");
      sb.Append(" WHERE ");
      string pkWhere = Metadata.ComposePKCondition();
      sb.Append(pkWhere);
      return sb.ToString();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Inserts entity (and all "owned" child) entities to the database.
    /// </summary>
    /// <param name="connection">connection an INSERT should work in context of</param>
    virtual public void Insert(CxDbConnection connection)
    {
      ComposeAndExecuteSQL(connection, ComposeInsert);
      DoAfterInsert(connection);
      InsertExtraDbObjects(connection);
      InsertEntitiesBeforeChildren(connection);
      InsertChildren(connection);
      Metadata.Holder.ClearEntityIdCache(Metadata.EntityId);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Updates entity (and all "owned" child entities) in the database.
    /// </summary>
    /// <param name="connection">connection an UPDATE should work in context of</param>
    virtual public void Update(CxDbConnection connection)
    {
      DeleteChildren(connection);
      ComposeAndExecuteSQL(connection, ComposeUpdate);
      UpdateExtraDbObjects(connection);
      InsertEntitiesBeforeChildren(connection);
      InsertChildren(connection);
      UpdateChildren(connection);
      ApplyUnchangedChildrenChildUpdates(connection);
      Metadata.Holder.ClearEntityIdCache(Metadata.EntityId);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Deletes entity (and all "owned" child entities) from the database.
    /// </summary>
    /// <param name="connection">connection an DELETE should work in context of</param>
    virtual public void Delete(CxDbConnection connection)
    {
      ComposeAndExecuteSQL(connection, ComposeDelete);
      Metadata.Holder.ClearEntityIdCache(Metadata.EntityId);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Does actions after entity is inserted into database.
    /// </summary>
    /// <param name="connection">database connection</param>
    virtual protected void DoAfterInsert(CxDbConnection connection)
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Inserts data into the extra DB objects (if present).
    /// </summary>
    /// <param name="connection">database connection</param>
    virtual protected void InsertExtraDbObjects(CxDbConnection connection)
    {
      if (Metadata.DbObjectCount > 1)
      {
        for (int i = 1; i < Metadata.DbObjectCount; i++)
        {
          CxAttributeMetadata[] attributes = Metadata.GetDbObjectAttributes(i);
          bool doInsert = false;
          foreach (CxAttributeMetadata attribute in attributes)
          {
            if (!IsEmpty(this[attribute.Id]))
            {
              doInsert = true;
              break;
            }
          }
          if (doInsert)
          {
            ExecuteSQL(connection, ComposeInsert(i));
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Updates data in the extra DB objects (if present).
    /// </summary>
    /// <param name="connection">database connection</param>
    virtual protected void UpdateExtraDbObjects(CxDbConnection connection)
    {
      if (Metadata.DbObjectCount > 1)
      {
        for (int i = 1; i < Metadata.DbObjectCount; i++)
        {
          CxAttributeMetadata[] attributes = Metadata.GetDbObjectAttributes(i);

          bool isModified = false;
          bool isEmptyInDb = true;
          bool isEmptyInMemory = true;
          foreach (CxAttributeMetadata attribute in attributes)
          {
            if (m_OriginalEntity != null)
            {
              if (!CompareValues(this[attribute.Id], m_OriginalEntity[attribute.Id]))
              {
                isModified = true;
              }
              if (!IsEmpty(m_OriginalEntity[attribute.Id]))
              {
                isEmptyInDb = false;
              }
            }
            if (!IsEmpty(this[attribute.Id]))
            {
              isEmptyInMemory = false;
            }
          }
          if (m_OriginalEntity == null)
          {
            isModified = true;
          }

          if (isModified)
          {
            if (isEmptyInMemory)
            {
              ExecuteSQL(connection, ComposeDelete(i));
            }
            else
            {
              if (isEmptyInDb)
              {
                try
                {
                  ExecuteSQL(connection, ComposeInsert(i));
                }
                catch (ExDbException)
                {
                  ExecuteSQL(connection, ComposeUpdate(i));
                }
              }
              else
              {
                ExecuteSQL(connection, ComposeUpdate(i));
              }
            }
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns names and values corresponding to the given array of attributes.
    /// </summary>
    /// <param name="attrArray">attributes to get names and values</param>
    /// <param name="names">output array of names</param>
    /// <param name="values">output array of values</param>
    protected void GetAttributeNamesAndValues(
      CxAttributeMetadata[] attrArray,
      out string[] names,
      out object[] values)
    {
      int count = attrArray.Length;
      names = new string[count];
      values = new object[count];
      for (int i = 0; i < count; i++)
      {
        CxAttributeMetadata attribute = attrArray[i];
        names[i] = attribute.Id;
        values[i] = this[attribute.Id];
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns names of primary key attributes and their values.
    /// </summary>
    /// <param name="pkNames">array with names of primary key attributes</param>
    /// <param name="pkValues">array with values of primary key attributes</param>
    public void GetPrimaryKeyValues(out string[] pkNames, out object[] pkValues)
    {
      GetAttributeNamesAndValues(Metadata.PrimaryKeyAttributes, out pkNames, out pkValues);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns names of alternative key attributes and their values.
    /// </summary>
    /// <param name="pkNames">array with names of alternative key attributes</param>
    /// <param name="pkValues">array with values of alternative key attributes</param>
    public void GetAlternativeKeyValues(out string[] pkNames, out object[] pkValues)
    {
      GetAttributeNamesAndValues(Metadata.AlternativeKeyAttributes, out pkNames, out pkValues);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// true if entity does not exists in the database.
    /// </summary>
    public bool IsNew
    {
      get { return m_IsNew; }
      set { m_IsNew = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if entity exists in the database but was updated.
    /// </summary>
    public bool IsUpdated
    {
      get { return m_IsUpdated; }
      set
      {
        m_IsUpdated = value;
        if (m_IsUpdated) SaveValues();
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if entity exists in the database but was deleted.
    /// </summary>
    public bool IsDeleted
    {
      get { return m_IsDeleted; }
      set { m_IsDeleted = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns internal ID of entity.
    /// </summary>
    /// <returns>internal ID of entity</returns>
    virtual public string GetInternalId()
    {
      bool emptyPK = true;
      string[] pkNames;
      object[] pkValues;
      GetPrimaryKeyValues(out pkNames, out pkValues);
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < pkNames.Length; i++)
      {
        sb.AppendFormat("{0}={1};", pkNames[i], pkValues[i]);
        emptyPK &= CxUtils.IsEmpty(pkValues[i]);
      }
      return (emptyPK ? m_Guid : CxText.TrimSuffix(sb.ToString(), ";"));
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Finds data row that corresponds to the entity.
    /// </summary>
    /// <param name="dt">data table to find row in</param>
    /// <returns>data row that corresponds to the given entity</returns>
    virtual public DataRow FindDataRow(DataTable dt)
    {
      if (m_DataRow != null &&
          m_DataRow.Table == dt &&
          m_DataRow.RowState != DataRowState.Detached)
      {
        CxBaseEntity entity2 = CreateByDataRow(Metadata, m_DataRow);
        if (CompareByPK(entity2))
        {
          return m_DataRow;
        }
      }
      foreach (DataRow dr in dt.Rows)
      {
        if (CompareByPK(dr))
        {
          return dr;
        }
      }
      return null;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Finds data row that corresponds to the entity.
    /// </summary>
    /// <param name="dataSource">data source to find row in</param>
    /// <returns>data row that corresponds to the given entity</returns>
    virtual public DataRow FindDataRow(IxGenericDataSource dataSource)
    {
      if (m_DataRow != null &&
          m_DataRow is CxGenericDataRow &&
          dataSource.DoesRowBelongToDataSource((CxGenericDataRow) m_DataRow) &&
          m_DataRow.RowState != DataRowState.Detached)
      {
        CxBaseEntity entity2 = CreateByDataRow(Metadata, m_DataRow);
        if (CompareByPK(entity2))
        {
          return m_DataRow;
        }
      }

      int rowIndex = dataSource.FindByKey(PrimaryKeyInfo);
      if (rowIndex == -1)
        rowIndex = dataSource.FindByKey(AlternativeKeyInfo);
      if (rowIndex > -1)
        return dataSource[rowIndex];
      else
        return null;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Sets the given data-row as the data-row backing the current entity.
    /// </summary>
    /// <param name="dataRow">the data-row to set</param>
    public void SetDataRow(DataRow dataRow)
    {
      if (dataRow != DataRow)
      {
        if (m_DataRow is CxGenericDataRow)
        {
          ((CxGenericDataRow) m_DataRow).Tag = null;
        }
        DataRow = dataRow;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Compares this entity with another one by values of primary key attributes.
    /// </summary>
    /// <param name="entity">entity to compare this one with</param>
    /// <returns>true if values of primary key attributes of this entity
    /// are equal with ones of the given entity</returns>
    virtual public bool CompareByPK(CxBaseEntity entity)
    {
      if (entity == null)
      {
        return false;
      }
      string[] pkNames;
      object[] pkValues1;
      object[] pkValues2;
      GetPrimaryKeyValues(out pkNames, out pkValues1);
      entity.GetPrimaryKeyValues(out pkNames, out pkValues2);
      if ((IsPkEmpty || entity.IsPkEmpty) &&
          Metadata.IsAlternativeKeyDefined && entity.Metadata.IsAlternativeKeyDefined)
      {
        GetAlternativeKeyValues(out pkNames, out pkValues1);
        entity.GetAlternativeKeyValues(out pkNames, out pkValues2);
      }
      for (int i = 0; i < pkValues1.Length && i < pkValues2.Length; i++)
      {
        if (!CompareValues(pkValues1[i], pkValues2[i]))
        {
          return false;
        }
      }
      return true;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Compares this entity with data row by values of primary key attributes.
    /// </summary>
    /// <param name="row">data row to compare with</param>
    /// <returns>true if values of primary key attributes of this entity
    /// are equal with data row values</returns>
    virtual public bool CompareByPK(DataRow row)
    {
      if (row == null)
      {
        return false;
      }
      string[] pkNames;
      object[] pkValues;
      GetPrimaryKeyValues(out pkNames, out pkValues);
      if (IsPkEmpty && Metadata.IsAlternativeKeyDefined)
      {
        GetAlternativeKeyValues(out pkNames, out pkValues);
      }
      for (int i = 0; i < pkNames.Length && i < pkValues.Length; i++)
      {
        object value = CxData.GetValue(row, pkNames[i]);
        if (!CompareValues(pkValues[i], value))
        {
          return false;
        }
      }
      return true;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns entity display name.
    /// </summary>
    public virtual string ToString(CxEntityUsageMetadata entityUsage, string purpose)
    {
      return CxText.Append(entityUsage.SingleCaption, DisplayName, ": ");
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns entity display name.
    /// </summary>
    override public string ToString()
    {
      return ToString(Metadata, string.Empty);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns entity display name.
    /// </summary>
    /// <param name="inShortenedFormat">if true, returns a shortened form of entity display name</param>
    /// <returns>entity display name</returns>
    public virtual string ToString(bool inShortenedFormat)
    {
      if (inShortenedFormat)
        return DisplayName;
      return ToString();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns the display name of the entity which may depend on the purpose provided.
    /// </summary>
    /// <param name="purpose">purpose of the caption requested</param>
    /// <returns>the display name of the entity, may depend on the purpose provided</returns>
    public string ToString(string purpose)
    {
      return ToString(Metadata, purpose);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns title for definition views (dialog, widget, etc.)
    /// </summary>
    /// <returns>title for definition views (dialog, widget, etc.)</returns>
    override public string GetViewTitle()
    {
      string newText = Metadata.Holder.GetTxt("New", "CxBaseEntity.NewText");
      return IsNew ? newText + " " + Metadata.SingleCaption : ToString(CxEntityDisplayNamePurposes.ViewTitle);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if data should be written to the database on ApplyUpdate().
    /// </summary>
    public bool WriteToDb
    {
      get { return m_WriteToDb; }
      set { m_WriteToDb = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// For each entity in entity list saves changes to database.
    /// </summary>
    /// <param name="connection">connection to write changes to</param>
    /// <param name="entities">entity to write</param>
    static public void WriteChangesToDb(CxDbConnection connection, IList<CxBaseEntity> entities)
    {
      bool ownsTransaction = !connection.InTransaction;
      if (ownsTransaction)
      {
        foreach (CxBaseEntity entity in entities)
        {
          entity.DoBeforeTransaction(connection);
        }
        connection.BeginTransaction();
      }
      try
      {
        foreach (CxBaseEntity entity in entities)
        {
          entity.PerformDmlOperation(connection);
        }
        if (ownsTransaction)
        {
          connection.Commit();
          foreach (CxBaseEntity entity in entities)
          {
            entity.DoAfterTransactionCommit(connection);
          }
        }
      }
      catch (Exception e)
      {
        if (ownsTransaction)
        {
          connection.Rollback();
          foreach (CxBaseEntity entity in entities)
          {
            entity.DoAfterTransactionRollback(connection);
          }
        }
        throw new ExIncapsulatedException(e);
      }
      foreach (CxBaseEntity entity in entities)
      {
        CxBaseEntity parentEntity = entity.Parent;
        if (parentEntity != null)
        {
          parentEntity.DoAfterChildEntityIsSyncedWithDb(entity);
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// The method is called after any child entity to the current one was somehow synced with
    /// the database. For instance, that entity has been re-read from the database, or 
    /// just saved directly there.
    /// </summary>
    /// <param name="childEntity">child entity object</param>
    protected void DoAfterChildEntityIsSyncedWithDb(CxBaseEntity childEntity)
    {
      // We remove the child entity from the collections of changed entities,
      // so that the entity won't be written to the database again.
      CxChildEntityUsageMetadata childMetadata =
        Metadata.GetChildEntityUsageOrInheritedByEntityUsageId(childEntity.Metadata.Id);
      if (childMetadata != null)
      {
        if (m_InsertedChildEntities.ContainsKey(childMetadata))
        {
          IList<CxBaseEntity> insertedEntities = m_InsertedChildEntities[childMetadata];
          int index = childEntity.FindInListByPK(insertedEntities);
          if (index > -1)
            insertedEntities.RemoveAt(index);
        }
        if (m_UpdatedChildEntities.ContainsKey(childMetadata))
        {
          IList<CxBaseEntity> updatedEntities = m_UpdatedChildEntities[childMetadata];
          int index = childEntity.FindInListByPK(updatedEntities);
          if (index > -1)
            updatedEntities.RemoveAt(index);
        }
        if (m_InitialChildEntities.ContainsKey(childMetadata))
        {
          IList<CxBaseEntity> initialEntities = m_InitialChildEntities[childMetadata];
          int index = childEntity.FindInListByPK(initialEntities);
          if (index > -1)
          {
            CxBaseEntity initialEntity = initialEntities[index];
            initialEntity.ReadValueProvider(childEntity);
          }
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Writes entity changes to the database.
    /// </summary>
    /// <param name="connection">connection to use to write changes</param>
    virtual public void WriteChangesToDb(CxDbConnection connection)
    {
      WriteChangesToDb(connection, new CxBaseEntity[] { this });
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Does necessary action before transaction opening.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    virtual public void DoBeforeTransaction(CxDbConnection connection)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Does necessary action after transaction comitted.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    virtual public void DoAfterTransactionCommit(CxDbConnection connection)
    {
      EntitiesToInsertBeforeChildren.Clear();
      ResetChildEntityChanges();
      ResetChildEntityErrorData();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Does necessary action after transaction rolled back.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    virtual public void DoAfterTransactionRollback(CxDbConnection connection)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Re-reads entity data from the database.
    /// Finds entity record by primary key or alternative key.
    /// If entity with such PK or AK not found returns false.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    virtual public bool ReadFromDb(CxDbConnection connection)
    {
      DataRow dataRow = null;
      if (!IsPkEmpty)
      {
        dataRow = GetEntityDataRow(Metadata, connection, this);
      }
      else if (Metadata.IsAlternativeKeyDefined)
      {
        dataRow = Metadata.ReadDataRow(connection, Metadata.ComposeAKCondition(), this);
      }
      if (dataRow != null)
      {
        ReadDataRow(dataRow);
        SetDataRow(dataRow);

        SetPermanentCalculatedValues(connection);
        m_OldProperties.Clear();

        // Notify parent so that it may react on such action. 
        if (Parent != null)
        {
          Parent.DoAfterChildEntityIsSyncedWithDb(this);
        }
        return true;
      }
      else
      {
        return false;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads an array of entities of the given entity usage.
    /// </summary>
    /// <param name="connection">a database connection</param>
    /// <param name="entityUsage">an entity usage</param>
    /// <returns>an array of entities</returns>
    public static CxBaseEntity[] ReadEntities(
      CxDbConnection connection,
      CxEntityUsageMetadata entityUsage)
    {
      return ReadEntities(connection, entityUsage, null, null);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads an array of entities of the given entity usage.
    /// </summary>
    /// <param name="connection">a database connection</param>
    /// <param name="entityUsage">an entity usage</param>
    /// <param name="where">where clause (filter condition)</param>
    /// <param name="valueProvider">a provider for parameter values</param>
    /// <returns>an array of entities</returns>
    public static CxBaseEntity[] ReadEntities(
      CxDbConnection connection,
      CxEntityUsageMetadata entityUsage,
      string where,
      IxValueProvider valueProvider)
    {
      List<CxBaseEntity> entityList = new List<CxBaseEntity>();

      CxGenericDataTable table = new CxGenericDataTable();
      entityUsage.ReadData(connection, table, where, valueProvider);
      foreach (DataRow row in table.Rows)
      {
        entityList.Add(CreateByDataRow(entityUsage, row));
      }
      return entityList.ToArray();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads an array of entities of the given entity usage.
    /// </summary>
    /// <param name="connection">a database connection</param>
    /// <param name="entityUsage">an entity usage</param>
    /// <param name="where">where clause (filter condition)</param>
    /// <param name="valueProvider">a provider for parameter values</param>
    /// <param name="startRecordIndex">record index to start reading from</param>
    /// <param name="recordsAmount">amount of records to read</param>
    /// <returns>an array of entities</returns>
    public static CxBaseEntity[] ReadEntities(
      CxDbConnection connection,
      CxEntityUsageMetadata entityUsage,
      string where,
      IxValueProvider valueProvider,
      int startRecordIndex,
      int recordsAmount)
    {
      return ReadEntities(
        connection,
        entityUsage,
        where,
        valueProvider,
        startRecordIndex,
        recordsAmount,
        new CxSortDescriptorList());
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads an array of entities of the given entity usage.
    /// </summary>
    /// <param name="connection">a database connection</param>
    /// <param name="entityUsage">an entity usage</param>
    /// <param name="where">where clause (filter condition)</param>
    /// <param name="valueProvider">a provider for parameter values</param>
    /// <param name="startRecordIndex">record index to start reading from</param>
    /// <param name="recordsAmount">amount of records to read</param>
    /// <param name="sortings">a set of sort conditions to be applied for reading</param>
    /// <returns>an array of entities</returns>
    public static CxBaseEntity[] ReadEntities(
      CxDbConnection connection,
      CxEntityUsageMetadata entityUsage,
      string where,
      IxValueProvider valueProvider,
      int startRecordIndex,
      int recordsAmount,
      CxSortDescriptorList sortings)
    {
      List<IxEntity> entities = new List<IxEntity>(entityUsage.ReadEntities(
        connection, where, valueProvider, startRecordIndex, recordsAmount, sortings));
      return entities.ConvertAll<CxBaseEntity>(
        delegate(IxEntity entity)
        {
          return (CxBaseEntity) entity;
        }).ToArray();
    }
    //----------------------------------------------------------------------------
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
    public virtual IxEntity[] ReadEntities(
      CxDbConnection connection,
      string where,
      IxValueProvider valueProvider,
      int startRecordIndex = -1,
      int recordsAmount = -1,
      CxSortDescriptorList sortings = null)
    {
      List<CxBaseEntity> entityList = new List<CxBaseEntity>();

      //new string[] { entityUsage.OrderByClause }
      List<string> orderByClauses = new List<string>();
      if (sortings != null)
        orderByClauses.Add(connection.ScriptGenerator.GetOrderByClause(sortings));
      string completeOrderByClause = string.Join(",", orderByClauses.ToArray());

      CxGenericDataTable table = new CxGenericDataTable();
      Metadata.ReadData(
        connection, table, where, valueProvider,
        completeOrderByClause, startRecordIndex,
        recordsAmount);
      foreach (DataRow row in table.Rows)
      {
        entityList.Add(CreateByDataRow(Metadata, row));
      }
      return entityList.ToArray();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads an entity of the given entity usage and with the given entity PK.
    /// </summary>
    /// <param name="connection">a database connection</param>
    /// <param name="where">where clause (filter condition)</param>
    /// <param name="valueProvider">a provider for parameter values, including PK</param>
    /// <returns>the entity read</returns>
    public virtual IxEntity ReadEntity(
      CxDbConnection connection,
      string where,
      IxValueProvider valueProvider)
    {
      DataRow row = Metadata.ReadDataRow(connection, where, valueProvider);
      return row != null ? CreateByDataRow(Metadata, row) : null;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns the overall amount of entities (not taking into account the record
    /// frame probably used).
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="entityUsage">entity usage</param>
    /// <param name="where">where clause</param>
    /// <param name="valueProvider">value provider for parameters</param>
    /// <returns>amount of entities available</returns>
    public static int ReadEntityAmount(
      CxDbConnection connection,
      CxEntityUsageMetadata entityUsage,
      string where,
      IxValueProvider valueProvider)
    {
      return entityUsage.ReadDataRowAmount(connection, where, valueProvider);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns the overall amount of entities taking into account
    /// the existing parent-child relationships 
    /// (not taking into account the record frame probably used).
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="entityUsage">entity usage</param>
    /// <param name="where">where clause</param>
    /// <param name="valueProvider">value provider for parameters</param>
    /// <returns>amount of entities available</returns>
    public static int ReadChildEntityAmount(
      CxDbConnection connection,
      CxEntityUsageMetadata entityUsage,
      string where,
      IxValueProvider valueProvider)
    {
      return entityUsage.ReadChildDataRowAmount(connection, where, valueProvider);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Writes entity changes to the database.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    virtual public void PerformDmlOperation(CxDbConnection connection)
    {
      ResetChildEntityErrorData();

      NxDmlOperation operation = GetDmlOperation();

      if (operation == NxDmlOperation.Insert)
      {
        Insert(connection);
      }
      else if (operation == NxDmlOperation.Update)
      {
        if (Metadata.IsInsertOnUpdate)
        {
          Insert(connection);
        }
        else
        {
          Update(connection);
        }
      }
      else if (operation == NxDmlOperation.Delete)
      {
        Delete(connection);
      }

      m_OriginalEntity = null;

      if (IsRefreshAfterInsertOrUpdatePerformed &&
          (operation == NxDmlOperation.Insert || operation == NxDmlOperation.Update))
      {
        DataRow dataRow = GetEntityDataRow(Metadata, connection, this);
        if (dataRow != null)
        {
          ReplaceDataRowData(dataRow);
        }
        else if (operation == NxDmlOperation.Update && Metadata.MayDisappearAfterUpdate)
        {
          DisappearedAfterUpdate = true;
        }
        else
        {
          string[] pkNames;
          object[] pkValues;
          GetPrimaryKeyValues(out pkNames, out pkValues);
          throw new ExNoDataFoundException(Metadata.SingleCaption, pkNames, pkValues);
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Replaces entity data row data using another data row.
    /// Also refreshes entity attribute values from this data row.
    /// </summary>
    /// <param name="dataRow">data row that contains values to use from now on</param>
    virtual protected void ReplaceDataRowData(DataRow dataRow)
    {
      DataRow oldDataRow = m_DataRow;
      if (m_DataRow != null)
      {
        CxData.CopyDataRow(dataRow, m_DataRow);
      }
      ReadDataRow(dataRow);
      m_DataRow = oldDataRow;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Sets initial state of child entities.
    /// </summary>
    /// <param name="child">child entity usage definiiton</param>
    /// <param name="entityList">list of entities (in initial state)</param>
    virtual public void SetInitialChildren(
      CxChildEntityUsageMetadata child, IList<CxBaseEntity> entityList)
    {
      List<CxBaseEntity> initialEntities = new List<CxBaseEntity>();
      foreach (CxBaseEntity entity in entityList)
      {
        CxBaseEntity copy = entity.Copy();
        initialEntities.Add(copy);
      }
      m_InitialChildEntities[child] = initialEntities;
      m_CurrentChildEntities[child] = entityList;
      m_InsertedChildEntities[child] = new List<CxBaseEntity>();
      m_UpdatedChildEntities[child] = new List<CxBaseEntity>();
      m_DeletedChildEntities[child] = new List<CxBaseEntity>();
      m_UnchangedChildEntities[child] = entityList;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Compares initial children entity list with the given one 
    /// and composes inserted, updated and deleted list.
    /// </summary>
    /// <param name="child">child entity usage definition</param>
    /// <param name="entityList">list of entities (in current state)</param>
    virtual public void ApplyChildrenUpdates(CxChildEntityUsageMetadata child, IList<CxBaseEntity> entityList)
    {
      m_CurrentChildEntities[child] = entityList;
      IList<CxBaseEntity> oldList = m_InitialChildEntities.ContainsKey(child) ? m_InitialChildEntities[child] : null;
      IList<CxBaseEntity> newList = entityList;
      IList<CxBaseEntity> inserted = new List<CxBaseEntity>();
      IList<CxBaseEntity> updated = new List<CxBaseEntity>();
      IList<CxBaseEntity> deleted = new List<CxBaseEntity>();
      IList<CxBaseEntity> unchanged = new List<CxBaseEntity>();

      // Form the new entities dictionary.
      Dictionary<string, CxBaseEntity> newDictionary = new Dictionary<string, CxBaseEntity>();
      foreach (CxBaseEntity newEntity in newList)
      {
        string pk = null;
        if (newEntity.IsPkEmpty)
        {
          if (newEntity.Metadata.IsAlternativeKeyDefined)
          {
            pk = newEntity.AlternativeKeyAsString;
          }
        }
        else
        {
          pk = newEntity.PrimaryKeyAsString;
        }
        if (string.IsNullOrEmpty(pk))
          throw new ExException(string.Format("An error while applying updates to children <{0}> occured: cannot determine the primary key of one of the records", child.Id));
        if (newDictionary.ContainsKey(pk))
          throw new ExException("The entity with the same primary key was encountered. This probably caused by a non-unique primary key value");
        newDictionary.Add(pk, newEntity);
      }
      // Form the old entities dictionary.
      Dictionary<string, CxBaseEntity> oldDictionary = new Dictionary<string, CxBaseEntity>();
      if (oldList != null)
      {
        foreach (CxBaseEntity oldEntity in oldList)
        {
          string pk = null;
          if (oldEntity.IsPkEmpty)
          {
            if (oldEntity.Metadata.IsAlternativeKeyDefined)
            {
              pk = oldEntity.AlternativeKeyAsString;
            }
          }
          else
          {
            pk = oldEntity.PrimaryKeyAsString;
          }
          if (string.IsNullOrEmpty(pk))
            throw new ExException(string.Format("An error while applying updates to children <{0}> occured: cannot determine the primary key of one of the records", child.Id));
          oldDictionary.Add(pk, oldEntity);
        }
      }

      // Perform comparison to clear up if there some deleted or added entities available.
      foreach (KeyValuePair<string, CxBaseEntity> pairNew in newDictionary)
      {
        if (!oldDictionary.ContainsKey(pairNew.Key))
        {
          inserted.Add(pairNew.Value);
        }
        else if (!pairNew.Value.Compare(oldDictionary[pairNew.Key]))
        {
          updated.Add(pairNew.Value);
          pairNew.Value.OriginalEntity = oldDictionary[pairNew.Key];
        }
        else
        {
          unchanged.Add(pairNew.Value);
        }
      }

      foreach (KeyValuePair<string, CxBaseEntity> pairOld in oldDictionary)
      {
        if (!newDictionary.ContainsKey(pairOld.Key))
        {
          deleted.Add(pairOld.Value);
        }
      }

      m_InsertedChildEntities[child] = inserted;
      m_UpdatedChildEntities[child] = updated;
      m_DeletedChildEntities[child] = deleted;
      m_UnchangedChildEntities[child] = unchanged;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Compares initial children entity list with the given one.
    /// </summary>
    /// <param name="child">child entity usage definition</param>
    /// <param name="entityList">list of entities (in current state)</param>
    /// <returns>true if children entity list is equal to original one or false otherwise</returns>
    virtual public bool CompareChildren(
      CxChildEntityUsageMetadata child, IList<CxBaseEntity> entityList)
    {
      if (!m_InitialChildEntities.ContainsKey(child))
        throw new ExException("Cannot find the child entity usage key in the dictionary");

      IList<CxBaseEntity> oldList = m_InitialChildEntities[child];
      IList<CxBaseEntity> newList = entityList;

      foreach (CxBaseEntity newEntity in newList)
      {
        int i = newEntity.FindInListByPK(oldList);
        if (i == -1)
        {
          return false;
        }
        else
        {
          if (!newEntity.Compare(oldList[i]))
            return false;
        }
      }
      foreach (CxBaseEntity oldEntity in oldList)
      {
        if (oldEntity.FindInListByPK(newList) == -1)
        {
          return false;
        }
      }
      return true;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Finds if this entity exists in the entity list (making primary key comparison).
    /// </summary>
    /// <param name="entityList">list of entities to find this in</param>
    /// <returns>index of this entity in the list or -1 if not found</returns>
    virtual public int FindInListByPK(IList<CxBaseEntity> entityList)
    {
      for (int i = 0; i < entityList.Count; i++)
      {
        CxBaseEntity entity = entityList[i];
        if (entity.CompareByPK(this))
        {
          return i;
        }
      }
      return -1;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Inserts child entities to the database.
    /// </summary>
    /// <param name="connection">Db connection an UPDATE should work in context of</param>
    virtual public void InsertChildren(CxDbConnection connection)
    {
      List<CxChildEntityUsageMetadata> children = new List<CxChildEntityUsageMetadata>();
      children.AddRange(CxList.ToList<CxChildEntityUsageMetadata>(Metadata.ChildEntityUsagesList));
      CxList.AddIfNotInList(children, m_InsertedChildEntities.Keys);
      foreach (CxChildEntityUsageMetadata child in children)
      {
        if (m_InsertedChildEntities.ContainsKey(child))
        {
          IList<CxBaseEntity> inserted = m_InsertedChildEntities[child];
          foreach (CxBaseEntity entity in inserted)
          {
            try
            {
              entity.Insert(connection);
            }
            catch
            {
              SetChildEntityErrorData(child, entity);
              throw;
            }
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Updates child entities in the database.
    /// </summary>
    /// <param name="connection">Db connection an UPDATE should work in context of</param>
    virtual public void UpdateChildren(CxDbConnection connection)
    {
      List<CxChildEntityUsageMetadata> children = new List<CxChildEntityUsageMetadata>();
      children.AddRange(CxList.ToList<CxChildEntityUsageMetadata>(Metadata.ChildEntityUsagesList));
      CxList.AddIfNotInList(children, m_UpdatedChildEntities.Keys);
      foreach (CxChildEntityUsageMetadata child in children)
      {
        if (m_UpdatedChildEntities.ContainsKey(child))
        {
          IList<CxBaseEntity> updated = m_UpdatedChildEntities[child];
          foreach (CxBaseEntity entity in updated)
          {
            try
            {
              entity.Update(connection);
            }
            catch
            {
              SetChildEntityErrorData(child, entity);
              throw;
            }
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Delete child entities in the database.
    /// </summary>
    /// <param name="connection">Db connection a DELETE should work in context of</param>
    virtual public void DeleteChildren(CxDbConnection connection)
    {
      List<CxChildEntityUsageMetadata> children = new List<CxChildEntityUsageMetadata>();
      children.AddRange(CxList.ToList<CxChildEntityUsageMetadata>(Metadata.ChildEntityUsagesList));
      CxList.AddIfNotInList(children, m_DeletedChildEntities.Keys);
      for (int i = children.Count - 1; i >= 0; i--)
      {
        if (m_DeletedChildEntities.ContainsKey(children[i]))
        {
          IList<CxBaseEntity> deleted = m_DeletedChildEntities[children[i]];
          foreach (CxBaseEntity entity in deleted)
          {
            try
            {
              entity.Delete(connection);
            }
            catch
            {
              SetChildEntityErrorData(children[i], entity);
              throw;
            }
          }
        }
      }
      DeleteEntitiesAfterChildren(connection);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Applies updates to the unchanged children child entities (if exists).
    /// </summary>
    /// <param name="connection">Db connection should work in context of</param>
    virtual public void ApplyUnchangedChildrenChildUpdates(CxDbConnection connection)
    {
      List<CxChildEntityUsageMetadata> children = new List<CxChildEntityUsageMetadata>();
      children.AddRange(CxList.ToList<CxChildEntityUsageMetadata>(Metadata.ChildEntityUsagesList));
      CxList.AddIfNotInList(children, m_UnchangedChildEntities.Keys);
      foreach (CxChildEntityUsageMetadata child in children)
      {
        if (m_UnchangedChildEntities.ContainsKey(child))
        {
          IList<CxBaseEntity> unchanged = m_UnchangedChildEntities[child];
          foreach (CxBaseEntity entity in unchanged)
          {
            entity.DeleteChildren(connection);
            entity.InsertChildren(connection);
            entity.UpdateChildren(connection);
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns DML operation for the saving entity into the database.
    /// </summary>
    /// <returns>DML operation for the saving entity into the database</returns>
    virtual public NxDmlOperation GetDmlOperation()
    {
      return m_IsDeleted ? NxDmlOperation.Delete :
                           (m_IsNew ? NxDmlOperation.Insert : NxDmlOperation.Update);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns "before update" value of the property with the given name.
    /// </summary>
    /// <param name="name">name of the property to return</param>
    virtual protected object GetOldValue(string name)
    {
      return (m_OldProperties.ContainsKey(name) ? m_OldProperties[name] : this[name]);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets "before update" value of the property with the given name.
    /// </summary>
    /// <param name="name">name of the property to set</param>
    /// <param name="value">value to assign to</param>
    virtual protected void SetOldValue(string name, object value)
    {
      m_OldProperties[name] = value;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Saves current properties values to old ones.
    /// </summary>
    virtual protected void SaveValues()
    {
      foreach (string name in m_Properties.Keys)
      {
        m_OldProperties[name] = m_Properties[name];
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Compares two entities by all property values.
    /// </summary>
    /// <param name="entity">entity to compare with this one</param>
    /// <returns>true if entities are equal or false otherwise</returns>
    virtual public bool Compare(CxBaseEntity entity)
    {
      if (entity == null)
      {
        return false;
      }

      if (Metadata != entity.Metadata)
      {
        throw new ExInternalException(string.Format("Entity usages of entities to compare are not the same (<{0}> and <{1}>)", Metadata.Id, entity.Metadata.Id));
      }

      foreach (string name in AllProperties)
      {
        if (Metadata.GetAttribute(name).Incomparable) continue;
        object value1 = this[name];
        object value2 = entity[name];
        if (!CompareValues(value1, value2))
        {
          return false;
        }
      }
      return true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates and returns filter operator for the given filter element.
    /// </summary>
    /// <param name="element">filter element</param>
    /// <returns>created filter operator or null</returns>
    virtual protected CxFilterOperator CreateFilterOperator(IxFilterElement element)
    {
      return CxFilterOperator.Create(Metadata, element);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns WHERE condition composed from the given filter elements. 
    /// </summary>
    /// <param name="filterElements">list of IxFilterElement objects</param>
    /// <returns>composed WHERE clause</returns>
    virtual public string GetFilterCondition(IList<IxFilterElement> filterElements)
    {
      string where = "";
      if (filterElements != null && Metadata.IsFilterConditionAutoGenerated)
      {
        foreach (IxFilterElement element in filterElements)
        {
          CxFilterOperator filterOperator = CreateFilterOperator(element);
          if (filterOperator != null)
          {
            string wherePart = filterOperator.GetCondition();
            if (CxUtils.NotEmpty(wherePart))
            {
              where += (where != "" ? " AND " : "") + wherePart;
            }
          }
        }
      }
      return where;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns value provider to get WHERE condition parameters. 
    /// </summary>
    /// <param name="filterElements">list of IxFilterElement objects</param>
    /// <returns>instance of IxValueProvider object</returns>
    virtual public IxValueProvider GetFilterValueProvider(IList<IxFilterElement> filterElements)
    {
      CxHashtable values = new CxHashtable();
      if (filterElements != null)
      {
        foreach (IxFilterElement element in filterElements)
        {
          CxFilterOperator filterOperator = CreateFilterOperator(element);
          if (filterOperator != null)
          {
            filterOperator.InitializeValueProvider(values);
          }
        }
      }
      return values;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns display text to show filter condition to user.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="filterElements">list of IxFilterElement objects</param>
    virtual public string GetFilterConditionText(
      CxDbConnection connection,
      IList<IxFilterElement> filterElements)
    {
      string text = "";
      if (filterElements != null)
      {
        foreach (IxFilterElement element in filterElements)
        {
          CxFilterOperator filterOperator = CreateFilterOperator(element);
          if (filterOperator != null)
          {
            string textPart = filterOperator.GetDisplayText(connection);
            if (CxUtils.NotEmpty(textPart))
            {
              text += (text != "" ? "; " : "") + textPart;
            }
          }
        }
      }
      return text;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns array of available filter operations for the given attribute.
    /// </summary>
    /// <param name="attribute">attribute metadata</param>
    virtual public NxFilterOperation[] GetAttributeFilterOperations(CxAttributeMetadata attribute)
    {
      return CxFilterElement.GetAttributeFilterOperations(attribute);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns display text for each filter operation.
    /// </summary>
    /// <param name="operation">operation to get text for</param>
    /// <returns>text to display</returns>
    virtual public string GetFilterOperationText(NxFilterOperation operation)
    {
      return CxFilterElement.GetFilterOperationText(Metadata.Holder, operation);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns count of filter values for the given filter operation.
    /// </summary>
    virtual public int GetFilterOperationValueCount(NxFilterOperation operation)
    {
      return CxFilterElement.GetFilterOperationValueCount(operation);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets, if entity refresh should be performed after 
    /// insert or update operation.
    /// </summary>
    static public bool IsRefreshAfterInsertOrUpdatePerformed
    {
      get { return m_IsRefreshAfterInsertOrUpdatePerformed; }
      set { m_IsRefreshAfterInsertOrUpdatePerformed = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity instance display name.
    /// </summary>
    public string DisplayName
    {
      get
      {
        CxAttributeMetadata nameAttr = Metadata.NameAttribute;
        if (nameAttr != null)
        {
          return CxUtils.ToString(this[nameAttr.Id]);
        }
        return "";
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Replaces placeholders with entity properties.
    /// </summary>
    /// <param name="text">text to replace</param>
    /// <param name="metadata">an entity metadata to be used to replace placeholders</param>
    /// <returns>replaced text</returns>
    public string ReplacePlaceholders(
      string text, CxEntityUsageMetadata metadata)
    {
      if (CxUtils.NotEmpty(text))
      {
        text = text.Replace("%entity_name%", DisplayName);
        text = metadata != null ? metadata.ReplacePlaceholders(text) : Metadata.ReplacePlaceholders(text);
      }
      return text;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Replaces placeholders with entity properties.
    /// </summary>
    static public string ReplacePlaceholders(
      CxBaseEntity entity,
      CxEntityUsageMetadata entityUsage,
      string text)
    {
      if (entity != null)
      {
        text = entity.ReplacePlaceholders(text, entityUsage);
      }
      else if (entityUsage != null)
      {
        text = entityUsage.ReplacePlaceholders(text);
      }
      else
      {
        text = CxText.RemovePlaceholders(text, '%');
      }
      return text;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns value of the first primary key field converted to string.
    /// </summary>
    public string PrimaryKeyAsString
    {
      get
      {
        return Metadata.EncodePrimaryKeyValuesAsString(this);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns current entity primary key values.
    /// </summary>
    public CxKeyInfo PrimaryKeyInfo
    {
      get
      {
        CxKeyInfo keyInfo = new CxKeyInfo();
        foreach (CxAttributeMetadata attribute in Metadata.PrimaryKeyAttributes)
          keyInfo[attribute.Id] = this[attribute.Id];
        return keyInfo;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns current entity primary key values.
    /// </summary>
    public CxKeyInfo AlternativeKeyInfo
    {
      get
      {
        CxKeyInfo keyInfo = new CxKeyInfo();
        foreach (CxAttributeMetadata attribute in Metadata.AlternativeKeyAttributes)
          keyInfo[attribute.Id] = this[attribute.Id];
        return keyInfo;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns current entity alternative key values.
    /// </summary>
    public string AlternativeKeyAsString
    {
      get { return Metadata.EncodeAlternativeKeyValuesAsString(this); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns information about DB file/image field.
    /// </summary>
    /// <param name="attrName">entity attribute name</param>
    /// <param name="fileContentEntityUsage">entity usage with file content</param>
    /// <param name="fileContentAttribute">attribute with file content</param>
    /// <param name="entityPkValue">primary key of entity instance with file content</param>
    public void GetDbFileField(
      string attrName,
      out CxEntityUsageMetadata fileContentEntityUsage,
      out CxAttributeMetadata fileContentAttribute,
      out string entityPkValue)
    {
      fileContentEntityUsage = null;
      fileContentAttribute = null;
      entityPkValue = "";

      CxAttributeMetadata attribute = Metadata.GetAttribute(attrName);
      if (attribute != null && attribute.IsDbFile)
      {
        CxAttributeMetadata contentAttr =
          Metadata.GetAttribute(attribute.FileContentAttributeId);
        if (contentAttr != null)
        {
          object contentValue = this[contentAttr.Id];
          if ((contentValue is byte[]) && ((byte[]) contentValue).Length > 0)
          {
            fileContentEntityUsage = Metadata;
            fileContentAttribute = contentAttr;
            entityPkValue = PrimaryKeyAsString;
            return;
          }
        }

        CxAttributeMetadata refAttr =
          Metadata.GetAttribute(attribute.FileLibraryReferenceAttributeId);
        if (refAttr != null)
        {
          CxEntityUsageMetadata fileEntityUsage = attribute.FileLibraryEntityUsage;
          if (fileEntityUsage != null)
          {
            CxAttributeMetadata fileAttr = fileEntityUsage.GetFirstDbFileAttribute();
            string entityPk = CxUtils.ToString(this[refAttr.Id]);
            if (fileAttr != null && CxUtils.NotEmpty(entityPk))
            {
              fileContentEntityUsage = fileEntityUsage;
              fileContentAttribute = fileAttr;
              entityPkValue = entityPk;
            }
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns information about DB file/image field.
    /// </summary>
    /// <param name="attrName">entity attribute name</param>
    /// <param name="connection">database connection</param>
    /// <param name="fileContentEntity">entity with file content</param>
    /// <param name="fileContentAttributeName">attribute name with file content</param>
    public void GetDbFileField(
      string attrName,
      CxDbConnection connection,
      out CxBaseEntity fileContentEntity,
      out string fileContentAttributeName)
    {
      fileContentEntity = null;
      fileContentAttributeName = null;

      CxEntityUsageMetadata fileContentEntityUsage;
      CxAttributeMetadata fileContentAttribute;
      string entityPkValue;
      GetDbFileField(
        attrName,
        out fileContentEntityUsage,
        out fileContentAttribute,
        out entityPkValue);
      if (fileContentEntityUsage != null &&
          fileContentAttribute != null &&
          CxUtils.NotEmpty(entityPkValue))
      {
        if (fileContentEntityUsage == Metadata)
        {
          fileContentEntity = this;
          fileContentAttributeName = fileContentAttribute.Id;
          return;
        }
        else
        {
          fileContentEntity = CreateAndReadFromDb(
            fileContentEntityUsage,
            connection,
            new object[] { entityPkValue });
          if (fileContentEntity != null)
          {
            fileContentAttributeName = fileContentAttribute.Id;
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns value of DB file/image field.
    /// </summary>
    /// <param name="attrName">entity attribute name</param>
    /// <param name="connection">database connection</param>
    public byte[] GetDbFileFieldValue(
      string attrName,
      CxDbConnection connection)
    {
      CxBaseEntity fileContentEntity;
      string fileContentAttributeName;

      GetDbFileField(
        attrName,
        connection,
        out fileContentEntity,
        out fileContentAttributeName);

      if (fileContentEntity != null &&
          CxUtils.NotEmpty(fileContentAttributeName))
      {
        object result = fileContentEntity[fileContentAttributeName];
        if (result is byte[])
        {
          return (byte[]) result;
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets content of the DB file/image field.
    /// </summary>
    /// <param name="attrName">field name</param>
    /// <param name="fieldValue">field value</param>
    public void SetDbFileFieldContent(string attrName, object fieldValue)
    {
      CxAttributeMetadata attribute = Metadata.GetAttribute(attrName);
      if (attribute != null && attribute.IsDbFile)
      {
        CxAttributeMetadata contentAttr =
          Metadata.GetAttribute(attribute.FileContentAttributeId);
        if (contentAttr != null)
        {
          this[contentAttr.Id] = fieldValue;
        }
        CxAttributeMetadata referenceAttr =
          Metadata.GetAttribute(attribute.FileLibraryReferenceAttributeId);
        if (referenceAttr != null)
        {
          this[referenceAttr.Id] = null;
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets content of the DB file/image field.
    /// </summary>
    /// <param name="attrName">field name</param>
    /// <param name="referenceValue">library reference value</param>
    public void SetDbFileFieldReference(string attrName, object referenceValue)
    {
      CxAttributeMetadata attribute = Metadata.GetAttribute(attrName);
      if (attribute != null && attribute.IsDbFile)
      {
        CxAttributeMetadata contentAttr =
          Metadata.GetAttribute(attribute.FileContentAttributeId);
        if (contentAttr != null)
        {
          this[contentAttr.Id] = null;
        }
        CxAttributeMetadata referenceAttr =
          Metadata.GetAttribute(attribute.FileLibraryReferenceAttributeId);
        if (referenceAttr != null)
        {
          this[referenceAttr.Id] = referenceValue;
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true DB file/image field is empty.
    /// </summary>
    /// <param name="attrName">entity attribute name</param>
    public bool IsDbFileFieldEmpty(string attrName)
    {
      CxAttributeMetadata attribute = Metadata.GetAttribute(attrName);
      if (attribute != null && attribute.IsDbFile)
      {
        CxAttributeMetadata contentAttr =
          Metadata.GetAttribute(attribute.FileContentAttributeId);
        if (contentAttr != null)
        {
          object fieldValue = this[contentAttr.Id];
          if ((fieldValue is byte[]) && ((byte[]) fieldValue).Length > 0)
          {
            return false;
          }
        }
        CxAttributeMetadata refAttr =
          Metadata.GetAttribute(attribute.FileLibraryReferenceAttributeId);
        if (refAttr != null)
        {
          object fieldValue = this[refAttr.Id];
          if (CxUtils.NotEmpty(fieldValue))
          {
            return false;
          }
        }
      }
      return true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Calculates an expression by the means of standard DataTable functionality.
    /// </summary>
    /// <param name="expr">expression to calculate</param>
    /// <returns>result of the expression calculation</returns>
    public object CalculateEntityFieldExpression(string expr)
    {
      DataTable dt = new DataTable();
      foreach (CxAttributeMetadata attr in Metadata.Attributes)
      {
        DataColumn dc = new DataColumn(attr.Id, attr.GetPropertyType());
        if (dc.DataType == typeof(string) && attr.Type == CxAttributeMetadata.TYPE_LONGSTRING)
          dc.MaxLength = int.MaxValue;
        dt.Columns.Add(dc);
      }
      DataColumn resultColumn = new DataColumn("__RESULT__", typeof(object), expr);
      dt.Columns.Add(resultColumn);

      DataRow dr = dt.NewRow();
      dt.Rows.Add(dr);

      foreach (CxAttributeMetadata attr in Metadata.Attributes)
      {
        dr[attr.Id] = CxUtils.NotEmpty(this[attr.Id]) ? this[attr.Id] : DBNull.Value;
      }

      return dr["__RESULT__"];
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Copies hashtable containing lists of child entities for each
    /// child entity usage.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    protected CxChildEntitiesDictionary CopyChildEntityTable(
      CxChildEntitiesDictionary source)
    {
      if (source != null)
      {
        CxChildEntitiesDictionary copy = new CxChildEntitiesDictionary(source);
        foreach (CxChildEntityUsageMetadata child in source.Keys)
        {
          copy[child] = new List<CxBaseEntity>(source[child]);
        }
        return copy;
      }
      return new CxChildEntitiesDictionary();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts this entity to the given entity usage.
    /// </summary>
    /// <param name="entityUsage">entity usage to convert to</param>
    /// <returns>converted copy</returns>
    public CxBaseEntity ConvertToEntityUsage(CxEntityUsageMetadata entityUsage)
    {
      if (Metadata.Id == entityUsage.Id)
      {
        return this;
      }
      CxBaseEntity copy = CreateByValueProvider(entityUsage, this);
      copy.Parent = Parent;
      copy.m_InitialChildEntities = CopyChildEntityTable(m_InitialChildEntities);
      copy.m_CurrentChildEntities = CopyChildEntityTable(m_CurrentChildEntities);
      copy.m_InsertedChildEntities = CopyChildEntityTable(m_InsertedChildEntities);
      copy.m_UpdatedChildEntities = CopyChildEntityTable(m_UpdatedChildEntities);
      copy.m_DeletedChildEntities = CopyChildEntityTable(m_DeletedChildEntities);
      copy.m_UnchangedChildEntities = CopyChildEntityTable(m_UnchangedChildEntities);
      copy.IsNew = IsNew;
      return copy;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds and loads parent entity.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="entityToFind">entity usage metadata to find</param>
    /// <param name="returnThis">if true and this entity matches to 
    /// the search entity returns this entity</param>
    /// <param name="lookThruDescendantsFirst">if true the parent entity 
    /// will be searched for in the actual parent's descendants first</param>
    public CxBaseEntity GetParentEntity(
      CxDbConnection connection,
      CxEntityUsageMetadata entityToFind,
      bool returnThis,
      bool lookThruDescendantsFirst)
    {
      // If this entity match, return this entity
      if (returnThis && Metadata.Id == entityToFind.Id)
      {
        return this;
      }
      // Try to find needed entity in the parent instance chain by the direct match.
      CxBaseEntity parentEntity = Parent;
      IList<CxEntityUsageMetadata> inheritedEntityUsages = null;
      if (lookThruDescendantsFirst)
        inheritedEntityUsages = entityToFind.GetDirectlyInheritedEntityUsages();

      while (parentEntity != null)
      {
        if (lookThruDescendantsFirst)
        {
          foreach (CxEntityUsageMetadata entityUsage in inheritedEntityUsages)
          {
            if (parentEntity.Metadata.Id == entityUsage.Id)
              return parentEntity;
          }
        }
        if (parentEntity.Metadata.Id == entityToFind.Id)
        {
          return parentEntity;
        }
        parentEntity = parentEntity.Parent;
      }
      // Check if this entity usage belongs to the same entity
      if (returnThis && Metadata.EntityId == entityToFind.EntityId)
      {
        if (entityToFind.IsCompatibleByAttributes(Metadata))
        {
          return ConvertToEntityUsage(entityToFind);
        }
        if (!IsNew)
        {
          // Read necessary entity usage from DB by the same primary key values.
          CxBaseEntity entity = CreateAndReadFromDb(entityToFind, connection, this);
          if (entity != null)
          {
            return entity;
          }
        }
      }
      // Try to find needed entity in the parent instance chain by the indirect match.
      parentEntity = Parent;
      while (parentEntity != null)
      {
        if (parentEntity.Metadata.EntityId == entityToFind.EntityId)
        {
          if (entityToFind.IsCompatibleByAttributes(parentEntity.Metadata))
          {
            return parentEntity.ConvertToEntityUsage(entityToFind);
          }
          if (!parentEntity.IsNew)
          {
            CxBaseEntity entity = CreateAndReadFromDb(entityToFind, connection, parentEntity);
            if (entity != null)
            {
              return entity;
            }
          }
        }
        parentEntity = parentEntity.Parent;
      }
      // Try to find parent entity via the parent entity path.
      CxBaseEntity paramProvider = null;
      parentEntity = this;
      IList<CxParentEntityMetadata> path = null;
      while (parentEntity != null && (path == null || path.Count == 0))
      {
        path = parentEntity.Metadata.Entity.GetParentEntityPath(entityToFind);
        if (path != null && path.Count > 0)
        {
          paramProvider = parentEntity;
        }
        parentEntity = parentEntity.Parent;
      }
      if (path != null && path.Count > 0 && paramProvider != null)
      {
        foreach (CxParentEntityMetadata parentMetadata in path)
        {
          CxEntityMetadata currentEntityMetadata = parentMetadata.Entity;

          CxEntityUsageMetadata currentEntityUsageMetadata =
            parentMetadata.Id == entityToFind.EntityId ?
              entityToFind :
              currentEntityMetadata.DefaultEntityUsage;

          CxBaseEntity currentEntity = CreateAndReadFromDb(
            currentEntityUsageMetadata,
            connection,
            parentMetadata.WhereClause,
            paramProvider);
          if (currentEntity == null)
          {
            break;
          }
          paramProvider = currentEntity;
          if (parentMetadata.Id == entityToFind.EntityId)
          {
            return currentEntity;
          }
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds and loads parent entity.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="entityToFind">entity usage metadata to find</param>
    /// <param name="returnThis">if true and this entity matches to 
    /// the search entity returns this entity</param>
    public CxBaseEntity GetParentEntity(
      CxDbConnection connection,
      CxEntityUsageMetadata entityToFind,
      bool returnThis)
    {
      return GetParentEntity(
        connection, entityToFind, returnThis, false);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds and loads parent entity.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="entityToFind">entity usage metadata to find</param>
    public CxBaseEntity GetParentEntity(
      CxDbConnection connection,
      CxEntityUsageMetadata entityToFind)
    {
      return GetParentEntity(connection, entityToFind, true);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns value from the parent entity (or from the current entity).
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="expression">parent value expression</param>
    /// <param name="returnThis">true to return value from the current entity, if found</param>
    /// <returns>parent or current entity value</returns>
    public object GetParentEntityValue(
      CxDbConnection connection,
      string expression,
      bool returnThis)
    {
      if (CxUtils.NotEmpty(expression))
      {
        string entityId = "";
        bool lookThruDescendantsFirst = false;
        string attributeId = expression;
        if (expression.IndexOf(".") >= 0)
        {
          entityId = expression.Substring(0, expression.IndexOf("."));
          if (entityId.Length > 0 && entityId[entityId.Length - 1] == '^')
          {
            lookThruDescendantsFirst = true;
            entityId = entityId.Substring(0, entityId.Length - 1);
          }
          attributeId = expression.Substring(expression.IndexOf(".") + 1);
        }
        if (CxUtils.NotEmpty(attributeId))
        {
          CxEntityUsageMetadata entityUsage = null;
          if (CxUtils.NotEmpty(entityId))
          {
            CxEntityMetadata entityMetadata = Metadata.Holder.Entities.Find(entityId);
            entityUsage = entityMetadata != null ? entityMetadata.DefaultEntityUsage : Metadata.Holder.EntityUsages.Find(entityId);
          }
          CxBaseEntity entity;
          if (entityUsage != null)
          {
            entity = GetParentEntity(connection, entityUsage, returnThis, lookThruDescendantsFirst);
          }
          else
          {
            entity = this;
          }
          if (entity != null)
          {
            return entity[attributeId];
          }
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns value from the parent entity.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="expression">parent value expression</param>
    /// <returns>parent entity value</returns>
    public object GetParentEntityValue(
      CxDbConnection connection,
      string expression)
    {
      return GetParentEntityValue(connection, expression, false);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns value from the parent entity (or from the current entity).
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="expression">parent value expression</param>
    /// <returns>parent or current entity value</returns>
    public object GetCurrentOrParentEntityValue(
      CxDbConnection connection,
      string expression)
    {
      return GetParentEntityValue(connection, expression, true);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Checks disable condition.
    /// If disable condition expression calculates to true, 
    /// returns error message to display. Otherwise returns null.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="entityUsage">default entity usage to check condition for</param>
    /// <param name="condition">condition metadata to check</param>
    /// <param name="getEntity">method to get entity to check by the entity usage</param>
    /// <param name="checkJustLocalExpression">if true, no database expression being checked</param>
    static public string GetDisableConditionErrorText(
      CxDbConnection connection,
      CxEntityUsageMetadata entityUsage,
      CxErrorConditionMetadata condition,
      DxGetEntityByEntityUsage getEntity,
      bool checkJustLocalExpression)
    {
      if (condition != null && condition.IsActive)
      {
        CxEntityUsageMetadata currentEntityUsage = condition.EntityUsage;
        if (currentEntityUsage == null && condition.Entity != null)
        {
          if (entityUsage.Entity == condition.Entity)
            currentEntityUsage = entityUsage;
          else
            currentEntityUsage = condition.Entity.DefaultEntityUsage;
        }
        if (currentEntityUsage == null)
        {
          currentEntityUsage = entityUsage;
        }
        CxBaseEntity entity = getEntity(currentEntityUsage);
        if (entity != null)
        {
          if (CxUtils.NotEmpty(condition.ErrorTextExpression))
          {
            string errorText = CxUtils.ToString(
              entity.CalculateLocalExpression(connection, condition.ErrorTextExpression, checkJustLocalExpression));
            if (CxUtils.NotEmpty(errorText))
            {
              return errorText;
            }
          }
          else
          {
            bool result = entity.CalculateBoolExpression(connection, condition.Expression, checkJustLocalExpression);
            if (result)
            {
              return condition.ErrorText;
            }
          }
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Checks list of disable conditions.
    /// If at least one disable condition expression calculates to true, 
    /// returns error message to display. Otherwise returns null.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="entityUsage">default entity usage to check condition for</param>
    /// <param name="conditionList">list of condition metadata to check</param>
    /// <param name="getEntity">method to get entity to check by the entity usage</param>
    static public string GetDisableConditionsErrorText(
      CxDbConnection connection,
      CxEntityUsageMetadata entityUsage,
      IList<CxErrorConditionMetadata> conditionList,
      DxGetEntityByEntityUsage getEntity)
    {
      return GetDisableConditionsErrorText(connection, entityUsage, conditionList, getEntity, false);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Checks list of disable conditions.
    /// If at least one disable condition expression calculates to true, 
    /// returns error message to display. Otherwise returns null.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="entityUsage">default entity usage to check condition for</param>
    /// <param name="conditionList">list of condition metadata to check</param>
    /// <param name="getEntity">method to get entity to check by the entity usage</param>
    /// <param name="checkJustLocalExpressions">if true, no database expressions are being checked</param>
    static public string GetDisableConditionsErrorText(
      CxDbConnection connection,
      CxEntityUsageMetadata entityUsage,
      IList<CxErrorConditionMetadata> conditionList,
      DxGetEntityByEntityUsage getEntity,
      bool checkJustLocalExpressions)
    {
      if (conditionList != null)
      {
        foreach (CxErrorConditionMetadata condition in conditionList)
        {
          string result = GetDisableConditionErrorText(
            connection,
            entityUsage,
            condition,
            getEntity,
            checkJustLocalExpressions);
          if (CxUtils.NotEmpty(result))
          {
            return result;
          }
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Replaces text placeholders with actual field values.
    /// Placeholders are in format: %field_name%.
    /// </summary>
    /// <param name="entity">an entity to be processed</param>
    /// <param name="text">text to replace</param>
    /// <returns>text with placeholders replaced to values</returns>
    static public string ReplaceFieldNamesWithValues(
      CxBaseEntity entity,
      string text)
    {
      if (CxUtils.NotEmpty(text))
      {
        if (entity != null)
        {
          foreach (CxAttributeMetadata attr in entity.Metadata.Attributes)
          {
            string placeHolder = "%" + attr.Id + "%";
            if (text.IndexOf(placeHolder) >= 0)
            {
              text = text.Replace(placeHolder, CxUtils.ToString(entity[attr.Id]));
            }
          }
        }
        text = CxText.RemovePlaceholders(text, '%');
      }
      return CxUtils.Nvl(text);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns copy of the entity.
    /// </summary>
    virtual public CxBaseEntity Copy()
    {
      return CreateByValueProvider(Metadata, this);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if this entity has modified child entities.
    /// The result is available only after ApplyChildrenUpdates method call.
    /// </summary>
    public bool HasModifiedChildren
    {
      get
      {
        foreach (IList<CxBaseEntity> list in m_InsertedChildEntities.Values)
        {
          if (list.Count > 0)
          {
            return true;
          }
        }
        foreach (IList<CxBaseEntity> list in m_UpdatedChildEntities.Values)
        {
          if (list.Count > 0)
          {
            return true;
          }
        }
        foreach (IList<CxBaseEntity> list in m_DeletedChildEntities.Values)
        {
          if (list.Count > 0)
          {
            return true;
          }
        }
        foreach (IList<CxBaseEntity> list in m_UnchangedChildEntities.Values)
        {
          foreach (CxBaseEntity childEntity in list)
          {
            if (childEntity.HasModifiedChildren)
            {
              return true;
            }
          }
        }
        return false;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if given child entity has modified records.
    /// The result is available only after ApplyChildrenUpdates method call.
    /// </summary>
    /// <param name="child">child metadata</param>
    public bool IsChildEntityModified(CxChildEntityUsageMetadata child)
    {
      if (GetInsertedChildEntities(child).Count > 0 ||
          GetUpdatedChildEntities(child).Count > 0 ||
          GetDeletedChildEntities(child).Count > 0)
      {
        return true;
      }
      IList<CxBaseEntity> unchangedEntities = GetUnchangedChildEntities(child);
      foreach (CxBaseEntity childEntity in unchangedEntities)
      {
        if (childEntity.HasModifiedChildren)
        {
          return true;
        }
      }
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of all inserted child entities for the given child metadata.
    /// The result is available only after ApplyChildrenUpdates method call.
    /// </summary>
    /// <param name="child">child metadata</param>
    public IList<CxBaseEntity> GetInsertedChildEntities(CxChildEntityUsageMetadata child)
    {
      List<CxBaseEntity> result = new List<CxBaseEntity>();
      IList<CxBaseEntity> list =
        child != null && m_InsertedChildEntities.ContainsKey(child) ? m_InsertedChildEntities[child] : null;
      if (list != null)
      {
        result.AddRange(list);
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of all updated child entities for the given child metadata.
    /// The result is available only after ApplyChildrenUpdates method call.
    /// </summary>
    /// <param name="child">child metadata</param>
    public IList<CxBaseEntity> GetUpdatedChildEntities(CxChildEntityUsageMetadata child)
    {
      List<CxBaseEntity> result = new List<CxBaseEntity>();
      IList<CxBaseEntity> list =
        child != null && m_UpdatedChildEntities.ContainsKey(child) ? m_UpdatedChildEntities[child] : null;
      if (list != null)
      {
        result.AddRange(list);
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of all deleted child entities for the given child metadata.
    /// The result is available only after ApplyChildrenUpdates method call.
    /// </summary>
    /// <param name="child">child metadata</param>
    public IList<CxBaseEntity> GetDeletedChildEntities(CxChildEntityUsageMetadata child)
    {
      List<CxBaseEntity> result = new List<CxBaseEntity>();
      IList<CxBaseEntity> list =
        child != null && m_DeletedChildEntities.ContainsKey(child) ? m_DeletedChildEntities[child] : null;
      if (list != null)
      {
        result.AddRange(list);
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of all current child entities for the given child entity usage.
    /// The result is available only after ApplyChildrenUpdates method call.
    /// </summary>
    /// <param name="childEntityUsage">child entity usage</param>
    public IList<CxBaseEntity> GetCurrentChildEntities(CxEntityUsageMetadata childEntityUsage)
    {
      foreach (KeyValuePair<string, CxChildEntityUsageMetadata> pair in Metadata.ChildEntityUsages)
      {
        if (pair.Value.EntityUsage == childEntityUsage)
          return GetCurrentChildEntities(pair.Value);
      }
      return new List<CxBaseEntity>();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of all current child entities for the given child metadata.
    /// The result is available only after ApplyChildrenUpdates method call.
    /// </summary>
    /// <param name="child">child metadata</param>
    public IList<CxBaseEntity> GetCurrentChildEntities(CxChildEntityUsageMetadata child)
    {
      List<CxBaseEntity> result = new List<CxBaseEntity>();

      IList<CxBaseEntity> list =
        child != null && m_CurrentChildEntities.ContainsKey(child) ? m_CurrentChildEntities[child] : null;
      if (list != null)
      {
        result.AddRange(list);
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the child list is currently loaded to the entity.
    /// </summary>
    public bool GetIsCurrentChildEntitiesLoaded(CxChildEntityUsageMetadata child)
    {
      return child != null && m_CurrentChildEntities.ContainsKey(child);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the command should be visible from the current entity usage's 
    /// point of view.
    /// </summary>
    public virtual bool GetIsCommandVisible(
      CxCommandMetadata commandMetadata, IxEntity parentEntity)
    {
      return true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of all unchanged child entities for the given child metadata.
    /// The result is available only after ApplyChildrenUpdates method call.
    /// </summary>
    /// <param name="child">child metadata</param>
    public IList<CxBaseEntity> GetUnchangedChildEntities(CxChildEntityUsageMetadata child)
    {
      List<CxBaseEntity> result = new List<CxBaseEntity>();
      IList<CxBaseEntity> list =
        child != null && m_UnchangedChildEntities.ContainsKey(child) ? m_UnchangedChildEntities[child] : null;
      if (list != null)
      {
        result.AddRange(list);
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of current child entity usage metadata.
    /// </summary>
    /// <returns>list of CxChildEntityUsageMetadata objects</returns>
    public IList<CxChildEntityUsageMetadata> GetCurrentChildEntityUsages()
    {
      List<CxChildEntityUsageMetadata> result = new List<CxChildEntityUsageMetadata>();
      result.AddRange(m_CurrentChildEntities.Keys);
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Resets child entities data.
    /// </summary>
    public void ResetChildEntityData()
    {
      m_InitialChildEntities.Clear();
      m_CurrentChildEntities.Clear();
      m_InsertedChildEntities.Clear();
      m_UpdatedChildEntities.Clear();
      m_DeletedChildEntities.Clear();
      m_UnchangedChildEntities.Clear();
      ResetChildEntityErrorData();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Resets just the changes done to the child entities, keeping all 
    /// the initial and current entities in place.
    /// </summary>
    public void ResetChildEntityChanges()
    {
      m_InsertedChildEntities.Clear();
      m_UpdatedChildEntities.Clear();
      m_DeletedChildEntities.Clear();
      m_UnchangedChildEntities.Clear();
      ResetChildEntityErrorData();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Resets stored child entity error information.
    /// </summary>
    public void ResetChildEntityErrorData()
    {
      SetChildEntityErrorData(null, null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets child entity usage error data (context).
    /// </summary>
    /// <param name="childMetadata">child entity usage metadata</param>
    /// <param name="entity">child entity caused the error</param>
    public void SetChildEntityErrorData(
      CxChildEntityUsageMetadata childMetadata,
      CxBaseEntity entity)
    {
      m_ErrorChildEntityUsage = childMetadata;
      m_ErrorChildEntity = entity;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if current entity primary key is empty.
    /// </summary>
    public bool IsPkEmpty
    {
      get
      {
        CxAttributeMetadata[] pkAttrs = Metadata.PrimaryKeyAttributes;
        if (pkAttrs != null && pkAttrs.Length > 0)
        {
          foreach (CxAttributeMetadata attr in pkAttrs)
          {
            if (IsPkValueEmpty(this[attr.Id]))
            {
              return true;
            }
          }
          return false;
        }
        return true;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns unique entity ID composed from the entity primary or alternative
    /// key values.
    /// </summary>
    public string UniqueId
    {
      get
      {
        string[] pkNames;
        object[] pkValues;
        GetPrimaryKeyValues(out pkNames, out pkValues);
        if (IsPkEmpty && Metadata.IsAlternativeKeyDefined)
        {
          GetAlternativeKeyValues(out pkNames, out pkValues);
        }
        return Metadata.EntityId + "\t" + Metadata.EncodePrimaryKeyValuesAsString(pkValues);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns corrected attribute value to paste.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="attribute">attribute to return value for</param>
    /// <param name="value">current value</param>
    /// <returns>corrected value</returns>
    virtual protected object GetAttributeValueForPaste(
      CxDbConnection connection,
      CxAttributeMetadata attribute,
      object value)
    {
      if (CxUtils.NotEmpty(attribute.PasteDefault))
      {
        switch (attribute.PasteDefault.ToLower())
        {
          case "=this": return value;
          case "=null": return null;
          case "=default":
            return CalculateDefaultValue(attribute.Default, attribute, connection, this, null, null);
        }
        return CalculateDefaultValue(attribute.PasteDefault, attribute, connection, this, null, null);
      }
      // Set default values for attributes non-available to edit by user.
      // Also default values are set for primary key and alternative key attributes.
      if (CxUtils.NotEmpty(attribute.Default) &&
          (!Metadata.GetIsAttributeEditable(attribute) ||
           attribute.ReadOnly ||
           attribute.PrimaryKey ||
           attribute.AlternativeKey))
      {
        return CalculateDefaultValue(attribute.Default, attribute, connection, this, null, null);
      }
      // Set 'Copy of <CurrentName>' value to the unique string attributes available to edit by user
      if ((attribute.PrimaryKey || attribute.AlternativeKey) &&
          (attribute.Type == CxAttributeMetadata.TYPE_STRING || attribute.Type == CxAttributeMetadata.TYPE_LONGSTRING) &&
          !attribute.ReadOnly &&
          Metadata.GetIsAttributeEditable(attribute) &&
          attribute.RowSource == null)
      /* Commented out, key may be a part of complex key
      ((attribute.PrimaryKey && Metadata.PrimaryKeyAttributes.Length == 1) ||
       (attribute.AlternativeKey && Metadata.GetAlternativeKeyAttributes(attribute.AlternativeKeyIndex).Length == 1))
      */
      {
        string prefix = Metadata.Holder.GetTxt("Copy of") + " ";
        if (attribute.ControlModifiers.Contains("U"))
        {
          prefix = prefix.ToUpper();
        }
        else if (attribute.ControlModifiers.Contains("L"))
        {
          prefix = prefix.ToLower();
        }
        string valueToPaste = CxUtils.ToString(value);
        if (!valueToPaste.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
          valueToPaste = prefix + valueToPaste;

        CxDbParameter valueOutput = connection.CreateParameter("valueOutput", null, ParameterDirection.Output, DbType.String);
        connection.ExecuteCommandSP("p_GetNextUniqueFieldValueAvailable",
          connection.CreateParameter("tableOrViewName", Metadata.DbObject),
          connection.CreateParameter("fieldName", attribute.Id),
          connection.CreateParameter("valueInput", valueToPaste),
          connection.CreateParameter("whereClause", null),
          valueOutput);
        if (!string.IsNullOrEmpty(CxUtils.ToString(valueOutput.Value)))
        {
          valueToPaste = CxUtils.ToString(valueOutput.Value);
        }
        return valueToPaste;
      }
      // Set empty value for PK fields marked as non-storable (usually it is identity PK fields)
      if (attribute.PrimaryKey && !attribute.Storable)
      {
        return null;
      }
      return value;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Corrects attribute values before paste.
    /// </summary>
    virtual protected void SetValuesForPaste(CxDbConnection connection)
    {
      foreach (CxAttributeMetadata attribute in Metadata.Attributes)
      {
        this[attribute.Id] = GetAttributeValueForPaste(connection, attribute, this[attribute.Id]);
      }
      m_OldProperties.Clear();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Calculates entity on-save expressions.
    /// </summary>
    /// <param name="connection">database connection</param>
    public void CalculateOnSaveExpressions(CxDbConnection connection)
    {
      foreach (CxAttributeMetadata attribute in Metadata.Attributes)
      {
        if (CxUtils.NotEmpty(attribute.OnSaveExpression))
        {
          this[attribute.Id] = CalculateLocalExpression(connection, attribute.OnSaveExpression);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Validates entity mandatory conditions.
    /// Raises exception if not valid.
    /// </summary>
    /// <param name="connection">database connection</param>
    public void ValidateMandatoryConditions(CxDbConnection connection)
    {
      foreach (CxAttributeMetadata attribute in Metadata.Attributes)
      {
        if (CxUtils.IsEmpty(this[attribute.Id]) && CxUtils.NotEmpty(attribute.MandatoryCondition))
        {
          bool isMandatory = CalculateBoolExpression(connection, attribute.MandatoryCondition);
          if (isMandatory)
          {
            throw new ExMandatoryViolationException(this, attribute.Id);
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Executes specified SQL command on the specified entity.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="command">command to execute</param>
    /// <param name="entity">entity to perform command on</param>
    static public void ExecuteSqlCommand(
      CxDbConnection connection,
      CxCommandMetadata command,
      CxBaseEntity entity)
    {
      if (CxUtils.IsEmpty(command.SqlCommandText))
      {
        throw new ExException(
          String.Format("SQL command is empty for command '{0}'.", command.Id));
      }
      if (command.IsEntityInstanceRequired && entity == null)
      {
        throw new ExException(
          String.Format("There is no entity to execute SQL command '{0}'.", command.Id));
      }

      IxValueProvider valueProvider =
        entity != null ?
        entity.Metadata.PrepareValueProvider(entity) :
        command.Holder.PrepareValueProvider(null);

      command.ExecuteSqlCommand(connection, valueProvider);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Executes specified entity instance command on the specified entity.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="command">command to execute</param>
    /// <param name="entity">entity to perform command on</param>
    static public void ExecuteInstanceCommand(
      CxDbConnection connection,
      CxCommandMetadata command,
      CxBaseEntity entity)
    {
      if (CxUtils.IsEmpty(command.InstanceMethodName))
      {
        throw new ExException(
          String.Format("Instance method name is empty for command '{0}'.", command.Id));
      }
      if (entity == null)
      {
        throw new ExException(
          String.Format("There is no entity to execute instance method for '{0}'.", command.Id));
      }

      MethodInfo method = entity.GetType().GetMethod(
        command.InstanceMethodName,
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
        null,
        new Type[] { typeof(CxDbConnection), typeof(CxCommandMetadata) },
        null);
      if (method == null)
      {
        throw new ExException(String.Format(
          "Instance method '{0}' is not found for command '{1}'.",
          command.InstanceMethodName, command.Id));
      }
      method.Invoke(entity, new object[] { connection, command });
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Validates command security permission. Raises an exception if needed.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="command">command to check permission for</param>
    /// <param name="entityUsage">entity usage to check permission</param>
    /// <param name="entity">entity to check permission (for instance commands)</param>
    static public void CheckCommandPermission(
      CxDbConnection connection,
      CxCommandMetadata command,
      CxEntityUsageMetadata entityUsage,
      CxBaseEntity entity)
    {
      bool isAllowed;
      if (command.IsEntityInstanceRequired)
      {
        isAllowed = command.GetIsEnabled(entityUsage, connection, entity);
      }
      else
      {
        isAllowed = command.GetIsEnabled(entityUsage);
      }
      if (!isAllowed)
      {
        string entityName = "";
        if (entity != null)
        {
          entityName = entity.DisplayName;
        }
        if (CxUtils.NotEmpty(entityName))
        {
          entityName = " " + entityName;
        }
        throw new ExValidationException(command.Holder.GetErr(
          "Command '{0}' is forbidden for '{1}'.",
          new object[] { command.Text, entityUsage.SingleCaption + entityName }));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Compares entity values.
    /// </summary>
    /// <param name="v1">value #1</param>
    /// <param name="v2">value #2</param>
    /// <returns>true if values are equal</returns>
    virtual public bool CompareValues(object v1, object v2)
    {
      if (CxUtils.IsEmpty(v1) && CxUtils.IsEmpty(v2))
      {
        return true;
      }
      if (CxUtils.NotEmpty(v1) && CxUtils.NotEmpty(v2))
      {
        if (v1 is byte[] && v2 is byte[])
        {
          return CxByteArray.Equals((byte[]) v1, (byte[]) v2);
        }
        if (!(v1 is byte[]) && !(v2 is byte[]))
        {
          return CxUtils.Compare(v1, v2);
        }
      }
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if entity value is empty.
    /// </summary>
    static public bool IsEmpty(object value)
    {
      return CxUtils.IsEmpty(value) || (value is byte[] && ((byte[]) value).Length == 0);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of entities created by the data table rows.
    /// </summary>
    /// <param name="entityUsage">entity usage metadata</param>
    /// <param name="dt">data table containing entity rows</param>
    /// <returns>list of CxBaseEntity objects</returns>
    static public IList GetEntityListFromTable(
      CxEntityUsageMetadata entityUsage,
      DataTable dt)
    {
      ArrayList list = new ArrayList();
      if (entityUsage != null && dt != null)
      {
        foreach (DataRow dr in dt.Rows)
        {
          CxBaseEntity entity = CreateByDataRow(entityUsage, dr);
          list.Add(entity);
        }
      }
      return list;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if primary key value can be considered as empty.
    /// </summary>
    /// <param name="value">value to check</param>
    static public bool IsPkValueEmpty(object value)
    {
      return CxUtils.IsEmpty(value);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates and returns copy of the entity containing original values taken 
    /// from the entity data row. Returns null if original values are not available.
    /// </summary>
    public CxBaseEntity GetOriginalCopy()
    {
      if (m_DataRow != null)
      {
        try
        {
          return CreateByValueProvider(
            Metadata, new CxDataRowValueProvider(m_DataRow, DataRowVersion.Original));
        }
        catch (Exception)
        {
          return null;
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets entity containing original values to compare with.
    /// </summary>
    public CxBaseEntity OriginalEntity
    { get { return m_OriginalEntity; } set { m_OriginalEntity = value; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Invoked immediately after a data source has been initiated.
    /// </summary>
    /// <param name="dataSource">a data source containing entity or entity list</param>
    /// <param name="customHandler">a custom handler to be used for data pre-processing</param>
    virtual public void PreProcessDataSource(
      IxGenericDataSource dataSource,
      DxCustomPreProcessDataSource customHandler)
    {
      if (customHandler != null)
        customHandler(dataSource);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Invoked immediately after a data row has been initiated.
    /// </summary>
    /// <param name="dataRow">a data row containing entity</param>
    virtual public void PreProcessDataRow(CxGenericDataRow dataRow)
    {
      PreProcessHyperlinkXmlComposeAttributes(dataRow);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns custom WHERE clause.
    /// </summary>
    virtual public string GetCustomWhereClause()
    {
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns custom workspace filter WHERE clause 
    /// or null if custom workspace clause is not needed.
    /// </summary>
    /// <param name="defaultWorkspaceClause">default workspace WHERE clause</param>
    /// <returns>custom clause or null if no custom clause should be applied</returns>
    virtual public string GetCustomWorkspaceWhereClause(string defaultWorkspaceClause)
    {
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Swaps field #1 and field #2 values.
    /// </summary>
    /// <param name="fieldName1">first field name</param>
    /// <param name="fieldName2">second field name</param>
    public void SwapValues(string fieldName1, string fieldName2)
    {
      if (CxUtils.NotEmpty(fieldName1) && CxUtils.NotEmpty(fieldName2))
      {
        object value = this[fieldName1];
        this[fieldName1] = this[fieldName2];
        this[fieldName2] = value;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns child entity usage metadata caused last save or validate error.
    /// </summary>
    public CxChildEntityUsageMetadata ErrorChildEntityUsage
    { get { return m_ErrorChildEntityUsage; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns child entity caused last save or validate error.
    /// </summary>
    public CxBaseEntity ErrorChildEntity
    { get { return m_ErrorChildEntity; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if NotifyInMemoryChildEntityChange method should be called
    /// when cached in-memory (in a child grid of an entity dialog) 
    /// child entity is changed.
    /// </summary>
    virtual public bool IsInMemoryNotificationRequired
    { get { return Metadata.IsInMemoryNotificationRequired; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The method should be called for the changed entity when cached in-memory 
    /// (in a child grid of an entity dialog) child entity is changed.
    /// parentEntity parameter should contain initialized lists of child
    /// entities.
    /// </summary>
    /// <param name="operation">operation (create, insert, update, delete)</param>
    /// <param name="parentEntity">parent entity</param>
    /// <param name="changedEntity">changed entity</param>
    /// <param name="childMetadata">child entity metadata</param>
    /// <param name="insertList">list of entities inserted in the method</param>
    /// <param name="updateList">list of entities updated in the method</param>
    /// <param name="deleteList">list of entities deleted in the method</param>
    virtual public void NotifyInMemoryChildEntityChange(
      NxInMemoryOperation operation,
      CxBaseEntity parentEntity,
      CxBaseEntity changedEntity,
      CxChildEntityUsageMetadata childMetadata,
      ref IList<CxBaseEntity> insertList,
      ref IList<CxBaseEntity> updateList,
      ref IList<CxBaseEntity> deleteList)
    {
      if (insertList == null)
      {
        insertList = new UniqueList<CxBaseEntity>();
      }
      if (updateList == null)
      {
        updateList = new UniqueList<CxBaseEntity>();
      }
      if (deleteList == null)
      {
        deleteList = new UniqueList<CxBaseEntity>();
      }

      List<CxAttributeMetadata> onlyOneSelectedAttributes = new List<CxAttributeMetadata>();
      List<CxAttributeMetadata> incrementOnCreateAttributes = new List<CxAttributeMetadata>();
      if (Metadata == childMetadata.EntityUsage)
      {
        foreach (CxAttributeMetadata attribute in childMetadata.EntityUsage.Attributes)
        {
          if (attribute.OnlyOneSelected)
          {
            onlyOneSelectedAttributes.Add(attribute);
          }
          if (attribute.IncrementOnCreate)
          {
            incrementOnCreateAttributes.Add(attribute);
          }
        }
      }
      if (onlyOneSelectedAttributes.Count > 0 || incrementOnCreateAttributes.Count > 0)
      {
        IList<CxBaseEntity> entities = parentEntity.GetCurrentChildEntities(childMetadata);
        if (entities != null)
        {
          bool hasChecked;
          switch (operation)
          {
            case NxInMemoryOperation.Create:
              foreach (CxAttributeMetadata attribute in incrementOnCreateAttributes)
              {
                int maxValue = 0;
                foreach (CxBaseEntity entity in entities)
                {
                  int value = CxInt.Parse(entity[attribute.Id], 0);
                  if (value > maxValue)
                  {
                    maxValue = value;
                  }
                }
                if (maxValue > 0)
                {
                  int currentValue = CxInt.Parse(this[attribute.Id], 0);
                  if (currentValue < maxValue + 1)
                  {
                    this[attribute.Id] = maxValue + 1;
                    updateList.Add(this);
                  }
                }
              }
              break;

            case NxInMemoryOperation.Insert:
            case NxInMemoryOperation.Update:
              foreach (CxAttributeMetadata attribute in onlyOneSelectedAttributes)
              {
                if (CxBool.Parse(this[attribute.Id]))
                {
                  foreach (CxBaseEntity entity in entities)
                  {
                    if (!CompareByPK(entity))
                    {
                      if (CxBool.Parse(entity[attribute.Id]))
                      {
                        entity[attribute.Id] = false;
                        updateList.Add(entity);
                      }
                    }
                  }
                }
                else
                {
                  hasChecked = false;
                  foreach (CxBaseEntity entity in entities)
                  {
                    if (!CompareByPK(entity) && CxBool.Parse(entity[attribute.Id]))
                    {
                      hasChecked = true;
                      break;
                    }
                  }
                  if (!hasChecked)
                  {
                    this[attribute.Id] = true;
                    updateList.Add(this);
                  }
                }
              }
              break;

            case NxInMemoryOperation.Delete:
              foreach (CxAttributeMetadata attribute in onlyOneSelectedAttributes)
              {
                hasChecked = false;
                CxBaseEntity firstEntity = null;
                foreach (CxBaseEntity entity in entities)
                {
                  if (!CompareByPK(entity))
                  {
                    if (firstEntity == null)
                    {
                      firstEntity = entity;
                    }
                    if (CxBool.Parse(entity[attribute.Id]))
                    {
                      hasChecked = true;
                      break;
                    }
                  }
                }
                if (!hasChecked && firstEntity != null)
                {
                  firstEntity[attribute.Id] = true;
                  updateList.Add(firstEntity);
                }
              }
              break;
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The method should be called when this entity updated to inform
    /// child grids of an entity dialog about parent entity change.
    /// This entity should contain initialized lists of child entities.
    /// </summary>
    /// <param name="attributeId">changed attribute ID</param>
    /// <param name="insertList">list of entities inserted in the method</param>
    /// <param name="updateList">list of entities updated in the method</param>
    /// <param name="deleteList">list of entities deleted in the method</param>
    virtual public void NotifyInMemoryParentEntityChange(
      string attributeId,
      ref IList<CxBaseEntity> insertList,
      ref IList<CxBaseEntity> updateList,
      ref IList<CxBaseEntity> deleteList)
    {
      if (insertList == null)
      {
        insertList = new UniqueList<CxBaseEntity>();
      }
      if (updateList == null)
      {
        updateList = new UniqueList<CxBaseEntity>();
      }
      if (deleteList == null)
      {
        deleteList = new UniqueList<CxBaseEntity>();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Should return true if child entity lists should be completely loaded 
    /// before the parent entity in-memory change notification.
    /// </summary>
    /// <param name="changedAttrId">ID of the changed attribute</param>
    /// <returns>true or false</returns>
    virtual public bool IsChildLoadRequiredBeforeInMemoryParentNotify(string changedAttrId)
    {
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds parent entity (located in memory) by primary key values.
    /// </summary>
    /// <param name="pkValues">array of primary key values</param>
    /// <returns>found entity</returns>
    public CxBaseEntity FindParentByPk(object[] pkValues)
    {
      CxBaseEntity parent = Parent;
      while (parent != null)
      {
        string[] parentPkNames;
        object[] parentPkValues;
        parent.GetPrimaryKeyValues(out parentPkNames, out parentPkValues);
        if (pkValues != null &&
            parentPkValues != null &&
            pkValues.Length > 0 &&
            parentPkValues.Length > 0 &&
            pkValues.Length == parentPkValues.Length)
        {
          bool equals = true;
          for (int i = 0; i < pkValues.Length; i++)
          {
            if (!CompareValues(pkValues[i], parentPkValues[i]))
            {
              equals = false;
              break;
            }
          }
          if (equals)
          {
            return parent;
          }
        }
        parent = parent.Parent;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Custom flag indicating main dialog entity.
    /// </summary>
    public bool IsMain
    { get { return m_IsMain; } set { m_IsMain = value; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets all attributes not marked as read-only, hence under certain conditions such attributes
    /// can be edited.
    /// </summary>
    private IList<CxAttributeMetadata> NonReadOnlyAttributes
    {
      get { return GetNonReadOnlyAttributes(); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets all attributes not marked as read-only, hence under certain conditions such attributes
    /// can be edited.
    /// </summary>
    protected virtual IList<CxAttributeMetadata> GetNonReadOnlyAttributes()
    {
      List<CxAttributeMetadata> result = new List<CxAttributeMetadata>();
      foreach (CxAttributeMetadata attributeMetadata in Metadata.Attributes)
      {
        if (!attributeMetadata.ReadOnly)
          result.Add(attributeMetadata);
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Fulfills the current entity's attribute with some values of the given referenced entity.
    /// </summary>
    /// <param name="referencedEntity">an entity the attribute references to</param>
    /// <param name="attribute">the attribute of the current entity to be fulfilled</param>
    /// <returns>true if the current entity has been changed</returns>
    virtual protected bool ApplyReferencedEntityToEntityAttribute(
      CxBaseEntity referencedEntity,
      CxAttributeMetadata attribute)
    {
      if (CxUtils.NotEmpty(attribute.ReferenceAttributeId))
      {
        CxAttributeMetadata referenceAttribute = referencedEntity.Metadata.GetAttribute(attribute.ReferenceAttributeId);
        CxAttributeMetadata valueAttribute = Metadata.GetValueDefinedAttribute(attribute);
        if (valueAttribute != null)
        {
          CxAttributeMetadata valueReferencedAttribute =
            referencedEntity.Metadata.GetValueDefinedAttribute(referenceAttribute);
          if (valueReferencedAttribute != null)
            this[valueAttribute.Id] = referencedEntity[valueReferencedAttribute.Id];
        }
        this[attribute.Id] = referencedEntity[referenceAttribute.Id];
        return true;
      }
      else if (attribute.IsHyperLink)
      {
        CxAttributeMetadata nameAttribute = referencedEntity.Metadata.NameAttribute;
        if (nameAttribute != null)
        {
          if (attribute.HyperLinkComposeXml)
          {
            this[attribute.Id] = CreateEntityHyperlinkXml(referencedEntity.Metadata.Id, referencedEntity.PrimaryKeyAsString, referencedEntity.DisplayName);
          }
          else
          {
            this[attribute.Id] = referencedEntity[nameAttribute.Id];
          }
          return true;
        }
      }
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Updates (refreshes) the attribute of the current entity with values of the
    /// referenced entity.
    /// </summary>
    /// <param name="referencedEntity">an entity the attribute references to</param>
    /// <param name="attribute">the attribute of the current entity to be updated</param>
    /// <param name="referencePkAttribute">the attribute of the referenced entity that represents
    /// the primary key</param>
    /// <returns>true if the current entity has been changed</returns>
    virtual protected bool RefreshAttributeFromReferencedEntity(
      CxBaseEntity referencedEntity,
      CxAttributeMetadata attribute,
      CxAttributeMetadata referencePkAttribute)
    {
      CxEntityUsageMetadata[] entityUsages = null;

      CxAttributeMetadata referenceValueAttribute = attribute.ReferenceValueAttribute;

      if (attribute.Type == CxAttributeMetadata.TYPE_LINK &&
          attribute.HyperLinkAutoRefresh)
      {
        if (referenceValueAttribute == null)
          referenceValueAttribute = Metadata.GetValueDefinedAttribute(attribute);

        if (referenceValueAttribute != null && CxUtils.NotEmpty(this[referenceValueAttribute.Id]))
        {
          // If the attribute is a hyperlink dependent on the changed entity
          if (CxUtils.NotEmpty(attribute.HyperLinkEntityUsageId))
          {
            entityUsages = new CxEntityUsageMetadata[]
              {
                Metadata.Holder.EntityUsages[attribute.HyperLinkEntityUsageId]
              };
          }
          // If we need to resolve the hyperlinked entity usage id
          else if (CxUtils.NotEmpty(attribute.HyperLinkEntityUsageAttrId))
          {
            string entityUsageId = CxUtils.ToString(this[attribute.HyperLinkEntityUsageAttrId]);
            if (CxUtils.NotEmpty(entityUsageId))
            {
              entityUsages = new CxEntityUsageMetadata[]
                {
                  Metadata.Holder.EntityUsages[entityUsageId]
                };
            }
          }
        }
      }
      else if (attribute.ReferenceAutoRefresh)
      {
        entityUsages = attribute.ReferenceEntityUsages;
      }

      if (entityUsages != null)
      {
        foreach (CxEntityUsageMetadata entityUsage in entityUsages)
        {
          if (entityUsage.EntityId == referencedEntity.Metadata.EntityId &&
              CompareValues(this[referenceValueAttribute.Id], referencedEntity[referencePkAttribute.Id]))
          {
            return ApplyReferencedEntityToEntityAttribute(referencedEntity, attribute);
          }
        }
      }

      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Updates (refreshes) the current entity with some values of the referenced one,
    /// depending on the current entity's metadata.
    /// </summary>
    /// <param name="referencedEntity">an entity to update the current entity by</param>
    /// <returns>true if the current entity has been changed</returns>
    public bool RefreshEntityFromReferenced(CxBaseEntity referencedEntity)
    {
      return RefreshEntityFromReferenced(referencedEntity, Metadata.Attributes);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Updates (refreshes) the current entity with some values of the referenced one,
    /// depending on the current entity's metadata.
    /// </summary>
    /// <param name="referencedEntity">an entity to update the current entity by</param>
    /// <param name="attributesToRefresh">a list of attributes to be refreshed</param>
    /// <returns>true if the current entity has been changed</returns>
    virtual public bool RefreshEntityFromReferenced(
      CxBaseEntity referencedEntity, IList<CxAttributeMetadata> attributesToRefresh)
    {
      if (referencedEntity == null)
        return false;

      // If the changed entity has got no primary key, we have no possibility
      // to determine which dependent records should be refreshed.
      CxAttributeMetadata pkAttribute = referencedEntity.Metadata.PrimaryKeyAttribute;
      if (pkAttribute == null)
        return false;

      bool result = false;
      foreach (CxAttributeMetadata attribute in attributesToRefresh)
      {
        result |= RefreshAttributeFromReferencedEntity(referencedEntity, attribute, pkAttribute);
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds entity to insert to database before child entities insert or update.
    /// </summary>
    /// <param name="entity">entity to add to list</param>
    public void AddEntityToInsertBeforeChildren(CxBaseEntity entity)
    {
      m_EntitiesToInsertBeforeChildren.Add(entity);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Inserts entities before child entities insert or update.
    /// </summary>
    /// <param name="connection">database connection</param>
    protected void InsertEntitiesBeforeChildren(CxDbConnection connection)
    {
      foreach (CxBaseEntity entity in m_EntitiesToInsertBeforeChildren)
      {
        entity.Insert(connection);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds entity to delete from database after child entities delete.
    /// </summary>
    /// <param name="entity">entity to add to list</param>
    public void AddEntityToDeleteAfterChildren(CxBaseEntity entity)
    {
      m_EntitiesToDeleteAfterChildren.Add(entity);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Deletes entities after child entities delete.
    /// </summary>
    /// <param name="connection">database connection</param>
    protected void DeleteEntitiesAfterChildren(CxDbConnection connection)
    {
      foreach (CxBaseEntity entity in m_EntitiesToDeleteAfterChildren)
      {
        entity.Delete(connection);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Writes the entity's properties to the entity's underlying data row.
    /// </summary>
    public void WriteToOwnDataRow()
    {
      WriteToOwnDataRow(true);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Writes the entity's properties to the entity's underlying data row.
    /// </summary>
    /// <param name="acceptChanges">if true, the AcceptChanges method 
    /// is automatically called on the underlying data row</param>
    public void WriteToOwnDataRow(bool acceptChanges)
    {
      if (DataRow != null)
      {
        WriteDataRow(DataRow);
        if (acceptChanges)
          DataRow.AcceptChanges();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns scrambled value of the attribute with the specified ID.
    /// </summary>
    /// <param name="attributeId">attribute ID</param>
    /// <returns>scrambled value</returns>
    public string GetScrambledValue(string attributeId)
    {
      return CxScrambler.Scramble(
        Metadata.GetAttribute(attributeId), CxUtils.ToString(this[attributeId]));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a dictionary of items' amount by visible child entitiy usage. 
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="orderType">the type of the order the child entity usages are located in</param>
    public Dictionary<string, int> GetAmountOfItemsForChildEntities(CxDbConnection connection, NxChildEntityUsageOrderType orderType)
    {
      Dictionary<string, int> amountOfItems = new Dictionary<string, int>();
      CxChildEntityUsageOrder order = Metadata.GetChildEntityUsageOrder(orderType);
      if (order.OrderPlusNewChildEntityUsages.Count > 0)
      {
        Dictionary<string, string> listChildDataQuery = new Dictionary<string, string>();
        IxValueProvider providers = Metadata.PrepareValueProvider(this);

        DataTable dt = new DataTable();

        foreach (CxChildEntityUsageMetadata child in order.OrderPlusNewChildEntityUsages)
        {
          if (child.IsVisibleInList && child.IsShowAmountOfItems)
          {
            listChildDataQuery.Add(child.Id, child.EntityUsage.GetChildDataQuery(connection, ""));
          }
        }
        if (listChildDataQuery.Count > 0)
        {
          string query = connection.ScriptGenerator.GetQueryForAmountOfItems(listChildDataQuery);
          if (!string.IsNullOrEmpty(query))
          {
            connection.GetQueryResult(dt, query, providers);

            foreach (DataRow row in dt.Rows)
            {
              amountOfItems.Add(CxUtils.ToString(row.ItemArray[0]), CxInt.Parse(row.ItemArray[1], 0));
            }
          }
        }
      }

      return amountOfItems;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates entity hyperlink XML structure.
    /// </summary>
    /// <returns>XML string representation</returns>
    public string CreateEntityHyperlinkXml(string entityUsageId, string id, string name)
    {
      XmlDocument doc = new XmlDocument();
      doc.AppendChild(doc.CreateElement("root"));
      if (doc.DocumentElement == null)
        throw new ExException("Document Element is null for some unknown reason");

      doc.DocumentElement.Attributes.Append(doc.CreateAttribute("EntityUsageId"));
      doc.DocumentElement.Attributes.Append(doc.CreateAttribute("Id"));
      doc.DocumentElement.Attributes.Append(doc.CreateAttribute("Name"));
      doc.DocumentElement.Attributes["EntityUsageId"].Value = entityUsageId;
      doc.DocumentElement.Attributes["Id"].Value = id;
      doc.DocumentElement.Attributes["Name"].Value = name;
      return doc.DocumentElement.OuterXml;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Preprocesses XML compose hyperlink attributes.
    /// </summary>
    public void PreProcessHyperlinkXmlComposeAttributes(IxValueProvider provider)
    {
      foreach (CxAttributeMetadata attributeMetadata in Metadata.GetHyperlinkComposeXmlAttributes())
      {
        CxAttributeMetadata valueAttribute = Metadata.GetValueDefinedAttribute(attributeMetadata);
        CxAttributeMetadata textAttribute = Metadata.GetTextDefinedAttribute(attributeMetadata) ?? attributeMetadata;
        if (valueAttribute != null && !string.IsNullOrEmpty(attributeMetadata.HyperLinkEntityUsageAttrId))
        {
          string entityUsageId = CxUtils.ToString(provider[attributeMetadata.HyperLinkEntityUsageAttrId]);
          if (string.IsNullOrEmpty(entityUsageId))
            entityUsageId = attributeMetadata.HyperLinkEntityUsageId;
          if (!string.IsNullOrEmpty(entityUsageId))
          {
            string name = CxUtils.ToString(provider[textAttribute.Id]);
            string id = CxUtils.ToString(provider[valueAttribute.Id]);
            provider[attributeMetadata.Id] = CreateEntityHyperlinkXml(entityUsageId, id, name);
          }
        }
      }
    }
    //-------------------------------------------------------------------------
  }
  //----------------------------------------------------------------------------
  /// <summary>
  /// Enumeration for SQL DML operation types.
  /// </summary>
  public enum NxDmlOperation { Insert, Update, Delete }
  //----------------------------------------------------------------------------
  /// <summary>
  /// Enumeration for in-memory entity change kind.
  /// </summary>
  public enum NxInMemoryOperation { Create, Insert, Update, Delete }
  //----------------------------------------------------------------------------
  /// <summary>
  /// Delegate to get value from parent entity.
  /// </summary>
  public delegate object DxGetParentValue(string expression);
  //----------------------------------------------------------------------------
  /// <summary>
  /// Delegate to get next sequence value for entity.
  /// </summary>
  public delegate int DxGetSequenceValue(
    CxAttributeMetadata attr,
    CxDbConnection connection);
  //----------------------------------------------------------------------------
  /// <summary>
  /// Delegate to get entity by the required entity usage.
  /// </summary>
  public delegate CxBaseEntity DxGetEntityByEntityUsage(CxEntityUsageMetadata entityUsage);
}