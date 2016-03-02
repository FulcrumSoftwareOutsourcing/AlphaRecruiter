using App.Server.Models.Markup;
using App.Server.Models.Settings;
using Framework.Db;
using Framework.Entity;
using Framework.Entity.Filter;
using Framework.Metadata;
using Framework.Remote;
using Framework.Remote.Mobile;
using Framework.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
namespace App.Server.Controllers
{
    public partial class HomeController
    {
        private CxClientEntityMetadata GetEntityMetadata(string entityUsageId)
        {
          
           CxEntityUsageMetadata meta = mHolder.EntityUsages[entityUsageId];

            Dictionary<string, object> filterDefaults1 = new Dictionary<string, object>();
            Dictionary<string, object> filterDefaults2 = new Dictionary<string, object>();
            using (CxDbConnection conn = CxDbConnections.CreateEntityConnection())
            {
                foreach (var attr in meta.Attributes)
                {
                    if (!string.IsNullOrWhiteSpace(attr.FilterDefault1))
                        filterDefaults1.Add(attr.Id, CxBaseEntity.CalculateDefaultValue(attr.FilterDefault1, attr, conn, null, null, null) );
                    if (!string.IsNullOrWhiteSpace(attr.FilterDefault2))
                        filterDefaults2.Add(attr.Id, CxBaseEntity.CalculateDefaultValue(attr.FilterDefault2, attr, conn, null, null, null));
                }
            }





            CxClientEntityMetadata entityMetadata = new CxClientEntityMetadata(mHolder, meta, filterDefaults1, filterDefaults2);
           entityMetadata.AttributesList.AddRange(entityMetadata.Attributes.Values);

           InitApplicationValues(entityMetadata.ApplicationValues);

            


           return entityMetadata;
          

        }

        private IEnumerable<CxClientEntityMetadata> GetMetadata(IEnumerable<string> ids)
        {
            List<CxClientEntityMetadata> metadata = new List<CxClientEntityMetadata>();
            if (ids != null)
            {
                foreach (string entityUsageId in ids)
                {
                    metadata.Add(GetEntityMetadata(entityUsageId));
                }
            }
            return metadata;
        }

        private void AddFileContentStateAttrsInGridOrder(CxClientEntityMetadata meta)
        {
            List<string> toAdd = new List<string>();
            foreach(string attrId in meta.GridOrderedAttributes)
            {
                CxClientAttributeMetadata attr = meta.Attributes[attrId];
                if (attr.Type == "file" || attr.Type == "photo")
                {
                    toAdd.Add(attrId + "STATE");
                }
            }

            meta.GridOrderedAttributes.AddRange(toAdd);

        }
    }
}