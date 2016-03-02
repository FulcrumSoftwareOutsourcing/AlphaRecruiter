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
using System.Configuration;
using System.Data;
using System.Text;
using System.Xml;
using Framework.Db;
using Framework.Metadata;
using Framework.Utils;

namespace Framework.Entity
{
  /// <summary>
  /// Class for recent, open items and bookmarks management.
  /// </summary>
  public class CxEntityMarks
  {
    //-------------------------------------------------------------------------
    protected int m_UserId = 0;
    protected Dictionary<NxEntityMarkType, List<CxEntityMark>>
      m_MarksByType = new Dictionary<NxEntityMarkType, List<CxEntityMark>>();
    protected List<CxEntityMark> m_DeletedMarks = new List<CxEntityMark>();
    protected int m_EventDisableCount = 0;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public CxEntityMarks(int userId)
    {
      m_UserId = userId;
      foreach (NxEntityMarkType markType in Enum.GetValues(typeof(NxEntityMarkType)))
      {
        m_MarksByType[markType] = new List<CxEntityMark>();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates instance and loads marked entities from database.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="userId">user ID</param>
    /// <param name="holder">metadata holder</param>
    /// <returns>created instance</returns>
    static public CxEntityMarks Create(
      CxDbConnection connection, 
      int userId,
      CxMetadataHolder holder)
    {
      CxEntityMarks instance = new CxEntityMarks(userId);
      instance.Load(connection, holder);
      return instance;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads marked entities list from the database.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="holder">metadata holder</param>
    protected void Load(CxDbConnection connection, CxMetadataHolder holder)
    {
      foreach (KeyValuePair<NxEntityMarkType, List<CxEntityMark>> pair in m_MarksByType)
      {
        if (IsStorable(pair.Key))
        {
          pair.Value.Clear();
        }
      }
      m_DeletedMarks.Clear();

      DataTable dt = new DataTable();
      connection.GetQueryResult(
        dt, 
        @"select t.* 
            from Framework_MarkedItems t
           where t.UserId = :UserId
           order by t.DisplayOrder, t.Name", 
        m_UserId);

      foreach (DataRow dr in dt.Rows)
      {
        CxEntityMark mark = null;
        try
        {
          mark = new CxEntityMark(
            holder,
            dr["EntityUsageId"].ToString(),
            dr["PrimaryKeyText"].ToString(),
            GetMarkTypeByCode(dr["ItemTypeCd"].ToString()),
            dr["Name"].ToString());
          mark.DisplayOrder = CxInt.Parse(dr["DisplayOrder"], 0);
          object openMode = dr["OpenMode"];
          mark.OpenMode = (openMode == null || openMode is DBNull || Convert.ToString(openMode) == string.Empty) ? "Edit" : openMode.ToString();
          mark.ApplicationCd = dr["ApplicationCd"].ToString();
        }
        catch (Exception e)
        {
          connection.WriteToLog("Entity Mark Load Error:\r\n" + e.Message + "\r\n" + e.StackTrace);
        }
        if (mark != null)
        {
          m_MarksByType[mark.MarkType].Add(mark);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Saves unsaved changed and reloads entity marks from the database.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="holder">metadata holder</param>
    public void SaveAndReload(CxDbConnection connection, CxMetadataHolder holder)
    {
      Save(connection);
      Load(connection, holder);
      DoOnChanged();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Reloads entity marks from the database.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="holder">metadata holder</param>
    /// <param name="userId">user ID</param>
    public void Reload(CxDbConnection connection, CxMetadataHolder holder, int userId)
    {
      m_UserId = userId;
      Load(connection, holder);
      DoOnChanged();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Reloads entity marks from the database.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="holder">metadata holder</param>
    public void Reload(CxDbConnection connection, CxMetadataHolder holder)
    {
      Reload(connection, holder, m_UserId);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if collection is modified.
    /// </summary>
    public bool IsModified
    {
      get
      {
        if (m_DeletedMarks.Count > 0)
        {
          return true;
        }
        foreach (KeyValuePair<NxEntityMarkType, List<CxEntityMark>> pair in m_MarksByType)
        {
          if (IsStorable(pair.Key))
          {
            foreach (CxEntityMark mark in pair.Value)
            {
              if (mark.Inserted || mark.Updated)
              {
                return true;
              }
            }
          }
        }
        return false;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Saves all unsaved marks to the database.
    /// </summary>
    /// <param name="connection">database connection</param>
    public void Save(CxDbConnection connection)
    {
      if (!IsModified)
      {
        return;
      }
      connection.BeginTransaction();
      try
      {
        XmlDocument doc = CxXml.CreateDocument("items");
        foreach (KeyValuePair<NxEntityMarkType, List<CxEntityMark>> pair in m_MarksByType)
        {
          if (IsStorable(pair.Key))
          {
            AppendXmlItems(doc, pair.Value);
          }
        }
        CxDbParameter pUserId = connection.CreateParameter("UserId", m_UserId);
        CxDbParameter pItems = connection.CreateParameter("xmlItems", CxXml.DocToString(doc));
        connection.ExecuteCommandSP("p_Framework_MarkedItems_Update", pUserId, pItems);
        connection.Commit();
        ClearModificationFlags();
      }
      catch
      {
        connection.Rollback();
        throw;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Appends XML document with the non-deleted items.
    /// </summary>
    /// <param name="doc">document to append</param>
    /// <param name="marks">list of marks</param>
    protected void AppendXmlItems(XmlDocument doc, List<CxEntityMark> marks)
    {
      foreach (CxEntityMark mark in marks)
      {
        if (!mark.Deleted)
        {
          XmlElement item = doc.CreateElement("item");
          item.SetAttribute("EntityUsageId", mark.EntityUsage.Id);
          item.SetAttribute("PrimaryKeyText", mark.PrimaryKeyText);
          item.SetAttribute("ItemTypeCd", GetMarkTypeCode(mark.MarkType));
          item.SetAttribute("UserId", m_UserId.ToString());
          item.SetAttribute("Name", mark.Name);
          item.SetAttribute("DisplayOrder", mark.DisplayOrder.ToString());
          item.SetAttribute("OpenMode", mark.OpenMode);
          item.SetAttribute("ApplicationCd", mark.ApplicationCd);
          doc.DocumentElement.AppendChild(item);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Clears all modification flags.
    /// </summary>
    protected void ClearModificationFlags()
    {
      m_DeletedMarks.Clear();
      foreach (List<CxEntityMark> marks in m_MarksByType.Values)
      {
        foreach (CxEntityMark mark in marks)
        {
          mark.ClearModificationFlags();
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if mark type is storable.
    /// </summary>
    /// <param name="markType">mark type</param>
    protected bool IsStorable(NxEntityMarkType markType)
    {
      return markType != NxEntityMarkType.Open;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns record limit for the mark type.
    /// </summary>
    /// <param name="markType">mark type</param>
    protected int GetRecordLimit(NxEntityMarkType markType)
    {
      switch (markType)
      {
        case NxEntityMarkType.Bookmark: 
          return 100;
        case NxEntityMarkType.Recent:
          return CxOptions.Instance != null ? CxOptions.Instance.AmountOfRecentItemsVisible : 10;
      }
      return -1;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity mark type from string.
    /// </summary>
    protected NxEntityMarkType GetMarkTypeByCode(string s)
    {
      switch (s)
      {
        case "R": return NxEntityMarkType.Recent;
        case "B": return NxEntityMarkType.Bookmark;
      }
      return NxEntityMarkType.Bookmark;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity mark type string code.
    /// </summary>
    protected string GetMarkTypeCode(NxEntityMarkType markType)
    {
      switch (markType)
      {
        case NxEntityMarkType.Bookmark: return "B";
        case NxEntityMarkType.Recent: return "R";
        case NxEntityMarkType.Open: return "O";
      }
      return "";
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Does actions when collection is changed.
    /// </summary>
    protected void DoOnChanged()
    {
      if (m_EventDisableCount == 0 && OnChanged != null)
      {
        OnChanged(this, new EventArgs());
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds entity mark by the entity and type.
    /// </summary>
    /// <param name="entity">entity to find</param>
    /// <param name="markType">type of mark</param>
    /// <param name="applicationCd">Application Cd</param>
    /// <returns>found mark or null</returns>
    public CxEntityMark Find(CxBaseEntity entity, NxEntityMarkType markType, string applicationCd)
    {
      return Find(entity, markType, "Edit", applicationCd);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds entity mark by the entity and type.
    /// </summary>
    /// <param name="entity">entity to find</param>
    /// <param name="markType">type of mark</param>
    /// <param name="openMode">entity open mode</param>
    /// <param name="applicationCd">Application cd</param>
    /// <returns>found mark or null</returns>
    public CxEntityMark Find(CxBaseEntity entity, NxEntityMarkType markType, string openMode, string applicationCd)
    {
      if (entity != null)
      {
        foreach (CxEntityMark mark in m_MarksByType[markType])
        {
          if (mark.Equals(entity, openMode, applicationCd))
          {
            return mark;
          }
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds mark to the list of marks.
    /// </summary>
    /// <param name="entity">entity</param>
    /// <param name="markType">mark type</param>
    /// <param name="addToBeginning">true if item should be added to the beginning of the list</param>
    /// <param name="applicationCd">application Cd</param>
    public void AddMark(CxBaseEntity entity, NxEntityMarkType markType, bool addToBeginning,
      string applicationCd)
    {
      AddMark(entity, markType, addToBeginning, "Edit", applicationCd);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds mark to the list of marks.
    /// </summary>
    /// <param name="entity">entity</param>
    /// <param name="markType">mark type</param>
    /// <param name="addToBeginning">true if item should be added to the beginning of the list</param>
    /// <param name="openMode">Marked entity open mode.</param>
    /// <param name="applicationCd">Application Cd</param>
    public bool AddMark(
      CxBaseEntity entity,
      NxEntityMarkType markType,
      bool addToBeginning, 
      string openMode,
      string applicationCd)
    {
      bool result = false;
      if (entity != null &&
          (markType == NxEntityMarkType.Open || !entity.Metadata.BookmarksAndRecentItemsDisabled))
      {
        bool isChanged = false;

        CxEntityMark mark = Find(entity, markType, openMode, applicationCd);
        if (mark == null)
        {
          mark = new CxEntityMark(entity, markType, applicationCd);
          mark.Inserted = true;
          mark.OpenMode = openMode;
          int limit = GetRecordLimit(markType);
          if (limit > 0 && m_MarksByType[markType].Count >= limit)
          {
            int deleteIndex = addToBeginning ? m_MarksByType[markType].Count - 1 : 0;
            InternalDelete(m_MarksByType[markType][deleteIndex]);
            m_MarksByType[markType].RemoveAt(deleteIndex);
          }

          int insertIndex = addToBeginning ? 0 : m_MarksByType[markType].Count;
          m_MarksByType[markType].Insert(insertIndex, mark);

          result = true;
          isChanged = true;
        }
        else
        {
          int oldIndex = m_MarksByType[markType].IndexOf(mark);
          int newIndex = addToBeginning ? 0 : m_MarksByType[markType].Count - 1;
          if (oldIndex != newIndex)
          {
            m_MarksByType[markType].RemoveAt(oldIndex);
            m_MarksByType[markType].Insert(newIndex, mark);

            result = true;
            isChanged = true;
          }
        }

        for (int i = 0; i < m_MarksByType[markType].Count; i++)
        {
          if (m_MarksByType[markType][i].DisplayOrder != i)
          {
            m_MarksByType[markType][i].DisplayOrder = i;
            m_MarksByType[markType][i].Updated = true;
          }
        }

        if (isChanged)
        {
          DoOnChanged();
        }
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds mark to the beginning of list of marks.
    /// </summary>
    /// <param name="entity">entity</param>
    /// <param name="markType">mark type</param>
    /// <param name="applicationCd">application Cd</param>
    public void AddMark(CxBaseEntity entity, NxEntityMarkType markType, string applicationCd)
    {
      AddMark(entity, markType, true, applicationCd);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Marks entity mark as deleted.
    /// </summary>
    /// <param name="mark">entity mark</param>
    protected void InternalDelete(CxEntityMark mark)
    {
      if (!mark.Inserted && !mark.Deleted && IsStorable(mark.MarkType))
      {
        m_DeletedMarks.Add(mark);
      }
      mark.Deleted = true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Deletes entity mark instance from list.
    /// </summary>
    /// <param name="mark">mark to delete</param>
    public void DeleteMark(CxEntityMark mark)
    {
      if (mark != null)
      {
        if (m_MarksByType[mark.MarkType].Remove(mark))
        {
          if (!mark.Deleted)
          {
            InternalDelete(mark);
            DoOnChanged();
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Deletes entity mark.
    /// </summary>
    /// <param name="entity">entity</param>
    /// <param name="markType">mark type</param>
    /// <param name="applicationCd">Application Cd</param>
    public void DeleteMark(CxBaseEntity entity, NxEntityMarkType markType, string applicationCd)
    {
      DeleteMark(Find(entity, markType, applicationCd));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Deletes entity mark.
    /// </summary>
    /// <param name="entity">entity</param>
    /// <param name="applicationCd">Application Cd</param>
    public void DeleteMark(CxBaseEntity entity, string applicationCd)
    {
      foreach (NxEntityMarkType markType in m_MarksByType.Keys)
      {
        DeleteMark(entity, markType, applicationCd);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Deletes all entity marks.
    /// </summary>
    /// <param name="entity">entity</param>
    public void DeleteAllMarks(CxBaseEntity entity)
    {
      bool isChanged = false;
      DisableEvents();
      try
      {
        foreach (KeyValuePair<NxEntityMarkType, List<CxEntityMark>> pair in m_MarksByType)
        {
          for (int i = pair.Value.Count - 1; i >= 0; i--)
          {
            if (pair.Value[i].EqualsByPK(entity))
            {
              DeleteMark(pair.Value[i]);
              isChanged = true;
            }
          }
        }
      }
      finally
      {
        EnableEvents();
      }
      if (isChanged)
      {
        DoOnChanged();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Updates all entity marks.
    /// </summary>
    /// <param name="entity">entity</param>
    public void UpdateAllMarks(CxBaseEntity entity)
    {
      bool isChanged = false;
      DisableEvents();
      try
      {
        foreach (KeyValuePair<NxEntityMarkType, List<CxEntityMark>> pair in m_MarksByType)
        {
          foreach (CxEntityMark mark in pair.Value)
          {
            string name = entity.ToString(mark.EntityUsage, CxEntityDisplayNamePurposes.Bookmark);
            if (mark.EqualsByPK(entity) && mark.Name != name)
            {
              mark.Name = name;
              isChanged = true;
            }
          }
        }
      }
      finally
      {
        EnableEvents();
      }
      if (isChanged)
      {
        DoOnChanged();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Moves entity mark from source type to target type.
    /// </summary>
    /// <param name="entity">entity</param>
    /// <param name="sourceMarkType">source type</param>
    /// <param name="targetMarkType">target type</param>
    /// <param name="applicationCd">Application Cd</param>
    public void MoveMark(
      CxBaseEntity entity,
      NxEntityMarkType sourceMarkType,
      NxEntityMarkType targetMarkType,
      string applicationCd)
    {
      DisableEvents();
      try
      {
        DeleteMark(entity, sourceMarkType, applicationCd);
        AddMark(entity, targetMarkType, applicationCd);
      }
      finally
      {
        EnableEvents();
      }
      DoOnChanged();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Synchronizes entity marks with the entity change.
    /// </summary>
    /// <param name="ea">entity changed event arguments</param>
    public void SynchronizeWithEntityChange(CxEntityChangedEventArgs e)
    {
      if (!CxEntityChangedEventArgs.IsValid(e))
      {
        return;
      }
      switch (e.Change)
      {
        case NxEntityChange.Updated:
          UpdateAllMarks(e.Entity);
          break;
        case NxEntityChange.Deleted:
        case NxEntityChange.DisappearedAfterUpdate:
        case NxEntityChange.DisappearedAfterRefresh:
          DeleteAllMarks(e.Entity);
          break;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Disables events.
    /// </summary>
    public void DisableEvents()
    {
      m_EventDisableCount++;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Enables events.
    /// </summary>
    public void EnableEvents()
    {
      if (m_EventDisableCount > 0)
      {
        m_EventDisableCount--;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Refreshes entity marks.
    /// </summary>
    public void Refresh()
    {
      bool isChanged = false;
      DisableEvents();
      try
      {
        foreach (KeyValuePair<NxEntityMarkType, List<CxEntityMark>> pair in m_MarksByType)
        {
          int limit = GetRecordLimit(pair.Key);
          int count = m_MarksByType[pair.Key].Count;
          if (count > limit && limit >= 0)
          {
            pair.Value.RemoveRange(limit, count - limit);
            isChanged = true;
          }
        }
      }
      finally
      {
        EnableEvents();
      }
      if (isChanged)
      {
        DoOnChanged();
      }
    }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// List of recent items.
    /// </summary>
    public List<CxEntityMark> RecentItems
    { get { return m_MarksByType[NxEntityMarkType.Recent]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// List of bookmarks.
    /// </summary>
    public List<CxEntityMark> BookmarkItems
    { get { return m_MarksByType[NxEntityMarkType.Bookmark]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// List of open items.
    /// </summary>
    public List<CxEntityMark> OpenItems
    { get { return m_MarksByType[NxEntityMarkType.Open]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Invokes when entity marks collection is changed.
    /// </summary>
    public event EventHandler OnChanged;
    //-------------------------------------------------------------------------
  }
}