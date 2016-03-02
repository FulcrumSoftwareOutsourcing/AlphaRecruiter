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
  public class CxSlTreeItemsMetadata : CxMetadataCollection
  {
    //-------------------------------------------------------------------------
    protected List<CxSlTreeItemMetadata> m_Items = new List<CxSlTreeItemMetadata>();
    protected CxSlSectionMetadata m_Section = null;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">metadata holder</param>
    /// <param name="section">parent navigation section</param>
    public CxSlTreeItemsMetadata(
      CxMetadataHolder holder,
      CxSlSectionMetadata section) : base(holder)
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
    public CxSlTreeItemsMetadata(
      CxMetadataHolder holder, 
      CxSlSectionMetadata section,
      XmlElement element) : this(holder, section)
		{
      if (element != null)
      {
        XmlNodeList treeItems = element.SelectNodes("tree_item");
        if (treeItems == null)
          throw new ExNullReferenceException("treeItems");

        foreach (XmlElement itemElement in treeItems)
        {
          CxSlTreeItemMetadata treeItem = new CxSlTreeItemMetadata(Holder, Section, itemElement);
          Add(treeItem);
        }
      }
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds item to the collection.
    /// </summary>
    /// <param name="treeItem">item to add</param>
    public void Add(CxSlTreeItemMetadata treeItem)
    {
      m_Items.Add(treeItem);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Deletes dynamic items created by item provider with the specified item replacement parameter.
    /// </summary>
    protected void DeleteDynamicItems(
      bool itemProviderReplacement,
      Type itemProviderType,
      CxEntityUsageMetadata entityUsage)
    {
      for (int i = m_Items.Count - 1; i >= 0; i--)
      {
        CxSlTreeItemMetadata item = m_Items[i];
        if (item.ProviderItem != null && 
            item.ProviderItem.ItemProviderReplacement == itemProviderReplacement &&
            (itemProviderType == null || item.ProviderItem.ItemProviderClass.IsInheritedFrom(itemProviderType)) &&
            (entityUsage == null || IsProviderItemEntityUsageDependent(item.ProviderItem, entityUsage)))
        {
          m_Section.UnregisterTreeItem(item);
          m_Items.RemoveAt(i);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if entity usage of tree item is dependent on the given entity usage.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="entityUsage"></param>
    /// <returns></returns>
    protected bool IsProviderItemEntityUsageDependent(
      CxSlTreeItemMetadata item,
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
    /// <summary>
    /// Refreshes items created by tree item provider classes.
    /// </summary>
    public void RefreshDynamicItems(
      Type itemProviderType,
      CxEntityUsageMetadata entityUsage)
    {
      DeleteDynamicItems(true, itemProviderType, entityUsage);
      CxSlTreeItemMetadata[] items = new CxSlTreeItemMetadata[m_Items.Count];
      m_Items.CopyTo(items);
      foreach (CxSlTreeItemMetadata item in items)
      {
        if (item.ItemProviderClass != null &&
            (itemProviderType == null || item.ItemProviderClass.IsInheritedFrom(itemProviderType)) &&
            (entityUsage == null || IsProviderItemEntityUsageDependent(item, entityUsage)))
        {
          if (item.ItemProviderReplacement)
          {
            item["visible"] = "false";
            IList<CxSlTreeItemMetadata> providerItems = item.GetItemProviderItems();
            if (providerItems != null)
            {
              int index = m_Items.IndexOf(item) + 1;
              for (int i = providerItems.Count - 1; i >= 0; i--)
              {
                m_Items.Insert(index, providerItems[i]);
              }
            }
          }
          else
          {
            item.Items.DeleteDynamicItems(false, itemProviderType, entityUsage);
            IList<CxSlTreeItemMetadata> providerItems = item.GetItemProviderItems();
            if (providerItems != null)
            {
              foreach (CxSlTreeItemMetadata providerItem in providerItems)
              {
                item.Items.Items.Add(providerItem);
              }
            }
          }
        }
        item.Items.RefreshDynamicItems(itemProviderType, entityUsage);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds tree item by the entity usage metadata.
    /// </summary>
    public CxSlTreeItemMetadata FindByEntityUsage(CxEntityUsageMetadata entityUsage)
    {
      foreach (CxSlTreeItemMetadata item in m_Items)
      {
        if (item.EntityUsage == entityUsage)
        {
          return item;
        }
        CxSlTreeItemMetadata foundItem = item.Items.FindByEntityUsage(entityUsage);
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
    public CxSlTreeItemMetadata this[int index]
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
    public CxSlSectionMetadata Section
    { get {return m_Section;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of all tree items.
    /// </summary>
    public IList<CxSlTreeItemMetadata> Items
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
