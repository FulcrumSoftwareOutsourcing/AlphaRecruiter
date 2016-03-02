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

using System.Xml;
using System.Collections.Generic;

using Framework.Utils;

namespace Framework.Metadata
{
	/// <summary>
	/// Child collection of web parts that belongs to the particular page tab.
	/// </summary>
	public class CxWebPartsCollection
	{
    //-------------------------------------------------------------------------
    protected CxTabMetadata m_Tab = null;
    protected List<CxWebPartMetadata> m_WebPartList = new List<CxWebPartMetadata>();
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tab">web part container tab</param>
		public CxWebPartsCollection(CxTabMetadata tab)
		{
      m_Tab = tab;
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tab">web part container tab</param>
    /// <param name="element">XML element to load web parts from</param>
    public CxWebPartsCollection(CxTabMetadata tab, XmlElement element) : this(tab)
    {
      foreach (XmlElement partElement in element.SelectNodes("web_part"))
      {
        CxWebPartMetadata webPart = CreateWebPart(tab, partElement);
        Add(webPart);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates and returns web part metadata instance.
    /// </summary>
    /// <param name="tab">tab of the web part</param>
    /// <param name="element">XML element to create web part from</param>
    public CxWebPartMetadata CreateWebPart(CxTabMetadata tab, XmlElement element)
    {
      CxWebPartMetadata webPart = new CxWebPartMetadata(tab.Holder, element, tab);
      webPart.CopyPropertiesFrom(tab.Holder.WebParts[webPart.Id]);
      return webPart;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds web part to the collection.
    /// </summary>
    /// <param name="webPart">web part to add</param>
    public void Add(CxWebPartMetadata webPart)
    {
      m_WebPartList.Add(webPart);
    }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns web part by its index on the tab.
    /// </summary>
    public CxWebPartMetadata this[int index]
    {
      get
      {
        return m_WebPartList[index];
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns web part count.
    /// </summary>
    public int Count
    {
      get
      {
        return m_WebPartList.Count;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns index of the web part in the collection.
    /// </summary>
    /// <param name="webPart">web part to get index of</param>
    public int IndexOf(CxWebPartMetadata webPart)
    {
      return m_WebPartList.IndexOf(webPart);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds WebPart metadata by ID.
    /// </summary>
    /// <param name="webPartId"></param>
    /// <returns></returns>
    public CxWebPartMetadata FindById(string webPartId)
    {
      if (CxUtils.NotEmpty(webPartId))
      {
        foreach (CxWebPartMetadata wp in m_WebPartList)
        {
          if (wp.Id.ToUpper() == webPartId.ToUpper())
          {
            return wp;
          }
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Retruns list of web parts.
    /// </summary>
    public IList<CxWebPartMetadata> Items
    { get { return m_WebPartList; } }
    //-------------------------------------------------------------------------
  }
}