using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace App.Server.Models.Settings
{
    
    public class SettingsItem
    {
        public SettingsItem()
        {
            Items = new Dictionary<string, SettingsItem>();
        }

        public object Value { get; set; }

        public Dictionary<string, SettingsItem> Items { get; set; }
    }
}