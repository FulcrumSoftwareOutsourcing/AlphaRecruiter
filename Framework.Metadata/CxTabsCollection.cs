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
using System.Xml;

using Framework.Utils;

namespace Framework.Metadata
{
	/// <summary>
	/// Child collection of tabs for portal page.
	/// </summary>
	public class CxTabsCollection
	{
    //-------------------------------------------------------------------------
    protected CxPageMetadata m_Page = null;
    protected List<CxTabMetadata> m_TabList = new List<CxTabMetadata>();
    protected Hashtable m_TabMap = new Hashtable();
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="page">parent portal page</param>
		public CxTabsCollection(CxPageMetadata page)
		{
      m_Page = page;
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="page">parent portal page</param>
    /// <param name="element">XML element to load tabs from</param>
    public CxTabsCollection(CxPageMetadata page, XmlElement element) : this(page)
    {
      AddFrom(element);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds tab to tabs collection.
    /// </summary>
    /// <param name="tab">tab object to add</param>
    public void Add(CxTabMetadata tab)
    {
      m_TabList.Add(tab);
      m_TabMap.Add(tab.Id, tab);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Appends tab collection with items loaded from the given XML element.
    /// </summary>
    /// <param name="element">XML element to load tabs from</param>
    public void AddFrom(XmlElement element)
    {
      foreach (XmlElement tabElement in element.SelectNodes("tab"))
      {
        CxTabMetadata tab = new CxTabMetadata(Page.Holder, tabElement, Page);
        Add(tab);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns tab with the specified ID.
    /// </summary>
    /// <param name="id">ID of the tab</param>
    /// <returns>found tab or null</returns>
    public CxTabMetadata Find(string id)
    {
      return CxUtils.NotEmpty(id) ? (CxTabMetadata) m_TabMap[id.ToUpper()] : null;
    }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns tab object by its index.
    /// </summary>
    public CxTabMetadata this[int index]
    {
      get
      {
        return m_TabList[index];
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns count of tabs in the collection.
    /// </summary>
    public int Count
    {
      get
      {
        return m_TabList.Count;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns parent tab page.
    /// </summary>
    public CxPageMetadata Page
    { get {return m_Page;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Retruns list of tabs.
    /// </summary>
    public IList<CxTabMetadata> Items
    { get { return m_TabList; } }
    //-------------------------------------------------------------------------
  }
}