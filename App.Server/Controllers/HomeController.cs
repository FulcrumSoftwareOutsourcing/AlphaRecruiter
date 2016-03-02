using App.Server.Models.Markup;
using App.Server.Models.Settings;
using Framework.Db;
using Framework.Entity;
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
   
    public partial class HomeController : Controller
    {
        private static CxSlMetadataHolder mHolder;
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UnauthorizedResult()
        {
            return View();
        }

        [Authorize]
        public  ActionResult  GetStartObject(
            int? workspaceId,
            IEnumerable<string> requiredTemplates)
        {
            if(mHolder == null)
                mHolder = (CxSlMetadataHolder)HttpContext.Application[CxAppServerConsts.METADATA_APP_KEY];
            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            try
            {

                CxAppServerContext context = new CxAppServerContext();
                if (context.UserId == null)
                {
                    return View("UnauthorizedResult");
                }
                    

                using (CxDbConnection conn = CxDbConnections.CreateEntityConnection())
                {
                    if (mHolder.Multilanguage == null)
                    {
                        mHolder.InitMultilanguage(conn, GetUserLanguage(conn));
                    }
                    else
                    {
                        mHolder.Multilanguage.LanguageCode = GetUserLanguage(conn);
                    }
                }



                
               
                if (workspaceId != null)
                {
                    (new CxAppServerContext()).CurrentWorkspaceId = ((int)workspaceId);
                }

                List<CxClientRowSource> staticRowSourses = new List<CxClientRowSource>();

                IEnumerable<CxRowSourceMetadata> staticRsMetadataList =
                  from CxRowSourceMetadata r in mHolder.RowSources.RowSources.Values
                  where string.IsNullOrEmpty(r.EntityUsageId)
                  select r;

                foreach (CxRowSourceMetadata rsMetadata in staticRsMetadataList)
                {
                    List<CxClientRowSourceItem> rsItems =
                      (from i in rsMetadata.GetList(null, null, null, null, false)
                       select new CxClientRowSourceItem(i.Description, i.Value, i.ImageReference)).ToList();

                    CxClientRowSource clientRowSource = new CxClientRowSource
                    {
                        RowSourceId = rsMetadata.Id.ToUpper(),
                        RowSourceData = rsItems
                    };
                    staticRowSourses.Add(clientRowSource);
                }

                Framework.Remote.Mobile.CxClientPortalMetadata portalMetadata = new Framework.Remote.Mobile.CxClientPortalMetadata(
                  mHolder.SlSections,
                  staticRowSourses,
                  null,
                  null,
                  mHolder.SlFrames,
                  mHolder.Images,
                  mHolder.Constraints, 
                  mHolder);

                if (!mHolder.IsDevelopmentMode)
                {
                    CxClientSectionMetadata developerSection =
                      portalMetadata.Sections.FirstOrDefault(devSect =>
                        string.Compare(devSect.Id, "Development", true) == 0);

                    if (developerSection != null)
                        portalMetadata.Sections.Remove(developerSection);
                }

                portalMetadata.ApplicationValues = new Dictionary<string, object>();
                InitApplicationValues(portalMetadata.ApplicationValues);
                portalMetadata.MultilanguageItems = GetClientMultilanguage(mHolder);

                portalMetadata.Languages = GetLanguages();
                try
                {
                    //portalMetadata.Skins = GetSkins();
                }
                catch (Exception ex)
                {
                    CxLogger.SafeWrite(ex.ToString());
                }

                object userFullName = null;
                using (CxDbConnection conn = CxDbConnections.CreateEntityConnection())
                {
                    userFullName = conn.ExecuteScalar(@"select FullName from Framework_Users where UserId = " + context.UserId);
                }

                string clientUserName = context.UserName;
                if (userFullName is string && !string.IsNullOrWhiteSpace( Convert.ToString(userFullName) ) )
                {
                    clientUserName = Convert.ToString(userFullName);
                }

                TemplateProvider tp = new TemplateProvider();
                var result = new
                {
                    Templates = GetTemplates(requiredTemplates),
                    UserName = clientUserName,
                    Token = tp.GetTemplate("AntiForgeryToken", null, this, true),
                    PortalMetadata = portalMetadata,
                    Settings = GetSettings("app", null),
                };




























                
            

                
             
                    
                    //TemplateProvider prov = new TemplateProvider();
                    //result.Templates.Add(new { Id = HtmlTemplateIds.SectionsTemplate, Template = prov.GetTemplate(HtmlTemplateIds.SectionsTemplate, Url.SkinFolder(), this) });
                    //result.Templates.Add(new { Id = HtmlTemplateIds.TreeViewTemplate, Template = prov.GetTemplate(HtmlTemplateIds.TreeViewTemplate, Url.SkinFolder(), this) });
                    //result.Templates.Add(new { Id = HtmlTemplateIds.WorkspacesPnlTemplate, Template = prov.GetTemplate(HtmlTemplateIds.WorkspacesPnlTemplate, Url.SkinFolder(), this) });
                


                json.Data = result;
                return json;
            }
            catch (Exception ex)
            {
                CxLogger.SafeWrite(ex.ToString());
                var result = new { Error = ex.Message };

                json.Data = result;
                return json;
            }
        }



        [Authorize]
        public ActionResult Ping()
        {
            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            try
            {
                json.Data = "ok";
                return json;
            }
            catch (Exception ex)
            {
                CxLogger.SafeWrite(ex.ToString());
                var result = new { Error = ex.Message };

                json.Data = result;
                return json;
            }
        }



        //---------------------------------------------------------------------------
        /// <summary>
        /// Returns list of all languages.
        /// </summary>
        /// <returns>List of all languages</returns>
        public List<CxLanguage> GetLanguages()
        {
            List<CxLanguage> langs = new List<CxLanguage>();
            using (CxDbConnection conn = CxDbConnections.CreateEntityConnection())
            {
                string userLang = GetUserLanguage(conn);

                DataTable langsTbl = new DataTable();
                IDataReader reader = conn.ExecuteReader(@"select * from Framework_Languages");
                while (reader.Read())
                {
                    CxLanguage lang = new CxLanguage(
                        reader["LanguageCd"].ToString(),
                        reader["Name"].ToString());
                    if (lang.LanguageCd == userLang)
                        lang.IsSelected = true;
                    langs.Add(lang);
                }

            }
            return langs;
        }

        //----------------------------------------------------------------------------
        /// <summary>
        /// Adds 'APPLICATION$...' values into given dictionary.
        /// </summary>
        /// <param name="toInit">Dictionary to initialize.</param>
        private void InitApplicationValues(IDictionary<string, object> toInit)
        {
            CxAppServerContext serverContext = new CxAppServerContext();
            //toInit.Add("APPLICATION$USERID", serverContext["APPLICATION$USERID"]);
            toInit.Add("APPLICATION$USERNAME", serverContext["APPLICATION$USERNAME"]);
            //toInit.Add("APPLICATION$USEREMAIL", serverContext["APPLICATION$USEREMAIL"]);
            //toInit.Add("APPLICATION$APPLICATIONCODE", serverContext["APPLICATION$APPLICATIONCODE"]);
            //toInit.Add("APPLICATION$FRAMEWORKUSERID", serverContext["APPLICATION$FRAMEWORKUSERID"]);
            toInit.Add("APPLICATION$CURRENTWORKSPACEID", serverContext["APPLICATION$CURRENTWORKSPACEID"]);
            toInit.Add("APPLICATION$LANGUAGECODE", Convert.ToString( serverContext["APPLICATION$LANGUAGECODE"]).ToLower());
            //toInit.Add("APPLICATION$LOCALIZATIONAPPLICATIONCODE", serverContext["APPLICATION$LOCALIZATIONAPPLICATIONCODE"]);
            //toInit.Add("APPLICATION$WORKSPACEAVAILABLEFORUSERTABLE_XML", serverContext["APPLICATION$WORKSPACEAVAILABLEFORUSERTABLE_XML"]);


            DataTable workspacesTbl = serverContext.AvailableWorkspacesTable;
            List<object> workspacesList = new List<object>();
            foreach (DataRow row in workspacesTbl.Rows)
            {
                var ws = new { WorkspaceId = row["WorkspaceId"], Name = row["Name"], Code = row["Code"], Priority = row["Priority"], DefaultOrder = row["DefaultOrder"], };
                workspacesList.Add(ws);
            }

            toInit.Add("APPLICATION$WORKSPACEAVAILABLE", workspacesList);


            toInit.Add("APPLICATION$CLIENTDATEFORMAT", ConfigurationManager.AppSettings["ClientDateFormat"]);
            toInit.Add("APPLICATION$CLIENTDATETIMEFORMAT", ConfigurationManager.AppSettings["ClientDateTimeFormat"]);


            int maxUploadSize;
            bool convertResult =
              Int32.TryParse(ConfigurationManager.AppSettings["MaxUploadFileSize"], out maxUploadSize);
            if (!convertResult)
                maxUploadSize = int.MaxValue;
            toInit.Add("APPLICATION$APPLICATION$MAXUPLOADSIZE", maxUploadSize);

            int packetSize;
            convertResult =
              Int32.TryParse(ConfigurationManager.AppSettings["UploadPacketSize"], out packetSize);
            if (!convertResult)
                packetSize = 125000;
            toInit.Add("APPLICATION$UploadPacketSize", packetSize);
        }

        

        private string GetUserLanguage(CxDbConnection conn)
        {
            CxAppServerContext context = new CxAppServerContext();
            if(mHolder.SlSections.JsAppProperties.Count == 0)
                return "EN";
            if (context.UserId != null)
            {
                object userLangObj = conn.ExecuteScalar(@"select [Value] from Framework_UserSettings
                                   where UserId= " + context.UserId + @" and 
                                    ApplicationCd = '" + mHolder.SlSections.JsAppProperties["app_name"] + @"' and 
                                 OptionKey = 'UserLang' ");

                if (userLangObj != null && !(userLangObj is DBNull) && userLangObj is string)
                    return userLangObj.ToString();

                string fromSettings = ConfigurationManager.AppSettings["localizationLanguageCode"];
                if (!string.IsNullOrEmpty(fromSettings))
                    return fromSettings;
            }
            return "EN";
        }

        //---------------------------------------------------------------------------
        /// <summary>
        /// Returns list of all skins.
        /// </summary>
        /// <returns>List of all skins</returns>
        //public List<CxSkin> GetSkins()
        //{
        //    string selectedSkinId;
        //    using (CxDbConnection conn = CxDbConnections.CreateEntityConnection())
        //    {
        //        selectedSkinId = GetSelectedSkin(conn);
        //    }
        //    List<CxSkin> skins = new List<CxSkin>();
        //    foreach (CxSlSkinMetadata skinMetadata in mHolder.SlSkins.Items)
        //    {
        //        CxSkin skin = new CxSkin(
        //          skinMetadata.Id,
        //          skinMetadata.Text,
        //          null,
        //          string.Compare(skinMetadata.Id, selectedSkinId, true) == 0);

        //        if (skin.IsSelected)
        //        {
        //            CxSkin skinWithData = GetSkin(skinMetadata.Id);
        //            if (skinWithData.Error != null)
        //                throw new ExException(skinWithData.Error.Message);
        //            skin.SkinData = skinWithData.SkinData;
        //        }
        //        skins.Add(skin);
        //    }
        //    return skins;
        //}

        //---------------------------------------------------------------------------
        private string GetSelectedSkin(CxDbConnection conn)
        {
            CxAppServerContext context = new CxAppServerContext();
            object selSkinObj = conn.ExecuteScalar(@"select [Value] from Framework_UserSettings
                                   where UserId= " + context.UserId + @" and 
                                    ApplicationCd = '" + mHolder.ApplicationCode + @"' and 
                                 OptionKey = 'SkinId' ");

            if (selSkinObj != null && !(selSkinObj is DBNull) && selSkinObj is string)
                return selSkinObj.ToString();

            var defaultSkin = mHolder.SlSkins.AllItems.FirstOrDefault(s => s.IsDefault);
            if (defaultSkin != null)
                return defaultSkin.Id;

            return null;
        }

        private static string ServerDateFormat = ConfigurationManager.AppSettings["ServerDateFormat"];
        private static string ServerDateTimeFormat = ConfigurationManager.AppSettings["ServerDateTimeFormat"];

        private void FixJsFilterItems(List<CxFilterItem> items, CxEntityUsageMetadata usage)
        {
            foreach (CxFilterItem item in items)
            {
                var attr = usage.GetAttribute( item.Name );
                if (attr.Type == "date" || attr.Type == "datetime")
                {
                    string jsVal = Convert.ToString(item.Values[0]);
                    if (string.IsNullOrWhiteSpace(jsVal) == false)
                    {
                        item.Values[0] = ParseJsDate(jsVal, attr.Type);
                    }
                    jsVal = Convert.ToString(item.Values[1]);
                    if (string.IsNullOrWhiteSpace(jsVal) == false)
                    {
                        item.Values[1] = ParseJsDate(jsVal, attr.Type);
                    }
                }

                if( !string.IsNullOrEmpty( item.OperationAsString ))
                    item.Operation = (NxFilterOperation)Enum.Parse(typeof(NxFilterOperation), item.OperationAsString);
            }
        }

        public static void FixJsDataItems(Dictionary<string, object> items, CxEntityUsageMetadata usage)
        {
            Dictionary<string, object> fixedItems = new Dictionary<string, object>();
            foreach (var item in items)
            {
                var attr = usage.GetAttribute(item.Key);
                if (attr.Type == "date" || attr.Type == "datetime")
                {
                    string jsVal = Convert.ToString(item.Value);
                    if (string.IsNullOrWhiteSpace(jsVal) == false)
                    {
                        fixedItems.Add(item.Key, ParseJsDate(jsVal, attr.Type));
                    }
                }
                if (!string.IsNullOrEmpty( attr.RowSourceId) && attr.Nullable == true )
                {
                    int val = CxInt.Parse(item.Value, 0);
                    if(val == int.MinValue)
                        fixedItems.Add(item.Key, null);
                }
            }

            foreach (var item in fixedItems)
            {
                items[item.Key] = item.Value;
            }
        }

        public static DateTime ParseJsDate(string jsDate, string type)
        {
            if (type == "date")
            {
                //TODO: do some with it silly date parse
                try
                {
                    return DateTime.ParseExact(jsDate, ServerDateFormat, null);
                }
                catch{}
                
                return DateTime.Now;
            }
            if (type == "datetime")
            {
                return DateTime.ParseExact(jsDate, ServerDateTimeFormat, null);
            }
            throw new ArgumentException();
        }

        private void ParseQueryParams(
                out Dictionary<string, object> primaryKeysValues,
                out Dictionary<string, object> whereValues,
                out Dictionary<string, object> joinValues,
                out List<CxFilterItem> filterItems,
                out List<CxSortDescription> sortDescriptions,
                out Dictionary<string, object> parentPks,
                out Dictionary<string, object> currentEntity,
                out List<Dictionary<string, object>> selectedEntities,
                string pkVals,
                string whereVals,
                string joins,
                string filters,
                string sorts,
                string parentPrimaryKeys,
                string  currentEnt,
                string selectedEnts,
                CxEntityUsageMetadata usage,
                CxEntityUsageMetadata parentUsage
            )
        {
            
            primaryKeysValues = new Dictionary<string, object>();
            if (!string.IsNullOrWhiteSpace(pkVals))
            {
                primaryKeysValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(pkVals);
                FixJsDataItems(primaryKeysValues, usage);
            }

            whereValues = new Dictionary<string, object>();
            if (!string.IsNullOrWhiteSpace(whereVals))
            {
                whereValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(whereVals);
                FixJsDataItems(whereValues, usage);
            }

            joinValues = new Dictionary<string, object>();
            if (!string.IsNullOrWhiteSpace(joins))
            {
                joinValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(joins);
                FixJsDataItems(joinValues, usage);
            }

            filterItems = new List<CxFilterItem>();
            if (!string.IsNullOrWhiteSpace(filters))
            {
                filterItems = JsonConvert.DeserializeObject<List<CxFilterItem>>(filters);
                FixJsFilterItems(filterItems, usage);
            }
            sortDescriptions = new List<CxSortDescription>();
            if (!string.IsNullOrWhiteSpace(sorts))
            {
                sortDescriptions = JsonConvert.DeserializeObject<List<CxSortDescription>>(sorts);
            }
            parentPks = new Dictionary<string, object>();
            if (!string.IsNullOrWhiteSpace(parentPrimaryKeys))
            {
                parentPks = JsonConvert.DeserializeObject<Dictionary<string, object>>(parentPrimaryKeys);
                FixJsDataItems(parentPks, parentUsage);
            }

            currentEntity = new Dictionary<string, object>();
            if (!string.IsNullOrWhiteSpace(currentEnt))
            {
                currentEntity = JsonConvert.DeserializeObject<Dictionary<string, object>>(currentEnt);
                FixJsDataItems(currentEntity, usage);
            }

            selectedEntities = new List<Dictionary<string, object>>();
            if (!string.IsNullOrWhiteSpace(selectedEnts))
            {
                selectedEntities = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(selectedEnts);
                foreach (var ent in selectedEntities)
                {
                    FixJsDataItems(ent, usage);
                }
            }

        }


        private void SetPersistentCookie(string userId, bool isPersistent)
        {
            FormsAuthenticationTicket ticket;
            if (isPersistent)
            {
                ticket = new FormsAuthenticationTicket(1, userId,
                                                  DateTime.Now, DateTime.Now.AddMonths(3),
                                                  true, userId,
                                                  Request.ApplicationPath.ToLower());
            }
            else
            {
                ticket = new FormsAuthenticationTicket(1, userId,
                                                  DateTime.Now, DateTime.Now.AddHours(2),
                                                  false, userId,
                                                  Request.ApplicationPath.ToLower());
            }

            string ticketEncoded = FormsAuthentication.Encrypt(ticket);
            HttpCookie c = new HttpCookie(FormsAuthentication.FormsCookieName, ticketEncoded);
            if (isPersistent)
                c.Expires = DateTime.Now.AddMonths(3);
            c.Path = Request.ApplicationPath;
            Response.Cookies.Add(c);
        }
    }
}