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

using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.Xml.Linq;
using Framework.Db;
using Framework.Metadata;
using Framework.Remote.Mobile;
using Framework.Utils;
using Framework.Web.Utils;

namespace Framework.Remote
{
  /// <summary>
  /// Class that holds all application metadata. Singleton.
  /// </summary>
  public abstract class CxSlMetadataHolder : Metadata.CxMetadataHolder
  {
    
    protected CxSlMetadataHolder()
    {
      OnCreateDbConnection += CreateConnection;
      OnCreateUserPermissionProvider += CreatePermissionProvider;
      OnGetUserPermissionProvider += GetUserPermissionProvider;
      OnGetApplicationValueProvider += GetApplicationValueProvider;
      OnGetEntityDataCacheObject += CxWebUtils.GetApplicationCachedObject;
      OnSetEntityDataCacheObject += CxWebUtils.SetApplicationCachedObject;
      OnClearEntityIdCache += CxWebUtils.ClearEntityIdCache;
      IsEntityDataCacheEnabled = CxBool.Parse(ConfigurationSettings.AppSettings["enableEntityDataCache"]);
    }

    //----------------------------------------------------------------------------
    public Dictionary<string, CxLayoutElement> FramesMetadata { get; protected set; }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Loads SlFrame files.
    /// </summary>
    /// <param name="framesFilesNames">List of files names with SlFrames XML defenitions.</param>
    protected void LoadFramesMetadata(IEnumerable<string> framesFilesNames)
    {
      FramesMetadata = new Dictionary<string, CxLayoutElement>();
      foreach (string framesFile in framesFilesNames)
      {
        XmlDocument framesDoc = LoadMetadata(framesFile);


        XmlNodeList frameElemList = framesDoc.SelectNodes("//frames/frame");

        for (int i = 0; i < frameElemList.Count; i++)
        {
          XElement frameXml = XElement.Parse(frameElemList[i].OuterXml);
          string frameId = frameXml.Attribute("id").Value;
          if (FramesMetadata.ContainsKey(frameId))
          {
            throw new ExException("SlFrames dictionary already contains frame with Id '" +
                frameId + "' .");
          }
          CxLayoutElement layoutElement = new CxLayoutElement(frameXml);
          FramesMetadata.Add(frameId, layoutElement);
        }
      }
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns connection string.
    /// Must be overriden to get real connection string.
    /// </summary>
    /// <returns>Connection string.</returns>
    public abstract string GetConnectionString();

    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates and returns CxDbConnection.
    /// </summary>
    /// <returns>Created CxDbConnection.</returns>
    public virtual CxDbConnection CreateConnection()
    {
      return CxDbConnection.Create(NxDataProviderType.SqlClient, GetConnectionString());
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates and returns IxUserPermissionProvider.
    /// </summary>
    /// <returns>Created IxUserPermissionProvider.</returns>
    protected virtual IxUserPermissionProvider CreatePermissionProvider()
    {
      return new CxUserPermissionProvider(this);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates and returns IxUserPermissionProvider.
    /// </summary>
    /// <returns>Created IxUserPermissionProvider.</returns>
    protected virtual IxUserPermissionProvider GetUserPermissionProvider()
    {
      CxAppServerContext appServerContext = new CxAppServerContext();
      return appServerContext.GetUserPermissionProvider();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates and returns IxValueProvider with global app values.
    /// </summary>
    /// <returns>Created IxValueProvider.</returns>
    protected virtual IxValueProvider GetApplicationValueProvider()
    {
      return new CxAppServerContext();
    }

  }
}
