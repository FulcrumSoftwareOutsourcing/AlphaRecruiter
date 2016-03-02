/********************************************************************
 *  FulcrumWeb RAD Framework - Fulcrum of your business             *
 *  Copyright (c) 2002-2009 FulcrumWeb, ALL RIGHTS RESERVED         *
 *                                                                  *
 *  THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      *
 *  FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        *
 *  COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       *
 *  AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  *
 *  AND PERMISSION FROM FULCRUMWEB. CONSULT THE END USER LICENSE    *
 *  AGREEMENT FOR INFORMATION ON ADDITIONAL RESTRICTIONS.           *
 ********************************************************************/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

using Framework.Db;
using Framework.Entity;
using Framework.Metadata;
using Framework.Utils;

namespace Framework.Remote
{
  public class CxSlTreeItemProviderEntityList: IxSlTreeItemsProvider
  {
    //-------------------------------------------------------------------------
    static private readonly UniqueList<CxEntityUsageMetadata> m_EntityUsages =
      new UniqueList<CxEntityUsageMetadata>();
    static private readonly ReadOnlyCollection<CxEntityUsageMetadata> m_ReadOnlyEntityUsages =
      new ReadOnlyCollection<CxEntityUsageMetadata>(m_EntityUsages);
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of dynamic tree items.
    /// </summary>
    /// <param name="parentItem">parent tree item</param>
    /// <returns>list of CxWinTreeItemMetadata objects</returns>
    public IList<CxSlTreeItemMetadata> GetItems(CxSlTreeItemMetadata parentItem)
    {
      if (parentItem.EntityUsage != null)
      {
        m_EntityUsages.Add(parentItem.EntityUsage);
        IList<string> entityUsageIds = CxText.DecomposeWithSeparator(parentItem.DependsOnEntityUsageIds, ",");
        foreach (string entityUsageId in entityUsageIds)
        {
          CxEntityUsageMetadata entityUsage = parentItem.EntityUsage.Holder.EntityUsages.Find(entityUsageId);
          if (entityUsage != null)
          {
            m_EntityUsages.Add(entityUsage);
          }
        }

        List<CxSlTreeItemMetadata> result = new List<CxSlTreeItemMetadata>();
        DataTable dt = new DataTable();
        using (CxDbConnection connection = CxDbConnections.CreateEntityConnection())
        {
          parentItem.EntityUsage.ReadData(connection, dt);
        }
        DataView dv = new DataView(dt, null, parentItem.EntityUsage.OrderByClause, DataViewRowState.CurrentRows);
        foreach (DataRowView drv in dv)
        {
          CxBaseEntity entity = CxBaseEntity.CreateByDataRow(parentItem.EntityUsage, drv.Row);
          CxSlTreeItemMetadata item = new CxSlTreeItemMetadata(
            parentItem.Holder, 
            parentItem.Section, 
            parentItem.Id + " " + entity.PrimaryKeyAsString)
                                        {Text = entity.DisplayName};
          item["entity_usage_id"] = parentItem.EntityUsage.Id;
          item["image_id"] = parentItem.ImageId;
          item["frame_class_id"] = parentItem.FrameClassId;
          item.Tag = entity;
          result.Add(item);
        }
        return result;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// List of entity usages used in the entity list tree item providers.
    /// </summary>
    static public IList<CxEntityUsageMetadata> EntityUsages
    {
      get { return m_ReadOnlyEntityUsages; }
    }
    //-------------------------------------------------------------------------
  }
}
