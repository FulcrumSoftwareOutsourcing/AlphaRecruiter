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

namespace Framework.Metadata
{
	/// <summary>
	/// Navigation tree items collection
	/// </summary>
	public class CxTreeItemsMetadata : CxMetadataCollection
	{
    //-------------------------------------------------------------------------
    protected List<CxTreeItemMetadata> m_Items = new List<CxTreeItemMetadata>();
    protected CxPortalMetadata m_Portal = null;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
		public CxTreeItemsMetadata(
      CxMetadataHolder holder, 
      CxPortalMetadata portal,
      XmlElement element) : base(holder)
		{
      m_Portal = portal;
      if (element != null)
      {
        foreach (XmlElement itemElement in element.SelectNodes("tree_item"))
        {
          CxTreeItemMetadata treeItem = new CxTreeItemMetadata(Holder, Portal, itemElement);
          Add(treeItem);
        }
      }
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds tree item to the collection.
    /// </summary>
    /// <param name="treeItem">tree item to add</param>
    public void Add(CxTreeItemMetadata treeItem)
    {
      m_Items.Add(treeItem);
    }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns tree item metadata for the given index.
    /// </summary>
    public CxTreeItemMetadata this[int index]
    {
      get
      {
        return m_Items[index];
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns count of tree items.
    /// </summary>
    public int Count 
    {
      get
      {
        return m_Items.Count;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns parent portal.
    /// </summary>
    public CxPortalMetadata Portal
    { get {return m_Portal;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of all tree items.
    /// </summary>
    public IList<CxTreeItemMetadata> Items
    { get { return m_Items; } }
    //-------------------------------------------------------------------------
  }
}