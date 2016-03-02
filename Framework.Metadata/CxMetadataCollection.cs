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

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Framework.Utils;

namespace Framework.Metadata
{
	/// <summary>
	/// Base class for collection of metadata items.
	/// </summary>
	public class CxMetadataCollection
	{
    //-------------------------------------------------------------------------
    protected CxMetadataHolder m_Holder = null;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
		public CxMetadataCollection(CxMetadataHolder holder)
		{
      m_Holder = holder;
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="doc">document to load data from</param>
    public CxMetadataCollection(CxMetadataHolder holder, XmlDocument doc) : this(holder)
    {
      Load(doc);
      LoadIncludes(doc);
      LoadPlugins();
      DoAfterLoad();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads all the given documents, but doesn't manage includes.
    /// </summary>
    /// <param name="holder">parent metadata hodler object</param>
    /// <param name="docs">documents to load data from</param>
    public CxMetadataCollection(CxMetadataHolder holder, IEnumerable<XmlDocument> docs)
      : this(holder)
    {
      foreach (XmlDocument doc in docs)
      {
        Load(doc);
      }
      LoadPlugins();
      DoAfterLoad();
    }
	  //-------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata collection from the XML document.
    /// </summary>
    /// <param name="doc">document to load data from</param>
    virtual protected void Load(XmlDocument doc)
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Does actions after metadata loaded.
    /// </summary>
    virtual protected void DoAfterLoad()
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata from INCLUDE XML nodes.
    /// </summary>
    /// <param name="doc">XML document with INCLUDE nodes</param>
    protected void LoadIncludes(XmlDocument doc)
    {
      foreach (XmlElement element in doc.DocumentElement.SelectNodes("include"))
      {
        string fileName = CxXml.GetAttr(element, "file");
        XmlDocument includeDoc = Holder.LoadMetadata(fileName);
        Load(includeDoc);
        LoadIncludes(includeDoc);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads plugin metadata.
    /// </summary>
    protected void LoadPlugins()
    {
      if (Holder.Config != null)
      {
        string fileName;
        if (Holder.Config.PluginsHaveMetadataProjectFile)
          fileName = "MetadataProject.xml";
        else
          fileName = XmlFileName;

        if (CxUtils.NotEmpty(fileName))
        {
          foreach (KeyValuePair<string, Assembly> pair in Holder.Config.Plugins)
          {
            XmlDocument pluginDoc = Holder.LoadResourceFile(pair.Value, pair.Key, fileName);
            if (pluginDoc != null)
            {
              Load(pluginDoc);
              LoadPluginIncludes(pluginDoc, pair.Value, pair.Key);
            }
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata from INCLUDE XML nodes of the metadata plugin.
    /// </summary>
    /// <param name="doc">plugin XML document with INCLUDE nodes</param>
    /// <param name="assembly">plugin assembly</param>
    /// <param name="nameSpace">plugin namespace</param>
    protected void LoadPluginIncludes(XmlDocument doc, Assembly assembly, string nameSpace)
    {
      foreach (XmlElement element in doc.DocumentElement.SelectNodes("include"))
      {
        string fileName = CxXml.GetAttr(element, "file");
        XmlDocument includeDoc = Holder.LoadResourceFile(assembly, nameSpace, fileName);
        if (includeDoc == null)
        {
          throw new ExException(
            string.Format("Could not find metadata file: <{0}> in <{1}> namespace, <{2}> assembly", fileName, nameSpace, assembly));
        }
        Load(includeDoc);
        LoadPluginIncludes(includeDoc, assembly, nameSpace);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates and returns metadata XML element with the given tag and ID.
    /// </summary>
    public static XmlElement CreateXmlElementBase(string tagName, string id)
    {
      XmlDocument doc = new XmlDocument();
      XmlElement element = doc.CreateElement(tagName);
      doc.AppendChild(element);
      element.SetAttribute("id", id);
      return element;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates and returns metadata XML element with the given tag and ID.
    /// </summary>
    virtual public XmlElement CreateXmlElement(string tagName, string id)
    {
      return CreateXmlElementBase(tagName, id);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata overrides from the given XML document.
    /// </summary>
    /// <param name="doc">document to load overrides from</param>
    /// <param name="tagName">metadata override tag</param>
    /// <param name="metadataMap">loaded metadata dictionary</param>
    protected void LoadOverrides(XmlDocument doc, string tagName, IDictionary metadataMap)
    {
      foreach (XmlElement element in doc.DocumentElement.SelectNodes(tagName))
      {
        string id = CxXml.GetAttr(element, "id");
        if (CxUtils.NotEmpty(id))
        {
          CxMetadataObject metadataObject = (CxMetadataObject) metadataMap[id.ToUpper()];
          if (metadataObject != null)
          {
            LoadOverride(metadataObject, element);
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata object override.
    /// </summary>
    /// <param name="metadataObject">object to override</param>
    /// <param name="element">XML element to override from</param>
    virtual protected void LoadOverride(CxMetadataObject metadataObject, XmlElement element)
    {
      metadataObject.LoadOverride(element);
    }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Parent metadata holder object.
    /// </summary>
    public CxMetadataHolder Holder
    { get {return m_Holder;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns default name for the metadata XML file.
    /// </summary>
    virtual protected string XmlFileName
    { get { return null; } }
    //-------------------------------------------------------------------------
  }
}