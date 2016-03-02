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

using System;
using System.Configuration;
using System.Web;
using System.Web.Caching;
using Framework.Metadata;
using Framework.Remote.Mobile;
using Framework.Utils;

namespace Framework.Remote
{
  public partial class CxAppServer
  {
    /// <summary>
    /// Uploads data on server.
    /// </summary>
    /// <param name="uploadData">Upload data.</param>
    /// <param name="uploadParams">Upload parameters.</param>
    /// <returns>Initialized CxUploadResponse.</returns>
    public CxUploadResponse Upload(CxUploadData uploadData, CxUploadParams uploadParams)
    {
      try
      {
        CxUploadHandler uploadHandler = null;
        CxUploadResponse response;
        Cache cache = HttpContext.Current.Cache;

        //it is first chunk, create handler and start uploading
        if (uploadData.UploadId == Guid.Empty)
        {
          if (uploadParams == null)
          {
            throw new ExException("CxUploadParams for first uploading request must be initialized.");
          }
          CxEntityUsageMetadata meta = m_Holder.EntityUsages[uploadParams.EntityUsageId];
          CxAttributeMetadata attribute = meta.GetAttribute(uploadParams.AttributeId);
          uploadHandler = CxUploadHandler.Create(attribute);

          int timeout = CxInt.Parse(ConfigurationManager.AppSettings["WebServiceTimeout"], 30000);
          TimeSpan timeoutSpan = new TimeSpan(0, 0, 0, 0, timeout);

          cache.Insert(
            uploadHandler.UploadId.ToString(),
            uploadHandler,
            null,
            Cache.NoAbsoluteExpiration,
            timeoutSpan,
            CacheItemPriority.NotRemovable,
            CacheRemovedHandler);

          response = uploadHandler.HandleUpload(uploadData, uploadParams);
          return response;
        }

        //it is not first chunk, but cach is destroyed, upload is failed by timeout reason
        if (uploadData.UploadId != Guid.Empty && cache[uploadData.UploadId.ToString()] == null)
        {
          throw new ExException();
        }

        //it is not first chunk and uploading handler is exists, continue uploading
        if (uploadData.UploadId != Guid.Empty && cache[uploadData.UploadId.ToString()] != null)
        {
          uploadHandler = (CxUploadHandler)cache[uploadData.UploadId.ToString()];
          response = uploadHandler.HandleUpload(uploadData, uploadParams);
          return response;
        }

      }
      catch (Exception ex)
      {
        CxUploadResponse response = new CxUploadResponse
        {
          UploadId = uploadData.UploadId,
          ChunkNumber = uploadData.ChunkNumber,
          UploadError = new CxExceptionDetails(ex)
        };
        return response;
      }

      return null;
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Handles uploading Cache removing.
    /// </summary>
    public static void CacheRemovedHandler(string key, object removed, CacheItemRemovedReason reason)
    {
      CxUploadHandler uploadHandler = removed as CxUploadHandler;
      if (uploadHandler != null)
      {
        try
        {
          uploadHandler.Dispose();
        }
        catch { }
      }
    }
  }
}
