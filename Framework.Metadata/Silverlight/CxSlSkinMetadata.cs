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
using System.Xml;

using Framework.Utils;

namespace Framework.Metadata
{
  public class CxSlSkinMetadata : CxMetadataObject
  {
    //----------------------------------------------------------------------------

    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">metadata holder</param>
    /// <param name="element">XML element that holds metadata</param>
    public CxSlSkinMetadata(
      CxMetadataHolder holder, 
      XmlElement element) : base(holder, element)
    {
    }
    //----------------------------------------------------------------------------

    //----------------------------------------------------------------------------
    /// <summary>
    /// True if the skin is a default skin.
    /// </summary>
    public bool IsDefault
    { get { return this["is_default"] == "true"; } }
    //----------------------------------------------------------------------------
    /// <summary>
    /// The value of the display order for the skin.
    /// The less the value the earlier it will be placed in the list.
    /// </summary>
    public int DisplayOrder
    { get { return CxInt.Parse(this["display_order"], int.MaxValue); } }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns ID of the image to display for the tree item.
    /// </summary>
    public string ImageId
    {
      get
      {
        return this["image_id"];
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns metadata of the the image to display for the tree item.
    /// </summary>
    public CxImageMetadata Image
    {
      get
      {
        if (CxUtils.NotEmpty(ImageId))
        {
          return Holder.Images[ImageId];
        }
        return null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The path to the folder containing the skin.
    /// </summary>
    public string FolderPath
    {
      get { return this["folder_path"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Checks skin access permission depending on security settings.
    /// </summary>
    public bool GetIsAllowed()
    {
      if (Holder != null && Holder.Security != null)
      {
        return Holder.Security.GetRight(this);
      }
      return true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns object type code for localization.
    /// </summary>
    override public string LocalizationObjectTypeCode
    {
      get
      {
        return "Metadata.SlSkin";
      }
    }
    //----------------------------------------------------------------------------
  }
}