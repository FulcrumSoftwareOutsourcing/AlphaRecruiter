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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Framework.Db;
using Framework.Entity;
using Framework.Entity.Filter;
using Framework.Metadata;
using Framework.Remote.Mobile;
using Framework.Utils;
using Framework.Web.Utils;

namespace Framework.Remote
{
  public partial class CxAppServer
  {
    /// <summary>
    /// Exporets data to CSV format.
    /// </summary>
    /// <param name="prms">Query parameters.</param>
    /// <returns>Initialized CxExportToCsvInfo</returns>
    public CxExportToCsvInfo ExportToCsv(CxQueryParams prms)
    {
      try
      {
        string csv = GetCsv(prms);

        Guid csvId = Guid.NewGuid();
        TimeSpan timeoutSpan = new TimeSpan(0, 0, 0, 0, 60000);
        Cache cache = HttpContext.Current.Cache;
        cache.Insert(
          csvId.ToString(),
          csv,
          null,
          Cache.NoAbsoluteExpiration,
          timeoutSpan,
          CacheItemPriority.NotRemovable,
          null);

        return new CxExportToCsvInfo() { StreamId = csvId };
      }
      catch (Exception ex)
      {
        CxExportToCsvInfo emptyInfo = new CxExportToCsvInfo { Error = new CxExceptionDetails(ex) };
        return emptyInfo;
      }

    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates and returns csv string. 
    /// </summary>
    /// <param name="prms">Parameters to create csv.</param>
    /// <returns>Created csv string.</returns>
    private string GetCsv(CxQueryParams prms)
    {
      CxEntityUsageMetadata entityUsage = m_Holder.EntityUsages[prms.EntityUsageId];
      using (CxDbConnection connection = CxDbConnections.CreateEntityConnection())
      {
        IList<string> whereParamsMames =
          CxDbParamParser.GetList(entityUsage.WhereClause, true);

        // Obtaining the parent entity.
        CxBaseEntity parent = null;
        if (prms.ParentEntityUsageId != null)
        {
          CxEntityUsageMetadata parentEntityUsage = m_Holder.EntityUsages[prms.ParentEntityUsageId];
          IList<string> parentPkNames = parentEntityUsage.PrimaryKeyIds;
          IxValueProvider parentVlProvider =
            CxQueryParams.CreateValueProvider(prms.ParentPks);
          parent = CxBaseEntity.CreateAndReadFromDb
            (parentEntityUsage,
             connection,
             parentVlProvider);
        }

        IxValueProvider paramsProvider =
          CxValueProviderCollection.Create(
            parent,
            CxQueryParams.CreateValueProvider(prms.WhereValues),
            m_Holder.ApplicationValueProvider);

        foreach (CxFilterItem filterItem in prms.FilterItems)
        {
          filterItem.Operation =
            (NxFilterOperation) Enum.Parse(typeof(NxFilterOperation), filterItem.OperationAsString);
          CxFilterOperator filterOperator
            = CxFilterOperator.Create(entityUsage, filterItem);
          if (filterOperator != null)
          {
            filterOperator.InitializeValueProvider(paramsProvider);
          }

        }
        string filterCondition = GetFilterCondition(entityUsage, prms.FilterItems.ToList<IxFilterElement>());


        // Composing the list of sort descriptors from the input params.
        CxSortDescriptorList sortings = new CxSortDescriptorList();
        if (prms.SortDescriptions != null)
        {
          foreach (CxSortDescription sorting in prms.SortDescriptions)
          {
            sortings.Add(new CxSortDescriptor(sorting.AttributeId,
                                              sorting.Direction == NxListSortDirection.Ascending
                                                ? ListSortDirection.Ascending
                                                : ListSortDirection.Descending));
          }
        }

        List<string> orderByClauses = new List<string>();
        orderByClauses.Add(connection.ScriptGenerator.GetOrderByClause(sortings));
        string completeOrderByClause = string.Join(",", orderByClauses.ToArray());

        DataTable dt = new DataTable();
        if (prms.JoinValues != null && prms.JoinValues.Count() > 0)
        {
          IList<string> joinParamsNames = CxDbParamParser.GetList(entityUsage.JoinCondition, true);

          paramsProvider =
            CxValueProviderCollection.Create(
              CxQueryParams.CreateValueProvider(prms.JoinValues),
              m_Holder.ApplicationValueProvider);

          entityUsage.ReadChildData(
          connection, dt,
          paramsProvider, filterCondition,
          NxEntityDataCache.NoCache,
          prms.StartRecordIndex, prms.RecordsAmount);
        }
        else
        {
          entityUsage.ReadData(connection, dt, filterCondition, paramsProvider, completeOrderByClause);
        }

        Dictionary<string, string> columnsCaptions = new Dictionary<string, string>();
        List<string> columnsToExport = new List<string>();
        foreach (CxAttributeMetadata attribute in entityUsage.GetAttributeOrder(NxAttributeContext.GridVisible).OrderAttributes)
        {
          columnsToExport.Add(attribute.Id);
          columnsCaptions.Add(attribute.Id, string.Concat("\"", attribute.Caption, "\""));
        }

        IEnumerable<CxAttributeMetadata> rsAttributes =
          (from attribute in entityUsage.Attributes
           where attribute.RowSource != null &&
             string.IsNullOrEmpty(attribute.RowSource.EntityUsageId) == false
           select attribute).ToList();

        Dictionary<string, IList<CxComboItem>> rsValues = new Dictionary<string, IList<CxComboItem>>();
        foreach (CxAttributeMetadata attrMetadata in rsAttributes)
        {
          IList<CxComboItem> comboItems =
            attrMetadata.RowSource.GetList(null, connection, null, null, true);
          rsValues.Add(attrMetadata.Id, comboItems);

          int rsClmOrdinal = 0;
          List<DataColumn> columnsToAdd = new List<DataColumn>();
          foreach (DataColumn column in dt.Columns)
          {
            if (column.ColumnName.ToUpper() == attrMetadata.Id)
            {
              DataColumn newRsTextClm = new DataColumn(attrMetadata.Id)
              {
                DataType = typeof(string)
              };
              columnsToAdd.Add(newRsTextClm);
              column.ColumnName = string.Concat("_", column.ColumnName);
              rsClmOrdinal = column.Ordinal;
              break;
            }
          }
          dt.Columns.AddRange(columnsToAdd.ToArray());

          if (dt.Columns.Cast<DataColumn>().Contains(x => CxText.Equals(x.ColumnName, attrMetadata.Id)))
          {
            foreach (DataRow row in dt.Rows)
            {
              object rsID = row[rsClmOrdinal];
              List<string> rsTexts = (from item in comboItems
                                      where (rsID != null) &&
                                            (item.Value != null) &&
                                            item.Value.Equals(rsID)
                                      select item.Description).ToList();

              row[attrMetadata.Id] = rsTexts.Count > 0 ? rsTexts[0] : string.Empty;
            }
          }
        }

        return CxCSV.DataTableToCsv(
          dt,
          columnsToExport,
          true,
          columnsCaptions,
          null,
          CxWebUtils.CurrentCulture.TextInfo.ListSeparator,
          CxWebUtils.CurrentCulture.TextInfo.ListSeparator);
      }
    }
  }
}
