using System;
using Framework.Metadata;
using Framework.Remote;
using Framework.Remote.Mobile;
using Framework.Utils;
using System.Web.Caching;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Configuration;

namespace App.Server.Controllers
{
    public partial class HomeController
    {
        [Authorize]
        public ActionResult Upload(
            string uploadId,
            string dataStr,
            int chunkNumber,
            long fileLenght, 
            string entityUsageId,
            string attributeId)
        {
            
            byte[] data = JsonConvert.DeserializeObject<byte[]>(dataStr);

            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            try
            {
                CxUploadHandler uploadHandler = null;
                CxUploadResponse response;



                Cache cache =  HttpContext.Cache;

                //it is first chunk, create handler and start uploading
                if (string.IsNullOrEmpty(uploadId) )
                {
                    
                    CxEntityUsageMetadata meta = mHolder.EntityUsages[entityUsageId];
                    CxAttributeMetadata attribute = meta.GetAttribute(attributeId);
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

                    CxUploadData uploadData = new CxUploadData() {ChunkNumber = chunkNumber, Data = data, UploadId = uploadHandler.UploadId };
                    CxUploadParams uploadParams = new CxUploadParams() { AttributeId = attributeId, ChunkSize = data.Length, EntityUsageId = entityUsageId, FileLenght = fileLenght };
                    response = uploadHandler.HandleUpload(uploadData, uploadParams);

                    var result = new { uploadId = uploadHandler.UploadId };
                    json.Data = result;
                    return json;

                    
                }

                //it is not first chunk, but cach is destroyed, upload is failed by timeout reason
                if (string.IsNullOrEmpty(uploadId) == false && cache[uploadId] == null)
                {
                    throw new ExException();
                }

                //it is not first chunk and uploading handler is exists, continue uploading
                if (string.IsNullOrEmpty(uploadId) == false && cache[uploadId] != null)
                {
                    uploadHandler = (CxUploadHandler)cache[uploadId];

                    CxUploadData uploadData = new CxUploadData() { ChunkNumber = chunkNumber, Data = data, UploadId = uploadHandler.UploadId };
                    CxUploadParams uploadParams = new CxUploadParams() { AttributeId = attributeId, ChunkSize = data.Length, EntityUsageId = entityUsageId, FileLenght = fileLenght };

                    response = uploadHandler.HandleUpload(uploadData, uploadParams);
                    var result = new { uploadId = uploadHandler.UploadId };
                    json.Data = result;
                    return json;
                    
                }

            }
            catch (Exception ex)
            {
                CxUploadResponse response = new CxUploadResponse
                {
            //        UploadId = uploadData.UploadId,
              //      ChunkNumber = uploadData.ChunkNumber,
                //    UploadError = new CxExceptionDetails(ex)
                };

                var result = new { Error = ex.Message };

                json.Data = result;
                return json;


                
            }

            return new EmptyResult();
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