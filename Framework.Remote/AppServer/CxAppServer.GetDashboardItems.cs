using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Framework.Db;
using Framework.Entity;
using Framework.Metadata;
using Framework.Remote.Mobile;
using Framework.Utils;

namespace Framework.Remote
{
  public partial class CxAppServer
  {
    /// <summary>
    /// Returns dashboard items
    /// </summary>
    public CxClientDashboardData GetDashboardData(string dasboardId)
    {
      try
      {
        var dashboard = m_Holder.SlDashboards[dasboardId];
        Dictionary<string, IList<CxSlDashboardItemMetadata>> itemsByEntityUsage = new Dictionary<string,IList<CxSlDashboardItemMetadata>>();
        Dictionary<string, string> entityUsageSelects = new Dictionary<string, string>();
        foreach (var dashboardItem in dashboard.Items)
        {
          if (string.IsNullOrEmpty(dashboardItem.EntityUsageId))
            throw new Exception("Entity Usage Id not defined");
          CxEntityUsageMetadata entityUsage = m_Holder.EntityUsages.Find(dashboardItem.EntityUsageId);
          if (entityUsage == null)
            throw new Exception(string.Format("No entity usage found for dashboard item <{0}>", dashboardItem.Id));
          if (!m_Holder.Security.GetRight(entityUsage))
            continue;
          if (!entityUsageSelects.ContainsKey(entityUsage.Id))
            entityUsageSelects.Add(entityUsage.Id, entityUsage.ComposeReadDataSql());

          if (!itemsByEntityUsage.ContainsKey(entityUsage.Id))
            itemsByEntityUsage[entityUsage.Id] = new List<CxSlDashboardItemMetadata>();
          itemsByEntityUsage[entityUsage.Id].Add(dashboardItem);
        }
        DataTable queryResult = new DataTable();
        using (CxDbConnection conn = CxDbConnections.CreateEntityConnection())
        {
          string query = conn.ScriptGenerator.GetQueryForAmountOfItems(entityUsageSelects);
          conn.GetQueryResult(queryResult, query, m_Holder.ApplicationValueProvider);
        }

        List<CxClientDashboardItem> clientItems = new List<CxClientDashboardItem>();
        foreach (DataRow resultRow in queryResult.Rows)
        {
          var entityUsageId = Convert.ToString(resultRow[0]);
          var slItems = itemsByEntityUsage[entityUsageId];
          foreach (var slItem in slItems)
          {
            CxClientDashboardItem item = new CxClientDashboardItem();
            item.EntityUsageId = entityUsageId;
            if (slItem.ImageId != null)
              item.ImageId = slItem.ImageId.ToUpper();
            item.Text = slItem.Text;
            item.Content = Convert.ToString(resultRow[1]);
            item.TreeItemId = slItem.TreeItemId;
            item.SectionId = slItem.SectionId;
            clientItems.Add(item);
          }
        }
        return new CxClientDashboardData { 
          DashboardItems = clientItems.ToArray(),
          Text = dashboard.Text
        };
      }
      catch (Exception ex)
      {
        CxExceptionDetails exceptionDetails = new CxExceptionDetails(ex);
        CxClientDashboardData board = new CxClientDashboardData { Error = exceptionDetails };
        return board;
      }
    }

   

  }
}

