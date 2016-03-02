/********************************************************************
 *  FulcrumWeb RAD Framework - Fulcrum of your business             *
 *  Copyright (c) 2002-2009 FulcrumWeb, ALL RIGHTS RESERVED         *
 *                                                                  *
 *  THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      *
 *  FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        *
 *  COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       *
 *  AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  *
 *  AND PERMISSION FROM FULCRUMWEB. CONSULT THE END USER LICENSE    *
 *  AGREEMENT FOR INFORMATION ON ADDITIONAL RESTRICTIONS.           *
 ********************************************************************/

using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;

using Framework.Db;
using Framework.Metadata;
using Framework.Utils;
using Framework.Web.Utils;

namespace Framework.Remote
{
  /// <summary>
  /// Base class for the portal login page.
  /// </summary>
  public class CxBaseLoginPage : Page
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads controls.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {

      if (Request.QueryString["RETURNURL"] == null)
      {
        Response.Redirect("Login.aspx?ReturnURL=" + DefaultRedirectPage, true);
      }
      base.OnLoad(e);
    }


    //-------------------------------------------------------------------------
    /// <summary>
    /// Try to authenticate user.
    /// </summary>
    protected virtual void Authenticate(
        string userName,
        string password,
        bool signAuto)
    {
      try
      {
        ValidateLogin(userName, password);
        // <see cref="http://www.codeguru.com/csharp/.net/net_asp/miscellaneous/article.php/c12769/"/>
        SetPersistentCookie(userName, signAuto);
        Response.Redirect(FormsAuthentication.GetRedirectUrl(userName, false));
        //FormsAuthentication.RedirectFromLoginPage(userName, signAuto);
      }
      catch (Exception e)
      {
        throw;
        //HandleLoginError(e);
      }
    }

    //-------------------------------------------------------------------------
    // <see cref="http://www.codeguru.com/csharp/.net/net_asp/miscellaneous/article.php/c12769/"/>
    private void SetPersistentCookie(string userId, bool isPersistent)
    {
      FormsAuthenticationTicket ticket;
      if (isPersistent)
      {
        ticket = new FormsAuthenticationTicket(1, userId,
                                          DateTime.Now, DateTime.Now.AddMonths(3),
                                          true, userId,
                                          Request.ApplicationPath.ToLower());
      }
      else
      {
        ticket = new FormsAuthenticationTicket(1, userId,
                                          DateTime.Now, DateTime.Now.AddHours(2),
                                          false, userId,
                                          Request.ApplicationPath.ToLower());
      }

      string ticketEncoded = FormsAuthentication.Encrypt(ticket);
      HttpCookie c = new HttpCookie(FormsAuthentication.FormsCookieName, ticketEncoded);
      if (isPersistent)
        c.Expires = DateTime.Now.AddMonths(3);
      c.Path = Request.ApplicationPath;
      Response.Cookies.Add(c);
    }

    //-------------------------------------------------------------------------
    /// <summary>
    /// Validates entered user name and password.
    /// </summary>
    /// <param name="userName">user name</param>
    /// <param name="password">password</param>
    protected virtual void ValidateLogin(string userName, string password)
    {
      if (CxUtils.IsEmpty(userName))
        throw new ExValidationException("User name could not be empty.");
      if (CxUtils.IsEmpty(password))
        throw new ExValidationException("Password could not be empty.");
      CxMetadataHolder metadata =
          (CxSlMetadataHolder) HttpContext.Current.Application[CxAppServerConsts.METADATA_APP_KEY];
      if (metadata == null)
        throw new ExException("No metadata initialized");
      if (metadata.Security == null)
        throw new ExException("Could not validate user name and password. Security metadata object was not loaded.");

      //string connStr = (string)HttpContext.Current.Application[CxAppServerConsts.CONN_STRING_APP_KEY];
      using (CxDbConnection connection = CxDbConnections.CreateEntityConnection())
      {
        metadata.Security.ValidateLogin(connection, userName, password, true);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Handles login error.
    /// </summary>
    /// <param name="e">exception to handle</param>
    protected virtual void HandleLoginError(Exception e)
    {
      throw new Exception("Login error", e);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns default application page.
    /// </summary>
    public virtual string DefaultRedirectPage
    {
      get { return CxWebUtils.GetAbsUrl(CxAppServerConsts.DEFAULT_APP_PAGE); }
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Signs current user out.
    /// </summary>
    static public void UserLogout()
    {
      FormsAuthentication.SignOut();

      HttpCookie cookie = HttpContext.Current.Response.Cookies[FormsAuthentication.FormsCookieName];
      if (cookie != null)
      {
        cookie.Path = HttpContext.Current.Request.ApplicationPath;
        cookie.Value = "";
      }
      HttpContext.Current.Session.Abandon();
    }
  }
}
