using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using Framework.Db;
using Framework.Metadata;
using Framework.Utils;

namespace Framework.Remote
{
  public class CxSlImageLibraryEntity : CxSlEntity
  {
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="metadata">metadata that describes this entity</param>
    public CxSlImageLibraryEntity(CxEntityUsageMetadata metadata) : base(metadata)
    {
    }

    //----------------------------------------------------------------------------
    public const string IMAGE_WIDTH_ATTR  = "IMAGEWIDTH";
    public const string IMAGE_HEIGHT_ATTR = "IMAGEHEIGHT";
    //----------------------------------------------------------------------------
   
   
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns image attribute.
    /// </summary>
    /// <returns>image attribute</returns>
    virtual protected CxAttributeMetadata GetImageAttribute()
    {
      return Metadata.GetFirstDbFileAttribute();
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Inserts entity (and all "owned" child) entities to the database.
    /// </summary>
    /// <param name="connection">connection an INSERT should work in context of</param>
    public override void Insert(CxDbConnection connection)
    {
      //CxAppServerContext context = new CxAppServerContext();
      //this["CreatedByUserId".ToUpper()] = context.UserId;
      //this["ModifiedByUserId".ToUpper()] = context.UserId;
      base.Insert(connection);
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Updates entity (and all "owned" child entities) in the database.
    /// </summary>
    /// <param name="connection">connection an UPDATE should work in context of</param>
    public override void Update(CxDbConnection connection)
    {
      CxAppServerContext context = new CxAppServerContext();
      this["ModifiedByUserId".ToUpper()] = context.UserId;

      base.Update(connection);
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Sets binary data from cache nt fields with type equals 'file'  
    /// </summary>
    protected override void AssignBlobsFromCache()
    {
      foreach (Metadata.CxAttributeMetadata atrMetadata in Metadata.Attributes)
      {
        if (IsBlobToRestoreFromCache(atrMetadata))
        {
          string cacheId = this[atrMetadata.Id].ToString();
          Cache cache = HttpContext.Current.Cache;
          if (cache[cacheId] == null)
            throw new ExException("Cached binary data is expired.");
          byte[] imageData = ((CxUploadHandler) cache[cacheId]).GetData();
          cache.Remove(cacheId);

          CxBlobFile file = new CxBlobFile();
          CxBlobFileHeader header = new CxBlobFileHeader();
          header.ContentLength = imageData.Length;
          header.ContentType = "image/x-png";
          header.FileName = this["Name".ToLower()].ToString();
          header.UploadDateTime = DateTime.Now;
        
          file.Load(header, imageData);


          this[atrMetadata.Id] = file.FieldValue;
          //cache.Remove(cacheId);
          this[GetBlobStateAttributeId(atrMetadata.Id)] = BLOB_PRESENT_IN_DB;
        }
        if (IsBlobToRemoveAttribute(atrMetadata))
        {
          this[atrMetadata.Id] = null;
          this[GetBlobStateAttributeId(atrMetadata.Id)] = REMOVE_BLOB_FROM_DB;
        }
      }
    }

    /// <summary>
    /// Validates entity.
    /// </summary>
    //override public void Validate()
    //{
    //  base.Validate();

    //  CxAttributeMetadata imageAttr = GetImageAttribute();
    //  if (imageAttr != null)
    //  {
    //    object imageValue = this[imageAttr.Id];
    //    if (!(imageValue is byte[]))
    //    {
    //      throw new ExValidationException(
    //        Metadata.Holder.GetErr("Image is empty or invalid."), imageAttr.Id);
    //    }
    //    CxBlobFile bFile = new CxBlobFile();
    //    bFile.LoadFromDbField((byte[])imageValue);
    //    if (bFile.IsEmpty)
    //    {
    //      throw new ExValidationException(
    //        Metadata.Holder.GetErr("Image could not be empty."), imageAttr.Id);
    //    }

    //    CxAttributeMetadata nameAttr = Metadata.NameAttribute;
    //    if (nameAttr != null && nameAttr.ReadOnly)
    //    {
    //      if (CxUtils.NotEmpty(bFile.FileName))
    //      {
    //        this[nameAttr.Id] = bFile.FileName;
    //      }
    //      else if (CxUtils.NotEmpty(this[nameAttr.Id]))
    //      {
    //        bFile.Header.FileName = CxUtils.ToString(this[nameAttr.Id]);
    //      }
    //    }

    //    CxAttributeMetadata widthAttr = Metadata.GetAttribute(IMAGE_WIDTH_ATTR);
    //    CxAttributeMetadata heightAttr = Metadata.GetAttribute(IMAGE_HEIGHT_ATTR);
    //    if (widthAttr != null && heightAttr != null)
    //    {
    //      Size imageSize;
    //      try
    //      {
    //        imageSize = CxImage.GetSize(bFile.Data);
    //      }
    //      catch
    //      {
    //        imageSize = Size.Empty;
    //      }
    //      if (imageSize != Size.Empty)
    //      {
    //        this[widthAttr.Id] = imageSize.Width;
    //        this[heightAttr.Id] = imageSize.Height;
    //      }
    //      else
    //      {
    //        this[widthAttr.Id] = null;
    //        this[heightAttr.Id] = null;
    //      }
    //    }
    //  }
    //}
    //----------------------------------------------------------------------------





  }
}
