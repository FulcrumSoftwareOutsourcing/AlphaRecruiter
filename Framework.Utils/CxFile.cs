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
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace Framework.Utils
{
  /// <summary>
  /// Utility methods to work with files.
  /// </summary>
  public class CxFile
  {
    //-------------------------------------------------------------------------
    static Dictionary<string, Icon> m_ExtensionIcons =
      new Dictionary<string, Icon>(StringComparer.OrdinalIgnoreCase);
    static UniqueList<string> m_ExtensionEmptyIcons =
      new UniqueList<string>(StringComparer.OrdinalIgnoreCase);
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Checks if file available for writing. 
    /// Returns write exception or null if file is available for writing.
    /// </summary>
    /// <param name="fileName">name of file to check</param>
    /// <returns>file write exception or null if file is available for writing</returns>
    static public Exception GetWriteException(string fileName)
    {
      try
      {
        if (File.Exists(fileName))
        {
          using (File.OpenWrite(fileName)) { }
        }
        else
        {
          using (File.Create(fileName)) { }
          File.Delete(fileName);
        }
        return null;
      }
      catch (Exception e)
      {
        return e;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Checks if file available for writing.
    /// </summary>
    /// <param name="fileName">name of file to check</param>
    /// <returns>true if file can be written or false otherwise</returns>
    static public bool IsAvailableToWrite(string fileName)
    {
      return GetWriteException(fileName) == null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Check if given file is available to write. 
    /// If write exception occurred, executes exception handler.
    /// </summary>
    /// <param name="fileName">file to check</param>
    /// <param name="exceptionHandler">exception handler to execute on write error</param>
    static public void TryToWrite(string fileName, DxFileExceptionHandler exceptionHandler)
    {
      Exception e = GetWriteException(fileName);
      if (e != null)
      {
        if (exceptionHandler != null)
        {
          exceptionHandler(fileName, e);
        }
        else
        {
          throw e;
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates temporary file in temporary folder.
    /// </summary>
    /// <returns>created file name</returns>
    static public string CreateTempFile()
    {
      return Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Deletes a file without raising an exception.
    /// </summary>
    /// <param name="fileName">file to delete</param>
    /// <returns>true if file was deleted indeed</returns>
    static public bool SilentDelete(string fileName)
    {
      if (CxUtils.NotEmpty(fileName))
      {
        try
        {
          File.Delete(fileName);
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
    /// Sets file attributes for all files in specified path, including all 
    /// subfolders.
    /// </summary>
    /// <param name="path">target path</param>
    /// <param name="fileMask">target file mask to set attributes</param>
    /// <param name="fileAttr">attributes to set</param>
    static public void SetAttrRecurrent(string path,
                                        string fileMask,
                                        FileAttributes fileAttr)
    {
      string[] files = Directory.GetFiles(path, fileMask);
      foreach (string fileName in files)
      {
        File.SetAttributes(fileName, fileAttr);
      }
      string[] dirs = Directory.GetDirectories(path, fileMask);
      foreach (string dirName in dirs)
      {
        SetAttrRecurrent(dirName, fileMask, fileAttr);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads file from embedded resources.
    /// </summary>
    /// <param name="fileName">name of the file</param>
    /// <param name="type">type to get assembly and namespace</param>
    /// <returns>file contents</returns>
    static public string LoadContentFromResource(string fileName, Type type)
    {
      Stream stream = type.Assembly.GetManifestResourceStream(type.Namespace + "." + fileName);
      byte[] bytes = new byte[stream.Length];
      stream.Read(bytes, 0, (int)stream.Length);
      Decoder decoder = Encoding.ASCII.GetDecoder();
      char[] chars = new char[decoder.GetCharCount(bytes, 0, bytes.Length)];
      decoder.GetChars(bytes, 0, bytes.Length, chars, 0);
      string s = new string(chars);
      return s;
    }
    //-------------------------------------------------------------------------

    #region SHGetFileInfo Import
    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern int SHGetFileInfo(
      string pszPath,
      int dwFileAttributes,
      out    SHFILEINFO psfi,
      uint cbfileInfo,
      SHGFI uFlags);

    private const int MAX_PATH = 260;
    private const int MAX_TYPE = 80;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct SHFILEINFO
    {
      public SHFILEINFO(bool b)
      {
        hIcon = IntPtr.Zero;
        iIcon = 0;
        dwAttributes = 0;
        szDisplayName = "";
        szTypeName = "";
      }


      public IntPtr hIcon;
      public int iIcon;
      public uint dwAttributes;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
      public string szDisplayName;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_TYPE)]
      public string szTypeName;
    };

    [Flags]
    enum SHGFI : int
    {
      /// <summary>get icon</summary>
      Icon = 0x000000100,

      /// <summary>get display name</summary>
      DisplayName = 0x000000200,

      /// <summary>get type name</summary>
      TypeName = 0x000000400,

      /// <summary>get attributes</summary>
      Attributes = 0x000000800,

      /// <summary>get icon location</summary>
      IconLocation = 0x000001000,

      /// <summary>return exe type</summary>
      ExeType = 0x000002000,

      /// <summary>get system icon index</summary>
      SysIconIndex = 0x000004000,

      /// <summary>put a link overlay on icon</summary>
      LinkOverlay = 0x000008000,

      /// <summary>show icon in selected state</summary>
      Selected = 0x000010000,

      /// <summary>get only specified attributes</summary>
      Attr_Specified = 0x000020000,

      /// <summary>get large icon</summary>
      LargeIcon = 0x000000000,

      /// <summary>get small icon</summary>
      SmallIcon = 0x000000001,

      /// <summary>get open icon</summary>
      OpenIcon = 0x000000002,

      /// <summary>get shell size icon</summary>
      ShellIconize = 0x000000004,

      /// <summary>pszPath is a pidl</summary>
      PIDL = 0x000000008,

      /// <summary>use passed dwFileAttribute</summary>
      UseFileAttributes = 0x000000010,

      /// <summary>apply the appropriate overlays</summary>
      AddOverlays = 0x000000020,

      /// <summary>Get the index of the overlay in the upper 8 bits of the iIcon</summary>
      OverlayIndex = 0x000000040,
    }
    #endregion

    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns associated icon for the file.
    /// </summary>
    /// <param name="fileName">file name</param>
    /// <param name="large">true to return large icon</param>
    /// <returns>associated icon</returns>
    static private Icon GetAssociatedIcon(string fileName, bool large)
    {
      SHFILEINFO info = new SHFILEINFO(true);
      int cbFileInfo = Marshal.SizeOf(info);
      SHGFI flags;

      if (large)
        flags = SHGFI.Icon | SHGFI.LargeIcon | SHGFI.UseFileAttributes;
      else
        flags = SHGFI.Icon | SHGFI.SmallIcon | SHGFI.UseFileAttributes;


      SHGetFileInfo(fileName, 256, out info, (uint)cbFileInfo, flags);
      Icon result = (Icon)Icon.FromHandle(info.hIcon).Clone();
      CxImports.DestroyIcon(info.hIcon);
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns icon associated with the file extension.
    /// </summary>
    /// <param name="extension">file extension</param>
    /// <returns>associated icon or null</returns>
    static public Icon GetExtensionAssociatedIcon(string extension)
    {
      if (CxUtils.IsEmpty(extension))
      {
        return null;
      }

      if (!extension.StartsWith("."))
      {
        extension = "." + extension;
      }

      if (m_ExtensionEmptyIcons.Contains(extension))
      {
        return null;
      }

      Icon icon;
      if (m_ExtensionIcons.TryGetValue(extension, out icon))
      {
        return icon;
      }

      string tempPath = CxPath.GetTempPath();
      string emptyFileName = Path.Combine(tempPath, "0" + extension);
      if (!File.Exists(emptyFileName))
      {
        File.Create(emptyFileName);
      }
      icon = GetAssociatedIcon(emptyFileName, false);
      if (icon != null)
      {
        m_ExtensionIcons[extension] = icon;
      }
      else
      {
        m_ExtensionEmptyIcons.Add(extension);
      }
      SilentDelete(emptyFileName);
      return icon;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns icon associated with the file extension.
    /// </summary>
    /// <param name="fileName">file name</param>
    /// <returns>associated icon or null</returns>
    static public Icon GetFileExtensionAssociatedIcon(string fileName)
    {
      if (CxUtils.IsEmpty(fileName))
      {
        return null;
      }
      string extension = Path.GetExtension(fileName);
      return GetExtensionAssociatedIcon(extension);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Resets internal cache of icons associated with file extensions.
    /// </summary>
    static public void ResetExtensionAssociatedCache()
    {
      m_ExtensionIcons.Clear();
      m_ExtensionEmptyIcons.Clear();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns content type for the given file. 
    /// Content type is determined by the file extension.
    /// </summary>
    /// <param name="fileName">file name to return content type for</param>
    /// <returns>content type string</returns>
    static public string GetFileContentType(string fileName)
    {
      if (CxUtils.IsEmpty(fileName))
      {
        return null;
      }

      string extension = Path.GetExtension(fileName);
      if (CxUtils.IsEmpty(extension))
      {
        return null;
      }
      if (!extension.StartsWith("."))
      {
        extension = "." + extension;
      }

      RegistryKey extKey = Registry.ClassesRoot.OpenSubKey(extension);
      if (extKey != null)
      {
        string contentType = CxUtils.ToString(extKey.GetValue("Content Type"));
        extKey.Close();
        return contentType;
      }

      return null;
    }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Delegate to handle file exception.
    /// </summary>
    /// <param name="fileName">file name</param>
    /// <param name="e">exception</param>
    public delegate void DxFileExceptionHandler(string fileName, Exception e);
    //-------------------------------------------------------------------------
  }
}