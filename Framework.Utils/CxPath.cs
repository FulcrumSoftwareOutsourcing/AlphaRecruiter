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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Framework.Utils
{
  /// <summary>
  /// Utliity methods to work with file path.
  /// </summary>
  public class CxPath
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Convert absolute path into relative path
    /// </summary>
    /// <param name="absolutePath">absolute path need to convert into relative path</param>
    /// <param name="basePath">current absolute path</param>
    /// <returns>relative path</returns>
    static public string ConvertToRelative(string absolutePath, string basePath)
    {
      string dirSeparator = Path.DirectorySeparatorChar.ToString();
      string relativeSeparator = ".." + dirSeparator;
      string relativePath = absolutePath.ToLower();
      string currentPath = Path.GetDirectoryName(basePath).ToLower();

      int pos = relativePath.IndexOf(currentPath);
      int pos_cut;
      int levelUp = 0;

      if (pos == -1)
      {
        // Replace all slash with correct directory separator
        string tmpPath = currentPath.Replace("/", dirSeparator);
        // Remove last slash        
        if (tmpPath.Substring(tmpPath.Length - 1, 1) == dirSeparator)
        {
          tmpPath = tmpPath.Substring(0, tmpPath.Length - 2);
        }
        do
        {
          pos = tmpPath.LastIndexOf(dirSeparator);
          if (pos != -1)
          {
            tmpPath = tmpPath.Substring(0, pos);
            pos_cut = relativePath.IndexOf(tmpPath);
            if (pos_cut == -1)
            {
              levelUp++;
            }
            else
            {
              relativePath = relativePath.Substring(pos_cut + tmpPath.Length + 1);
              tmpPath = "";
              for (int i = 0; i <= levelUp; i++)
              {
                tmpPath += relativeSeparator;
              }
              relativePath = tmpPath + relativePath;
              tmpPath = "";
            }
          }
          else
          {
            tmpPath = "";
          }
        } while (tmpPath.Length > 0);
      }
      else if (CxText.NotEmpty(currentPath))
      {
        relativePath = absolutePath.Substring(pos + currentPath.Length + 1);
      }

      return relativePath;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Convert relative path into absolute path
    /// </summary>
    /// <param name="relativePath">relative path need to convert into absolute path</param>
    /// <param name="pathToCompare">absolute path to use as base</param>
    /// <returns>absolute path</returns>
    static public string ConvertToAbsolute(string relativePath, string pathToCompare)
    {
      if (CxText.IsEmpty(relativePath))
      {
        return pathToCompare + @"\.";
      }
      else
      {
        string currentPath = Directory.GetCurrentDirectory();
        try
        {
          if (CxText.NotEmpty(pathToCompare))
          {
            Directory.SetCurrentDirectory(pathToCompare);
          }
          return Path.GetFullPath(relativePath);
        }
        finally
        {
          Directory.SetCurrentDirectory(currentPath);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns system temporary path. Creates temporary directory if does not exist.
    /// </summary>
    /// <returns>system temporary path</returns>
    static public string GetTempPath()
    {
      string path = Path.GetTempPath();
      if (!Directory.Exists(path)) Directory.CreateDirectory(path);
      return path;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates temporary directory in temporary folder.
    /// </summary>
    /// <returns>created directory name</returns>
    static public string CreateTempFolder()
    {
      string fileName = CxFile.CreateTempFile();
      File.Delete(fileName);
      Directory.CreateDirectory(fileName);
      return fileName;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns application assemblies folder (folder where the entry assembly is located).
    /// Does not contain the trailing backslash.
    /// </summary>
    /// <returns>assemblies folder (entry assembly folder)</returns>
    static public string GetApplicationBinaryFolder()
    {
      string path = "";
      Assembly entry = Assembly.GetEntryAssembly();
      AppDomain appDomain = AppDomain.CurrentDomain;
      if (entry != null)
      {
        path = Path.GetDirectoryName(entry.Location);
      }
      else if (appDomain != null)
      {
        path = Path.Combine(appDomain.BaseDirectory, CxUtils.Nvl(appDomain.RelativeSearchPath));
      }
      return path;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Deletes a directory and its content without raising an exception.
    /// </summary>
    /// <param name="dirName">folder to delete</param>
    /// <returns>true if folder was deleted indeed</returns>
    static public bool SilentDelete(string dirName)
    {
      if (CxUtils.NotEmpty(dirName))
      {
        try
        {
          CxFile.SetAttrRecurrent(dirName, "*.*", FileAttributes.Normal);
          Directory.Delete(dirName, true);
          return true;
        }
        catch
        {
          return false;
        }
      }
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns path to current user's folder in Documents and Settings folder.
    /// </summary>
    /// <returns>path to current user's folder in Documents and Settings folder</returns>
    static public string GetUserSettingsFolder()
    {
      string s = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      if (CxUtils.NotEmpty(CxAppInfo.CompanyName))
      {
        s += "\\" + CxAppInfo.CompanyName;
      }
      if (CxUtils.NotEmpty(CxAppInfo.FrontendCode))
      {
        s += "\\" + CxAppInfo.FrontendCode;
      }
      return Directory.CreateDirectory(s).FullName;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns path to current user's folder in Documents and Settings folder.
    /// </summary>
    /// <returns>path to current user's folder in Documents and Settings folder</returns>
    static public string GetDocumentsFolder()
    {
      string s = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
      return Directory.CreateDirectory(s).FullName;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns array of file names containing in the given folder and all subfolders.
    /// </summary>
    /// <param name="paths">collection of root paths to search files</param>
    /// <param name="fileMaskCsvList">comma-separated list of file masks</param>
    /// <returns>array of file names</returns>
    static public string[] GetFilesRecurrent(ICollection<string> paths, string fileMaskCsvList)
    {
      UniqueList<string> list = new UniqueList<string>(StringComparer.OrdinalIgnoreCase);
      IList<string> maskList = CxText.DecomposeWithSeparator(fileMaskCsvList, ",");
      foreach (string path in paths)
      {
        foreach (string mask in maskList)
        {
          string[] files = Directory.GetFiles(path, mask);
          foreach (string fileName in files)
          {
            list.Add(fileName);
          }
        }
        string[] dirs = Directory.GetDirectories(path, "*.*");
        foreach (string dirName in dirs)
        {
          list.AddRange(GetFilesRecurrent(dirName, fileMaskCsvList));
        }
      }
      return list.ToArray();
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Returns array of file names containing in the given folder and all subfolders.
    /// </summary>
    /// <param name="path">root path to search files</param>
    /// <param name="fileMaskCsvList">comma-separated list of file masks</param>
    /// <returns>array of file names</returns>
    static public string[] GetFilesRecurrent(string path, string fileMaskCsvList)
    {
      return GetFilesRecurrent(new string[] { path }, fileMaskCsvList);
    }
    //--------------------------------------------------------------------------
  }
}