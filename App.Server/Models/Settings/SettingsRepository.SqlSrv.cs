using Framework.Db;
using Framework.Remote;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;

namespace App.Server.Models.Settings
{
    public class SettingsRepositorySqlSrv : ISettingsRepository
    {
        public AppSettings GetSettings(int userId, string appCode)
        {
            DataTable settingsTbl = null;
            using (CxDbConnection conn = CxDbConnections.CreateEntityConnection())
            {
                settingsTbl = conn.GetQueryResult(@"
                    SELECT UserSettingId, [Value] from Framework_UserSettings
                    WHERE UserId=" + userId + " AND ApplicationCd = '" + appCode + "' AND OptionKey = 'JsAppSettings' ");
            }

            if (settingsTbl == null || settingsTbl.Rows.Count == 0)
            {
                return null;
            }
            else
            {
                AppSettings settings = new AppSettings();
                settings.Id = (int)settingsTbl.Rows[0][0];
                settings.Items = JsonConvert.DeserializeObject<Dictionary<string, SettingsItem>>((string)settingsTbl.Rows[0][1]);
                return settings;
                //byte[] settingsBytes = Convert.FromBase64String((string)settingsTbl.Rows[0][1]);
                //using (MemoryStream output = new MemoryStream())
                //{
                //    using (MemoryStream compressedStream = new MemoryStream(settingsBytes))
                //    {
                //        using (GZipStream zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                //        {
                //            zipStream.CopyTo(output);
                //            zipStream.Close();
                //            output.Position = 0;
                //        }
                //    }

                //    BinaryFormatter formatter = new BinaryFormatter();
                //    settings.Items = (Dictionary<string, SettingsItem>)formatter.Deserialize(output);
                  //  return settings;
                //}
            }

        }

        public void SaveSettings( AppSettings settings, int userId, string appCode)
        {
            //byte[] settingsBytes;
            //using (MemoryStream input = new MemoryStream())
            //{
            //    BinaryFormatter formatter = new BinaryFormatter();
            //    formatter.Serialize(input, settings.Items);
            //    using (MemoryStream compressed = new MemoryStream())
            //    {
            //        using (var zipStream = new GZipStream(input, CompressionMode.Compress))
            //        {
            //            input.CopyTo(compressed);
            //            zipStream.Close();
            //            compressed.Position = 0;
            //            settingsBytes = compressed.ToArray();
            //        }
            //    }
            //}

            string settingsString = JsonConvert.SerializeObject(settings.Items); //Convert.ToBase64String(settingsBytes);

            using (CxDbConnection conn = CxDbConnections.CreateEntityConnection())
            {
                if (settings.Id != null)
                {
                    conn.ExecuteScalar(
                        "UPDATE Framework_UserSettings" +
                            " [Value] = '" + settingsString + "' " +
                        " WHERE UserSettingId = " + settings.Id);
                }
                else
                {
                    settings.Id = conn.GetNextId();
                    conn.ExecuteScalar(
                        "INSERT INTO Framework_UserSettings" +
                         " ( UserSettingId, " +
                            "OptionKey, " +
                            "OptionType, " +
                            "UserId, " +
                            "ApplicationCd, " +
                            "[Value]) " +
                        "VALUES " +
                            "(" + settings.Id + ", " +
                            "'JsAppSettings', " +
                            "'JsOption', " +
                            userId + ", " +
                            "'" + appCode + ", " +
                            "'" + settingsString + " )");
                }

            }
        }
    }
}