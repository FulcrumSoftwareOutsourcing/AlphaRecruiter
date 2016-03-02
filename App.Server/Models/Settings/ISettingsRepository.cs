using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Server.Models.Settings
{
    public interface ISettingsRepository
    {
        AppSettings GetSettings(int userId, string appCode);
        void SaveSettings(AppSettings settings, int userId, string appCode);
    }
}
