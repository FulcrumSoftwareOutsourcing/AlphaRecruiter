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
        [Authorize]
        public ActionResult GetMetadata(string entityUsageId, string[] relaredEntityUsageIds, string requiredSettings, string settingsToSave)
        {
            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
//#if (!DEBUG)
            try
            {
                //#endif


                


                if (string.IsNullOrEmpty(entityUsageId) == false)
                {

                    CxClientEntityMetadata meta = GetEntityMetadata(entityUsageId);

                  


                    AddFileContentStateAttrsInGridOrder(meta);
                    var relatedMetaIds = meta.Commands.Where(c => string.IsNullOrEmpty(c.EntityUsageId) == false).Select(c => c.EntityUsageId);
                    List<CxClientEntityMetadata> relatedMeta = new List<CxClientEntityMetadata>();
                    foreach (string id in relatedMetaIds)
                    {
                        CxClientEntityMetadata relMeta = GetEntityMetadata(id);
                        AddFileContentStateAttrsInGridOrder(relMeta);
                        relatedMeta.Add(relMeta);
                    }

                    var result = new { Metadata = meta, RelatedMetadata = relatedMeta, Settings = GetSettings(requiredSettings, null) };
                    json.Data = result;

                    SaveSetting(settingsToSave);

                    return json;
                }
                if(relaredEntityUsageIds != null)
                {
                    List<CxClientEntityMetadata> relatedMeta = new List<CxClientEntityMetadata>();
                    foreach (string id in relaredEntityUsageIds)
                    {
                        CxClientEntityMetadata relMeta = GetEntityMetadata(id);
                        AddFileContentStateAttrsInGridOrder(relMeta);
                        relatedMeta.Add(relMeta);
                    }

                    var result = new { RelatedMetadata = relatedMeta, Settings = GetSettings(requiredSettings, null) };
                    json.Data = result;

                    SaveSetting(settingsToSave);

                    return json;
                }


                return null;
                
//#if (!DEBUG)
            }
            catch (Exception ex)
            {
                var result = new { Error = ex.Message };

                json.Data = result;
                return json;
            }
//#endif
        }
    }

}
