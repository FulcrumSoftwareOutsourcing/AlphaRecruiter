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

using System.Collections.Generic;
using System.Xml;
using System.Collections;

namespace Framework.Metadata
{
	/// <summary>
	/// Collection of all registered web parts.
	/// </summary>
	public class CxWebPartsMetadata : CxMetadataCollection
	{
    //-------------------------------------------------------------------------
    protected List<CxWebPartMetadata> m_WebPartList = new List<CxWebPartMetadata>();
    protected Hashtable m_WebPartMap = new Hashtable();
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="doc">XML document to load metadata from</param>
    public CxWebPartsMetadata(CxMetadataHolder holder, XmlDocument doc)
      : 
      base(holder, doc)
		{
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="docs">XML documents to load metadata from</param>
    public CxWebPartsMetadata(CxMetadataHolder holder, IEnumerable<XmlDocument> docs)
      : base(holder, docs)
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata collection from the XML document.
    /// </summary>
    /// <param name="doc">document to load data from</param>
    override protected void Load(XmlDocument doc)
    {
      base.Load(doc);
      foreach (XmlElement element in doc.DocumentElement.SelectNodes("web_part"))
      {
        CxWebPartMetadata webPart = new CxWebPartMetadata(Holder, element);
        m_WebPartMap.Add(webPart.Id, webPart);
        m_WebPartList.Add(webPart);
      }
      LoadOverrides(doc, "web_part_override", m_WebPartMap);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// WebPart with the given ID.
    /// </summary>
    public CxWebPartMetadata this[string id]
    {
      get
      { 
        CxWebPartMetadata webPart = (CxWebPartMetadata) m_WebPartMap[id.ToUpper()];
        if (webPart != null)
          return webPart;
        else
          throw new ExMetadataException(string.Format("WebPart with ID=\"{0}\" not defined", id));
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns list of all available web parts.
    /// </summary>
    public IList<CxWebPartMetadata> Items
    { get {return m_WebPartList;} }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns default name for the metadata XML file.
    /// </summary>
    override protected string XmlFileName
    { get { return "WebParts.xml"; } }
    //-------------------------------------------------------------------------
  }
}