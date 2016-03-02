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
using Framework.Db;
using Framework.Utils;

namespace Framework.Metadata
{
  /// <summary>
  /// Class for managing multilanguage texts
  /// </summary>
  public class CxMultilanguage
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Predefined object type codes.
    /// </summary>
    public const string OTC_TEXT  = "Text";
    public const string OTC_ERROR = "Error";
    //-------------------------------------------------------------------------
    /// <summary>
    /// Predefined property codes.
    /// </summary>
    public const string PC_TEXT = "Text";
    //-------------------------------------------------------------------------
    public const string DEFAULT_LANGUAGE = "EN"; // Default language code
    //-------------------------------------------------------------------------
    protected CxMetadataHolder m_Holder = null;
    protected string m_ApplicationCode = null;
    protected string m_LocalizationApplicationCode = null;
    protected string m_LanguageCode = DEFAULT_LANGUAGE;
    protected bool m_IsLoaded = false;
    private Hashtable m_ObjectProperties = new Hashtable();
    private Hashtable m_LocalizedValues = new Hashtable();
    private Hashtable m_DictionaryValues = new Hashtable();
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    public CxMultilanguage(CxMetadataHolder holder)
    {
      m_Holder = holder;
      m_ApplicationCode = m_Holder.ApplicationCode;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Reads multilanguage data from the database
    /// </summary>
    public void ReadFromDatabase(CxDbConnection connection, string localizationApplicationCode)
    {
      m_LocalizationApplicationCode = localizationApplicationCode;
      if (CxUtils.IsEmpty(m_LocalizationApplicationCode))
      {
        m_LocalizationApplicationCode = m_ApplicationCode;
      }

            string appCode = Convert.ToString( m_Holder.SlSections.JsAppProperties["app_name"]);
            if (string.IsNullOrWhiteSpace(appCode))
                appCode = m_LocalizationApplicationCode;

      // Load object type and property dictionary
            ObjectProperties.Clear();

      DataTable dtObjectProperties = new DataTable();
      connection.GetQueryResult(
        dtObjectProperties,
        @"select t.*
            from Framework_LocalizationObjectProperties t
           where t.ApplicationCd = :ApplicationCd OR t.ApplicationCd = :AppCode",
        m_LocalizationApplicationCode, appCode);

      foreach (DataRow dr in dtObjectProperties.Rows)
      {
        string key = dr["ObjectTypeCd"].ToString().ToUpper() + "." + 
                     dr["PropertyCd"].ToString().ToUpper();
        string value = dr["Name"].ToString();
        ObjectProperties[key] = value;
      }

      // Load localized values
      LocalizedValues.Clear();

      DataTable dtLocalizedValues = new DataTable();
      connection.GetQueryResult(
        dtLocalizedValues,
        @"select t.*
            from Framework_LocalizedValues t
           where t.ApplicationCd = :ApplicationCd OR t.ApplicationCd = :AppCode",
        m_LocalizationApplicationCode, appCode);

      foreach (DataRow dr in dtLocalizedValues.Rows)
      {
        string key = dr["LanguageCd"].ToString().ToUpper() + "." +
                     dr["ObjectTypeCd"].ToString().ToUpper() + "." +
                     dr["PropertyCd"].ToString().ToUpper() + "." +
                     dr["ObjectName"].ToString().ToUpper();
        string value = dr["Value"].ToString();
        LocalizedValues[key] = value;
      }

      // Load localization dictionary values
      DictionaryValues.Clear();
      DataTable dtDictionaryValues = new DataTable();
      connection.GetQueryResult(
        dtDictionaryValues,
        @"select t.*
            from Framework_LocalizationDictionary t
           where t.ApplicationCd = :ApplicationCd OR t.ApplicationCd = :AppCode",
        m_LocalizationApplicationCode, appCode);

      foreach (DataRow dr in dtDictionaryValues.Rows)
      {
        string key = dr["LanguageCd"].ToString().ToUpper() + "." +
                     dr["DefaultValue"].ToString().ToUpper();
        string value = dr["Value"].ToString();
        DictionaryValues[key] = value;
      }

      IsLoaded = true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns localized value for the given object type, property and object name.
    /// Returns null if value is not localized.
    /// </summary>
    /// <param name="languageCode">language code</param>
    /// <param name="objectTypeCode">code of object type</param>
    /// <param name="propertyCode">property code</param>
    /// <param name="objectName">object unique name</param>
    /// <param name="defaultValue">current (default) property value</param>
    /// <returns>localized value</returns>
    public string GetLocalizedValue(
      string languageCode,
      string objectTypeCode,
      string propertyCode,
      string objectName,
      string defaultValue)
    {
      if (IsLocalizable(objectTypeCode, propertyCode))
      {

        string key = CxText.ToUpper(languageCode) + "." +
                     CxText.ToUpper(objectTypeCode) + "." +
                     CxText.ToUpper(propertyCode) + "." +
                     CxText.ToUpper(objectName);
        string value = (string)LocalizedValues[key];
        if (value == null)
        {
          key = CxText.ToUpper(languageCode) + "." +
                CxText.ToUpper(defaultValue);
          value = (string)DictionaryValues[key];
        }

        //string keyDefault = CxText.ToUpper("EN") + "." +
        //             CxText.ToUpper(objectTypeCode) + "." +
        //             CxText.ToUpper(propertyCode) + "." +
        //             CxText.ToUpper(objectName);
        //if (LocalizedValues[keyDefault] == null)
        //  LocalizedValues[keyDefault] = defaultValue;

        return value;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns localized value for the given object type, property and object name.
    /// Returns null if value is not localized.
    /// </summary>
    /// <param name="objectTypeCode">code of object type</param>
    /// <param name="propertyCode">property code</param>
    /// <param name="objectName">object unique name</param>
    /// <param name="defaultValue">current (default) property value</param>
    /// <returns>localized value</returns>
    public string GetLocalizedValue(
      string objectTypeCode,
      string propertyCode,
      string objectName,
      string defaultValue)
    {
      return GetLocalizedValue(LanguageCode, objectTypeCode, propertyCode, objectName, defaultValue);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns localized value for the given object type, property and object name.
    /// Returns currentValue if value is not localized.
    /// </summary>
    /// <param name="languageCode">language code</param>
    /// <param name="objectTypeCode">code of object type</param>
    /// <param name="propertyCode">property code</param>
    /// <param name="objectName">object unique name</param>
    /// <param name="defaultValue">current (default) property value</param>
    /// <returns>localized value</returns>
    public string GetValue(
      string languageCode,
      string objectTypeCode,
      string propertyCode,
      string objectName,
      string defaultValue)
    {
      string localizedValue =
        GetLocalizedValue(languageCode, objectTypeCode, propertyCode, objectName, defaultValue);
      return localizedValue ?? defaultValue;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns localized value for the given object type, property and object name.
    /// Returns currentValue if value is not localized.
    /// </summary>
    /// <param name="objectTypeCode">code of object type</param>
    /// <param name="propertyCode">property code</param>
    /// <param name="objectName">object unique name</param>
    /// <param name="currentValue">current (default) property value</param>
    /// <returns>localized value</returns>
    public string GetValue(
      string objectTypeCode,
      string propertyCode,
      string objectName,
      string currentValue)
    {
      return GetValue(LanguageCode, objectTypeCode, propertyCode, objectName, currentValue);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns localized value for the given multilanguage item.
    /// </summary>
    /// <param name="item">multilanguage item</param>
    /// <param name="defaultValue">current (default) property value</param>
    /// <returns>localized value</returns>
    public string GetValue(CxMultilanguageItem item, string defaultValue)
    {
      return item != null ? 
        GetValue(item.ObjectTypeCd, item.PropertyCd, item.ObjectName, defaultValue) : defaultValue;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if property with the given code of the object of the given type
    /// is registered as localizable.
    /// </summary>
    /// <param name="objectTypeCode">code of the object type</param>
    /// <param name="propertyCode">property code</param>
    public bool IsLocalizable(string objectTypeCode, string propertyCode)
    {
      string key = CxText.ToUpper(objectTypeCode + "." + propertyCode);
      return ObjectProperties.ContainsKey(key);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if value is localized.
    /// </summary>
    /// <param name="objectTypeCode">code of the object type</param>
    /// <param name="propertyCode">property code</param>
    /// <param name="objectName">unique object name</param>
    /// <param name="currentValue">current (default) value</param>
    public bool IsLocalized(
      string objectTypeCode,
      string propertyCode,
      string objectName,
      string currentValue)
    {
      string localizedValue = 
        GetLocalizedValue(objectTypeCode, propertyCode, objectName, currentValue);
      return localizedValue != null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Exports all non-translated maultilanguage items into the given datatable.
    /// </summary>
    /// <param name="connection">db connection to multilanguage tables</param>
    /// <param name="dt">target data table</param>
    /// <param name="languageCd">language code to export (current if empty)</param>
    /// <param name="applicationCd">application code to export (default if empty)</param>
    public void ExportNonTranslatedItems(
      CxDbConnection connection,
      DataTable dt,
      string languageCd,
      string applicationCd)
    {
      if (CxUtils.IsEmpty(languageCd))
      {
        languageCd = LanguageCode;
      }
      if (CxUtils.IsEmpty(applicationCd))
      {
        applicationCd = LocalizationApplicationCode;
      }

      string sql =
        @"select t.ApplicationCd,
                 t.LanguageCd,
                 t.ObjectTypeCd,
                 t.PropertyCd,
                 t.ObjectName,
                 t.ObjectTypeName,
                 t.PropertyName,
                 t.OriginalValue,
                 t.TranslatedValue,
                 t.DictionaryValue
            from v_Framework_LocalizationValues t
           where t.ApplicationCd = :ApplicationCd
             and t.LanguageCd = :LanguageCd
             and ((t.TranslatedValue is null and t.DictionaryValue is null) 
                  or 
                  t.IsNotSynchronized = 1)
           order by t.ApplicationCd, t.LanguageCd, t.ObjectTypeCd, t.PropertyCd, t.ObjectName";

      CxHashtable valueProvider = new CxHashtable();
      valueProvider["ApplicationCd"] = applicationCd;
      valueProvider["LanguageCd"] = languageCd;

      connection.GetQueryResult(dt, sql, valueProvider);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Exports all translated maultilanguage items into the given datatable.
    /// </summary>
    /// <param name="connection">db connection to multilanguage tables</param>
    /// <param name="dt">target data table</param>
    /// <param name="languageCd">language code to export (current if empty)</param>
    /// <param name="applicationCd">application code to export (default if empty)</param>
    public void ExportTranslatedItems(
      CxDbConnection connection,
      DataTable dt,
      string languageCd,
      string applicationCd)
    {
      if (CxUtils.IsEmpty(languageCd))
      {
        languageCd = LanguageCode;
      }
      if (CxUtils.IsEmpty(applicationCd))
      {
        applicationCd = LocalizationApplicationCode;
      }

      string sql =
        @"select t.ApplicationCd,
                 t.LanguageCd,
                 t.ObjectTypeCd,
                 t.PropertyCd,
                 t.ObjectName,
                 t.ObjectTypeName,
                 t.PropertyName,
                 t.OriginalValue,
                 case when t.DictionaryValue is null then t.TranslatedValue else null end as TranslatedValue,
                 t.DictionaryValue
            from v_Framework_LocalizationValues t
           where t.ApplicationCd = :ApplicationCd
             and t.LanguageCd = :LanguageCd
             and ((t.TranslatedValue is not null or t.DictionaryValue is not null) 
                  and (t.IsNotSynchronized = 0 or t.IsNotSynchronized is null))
           order by t.ApplicationCd, t.LanguageCd, t.ObjectTypeCd, t.PropertyCd, t.ObjectName";

      CxHashtable valueProvider = new CxHashtable();
      valueProvider["ApplicationCd"] = applicationCd;
      valueProvider["LanguageCd"] = languageCd;

      connection.GetQueryResult(dt, sql, valueProvider);
    }
    //-------------------------------------------------------------------------
    private List<string> GetKeysStartingFrom(Hashtable hashtable, string startsWith)
    {
      List<string> result = new List<string>();
      foreach (string key in hashtable.Keys)
      {
        if (key.StartsWith(startsWith))
          result.Add(key);
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Imports translated multilanguage items from the data table.
    /// DataTable must have the following columns:
    ///   ApplicationCd,
    ///   LanguageCd,
    ///   ObjectTypeCd,
    ///   PropertyCd,
    ///   ObjectName,
    ///   OriginalValue,
    ///   TranslatedValue,
    ///   DictionaryValue
    /// </summary>
    /// <param name="connection">DB connection to the multilanguage tables</param>
    /// <param name="dt">data table with translated values</param>
    /// <param name="languageCd">language to import (or null to import all records)</param>
    /// <param name="applicationCd">application to import (or null to import all records)</param>
    /// <param name="doOverwrite">do overwrite existing translations</param>
    /// <param name="importLogText">string to put import log</param>
    public void ImportTranslatedItems(
      CxDbConnection connection,
      DataTable dt,
      string languageCd,
      string applicationCd,
      bool doOverwrite,
      out string importLogText)
    {
      if (dt.Rows.Count == 0)
      {
        importLogText = m_Holder.GetTxt("No rows imported. Source table is empty.");
        return;
      }

      // Check required columns
      string[] requiredColumns = 
        {"ApplicationCd", "LanguageCd", "ObjectTypeCd", "PropertyCd", "ObjectName",
         "OriginalValue", "TranslatedValue", "DictionaryValue"};

      foreach (string columnName in requiredColumns)
      {
        if (!dt.Columns.Contains(columnName))
        {
          throw new ExValidationException(
            m_Holder.GetErr("Source table is invalid. Must have column with name '{0}'.", 
            new object[]{columnName}));
        }
      }

      // Read existing multilanguage data from the database
      DataTable dtTarget = new DataTable();

      string sql =
        @"select t.ApplicationCd,
                 t.LanguageCd,
                 t.ObjectTypeCd,
                 t.PropertyCd,
                 t.ObjectName,
                 t.OriginalValue,
                 t.TranslatedValue,
                 t.IsTranslated,
                 t.IsNotSynchronized,
                 t.DictionaryValue
            from v_Framework_LocalizationValues t";

      if (CxUtils.NotEmpty(applicationCd))
      {
        CxDbUtils.AddToWhere(sql, "ApplicationCd = :ApplicationCd");
      }
      if (CxUtils.NotEmpty(languageCd))
      {
        CxDbUtils.AddToWhere(sql, "LanguageCd = :LanguageCd");
      }

      CxHashtable valueProvider = new CxHashtable();
      valueProvider["ApplicationCd"] = applicationCd;
      valueProvider["LanguageCd"] = languageCd;

      connection.GetQueryResult(dtTarget, sql, valueProvider);

      // Compose hashtables with existing multilanguage data
      Hashtable targetItems = new Hashtable();
      Hashtable targetValues = new Hashtable();
      Hashtable targetDictionary = new Hashtable();

      foreach (DataRow dr in dtTarget.Rows)
      {
        string itemKey = CxUtils.ToString(dr["ApplicationCd"]).ToUpper() + "." +
                         CxUtils.ToString(dr["ObjectTypeCd"]).ToUpper() + "." +
                         CxUtils.ToString(dr["PropertyCd"]).ToUpper() + "." +  
                         CxUtils.ToString(dr["ObjectName"]).ToUpper();
        targetItems[itemKey] = true;

        if (CxBool.Parse(dr["IsTranslated"]))
        {
          string valueKey = CxUtils.ToString(dr["ApplicationCd"]).ToUpper() + "." +
                            CxUtils.ToString(dr["LanguageCd"]).ToUpper() + "." +
                            CxUtils.ToString(dr["ObjectTypeCd"]).ToUpper() + "." +
                            CxUtils.ToString(dr["PropertyCd"]).ToUpper() + "." +  
                            CxUtils.ToString(dr["ObjectName"]).ToUpper();
          targetValues[valueKey] = dr;
        }

        if (CxUtils.NotEmpty(dr["DictionaryValue"]))
        {
          string dictionaryKey = CxUtils.ToString(dr["ApplicationCd"]).ToUpper() + "." +
                                 CxUtils.ToString(dr["LanguageCd"]).ToUpper() + "." +
                                 CxUtils.ToString(dr["OriginalValue"]).ToUpper();
          targetDictionary[dictionaryKey] = dr;
        }
      }

      // Initialize counters
      int importedRows = 0;
      int skippedRows = 0;
      int rejectedRows = 0;

      int rejectedByApplication = 0;
      int rejectedBySpecificError = 0;
      int rejectedByLanguage = 0;
      int rejectedByEmptyValue = 0;
      int rejectedByInvalidKey = 0;

      string errorLog = "";

      // Initialize SQL statements
      string insertValueSql =
        @"insert into Framework_LocalizedValues
          (
            ObjectTypeCd,
            ObjectName,
            LanguageCd,
            PropertyCd,
            ApplicationCd,
            Value
          )
          values
          (
            :ObjectTypeCd,
            :ObjectName,
            :LanguageCd,
            :PropertyCd,
            :ApplicationCd,
            :TranslatedValue
          )";

      string updateValueSql =
        @"update Framework_LocalizedValues
             set Value = :TranslatedValue,
                 IsNotSynchronized = 0
           where ObjectTypeCd = :ObjectTypeCd
             and ObjectName = :ObjectName
             and LanguageCd = :LanguageCd
             and PropertyCd = :PropertyCd
             and ApplicationCd = :ApplicationCd";

      string insertDictionarySql =
        @"insert into Framework_LocalizationDictionary
          (
            DefaultValue,
            ApplicationCd,
            LanguageCd,
            Value
          )
          values
          (
            :OriginalValue,
            :ApplicationCd,
            :LanguageCd,
            :DictionaryValue
          )";

      string updateDictionarySql =
        @"update Framework_LocalizationDictionary
             set Value = :DictionaryValue
           where LanguageCd = :LanguageCd
             and DefaultValue = :OriginalValue
             and ApplicationCd = :ApplicationCd";

      // Perform import
      int rowIndex = 1;
      foreach (DataRow dr in dt.Rows)
      {
        // Validate application code
        if (CxUtils.NotEmpty(applicationCd) &&
            CxUtils.ToString(dr["ApplicationCd"]).ToUpper() != CxText.ToUpper(applicationCd))
        {
          rejectedByApplication++;
          rejectedRows++;
          continue;
        }

        // Validate language code
        if (CxUtils.NotEmpty(languageCd) &&
            CxUtils.ToString(dr["LanguageCd"]).ToUpper() != CxText.ToUpper(languageCd))
        {
          rejectedByLanguage++;
          rejectedRows++;
          continue;
        }

        // Check for non-empty values
        string translatedValue = CxUtils.ToString(dr["TranslatedValue"]);
        string dictionaryValue = CxUtils.ToString(dr["DictionaryValue"]);
        if (CxUtils.IsEmpty(translatedValue) && CxUtils.IsEmpty(dictionaryValue))
        {
          rejectedByEmptyValue++;
          rejectedRows++;
          continue;
        }

        // Check for valid multilanguage item key
        string itemKey = CxUtils.ToString(dr["ApplicationCd"]).ToUpper() + "." +
                         CxUtils.ToString(dr["ObjectTypeCd"]).ToUpper() + "." +
                         CxUtils.ToString(dr["PropertyCd"]).ToUpper() + "." +  
                         CxUtils.ToString(dr["ObjectName"]).ToUpper();
        //var keys = targetItems.Keys.Cast<string>().ToList();
        //keys.Sort();
        bool isImported = false;
        bool isImportError = false;
        if (!targetItems.ContainsKey(itemKey))
        {
          if (!doOverwrite)
          {
          rejectedByInvalidKey++;
          rejectedRows++;
          string displayKey = CxText.Format(
            "ApplicationCd = {0}; ObjectTypeCd = {1}; PropertyCd = {2}; ObjectName = {3}",
            dr["ApplicationCd"], dr["ObjectTypeCd"], dr["PropertyCd"], dr["ObjectName"]);
          errorLog += m_Holder.GetErr("Row {0} import error: Invalid key ({1})\r\n", 
                                      new object[]{rowIndex, displayKey});
          continue;
        }
          else
          {
            string sqlInsertItem = "insert into Framework_LocalizationItems " +
              "  (ApplicationCd, ObjectTypeCd, PropertyCd, ObjectName, DefaultValue, IsNotUsed) " +
              " values (:ApplicationCd, :ObjectTypeCd, :PropertyCd, :ObjectName, :OriginalValue, 0)";

            connection.BeginTransaction();
            try
            {
              connection.ExecuteCommand(sqlInsertItem, new CxDataRowValueProvider(dr));
              connection.Commit();
            }
            catch (Exception ex)
            {
              connection.Rollback();
              errorLog += m_Holder.GetErr("Row {0} localization item import error: {1}\r\n",
                                          new object[] { rowIndex, ex.Message });
              isImportError = true;
            }
          }
        }

        // Try to import translated value
        if (CxUtils.NotEmpty(translatedValue))
        {
          string valueKey = CxUtils.ToString(dr["ApplicationCd"]).ToUpper() + "." +
                            CxUtils.ToString(dr["LanguageCd"]).ToUpper() + "." +
                            CxUtils.ToString(dr["ObjectTypeCd"]).ToUpper() + "." +
                            CxUtils.ToString(dr["PropertyCd"]).ToUpper() + "." +  
                            CxUtils.ToString(dr["ObjectName"]).ToUpper();

          DataRow targetRow = (DataRow) targetValues[valueKey];
          string stmt = null;
          if (targetRow == null)
          {
            stmt = insertValueSql;
          }
          else if (doOverwrite && 
                   CxUtils.ToString(targetRow["TranslatedValue"]) != translatedValue ||
                   CxBool.Parse(targetRow["IsNotSynchronized"]))
          {
            stmt = updateValueSql;
          }

          if (CxUtils.NotEmpty(stmt))
          {
            connection.BeginTransaction();
            try
            {
              connection.ExecuteCommand(stmt, new CxDataRowValueProvider(dr));
              connection.Commit();
              isImported = true;
            }
            catch (Exception e)
            {
              connection.Rollback();
              rejectedBySpecificError++;
              errorLog += m_Holder.GetErr("Row {0} translated value import error: {1}\r\n", 
                                          new object[]{rowIndex, e.Message});
              isImportError = true;
            }
          }
        }

        // Try to import dictionary value
        // Dictionary value is imported only if there is no such dictionary value in DB
        if (CxUtils.NotEmpty(dictionaryValue))
        {
          string dictionaryKey = CxUtils.ToString(dr["ApplicationCd"]).ToUpper() + "." +
                                 CxUtils.ToString(dr["LanguageCd"]).ToUpper() + "." +
                                 CxUtils.ToString(dr["OriginalValue"]).ToUpper();

          DataRow targetRow = (DataRow) targetDictionary[dictionaryKey];
          string stmt = null;
          if (targetRow == null)
          {
            stmt = insertDictionarySql;
          }
          else if (doOverwrite &&
                   CxUtils.ToString(targetRow["DictionaryValue"]) != dictionaryValue)
          {
            stmt = updateDictionarySql;
          }

          if (CxUtils.NotEmpty(stmt))
          {
            connection.BeginTransaction();
            try
            {
              connection.ExecuteCommand(stmt, new CxDataRowValueProvider(dr));
              connection.Commit();
              isImported = true;
            }
            catch (Exception e)
            {
              rejectedBySpecificError++;
              connection.Rollback();
              errorLog += m_Holder.GetErr("Row {0} dictionary value import error: {1}\r\n", 
                new object[]{rowIndex, e.Message});
              isImportError = true;
            }
          }
        }

        if (isImported)
        {
          importedRows++;
        }
        else if (isImportError)
        {
          rejectedRows++;
        }
        else
        {
          skippedRows++;
        }

        rowIndex++;
      }

      // Compose import log
      importLogText = "";
      importLogText += m_Holder.GetTxt("Total rows: {0}", new object[]{dt.Rows.Count}) + Environment.NewLine;
      importLogText += m_Holder.GetTxt("Imported rows: {0}", new object[] { importedRows }) + Environment.NewLine;
      importLogText += m_Holder.GetTxt("Skipped rows: {0}", new object[] { skippedRows }) + Environment.NewLine;
      importLogText += m_Holder.GetTxt("Rejected rows: {0}", new object[] { rejectedRows }) + Environment.NewLine + Environment.NewLine;
      importLogText += m_Holder.GetTxt("Rejected by invalid application code: {0}", new object[] { rejectedByApplication }) + Environment.NewLine;
      importLogText += m_Holder.GetTxt("Rejected by invalid language code: {0}", new object[] { rejectedByLanguage }) + Environment.NewLine;
      importLogText += m_Holder.GetTxt("Rejected by empty translation: {0}", new object[] { rejectedByEmptyValue }) + Environment.NewLine;
      importLogText += m_Holder.GetTxt("Rejected by invalid key: {0}", new object[] { rejectedByInvalidKey }) + Environment.NewLine;
      importLogText += m_Holder.GetTxt("Rejected by specific error: {0}", new object[] { rejectedBySpecificError }) + Environment.NewLine + Environment.NewLine;
      importLogText += (CxUtils.NotEmpty(errorLog) ?
                        m_Holder.GetTxt("Database errors:") + Environment.NewLine + errorLog : "");

      // Reload multilanguage data from database.
      if (IsLoaded && importedRows > 0)
      {
        ReadFromDatabase(connection, CxAppInfo.LocalizationApplicationCode);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Updates localized value in the database.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="languageCode">language code</param>
    /// <param name="objectTypeCode">object type code</param>
    /// <param name="propertyCode">property code</param>
    /// <param name="objectName">object name</param>
    /// <param name="value">value to set</param>
    public void SetLocalizedValue(
      CxDbConnection connection,
      string languageCode,
      string objectTypeCode,
      string propertyCode,
      string objectName,
      string defaultValue,
      string value)
    {
      SetLocalizedValueInMemory(languageCode, objectTypeCode, propertyCode, objectName, value);

      string sql =
        @"exec p_Framework_LocalizedValue_Insert
            @ApplicationCd = :ApplicationCd,
            @ObjectTypeCd  = :ObjectTypeCd,
            @PropertyCd    = :PropertyCd,
            @ObjectName    = :ObjectName,
            @LanguageCd    = :LanguageCd,
            @DefaultValue  = :DefaultValue,
            @Value         = :Value";

      CxHashtable provider = new CxHashtable();
      provider["ApplicationCd"] = m_LocalizationApplicationCode;
      provider["ObjectTypeCd"] = objectTypeCode;
      provider["PropertyCd"] = propertyCode;
      provider["ObjectName"] = objectName;
      provider["LanguageCd"] = languageCode;
      provider["DefaultValue"] = defaultValue;
      provider["Value"] = value;

      connection.ExecuteCommand(sql, provider);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Updates the localized value in memory so that the application can start working with the newly 
    /// modified localization string immediately.
    /// </summary>
    public void SetLocalizedValueInMemory(
      string languageCode,
      string objectTypeCode,
      string propertyCode,
      string objectName,
      string value)
    {
      string key = languageCode.ToUpper() + "." +
                   objectTypeCode.ToUpper() + "." +
                   propertyCode.ToUpper() + "." +
                   objectName.ToUpper();
      LocalizedValues[key] = value;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Updates localized value in the database.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="objectTypeCode">object type code</param>
    /// <param name="propertyCode">property code</param>
    /// <param name="objectName">object name</param>
    /// <param name="value">value to set</param>
    public void SetLocalizedValue(
      CxDbConnection connection,
      string objectTypeCode,
      string propertyCode,
      string objectName,
      string defaultValue,
      string value)
    {
      SetLocalizedValue(connection, LanguageCode, objectTypeCode, propertyCode, objectName, defaultValue, value);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if multilanguage data is loaded from the database.
    /// </summary>
    public bool IsLoaded
    {
      get { return m_IsLoaded; }
      protected set { m_IsLoaded = value; }
    }

    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets code of the current language to return values.
    /// </summary>
    public string LanguageCode
    { get { return m_LanguageCode; } set { m_LanguageCode = value; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns code of the localization application.
    /// </summary>
    public string LocalizationApplicationCode
    { get { return CxUtils.Nvl(m_LocalizationApplicationCode, m_ApplicationCode); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets localized values.
    /// </summary>
    public Hashtable LocalizedValues
    {
      get { return m_LocalizedValues; }
      set { m_LocalizedValues = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets localization dictionary values.
    /// </summary>
    public Hashtable DictionaryValues
    {
      get { return m_DictionaryValues; }
      set { m_DictionaryValues = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets object type and property dictionary.
    /// </summary>
    public Hashtable ObjectProperties
    {
      get { return m_ObjectProperties; }
      set { m_ObjectProperties = value; }
    }
    //-------------------------------------------------------------------------
  }
}