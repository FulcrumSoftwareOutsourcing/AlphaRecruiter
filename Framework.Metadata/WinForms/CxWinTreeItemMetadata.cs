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
using System.Xml;
using System.Collections.Generic;

using Framework.Utils;

namespace Framework.Metadata
{
  public class CxWinTreeItemMetadata : CxMetadataObject
  {
    //-------------------------------------------------------------------------
    protected CxWinSectionMetadata m_Section;
    protected CxWinTreeItemsMetadata m_Items;
    private CxWinTreeItemMetadata m_ProviderItem;
    protected object m_Tag;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="section">parent section</param>
    /// <param name="id">tree item id</param>
    public CxWinTreeItemMetadata(
      CxMetadataHolder holder,
      CxWinSectionMetadata section,
      string id)
      : base(holder)
    {
      Id = id;
      m_Section = section;
      m_Section.RegisterTreeItem(this);
      m_Items = new CxWinTreeItemsMetadata(Holder, Section);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="section">parent section</param>
    /// <param name="element">XML element to load data from</param>
    public CxWinTreeItemMetadata(
      CxMetadataHolder holder,
      CxWinSectionMetadata section,
      XmlElement element)
      : base(holder, element)
    {
      m_Section = section;
      m_Section.RegisterTreeItem(this);
      m_Items = new CxWinTreeItemsMetadata(Holder, Section, element);
    }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Provider item.
    /// </summary>
    public CxWinTreeItemMetadata ProviderItem
    {
      get { return m_ProviderItem; }
      set { m_ProviderItem = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ID of the entity usage linked to the tree node.
    /// </summary>
    public string EntityUsageId
    {
      get
      {
        return this["entity_usage_id"];
      }
      set { this["entity_usage_id"] = value; }
    }
    //-------------------------------------------------------------------------
    protected CxEntityUsageMetadata m_EntityUsage_Cache;
    /// <summary>
    /// Corresponding entity usage metadata object.
    /// </summary>
    public CxEntityUsageMetadata EntityUsage
    {
      get
      {
        if (m_EntityUsage_Cache == null)
        {
          m_EntityUsage_Cache = CxUtils.NotEmpty(EntityUsageId) ? Holder.EntityUsages[EntityUsageId] : null;
        }
        return m_EntityUsage_Cache;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ID of the frame class to display for the content pane.
    /// </summary>
    public string FrameClassId
    {
      get
      {
        return this["frame_class_id"];
      }
      set { this["frame_class_id"] = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns metadata of the frame class to display for the content pane.
    /// </summary>
    public CxClassMetadata FrameClass
    {
      get
      {
        if (CxUtils.NotEmpty(FrameClassId))
        {
          return Holder.Classes[FrameClassId];
        }
        else if (EntityUsage != null)
        {
          return EntityUsage.FrameClass;
        }
        return null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ID of the image to display for the tree item.
    /// </summary>
    public string ImageId
    {
      get
      {
        return this["image_id"];
      }
      set { this["image_id"] = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns metadata of the the image to display for the tree item.
    /// </summary>
    public CxImageMetadata Image
    {
      get
      {
        if (CxUtils.NotEmpty(ImageId))
        {
          return Holder.Images[ImageId];
        }
        else if (EntityUsage != null)
        {
          return EntityUsage.Image;
        }
        return null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ID of the tree item provider class to get child or neighbour items.
    /// </summary>
    public string ItemProviderClassId
    {
      get { return this["item_provider_class_id"]; }
      set { this["item_provider_class_id"] = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns tree item provider class to get child or neighbour items.
    /// </summary>
    public CxClassMetadata ItemProviderClass
    {
      get
      {
        return CxUtils.NotEmpty(ItemProviderClassId) ? Holder.Classes[ItemProviderClassId] : null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Item provider constant parameters.
    /// </summary>
    public string ItemProviderParameters
    {
      get
      {
        return this["item_provider_parameters"];
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// If true, items returned by the item provider should replace this item.
    /// Otherwise, items returned by item provider are added as child items.
    /// </summary>
    public bool ItemProviderReplacement
    {
      get { return CxBool.Parse(this["item_provider_replacement"], false); }
      set { this["item_provider_replacement"] = value.ToString(); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True to make item initially expanded.
    /// </summary>
    public bool Expanded
    {
      get
      {
        return this["expanded"].ToLower() == "true";
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True to draw item with italic font.
    /// </summary>
    public bool Italic
    {
      get
      {
        return this["italic"].ToLower() == "true";
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True to set item unused mode. 
    /// </summary>
    public bool Unused
    {
      get { return CxBool.Parse(this["unused"], false); }
      set { this["unused"] = value.ToString(); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// For dynamic tree items created by item provider indicates that
    /// these dynamic items should be refreshed when some listed entity
    /// usage is changed. List is comma-separated.
    /// </summary>
    public string DependsOnEntityUsageIds
    {
      get { return this["depends_on_entity_usage_ids"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of items provided by the item provider class.
    /// </summary>
    public IList<CxWinTreeItemMetadata> GetItemProviderItems()
    {
      if (ItemProviderClass != null)
      {
        Type providerType = ItemProviderClass.Class;
        IxWinTreeItemsProvider provider = (IxWinTreeItemsProvider) CxType.CreateInstance(providerType);
        IList<CxWinTreeItemMetadata> items = provider.GetItems(this);
        if (items != null)
          SetProviderItemIfNull(items, this);
        return items;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets tree item provider class to get child or neighbour items.
    /// </summary>
    /// <param name="items">tree items</param>
    /// <param name="provider">tree item provider class to get child or neighbour items</param>
    private void SetProviderItemIfNull(IList<CxWinTreeItemMetadata> items, CxWinTreeItemMetadata provider)
    {
      for (int i = 0; i < items.Count; i++)
      {
        if (items[i].ProviderItem == null)
          items[i].ProviderItem = provider;
        SetProviderItemIfNull(items[i].Items.Items, provider);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Child tree items.
    /// </summary>
    public CxWinTreeItemsMetadata Items
    { get { return m_Items; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns parent portal.
    /// </summary>
    public CxWinSectionMetadata Section
    { get { return m_Section; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets custom tag of the tree item metadata.
    /// </summary>
    public object Tag
    {
      get { return m_Tag; }
      set { m_Tag = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Checks tree item access permission depending on security settings.
    /// </summary>
    public bool GetIsAllowed()
    {
      if (Holder != null && Holder.Security != null)
      {
        return Holder.Security.GetRight(this, EntityUsage);
      }
      return true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the item is provided by the item provider of the given type or its descendant.
    /// </summary>
    public bool GetIsProvidedBy(Type itemProviderType)
    {
      if (ProviderItem != null)
      {
        if (ProviderItem.ItemProviderClass.IsInheritedFrom(itemProviderType))
          return true;
        return ProviderItem.GetIsProvidedBy(itemProviderType);
      }
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if metadata object is visible
    /// </summary>
    protected override bool GetIsVisible()
    {
      bool isVisible = true;
      if (EntityUsage != null)
        isVisible = EntityUsage.Visible && EntityUsage.GetIsAccessGranted();

      // Just hide the item if it has some child items but no one of them is visible.
      if (isVisible && Items != null && Items.Items != null && Items.Items.Count > 0)
      {
        bool subItemsVisible = false;
        foreach (CxWinTreeItemMetadata item in Items.Items)
        {
          if (item.Visible)
          {
            subItemsVisible = true;
            break;
          }
        }
        isVisible = subItemsVisible;
      }
      return isVisible && base.GetIsVisible();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata object override.
    /// </summary>
    /// <param name="element">XML element to load overridden properties from</param>
    public override void LoadOverride(XmlElement element)
    {
      base.LoadOverride(element);
      
      XmlNodeList xmlNodes = element.SelectNodes("tree_item");
      if (xmlNodes != null)
      {
        foreach (XmlElement itemElement in xmlNodes)
        {
          CxWinTreeItemMetadata treeItem = Section.Find(CxXml.GetAttr(itemElement, "id"));
          if (treeItem != null)
          {
            treeItem.LoadOverride(itemElement);
          }
          else
          {
            treeItem = new CxWinTreeItemMetadata(Holder, Section, itemElement);
            Items.Add(treeItem);
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns object type code for localization.
    /// </summary>
    override public string LocalizationObjectTypeCode
    {
      get
      {
        return "Metadata.WinTreeItem";
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// User-friendly metadata object caption.
    /// </summary>
    public override string Text
    {
      get
      {
        string text = base.Text;
        if (!string.IsNullOrEmpty(text))
        {
          CxEntityUsageMetadata entityUsage = EntityUsage;
          if (entityUsage != null)
            text = entityUsage.ReplacePlaceholders(text);
        }
        return text;
      }
      set { base.Text = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns unique object name for localization.
    /// </summary>
    override public string LocalizationObjectName
    {
      get
      {
        return Section != null ? Section.LocalizationObjectName + "." + Id : base.LocalizationObjectName;
      }
    }
    //----------------------------------------------------------------------------
  }
}