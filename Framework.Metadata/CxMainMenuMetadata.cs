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
using System.Data;

using Framework.Utils;

namespace Framework.Metadata
{
	/// <summary>
  /// Class to read and hold information about application main menu.
  /// </summary>
	public class CxMainMenuMetadata : CxMetadataCollection
	{
    //----------------------------------------------------------------------------
    protected IList<CxMainMenuItemMetadata> m_Items = new List<CxMainMenuItemMetadata>(); // First-level menu items
    protected Dictionary<string, CxMainMenuItemMetadata> m_AllItems = new Dictionary<string, CxMainMenuItemMetadata>(); // All menu items (with IDs as keys)
    protected DataTable m_DataTable = null; // Data table for grid lookups
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">metadata holder</param>
    /// <param name="doc">XML doc to read metadata from</param>
    public CxMainMenuMetadata(CxMetadataHolder holder, XmlDocument doc) : 
      base(holder, doc)
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
      XmlElement menuItemsElement = (XmlElement) doc.DocumentElement.SelectSingleNode("menu_items");
      ReadMenuLevel(menuItemsElement, m_Items);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Reads menu items lying under the given node and adds them into the given list.
    /// Then tries to deep menu structure recursively.
    /// </summary>
    /// <param name="itemsElement">"menu_items" element menu items are lying under</param>
    /// <param name="items">list to all created menu items</param>
    protected void ReadMenuLevel(XmlElement itemsElement, IList<CxMainMenuItemMetadata> items)
    {
      foreach (XmlElement element in itemsElement.SelectNodes("menu_item"))
      {
        CxMainMenuItemMetadata item = new CxMainMenuItemMetadata(Holder, element);
        items.Add(item);
        m_AllItems.Add(item.Id, item);
        XmlElement subItemsElement = (XmlElement) element.SelectSingleNode("menu_items");
        if (subItemsElement != null)
        {
          ReadMenuLevel(subItemsElement, item.Items);
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// First-level menu items.
    /// </summary>
    public IList<CxMainMenuItemMetadata> Items
    {
      get { return m_Items; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Menu item with the given ID.
    /// </summary>
    public CxMainMenuItemMetadata this[string id]
    {
      get
      { 
        if (m_AllItems.ContainsKey(id.ToUpper()))
          return m_AllItems[id.ToUpper()];
        else
          throw new ExMetadataException(string.Format("Main menu item with ID=\"{0}\" not defined", id));
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Finds menu item that represents the given entity usage.
    /// </summary>
    /// <param name="entityUsageId">ID of the entity usage to find</param>
    /// <returns>menu item that represents the given entity usage or null if not found</returns>
    public CxMainMenuItemMetadata FindItemForEntityUsage(string entityUsageId)
    {
      CxEntityUsageMetadata entityUsage = Holder.EntityUsages[entityUsageId];
      foreach (string key in m_AllItems.Keys)
      {
        CxMainMenuItemMetadata menuItem = m_AllItems[key];
        if (CxUtils.NotEmpty(menuItem.EntityUsageId) &&
            (menuItem.EntityUsageId == entityUsageId || menuItem.EntityUsageId == entityUsage.MainMenuEntityUsageId))
        {
          return menuItem;
        }
      }
      return null;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns data table for grid lookups.
    /// </summary>
    /// <returns>data table for grid lookups</returns>
    public DataTable GetDataTable()
    {
      if (m_DataTable == null)
      {
        m_DataTable = new DataTable();
        m_DataTable.Columns.Add("ID", typeof(string));
        m_DataTable.Columns.Add("Name", typeof(string));
        foreach (string id in m_AllItems.Keys)
        {
          CxMainMenuItemMetadata menuItem = m_AllItems[id];
          m_DataTable.Rows.Add(new object[] {id, menuItem.Caption});
        }
      }
      return m_DataTable;
    }
    //----------------------------------------------------------------------------
  }
}
