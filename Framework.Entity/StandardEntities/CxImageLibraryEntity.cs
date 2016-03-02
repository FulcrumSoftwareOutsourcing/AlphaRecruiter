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

using System.Drawing;

using Framework.Metadata;
using Framework.Utils;

namespace Framework.Entity
{
  /// <summary>
  /// Entity representing DB image library element.
  /// </summary>
  public class CxImageLibraryEntity : CxBaseEntity
  {
    //----------------------------------------------------------------------------
    public const string IMAGE_WIDTH_ATTR = "IMAGEWIDTH";
    public const string IMAGE_HEIGHT_ATTR = "IMAGEHEIGHT";
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="metadata">metadata that describes this entity</param>
    public CxImageLibraryEntity(CxEntityUsageMetadata metadata)
      : base(metadata)
    {
    }
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
    /// Validates entity.
    /// </summary>
    override public void Validate()
    {
      base.Validate();

      CxAttributeMetadata imageAttr = GetImageAttribute();
      if (imageAttr != null)
      {
        object imageValue = this[imageAttr.Id];
        if (!(imageValue is byte[]))
        {
          throw new ExValidationException(
            Metadata.Holder.GetErr("Image is empty or invalid."), imageAttr.Id);
        }
        CxBlobFile bFile = new CxBlobFile();
        bFile.LoadFromDbField((byte[]) imageValue);
        if (bFile.IsEmpty)
        {
          throw new ExValidationException(
            Metadata.Holder.GetErr("Image could not be empty."), imageAttr.Id);
        }

        CxAttributeMetadata nameAttr = Metadata.NameAttribute;
        if (nameAttr != null && nameAttr.ReadOnly)
        {
          if (CxUtils.NotEmpty(bFile.FileName))
          {
            this[nameAttr.Id] = bFile.FileName;
          }
          else if (CxUtils.NotEmpty(this[nameAttr.Id]))
          {
            bFile.Header.FileName = CxUtils.ToString(this[nameAttr.Id]);
          }
        }

        CxAttributeMetadata widthAttr = Metadata.GetAttribute(IMAGE_WIDTH_ATTR);
        CxAttributeMetadata heightAttr = Metadata.GetAttribute(IMAGE_HEIGHT_ATTR);
        if (widthAttr != null && heightAttr != null)
        {
          Size imageSize;
          try
          {
            imageSize = CxImage.GetSize(bFile.Data);
          }
          catch
          {
            imageSize = Size.Empty;
          }
          if (imageSize != Size.Empty)
          {
            this[widthAttr.Id] = imageSize.Width;
            this[heightAttr.Id] = imageSize.Height;
          }
          else
          {
            this[widthAttr.Id] = null;
            this[heightAttr.Id] = null;
          }
        }
      }
    }
    //----------------------------------------------------------------------------
  }
}