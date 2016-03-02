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
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Framework.Utils
{
  /// <summary>
  /// Class to import WinAPI functions to work with INI files.
  /// </summary>
  public class CxIniFile
  {
    //----------------------------------------------------------------------------
    static public string GENERAL_INI_SECTION = "General";
    //----------------------------------------------------------------------------
    [DllImport("kernel32.dll")]
    static private extern bool WritePrivateProfileSection(string appName, string data, string fileName);
    [DllImport("kernel32.dll", EntryPoint="GetPrivateProfileString")]
    static private extern int GetPrivateProfileStringInternal(string appName, string keyName, string defaultValue, byte[] buffer, int size, string fileName);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Writes string value to the INI file.
    /// </summary>
    /// <param name="appName">name of section</param>
    /// <param name="keyName">name of the key</param>
    /// <param name="value">value to write</param>
    /// <param name="fileName">name of the INI file</param>
    [DllImport("kernel32.dll")]
    static public extern int WritePrivateProfileString(string appName, string keyName, string value, string fileName);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads string value from the INI file.
    /// </summary>
    /// <param name="appName">name of the section</param>
    /// <param name="keyName">name of the key</param>
    /// <param name="defaultValue">default value</param>
    /// <param name="fileName">name of the INI file</param>
    /// <returns>value of the given key or the default one</returns>
    static public string GetPrivateProfileString(string appName, string keyName, string defaultValue, string fileName)
    {
      const int SIZE = 32000;
      byte[] buffer = new byte[SIZE];
      int len = GetPrivateProfileStringInternal(appName, keyName, defaultValue, buffer, SIZE, fileName);
      char[] chars = new char[len];
      for (int i = 0; i < len; i++) chars[i] = (char) buffer[i];
      string result = new string(chars);
      return result;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads integer value from the INI file.
    /// </summary>
    /// <param name="appName">name of the section</param>
    /// <param name="keyName">name of the key</param>
    /// <param name="defaultValue">default value</param>
    /// <param name="fileName">name of the INI file</param>
    /// <returns>value of the given key or the default one</returns>
    static public int GetPrivateProfileInt(string appName, string keyName, int defaultValue, string fileName)
    {
      const int SIZE = 32000;
      byte[] buffer = new byte[SIZE];
      int len = GetPrivateProfileStringInternal(appName, keyName, defaultValue.ToString(), buffer, SIZE, fileName);
      char[] chars = new char[len];
      for (int i = 0; i < len; i++) chars[i] = (char) buffer[i];
      string s = new string(chars);
      try
      {
        return Int32.Parse(s);
      }
      catch (Exception)
      {
        return defaultValue;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Writes integer value to the INI file.
    /// </summary>
    /// <param name="appName">name of section</param>
    /// <param name="keyName">name of the key</param>
    /// <param name="value">value to write</param>
    /// <param name="fileName">name of the INI file</param>
    static public void WritePrivateProfileInt(string appName, string keyName, int value, string fileName)
    {
      WritePrivateProfileString(appName, keyName, value.ToString(), fileName);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Deletes all data from the specified section in INI file.
    /// </summary>
    /// <param name="appName">name of section</param>
    /// <param name="fileName">name of the INI file</param>
    static public void DeletePrivateProfileSection(string appName, string fileName)
    {
      WritePrivateProfileSection(appName, "", fileName);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns name of the INI file for the application.
    /// </summary>
    /// <returns>name of the INI file for the application</returns>
    static public string GetAppIniFileName()
    {
      return System.IO.Path.ChangeExtension(Application.ExecutablePath, ".ini");
    }
    //----------------------------------------------------------------------------
  }
}
