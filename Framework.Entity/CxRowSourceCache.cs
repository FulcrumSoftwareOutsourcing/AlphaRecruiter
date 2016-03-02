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
using Framework.Metadata;
using Framework.Utils;
using System.Collections;

namespace Framework.Entity
{
  /// <summary>
  /// Utility methods to work with row source cache
  /// </summary>
  public class CxRowSourceCache
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Refreshes row sources cache on entity changed.
    /// </summary>
    /// <param name="ea">entity change event arguments</param>
    static public void RefreshOnEntityChanged(CxEntityChangedEventArgs e)
    {
      if (!CxEntityChangedEventArgs.IsValid(e))
      {
        return;
      }
      foreach (CxRowSourceMetadata rowSource in e.Entity.Metadata.Holder.RowSources.RowSources.Values)
      {
        if (IsRowSourceMatch(rowSource, e))
        {
          RefreshOnEntityChanged(rowSource, e);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Refreshes a part of row source cache that concerns on the given entity usage.
    /// </summary>
    static public void RefreshOnEntityUsageChanged(CxEntityUsageMetadata entityUsage)
    {
      foreach (CxRowSourceMetadata rowSource in entityUsage.Holder.RowSources.RowSources.Values)
      {
        if (IsRowSourceMatch(rowSource, entityUsage))
          rowSource.ResetCache();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates if some of the given child entity usages match 
    /// the entity usage of the given row source.
    /// </summary>
    static protected bool IsRowSourceMatch(
      CxRowSourceMetadata rowSource,
      ICollection childEntityUsages)
    {
      foreach (CxChildEntityUsageMetadata childEntityUsage in childEntityUsages)
      {
        if (childEntityUsage.EntityUsage.EntityId == rowSource.EntityUsage.EntityId)
          return true;
      }
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates if the given entity usage matches the given row source.
    /// </summary>
    static protected bool IsRowSourceMatch(
      CxRowSourceMetadata rowSource,
      CxEntityUsageMetadata entityUsage)
    {
      return
        rowSource != null &&
        !rowSource.HardCoded &&
        CxUtils.NotEmpty(rowSource.EntityUsageId) &&
        (entityUsage.EntityId == rowSource.EntityUsage.EntityId ||
         IsRowSourceMatch(rowSource, entityUsage.ChildEntityUsages.Values));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if given row source should be refreshed on the entity change
    /// </summary>
    /// <param name="rowSource">row source metadata</param>
    /// <param name="ea">entity change event arguments</param>
    static protected bool IsRowSourceMatch(
      CxRowSourceMetadata rowSource,
      CxEntityChangedEventArgs e)
    {
      return rowSource != null &&
             !rowSource.HardCoded &&
             CxUtils.NotEmpty(rowSource.EntityUsageId) &&
             ((e.Change != NxEntityChange.ChildEntityModified && 
               e.Entity.Metadata.EntityId == rowSource.EntityUsage.EntityId) ||
              (e.Change == NxEntityChange.ChildEntityModified && 
               e.ChildEntityUsage != null &&
               e.ChildEntityUsage.EntityUsage.EntityId == rowSource.EntityUsage.EntityId));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Refreshes row source on entity value changed.
    /// </summary>
    /// <param name="rowSource">row source metadata</param>
    /// <param name="ea">entity change event arguments</param>
    static protected void RefreshOnEntityChanged(
      CxRowSourceMetadata rowSource,
      CxEntityChangedEventArgs e)
    {
      if (rowSource.EntityUsage.IsRefreshDependentOn(e.Entity.Metadata))
        rowSource.ResetCache();
      else
      {
        switch (e.Change)
        {
          case NxEntityChange.Inserted:
            if (e.Entity.Metadata.SqlSelect.ToUpper() == rowSource.EntityUsage.SqlSelect.ToUpper() &&
                (CxUtils.IsEmpty(rowSource.EntityUsage.WhereClause) ||
                 e.Entity.Metadata.WhereClause.ToUpper() == rowSource.EntityUsage.WhereClause.ToUpper()))
            {
              rowSource.UpdateCachedRow(e.Entity, e.Entity.Metadata);
            }
            break;
          case NxEntityChange.Updated:
            rowSource.UpdateCachedRow(e.Entity, e.Entity.Metadata, false);
            break;
          case NxEntityChange.Deleted:
          case NxEntityChange.DisappearedAfterUpdate:
          case NxEntityChange.DisappearedAfterRefresh:
            rowSource.DeleteCachedRow(e.Entity, e.Entity.Metadata);
            break;
          case NxEntityChange.ChildEntityModified:
            rowSource.ResetCache();
            break;
        }
      }
    }
    //-------------------------------------------------------------------------
  }
}