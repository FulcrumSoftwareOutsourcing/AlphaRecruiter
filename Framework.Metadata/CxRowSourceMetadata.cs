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
using System.Data;
using System.Linq;

using Framework.Utils;
using Framework.Db;
using System.Reflection;

namespace Framework.Metadata
{
  /// <summary>
  /// Summary description for CxRowSourceMetadata.
  /// </summary>
  public class CxRowSourceMetadata : CxMetadataObject
  {
    //-------------------------------------------------------------------------
    protected const string TYPE_STRING = "string";
    protected const string TYPE_INT = "int";
    protected const string TYPE_BOOLEAN = "boolean";
    //-------------------------------------------------------------------------
    // True if row source hardcoded in the XML file rather than references to entity usage
    protected bool m_HardCoded = true;
    // Hard coded list
    protected List<CxComboItem> m_List = new List<CxComboItem>();
    // Cached lookup data
    protected DataTable m_CacheTable = null;
    // Cached lookup data value-row map
    protected Dictionary<string, DataRow> m_CacheValueRowMap = new Dictionary<string, DataRow>();
    // Cached lookup data value-empty row map
    protected Dictionary<string, DataRow> m_CacheValueEmptyRowMap = new Dictionary<string, DataRow>();
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public CxRowSourceMetadata(CxMetadataHolder holder)
      : base(holder)
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public CxRowSourceMetadata(CxMetadataHolder holder, bool isHardCoded)
      : base(holder)
    {
      HardCoded = isHardCoded;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">a metadata holder this row source belongs to</param>
    /// <param name="element">XML element that holds metadata</param>
    public CxRowSourceMetadata(CxMetadataHolder holder, XmlElement element)
      : base(holder, element)
    {
      // Load values from the basic metadata XML
      XmlElement rowsElement = (XmlElement) element.SelectSingleNode("rows");
      if (rowsElement == null)
      {
        HardCoded = false;
        return;
      }
      XmlNodeList rows = rowsElement.SelectNodes("row");
      if (rows != null)
      {
        foreach (XmlElement rowElement in rows)
        {
          string value = CxXml.GetAttr(rowElement, "value");
          object key = GetHardcodedKey(CxXml.GetAttr(rowElement, "key", value));
          string imageId = CxXml.GetAttr(rowElement, "image_id");
          m_List.Add(new CxRowSourceItem(this, key, value, imageId));
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Converts hard-coded row source string key to the required type.
    /// </summary>
    /// <param name="keyStr">string representation of key</param>
    /// <returns>converted key</returns>
    protected object GetHardcodedKey(string keyStr)
    {
      if (KeyType.ToLower() == TYPE_INT)
      {
        int keyInt;
        if (!Int32.TryParse(keyStr, out keyInt))
        {
          throw new ExException(
            String.Format("Row source '{0}' key value '{1}' is not compatible with the <int> row source key type.", Id, keyStr));
        }
        return keyInt;
      }
      else if (KeyType.ToLower() == TYPE_BOOLEAN)
      {
        return CxBool.Parse(keyStr);
      }
      return keyStr;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads hard-coded items from the list of CxComboItem objects.
    /// </summary>
    /// <param name="items">list of CxComboItem objects</param>
    public void LoadFrom(IList<CxComboItem> items)
    {
      HardCoded = true;
      m_List.Clear();
      foreach (CxComboItem item in items)
      {
        m_List.Add(item);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Id of the entity usage that serves as row source.
    /// </summary>
    public string EntityUsageId
    {
      get { return this["entity_usage_id"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Id of the setup entity usage that serves as row source.
    /// </summary>
    public string SetupEntityUsageId
    {
      get { return this["setup_entity_usage_id"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Entity usage that serves as row source.
    /// </summary>
    public CxEntityUsageMetadata EntityUsage
    {
      get
      {
        if (!string.IsNullOrEmpty(EntityUsageId))
          return Holder.EntityUsages[EntityUsageId];
        return null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Setup entity usage that serves as row source.
    /// </summary>
    public CxEntityUsageMetadata SetupEntityUsage
    {
      get
      {
        if (CxUtils.IsEmpty(SetupEntityUsageId))
        {
          return EntityUsage;
        }
        return Holder.EntityUsages[SetupEntityUsageId];
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity usage for the dynamic row source.
    /// </summary>
    public CxEntityUsageMetadata ActualEntityUsage
    {
      get
      {
        return !HardCoded ? EntityUsage : null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// true if data should be hierarchical (works with self-reference entity usages only).
    /// </summary>
    public bool IsHierarchical
    {
      get { return this["hierarchical"].ToLower() == "true"; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The id of the attribute which value indicates whether the record can be selected in the
    /// lookup control or not.
    /// For now, this option works just for the Windows scope, and just for tree-list drop-downs.
    /// </summary>
    public string AllowSelectingByAttributeId
    {
      get { return this["allow_selecting_by_attribute_id"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The attribute which value indicates whether the record can be selected in the
    /// lookup control or not.
    /// </summary>
    public CxAttributeMetadata AllowSelectingByAttribute
    {
      get
      {
        if (!string.IsNullOrEmpty(AllowSelectingByAttributeId) && EntityUsage != null)
        {
          return EntityUsage.GetAttribute(AllowSelectingByAttributeId);
        }
        return null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// true if hierarchy orphan records should be visible (moved to the first level of hierarchy)
    /// </summary>
    public bool IsHierarchyOrphansVisible
    {
      get { return this["hierarchy_orphan_visible"].ToLower() == "true"; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if row source items should be localizable (for static row sources only)
    /// </summary>
    public bool IsLocalizable
    {
      get { return this["localizable"].ToLower() != "false"; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ID of the row source entity usage attribute 
    /// containing valid image metadata ID.
    /// </summary>
    public string ImageAttributeId
    { get { return this["image_attr_id"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ID of the row source entity usage attribute 
    /// containing valid RGB-color data (comma-separated list of colors: R,G,B).
    /// </summary>
    public string RgbColorAttributeId
    { get { return this["rgb_color_attr_id"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the color should be displayed to the end-user of the lookup.
    /// </summary>
    public bool DisplayColor
    {
      get { return CxBool.Parse(this["display_color"], !string.IsNullOrEmpty(RgbColorAttributeId)); }
      set { this["display_color"] = value.ToString(); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if row source is huge.
    /// The whole row source dataset should be never selected from the database.
    /// </summary>
    public bool IsHuge
    {
      get { return this["huge"].ToLower() == "true"; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if row source items should be cached for better performance
    /// </summary>
    public bool IsCached
    {
      get
      {
        return Holder.IsLookupCacheEnabled && !IsHuge && this["cached"].ToLower() != "false";
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// true if row source hardcoded in the cobnfiguration file 
    /// rather than references to entity usage.
    /// </summary>
    public bool HardCoded
    {
      get { return m_HardCoded; }
      protected set { m_HardCoded = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Hard coded data.
    /// </summary>
    public IList<CxComboItem> List
    {
      get { return m_List; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the id of the class that provides row source custom data.
    /// </summary>
    public string CustomDataProviderClassId
    {
      get { return this["custom_data_provider_class_id"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the metadata of the class that is capable to provide custom data for the row source.
    /// </summary>
    public CxClassMetadata CustomDataProviderClassMetadata
    {
      get { return CxUtils.NotEmpty(CustomDataProviderClassId) ? Holder.Classes[CustomDataProviderClassId] : null; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the name of the static method that provides row source custom data.
    /// </summary>
    public string CustomDataProviderMethodName
    {
      get { return this["custom_data_provider_method"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// true if row source items are visible in setup.
    /// </summary>
    public bool WinIsLookupVisible
    {
      get { return CxBool.Parse(this["win_is_lookup_visible"], false); }
      set { this["win_is_lookup_visible"] = value.ToString(); }
    }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of CxComboItem objects to populate combobox.
    /// </summary>
    /// <param name="owner">owner of the row source</param>
    /// <param name="connection">database connection to use during queries</param>
    /// <param name="filterCondition">filter for the data</param>
    /// <param name="valueProvider">provider of parameter values</param>
    /// <param name="addEmptyRow">if true adds emty row as a first one</param>
    /// <returns>list of CxComboItem objects to populate combobox</returns>
    public IList<CxComboItem> GetList(IxRowSourceOwner owner,
                         CxDbConnection connection,
                         string filterCondition,
                         IxValueProvider valueProvider,
                         bool addEmptyRow)
    {
      if (HardCoded)
      {
        if (addEmptyRow)
        {
          List<CxComboItem> list = new List<CxComboItem>();
          list.Add(new CxComboItem(null, ""));
          list.AddRange(m_List);
          return list;
        }
        return new List<CxComboItem>(m_List);
      }
      else
      {
        List<CxComboItem> list = new List<CxComboItem>();

        DataTable table = LoadDataTable(connection, filterCondition, valueProvider, false);

        string keyName = KeyAttribute != null ? KeyAttribute.Id : KeyAttributeId;
        string valueName = ValueAttribute != null ? ValueAttribute.Id : NameAttributeId;
        string imageIdName = ImageAttributeId;
        string rgbColorName = RgbColorAttributeId;
        if (!table.Columns.Contains(imageIdName))
        {
          imageIdName = null;
        }

        if (addEmptyRow)
        {
          list.Add(new CxComboItem(null, ""));
        }

        if (IsHierarchical && EntityUsage.IsSelfReferencing)
        {
          if (IsHierarchyOrphansVisible)
          {
            MoveHierarchyOrphansToFirstLevel(
              table,
              EntityUsage.PrimaryKeyAttribute.Id,
              EntityUsage.SelfReferenceAttrId);
          }
          LoadHierarchicalList(
            table,
            keyName,
            valueName,
            imageIdName,
            rgbColorName,
            list,
            EntityUsage.PrimaryKeyAttribute.Id,
            EntityUsage.SelfReferenceAttrId,
            null,
            0);
        }
        else
        {
          LoadPlainList(table, keyName, valueName, imageIdName, rgbColorName, list);
        }

        return list;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads plain data from datatable to list.
    /// </summary>
    protected void LoadPlainList(
      DataTable table,
      string keyName,
      string valueName,
      string imageIdName,
      string rgbColorName,
      IList<CxComboItem> list)
    {
      string orderBy = table.Columns.Count > 0 && EntityUsage != null ? EntityUsage.OrderByClause : "";
      string filter = "";
      DataView dataView = new DataView(table, filter, orderBy, DataViewRowState.CurrentRows);
      foreach (DataRowView rowView in dataView)
      {
        DataRow dr = rowView.Row;
        object key = CxData.GetColumnValue(dr, keyName);
        string value = CxUtils.Nvl(CxData.GetColumnValue(dr, valueName), "").ToString();
        CxComboItem item = new CxComboItem(key, value);
        if (CxUtils.NotEmpty(imageIdName))
        {
          Type type = CxData.GetColumn(table, imageIdName).DataType;
          if (type == typeof(string))
            item.ImageReference = CxUtils.ToString(dr[imageIdName]);
          else if (type == typeof(byte[]))
            item.Image = CxBlobFile.GetImageFromBlob(dr[imageIdName]);
        }
        if (CxUtils.NotEmpty(rgbColorName))
        {
          item.ColorReference = CxUtils.ToString(dr[rgbColorName]);
        }
        list.Add(item);
      }
    }
    //-------------------------------------------------------------------------
    public override void LoadOverride(XmlElement element)
    {
      base.LoadOverride(element);
      XmlElement nodeRows = (XmlElement) element.SelectSingleNode("rows");
      if (nodeRows != null)
      {
        var nodeRowsChildren = nodeRows.SelectNodes("row");
        if (nodeRowsChildren != null)
        {
          foreach (XmlElement nodeRow in nodeRowsChildren)
          {
            string value = CxXml.GetAttr(nodeRow, "value");
            object key = GetHardcodedKey(CxXml.GetAttr(nodeRow, "key", value));
            var itemExisting = List.FirstOrDefault(x => CxUtils.Compare(x.Value, key));
            if (itemExisting != null)
            {
              string imageId = CxXml.GetAttr(nodeRow, "image_id");
              itemExisting.Description = value;
              itemExisting.ImageReference = imageId;
            }
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the XML tag name applicable for the current metadata object.
    /// </summary>
    public override string GetTagName()
    {
      return "row_source";
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Moves hierarchy orphan records (the ones with a reference present, 
    /// but with no target record available in fact) to the first level.
    /// </summary>
    protected void MoveHierarchyOrphansToFirstLevel(
      DataTable dt,
      string pkName,
      string selfRefName)
    {
      foreach (DataRow dr in dt.Rows)
      {
        if (CxUtils.NotEmpty(dr[selfRefName]))
        {
          string selfRefValue = CxUtils.ToString(dr[selfRefName]);
          string filter = pkName + " = '" + selfRefValue + "'";
          DataRow[] rows = dt.Select(filter);
          if (rows == null || rows.Length == 0)
          {
            dr[selfRefName] = DBNull.Value;
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads plain data from datatable to list.
    /// </summary>
    protected void LoadHierarchicalList(
      DataTable table,
      string keyName,
      string valueName,
      string imageIdName,
      string rgbColorName,
      IList<CxComboItem> list,
      string pkName,
      string selfRefName,
      object pkValue,
      int level)
    {
      string orderBy =
        table.Columns.Count > 0 ? EntityUsage.OrderByClause : "";
      string filter =
        CxUtils.IsEmpty(pkValue) ? selfRefName + " IS NULL" : selfRefName + " = '" + CxUtils.ToString(pkValue) + "'";

      DataView dataView = new DataView(table, filter, orderBy, DataViewRowState.CurrentRows);
      foreach (DataRowView rowView in dataView)
      {
        DataRow dr = rowView.Row;
        object key = CxData.GetColumnValue(dr, keyName);
        string value = CxUtils.Nvl(CxData.GetColumnValue(dr, valueName), "").ToString();
        if (level > 0)
        {
          value = new string(CxConst.cNBSP, level * 4) + value;
        }
        CxComboItem item = new CxComboItem(key, value);
        if (CxUtils.NotEmpty(imageIdName))
        {
          Type type = CxData.GetColumn(table, imageIdName).DataType;
          if (type == typeof(string))
            item.ImageReference = CxUtils.ToString(dr[imageIdName]);
          else if (type == typeof(byte[]))
            item.Image = CxBlobFile.GetImageFromBlob(dr[imageIdName]);
        }
        if (CxUtils.NotEmpty(rgbColorName))
        {
          item.ColorReference = CxUtils.ToString(dr[rgbColorName]);
        }
        list.Add(item);
        LoadHierarchicalList(
          table,
          keyName,
          valueName,
          imageIdName,
          rgbColorName,
          list,
          pkName,
          selfRefName,
          dr[pkName],
          level + 1);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Lookups row source to find text description corresponding to the given key value.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="value">key to find by</param>
    /// <param name="isMultiValue">true if key is comma-separated list of values</param>
    /// <returns>found text value</returns>
    public string GetDescriptionByValue(
      CxDbConnection connection,
      object value,
      bool isMultiValue)
    {
      if (CxUtils.IsEmpty(value))
      {
        return null;
      }
      if (HardCoded)
      {
        if (!isMultiValue)
        {
          foreach (CxComboItem item in m_List)
          {
            if (CxUtils.Compare(item.Value, value))
            {
              return item.Description;
            }
          }
        }
        else
        {
          List<string> valuesList = CxText.DecomposeWithSeparator(CxUtils.ToString(value), ",");
          IDictionary valuesMap = CxList.GetDictionaryFromList(valuesList);
          string text = "";
          foreach (CxComboItem item in m_List)
          {
            if (valuesMap.Contains(CxUtils.ToString(item.Value)))
            {
              text += (text != "" ? ", " : "") + item.Description;
            }
          }
          return text;
        }
      }
      else
      {
        string keyName = KeyAttribute.Id;
        string valueName = ValueAttribute.Id;
        if (keyName == valueName)
        {
          return CxUtils.ToString(value);
        }
        if (!isMultiValue)
        {
          bool cacheWasReadNow = false;
          if (IsCached)
          {
            cacheWasReadNow = LoadCacheTable(connection);
            DataRow dr = GetCachedRow(value);
            if (dr != null)
            {
              return CxUtils.ToString(dr[valueName]);
            }
            SetEmptyCachedRow(value);
          }
          if (!cacheWasReadNow)
          {
            string pkWhere = keyName + " = :" + keyName;
            object[] pkValues = new object[] { value };
            CxGenericDataTable dt = new CxGenericDataTable();
            EntityUsage.ReadData(connection, dt, pkWhere, pkValues);
            if (dt.Rows.Count > 0)
            {
              if (IsCached)
              {
                UpdateCachedRow(dt.Rows[0], EntityUsage);
              }
              return CxUtils.ToString(dt.Rows[0][valueName]);
            }
          }
        }
        else
        {
          string text = "";
          IList<string> valuesList = CxText.DecomposeWithSeparator(CxUtils.ToString(value), ",");
          bool cacheWasReadNow = false;
          if (IsCached)
          {
            bool isPresentInCache = true;
            cacheWasReadNow = LoadCacheTable(connection);
            foreach (string s in valuesList)
            {
              DataRow dr = GetCachedRow(s);
              if (dr != null)
              {
                text += (text != "" ? ", " : "") + CxUtils.ToString(dr[valueName]);
              }
              else
              {
                SetEmptyCachedRow(s);
                isPresentInCache = false;
              }
            }
            if (isPresentInCache)
            {
              return text;
            }
          }
          if (!cacheWasReadNow)
          {
            string sqlValuesList = "";
            foreach (string s in valuesList)
            {
              sqlValuesList += (sqlValuesList != "" ? "," : "") + CxText.GetQuotedString(s, '\'', "''");
            }
            CxGenericDataTable dt = new CxGenericDataTable();
            EntityUsage.ReadData(
              connection,
              dt,
              keyName + " IN (" + sqlValuesList + ")",
              null,
              EntityUsage.OrderByClause,
              -1);
            foreach (DataRow dr in dt.Rows)
            {
              text += (text != "" ? ", " : "") + CxUtils.ToString(dr[valueName]);
              if (IsCached)
              {
                UpdateCachedRow(dr, EntityUsage);
              }
            }
          }
          return text;
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns row source image ID corresponding to the given value.
    /// </summary>
    /// <param name="value">value to find</param>
    /// <returns>image ID</returns>
    public string GetHardCodedImageIdByValue(object value)
    {
      if (HardCoded && CxUtils.NotEmpty(value))
      {
        foreach (CxComboItem item in m_List)
        {
          if (CxUtils.Compare(item.Value, value))
          {
            return item.ImageReference;
          }
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns default value provided by the row source.
    /// If valueToTry is not empty and the value exists in the row source list,
    /// returns valueToTry. If valueToTry is empty or does not exist in the row
    /// source list, returns the first existing row source value.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="filterCondition">row source filter to apply</param>
    /// <param name="valueProvider">value provider for row source filter condition</param>
    /// <param name="isMandatory">true if row source does not have empty row</param>
    /// <param name="isMultiValue">true if target attribute supports comma-separated values list</param>
    /// <param name="valueToTry">value to try to return</param>
    /// <param name="defaultValue">default value to be got</param>
    /// <param name="defaultText">default text to be got</param>
    /// <returns>default value provided by the row source</returns>
    public void GetDefaultValue(
      CxDbConnection connection,
      string filterCondition,
      IxValueProvider valueProvider,
      bool isMandatory,
      bool isMultiValue,
      object valueToTry,
      out object defaultValue,
      out string defaultText)
    {
      defaultValue = null;
      defaultText = null;
      if (isMultiValue)
      {
        return;
      }
      if (HardCoded)
      {
        if (CxUtils.NotEmpty(valueToTry))
        {
          foreach (CxComboItem item in m_List)
          {
            if (CxUtils.Compare(item.Value, valueToTry))
            {
              defaultValue = valueToTry;
              defaultText = item.Description;
              return;
            }
          }
        }
        if (isMandatory && m_List.Count > 0)
        {
          defaultValue = m_List[0].Value;
          defaultText = m_List[0].Description;
          return;
        }
      }
      else
      {
        string keyName = KeyAttribute != null ? KeyAttribute.Id : KeyAttributeId;
        string valueName = ValueAttribute != null ? ValueAttribute.Id : NameAttributeId;
        CxGenericDataTable dt = new CxGenericDataTable();
        if (CxUtils.NotEmpty(valueToTry))
        {
          bool cacheWasReadNow = false;
          if (IsCached && CxUtils.IsEmpty(filterCondition))
          {
            cacheWasReadNow = LoadCacheTable(connection);
            DataRow dr = GetCachedRow(valueToTry);
            if (dr != null)
            {
              defaultValue = valueToTry;
              defaultText = CxUtils.ToString(dr[valueName]);
              return;
            }
            SetEmptyCachedRow(valueToTry);
          }
          if (!cacheWasReadNow)
          {
            string where = CxDbUtils.ComposeWhereClause(filterCondition, keyName + " = :RowSource$PrimaryKeyValue");
            valueProvider["RowSource$PrimaryKeyValue"] = valueToTry;
            EntityUsage.ReadData(connection, dt, where, valueProvider);
            if (dt.Rows.Count > 0)
            {
              defaultValue = valueToTry;
              defaultText = CxUtils.ToString(dt.Rows[0][valueName]);
              if (IsCached && CxUtils.IsEmpty(filterCondition))
              {
                UpdateCachedRow(dt.Rows[0], EntityUsage);
              }
              return;
            }
          }
        }
        if (isMandatory)
        {
          if (IsCached && CxUtils.IsEmpty(filterCondition))
          {
            LoadCacheTable(connection);
            if (m_CacheTable.Rows.Count > 0)
            {
              defaultValue = m_CacheTable.Rows[0][keyName];
              defaultText = CxUtils.ToString(m_CacheTable.Rows[0][valueName]);
            }
            return;
          }
          EntityUsage.ReadData(connection, dt, filterCondition, valueProvider, EntityUsage.OrderByClause, 1);
          if (dt.Rows.Count > 0)
          {
            defaultValue = dt.Rows[0][keyName];
            defaultText = CxUtils.ToString(dt.Rows[0][valueName]);
            return;
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns default value provided by the row source.
    /// If valueToTry is not empty and the value exists in the row source list,
    /// returns valueToTry. If valueToTry is empty or does not exist in the row
    /// source list, returns the first existing row source value.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="filterCondition">row source filter to apply</param>
    /// <param name="valueProvider">value provider for row source filter condition</param>
    /// <param name="isMandatory">true if row source does not have empty row</param>
    /// <param name="isMultiValue">true if target attribute supports comma-separated values list</param>
    /// <param name="valueToTry">value to try to return</param>
    /// <returns>default value provided by the row source</returns>
    public object GetDefaultValue(
      CxDbConnection connection,
      string filterCondition,
      IxValueProvider valueProvider,
      bool isMandatory,
      bool isMultiValue,
      object valueToTry)
    {
      object defaultValue;
      string defaultText;
      GetDefaultValue(
        connection, filterCondition, valueProvider, isMandatory, isMultiValue, valueToTry,
        out defaultValue, out defaultText);
      return defaultValue;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns table the with row source data.
    /// </summary>
    /// <param name="owner">owner of the row source</param>
    /// <param name="connection">database connetion to use</param>
    /// <param name="filterCondition">a filter condition to be used while getting data table</param>
    /// <param name="valueProvider">provider of parameter values</param>
    /// <param name="addEmptyRow">if true adds emty row as a first one</param>
    /// <returns>table the with row source data</returns>
    public CxGenericDataTable GetDataTable(IxRowSourceOwner owner,
                                  CxDbConnection connection,
                                  string filterCondition,
                                  IxValueProvider valueProvider,
                                  bool addEmptyRow)
    {
      CxGenericDataTable dt = LoadDataTable(connection, filterCondition, valueProvider, addEmptyRow);
      if (IsHierarchical && EntityUsage.IsSelfReferencing && IsHierarchyOrphansVisible)
      {
        MoveHierarchyOrphansToFirstLevel(
          dt,
          EntityUsage.PrimaryKeyAttribute.Id,
          EntityUsage.SelfReferenceAttrId);
      }
      return dt;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads data table.
    /// </summary>
    /// <param name="connection">database connetion to use</param>
    /// <param name="filterCondition">row source filter to apply</param>
    /// <param name="valueProvider">provider of parameter values</param>
    /// <param name="addEmptyRow">if true adds emty row as a first one</param>
    /// <returns>table the with row source data</returns>
    protected CxGenericDataTable LoadDataTable(CxDbConnection connection,
                                      string filterCondition,
                                      IxValueProvider valueProvider,
                                      bool addEmptyRow)
    {
      CxGenericDataTable table = new CxGenericDataTable();
      LoadDataTable(table, connection, filterCondition, valueProvider, addEmptyRow);
      return table;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads data table.
    /// </summary>
    /// <param name="table">data table to load data in</param>
    /// <param name="connection">database connetion to use</param>
    /// <param name="filterCondition">row source filter to apply</param>
    /// <param name="valueProvider">provider of parameter values</param>
    /// <param name="addEmptyRow">if true adds emty row as a first one</param>
    public void LoadDataTable(
      DataTable table,
      CxDbConnection connection,
      string filterCondition,
      IxValueProvider valueProvider,
      bool addEmptyRow)
    {
      LoadDataTable(table, connection, filterCondition, valueProvider, addEmptyRow, null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads data table.
    /// </summary>
    /// <param name="table">data table to load data in</param>
    /// <param name="connection">database connetion to use</param>
    /// <param name="filterCondition">row source filter to apply</param>
    /// <param name="valueProvider">provider of parameter values</param>
    /// <param name="addEmptyRow">if true adds emty row as a first one</param>
    /// <param name="customPreProcessHandler">a custom pre-processing handler to be applied to
    /// the data obtained (null if no pre-processing should be done)</param>
    virtual public void LoadDataTable(
      DataTable table,
      CxDbConnection connection,
      string filterCondition,
      IxValueProvider valueProvider,
      bool addEmptyRow,
      DxCustomPreProcessDataSource customPreProcessHandler)
    {
      if (valueProvider == null)
      {
        valueProvider = new CxHashtable();
      }

      if (IsCached && CxUtils.IsEmpty(filterCondition))
      {
        LoadCacheTable(connection);
        CxData.CopyDataTable(m_CacheTable, table);
      }
      else
      {
        table.Clear();
        table.Columns.Clear();
        if (IsCustomDataProvided)
        {
          LoadCustomData(connection, table, filterCondition, valueProvider, customPreProcessHandler);
        }
        else
        {
          EntityUsage.ReadData(connection, table, filterCondition, valueProvider, customPreProcessHandler);
        }
      }

      if (addEmptyRow)
      {
        table.Rows.InsertAt(table.NewRow(), 0);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads the custom data from the custom data provider.
    /// Will throw an exception if no custom data provider available.
    /// Please use IsCustomDataProvided to eliminate it.
    /// </summary>
    private void LoadCustomData(
      CxDbConnection connection, DataTable table,
      string filterCondition, IxValueProvider valueProvider,
      DxCustomPreProcessDataSource customPreProcessHandler)
    {
      MethodInfo method = CustomDataProviderClassMetadata.Class.GetMethod(
        CustomDataProviderMethodName,
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
        null,
        new Type[] 
            { 
              typeof(CxDbConnection), 
              typeof(DataTable), 
              typeof(string), 
              typeof(IxValueProvider), 
              typeof(DxCustomPreProcessDataSource) 
            },
        null);
      if (method == null)
      {
        throw new ExException(
          "Static method '" + CustomDataProviderMethodName +
          "' is not found for row source '" + Id + "'.");
      }
      method.Invoke(null, new object[] { connection, table, filterCondition, valueProvider, customPreProcessHandler });
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if custom data is available for the row-source
    /// </summary>
    private bool IsCustomDataProvided
    {
      get
      {
        return CxUtils.NotEmpty(CustomDataProviderMethodName) &&
            CustomDataProviderClassMetadata != null &&
            CustomDataProviderClassMetadata.Class != null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads cache table if it is was not loaded yet.
    /// </summary>
    /// <returns>true if table was read from database</returns>
    protected bool LoadCacheTable(CxDbConnection connection)
    {
      if (m_CacheTable == null)
      {
        CxGenericDataTable dt = new CxGenericDataTable();
        string orderByClause = string.Empty;

        if (IsCustomDataProvided)
        {
          LoadCustomData(connection, dt, null, null, null);
        }
        else
        {
          EntityUsage.ReadData(connection, dt);
          orderByClause = EntityUsage.OrderByClause;
        }

        if (CxUtils.NotEmpty(orderByClause))
        {
          using (DataView dv = new DataView(dt, "", orderByClause, DataViewRowState.CurrentRows))
          {
            m_CacheTable = dv.ToTable();
          }
        }
        else
        {
          m_CacheTable = dt;
        }
        m_CacheValueRowMap.Clear();
        m_CacheValueEmptyRowMap.Clear();
        string keyAttributeId = KeyAttribute != null ? KeyAttribute.Id : KeyAttributeId;
        foreach (DataRow dr in m_CacheTable.Rows)
        {
          string value = CxUtils.ToString(dr[keyAttributeId]);
          m_CacheValueRowMap[value] = dr;
        }
        return true;
      }
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns copy of cached data table or null.
    /// </summary>
    public DataTable GetCacheTable()
    {
      if (m_CacheTable != null)
      {
        CxGenericDataTable dt = new CxGenericDataTable();
        CxData.CopyDataTable(m_CacheTable, dt);
        return dt;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Resets cached lookup data.
    /// </summary>
    public void ResetCache()
    {
      m_CacheValueRowMap.Clear();
      m_CacheValueEmptyRowMap.Clear();
      if (m_CacheTable != null)
      {
        m_CacheTable = null;
        DoOnCachedRowChanged();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns cached row by the key value.
    /// </summary>
    /// <param name="value">primary key value</param>
    /// <returns>found row or null</returns>
    protected DataRow GetCachedRow(object value)
    {
      if (CxUtils.NotEmpty(value))
      {
        DataRow dr;
        if (m_CacheValueRowMap.TryGetValue(CxUtils.ToString(value), out dr))
        {
          return dr;
        }
        if (m_CacheValueEmptyRowMap.TryGetValue(CxUtils.ToString(value), out dr))
        {
          return dr;
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns cached data row if corresponds to the given value provider and entity usage.
    /// </summary>
    /// <param name="provider">value provider</param>
    /// <param name="entityUsage">entity usage metadata</param>
    /// <returns>data row or null</returns>
    protected DataRow GetCachedRow(IxValueProvider provider, CxEntityUsageMetadata entityUsage)
    {
      if (entityUsage != null && entityUsage.Entity.Id != EntityUsage.Entity.Id)
      {
        // Different entity is not compatible.
        return null;
      }
      string value = CxUtils.ToString(provider[KeyAttribute.Id]);
      if (CxUtils.NotEmpty(value))
      {
        DataRow dr;
        if (m_CacheValueRowMap.TryGetValue(value, out dr))
        {
          // Make sure it is the same data row.
          if (entityUsage == null ||
              entityUsage.CompareByPK(provider, new CxDataRowValueProvider(dr)))
          {
            return dr;
          }
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates empty data row for the given value and adds this row to cache.
    /// </summary>
    /// <param name="value">key value</param>
    protected void SetEmptyCachedRow(object value)
    {
      if (m_CacheTable != null &&
          CxUtils.NotEmpty(value) &&
          !m_CacheValueRowMap.ContainsKey(CxUtils.ToString(value)) &&
          !m_CacheValueEmptyRowMap.ContainsKey(CxUtils.ToString(value)))
      {
        DataRow dr = m_CacheTable.NewRow();
        dr[KeyAttribute.Id] = value;
        m_CacheValueEmptyRowMap[CxUtils.ToString(value)] = dr;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Updates (or inserts) cached data row from the value provider values.
    /// </summary>
    /// <param name="provider">value provider to get values from</param>
    /// <param name="entityUsage">metadata of the value provider</param>
    /// <param name="insertIfNotFound">true to add row to cache if such row is not found in cache</param>
    public void UpdateCachedRow(
      IxValueProvider provider,
      CxEntityUsageMetadata entityUsage,
      bool insertIfNotFound)
    {
      if (m_CacheTable == null || provider == null)
      {
        return;
      }
      string value = CxUtils.ToString(provider[KeyAttribute.Id]);
      if (CxUtils.NotEmpty(value))
      {
        DataRow dr = GetCachedRow(provider, entityUsage);
        if (dr == null)
        {
          if (insertIfNotFound)
          {
            dr = m_CacheTable.NewRow();
            m_CacheTable.Rows.Add(dr);
            m_CacheValueRowMap[value] = dr;
          }
          else
          {
            return;
          }
        }
        bool isChanged = false;
        foreach (DataColumn dc in m_CacheTable.Columns)
        {
          object rowValue = CxUtils.Nvl(provider[dc.ColumnName], DBNull.Value);
          if (!isChanged && !CxUtils.Compare(dr[dc], rowValue))
          {
            isChanged = true;
          }
          dr[dc] = rowValue;
        }
        m_CacheValueEmptyRowMap.Remove(value);
        if (isChanged)
        {
          DoOnCachedRowChanged();
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Updates (or inserts) cached data row from the value provider values.
    /// </summary>
    /// <param name="provider">value provider to get values from</param>
    /// <param name="entityUsage">metadata of the value provider</param>
    public void UpdateCachedRow(IxValueProvider provider, CxEntityUsageMetadata entityUsage)
    {
      UpdateCachedRow(provider, entityUsage, true);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Updates (or inserts) cached data row from the value provider values.
    /// </summary>
    /// <param name="row">data row to get values from</param>
    /// <param name="entityUsage">metadata of the value provider</param>
    /// <param name="insertIfNotFound">true to add row to cache if such row is not found in cache</param>
    public void UpdateCachedRow(
      DataRow row,
      CxEntityUsageMetadata entityUsage,
      bool insertIfNotFound)
    {
      if (row != null)
      {
        UpdateCachedRow(new CxDataRowValueProvider(row), entityUsage, insertIfNotFound);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Updates (or inserts) cached data row from the value provider values.
    /// </summary>
    /// <param name="row">data row to get values from</param>
    /// <param name="entityUsage">metadata of the value provider</param>
    public void UpdateCachedRow(DataRow row, CxEntityUsageMetadata entityUsage)
    {
      UpdateCachedRow(row, entityUsage, true);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Deletes cached data row with the primary key defined in the value provider.
    /// </summary>
    /// <param name="provider">value provider to get primary key from</param>
    /// <param name="entityUsage">metadata of the value provider</param>
    public void DeleteCachedRow(IxValueProvider provider, CxEntityUsageMetadata entityUsage)
    {
      if (m_CacheTable == null || provider == null)
      {
        return;
      }
      string value = CxUtils.ToString(provider[KeyAttribute.Id]);
      if (CxUtils.NotEmpty(value))
      {
        DataRow dr = GetCachedRow(provider, entityUsage);
        if (dr != null)
        {
          m_CacheTable.Rows.Remove(dr);
          m_CacheValueRowMap.Remove(value);
        }
        m_CacheValueEmptyRowMap.Remove(value);
        DoOnCachedRowChanged();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Deletes cached data row with the primary key defined in the value provider.
    /// </summary>
    /// <param name="row">data row to get primary key from</param>
    /// <param name="entityUsage">metadata of the value provider</param>
    public void DeleteCachedRow(DataRow row, CxEntityUsageMetadata entityUsage)
    {
      if (row != null)
      {
        DeleteCachedRow(new CxDataRowValueProvider(row), entityUsage);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Does actions on cached row changed or deleted.
    /// </summary>
    protected void DoOnCachedRowChanged()
    {
      if (Holder != null)
      {
        Holder.DoOnRowSourceCachedRowChanged(this);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Attribute that serves as primary key in row source.
    /// </summary>
    public string KeyAttributeId
    {
      get { return this["key_attr_id"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Attribute to show as name (text) in the control.
    /// </summary>
    public string NameAttributeId
    {
      get { return this["name_attr_id"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Attribute that serves as primary key in row source.
    /// </summary>
    public CxAttributeMetadata KeyAttribute
    {
      get
      {
        CxEntityUsageMetadata entityUsage = EntityUsage;
        if (entityUsage != null)
        {
          if (CxUtils.NotEmpty(KeyAttributeId))
          {
            CxAttributeMetadata attribute = entityUsage.GetAttribute(KeyAttributeId);
            if (attribute == null)
            {
              throw new ExMetadataException(String.Format(
                "Could not get key attribute for the row source with ID='{0}'. Attribute with ID='{1}' could not be found.", Id, KeyAttributeId));
            }
            return attribute;
          }
          CxAttributeMetadata[] pkAttributes = entityUsage.PrimaryKeyAttributes;
          if (pkAttributes.Length > 0)
          {
            return pkAttributes[0];
          }
          else if (entityUsage.Attributes.Count > 0)
          {
            return entityUsage.Attributes[0];
          }
          throw new ExMetadataException(String.Format(
            "Could not get key attribute for the row source with ID='{0}'. Entity usage does not have attributes.", Id));
        }
        return null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Attribute to show as value of control.
    /// </summary>
    public CxAttributeMetadata ValueAttribute
    {
      get
      {
        CxEntityUsageMetadata entityUsage = EntityUsage;
        if (entityUsage != null)
        {
          if (CxUtils.NotEmpty(NameAttributeId))
          {
            CxAttributeMetadata attribute = entityUsage.GetAttribute(NameAttributeId);
            if (attribute == null)
            {
              throw new ExMetadataException(String.Format(
                "Could not get value (name) attribute for the row source with ID='{0}'. Attribute with ID='{1}' could not be found.", Id, NameAttributeId));
            }
            return attribute;
          }
          else if (entityUsage.NameAttribute != null)
          {
            return entityUsage.NameAttribute;
          }
          else if (entityUsage.Attributes.Count > 1)
          {
            return entityUsage.Attributes[1];
          }
          else if (entityUsage.Attributes.Count > 0)
          {
            return entityUsage.Attributes[0];
          }
          throw new ExMetadataException(String.Format(
            "Could not get value (name) attribute for the row source with ID='{0}'. Entity usage does not have attributes.", Id));
        }
        return null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the list of entities that use current row-source in some way.
    /// </summary>
    /// <returns></returns>
    public IList<CxLookupUsage> GetLookupUsages()
    {
      List<CxLookupUsage> result = new List<CxLookupUsage>();
      foreach (CxEntityUsageMetadata entityUsageMetadata in Holder.EntityUsages.Items)
      {
        foreach (string attributeId in CxMetadataObject.ExtractIds(entityUsageMetadata.Attributes))
        {
          CxAttributeMetadata attributeMetadata = entityUsageMetadata.GetAttribute(attributeId);
          if (!string.IsNullOrEmpty(attributeMetadata.RowSourceId) && CxText.Equals(attributeMetadata.RowSourceId, Id))
          {
            bool doInclude = entityUsageMetadata.Customizable;
            if (!doInclude)
            {
              foreach (CxEntityUsageMetadata entityUsage in entityUsageMetadata.FindParentEntityUsages())
              {
                foreach (CxChildEntityUsageMetadata childEntityUsageMetadata in entityUsage.ChildEntityUsagesList)
                {
                  if (childEntityUsageMetadata.EntityUsage == entityUsageMetadata && childEntityUsageMetadata.Customizable)
                  {
                    doInclude = true;
                    break;
                  }
                }
                if (doInclude)
                  break;
              }
            }

            if (doInclude) result.Add(new CxLookupUsage(entityUsageMetadata, attributeMetadata));
          }
        }
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Type of the key value for the hard-coded row sources.
    /// </summary>
    public string KeyType
    { get { return this["key_type"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the row source is customizable.
    /// </summary>
    public bool Customizable
    {
      get { return CxBool.Parse(this["customizable"], false); }
    }
    //-------------------------------------------------------------------------
  }
}