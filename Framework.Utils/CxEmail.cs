/********************************************************************
 *  FulcrumWeb RAD Framework - Fulcrum of your business             *
 *  Copyright (c) 2002-2010 FulcrumWeb, ALL RIGHTS RESERVED         *
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
using System.Text;

namespace Framework.Utils
{
  /// <summary>
  /// Utility methods to work with email.
  /// </summary>
  public class CxEmail
  {
    //--------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given string looks like valid email address.
    /// </summary>
    /// <param name="email">email to validate</param>
    /// <returns>true if email format looks valid</returns>
    static public bool IsValid(string email)
    {
      return CxText.NotEmpty(email) ?
        CxText.RegexValidate(email, "\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*") : 
        false;
    }
    //--------------------------------------------------------------------------
  }
}
