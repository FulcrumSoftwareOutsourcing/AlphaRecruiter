using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Server.Models.Settings
{
    public class AppSettings
    {
        public AppSettings()
        {
            Items = new Dictionary<string, SettingsItem>();
        }
        public int? Id { get; set; }
        public Dictionary<string, SettingsItem> Items { get; set; }
    }
}
