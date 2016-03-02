using App.Metadata;
using Framework.Entity;
using Framework.Remote;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace App.Server
{
    public class MvcApplication : System.Web.HttpApplication
    {
       // private string rootPath;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            HttpContext.Current.Application[CxAppServerConsts.METADATA_APP_KEY] = new CxSlMetadata();
            CxBaseEntity.IsRefreshAfterInsertOrUpdatePerformed = false;

            ModelBinders.Binders.Add(typeof(System.Web.Mvc.Exstensions.JsonDictionary), new System.Web.Mvc.Exstensions.JsonDictionaryModelBinder());

            //try
            //{
            //    rootPath = Server.MapPath("Logs");
            //}
            //catch (Exception ex)
            //{
            //    StreamWriter wr = new StreamWriter(Path.Combine(rootPath, "Log.txt"), true);
            //    wr.Write(ex);
            //    wr.Close();
            //}
        }
    }
}
