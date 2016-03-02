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
using System.Text;
using Framework.Db;
using Framework.Metadata;
using Framework.Utils;

namespace Framework.Entity
{
  //---------------------------------------------------------------------------
  /// <summary>
  /// Type of marked entity.
  /// </summary>
  public enum NxEntityMarkType { Recent, Bookmark, Open }
  //---------------------------------------------------------------------------

  //---------------------------------------------------------------------------
  /// <summary>
  /// Class representing marked entity information.
  /// </summary>
  public class CxEntityMark
  {
    //-------------------------------------------------------------------------
    protected CxEntityUsageMetadata m_EntityUsage = null;
    protected object[] m_PrimaryKeyValues = null;
    protected NxEntityMarkType m_MarkType = NxEntityMarkType.Recent;
    protected string m_Name = null;
    protected int m_DisplayOrder = 0;
    protected bool m_Inserted = false;
    protected bool m_Updated = false;
    protected bool m_Deleted = false;
    private string m_OpenMode = "Edit";
    private string m_ApplicationCd = string.Empty;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">metadata holder</param>
    /// <param name="entityUsageId">ID of the entity usage metadata</param>
    /// <param name="primaryKeyText">primary key values encoded into string</param>
    /// <param name="markType">type of entity mark</param>
    /// <param name="displayName">display name</param>
    public CxEntityMark(
      CxMetadataHolder holder,
      string entityUsageId,
      string primaryKeyText,
      NxEntityMarkType markType,
      string displayName)
    {
      m_EntityUsage = holder.EntityUsages[entityUsageId];
      m_PrimaryKeyValues = m_EntityUsage.DecodePrimaryKeyValuesFromString(primaryKeyText);
      m_MarkType = markType;
      m_Name = displayName;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="entity">entity to create mark for</param>
    /// <param name="markType">type of entity mark</param>
    /// <param name="applicationCd">Application Cd</param>
    public CxEntityMark(CxBaseEntity entity, NxEntityMarkType markType, string applicationCd)
    {
      m_EntityUsage = entity.Metadata.BookmarksAndRecentItemsEntityMetadata;
      string[] pkNames;
      entity.GetPrimaryKeyValues(out pkNames, out m_PrimaryKeyValues);
      m_MarkType = markType;
      m_Name = entity.ToString(CxEntityDisplayNamePurposes.Bookmark);
      m_ApplicationCd = applicationCd;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates entity and reads it from the database.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <returns>created entity or null</returns>
    public CxBaseEntity CreateAndReadFromDb(CxDbConnection connection)
    {
      if (m_PrimaryKeyValues != null && 
          m_PrimaryKeyValues.Length > 0 && 
          CxUtils.NotEmpty(m_PrimaryKeyValues[0]))
      {
        return CxBaseEntity.CreateAndReadFromDb(m_EntityUsage, connection, m_PrimaryKeyValues);
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Clears inserted, updated and deleted flags.
    /// </summary>
    public void ClearModificationFlags()
    {
      m_Inserted = false;
      m_Updated = false;
      m_Deleted = false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if mark entity equals to the given entity.
    /// </summary>
    /// <param name="entity">entity to compare with</param>
    public bool Equals(CxBaseEntity entity)
    {
      return Equals(entity, "");
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if mark entity equals to the given entity.
    /// </summary>
    /// <param name="entity">entity to compare with</param>
    /// <param name="openMode">Entity open mode.</param>
    /// <param name="applicationCd">ApplicationCd.</param>
    public bool Equals(CxBaseEntity entity, string openMode, string applicationCd)
    {
      return entity != null && 
             entity.Metadata.BookmarksAndRecentItemsEntityMetadata == m_EntityUsage && 
             entity.PrimaryKeyAsString == PrimaryKeyText &&
             OpenMode == openMode &&
             ApplicationCd == applicationCd;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if mark entity equals to the given entity.
    /// </summary>
    /// <param name="entity">entity to compare with</param>
    public bool EqualsByPK(CxBaseEntity entity)
    {
      return entity != null &&
             entity.Metadata.EntityId == m_EntityUsage.EntityId &&
             entity.PrimaryKeyAsString == PrimaryKeyText;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Compares two entity marks by the order.
    /// </summary>
    static public int CompareByOrder(CxEntityMark x, CxEntityMark y)
    {
      if (x == null && y == null)
      {
        return 0;
      }
      else if (x == null && y != null)
      {
        return -1;
      }
      else if (x != null && y == null)
      {
        return 1;
      }
      else
      {
        int result = x.DisplayOrder.CompareTo(y.DisplayOrder);
        if (result == 0)
        {
          result = String.Compare(x.Name, y.Name);
        }
        return result;
      }
    }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity usage metadata.
    /// </summary>
    public CxEntityUsageMetadata EntityUsage
    { get { return m_EntityUsage; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns primary key values array.
    /// </summary>
    public object[] PrimaryKeyValues
    { get { return m_PrimaryKeyValues; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity mark type.
    /// </summary>
    public NxEntityMarkType MarkType
    { get { return m_MarkType; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets entity display name.
    /// </summary>
    public string Name
    { get { return m_Name; } set { m_Name = value; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets display order.
    /// </summary>
    public int DisplayOrder
    { get { return m_DisplayOrder; } set { m_DisplayOrder = value; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns primary key values encoded into the string.
    /// </summary>
    public string PrimaryKeyText
    {
      get
      {
        return EntityUsage.EncodePrimaryKeyValuesAsString(m_PrimaryKeyValues);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns unique ID to identify within application.
    /// </summary>
    public string UniqueId
    {
      get
      {
        string openMode = CxUtils.NotEmpty(OpenMode) ? OpenMode : "Edit";
        return m_EntityUsage.Id + ":" + m_MarkType + ":" + PrimaryKeyText + openMode + ApplicationCd;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if entity mark is inserted.
    /// </summary>
    public bool Inserted
    { get { return m_Inserted; } set { m_Inserted = value; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if entity mark is updated.
    /// </summary>
    public bool Updated
    { get { return m_Updated; } set { m_Updated = value; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if entity mark is deleted.
    /// </summary>
    public bool Deleted
    { get { return m_Deleted; } set { m_Deleted = value; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Marked entity open mode.
    /// </summary>
    public string OpenMode 
    {
      get { return m_OpenMode; }
      set { m_OpenMode = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Marked entity open mode.
    /// </summary>
    public string ApplicationCd
    {
      get { return m_ApplicationCd; }
      set { m_ApplicationCd = value; }
    }
    //-------------------------------------------------------------------------
  }
}