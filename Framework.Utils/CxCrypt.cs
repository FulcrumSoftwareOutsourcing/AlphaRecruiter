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
using System.Security.Cryptography;
using System.Text;

namespace Framework.Utils
{
  public class CxCrypt
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Calculates hash value from the given string.
    /// </summary>
    /// <param name="data">string to get hash from</param>
    /// <returns>hash code</returns>
    static public string GetMD5Hash(string data)
    {
      return CxUtils.NotEmpty(data) ? Convert.ToBase64String(
        new MD5CryptoServiceProvider().ComputeHash(CxText.ToByteArray(data))) : null;
    }
    //-------------------------------------------------------------------------
  }
}
