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
    /// Returns roesources for filter form.
    /// </summary>
    /// <param name="marker">Server operation identifier.</param>
    /// <param name="prms">Query parameters.</param>
    /// <returns>Initialized CxModel</returns>    
    public CxModel GetFilterFormRowSources(Guid marker, CxQueryParams prms)
    {
      try
      {
        CxEntityUsageMetadata entityUsage = m_Holder.EntityUsages[prms.EntityUsageId];
        CxModel model = new CxModel();
        using (CxDbConnection conn = CxDbConnections.CreateEntityConnection())
        {
          CxBaseEntity[] entities = new CxBaseEntity[] { };


          IEnumerable<CxAttributeMetadata> rsAttributes =
            (from attribute in entityUsage.Attributes
             where attribute.RowSource != null &&
               string.IsNullOrEmpty(attribute.RowSource.EntityUsageId) == false
             select attribute).ToList();

          Dictionary<string, CxClientRowSource> unfilteredRowSources = new Dictionary<string, CxClientRowSource>();
          foreach (CxAttributeMetadata attrMetadata in rsAttributes)
          {
            CxClientRowSource clientRS = new CxClientRowSource
            {
              RowSourceId = attrMetadata.RowSourceId,
              RowSourceData = new List<CxClientRowSourceItem>()
            };


            IList<CxComboItem> comboItems =
              attrMetadata.RowSource.GetList(null, conn, null, null, true);




            foreach (CxComboItem comboItem in comboItems)
            {
              CxClientRowSourceItem clienRsItem =
                new CxClientRowSourceItem
                {
                  Value = comboItem.Value,
                  Text = comboItem.Description,
                  ImageId = comboItem.ImageReference
                };

              clientRS.RowSourceData.Add(clienRsItem);
            }
            if (!unfilteredRowSources.ContainsKey(clientRS.RowSourceId.ToUpper()))
            {
              unfilteredRowSources.Add(clientRS.RowSourceId.ToUpper(), clientRS);
            }
          }
          model = new CxModel
          {
            Marker = marker,
            EntityUsageId = entityUsage.Id,
            UnfilteredRowSources = unfilteredRowSources,
            FilteredRowSources = new List<CxClientRowSource>(),
            TotalDataRecordAmount = entities.Length
          };
          model.SetData(entityUsage, entities, conn);


          InitApplicationValues(model.ApplicationValues);
          return model;
        }
      }
      catch (Exception ex)
      {
        CxExceptionDetails exceptionDetails = new CxExceptionDetails(ex);
        CxModel model = new CxModel { Error = exceptionDetails };
        return model;
      }
    }
  }
}
