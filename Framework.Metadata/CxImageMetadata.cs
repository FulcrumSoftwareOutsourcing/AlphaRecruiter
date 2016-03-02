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
using System.Drawing;
using System.Reflection;
using System.Collections;
using System.Xml;

using Framework.Utils;

namespace Framework.Metadata
{
  /// <summary>
  /// Class to hold information about image.
  /// </summary>
  public class CxImageMetadata : CxMetadataObject
  {
    //----------------------------------------------------------------------------
    static protected Hashtable m_ImageProviders = new Hashtable(); // List of class/instance pairs
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="element">XML element that holds metadata</param>
    public CxImageMetadata(CxMetadataHolder holder, XmlElement element) : base(holder, element)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Id of the image provider class.
    /// </summary>
    public string ImageProviderClassId
    {
      get { return CxText.ToUpper(CxUtils.Nvl(this["image_provider_class_id"], this["provider_class_id"])); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Image index.
    /// </summary>
    public int ImageIndex
    {
      get { return CxInt.Parse(this["image_index"], CxInt.Parse(this["index"], -1)); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Class that provides images.
    /// </summary>
    public IxImageProvider ImageProvider 
    {
      get
      {
        Type klass = Holder.Classes[ImageProviderClassId].Class;
        IxImageProvider provider = (IxImageProvider) m_ImageProviders[klass];
        if (provider == null)
        {
          provider = (IxImageProvider) klass.Assembly.CreateInstance(klass.FullName);
          m_ImageProviders.Add(klass, provider);
        }
        return provider;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns image location folder ID (for Web images).
    /// </summary>
    public string FolderId
    { get {return this["folder_id"];} }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns image location folder Name (for JS Application).
    /// </summary>
    public string Folder
    { get { return this["folder"]; } }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns image file name (for Web images).
    /// </summary>
    public string FileName
    { get {return this["file_name"];} }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns full path to the image file (for Web images).
    /// </summary>
    public string FullPath
    {
      get
      {
        if (CxUtils.NotEmpty(FolderId))
        {
          string path = Holder.Images.GetFolder(FolderId).Path;
          string fileName = FileName;
          if (path.EndsWith("/"))
          {
            path = path.Substring(0, path.Length - 1);
          }
          if (fileName.StartsWith("/"))
          {
            fileName = fileName.Substring(1);
          }
          return path + "/" + fileName;
        }
        else
        {
          return FileName;
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets the icon image from the image provider.
    /// The method is here just to simplify and improve the readability of the code,
    /// because getting the icon image is very intensively used operation, the most
    /// used comparing to other types of images.
    /// </summary>
    /// <returns></returns>
    public Image GetIconImage()
    {
      return ImageProvider.GetImage("icon", ImageIndex);
    }
    //----------------------------------------------------------------------------
  }
}