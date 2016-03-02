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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace Framework.Utils
{
  /// <summary>
  /// Protection utility methods for the .NET framework 2.0
  /// </summary>
  public class CxProtection
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Encrypts string using .NET 2.0 data protection methods.
    /// </summary>
    /// <param name="s">string to protect</param>
    /// <param name="scope">scope (current user or local machine)</param>
    /// <returns>encrypted string</returns>
    static public string ProtectString(string s, DataProtectionScope scope)
    {
      string encryptedString = "";
      if (CxUtils.NotEmpty(s))
      {
        MemoryStream stream = new MemoryStream();
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, s);
        byte[] decryptedData = stream.ToArray();
        byte[] encryptedData = ProtectedData.Protect(decryptedData, null, scope);
        encryptedString = Convert.ToBase64String(encryptedData);
      }
      return encryptedString;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Decrypts string using .NET 2.0 data protection methods.
    /// </summary>
    /// <param name="s">encrypted (protected) string</param>
    /// <param name="scope">scope (current user or local machine)</param>
    /// <returns>decrypted string</returns>
    static public string UnprotectString(string s, DataProtectionScope scope)
    {
      string decryptedString = "";
      if (CxUtils.NotEmpty(s))
      {
        byte[] encryptedData = Convert.FromBase64String(s);
        byte[] decryptedData = ProtectedData.Unprotect(encryptedData, null, scope);
        MemoryStream stream = new MemoryStream(decryptedData);
        BinaryFormatter formatter = new BinaryFormatter();
        decryptedString = (string) formatter.Deserialize(stream);
      }
      return decryptedString;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Decrypts string using .NET 2.0 data protection methods (without exceptions).
    /// </summary>
    /// <param name="s">encrypted (protected) string</param>
    /// <param name="scope">scope (current user or local machine)</param>
    /// <returns>decrypted string</returns>
    static public string SafeUnprotectString(string s, DataProtectionScope scope)
    {
      try
      {
        return UnprotectString(s, scope);
      }
      catch
      {
        return "";
      }
    }
    //-------------------------------------------------------------------------
  }
}
