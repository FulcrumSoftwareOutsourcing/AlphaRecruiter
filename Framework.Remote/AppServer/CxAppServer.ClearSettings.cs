using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Db;
using Framework.Utils;

namespace Framework.Remote
{
  public partial class CxAppServer
  {
    public void ClearSettingsSafe()
    {
      try
      {
        ClearSettings();
      }
      catch (Exception ex)
      {
        CxLogger.SafeWrite(ex.ToString());
      }
    }

    public void ClearSettings()
    {
      using (CxDbConnection connection = CxDbConnections.CreateEntityConnection())
      {
        CxAppServerContext context = new CxAppServerContext();
        string sql = "delete from Framework_usersettings where userId = " + context.UserId;
        connection.ExecuteCommand(sql);
      }
    }
  }
}
