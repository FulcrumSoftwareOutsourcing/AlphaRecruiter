using App.Server.Models.Markup;
using App.Server.Models.Settings;
using Framework.Db;
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
    public partial class HomeController
    {
        public ActionResult demo_login(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return View();


            using (CxDbConnection conn = CxDbConnections.CreateEntityConnection())
            {
                //try
                //{
                //    conn.BeginTransaction();
                //    string uniqueLogin = GetUniqueLogin(conn, userName);
                //    int userId = CreateUser(conn, uniqueLogin, userName);
                //    AssignRole(conn, userId);
                //    conn.Commit();

                //    if (userId > 0)
                //    {
                //        SetPersistentCookie(uniqueLogin, false);
                //        return RedirectToAction("Index");
                //    }


                //}
                //catch (Exception)
                //{
                //    conn.Rollback();
                //}


                CxDbParameter p1 = conn.CreateParameter ("UserName", userName, ParameterDirection.Input, DbType.String);
                CxDbParameter p2 = conn.CreateParameter("CreatedLogin", DBNull.Value, ParameterDirection.Output, DbType.String);
                CxDbParameter p3 = conn.CreateParameter("UserId", DBNull.Value, ParameterDirection.Output, DbType.Int32);
                
                conn.ExecuteCommandSP("p_CreateDemoLogin", new CxDbParameter[] { p1, p2, p3 });


                //DemoLoginDataContext db = new DemoLoginDataContext(conn.Connection);
                //string createdLogin = "";
                //int? userId = 0;
                //db.p_CreateDemoLogin(userName, ref createdLogin, ref userId);

                int userId = 0;
                Int32.TryParse(Convert.ToString(p3.Value), out userId);
                

                

                if (userId > 0)
                {
                    SetPersistentCookie((string)p2.Value, false);
                    return RedirectToAction("Index");
                }


            }




            return View();

        }

        private string GetUniqueLogin(CxDbConnection conn, string proposedLogin)
        {
            string uniqueLogin = proposedLogin;
            int tryCount = 0;
            do
            {

                object alreadyExists =
                    conn.ExecuteScalar("select top 1 [Login] from Framework_Users where [Login] = :proposedLogin",
                                       uniqueLogin);
                if (alreadyExists == null || alreadyExists is DBNull)
                    return uniqueLogin;

                tryCount++;
                uniqueLogin = string.Concat(proposedLogin, "(", tryCount, ")");
            }
            while (tryCount < 100);
            return string.Concat(proposedLogin, "_", Guid.NewGuid().ToString());

        }

        //----------------------------------------------------------------------------
        /// <summary>
        /// Creates user in DB.
        /// </summary>
        private int CreateUser(CxDbConnection conn, string uniqueLogin, string proposedLogin)
        {
            //2FeO34RYzgb7xbt2pYxcpA==   (qwerty)
            int userId = conn.GetNextId();
            conn.ExecuteCommand(@"
        INSERT INTO [Framework_Users]
           ([UserId]
           ,[Login]
           ,[FullName]
           ,[Password]
           ,[IsDeactivated] 
           ,[DefaultWorkspaceId]
          )
     VALUES
           (:UserId
           ,:UniqueLogin
           ,:ProposedLogin
           ,'2FeO34RYzgb7xbt2pYxcpA=='
           ,0
           ,1)", userId, uniqueLogin, proposedLogin);

            return userId;
        }

        //----------------------------------------------------------------------------
        /// <summary>
        /// Assigns 'Ordinary User' role for guest user.
        /// </summary>
        private void AssignRole(CxDbConnection conn, int userId)
        {
            string demoRole = ConfigurationManager.AppSettings["DemoLoginRoleName"];
            int demoRoleId = (int)conn.ExecuteScalar("select RoleId from Framework_Roles Name where Name='" + demoRole + "' ");


            conn.ExecuteCommand(@"
        INSERT INTO [Framework_UserRoles]
           ([RoleId]
           ,[UserId])
      VALUES
           (:DemoRoleId
           ,:UserId)", demoRoleId, userId);
        }
    }
}