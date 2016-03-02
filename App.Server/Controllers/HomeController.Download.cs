using Framework.Db;
using Framework.Entity;
using Framework.Entity.Filter;
using Framework.Metadata;
using Framework.Remote;
using Framework.Remote.Mobile;
using Framework.Utils;
using Framework.Web.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web.Mvc;
using System.Linq;
using System.Web.Caching;
using System.Text;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace App.Server.Controllers
{
    public partial class HomeController
    {

        [Authorize]
        public ActionResult ShowImage(string imageUrl)
        {
            ViewBag.ImageUrl = imageUrl;
            return View();
        }


        protected const string RESPONSE_CONTENT_TYPE = "application/octet-stream";
        protected const string RESPONSE_CONTENT_TYPE_IMAGE = "image";
        protected const string RESPONSE_DEFAULT_DOWNLOAD_FILE_NAME = "file_content";

        [Authorize]
        public ActionResult Download(
               string entityUsageId,
               string attributeId,
               string pkVals, 
               string settingsToSave)
        {

            SaveSetting(settingsToSave);

            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
//#if (!DEBUG)
            try
            {
                //#endif

                

            List<Dictionary<string, object>> selectedEntities;
            Dictionary<string, object> currentEntity;
            Dictionary<string, object> primaryKeysValues;
            Dictionary<string, object> whereValues;
            Dictionary<string, object> joinValues;
            List<CxFilterItem> filterItems;
            List<CxSortDescription> sortDescriptions;
            Dictionary<string, object> parentPks;

            List<string> validationErrors = new List<string>();

            CxEntityUsageMetadata entityUsage = mHolder.EntityUsages[entityUsageId];
               



                ParseQueryParams(
                out primaryKeysValues,
                out whereValues,
                out joinValues,
                out filterItems,
                out sortDescriptions,
                out parentPks,
                out currentEntity,
                out selectedEntities,
                pkVals,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                entityUsage, 
                null);


            if (string.IsNullOrEmpty(entityUsageId) ||
                string.IsNullOrEmpty(attributeId) ||
                primaryKeysValues == null ||
                primaryKeysValues.Count == 0)
                throw new ExException("Invalid download parameters.");


            
            
            CxAttributeMetadata blobAttrMetadata = entityUsage.GetAttribute(attributeId);

            CxHashtable primaryKeys = new CxHashtable();
            foreach (var v in primaryKeysValues)
            {
                primaryKeys.Add(v.Key, v.Value);
            }

            string fileName = GetFileName(entityUsage, blobAttrMetadata, primaryKeys);

            
            Response.ContentType = RESPONSE_CONTENT_TYPE;
            Response.AppendHeader(
              "Content-Disposition", "attachment; filename=\"" + fileName + "\"");




            try
            {



                using (CxDbConnection conn = CxDbConnections.CreateEntityConnection(entityUsage))
                {
                    #region Workaround, because CxDbConnection.ExecuteReader method do not supports IEnumerable or IxValueProvider as query parameters

                    StringBuilder whereCondition = new StringBuilder();
                    whereCondition.Append(" where ");
                    int pkCount = 0;
                    foreach (DictionaryEntry entry in primaryKeys)
                    {
                        whereCondition.Append(entry.Key).Append(" = ");
                        if (entry.Value is int || entry.Value is Int64)
                        {
                            whereCondition.Append(entry.Value);
                        }
                        else if (entry.Value is string)
                        {
                            whereCondition.Append(" '").Append(entry.Value).Append("' ");
                        }
                        //todo: add other types conversion if needed
                        else
                        {
                            throw new ExException(string.Format("Unsupported PK type '{0}'.",
                                                                entry.Value.GetType().Name));
                        }
                        if (pkCount < (primaryKeys.Count - 1))
                        {
                            whereCondition.Append(" , ");
                        }
                    }

                    #endregion

                    using (IDataReader reader = conn.ExecuteReader(@"select " + blobAttrMetadata.Id +
                                                                   " from " + entityUsage.DbObject + whereCondition))
                    {
                        reader.Read();
                        int chunkSize = 102400;
                        BinaryWriter writer = new BinaryWriter(Response.OutputStream);
                        byte[] buffer = new byte[chunkSize];
                        long blobSize = reader.GetBytes(0, 0, null, 0, 0);
                      
                        long currPos = 0;
                        while (currPos < blobSize)
                        {
                            if (Response.IsClientConnected)
                            {
                                if ((currPos + chunkSize) > blobSize)
                                {
                                    buffer = new byte[blobSize - currPos];
                                }
                                currPos += reader.GetBytes(0, currPos, buffer, 0, buffer.Length);
                                writer.Write(buffer);
                                writer.Flush();
                                Response.Flush();
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }


            }

            finally
            {
                Response.End();
                Response.Close();
            }

            return new EmptyResult();

                
//#if (!DEBUG)
            }
            catch (Exception ex)
            {
                


                byte[] utf8 = System.Text.Encoding.UTF8.GetBytes(ex.Message);

                return File(utf8, "text/plain", "error.txt");

                
                //var result = new { Error = ex.Message };

                //json.Data = result;
                //return json;
            }
//#endif
    }



        [Authorize]
        public ActionResult Photo(
              string entityUsageId,
              string attributeId,
              string pkVals, 
              bool thumb)
        {
            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //#if (!DEBUG)
            try
            {
                //#endif
                List<Dictionary<string, object>> selectedEntities;
                Dictionary<string, object> currentEntity;
                Dictionary<string, object> primaryKeysValues;
                Dictionary<string, object> whereValues;
                Dictionary<string, object> joinValues;
                List<CxFilterItem> filterItems;
                List<CxSortDescription> sortDescriptions;
                Dictionary<string, object> parentPks;

                List<string> validationErrors = new List<string>();

                CxEntityUsageMetadata entityUsage = mHolder.EntityUsages[entityUsageId];




                ParseQueryParams(
                out primaryKeysValues,
                out whereValues,
                out joinValues,
                out filterItems,
                out sortDescriptions,
                out parentPks,
                out currentEntity,
                out selectedEntities,
                pkVals,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                entityUsage,
                null);


                if (string.IsNullOrEmpty(entityUsageId) ||
                    string.IsNullOrEmpty(attributeId) ||
                    primaryKeysValues == null ||
                    primaryKeysValues.Count == 0)
                    throw new ExException("Invalid download parameters.");




                CxAttributeMetadata blobAttrMetadata = entityUsage.GetAttribute(attributeId);

                CxHashtable primaryKeys = new CxHashtable();
                foreach (var v in primaryKeysValues)
                {
                    primaryKeys.Add(v.Key, v.Value);
                }

                string fileName = GetFileName(entityUsage, blobAttrMetadata, primaryKeys);


                Response.ContentType = RESPONSE_CONTENT_TYPE_IMAGE;
                Response.AppendHeader(
                  "Content-Disposition", "attachment; filename=\"" + fileName + "\"");




                try
                {



                    using (CxDbConnection conn = CxDbConnections.CreateEntityConnection(entityUsage))
                    {
                        #region Workaround, because CxDbConnection.ExecuteReader method do not supports IEnumerable or IxValueProvider as query parameters

                        StringBuilder whereCondition = new StringBuilder();
                        whereCondition.Append(" where ");
                        int pkCount = 0;
                        foreach (DictionaryEntry entry in primaryKeys)
                        {
                            whereCondition.Append(entry.Key).Append(" = ");
                            if (entry.Value is int || entry.Value is Int64)
                            {
                                whereCondition.Append(entry.Value);
                            }
                            else if (entry.Value is string)
                            {
                                whereCondition.Append(" '").Append(entry.Value).Append("' ");
                            }
                            //todo: add other types conversion if needed
                            else
                            {
                                throw new ExException(string.Format("Unsupported PK type '{0}'.",
                                                                    entry.Value.GetType().Name));
                            }
                            if (pkCount < (primaryKeys.Count - 1))
                            {
                                whereCondition.Append(" , ");
                            }
                        }

                        #endregion
                        if (thumb == false)
                        {
                            using (IDataReader reader = conn.ExecuteReader(@"select " + blobAttrMetadata.Id +
                                                                           " from " + entityUsage.DbObject + whereCondition))
                            {
                                reader.Read();
                                int chunkSize = 102400;
                                BinaryWriter writer = new BinaryWriter(Response.OutputStream);
                                byte[] buffer = new byte[chunkSize];
                                long blobSize = reader.GetBytes(0, 0, null, 0, 0);
                              
                                long currPos = 0;
                                while (currPos < blobSize)
                                {
                                    if (Response.IsClientConnected)
                                    {
                                        if ((currPos + chunkSize) > blobSize)
                                        {
                                            buffer = new byte[blobSize - currPos];
                                        }
                                        currPos += reader.GetBytes(0, currPos, buffer, 0, buffer.Length);
                                        writer.Write(buffer);
                                        writer.Flush();
                                        Response.Flush();
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {







                            using (IDataReader reader = conn.ExecuteReader(@"select " + blobAttrMetadata.Id +
                                                                          " from " + entityUsage.DbObject + whereCondition))
                            {
                                long blobSize = 0;
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    reader.Read();
                                    int chunkSize = 102400;
                                    BinaryWriter writer = new BinaryWriter(ms);
                                    byte[] buffer = new byte[chunkSize];
                                    blobSize = reader.GetBytes(0, 0, null, 0, 0);

                                    long currPos = 0;
                                    while (currPos < blobSize)
                                    {
                                        if (Response.IsClientConnected)
                                        {
                                            if ((currPos + chunkSize) > blobSize)
                                            {
                                                buffer = new byte[blobSize - currPos];
                                            }
                                            currPos += reader.GetBytes(0, currPos, buffer, 0, buffer.Length);
                                            writer.Write(buffer);
                                            writer.Flush();
                                           
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }

                                    ms.Position = 0;
                                    Image img = Image.FromStream(ms);
                                    Image thumbImg = FixedSize(img, blobAttrMetadata.ThumbnailWidth, blobAttrMetadata.ThumbnailHeight);

                                    if (Response.IsClientConnected)
                                    {

                                        ImageFormat imgFormat;
                                        string imgExt = Path.GetExtension(fileName);
                                        if (string.Compare(imgExt, "Bmp", true) == 0)
                                            imgFormat = ImageFormat.Bmp;
                                        else if (string.Compare(imgExt, "Emf", true) == 0)
                                            imgFormat = ImageFormat.Emf;
                                        else if (string.Compare(imgExt, "Exif", true) == 0)
                                            imgFormat = ImageFormat.Exif;
                                        else if (string.Compare(imgExt, "Gif", true) == 0)
                                            imgFormat = ImageFormat.Gif;
                                        else if (string.Compare(imgExt, "ico", true) == 0)
                                            imgFormat = ImageFormat.Icon;
                                        else if (string.Compare(imgExt, "Jpeg", true) == 0 || string.Compare(imgExt, "jpg", true) == 0)
                                            imgFormat = ImageFormat.Jpeg;
                                        else if (string.Compare(imgExt, "Png", true) == 0)
                                            imgFormat = ImageFormat.Png;
                                        else if (string.Compare(imgExt, "Tiff", true) == 0)
                                            imgFormat = ImageFormat.Tiff;
                                        else if (string.Compare(imgExt, "Wmf", true) == 0)
                                            imgFormat = ImageFormat.Wmf;
                                        else
                                            imgFormat = ImageFormat.Jpeg;


                                        

                                        thumbImg.Save(Response.OutputStream, imgFormat);

                                        return File(ms.ToArray(), "image", fileName);
                                    }
                                }


                                
                                    
                            

                            }










                        }
                    }


                }

                finally
                {
                    Response.End();
                    Response.Close();
                }

                return new EmptyResult();


                //#if (!DEBUG)
            }
            catch (Exception ex)
            {
                var result = new { Error = ex.Message };

                json.Data = result;
                return json;
            }
            //#endif
        }


        private Image FixedSize(Image imgPhoto, int Width, int Height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((Width -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((Height -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Width, Height,
                              PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                             imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Gray);
            grPhoto.InterpolationMode =
                    InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }


        //----------------------------------------------------------------------------
        /// <summary>
        /// Returns file name.
        /// </summary>
        private string GetFileName(
          CxEntityUsageMetadata entityUsage,
          CxAttributeMetadata blobAttrMetadata,
          IxValueProvider primaryKeys)
        {
            if (string.IsNullOrEmpty(blobAttrMetadata.BlobFileNameAttributeId))
                return RESPONSE_DEFAULT_DOWNLOAD_FILE_NAME;

            object fileName = null;
            using (CxDbConnection conn = CxDbConnections.CreateEntityConnection(entityUsage))
            {
                fileName = conn.ExecuteScalar(@"select " +
                  blobAttrMetadata.BlobFileNameAttributeId + " from " +
                    entityUsage.DbObject + " where " + entityUsage.ComposePKCondition()
                  , primaryKeys);
            }

            if (fileName == null || fileName is DBNull)
            {
                return RESPONSE_DEFAULT_DOWNLOAD_FILE_NAME;
            }
            return fileName.ToString();
        }




    }





    public class SqlReaderStream : Stream
    {
        private IDataReader reader;
        private int columnIndex;
        private long position;

        public SqlReaderStream(
            IDataReader reader,
            int columnIndex)
        {
            this.reader = reader;
            this.columnIndex = columnIndex;
        }

        public override long Position
        {
            get { return position; }
            set { throw new NotImplementedException(); }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            long bytesRead = reader.GetBytes(columnIndex, position, buffer, offset, count);
            position += bytesRead;
            return (int)bytesRead;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && null != reader)
            {
                reader.Dispose();
                reader = null;
            }
            base.Dispose(disposing);
        }
    }








    public class DownloadResult : FileStreamResult
    {
        public DownloadResult(Stream fileStream, string contentType)
            : base(fileStream, contentType)
        { }


        public long? FileSize { get; set; }


        protected override void WriteFile(HttpResponseBase response)
        {
            response.BufferOutput = false;


            if (FileSize.HasValue)
            {
                response.AddHeader("Content-Length", FileSize.ToString());
            }
            base.WriteFile(response);
        }
    }



}