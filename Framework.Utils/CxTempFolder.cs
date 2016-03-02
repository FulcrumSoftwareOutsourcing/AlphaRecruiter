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
using System.Text;

namespace Framework.Utils
{
  public class CxTempFolder
  {
    //--------------------------------------------------------------------------  
    /// <summary>
    /// Returns full name of private user temp folder.
    /// </summary>
    /// <returns>name of the private user temp folder</returns>
    static public string GetName()
    {
      return CxPath.GetUserSettingsFolder() + @"\Temp\";
    }
    //--------------------------------------------------------------------------  
    /// <summary>
    /// Writes data as the file with the given name into the private user folder.
    /// </summary>
    /// <param name="fileName">name of the file to write into</param>
    /// <param name="data">file data</param>
    /// <returns>full name of the file (including folder name)</returns>
    static public string WriteFile(string fileName, byte[] data)
    {
      string folder = GetName();
      if (!Directory.Exists(folder))
      {
        Directory.CreateDirectory(folder);
      }
      string fullFileName = Path.Combine(folder, fileName);
      using (FileStream fs = File.OpenWrite(fullFileName))
      {
        fs.Write(data, 0, data.Length);
      }
      return fullFileName;
    }
    //--------------------------------------------------------------------------  
    /// <summary>
    /// Writes data as the file with the given name into the private user folder.
    /// </summary>
    /// <param name="fileName">name of the file to write into</param>
    /// <param name="text">file data</param>
    /// <returns>full name of the file (including folder name)</returns>
    static public string WriteFile(string fileName, string text)
    {
      string folder = GetName();
      if (!Directory.Exists(folder))
      {
        Directory.CreateDirectory(folder);
      }
      string fullFileName = Path.Combine(folder, fileName);
      using (StreamWriter sw = File.CreateText(fullFileName))
      {
        sw.Write(text);
      }
      return fullFileName;
    }
    //--------------------------------------------------------------------------  
    /// <summary>
    /// Deletes all files from private temporarily folder with last accessed 
    /// before the given validity period.
    /// </summary>
    /// <param name="validityPeriod">period to keep file last accessed in</param>
    static public void Clean(TimeSpan validityPeriod)
    {
      string folder = GetName();
      if (!Directory.Exists(folder)) return;

      DateTime now = DateTime.Now;
      DirectoryInfo dirInfo = new DirectoryInfo(folder);
      FileInfo[] files = dirInfo.GetFiles();
      foreach (FileInfo file in files)
      {
        if (now.Subtract(file.LastWriteTime).CompareTo(validityPeriod) >= 0)
        {
          CxFile.SilentDelete(file.FullName);
        }
      }
    }
    //--------------------------------------------------------------------------  
  }
}
