using Framework.Db;
using Framework.Remote;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.Caching;

namespace App.Server.Models.Settings
{
    public class AppSettingsProvider
    {
        private const string SETTINGS_CACHE_KEY = "app_settings_";
        private AppSettings settings;
        private CxSlMetadataHolder metadata;
        CxAppServerContext context = new CxAppServerContext();
        
        public AppSettingsProvider()
        {
            metadata = (CxSlMetadataHolder)HttpContext.Current.Application[CxAppServerConsts.METADATA_APP_KEY];
            
            settings = GetFromCache();
            if (settings == null)
            {
                settings = GetFromRepository();
            }
        }

        private AppSettings GetFromCache()
        {
            return HttpContext.Current.Cache[SETTINGS_CACHE_KEY + context.UserId + metadata.ApplicationCode] as AppSettings;
        }

        private AppSettings GetFromRepository()
        {
            ISettingsRepository repository = new SettingsRepositorySqlSrv(); //TODO: create depends from used DB
            settings = repository.GetSettings((int)context.UserId, metadata.ApplicationCode);
            HttpContext.Current.Cache.Add(
                            SETTINGS_CACHE_KEY + context.UserId + metadata.ApplicationCode,
                            settings,
                            null,
                            System.Web.Caching.Cache.NoAbsoluteExpiration,
                            TimeSpan.FromMinutes(10),
                            System.Web.Caching.CacheItemPriority.Normal,
                            SettingsCacheItemRemoved);

            return settings;
        }

        private static void SaveSettings(AppSettings settings)
        {
            ISettingsRepository repository = new SettingsRepositorySqlSrv(); //TODO: create the repository depend from used DB
              CxSlMetadataHolder metadata = (CxSlMetadataHolder)HttpContext.Current.Application[CxAppServerConsts.METADATA_APP_KEY]; 
            CxAppServerContext context = new CxAppServerContext();
            repository.SaveSettings(settings, (int)context.UserId, metadata.ApplicationCode);
        }
        
        private static void SettingsCacheItemRemoved(string key, object value, CacheItemRemovedReason reason)
        {
            AppSettingsProvider.SaveSettings((AppSettings)value);
        }

        public SettingsItem GetSettings(string keyName)
        {
            return FindRecursive(settings.Items, keyName);
        }

        public IEnumerable<SettingsItem> GetSettings(IEnumerable<string> keys)
        {
            List<SettingsItem> result = new List<SettingsItem>();
            foreach (string key in keys)
            {
                SettingsItem found = FindRecursive(settings.Items, key);
                if (found != null)
                {
                    result.Add(found);
                }
            }
            return result;
        }

        public void SaveSettings(IDictionary<string, SettingsItem> items, bool saveImmediately = false)
        {
            List<KeyValuePair<string, SettingsItem>> toAdd = new List<KeyValuePair<string, SettingsItem>>();
            foreach (var i in items)
            {
                SettingsItem found = FindRecursive(settings.Items, i.Key);
                if (found == null)
                {
                    toAdd.Add(i);
                    continue;
                }
                else
                {
                    found.Value = i.Value.Value;
                    UpdateRecursive(found.Items, i.Value.Items);
                }
            }

            foreach (var i in toAdd)
            {
                settings.Items.Add(i.Key, i.Value);
            }
          
        }

        private void UpdateRecursive(IDictionary<string, SettingsItem> existing, IDictionary<string, SettingsItem> @new)
        {
            //foreach(var i in @new)
            //{
            //    if()
            //}
            //if (existing.ContainsKey(keyName))
            //{
            //    return source[keyName];
            //}
            //else
            //{
            //    foreach (var i in source)
            //    {
            //        return FindRecursive(i.Value.Items, keyName);
            //    }
            //}
        }

        public void SaveSettings( IEnumerable<SettingsItem> items, bool saveImmediately = false)
        {

        }

        private SettingsItem FindRecursive(IDictionary<string, SettingsItem> source, string keyName)
        {
            if(source.ContainsKey(keyName))
            {
                return source[keyName];
            }
            else
            {
                foreach (var i in source)
                {
                    return FindRecursive(i.Value.Items, keyName);
                }
            }
                
            return null;
            
        }
    }
}