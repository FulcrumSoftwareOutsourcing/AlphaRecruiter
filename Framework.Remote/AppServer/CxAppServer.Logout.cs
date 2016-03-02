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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Remote.Mobile;

namespace Framework.Remote
{
  public partial class CxAppServer
  {
    /// <summary>
    /// Logouts current user.
    /// </summary>
    /// <param name="marker">Server operation identifier.</param>
    /// <returns>Initialized CxModel</returns>
    public CxModel Logout(Guid marker)
    {
      try
      {
        CxModel empty = new CxModel(marker);
        CxBaseLoginPage.UserLogout();
        empty.ApplicationValues.Add("APPLICATION$USERID", null);
        return empty;
      }
      catch (Exception ex)
      {
        CxModel model = new CxModel();
        CxExceptionDetails exceptionDetails = new CxExceptionDetails(ex);
        model.Error = exceptionDetails;
        return model;
      }
    }
  }
}
