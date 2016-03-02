using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace App.Server.Models.Markup
{
    public class TemplateProvider
    {
        //private CxSlMetadataHolder m_Holder;

        public TemplateProvider()
        {
            //m_Holder = holder;
        }


        protected const string HtmlCachePrefix = "html_";
        protected const string HtmlDefaultCachePrefix = "html_#def#_";
        protected const string CssCachePrefix = "css_";
        protected const string CssDefaultCacheKey = "css_def";


        private string GetSkinPath(string skinId)
        {
            //  CxSlSkinMetadata skinMeta = m_Holder.SlSkins[skinId];
            //  string skinFolder = skinMeta["skin_folder"];
            return Path.Combine("~/Content/Skins", "Default" /*skinFolder*/);
        }

        public  string GetTemplate(string templateId, string skinId, Controller controller, bool noCache = false)
        {
            MarkupCacheItem template = GetTemplateFromCache(skinId, templateId);
            if (template != null)
                return template.Markup;

            string templateString = GetTemplateFromSkin(skinId, templateId);
            if (!string.IsNullOrEmpty(templateString))
            {
                templateString = CombineCssAndTemplate(templateString, skinId);
                MarkupCacheItem item = new MarkupCacheItem() { Markup = templateString };
                PutTemplateToCache(skinId, templateId, item);
                return templateString;
            }
            
            template = GetDefaultTemplateFromCache(templateId);
            if (template != null)
            {
                PutTemplateToCache(skinId, templateId, template);
                return template.Markup;
            }

            templateString = GetDefaultTemplate(templateId, controller);
            templateString = CombineCssAndTemplate(templateString, skinId);
            MarkupCacheItem itemDef = new MarkupCacheItem() { Markup = templateString };
            if (noCache == false)
            {
                PutTemplateToCache(skinId, templateId, itemDef);
                PutDefaultHtmlToCache(templateId, itemDef);
            }

           
            return templateString;

        }

        private string GetCss(string cssFile, string skinId, Controller controller, bool noCache = false)
        {
            string pathToFile = Path.Combine( HttpContext.Current.Server.MapPath( GetSkinPath(skinId) ), cssFile );

            if ( File.Exists( pathToFile ) )
                return File.ReadAllText( pathToFile );

            pathToFile = Path.Combine(HttpContext.Current.Server.MapPath(GetSkinPath("Default")), cssFile);

            if (File.Exists(pathToFile))
                return File.ReadAllText(pathToFile);

            return string.Empty;

        }


        private string CombineCssAndTemplate(string template, string skinId)
        {
            Regex r = new Regex("<!--css file:(.*?)-->");
            var found = r.Matches(template);
            string templateWithCss = template;
            foreach (Match mach in found)
            {
                string fileName = mach.Value.Replace("<!--", "").Replace("css file:", "").Replace("-->", "").TrimStart().TrimEnd();
                string css = GetCssString(fileName, skinId);
                if (!string.IsNullOrEmpty(css))
                    templateWithCss = templateWithCss.Replace(mach.Value, string.Concat("<style type=\"text/css\">", css, "</style>"));
            }
            return templateWithCss;        
        }


        private string GetCssString(string cssFileName, string skinId)
        {
            string pathToSkin = HttpContext.Current.Server.MapPath(GetSkinPath(skinId));
            string pathToCssFile = Path.Combine(pathToSkin, cssFileName);
            if (File.Exists(pathToCssFile))
                return File.ReadAllText(pathToCssFile);
            else
            {
                pathToSkin = HttpContext.Current.Server.MapPath(GetSkinPath("Default"));
                pathToCssFile = Path.Combine(pathToSkin, cssFileName);
                if (File.Exists(pathToCssFile))
                    return File.ReadAllText(pathToCssFile);
                return string.Empty;
            }
            

        }

        private string GetTemplateFromSkin(string skinId, string templateId)
        {
            string htmlFileName = Path.GetFileName(templateId + ".html");
            string pathToSkin = HttpContext.Current.Server.MapPath(GetSkinPath(skinId));
            string pathToHtmlFile = Path.Combine(pathToSkin, "html", htmlFileName);
            string result = null;
            if (File.Exists(pathToHtmlFile))
                    result = File.ReadAllText(pathToHtmlFile);
            return result;
        }

        private string GetDefaultTemplate(string templateId, Controller controller)
        {
            return controller.RenderViewToString(templateId);
        }

        private MarkupCacheItem GetTemplateFromCache(string skinId, string templateId)
        {
            return HttpContext.Current.Cache[HtmlCachePrefix + skinId + templateId] as MarkupCacheItem;
        }

        private void PutTemplateToCache(string skinId, string templateId, MarkupCacheItem item)
        {
#if (!DEBUG)
      HttpContext.Current.Cache.Insert(HtmlCachePrefix + skinId + templateId, item, null,
        Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(10));
#endif
        }

        private MarkupCacheItem GetDefaultTemplateFromCache(string templateId)
        {
            return HttpContext.Current.Cache[HtmlDefaultCachePrefix + templateId] as MarkupCacheItem;
        }

        private void PutDefaultHtmlToCache(string templateId, MarkupCacheItem item)
        {
#if (!DEBUG)
    HttpContext.Current.Cache.Insert(HtmlDefaultCachePrefix + templateId, item, null,
      Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes (10));
#endif
        }

        public string GetSkinCss(string skinId)
        {


            MarkupCacheItem css = GetSkinCssFromCache(skinId);
            if (css != null)
                return css.Markup;

            string cssStr = GetCssFromSkin(skinId);
            if (!string.IsNullOrEmpty(cssStr))
            {
                MarkupCacheItem item = new MarkupCacheItem() { Markup = cssStr };
                PutSkinCssToCache(skinId, item);
                return cssStr;
            }

            css = GetDefaultCssFromCache();
            if (css != null)
            {
                PutSkinCssToCache(skinId, css);
                return css.Markup;
            }

            cssStr = GetDefaultCss();
            MarkupCacheItem itemDef = new MarkupCacheItem() { Markup = cssStr };
            PutSkinCssToCache(skinId, itemDef);
            //    PutDefaultCssToCache(itemDef);
            return cssStr;

        }

        private string GetCssFromSkin(string skinId)
        {
            string pathToFile = HttpContext.Current.Server.MapPath(Path.Combine(GetSkinPath(skinId), "skin.css"));
            if (!File.Exists(pathToFile))
                return null;
            return File.ReadAllText(pathToFile);
        }

        private string GetDefaultCss()
        {
            string pathToFile = HttpContext.Current.Server.MapPath("~/Content/index.css");
            if (!File.Exists(pathToFile))
                return null;
            return File.ReadAllText(pathToFile);
        }

        private MarkupCacheItem GetSkinCssFromCache(string skinId)
        {
            return HttpContext.Current.Cache[CssCachePrefix + skinId] as MarkupCacheItem;
        }

        private void PutSkinCssToCache(string skinId, MarkupCacheItem item)
        {
#if (!DEBUG)
      HttpContext.Current.Cache.Insert(CssCachePrefix + skinId, item, null,
        Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(5));
#endif
        }

        private MarkupCacheItem GetDefaultCssFromCache()
        {
            return HttpContext.Current.Cache[CssDefaultCacheKey] as MarkupCacheItem;
        }

        //        private void PutDefaultCssToCache(MarkupCacheItem item)
        //        {
        //#if (!DEBUG)
        //    HttpContext.Current.Cache.Insert(DefaultCssCacheKey, css, null,
        //      Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes (5));
        //#endif
        //        }

     


    }

    public class MarkupCacheItem
    {
        public string Markup { get; set; }
    }

   
}