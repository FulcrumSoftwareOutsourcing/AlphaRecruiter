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
using System.Xml;

using Framework.Utils;

namespace Framework.Db
{
  //---------------------------------------------------------------------------
  /// <summary>
  /// Result of section save method.
  /// </summary>
  public enum NxSaveSectionResult 
  { 
    SectionNotFound, // Section is not found
    SectionSaved, // Section data saved
    AllDataSaved, // Section could not be saved separately, all settings are saved
    SectionNotSaved, // Section is not saved because it is not modified
    SectionDeleted // Section was marked to delete and is successfully deleted
  }
  //---------------------------------------------------------------------------

  //---------------------------------------------------------------------------
  /// <summary>
  /// Database user settings storage.
  /// </summary>
  public class CxDbSettingsStorage : CxSettingsStorage
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Class to hold user setting element.
    /// </summary>
    protected class CxSetting
    {
      //-----------------------------------------------------------------------
      protected DataRow m_Row = null;
      protected CxSetting m_Parent = null;
      protected Dictionary<string, CxSetting> m_ParentCollection = null;
      protected Dictionary<string, CxSetting> m_Children = new Dictionary<string, CxSetting>();
      protected bool m_IsNew = false;
      protected bool m_IsModified = false;
      protected bool m_IsDeleted = false;
      protected int m_DeletedId = 0;
      protected string m_DeletedKey = null;
      protected int m_DeletedParentId = 0;
      protected List<CxSetting> m_DeletedChildren = new List<CxSetting>();
      //-----------------------------------------------------------------------
      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="parent">parent setting object</param>
      /// <param name="parentCollection">collection this setting is adding</param>
      /// <param name="row">data row</param>
      public CxSetting(
        CxSetting parent, Dictionary<string, CxSetting> parentCollection, DataRow row)
      {
        m_Parent = parent;
        m_ParentCollection = parent != null ? parent.Children : parentCollection;
        m_Row = row;
      }
      //-----------------------------------------------------------------------
      /// <summary>
      /// Creates inserted XML element in the given document.
      /// </summary>
      /// <param name="doc">document to create element in</param>
      public void CreateInsertedXmlElement(XmlDocument doc)
      {
        XmlElement element = doc.CreateElement("item");
        SaveToXmlElement(element);
        doc.DocumentElement.AppendChild(element);
      }
      //-----------------------------------------------------------------------
      /// <summary>
      /// Creates updated XML element in the given document.
      /// </summary>
      /// <param name="doc">document to create element in</param>
      public void CreateUpdatedXmlElement(XmlDocument doc)
      {
        XmlElement element = doc.CreateElement("item");
        SaveToXmlElement(element, new string[]{"UserSettingId", "Value"});
        doc.DocumentElement.AppendChild(element);
      }
      //-----------------------------------------------------------------------
      /// <summary>
      /// Creates updated XML element in the given document.
      /// </summary>
      /// <param name="doc">document to create element in</param>
      public void CreateDeletedXmlElement(XmlDocument doc)
      {
        XmlElement element = doc.CreateElement("item");
        SaveToXmlElement(element, new string[]{"UserSettingId"});
        doc.DocumentElement.AppendChild(element);
      }
      //-----------------------------------------------------------------------
      /// <summary>
      /// Saves properties to the XML element attributes.
      /// </summary>
      /// <param name="element">XML element to save properties to</param>
      /// <param name="fieldList">IDs of fields to include or null to include all</param>
      public void SaveToXmlElement(XmlElement element, string[] fieldList)
      {
        if (m_IsDeleted)
        {
          element.SetAttribute("UserSettingId", m_DeletedId.ToString());
          return;
        }
        if (fieldList != null && fieldList.Length > 0)
        {
          foreach (string fieldName in fieldList)
          {
            if (CxUtils.NotEmpty(m_Row[fieldName]))
            {
              element.SetAttribute(fieldName, CxUtils.ToString(m_Row[fieldName]));
            }
          }
        }
        else
        {
          foreach (DataColumn dc in m_Row.Table.Columns)
          {
            if (CxUtils.NotEmpty(m_Row[dc]))
            {
              element.SetAttribute(dc.ColumnName, CxUtils.ToString(m_Row[dc]));
            }
          }
        }
      }
      //-----------------------------------------------------------------------
      /// <summary>
      /// Saves properties to the XML element attributes.
      /// </summary>
      /// <param name="element">XML element to save properties to</param>
      public void SaveToXmlElement(XmlElement element)
      {
        SaveToXmlElement(element, null);
      }
      //-----------------------------------------------------------------------
      /// <summary>
      /// Marks object as deleted.
      /// </summary>
      public void Delete()
      {
        if (!m_IsDeleted)
        {
          m_DeletedId = Id;
          m_DeletedParentId = ParentId;
          m_DeletedKey = Key;
          m_IsDeleted = true;
        }
      }
      //-----------------------------------------------------------------------
      /// <summary>
      /// Updates ParentId field from the parent setting ID.
      /// </summary>
      public void UpdateParentReference()
      {
        if (IsNew && ParentId == 0 && Parent != null && Parent.Id > 0)
        {
          ParentId = Parent.Id;
        }
      }
      //-----------------------------------------------------------------------
      /// <summary>
      /// Clears modification flags.
      /// </summary>
      public void ClearModificationFlags()
      {
        m_IsNew = false;
        m_IsModified = false;
        if (m_IsDeleted && m_Parent != null)
        {
          m_Parent.DeletedChildren.Remove(this);
        }
      }
      //-----------------------------------------------------------------------
      /// <summary>
      /// Returns parent setting.
      /// </summary>
      public CxSetting Parent
      { get { return m_Parent; } }
      //-----------------------------------------------------------------------
      /// <summary>
      /// Returns parent collection this setting belongs to.
      /// </summary>
      public Dictionary<string, CxSetting> ParentCollection
      { get { return m_ParentCollection; } }
      //-----------------------------------------------------------------------
      /// <summary>
      /// User setting record.
      /// </summary>
      public DataRow Row
      { get { return m_Row; } }
      //-----------------------------------------------------------------------
      /// <summary>
      /// Table of child settings. Key is string, value is CxSetting.
      /// </summary>
      public Dictionary<string, CxSetting> Children
      { get { return m_Children; } }
      //-----------------------------------------------------------------------
      /// <summary>
      /// True if setting is created and not saved to database.
      /// </summary>
      public bool IsNew
      { get { return m_IsNew; } set { m_IsNew = value; } }
      //-----------------------------------------------------------------------
      /// <summary>
      /// True if setting is updated and not saved to database.
      /// </summary>
      public bool IsModified
      { get { return m_IsModified; } set { m_IsModified = value; } }
      //-----------------------------------------------------------------------
      /// <summary>
      /// True if setting is deleted and not saved to database.
      /// </summary>
      public bool IsDeleted
      { get { return m_IsDeleted; } }
      //-----------------------------------------------------------------------
      /// <summary>
      /// Gets or sets primary key ID.
      /// </summary>
      public int Id
      {
        get
        {
          if (m_IsDeleted)
          {
            return m_DeletedId;
          }
          return m_Row != null ? CxInt.Parse(m_Row["UserSettingId"], 0) : 0;
        }
        set
        {
          if (m_IsDeleted)
          {
            m_DeletedId = value;
          }
          else if (m_Row != null)
          {
            m_Row["UserSettingId"] = value;
          }
        }
      }
      //-----------------------------------------------------------------------
      /// <summary>
      /// Gets or sets parent ID.
      /// </summary>
      public int ParentId
      { 
        get 
        {
          if (m_IsDeleted)
          {
            return m_DeletedParentId;
          }
          return m_Row != null ? CxInt.Parse(m_Row["ParentId"], 0) : 0; 
        }
        set
        {
          if (m_IsDeleted)
          {
            m_DeletedParentId = value;
          }
          else if (m_Row != null)
          {
            m_Row["ParentId"] = value;
          }
        }
      }
      //-----------------------------------------------------------------------
      /// <summary>
      /// Returns unique setting key.
      /// </summary>
      public string Key
      {
        get
        {
          return m_IsDeleted ? m_DeletedKey : GetKey(m_Row);
        }
      }
      //-----------------------------------------------------------------------
      /// <summary>
      /// Returns list of deleted children.
      /// </summary>
      public List<CxSetting> DeletedChildren
      { get { return m_DeletedChildren; } }
      //-----------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    public const string DEFAULT_WIN_OPTION_TYPE    = "WindowsOption";
    public const string DEFAULT_WEB_OPTION_TYPE    = "WebOption";
    public const string DEFAULT_SL_OPTION_TYPE = "SLOption";
    public const string DEFAULT_COMMON_OPTION_TYPE = "CommonOption";
    //-------------------------------------------------------------------------
    protected int m_UserId = 0;
    protected string m_ApplicationCd = null;
    protected string m_DefaultOptionType = null;
    protected DataTable m_Data = new DataTable();
    protected Dictionary<string, CxSetting> m_Tree = new Dictionary<string, CxSetting>();
    protected List<CxSetting> m_DeletedChildren = new List<CxSetting>();
    protected DxCreateDbConnection m_OnCreateDbConnection = null;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="applicationCd">application code</param>
    /// <param name="userId">layout user ID</param>
    public CxDbSettingsStorage(
      string applicationCd, 
      int userId,
      string defaultOptionType)
    {
      m_ApplicationCd = applicationCd;
      m_UserId = userId;
      m_DefaultOptionType = defaultOptionType;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates DB storage for the current user.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="applicationCd">application code</param>
    /// <param name="userId">user ID</param>
    /// <returns>created storage object</returns>
    static public CxDbSettingsStorage CreateCurrentUser(
      CxDbConnection connection,
      string applicationCd,
      int userId,
      string defaultOptionType)
    {
      CxDbSettingsStorage storage = new CxDbSettingsStorage(applicationCd, userId, defaultOptionType);
      storage.Load(connection);
      return storage;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates DB storage for all users.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="applicationCd">application code</param>
    /// <returns>created storage object</returns>
    static public CxDbSettingsStorage CreateAllUsers(
      CxDbConnection connection,
      string applicationCd,
      string defaultOptionType)
    {
      CxDbSettingsStorage storage = new CxDbSettingsStorage(applicationCd, 0, defaultOptionType);
      storage.Load(connection);
      return storage;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Reads setting from storage.
    /// </summary>
    /// <param name="sectionName">name of the section</param>
    /// <param name="keyName">name of the key</param>
    /// <param name="defValue">default value to return if not found</param>
    /// <returns>found setting of default value</returns>
    override public string Read(string sectionName, string keyName, string defValue)
    {
      return Read(m_DefaultOptionType, sectionName, keyName, defValue);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Writes setting to storage.
    /// </summary>
    /// <param name="sectionName">name of the section</param>
    /// <param name="keyName">name of the key</param>
    /// <param name="value">value to write</param>
    override public void Write(string sectionName, string keyName, string value)
    {
      Write(m_DefaultOptionType, sectionName, keyName, value);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of setting names from the section.
    /// </summary>
    /// <param name="sectionName">section name</param>
    /// <returns>list of setting names from the section</returns>
    override public IList<string> GetNames(string sectionName)
    {
      return GetNames(m_DefaultOptionType, sectionName);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of folder names from the section.
    /// </summary>
    /// <param name="sectionName">section name</param>
    /// <returns>list of folder names from the section</returns>
    override public IList<string> GetFolderNames(string sectionName)
    {
      return GetFolderNames(m_DefaultOptionType, sectionName);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Deletes section of settings.
    /// </summary>
    /// <param name="sectionName">name of section to delete</param>
    override public void Delete(string sectionName)
    {
      Delete(m_DefaultOptionType, sectionName);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Writes value to each user data present in the storage.
    /// </summary>
    /// <param name="sectionName">section name</param>
    /// <param name="keyName">key name</param>
    /// <param name="value">value to write</param>
    override public void WriteForEachUser(string sectionName, string keyName, string value)
    {
      if (m_OnCreateDbConnection == null)
      {
        throw new ExException("CxDbSettingsStorage.OnCreateDbConnection delegate is not assigned. " +
                              "WriteForEachUser method could not be executed.");
      }
      using (CxDbConnection connection = m_OnCreateDbConnection())
      {
        WriteForEachUser(connection, m_DefaultOptionType, sectionName, keyName, value);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Deletes section for each user whose data present in the storage.
    /// </summary>
    /// <param name="sectionName">section name</param>
    override public void DeleteForEachUser(string sectionName)
    {
      if (m_OnCreateDbConnection == null)
      {
        throw new ExException("CxDbSettingsStorage.OnCreateDbConnection delegate is not assigned. " +
                              "DeleteForEachUser method could not be executed.");
      }
      using (CxDbConnection connection = m_OnCreateDbConnection())
      {
        DeleteForEachUser(connection, m_DefaultOptionType, sectionName);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads settings from the database.
    /// </summary>
    /// <param name="connection">database connection</param>
    public void Load(CxDbConnection connection)
    {
      m_Data.Clear();
      m_Tree.Clear();
      connection.GetQueryResult(
        m_Data,
        @"select t.*
            from Framework_UserSettings t
           where t.ApplicationCd = :ApplicationCd
             and t.UserId = :UserId",
        m_ApplicationCd,
        m_UserId);
      LoadTreeLevel(m_Data, null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads settings from the database.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="userId">user ID to load settings from</param>
    public void Load(CxDbConnection connection, int userId)
    {
      m_UserId = userId;
      Load(connection);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads settings from the given data table.
    /// </summary>
    /// <param name="dt">data table to load data from</param>
    public void Load(DataTable dt)
    {
      m_Data.Clear();
      m_Tree.Clear();
      CxData.CopyDataTable(dt, m_Data);
      LoadTreeLevel(m_Data, null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Saves modified settings to the database.
    /// </summary>
    /// <param name="connection">database connection</param>
    public void Save(CxDbConnection connection)
    {
      SaveSection(connection, null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Saves section to the database and reloads section data after save.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="sectionName">section to save and reload</param>
    /// <returns>true if section is found after reload</returns>
    public bool SaveAndReloadSection(CxDbConnection connection, string sectionName)
    {
      NxSaveSectionResult saveResult = SaveSection(connection, sectionName);
      switch (saveResult)
      {
        case NxSaveSectionResult.SectionSaved:
          LoadSection(connection, sectionName);
          break;
        case NxSaveSectionResult.AllDataSaved:
          Load(connection);
          break;
      }
      return GetSettingByPath(m_DefaultOptionType, ParseSectionPath(sectionName)) != null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Reloads settings section from the database.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="sectionName">section name to reload</param>
    public void LoadSection(CxDbConnection connection, string sectionName)
    {
      if (CxUtils.IsEmpty(sectionName))
      {
        // Loads the whole tree
        Load(connection);
        return;
      }

      IList<string> path = ParseSectionPath(sectionName);
      CxSetting setting = GetSettingByPath(m_DefaultOptionType, path);
      if (setting == null)
      {
        return;
      }

      CxDbParameter pId = connection.CreateParameter("UserSettingId", setting.Id);
      DataTable dt = connection.GetQueryResultSP("p_Framework_UserSettings_GetSubTree", pId);

      if (dt == null || dt.Rows.Count == 0)
      {
        // Setting is not found by ID, try to find by unique key
        dt = new DataTable();
        connection.GetQueryResult(
          dt,
          @"select t.*
              from Framework_UserSettings t
             where t.OptionKey = :OptionKey
               and t.OptionType = :OptionType
               and t.ParentId = :ParentId
               and t.UserId = :UserId
               and t.ApplicationCd = :ApplicationCd",
          new CxDataRowValueProvider(setting.Row));

        if (dt.Rows.Count == 1)
        {
          pId = connection.CreateParameter("UserSettingId", dt.Rows[0]["UserSettingId"]);
          dt = connection.GetQueryResultSP("p_Framework_UserSettings_GetSubTree", pId);
        }
        else
        {
          dt = null;
        }
      }

      RemoveSetting(setting, false);

      if (dt == null || dt.Rows.Count == 0)
      {
        return;
      }

      foreach (DataRow dr in dt.Rows)
      {
        m_Data.ImportRow(dr);
      }

      DataRow[] rows = m_Data.Select("UserSettingId = " + setting.Id);
      if (rows == null || rows.Length == 0)
      {
        return;
      }

      Dictionary<string, CxSetting> level = setting.Parent != null ? setting.Parent.Children : m_Tree;
      CxSetting newSetting = new CxSetting(setting.Parent, level, rows[0]);
      level[newSetting.Key] = newSetting;

      LoadTreeLevel(m_Data, newSetting);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Saves modified settings of the given section to the database.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="sectionName">section name</param>
    public NxSaveSectionResult SaveSection(CxDbConnection connection, string sectionName)
    {
      Dictionary<string, CxSetting> level = m_Tree;
      IList<CxSetting> deletedList = m_DeletedChildren;

      CxSetting setting = null;
      if (CxUtils.NotEmpty(sectionName))
      {
        IList<string> path = ParseSectionPath(sectionName);
        setting = 
          GetSettingByPath(m_DefaultOptionType, path) ?? GetDeletedSettingByPath(m_DefaultOptionType, path);
        if (setting == null)
        {
          // Section is not found, nothing to save
          return NxSaveSectionResult.SectionNotFound;
        }
        if (setting.Parent != null && setting.Parent.IsNew)
        {
          // Parent setting is not added to database, the whole tree should be saved.
          Save(connection);
          return NxSaveSectionResult.AllDataSaved;
        }
        level = setting.Children;
        deletedList = setting.DeletedChildren;
      }

      CreateIDsForNewRecords(connection);

      if (setting != null)
      {
        setting.UpdateParentReference();
      }
      UpdateTreeLevelParentReferences(level);

      XmlDocument docInserted = CxXml.CreateDocument("items");
      XmlDocument docUpdated = CxXml.CreateDocument("items");
      XmlDocument docDeleted = CxXml.CreateDocument("items");

      int modifiedCount = 0;
      int deletedCount = 0;
      bool isDeletedSection = false;

      if (setting != null)
      {
        if (setting.IsDeleted)
        {
          isDeletedSection = true;
          deletedCount += CreateDeletedXmlElements(docDeleted, level, deletedList);
          setting.CreateDeletedXmlElement(docDeleted);
          deletedCount++;
        }
        else if (setting.IsNew)
        {
          setting.CreateInsertedXmlElement(docInserted);
          modifiedCount++;
        }
        else if (setting.IsModified)
        {
          setting.CreateUpdatedXmlElement(docUpdated);
          modifiedCount++;
        }
      }

      if (!isDeletedSection)
      {
        modifiedCount += CreateModifiedXmlElements(docInserted, docUpdated, level);
        deletedCount += CreateDeletedXmlElements(docDeleted, level, deletedList);
      }

      string strInserted = !IsEmptyDoc(docInserted) ? CxXml.DocToString(docInserted) : null;
      string strUpdated = !IsEmptyDoc(docUpdated) ? CxXml.DocToString(docUpdated) : null;
      string strDeleted = !IsEmptyDoc(docDeleted) ? CxXml.DocToString(docDeleted) : null;

      if (modifiedCount > 0 || deletedCount > 0)
      {
        bool ownsTransaction = !connection.InTransaction;
        if (ownsTransaction)
        {
          connection.BeginTransaction();
        }
        try
        {
          CxDbParameter pInserted = connection.CreateParameter("xmlInserted", strInserted);
          CxDbParameter pUpdated = connection.CreateParameter("xmlUpdated", strUpdated);
          CxDbParameter pDeleted = connection.CreateParameter("xmlDeleted", strDeleted);

          connection.ExecuteCommandSP("p_Framework_UserSettings_Update", pInserted, pUpdated, pDeleted);

          if (ownsTransaction)
          {
            connection.Commit();
          }

          if (setting != null)
          {
            setting.ClearModificationFlags();
            if (setting.IsDeleted && setting.Parent == null)
            {
              m_DeletedChildren.Remove(setting);
            }
          }
          ClearModificationFlags(level);

          return !isDeletedSection ? NxSaveSectionResult.SectionSaved : NxSaveSectionResult.SectionDeleted;
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
      return NxSaveSectionResult.SectionNotSaved;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns unique key by option type and option key.
    /// </summary>
    static protected string GetKey(string optionType, string optionKey)
    {
      return optionKey.ToUpper() + ":" + optionType.ToUpper();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns unique key by data row.
    /// </summary>
    static protected string GetKey(DataRow row)
    {
      return row != null ? GetKey(row["OptionType"].ToString(), row["OptionKey"].ToString()) : null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads tree level of user settings.
    /// </summary>
    /// <param name="dt">data table</param>
    /// <param name="parent">parent setting object or null</param>
    protected void LoadTreeLevel(
      DataTable dt,
      CxSetting parent)
    {
      Dictionary<string, CxSetting> level = parent != null ? parent.Children : m_Tree;
      int parentId = parent != null ? parent.Id : 0;
      string condition = parentId == 0 ? "ParentId IS NULL" : "ParentId = " + parentId;
      DataRow[] rows = dt.Select(condition);
      foreach (DataRow dr in rows)
      {
        CxSetting setting = new CxSetting(parent, level, dr);
        level[setting.Key] = setting;
        LoadTreeLevel(dt, setting);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets IDs for new created records.
    /// </summary>
    protected void CreateIDsForNewRecords(CxDbConnection connection)
    {
      int requiredIdCount = 0;
      foreach (DataRow dr in m_Data.Rows)
      {
        if (CxUtils.IsEmpty(dr["UserSettingId"]))
        {
          requiredIdCount++;
        }
      }
      if (requiredIdCount > 0)
      {
        int minId;
        int maxId;
        connection.GetNextIdRange(requiredIdCount, out minId, out maxId);
        foreach (DataRow dr in m_Data.Rows)
        {
          if (CxUtils.IsEmpty(dr["UserSettingId"]))
          {
            dr["UserSettingId"] = minId++;
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Updates parent references for new created records before save to database.
    /// </summary>
    /// <param name="level">current level hashtable</param>
    protected void UpdateTreeLevelParentReferences(Dictionary<string, CxSetting> level)
    {
      foreach (CxSetting setting in level.Values)
      {
        setting.UpdateParentReference();
        UpdateTreeLevelParentReferences(setting.Children);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates XML elements corresponding to the inserted or updated settings.
    /// </summary>
    /// <param name="docInserted">document to create inserted elements</param>
    /// <param name="docUpdated">document to create updated elements</param>
    /// <param name="level">current settings collection</param>
    protected int CreateModifiedXmlElements(
      XmlDocument docInserted,
      XmlDocument docUpdated,
      Dictionary<string, CxSetting> level)
    {
      int count = 0;
      foreach (CxSetting setting in level.Values)
      {
        if (setting.IsNew)
        {
          setting.CreateInsertedXmlElement(docInserted);
          count++;
        }
        else if (setting.IsModified)
        {
          setting.CreateUpdatedXmlElement(docUpdated);
          count++;
        }
        count += CreateModifiedXmlElements(docInserted, docUpdated, setting.Children);
      }
      return count;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates XML elements corresponding to the deleted settings.
    /// </summary>
    /// <param name="docDeleted">document to create deleted elements</param>
    /// <param name="level">level to search for deleted elements</param>
    /// <param name="deletedList">list of deleted settings</param>
    protected int CreateDeletedXmlElements(
      XmlDocument docDeleted,
      Dictionary<string, CxSetting> level,
      IList<CxSetting> deletedList)
    {
      int count = 0;
      foreach (CxSetting setting in level.Values)
      {
        count += CreateDeletedXmlElements(docDeleted, setting.Children, setting.DeletedChildren);
      }
      foreach (CxSetting setting in deletedList)
      {
        count += CreateDeletedXmlElements(docDeleted, setting.Children, setting.DeletedChildren);
        setting.CreateDeletedXmlElement(docDeleted);
        count++;
      }
      return count;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Clears modification flags for all cached settings objects.
    /// </summary>
    /// <param name="level">current collection of settings</param>
    protected void ClearModificationFlags(Dictionary<string, CxSetting> level)
    {
      if (level == m_Tree)
      {
        m_DeletedChildren.Clear();
      }
      foreach (CxSetting setting in level.Values)
      {
        setting.ClearModificationFlags();
        ClearModificationFlags(setting.Children);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if root element of the document does not have child nodes.
    /// </summary>
    protected bool IsEmptyDoc(XmlDocument doc)
    {
      return doc == null || doc.DocumentElement == null || doc.DocumentElement.ChildNodes.Count == 0;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns CxSetting object by the given path.
    /// </summary>
    /// <param name="optionType">option type</param>
    /// <param name="path">string list of option keys</param>
    /// <returns>CxSetting object or null if not found</returns>
    protected CxSetting GetSettingByPath(string optionType, IList<string> path)
    {
      CxSetting setting = null;
      Dictionary<string, CxSetting> level = m_Tree;
      foreach (string name in path)
      {
        string key = GetKey(optionType, name);
        if (!level.ContainsKey(key))
          return null;
        
        setting = level[key];
        level = setting.Children;
      }
      return setting;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns deleted CxSetting object by the given path.
    /// </summary>
    /// <param name="optionType">option type</param>
    /// <param name="path">string list of option keys</param>
    /// <returns>CxSetting object or null if not found</returns>
    protected CxSetting GetDeletedSettingByPath(string optionType, IList<string> path)
    {
      CxSetting setting = null;
      CxSetting parent = null;
      Dictionary<string, CxSetting> level = m_Tree;
      foreach (string name in path)
      {
        string key = GetKey(optionType, name);
        if (level.ContainsKey(key))
          setting = level[key];
        else
        {
          IList<CxSetting> deletedList = parent != null ? parent.DeletedChildren : m_DeletedChildren;
          foreach (CxSetting deletedSetting in deletedList)
          {
            if (deletedSetting.Key == key)
            {
              setting = deletedSetting;
              break;
            }
          }
        }
        if (setting == null)
        {
          return null;
        }
        parent = setting;
        level = setting.Children;
      }
      return setting;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns data row by the given path.
    /// </summary>
    /// <param name="optionType">option type</param>
    /// <param name="path">string list of option keys</param>
    /// <returns>data row or null if not found</returns>
    protected DataRow GetRowByPath(string optionType, IList<string> path)
    {
      CxSetting setting = GetSettingByPath(optionType, path);
      return setting != null ? setting.Row : null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates all path if not exists and returns leaf CxSetting object.
    /// </summary>
    /// <param name="optionType">option type</param>
    /// <param name="path">string list of option keys</param>
    /// <returns>leaf CxSetting object</returns>
    protected CxSetting CreatePath(string optionType, IList<string> path)
    {
      CxSetting setting = null;
      CxSetting parent = null;
      Dictionary<string, CxSetting> level = m_Tree;
      foreach (string name in path)
      {
        string key = GetKey(optionType, name);
        if (level.ContainsKey(key))
        {
          setting = level[key];
        }
        else
        {
          DataRow row = m_Data.NewRow();
          row["OptionType"] = optionType;
          row["OptionKey"] = name;
          row["UserId"] = m_UserId;
          row["ApplicationCd"] = m_ApplicationCd;
          m_Data.Rows.Add(row);
          setting = new CxSetting(parent, level, row);
          setting.IsNew = true;
          level[key] = setting;
        }
        parent = setting;
        level = setting.Children;
      }
      return setting;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Removes setting object from all collections.
    /// </summary>
    /// <param name="setting">setting to remove</param>
    protected void RemoveSetting(CxSetting setting, bool addToDeletedList)
    {
      if (setting != null)
      {
        setting.Delete();
        if (!setting.IsNew && addToDeletedList)
        {
          if (setting.Parent != null)
          {
            setting.Parent.DeletedChildren.Add(setting);
          }
          else
          {
            m_DeletedChildren.Add(setting);
          }
        }
        setting.ParentCollection.Remove(setting.Key);
        m_Data.Rows.Remove(setting.Row);
        CxSetting[] childArray = new CxSetting[setting.Children.Values.Count];
        setting.Children.Values.CopyTo(childArray, 0);
        foreach (CxSetting childSetting in childArray)
        {
          RemoveSetting(childSetting, addToDeletedList);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Reads setting from storage.
    /// </summary>
    /// <param name="optionType">option type</param>
    /// <param name="sectionName">name of the section</param>
    /// <param name="keyName">name of the key</param>
    /// <param name="defValue">default value to return if not found</param>
    /// <returns>found setting of default value</returns>
    public string Read(string optionType, string sectionName, string keyName, string defValue)
    {
      IList<string> path = ParseSectionPath(sectionName);
      path.Add(keyName);
      DataRow row = GetRowByPath(optionType, path);
      return row != null ? row["Value"].ToString() : defValue;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Writes setting to storage.
    /// </summary>
    /// <param name="optionType">option type</param>
    /// <param name="sectionName">name of the section</param>
    /// <param name="keyName">name of the key</param>
    /// <param name="value">value to write</param>
    public void Write(string optionType, string sectionName, string keyName, string value)
    {
      IList<string> path = ParseSectionPath(sectionName);
      path.Add(keyName);
      CxSetting setting = CreatePath(optionType, path);
      if (setting.Row["Value"].ToString() != value)
      {
        setting.Row["Value"] = value;
        setting.IsModified = true;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of all settings from the section.
    /// </summary>
    /// <param name="optionType">option type</param>
    /// <param name="sectionName">section name</param>
    /// <returns>list of CxSettings objects or null</returns>
    protected IList<CxSetting> GetSettings(string optionType, string sectionName)
    {
      IList<string> path = ParseSectionPath(sectionName);
      CxSetting setting = GetSettingByPath(optionType, path);
      if (setting != null && setting.Children.Count > 0)
      {
        return new List<CxSetting>(setting.Children.Values);
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of setting names from the section.
    /// </summary>
    /// <param name="optionType">option type</param>
    /// <param name="sectionName">section name</param>
    /// <returns>list of setting names from the section</returns>
    public IList<string> GetNames(string optionType, string sectionName)
    {
      IList<CxSetting> settings = GetSettings(optionType, sectionName);
      if (settings != null)
      {
        List<string> list = new List<string>();
        foreach (CxSetting setting in settings)
        {
          if (setting.Children.Count == 0)
          {
            list.Add(setting.Row["OptionKey"].ToString());
          }
        }
        return list.Count > 0 ? list : null;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of setting folder names from the section.
    /// </summary>
    /// <param name="optionType">option type</param>
    /// <param name="sectionName">section name</param>
    /// <returns>list of setting folder names from the section</returns>
    public IList<string> GetFolderNames(string optionType, string sectionName)
    {
      IList<CxSetting> settings = GetSettings(optionType, sectionName);
      if (settings != null)
      {
        List<string> list = new List<string>();
        foreach (CxSetting setting in settings)
        {
          if (setting.Children.Count > 0)
          {
            list.Add(setting.Row["OptionKey"].ToString());
          }
        }
        return list.Count > 0 ? list : null;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Deletes section of settings.
    /// </summary>
    /// <param name="sectionName">name of section to delete</param>
    public void Delete(string optionType, string sectionName)
    {
      IList<string> path = ParseSectionPath(sectionName);
      CxSetting setting = GetSettingByPath(optionType, path);
      if (setting != null)
      {
        RemoveSetting(setting, true);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Writes value for each user whose data are contained in the database.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="optionType">option type</param>
    /// <param name="sectionName">name of section to write value to</param>
    /// <param name="keyName">key name of the value</param>
    /// <param name="value">value to write</param>
    protected void WriteForEachUser(
      CxDbConnection connection,
      string optionType,
      string sectionName,
      string keyName,
      string value)
    {
      bool ownsTransaction = !connection.InTransaction;
      if (ownsTransaction)
      {
        connection.BeginTransaction();
      }
      try
      {
        CxDbParameter pSectionName = connection.CreateParameter("SectionName", sectionName);
        CxDbParameter pKeyName = connection.CreateParameter("KeyName", keyName);
        CxDbParameter pValue = connection.CreateParameter("Value", value);
        CxDbParameter pApplicationCd = connection.CreateParameter("ApplicationCd", m_ApplicationCd);
        CxDbParameter pOptionType = connection.CreateParameter("OptionType", optionType);
        connection.ExecuteCommandSP(
          "p_Framework_UserSettings_WriteForEachUser",
          pSectionName, 
          pKeyName,
          pValue,
          pApplicationCd,
          pOptionType);

        if (ownsTransaction)
        {
          connection.Commit();
        }
      }
      catch (Exception e)
      {
        if (ownsTransaction)
        {
          connection.Rollback();
        }
        throw new ExException(e.Message, e);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Deletes section for each user whose data are contained in the database.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="optionType">option type</param>
    /// <param name="sectionName">name of section to write value to</param>
    protected void DeleteForEachUser(
      CxDbConnection connection,
      string optionType,
      string sectionName)
    {
      bool ownsTransaction = !connection.InTransaction;
      if (ownsTransaction)
      {
        connection.BeginTransaction();
      }
      try
      {
        CxDbParameter pSectionName = connection.CreateParameter("SectionName", sectionName, null);
        CxDbParameter pApplicationCd = connection.CreateParameter("ApplicationCd", m_ApplicationCd, null);
        CxDbParameter pOptionType = connection.CreateParameter("OptionType", optionType, null);
        connection.ExecuteCommandSP(
          "p_Framework_UserSettings_DeleteForEachUser",
          pSectionName,
          pApplicationCd,
          pOptionType);

        if (ownsTransaction)
        {
          connection.Commit();
        }
      }
      catch (Exception e)
      {
        if (ownsTransaction)
        {
          connection.Rollback();
        }
        throw new ExException(e.Message, e);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns settings user ID.
    /// </summary>
    public int UserId
    {
      get { return m_UserId; }
      set { m_UserId = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Event handler to create database connection.
    /// Used for WriteForEachUser and DeleteForEachUser methods.
    /// </summary>
    public event DxCreateDbConnection OnCreateDbConnection
    {
      add
      {
        m_OnCreateDbConnection = null;
        m_OnCreateDbConnection += value;
      }
      remove
      {
        m_OnCreateDbConnection -= value;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns datatable with the loaded settings data.
    /// </summary>
    public DataTable Data
    { get { return m_Data; } }
    //----------------------------------------------------------------------------
  }
}