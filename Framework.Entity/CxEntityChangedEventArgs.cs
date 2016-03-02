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
using System.Text;
using Framework.Metadata;

namespace Framework.Entity
{
  //------------------------------------------------------------------------------
  /// <summary>
  /// Entity change notification enumeration.
  /// Disappeared means that entity was inserted or updated, 
  /// but it is not found during reload from DB.
  /// </summary>
  public enum NxEntityChange
  {
    None,
    Inserted,
    Updated,
    Deleted,
    DisappearedAfterInsert,
    DisappearedAfterUpdate,
    DisappearedAfterRefresh,
    ChildEntityModified
  }
  //------------------------------------------------------------------------------

  //------------------------------------------------------------------------------
  /// <summary>
  /// Entity changed notification event arguments.
  /// </summary>
  public class CxEntityChangedEventArgs
  {
    //----------------------------------------------------------------------------
    protected CxBaseEntity m_Entity = null;
    protected NxEntityChange m_Change = NxEntityChange.None;
    protected CxChildEntityUsageMetadata m_ChildEntityUsage = null;
    //----------------------------------------------------------------------------

    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public CxEntityChangedEventArgs(CxBaseEntity entity, NxEntityChange change)
    {
      m_Entity = entity;
      m_Change = change;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public CxEntityChangedEventArgs(
      CxBaseEntity entity, 
      CxChildEntityUsageMetadata childEntityUsage)
    {
      m_Entity = entity;
      m_Change = NxEntityChange.ChildEntityModified;
      m_ChildEntityUsage = childEntityUsage;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if given entity change event arguments are valid for processing.
    /// </summary>
    static public bool IsValid(CxEntityChangedEventArgs e)
    {
      return e != null && e.Entity != null && e.Change != NxEntityChange.None;
    }
    //-------------------------------------------------------------------------

    //----------------------------------------------------------------------------
    /// <summary>
    /// Modified entity.
    /// </summary>
    public CxBaseEntity Entity
    { get { return m_Entity; } }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Entity modification kind.
    /// </summary>
    public NxEntityChange Change
    { get { return m_Change; } }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Modified child entity usage.
    /// </summary>
    public CxChildEntityUsageMetadata ChildEntityUsage
    { get { return m_ChildEntityUsage; } }
    //----------------------------------------------------------------------------
  }
}