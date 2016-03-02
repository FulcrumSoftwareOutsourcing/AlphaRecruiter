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
using System.Collections.Generic;
using System.Xml;

using Framework.Utils;

namespace Framework.Metadata
{
  public class CxWinTreeItemsMetadata : CxMetadataCollection
  {
    //-------------------------------------------------------------------------
    protected List<CxWinTreeItemMetadata> m_Items = new List<CxWinTreeItemMetadata>();
    protected CxWinSectionMetadata m_Section = null;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">metadata holder</param>
    /// <param name="section">parent navigation section</param>
    public CxWinTreeItemsMetadata(
      CxMetadataHolder holder,
      CxWinSectionMetadata section) : base(holder)
    {
      m_Section = section;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">metadata holder</param>
    /// <param name="section">parent navigation section</param>
    /// <param name="element">XML element to load metadata from</param>
		public CxWinTreeItemsMetadata(
      CxMetadataHolder holder, 
      CxWinSectionMetadata section,
      XmlElement element) : this(holder, section)
		{
      if (element != null)
      {
        XmlNodeList treeItems = element.SelectNodes("tree_item");
        if (treeItems == null)
          throw new ExNullReferenceException("treeItems");

        foreach (XmlElement itemElement in treeItems)
        {
          CxWinTreeItemMetadata treeItem = new CxWinTreeItemMetadata(Holder, Section, itemElement);
          Add(treeItem);
        }
      }
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds item to the collection.
    /// </summary>
    /// <param name="treeItem">item to add</param>
    public void Add(CxWinTreeItemMetadata treeItem)
    {
      m_Items.Add(treeItem);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Deletes dynamic items created by item provider with the specified item replacement parameter.
    /// </summary>
    protected void DeleteDynamicItems(
      Type itemProviderType,
      CxEntityUsageMetadata entityUsage)
    {
      for (int i = m_Items.Count - 1; i >= 0; i--)
      {
        CxWinTreeItemMetadata item = m_Items[i];
        if (item.ProviderItem != null &&
           (itemProviderType == null || item.GetIsProvidedBy(itemProviderType)) &&
           (entityUsage == null || IsProviderItemEntityUsageDependent(item.ProviderItem, entityUsage)))
        {
          RemoveTreeItem(item);
        }
        else
        {
          item.Items.DeleteDynamicItems(itemProviderType, entityUsage);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Performs the correct removal of the given tree item from the collection.
    /// </summary>
    /// <param name="treeItem">tree item to remove</param>
    protected void RemoveTreeItem(CxWinTreeItemMetadata treeItem)
    {
      for (int i = treeItem.Items.Count - 1; i >= 0; i--)
      {
        treeItem.Items.RemoveTreeItem(treeItem.Items[i]);
      }
      m_Section.UnregisterTreeItem(treeItem);
      m_Items.Remove(treeItem);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if entity usage of tree item is dependent on the given entity usage.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="entityUsage"></param>
    /// <returns></returns>
    protected bool IsProviderItemEntityUsageDependent(
      CxWinTreeItemMetadata item,
      CxEntityUsageMetadata entityUsage)
    {
      if (item != null && item.EntityUsage != null && entityUsage != null)
      {
        if (item.EntityUsage.IsAncestorOrDescendant(entityUsage))
        {
          return true;
        }
        IList<string> entityUsageIds = CxText.DecomposeWithSeparator(item.DependsOnEntityUsageIds, ",");
        foreach (string entityUsageId in entityUsageIds)
        {
          CxEntityUsageMetadata eu = Holder.EntityUsages.Find(entityUsageId);
          if (eu != null && eu.Id == entityUsage.Id)
          {
            return true;
          }
        }
      }
      return false;
    }
    //-------------------------------------------------------------------------
    protected void CreateDynamicItems(
      Type itemProviderType,
      CxEntityUsageMetadata entityUsage)
    {
      CxWinTreeItemMetadata[] items = m_Items.ToArray();
      foreach (CxWinTreeItemMetadata item in items)
      {
        if (item.ItemProviderClass != null &&
            (itemProviderType == null || item.ItemProviderClass.IsInheritedFrom(itemProviderType)) &&
            (entityUsage == null || IsProviderItemEntityUsageDependent(item, entityUsage)))
        {
          IList<CxWinTreeItemMetadata> providerItems = item.GetItemProviderItems();
          if (providerItems != null)
          {
            foreach (CxWinTreeItemMetadata providerItem in providerItems)
            {
              item.Items.Add(providerItem);
            }
          }
        }
        else
        {
          item.Items.CreateDynamicItems(itemProviderType, entityUsage);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Refreshes items created by tree item provider classes.
    /// </summary>
    public void RefreshDynamicItems(
      Type itemProviderType,
      CxEntityUsageMetadata entityUsage)
    {
      // Here we clean up the collection from any dynamic items.
      DeleteDynamicItems(itemProviderType, entityUsage);

      // Then, we just regenerate the dynamic items.
      CreateDynamicItems(itemProviderType, entityUsage);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds tree item by the entity usage metadata.
    /// </summary>
    public CxWinTreeItemMetadata FindByEntityUsage(CxEntityUsageMetadata entityUsage)
    {
      foreach (CxWinTreeItemMetadata item in m_Items)
      {
        if (item.EntityUsage == entityUsage)
        {
          return item;
        }
        CxWinTreeItemMetadata foundItem = item.Items.FindByEntityUsage(entityUsage);
        if (foundItem != null)
        {
          return foundItem;
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns tree item metadata for the given index.
    /// </summary>
    public CxWinTreeItemMetadata this[int index]
    {
      get
      {
        return m_Items[index];
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns parent portal.
    /// </summary>
    public CxWinSectionMetadata Section
    { get {return m_Section;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of all tree items.
    /// </summary>
    public IList<CxWinTreeItemMetadata> Items
    { get { return m_Items; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns count of child items.
    /// </summary>
    public int Count
    { get { return m_Items.Count; } }
    //-------------------------------------------------------------------------
  }
}