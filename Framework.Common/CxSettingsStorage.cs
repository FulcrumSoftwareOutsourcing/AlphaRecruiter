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
using System.Collections.Specialized;
using Microsoft.Win32;
using System.Collections.Generic;

namespace Framework.Utils
{
  //---------------------------------------------------------------------------
  /// <summary>
	/// Class to store settings.
	/// </summary>
	public class CxSettingsStorage : IDisposable
	{
    //-------------------------------------------------------------------------
    protected const bool READ  = true;
    protected const bool WRITE = false;
    //-------------------------------------------------------------------------
    protected RegistryKey m_RootKey = null; // Root gegistry key
    protected RegistryKey m_RegistryKey = null; // Registry key for settings
    protected List<string> m_RegistryPath = new List<string>(); // List of registry key names
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    protected CxSettingsStorage()
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="rootKey">root registry key</param>
    /// <param name="registryPath">registry path to settings</param>
    public CxSettingsStorage(RegistryKey rootKey, string registryPath)
    {
      m_RootKey = rootKey;
      m_RegistryPath.AddRange(ParseRegistryPath(registryPath));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates and returns settings storage object initialized for
    /// CURRENT_USER registry key, company name, application name and version.
    /// </summary>
    static public CxSettingsStorage CreateCurrentUser()
    {
      string registryPath = CxCommon.REG_SOFTWARE + "\\" + CxAppInfo.CompanyName + "\\" + CxAppInfo.FrontendCodeAndVersion;
      
      return new CxSettingsStorage(
        Registry.CurrentUser,
        registryPath);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates and returns settings storage object initialized for
    /// LOCAL_MACHINE registry key, company name, application name and version.
    /// </summary>
    static public CxSettingsStorage CreateLocalMachine()
    {
      return new CxSettingsStorage(
        Registry.LocalMachine,
        CxCommon.REG_SOFTWARE + "\\" + CxAppInfo.CompanyName + "\\" + CxAppInfo.FrontendCodeAndVersion);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Frees all taken resources.
    /// </summary>
    virtual public void Dispose()
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Reads setting from storage.
    /// </summary>
    /// <param name="sectionName">name of the section</param>
    /// <param name="keyName">name of the key</param>
    /// <param name="defValue">default value to return if not found</param>
    /// <returns>found setting of default value</returns>
    virtual public string Read(string sectionName, string keyName, string defValue)
    {
      return ReadRegistry(sectionName, keyName, defValue);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Writes setting to storage.
    /// </summary>
    /// <param name="sectionName">name of the section</param>
    /// <param name="keyName">name of the key</param>
    /// <param name="value">value to write</param>
    virtual public void Write(string sectionName, string keyName, string value)
    {
      WriteRegistry(sectionName, keyName, value);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of setting names from the section.
    /// </summary>
    /// <param name="sectionName">section name</param>
    /// <returns>list of setting names from the section</returns>
    virtual public IList<string> GetNames(string sectionName)
    {
      return GetRegistryNames(sectionName);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of folder names from the section.
    /// </summary>
    /// <param name="sectionName">section name</param>
    /// <returns>list of folder names from the section</returns>
    virtual public IList<string> GetFolderNames(string sectionName)
    {
      return GetRegistryFolderNames(sectionName);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Deletes section of settings.
    /// </summary>
    /// <param name="sectionName">name of section to delete</param>
    virtual public void Delete(string sectionName)
    {
      DeleteRegistryKey(sectionName);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Writes value to each user data present in the storage.
    /// </summary>
    /// <param name="sectionName">section name</param>
    /// <param name="keyName">key name</param>
    /// <param name="value">value to write</param>
    virtual public void WriteForEachUser(string sectionName, string keyName, string value)
    {
      throw new NotImplementedException();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Deletes section for each user whose data present in the storage.
    /// </summary>
    /// <param name="sectionName">section name</param>
    virtual public void DeleteForEachUser(string sectionName)
    {
      throw new NotImplementedException();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Reads settings section.
    /// </summary>
    /// <param name="sectionName">name of the section</param>
    /// <param name="layout">map to store settings in</param>
    public bool Read(string sectionName, NameValueCollection layout)
    {
      if (layout == null)
      {
        return false;
      }
      IList<string> names = GetNames(sectionName);
      if (names == null)
      {
        return false;
      }
      foreach (string name in names)
      {
        layout[name] = Read(sectionName, name, CxUtils.Nvl(layout[name], ""));
      }
      return names.Count > 0;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Writes settings section.
    /// </summary>
    /// <param name="sectionName">name of the section</param>
    /// <param name="layout">map to get settings from</param>
    public void Write(string sectionName, NameValueCollection layout)
    {
      if (layout == null)
      {
        return;
      }
      foreach (string name in layout)
      {
        Write(sectionName, name, layout[name]);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Opens registry key.
    /// </summary>
    /// <param name="sectionName">name of the section</param>
    /// <param name="mode">true if key should be open in readonly mode</param>
    /// <returns>open key</returns>
    protected RegistryKey InternalOpenRegistryKey(string sectionName, bool mode)
    {
      RegistryKey key = m_RootKey;
      List<string> path = new List<string>(m_RegistryPath);
      path.AddRange(ParseRegistryPath(sectionName));
      foreach (string keyName in path)
      {
        if (key == null)
        {
          break;
        }
        if (mode == READ)
        {
          key = key.OpenSubKey(keyName);
        }
        else
        {
          key = key.CreateSubKey(keyName);
        }
      }
      return key;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Opens registry key.
    /// </summary>
    /// <param name="sectionName">name of the section</param>
    /// <param name="mode">true if key should be open in readonly mode</param>
    /// <returns>open key</returns>
    protected void OpenRegistryKey(string sectionName, bool mode)
    {
      m_RegistryKey = InternalOpenRegistryKey(sectionName, mode);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Closes registry key.
    /// </summary>
    protected void CloseRegistryKey()
    {
      if (m_RegistryKey != null)
      {
        m_RegistryKey.Close();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Reads setting from registry.
    /// </summary>
    /// <param name="sectionName">section name</param>
    /// <param name="keyName">settings key name</param>
    /// <param name="defValue">setting default value</param>
    /// <returns>read setting or default value if not found</returns>
    protected string ReadRegistry(string sectionName, string keyName, string defValue)
    {
      OpenRegistryKey(sectionName, READ);
      string result = defValue;
      if (m_RegistryKey != null)
      {
        result = CxUtils.Nvl(m_RegistryKey.GetValue(keyName, defValue), "").ToString();
      }
      CloseRegistryKey();
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Writes setting to registry.
    /// </summary>
    /// <param name="sectionName">section name</param>
    /// <param name="keyName">settings key name</param>
    /// <param name="value">value to write</param>
    protected void WriteRegistry(string sectionName, string keyName, string value)
    {
      OpenRegistryKey(sectionName, WRITE);
      if (m_RegistryKey != null)
      {
        m_RegistryKey.SetValue(keyName, value);
      }
      CloseRegistryKey();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of setting names from the section.
    /// </summary>
    /// <param name="sectionName">section name</param>
    /// <returns>list of setting names from the section</returns>
    protected IList<string> GetRegistryNames(string sectionName)
    {
      OpenRegistryKey(sectionName, READ);
      string[] keys = null;
      if (m_RegistryKey != null)
      {
        keys = m_RegistryKey.GetValueNames();
      }
      CloseRegistryKey();
      return keys;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of folder names from the section.
    /// </summary>
    /// <param name="sectionName">section name</param>
    /// <returns>list of folder names from the section</returns>
    protected IList<string> GetRegistryFolderNames(string sectionName)
    {
      OpenRegistryKey(sectionName, READ);
      string[] keys = null;
      if (m_RegistryKey != null)
      {
        keys = m_RegistryKey.GetSubKeyNames();
      }
      CloseRegistryKey();
      return keys;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Deletes registry section.
    /// </summary>
    /// <param name="sectionName">section name</param>
    protected void DeleteRegistryKey(string sectionName)
    {
      IList<string> path = ParseRegistryPath(sectionName);
      if (path.Count > 0)
      {
        string nameToDelete = path[path.Count - 1];
        path.RemoveAt(path.Count - 1);
        string[] parentPathArray = new string[path.Count - 1];
        path.CopyTo(parentPathArray, 0);
        string parentSectionName = String.Join("\\", parentPathArray);
        OpenRegistryKey(parentSectionName, READ);
        if (m_RegistryKey != null)
        {
          m_RegistryKey.DeleteSubKeyTree(nameToDelete);
          CloseRegistryKey();
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Decomposes section path to the list of key names.
    /// </summary>
    /// <param name="path">string with paths separated by backslashes</param>
    /// <returns>list of section key names</returns>
    static public IList<string> ParseSectionPath(string path)
    {
      return CxText.DecomposeWithSeparator(path, @"\");
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Decomposes registry path to the list of key names.
    /// </summary>
    /// <param name="path">string with paths separated by backslashes</param>
    /// <returns>list of registry key names</returns>
    static public IList<string> ParseRegistryPath(string path)
    {
      return ParseSectionPath(path);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns setting as integer value.
    /// </summary>
    /// <param name="layout">map with settings</param>
    /// <param name="name">name of the setting</param>
    /// <param name="defValue">default setting value</param>
    /// <returns>setting as integer value</returns>
    static public int GetIntValue(NameValueCollection layout, string name, int defValue)
    {
      if (CxUtils.NotEmpty(layout[name]))
      {
        return CxInt.Parse(layout[name], defValue);
      }
      return defValue;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns setting as String value.
    /// </summary>
    /// <param name="layout">map with settings</param>
    /// <param name="name">name of the setting</param>
    /// <param name="defValue">default setting value</param>
    /// <returns>setting as string value</returns>
    static public string GetStrValue(NameValueCollection layout, string name, string defValue)
    {
      return CxUtils.Nvl(layout[name], defValue);
    }
    //-------------------------------------------------------------------------

  }
}
