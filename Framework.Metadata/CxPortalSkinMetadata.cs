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

namespace Framework.Metadata
{
	/// <summary>
	/// Metadata for portal web skin.
	/// </summary>
	public class CxPortalSkinMetadata : CxMetadataObject
	{
    //-------------------------------------------------------------------------
    protected string m_StyleSheetsFolder = "";
    protected string[] m_StyleSheets = new string[0];
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder"></param>
    /// <param name="element"></param>
		public CxPortalSkinMetadata(CxMetadataHolder holder, XmlElement element) :
      base(holder, element)
		{
      LoadStyleSheets(element);
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="sourceObject">object to take propeties from</param>
    public CxPortalSkinMetadata(CxPortalSkinMetadata sourceObject) :
      base(sourceObject.Holder)
    {
      Id = sourceObject.Id;
      CopyPropertiesFrom(sourceObject);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads style sheets list from the portal skin XML element.
    /// </summary>
    /// <param name="element">portal skin XML element</param>
    protected void LoadStyleSheets(XmlElement element)
    {
      ArrayList list = new ArrayList();
      foreach (XmlElement sheetsElement in element.SelectNodes("stylesheets"))
      {
        if (CxUtils.IsEmpty(m_StyleSheetsFolder))
        {
          m_StyleSheetsFolder = CxXml.GetAttr(sheetsElement, "folder");
        }
        string folder = CxXml.GetAttr(sheetsElement, "folder").TrimEnd('/', '\\');
        foreach (XmlElement sheetElement in sheetsElement.SelectNodes("stylesheet"))
        {
          string name = CxXml.GetAttr(sheetElement, "name").TrimStart('/', '\\');
          if (CxUtils.NotEmpty(name))
          {
            list.Add(CxUtils.NotEmpty(folder) ? folder + "/" + name : name);
          }
        }
      }
      m_StyleSheets = new string[list.Count];
      list.CopyTo(m_StyleSheets);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Method is called after properties copying.
    /// </summary>
    /// <param name="sourceObj">object properties were taken from</param>
    override protected void DoAfterCopyProperties(CxMetadataObject sourceObj)
    {
      base.DoAfterCopyProperties(sourceObj);
      m_StyleSheetsFolder = ((CxPortalSkinMetadata)sourceObj).StyleSheetsFolder;
      m_StyleSheets = new string[((CxPortalSkinMetadata)sourceObj).StyleSheets.Length];
      ((CxPortalSkinMetadata)sourceObj).StyleSheets.CopyTo(m_StyleSheets, 0);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds template folder to template path, if necessary.
    /// </summary>
    /// <param name="template"></param>
    public string GetTemplateFullPath(string template)
    {
      if (CxUtils.IsEmpty(template))
      {
        return template;
      }
      string fullPath = CxUtils.Nvl(template);
      if (fullPath.StartsWith("~/"))
      {
        return fullPath.Substring(2);
      }
      if (CxUtils.IsEmpty(TemplateFolder))
      {
        return fullPath;
      }
      return TemplateFolder + "/" + fullPath;
    }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// True if current skin is default for all application portals.
    /// </summary>
    public bool IsDefault
    { get {return this["is_default"] == "true";} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// List of names (including path) of CSS style sheet files.
    /// </summary>
    public string[] StyleSheets
    { get {return m_StyleSheets;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Root path for all template user controls (ASCX).
    /// </summary>
    public string TemplateFolder
    { get {return this["template_folder"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The name (and path) of portal main page template user control (ASCX).
    /// </summary>
    public string PortalTemplate
    { get {return GetTemplateFullPath(this["portal_template"]);} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The name (and path) of portal popup page template user control (ASCX).
    /// </summary>
    public string PopupTemplate
    { get {return GetTemplateFullPath(this["popup_template"]);} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The name (and path) of web part template user control (ASCX).
    /// </summary>
    public string WebPartTemplate
    { get {return GetTemplateFullPath(this["web_part_template"]);} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The name (and path) of portal login page template user control (ASCX).
    /// </summary>
    public string LoginTemplate
    { get {return GetTemplateFullPath(this["login_template"]);} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Folder that is default for skin images.
    /// </summary>
    public string ImagesFolder
    { get {return this["images_folder"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Folder that is default for style sheet CSS files.
    /// </summary>
    public string StyleSheetsFolder
    { get {return m_StyleSheetsFolder;} }
    //-------------------------------------------------------------------------
  }
}