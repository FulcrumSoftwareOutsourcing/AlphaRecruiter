using App.Server.Models.Markup;
using App.Server.Models.Settings;
using Framework.Db;
using Framework.Metadata;
using Framework.Remote;
using Framework.Remote.Mobile;
using Framework.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;


namespace App.Server.Controllers
{
    public partial class HomeController
    {
        private List<CxClientMultilanguageItem> GetClientMultilanguage(CxSlMetadataHolder mHolder)
        {

            using (CxDbConnection conn = CxDbConnections.CreateEntityConnection())
            {
                if(mHolder.SlSections.JsAppProperties.Count == 0 ||  mHolder.SlSections.JsAppProperties["app_name"] == null)
                    return new List<CxClientMultilanguageItem>();


                string userLang = GetUserLanguage(conn);
                DataTable mlItemsTbl = new DataTable();
                conn.GetQueryResult(mlItemsTbl,
                    @"select * from Framework_LocalizationItems 
                where IsNotUsed = 0 and
                  ApplicationCd = '" + mHolder.SlSections.JsAppProperties["app_name"]  + "' OR ApplicationCd = '" + mHolder.ApplicationCode  +  @"' and
                  ObjectTypeCd like 'Sl_%'");

                List<CxClientMultilanguageItem> items = new List<CxClientMultilanguageItem>();
                foreach (DataRow row in mlItemsTbl.Rows)
                {
                    string objectTypeCd = row["ObjectTypeCd"].ToString();
                    string propertyCd = row["PropertyCd"].ToString();
                    string objectName = row["ObjectName"].ToString();
                    string defaultValue = row["DefaultValue"].ToString();


                 

                    string slLocalizedValue = mHolder.Multilanguage.GetLocalizedValue(
                        userLang,
                        objectTypeCd,
                        propertyCd,
                        objectName,
                        defaultValue);

                    string objectNamespace = "";
                    string objectNamePart = "";
                    string objectParent = "";

                    if (string.Compare(objectTypeCd, "SL_Text", true) != 0)
                    {
                        string[] splittedName = objectName.Split('|');
                        if (splittedName.Length > 1)
                        {
                            objectNamespace = splittedName[0];
                            string[] parentAndName = splittedName[1].Split('.');
                            objectParent = parentAndName[0];
                            objectNamePart = parentAndName[1];
                        }
                    }
                    else
                    {
                        objectNamePart = objectName;
                    }

                    CxClientMultilanguageItem item = new CxClientMultilanguageItem(
                        slLocalizedValue,
                        defaultValue,
                        objectTypeCd.TrimStart(new[] { 'S', 'L', '_' }),
                        objectNamespace,
                        objectNamePart,
                        propertyCd,
                        objectParent);
                    items.Add(item);
                }

                return items;
            }

        }

    }
}