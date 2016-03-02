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
using System.Collections;
using System.Xml;

using Framework.Utils;
using System.Collections.Generic;

namespace Framework.Metadata
{
  /// <summary>
  /// Class to read and hold information about images.
  /// </summary>
  public class CxImagesMetadata : CxMetadataCollection
  {
    //----------------------------------------------------------------------------
    protected Hashtable m_Images = new Hashtable(); // Images dictionary
    protected Hashtable m_Folders = new Hashtable(); // Folders dictionary
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="doc">name of file to read images metadata</param>
    public CxImagesMetadata(CxMetadataHolder holder, XmlDocument doc) : 
      base(holder, doc)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="docs">name of file to read assemblies metadata</param>
    public CxImagesMetadata(CxMetadataHolder holder, IEnumerable<XmlDocument> docs)
      :
      base(holder, docs)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata collection from the XML document.
    /// </summary>
    /// <param name="doc">document to load data from</param>
    override protected void Load(XmlDocument doc)
    {
      base.Load(doc);
      foreach (XmlElement element in doc.DocumentElement.SelectNodes("image_folder"))
      {
        CxFolderMetadata folder = new CxFolderMetadata(Holder, element);
        m_Folders.Add(folder.Id, folder);
      }
      foreach (XmlElement element in doc.DocumentElement.SelectNodes("image"))
      {
        CxImageMetadata image = new CxImageMetadata(Holder, element);
        m_Images.Add(image.Id, image);
      }
      LoadOverrides(doc, "image_folder_override", m_Folders);
      LoadOverrides(doc, "image_override", m_Images);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds and returns image metadata with the given ID.
    /// </summary>
    /// <param name="id">image ID</param>
    /// <returns>image metadata or null</returns>
    public CxImageMetadata Find(string id)
    {
      return id != null ? (CxImageMetadata)m_Images[id.ToUpper()] : null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Image with the given ID.
    /// </summary>
    public CxImageMetadata this[string id]
    {
      get
      { 
        CxImageMetadata image = Find(id);
        if (image != null)
          return image;
        else
          throw new ExMetadataException(string.Format("Image with ID=\"{0}\" not defined", id));
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns folder by the given ID.
    /// </summary>
    /// <param name="id">ID of folder to return</param>
    /// <returns>found folder metadata object</returns>
    public CxFolderMetadata GetFolder(string id)
    {
      CxFolderMetadata folder = (CxFolderMetadata) m_Folders[id.ToUpper()];
      if (folder != null)
        return folder;
      else
        throw new ExMetadataException(string.Format("Image folder with ID=\"{0}\" not defined", id));
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Images dictionary.
    /// </summary>
    public Hashtable Images
    {
      get { return m_Images; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Image folders dictionary.
    /// </summary>
    public Hashtable Folders
    {
      get { return m_Folders; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns default name for the metadata XML file.
    /// </summary>
    override protected string XmlFileName
    { get { return "Images.xml"; } }
    //-------------------------------------------------------------------------
  }
}