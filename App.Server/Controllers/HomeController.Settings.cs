using Framework.Db;
using Framework.Remote;
using Framework.Web.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace App.Server.Controllers
{
    public partial class HomeController
    {

        [Authorize]
        public ActionResult SaveSettings(string settingsToSave)
        {
            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //#if (!DEBUG)
            try
            {
                //#endif

                SaveSetting(settingsToSave);

                json.Data = "ok";
                return json;
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


        //----------------------------------------------------------------------------
        private void SaveSetting(string settings)
        {
            if (string.IsNullOrWhiteSpace(settings))
                return;

            CxWebUtils.HttpContextStatic = System.Web.HttpContext.Current;
            CxDbConnection conn = CxDbConnections.CreateEntityConnection();
            CxAppServerContext context = new CxAppServerContext();
            int userId = (int)context.UserId;

            var task = new Task((state) =>
            {

                Dictionary<string, Dictionary<string, object>> s = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(settings);
                if (s.Count == 0)
                    return;

                var paramsArr = (object[])state;
                CxDbConnection con = (CxDbConnection)paramsArr[0];

                using (con)
                {

                    foreach (var group in s)
                    {
                        string groupKey = group.Key;
                        int? groupId = GetGropId(groupKey, con);
                        if (groupId == null)
                        {
                            groupId = CreateSettingsGroup(groupKey, con, userId);
                        }

                        foreach (var sItem in group.Value)
                        {
                            string key = sItem.Key;
                            object value = sItem.Value;

                            int? keyId = GetKeyId(groupId.Value, key, userId, con);
                            if (keyId == null)
                            {
                                CreateKey(groupId.Value, key, userId, value, con);
                            }
                            else
                            {
                                UpdateKey(keyId.Value, value, con);
                            }
                        }
                    }

                }
            },
            new object[] { conn }
                );


            task.Start();


        }

        private void UpdateKey(int keyId, object value, CxDbConnection conn)
        {
            conn.ExecuteCommand(@"
                UPDATE Framework_UserSettings
                SET [Value] = '" + value + @"'
                WHERE UserSettingId = " + keyId);
        }

        private void CreateKey(int groupId, string key, int userId, object value, CxDbConnection conn)
        {
            int newId = conn.GetNextId();
            conn.ExecuteCommand(@"

            INSERT INTO Framework_UserSettings
                       ([UserSettingId]
                       ,[OptionKey]
                       ,[OptionType]
                       ,[ParentId]
                       ,[UserId]
                       ,[ApplicationCd]
                       ,[Value])
                 VALUES (
                       " + newId + @"
                       ,'" + key + @"'
                       ,'x'
                       ," + groupId + @"
                       ," + userId + @"
                       ,'" + Convert.ToString(mHolder.SlSections.JsAppProperties["app_name"]) + @"'
                       ,'" + value + "' )");


        }

        private int CreateSettingsGroup(string groupName, CxDbConnection conn, int userId)
        {
            int newId = conn.GetNextId();
            conn.ExecuteCommand(@"

            INSERT INTO Framework_UserSettings
                       ([UserSettingId]
                       ,[OptionKey]
                       ,[OptionType]
                       ,[ParentId]
                       ,[UserId]
                       ,[ApplicationCd]
                       ,[Value])
                 VALUES (
                       " + newId + @"
                       ,'" + groupName + @"'
                       ,'x'
                       ,NULL
                       ," + userId + @"
                       ,'" + Convert.ToString(mHolder.SlSections.JsAppProperties["app_name"]) + @"'
                       ,NULL )");

            return newId;
        }

        private int? GetGropId(string groupName, CxDbConnection conn)
        {
            var result = conn.ExecuteScalar("select top 1 UserSettingId from Framework_UserSettings where OptionKey = '" + groupName + "'");
            if (result is DBNull || result == null)
                return null;
            else
                return (int)result;


        }

        private int? GetKeyId(int groupId, string key, int userId, CxDbConnection conn)
        {
            if (!mHolder.SlSections.JsAppProperties.ContainsKey("app_name"))
            {
                return -1;
            }

            string app = Convert.ToString(mHolder.SlSections.JsAppProperties["app_name"]);

            var result = conn.ExecuteScalar(@"SELECT top 1
                                                us_child.UserSettingId
                                              FROM Framework_UserSettings us
                                                 left join  Framework_UserSettings us_child
                                                    on us_child.ParentId = us.UserSettingId

                                               where us_child.ParentId = " + groupId + " and us_child.ApplicationCd = '" + app + "' and us_child.UserId = " + userId + " and us_child.OptionKey = '" + key + "'");


            if (result is DBNull || result == null)
                return null;
            else
                return (int)result;
        }

        private List<object> GetSettings(string groupKey, string key)
        {
            List<object> result = new List<object>();
            if (mHolder.SlSections.JsAppProperties.Count == 0 || !mHolder.SlSections.JsAppProperties.ContainsKey("app_name"))
                return result;

            string app = Convert.ToString(mHolder.SlSections.JsAppProperties["app_name"]);



            using (CxDbConnection conn = CxDbConnections.CreateEntityConnection())
            {


                string individualKeyWhere = "";
                if (!string.IsNullOrWhiteSpace(key))
                {
                    individualKeyWhere = " AND us.OptionKey = '" + key + "' ";
                }

                CxAppServerContext context = new CxAppServerContext();

                var reader = conn.ExecuteReader(
                    @"
                        SELECT
                            us_child.OptionKey,
                            us_child.Value
                        FROM Framework_UserSettings us
                            left join  Framework_UserSettings us_child
                                on us_child.ParentId = us.UserSettingId

                        where us.ApplicationCd = '" + app + "' and us_child.UserId = " + context.UserId + " and us.OptionKey = '" + groupKey + "' " + individualKeyWhere
                    );



                while (reader.Read())
                {
                    result.Add(new { Group = groupKey, Key = reader[0], Value = reader[1] });
                }
                return result;

            }


        }



    }
}
