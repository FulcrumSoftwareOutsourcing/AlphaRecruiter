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
using System.IO;
using Framework.Db;
using Framework.Metadata;
using Framework.Utils;

namespace Framework.Entity
{
  public class CxFrameworkImage : CxBaseEntity
  {
    //-------------------------------------------------------------------------
    public const string SPLASH_FILE_NAME = "Splash.gif";
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="metadata">entity usage metadata</param>
    public CxFrameworkImage(CxEntityUsageMetadata metadata)
      : base(metadata)
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Downloads splash image from the database into the file system 
    /// (if download needed).
    /// </summary>
    /// <param name="connection">database connection</param>
    static public void DownloadSplash(CxDbConnection connection)
    {
      string fileName = Path.Combine(CxPath.GetApplicationBinaryFolder(), SPLASH_FILE_NAME);
      DateTime imageDate;
      if (CxDate.Parse(connection.ExecuteScalar(
            @"select fi.ModificationDate
                from Framework_Images fi
               where fi.ImageCd = :ImageCd",
            SPLASH_FILE_NAME),
          out imageDate))
      {
        DateTime fileDate = File.GetLastWriteTime(fileName);
        if (fileDate < imageDate)
        {
          object data = connection.ExecuteScalar(
            @"select fi.Content
                from Framework_Images fi
               where fi.ImageCd = :ImageCd",
            SPLASH_FILE_NAME);
          if (data is byte[])
          {
            byte[] fileContent = null;
            byte[] bytes = (byte[]) data;
            CxBlobFile blobFile = CxBlobFile.LoadBlobIfValid(bytes);
            if (blobFile != null)
              fileContent = blobFile.Data;
            else
              fileContent = bytes;
            CxFile.SilentDelete(fileName);
            using (FileStream stream = File.Create(fileName))
            {
              stream.Write(fileContent, 0, fileContent.Length);
            }
            File.SetLastWriteTime(fileName, imageDate);
          }
        }
      }
      else
      {
        // Image not found in the DB - then delete the file from the disk if any is there.
        // In this case user should see the default splash screen of the application.
        CxFile.SilentDelete(fileName);
      }
    }
    //-------------------------------------------------------------------------
  }
}
